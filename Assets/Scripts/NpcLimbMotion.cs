using System.Collections.Generic;
using UnityEngine;

public class NpcLimbMotion : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent;
    public NpcWalkMotion walkMotion;

    [Header("Bone names")]
    public string leftThigh = "LeftUpLeg";
    public string leftCalf = "LeftLeg";
    public string leftFoot = "LeftFoot";
    public string rightThigh = "RightUpLeg";
    public string rightCalf = "RightLeg";
    public string rightFoot = "RightFoot";
    public string leftUpperArm = "LeftArm";
    public string leftForearm = "LeftForeArm";
    public string leftHand = "LeftHand";
    public string rightUpperArm = "RightArm";
    public string rightForearm = "RightForeArm";
    public string rightHand = "RightHand";

    [Header("Arm rest pose (T-pose correction)")]
    [Range(0f, 90f)]
    public float armDownAngle = 75f;
    public float forearmRestBend = 8f;

    [Header("Walk motion amplitudes (degrees)")]
    public float thighSwing = 22f;
    public float calfBend = 15f;
    public float footRock = 6f;
    public float armSwing = 18f;
    public float forearmCurl = 10f;
    public float handRock = 4f;

    [Header("Tuning")]
    public float phaseSpeedMultiplier = 1f;
    public float settleSpeed = 8f;

    // Bone group structure: character prefabs in this pack contain multiple duplicate rigs
    // (e.g. body mesh + clothing meshes each have their own skeleton with identical bone names).
    // All matching bones must be tracked and animated together.
    struct BoneGroup
    {
        public Transform[] bones;
        public Quaternion[] restRot;
        public Vector3[] swingAxis;
    }

    BoneGroup groupLTh, groupLCa, groupLFo;
    BoneGroup groupRTh, groupRCa, groupRFo;
    BoneGroup groupLUa, groupLFa, groupLHa;
    BoneGroup groupRUa, groupRFa, groupRHa;

    float phase;
    bool initialized;

    void Start()
    {
        if (agent == null) agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (walkMotion == null) walkMotion = GetComponent<NpcWalkMotion>();

        // Disable all animators in hierarchy to prevent them from resetting bones to T-pose
        var anims = GetComponentsInChildren<Animator>(true);
        foreach (var a in anims)
        {
            if (a != null) a.enabled = false;
        }

        // Collect all transforms across the entire hierarchy
        var allTransforms = GetComponentsInChildren<Transform>(true);

        // Initialize bone groups (supporting duplicate bones across body & clothing rigs)
        groupLTh = CreateBoneGroup(allTransforms, leftThigh, "LeftUpLeg");
        groupLCa = CreateBoneGroup(allTransforms, leftCalf, "LeftLeg");
        groupLFo = CreateBoneGroup(allTransforms, leftFoot, "LeftFoot");

        groupRTh = CreateBoneGroup(allTransforms, rightThigh, "RightUpLeg");
        groupRCa = CreateBoneGroup(allTransforms, rightCalf, "RightLeg");
        groupRFo = CreateBoneGroup(allTransforms, rightFoot, "RightFoot");

        groupLUa = CreateBoneGroup(allTransforms, leftUpperArm, "LeftArm");
        groupLFa = CreateBoneGroup(allTransforms, leftForearm, "LeftForeArm");
        groupLHa = CreateBoneGroup(allTransforms, leftHand, "LeftHand");

        groupRUa = CreateBoneGroup(allTransforms, rightUpperArm, "RightArm");
        groupRFa = CreateBoneGroup(allTransforms, rightForearm, "RightForeArm");
        groupRHa = CreateBoneGroup(allTransforms, rightHand, "RightHand");

        // Rotate ALL upper arms down from T-pose (left: +armDownAngle, right: -armDownAngle around character forward)
        RotateGroupWorld(ref groupLUa, transform.forward, armDownAngle);
        RotateGroupWorld(ref groupRUa, transform.forward, -armDownAngle);

        // Slight natural forward bend for forearms
        if (forearmRestBend > 0.01f)
        {
            RotateGroupWorld(ref groupLFa, transform.right, -forearmRestBend);
            RotateGroupWorld(ref groupRFa, transform.right, -forearmRestBend);
        }

        // Compute local swing axes for all bones now that arms are in rest pose
        ComputeAxes(ref groupLTh);
        ComputeAxes(ref groupLCa);
        ComputeAxes(ref groupLFo);
        ComputeAxes(ref groupRTh);
        ComputeAxes(ref groupRCa);
        ComputeAxes(ref groupRFo);
        ComputeAxes(ref groupLUa);
        ComputeAxes(ref groupLFa);
        ComputeAxes(ref groupLHa);
        ComputeAxes(ref groupRUa);
        ComputeAxes(ref groupRFa);
        ComputeAxes(ref groupRHa);

        initialized = true;
    }

    BoneGroup CreateBoneGroup(Transform[] allTransforms, string primaryName, string fallbackName)
    {
        List<Transform> matched = new List<Transform>();
        foreach (var t in allTransforms)
        {
            if (t == null) continue;
            if (IsMatchingBone(t.name, primaryName) || IsMatchingBone(t.name, fallbackName))
            {
                matched.Add(t);
            }
        }

        BoneGroup bg = new BoneGroup();
        bg.bones = matched.ToArray();
        bg.restRot = new Quaternion[bg.bones.Length];
        bg.swingAxis = new Vector3[bg.bones.Length];

        for (int i = 0; i < bg.bones.Length; i++)
        {
            bg.restRot[i] = bg.bones[i].localRotation;
            bg.swingAxis[i] = Vector3.right;
        }

        return bg;
    }

    bool IsMatchingBone(string boneName, string target)
    {
        if (string.IsNullOrEmpty(boneName) || string.IsNullOrEmpty(target)) return false;
        if (boneName == target) return true;
        if (boneName.EndsWith(":" + target)) return true;
        if (boneName.EndsWith("_" + target)) return true;
        if (boneName.EndsWith(" " + target)) return true;
        return false;
    }

    void RotateGroupWorld(ref BoneGroup group, Vector3 worldAxis, float angleDeg)
    {
        if (group.bones == null) return;
        for (int i = 0; i < group.bones.Length; i++)
        {
            var b = group.bones[i];
            if (b != null)
            {
                b.Rotate(worldAxis, angleDeg, Space.World);
                group.restRot[i] = b.localRotation;
            }
        }
    }

    void ComputeAxes(ref BoneGroup group)
    {
        if (group.bones == null) return;
        for (int i = 0; i < group.bones.Length; i++)
        {
            var b = group.bones[i];
            if (b != null)
            {
                Vector3 localRight = Quaternion.Inverse(b.rotation) * transform.right;
                group.swingAxis[i] = localRight.sqrMagnitude > 0.001f ? localRight.normalized : Vector3.right;
            }
        }
    }

    void LateUpdate()
    {
        if (!initialized || agent == null || walkMotion == null) return;

        bool moving = agent.velocity.sqrMagnitude > 0.01f;
        float speedRatio = moving
            ? Mathf.Clamp01(agent.velocity.magnitude / Mathf.Max(agent.speed, 0.01f))
            : 0f;

        if (moving)
        {
            phase += Time.deltaTime * walkMotion.bobFrequency * phaseSpeedMultiplier * speedRatio;
            float s = Mathf.Sin(phase);
            float c = Mathf.Cos(phase);
            float a = speedRatio;

            // Legs swing in opposition
            SwingGroup(groupLTh, s * thighSwing * a);
            SwingGroup(groupRTh, -s * thighSwing * a);

            // Knees only flex backward, never hyperextend
            float lk = (1f - c) * 0.5f + Mathf.Max(0f, s) * 0.5f;
            float rk = (1f + c) * 0.5f + Mathf.Max(0f, -s) * 0.5f;
            SwingGroup(groupLCa, -lk * calfBend * a);
            SwingGroup(groupRCa, -rk * calfBend * a);

            // Feet rock
            SwingGroup(groupLFo, s * footRock * a * 0.5f);
            SwingGroup(groupRFo, -s * footRock * a * 0.5f);

            // Arms swing in opposition to legs (left arm forward when right leg forward)
            SwingGroup(groupLUa, -s * armSwing * a);
            SwingGroup(groupRUa, s * armSwing * a);

            // Forearms curl slightly on backswing
            SwingGroup(groupLFa, -Mathf.Max(0f, s) * forearmCurl * a);
            SwingGroup(groupRFa, -Mathf.Max(0f, -s) * forearmCurl * a);

            // Hands subtle rock
            SwingGroup(groupLHa, -s * handRock * a * 0.3f);
            SwingGroup(groupRHa, s * handRock * a * 0.3f);
        }
        else
        {
            // Settle smoothly back to corrected rest pose (arms hanging down naturally)
            SettleGroup(groupLTh);
            SettleGroup(groupLCa);
            SettleGroup(groupLFo);
            SettleGroup(groupRTh);
            SettleGroup(groupRCa);
            SettleGroup(groupRFo);
            SettleGroup(groupLUa);
            SettleGroup(groupLFa);
            SettleGroup(groupLHa);
            SettleGroup(groupRUa);
            SettleGroup(groupRFa);
            SettleGroup(groupRHa);

            phase = 0f;
        }
    }

    void SwingGroup(BoneGroup group, float deg)
    {
        if (group.bones == null) return;
        for (int i = 0; i < group.bones.Length; i++)
        {
            var b = group.bones[i];
            if (b != null)
            {
                b.localRotation = group.restRot[i] * Quaternion.AngleAxis(deg, group.swingAxis[i]);
            }
        }
    }

    void SettleGroup(BoneGroup group)
    {
        if (group.bones == null) return;
        for (int i = 0; i < group.bones.Length; i++)
        {
            var b = group.bones[i];
            if (b != null)
            {
                b.localRotation = Quaternion.Slerp(b.localRotation, group.restRot[i], Time.deltaTime * settleSpeed);
            }
        }
    }
}
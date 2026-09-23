using UnityEngine;

public class NpcWalkMotion : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent;
    public float bobAmount = 0.025f;
    public float bobFrequency = 8f;
    public float swayAmount = 0.018f;
    public float swayFrequency = 4f;
    public float leanStrength = 8f;
    public float leanSmooth = 5f;
    public float idleSettleSpeed = 6f;

    Transform body;
    Vector3 basePos;
    Quaternion baseRot;
    float walkPhase;
    float leanVel;

    void Awake()
    {
        if (agent == null) agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Ưu tiên tìm xương Hips để toàn bộ cơ thể và trang phục nhún nhảy đồng bộ
        Transform hips = null;
        foreach (var t in GetComponentsInChildren<Transform>(true))
        {
            if (t.name == "Hips" || t.name.EndsWith(":Hips") || t.name.EndsWith("_Hips") || t.name == "Pelvis")
            {
                hips = t;
                break;
            }
        }

        var smr = GetComponentInChildren<SkinnedMeshRenderer>(true);
        body = hips != null ? hips : (smr != null ? smr.transform : null);

        if (body != null)
        {
            basePos = body.localPosition;
            baseRot = body.localRotation;
        }
    }

    void Update()
    {
        if (agent == null || body == null) return;

        bool moving = agent.velocity.sqrMagnitude > 0.01f;
        float speedRatio = moving ? Mathf.Clamp01(agent.velocity.magnitude / Mathf.Max(agent.speed, 0.01f)) : 0f;

        if (moving)
        {
            walkPhase += Time.deltaTime * bobFrequency * speedRatio;

            float bob = Mathf.Sin(walkPhase) * bobAmount * speedRatio;
            float sway = Mathf.Sin(walkPhase * 0.5f) * swayAmount * speedRatio;

            body.localPosition = basePos + new Vector3(sway, bob, 0f);

            float targetLean = 0f;
            if (agent.angularSpeed > 0.1f)
            {
                Vector3 localVel = transform.InverseTransformDirection(agent.velocity);
                targetLean = Mathf.Clamp(-localVel.x * leanStrength, -10f, 10f);
            }
            leanVel = Mathf.Lerp(leanVel, targetLean, Time.deltaTime * leanSmooth);
            body.localRotation = baseRot * Quaternion.Euler(0f, 0f, leanVel);
        }
        else
        {
            walkPhase = 0f;
            body.localPosition = Vector3.Lerp(body.localPosition, basePos, Time.deltaTime * idleSettleSpeed);
            leanVel = Mathf.Lerp(leanVel, 0f, Time.deltaTime * idleSettleSpeed);
            body.localRotation = Quaternion.Slerp(body.localRotation, baseRot, Time.deltaTime * idleSettleSpeed);
        }
    }
}
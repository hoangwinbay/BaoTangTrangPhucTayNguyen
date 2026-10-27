using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class MuseumVRRigManager : MonoBehaviour
{
    public static MuseumVRRigManager Instance { get; private set; }

    [Header("Cài đặt Di chuyển")]
    [Tooltip("Tốc độ đi bộ trong bảo tàng (m/s)")]
    public float vrWalkSpeed = 2.5f;

    private XROrigin xrOrigin;
    private FPSController pcController;
    private Camera pcCamera;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoInit()
    {
        if (Instance == null && FindObjectOfType<MuseumVRRigManager>() == null)
        {
            GameObject go = new GameObject("MuseumVRRigManager");
            go.AddComponent<MuseumVRRigManager>();
            DontDestroyOnLoad(go);
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    IEnumerator Start()
    {
        yield return null;

        SetupEnvironment();
    }

    public void SetupEnvironment()
    {
        bool isVR = VRInputHelper.IsVREnvironment();
        pcController = FindObjectOfType<FPSController>();
        xrOrigin = FindObjectOfType<XROrigin>();

        EnsureInteractionManager();

        if (isVR)
        {
            ConfigureVRPlayer();
        }
        else
        {
            ConfigurePCPlayer();
        }

        ConfigureExhibitionCubeInteractables();
    }

    private void EnsureInteractionManager()
    {
        var manager = FindObjectOfType<XRInteractionManager>();
        if (manager == null)
        {
            GameObject managerObj = new GameObject("XR Interaction Manager");
            managerObj.AddComponent<XRInteractionManager>();
        }
    }

    private void ConfigureVRPlayer()
    {
        Debug.Log("[MuseumVRRigManager] Đang chạy trên môi trường VR Meta Quest. Tiến hành kích hoạt XR Origin...");

        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        if (pcController != null)
        {
            spawnPosition = pcController.transform.position;
            spawnRotation = pcController.transform.rotation;
        }

        if (xrOrigin == null)
        {
            GameObject vrPrefab = Resources.Load<GameObject>("VR/XROriginRig");
            if (vrPrefab != null)
            {
                GameObject vrInstance = Instantiate(vrPrefab, spawnPosition, spawnRotation);
                vrInstance.name = "XR Origin (XR Rig)";
                xrOrigin = vrInstance.GetComponent<XROrigin>();
            }
        }

        if (xrOrigin != null)
        {
            xrOrigin.gameObject.SetActive(true);

            if (spawnPosition != Vector3.zero && (xrOrigin.transform.position - spawnPosition).sqrMagnitude > 1f)
            {
                xrOrigin.transform.position = spawnPosition;
                xrOrigin.transform.rotation = spawnRotation;
            }

            var moveProvider = xrOrigin.GetComponentInChildren<ContinuousMoveProvider>();
            if (moveProvider != null)
            {
                moveProvider.moveSpeed = vrWalkSpeed;
                moveProvider.enableFly = false;
                moveProvider.useGravity = true;

                if (xrOrigin.Camera != null)
                {
                    xrOrigin.Camera.tag = "MainCamera";
                    moveProvider.forwardSource = xrOrigin.Camera.transform;
                }
            }

            var snapTurn = xrOrigin.GetComponentInChildren<SnapTurnProvider>();
            if (snapTurn != null)
            {
                snapTurn.enabled = false;
            }

            var continuousTurn = xrOrigin.GetComponentInChildren<ContinuousTurnProvider>();
            if (continuousTurn != null)
            {
                continuousTurn.enabled = false;
            }

            Transform turnObj = xrOrigin.transform.Find("Locomotion/Turn");
            if (turnObj != null)
            {
                turnObj.gameObject.SetActive(false);
            }

            var inputActionMgr = xrOrigin.GetComponentInChildren<InputActionManager>();
            if (inputActionMgr == null)
            {
                inputActionMgr = FindObjectOfType<InputActionManager>();
            }
            if (inputActionMgr != null && !inputActionMgr.enabled)
            {
                inputActionMgr.enabled = true;
            }
        }

        if (pcController != null)
        {
            pcController.enabled = false;

            if (pcController.playerCamera != null)
            {
                var cam = pcController.playerCamera.GetComponent<Camera>();
                if (cam != null) cam.enabled = false;

                var listener = pcController.playerCamera.GetComponent<AudioListener>();
                if (listener != null) listener.enabled = false;
            }
        }
    }

    private void ConfigurePCPlayer()
    {
        Debug.Log("[MuseumVRRigManager] Đang chạy trên máy tính (PC Desktop). Giữ nguyên điều khiển bàn phím/chuột.");

        if (pcController != null)
        {
            pcController.enabled = true;
            if (pcController.playerCamera != null)
            {
                pcController.playerCamera.tag = "MainCamera";
                var cam = pcController.playerCamera.GetComponent<Camera>();
                if (cam != null) cam.enabled = true;

                var listener = pcController.playerCamera.GetComponent<AudioListener>();
                if (listener != null) listener.enabled = true;
            }
        }

        if (xrOrigin != null)
        {
            xrOrigin.gameObject.SetActive(false);
        }
    }

    public static void ConfigureExhibitionCubeInteractables()
    {
        CubeExhibitionTrigger[] triggers = FindObjectsOfType<CubeExhibitionTrigger>();
        foreach (var t in triggers)
        {
            if (t == null) continue;

            var interactable = t.GetComponent<XRSimpleInteractable>();
            if (interactable == null)
            {
                interactable = t.gameObject.AddComponent<XRSimpleInteractable>();
            }

            interactable.selectEntered.RemoveAllListeners();
            interactable.selectEntered.AddListener((args) =>
            {
                VRInputHelper.TriggerHapticBoth(0.45f, 0.1f);
                t.TriggerInteraction();
            });

            interactable.hoverEntered.RemoveAllListeners();
            interactable.hoverEntered.AddListener((args) =>
            {
                VRInputHelper.TriggerHapticBoth(0.18f, 0.04f);
            });
        }
    }
}

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

/// <summary>
/// Quản lý thiết lập người chơi trong môi trường VR Meta Quest 2 và máy tính PC.
/// - Khi chạy trên Kính VR (Meta Quest 2):
///   + Kích hoạt XR Origin (XR Rig).
///   + Cần Joystick TRÁI: Di chuyển bước đi theo hướng nhìn của đầu.
///   + Nhìn quanh: Quay đầu tự nhiên 100% (6DoF Head Tracking), vô hiệu hóa xoay nhân tạo bằng joystick phải.
///   + 2 Tay cầm: Tia laser raycast tương tác bệ hiện vật và nút bấm UI.
///   + Tự động tắt FPSController máy tính để tránh xung đột camera/âm thanh.
/// - Khi chạy trên PC không cắm kính:
///   + Giữ nguyên FPSController điều khiển bằng bàn phím WASD và chuột.
/// </summary>
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
        // Chờ 1 frame để toàn bộ GameObjects trong Scene khởi tạo xong
        yield return null;

        SetupEnvironment();
    }

    /// <summary>
    /// Kiểm tra môi trường và cấu hình hệ thống điều khiển tương ứng
    /// </summary>
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

        // Tự động gắn XRSimpleInteractable cho 12 bệ hiện vật để tia laser có thể bấm
        ConfigureExhibitionCubeInteractables();
    }

    /// <summary>
    /// Đảm bảo luôn có XRInteractionManager trong scene để xử lý tương tác
    /// </summary>
    private void EnsureInteractionManager()
    {
        var manager = FindObjectOfType<XRInteractionManager>();
        if (manager == null)
        {
            GameObject managerObj = new GameObject("XR Interaction Manager");
            managerObj.AddComponent<XRInteractionManager>();
        }
    }

    /// <summary>
    /// Cấu hình người chơi cho kính VR Meta Quest 2
    /// </summary>
    private void ConfigureVRPlayer()
    {
        Debug.Log("[MuseumVRRigManager] Đang chạy trên môi trường VR Meta Quest. Tiến hành kích hoạt XR Origin...");

        // 1. Tọa độ xuất phát: Lấy từ vị trí của FPSController máy tính nếu có
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        if (pcController != null)
        {
            spawnPosition = pcController.transform.position;
            spawnRotation = pcController.transform.rotation;
        }

        // 2. Tìm hoặc nạp XR Origin
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

            // Đảm bảo XR Origin đứng đúng vị trí spawn ban đầu
            if (spawnPosition != Vector3.zero && (xrOrigin.transform.position - spawnPosition).sqrMagnitude > 1f)
            {
                xrOrigin.transform.position = spawnPosition;
                xrOrigin.transform.rotation = spawnRotation;
            }

            // 3. CẤU HÌNH DI CHUYỂN: Joystick TRÁI di chuyển theo hướng nhìn của đầu
            var moveProvider = xrOrigin.GetComponentInChildren<ContinuousMoveProvider>();
            if (moveProvider != null)
            {
                moveProvider.moveSpeed = vrWalkSpeed;
                moveProvider.enableFly = false;
                moveProvider.useGravity = true;

                // Hướng đi tới trùng khớp với hướng quay đầu của kính HMD
                if (xrOrigin.Camera != null)
                {
                    xrOrigin.Camera.tag = "MainCamera";
                    moveProvider.forwardSource = xrOrigin.Camera.transform;
                }
            }

            // 4. THEO YÊU CẦU NGƯỜI DÙNG: Không dùng Joystick phải để xoay nhân tạo!
            // Người chơi chỉ cần quay đầu thực tế để nhìn quanh (6DoF Head Tracking tự nhiên 100%)
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

            // 5. Đảm bảo có InputActionManager để kích hoạt cần gạt và nút bấm
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

        // 6. TẮT FPSController máy tính và camera máy tính để tránh xung đột
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

    /// <summary>
    /// Cấu hình người chơi cho máy tính PC không dùng kính VR
    /// </summary>
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

        // Tạm ẩn XR Origin nếu chưa có kính
        if (xrOrigin != null)
        {
            xrOrigin.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Gắn thêm XRSimpleInteractable cho các bệ Cube để tia laser tay cầm VR có thể trỏ và click mở popup
    /// </summary>
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

            // Cấu hình sự kiện khi bóp cò tia laser vào bệ
            interactable.selectEntered.RemoveAllListeners();
            interactable.selectEntered.AddListener((args) =>
            {
                VRInputHelper.TriggerHapticBoth(0.45f, 0.1f);
                t.TriggerInteraction();
            });

            // Rung nhẹ khi tia laser lướt qua bệ
            interactable.hoverEntered.RemoveAllListeners();
            interactable.hoverEntered.AddListener((args) =>
            {
                VRInputHelper.TriggerHapticBoth(0.18f, 0.04f);
            });
        }
    }
}

using System.IO;
using UnityEngine;

/// <summary>
/// Gắn vào bệ trưng bày (Cube).
/// Khi người chơi đến gần, biểu tượng ngón tay sẽ xuất hiện trên mặt trên của Cube và thực hiện animation nhấn xuống.
/// Khi ở gần, người chơi bấm phím Shift (trái hoặc phải) để xem ảnh thuyết minh pop-up ra giữa màn hình.
/// Bấm Shift lần nữa để thu ảnh lại.
/// </summary>
public class CubeExhibitionTrigger : MonoBehaviour
{
    [Header("1. Hình ảnh Thuyết minh (Kéo ảnh vào đây)")]
    [Tooltip("Kéo ảnh Texture2D từ thư mục 'Assets/3d/ảnh chi tiết' vào đây")]
    public Texture2D detailTexture;

    [Tooltip("Hoặc kéo Sprite vào đây (nếu ảnh đã đổi sang Sprite 2D and UI)")]
    public Sprite detailSprite;

    [Header("2. Tiêu đề Dân tộc")]
    public string ethnicTitle = "DÂN TỘC TÂY NGUYÊN";

    [Header("3. Cài đặt Khoảng cách")]
    [Tooltip("Khoảng cách tối đa (mét) để ngón tay hiện ra và cho phép bấm Shift")]
    public float triggerDistance = 2.8f;

    [Header("4. Cài đặt Biểu tượng Ngón tay Nhấn")]
    [Tooltip("Sprite ngón tay (tự nạp từ Assets/UI/FingerTapIcon.png nếu để trống)")]
    public Sprite fingerSprite;
    [Tooltip("Kích thước hiển thị của biểu tượng ngón tay")]
    public float fingerScale = 0.12f;
    [Tooltip("Độ cao ngón tay lơ lửng phía trên mặt trên của Cube")]
    public float heightAboveCube = 0.18f;
    [Tooltip("Tốc độ nhấn ngón tay")]
    public float tapSpeed = 3.8f;
    [Tooltip("Độ nhấp nhô khi ngón tay nhấn xuống")]
    public float tapDepth = 0.05f;
    [Tooltip("Màu sắc và độ sáng của icon")]
    public Color iconColor = Color.white;

    // Thành phần nội bộ
    private GameObject promptObj;
    private SpriteRenderer promptRenderer;
    private Vector3 baseTopCenter;
    private float currentAlpha = 0f;
    private bool isPlayerNear = false;
    private Transform playerCam;

    void Awake()
    {
        EnsureFingerSprite();
        CreatePromptObject();
    }

    void Start()
    {
        // Tự động thu nhỏ scale nếu trước đó bệ mang giá trị cũ quá lớn (> 0.20f)
        if (fingerScale > 0.20f)
        {
            fingerScale = 0.12f;
            heightAboveCube = 0.18f;
            tapDepth = 0.05f;
        }
        EnsureFingerSprite();
        UpdateCubeTopPosition();
        CachePlayerCamera();
    }

    void OnEnable()
    {
        UpdateCubeTopPosition();
    }

    /// <summary>
    /// Nạp Sprite ngón tay nếu chưa được gán trong Inspector
    /// </summary>
    public void EnsureFingerSprite()
    {
        if (fingerSprite != null) return;

#if UNITY_EDITOR
        fingerSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/FingerTapIcon.png");
        if (fingerSprite != null) return;
#endif

        string filePath = Path.Combine(Application.dataPath, "UI/FingerTapIcon.png");
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (tex.LoadImage(fileData))
            {
                tex.filterMode = FilterMode.Bilinear;
                fingerSprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    100f
                );
            }
        }
    }

    private static Transform globalPromptHolder;

    /// <summary>
    /// Tạo GameObject con chứa SpriteRenderer cho ngón tay (tách biệt transform để không bị méo góc/scale)
    /// </summary>
    private void CreatePromptObject()
    {
        if (promptObj != null) return;

        if (globalPromptHolder == null)
        {
            GameObject holderGo = GameObject.Find("_ExhibitionFingerPrompts");
            if (holderGo == null)
            {
                holderGo = new GameObject("_ExhibitionFingerPrompts");
            }
            globalPromptHolder = holderGo.transform;
        }

        string promptName = $"FingerPrompt_{gameObject.name}";
        Transform existing = globalPromptHolder.Find(promptName);
        if (existing != null)
        {
            promptObj = existing.gameObject;
            promptRenderer = promptObj.GetComponent<SpriteRenderer>();
        }
        else
        {
            promptObj = new GameObject(promptName);
            promptObj.transform.SetParent(globalPromptHolder, false);
            promptRenderer = promptObj.AddComponent<SpriteRenderer>();
        }

        if (fingerSprite != null && promptRenderer != null)
        {
            promptRenderer.sprite = fingerSprite;
        }

        if (promptRenderer != null)
        {
            promptRenderer.color = new Color(iconColor.r, iconColor.g, iconColor.b, 0f);
            promptRenderer.sortingOrder = 10;
        }

        // Gắn Collider và XRSimpleInteractable để tia laser từ tay cầm VR có thể trỏ và tương tác trực tiếp vào icon ngón tay
        SphereCollider col = promptObj.GetComponent<SphereCollider>();
        if (col == null)
        {
            col = promptObj.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.25f;
        }

        var promptInteractable = promptObj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (promptInteractable == null)
        {
            promptInteractable = promptObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        }
        promptInteractable.selectEntered.RemoveAllListeners();
        promptInteractable.selectEntered.AddListener((args) =>
        {
            VRInputHelper.TriggerHapticBoth(0.45f, 0.1f);
            TriggerInteraction();
        });
        promptInteractable.hoverEntered.RemoveAllListeners();
        promptInteractable.hoverEntered.AddListener((args) =>
        {
            VRInputHelper.TriggerHapticBoth(0.18f, 0.04f);
        });

        promptObj.SetActive(false);
    }

    void OnDisable()
    {
        if (promptObj != null) promptObj.SetActive(false);
    }

    void OnDestroy()
    {
        if (promptObj != null) Destroy(promptObj);
    }

    /// <summary>
    /// Tính toán tâm mặt trên của Cube
    /// </summary>
    public void UpdateCubeTopPosition()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            baseTopCenter = new Vector3(rend.bounds.center.x, rend.bounds.max.y, rend.bounds.center.z);
            return;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            baseTopCenter = new Vector3(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);
            return;
        }

        baseTopCenter = transform.position + Vector3.up * (transform.lossyScale.y * 0.5f);
    }

    private void CachePlayerCamera()
    {
        if (Camera.main != null)
        {
            playerCam = Camera.main.transform;
            return;
        }

        // Tìm Camera từ XR Origin nếu đang ở môi trường VR
        var xrCamera = GameObject.Find("Main Camera");
        if (xrCamera != null)
        {
            playerCam = xrCamera.transform;
            return;
        }

        var fps = FindObjectOfType<FPSController>();
        if (fps != null)
        {
            playerCam = fps.playerCamera != null ? fps.playerCamera : fps.transform;
        }
    }

    /// <summary>
    /// Kích hoạt mở bảng thuyết minh (gọi từ nút bấm, tia raycast, hoặc tương tác VR)
    /// </summary>
    public void TriggerInteraction()
    {
        if (MuseumPopupManager.Instance != null && !MuseumPopupManager.Instance.IsOpen)
        {
            if (Time.unscaledTime - MuseumPopupManager.lastToggleTime > 0.22f)
            {
                VRInputHelper.TriggerHapticBoth(0.45f, 0.1f);
                MuseumPopupManager.Instance.ShowPopup(this);
            }
        }
    }

    void Update()
    {
        if (playerCam == null)
        {
            CachePlayerCamera();
            if (playerCam == null) return;
        }

        // Tính khoảng cách từ camera / người chơi đến mặt trên của Cube
        float distance = Vector3.Distance(playerCam.position, baseTopCenter);
        isPlayerNear = (distance <= triggerDistance);

        // Hiệu ứng mờ dần (Fade In / Fade Out)
        float targetAlpha = isPlayerNear ? 1f : 0f;
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * 5f);

        if (promptObj != null)
        {
            if (currentAlpha > 0.001f)
            {
                if (!promptObj.activeSelf) promptObj.SetActive(true);

                // 1. ANIMATION NHẤN NGÓN TAY (Tapping motion)
                float cycle = (Time.time * tapSpeed) % (Mathf.PI * 2f);
                float tapMotion = Mathf.Max(0f, Mathf.Sin(cycle));
                tapMotion = Mathf.Pow(tapMotion, 1.8f); // Gia tốc cho động tác nhấn tự nhiên
                float currentY = baseTopCenter.y + heightAboveCube - (tapMotion * tapDepth);

                // 2. GIỮ ĐÚNG TỌA ĐỘ THẾ GIỚI
                promptObj.transform.position = new Vector3(baseTopCenter.x, currentY, baseTopCenter.z);

                // 3. BILLBOARD HƯỚNG VỀ CAMERA
                promptObj.transform.rotation = playerCam.rotation;

                // 4. GÁN TRỰC TIẾP WORLD SCALE (KHÔNG BỊ ẢNH HƯỞNG BỞI ROTATION HAY SCALE CỦA CUBE)
                float worldScale = fingerScale;
                if (tapMotion > 0.7f)
                {
                    worldScale *= (1f + (tapMotion - 0.7f) * 0.15f);
                }
                promptObj.transform.localScale = Vector3.one * worldScale;

                // 5. CẬP NHẬT ALPHA VÀ SPRITE CHO RENDERER
                if (promptRenderer != null)
                {
                    if (fingerSprite != null && promptRenderer.sprite != fingerSprite)
                    {
                        promptRenderer.sprite = fingerSprite;
                    }
                    promptRenderer.color = new Color(iconColor.r, iconColor.g, iconColor.b, currentAlpha);
                }
            }
            else
            {
                if (promptObj.activeSelf) promptObj.SetActive(false);
            }
        }

        // KIỂM TRA HÀNH ĐỘNG TƯƠNG TÁC (Cả PC và Tay cầm VR Meta Quest 2)
        bool userInteracted = false;

        // 1. Phím Shift / E / Space trên PC
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) ||
            Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            userInteracted = true;
        }

        // 2. Nút bấm trên tay cầm VR Meta Quest 2 (Cò trỏ Index Trigger, nút A, nút X, hoặc nút bóp Grip)
        if (VRInputHelper.IsTriggerDown() || VRInputHelper.IsPrimaryButtonDown() || VRInputHelper.IsAnyVRActionTriggered())
        {
            userInteracted = true;
        }

        // LẮNG NGHE KHI Ở ĐỦ GẦN (Chỉ kích hoạt khi Popup CHƯA mở để tránh lỗi chớp tắt cùng frame)
        if (MuseumPopupManager.Instance != null && !MuseumPopupManager.Instance.IsOpen)
        {
            if (isPlayerNear && userInteracted)
            {
                // Debounce sau khi vừa đóng popup
                if (Time.unscaledTime - MuseumPopupManager.lastToggleTime > 0.22f)
                {
                    // Chỉ cho phép bệ gần người chơi nhất mở popup (tránh xung đột khi đứng giữa 2 bệ)
                    if (IsClosestTriggerToPlayer())
                    {
                        TriggerInteraction();
                    }
                }
            }
        }
        else if (!isPlayerNear && MuseumPopupManager.Instance != null && MuseumPopupManager.Instance.IsOpen)
        {
            // Tự động thu lại nếu người chơi đi quá xa bệ đang mở
            if (MuseumPopupManager.Instance.CurrentTrigger == this &&
                distance > triggerDistance * 1.5f)
            {
                MuseumPopupManager.Instance.ClosePopup();
            }
        }
    }

    /// <summary>
    /// Kiểm tra xem bệ này có phải là bệ gần camera người chơi nhất trong số các bệ hợp lệ hay không
    /// </summary>
    private bool IsClosestTriggerToPlayer()
    {
        if (playerCam == null) return true;
        float myDist = Vector3.Distance(playerCam.position, baseTopCenter);
        CubeExhibitionTrigger[] allTriggers = FindObjectsOfType<CubeExhibitionTrigger>();
        foreach (var t in allTriggers)
        {
            if (t != this && t.enabled && t.gameObject.activeInHierarchy)
            {
                float otherDist = Vector3.Distance(playerCam.position, t.baseTopCenter);
                if (otherDist < myDist)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void OnDrawGizmosSelected()
    {
        UpdateCubeTopPosition();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(baseTopCenter, triggerDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(baseTopCenter + Vector3.up * heightAboveCube, 0.05f);
    }
}

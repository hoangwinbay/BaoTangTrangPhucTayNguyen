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
    /// Nạp Sprite ngón tay. Nếu mọi cách load đều thất bại, tự vẽ sprite
    /// bằng code để đảm bảo icon luôn hiển thị trên mọi nền tảng.
    /// </summary>
    public void EnsureFingerSprite()
    {
        if (fingerSprite != null) return;

        // Cách 1: Resources.Load — hoạt động trên cả Editor và Android APK
        fingerSprite = Resources.Load<Sprite>("FingerTapIcon");
        if (fingerSprite != null)
        {
            Debug.Log("[CubeExhibitionTrigger] Loaded FingerTapIcon from Resources.");
            return;
        }

#if UNITY_EDITOR
        // Cách 2: AssetDatabase — chỉ trong Editor
        fingerSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/FingerTapIcon.png");
        if (fingerSprite != null)
        {
            Debug.Log("[CubeExhibitionTrigger] Loaded FingerTapIcon from AssetDatabase.");
            return;
        }
#endif

        // Cách 3: Tự vẽ icon ngón tay chỉ trỏ (mũi tên trắng dạng cursor) bằng code
        Debug.LogWarning("[CubeExhibitionTrigger] FingerTapIcon not found in Resources or AssetDatabase — generating fallback sprite.");
        fingerSprite = GenerateFallbackFingerSprite();
    }

    /// <summary>
    /// Tạo sprite ngón tay trỏ đơn giản bằng code (hình mũi tên trắng trên nền trong suốt).
    /// Đảm bảo icon luôn hiển thị kể cả khi file ảnh bị thiếu.
    /// </summary>
    private static Sprite GenerateFallbackFingerSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = Color.clear;
        Color white = Color.white;
        Color outline = new Color(0f, 0f, 0f, 0.7f);

        // Xóa nền trong suốt
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;

        // Vẽ hình tròn nền trắng mờ (glow)
        Vector2 center = new Vector2(size * 0.5f, size * 0.35f);
        float radius = size * 0.30f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist < radius)
                {
                    float alpha = Mathf.Clamp01(1f - dist / radius);
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha * 0.9f);
                }
            }
        }

        // Vẽ thân ngón tay (hình chữ nhật dọc)
        int fingerW = Mathf.RoundToInt(size * 0.18f);
        int fingerH = Mathf.RoundToInt(size * 0.42f);
        int fingerX = Mathf.RoundToInt(center.x) - fingerW / 2;
        int fingerY = Mathf.RoundToInt(center.y) - fingerH / 2;
        for (int y = fingerY; y < fingerY + fingerH && y < size; y++)
        {
            for (int x = fingerX; x < fingerX + fingerW && x < size; x++)
            {
                if (x >= 0 && y >= 0)
                    pixels[y * size + x] = white;
            }
        }

        // Đầu ngón tay (hình bán cầu tròn)
        Vector2 tipCenter = new Vector2(center.x, fingerY + fingerH - 1);
        float tipR = fingerW * 0.55f;
        for (int y = (int)(tipCenter.y - tipR); y <= (int)(tipCenter.y + tipR); y++)
        {
            for (int x = (int)(tipCenter.x - tipR); x <= (int)(tipCenter.x + tipR); x++)
            {
                if (x >= 0 && y >= 0 && x < size && y < size)
                {
                    if (Vector2.Distance(new Vector2(x, y), tipCenter) <= tipR)
                        pixels[y * size + x] = white;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
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
            promptRenderer.sharedMaterial = GetFingerSpriteMaterial();
            promptRenderer.color = new Color(iconColor.r, iconColor.g, iconColor.b, 0f);
            promptRenderer.sortingOrder = 100;
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

    private static Material sharedFingerSpriteMat;

    private static Material GetFingerSpriteMaterial()
    {
        if (sharedFingerSpriteMat != null) return sharedFingerSpriteMat;

        // Ưu tiên shader Sprite Unlit của URP, fallback Sprites/Default hoặc UI/Default
        Shader s = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (s == null) s = Shader.Find("Sprites/Default");
        if (s == null) s = Shader.Find("UI/Default");
        if (s == null) s = Shader.Find("Universal Render Pipeline/Unlit");

        if (s != null)
        {
            sharedFingerSpriteMat = new Material(s);
            sharedFingerSpriteMat.name = "Runtime_FingerSprite_Unlit";
        }
        return sharedFingerSpriteMat;
    }

    /// <summary>
    /// Luôn tìm đúng Camera đang thực sự ACTIVE và render hình ảnh cho người chơi.
    /// Khắc phục triệt để lỗi cache nhầm camera bị vô hiệu hóa khi chuyển đổi giữa PC Desktop và kính VR.
    /// </summary>
    private Transform GetActivePlayerCamera()
    {
        // 1. Nếu playerCam hiện tại vẫn đang active trong scene, tái sử dụng ngay
        if (playerCam != null && playerCam.gameObject.activeInHierarchy)
        {
            return playerCam;
        }

        // 2. Nếu đang chạy trên PC (FPSController đang active)
        var fps = FindObjectOfType<FPSController>();
        if (fps != null && fps.gameObject.activeInHierarchy && fps.playerCamera != null && fps.playerCamera.gameObject.activeInHierarchy)
        {
            playerCam = fps.playerCamera;
            return playerCam;
        }

        // 3. Nếu đang chạy trên VR (XROrigin đang active)
        var xr = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (xr != null && xr.gameObject.activeInHierarchy && xr.Camera != null && xr.Camera.gameObject.activeInHierarchy)
        {
            playerCam = xr.Camera.transform;
            return playerCam;
        }

        // 4. Ưu tiên Camera.main nếu active trong scene
        if (Camera.main != null && Camera.main.gameObject.activeInHierarchy)
        {
            playerCam = Camera.main.transform;
            return playerCam;
        }

        // 5. Quét tất cả Camera trong scene, lấy camera enabled & active đầu tiên
        Camera[] allCams = FindObjectsOfType<Camera>();
        foreach (var c in allCams)
        {
            if (c.enabled && c.gameObject.activeInHierarchy)
            {
                playerCam = c.transform;
                return playerCam;
            }
        }

        return null;
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
        // Luôn đảm bảo lấy đúng Camera đang active (chuyển đổi linh hoạt giữa PC và VR)
        playerCam = GetActivePlayerCamera();
        if (playerCam == null) return;

        // Đảm bảo tâm mặt trên của Cube luôn chính xác
        if (baseTopCenter == Vector3.zero)
        {
            UpdateCubeTopPosition();
        }

        // Tính khoảng cách từ người chơi đến bệ (tính khoảng cách mặt phẳng XZ kết hợp độ cao)
        Vector3 camPos = playerCam.position;
        float horizontalDist = Vector2.Distance(new Vector2(camPos.x, camPos.z), new Vector2(baseTopCenter.x, baseTopCenter.z));
        float verticalDist = Mathf.Abs(camPos.y - baseTopCenter.y);
        
        // Người chơi được coi là ở gần khi khoảng cách ngang <= triggerDistance (mặc định ~3.2m) và độ cao chênh lệch hợp lý
        float effectiveDist = Vector3.Distance(camPos, baseTopCenter);
        isPlayerNear = (effectiveDist <= Mathf.Max(triggerDistance, 3.2f)) || (horizontalDist <= 2.8f && verticalDist <= 2.5f);

        // Hiệu ứng mờ dần (Fade In / Fade Out)
        float targetAlpha = isPlayerNear ? 1f : 0f;
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * 6f);

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
                float worldScale = Mathf.Max(fingerScale, 0.12f);
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
                    if (promptRenderer.sharedMaterial == null)
                    {
                        promptRenderer.sharedMaterial = GetFingerSpriteMaterial();
                    }
                    promptRenderer.color = new Color(iconColor.r, iconColor.g, iconColor.b, currentAlpha);
                }
            }
            else
            {
                if (promptObj.activeSelf) promptObj.SetActive(false);
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

using UnityEngine;

/// <summary>
/// Gắn vào bệ trưng bày (Cube).
/// Khi người chơi đến gần (< 3.2m):
/// - Xuất hiện 2 biểu tượng nằm cạnh nhau trên bệ: Ngón tay trỏ (bên trái) và Loa phát thanh (bên phải).
/// - Cả 2 icon được thu nhỏ bằng một nửa (scale 0.06f) để gọn gàng, tinh tế trên bệ.
/// - Nhấn phím Shift (hoặc cò trỏ / nút A trên VR): Mở / thu bảng thông tin thuyết minh.
/// - Nhấn phím V (hoặc nút B/Y trên VR): Bật / tắt bài đọc thuyết minh ghi âm (không hiển thị thêm giao diện nào).
/// - Khi người chơi đi ra xa: Âm lượng giọng đọc nhỏ dần rồi tự tắt hẳn. Khi lại gần không tự to lại mà phải nhấn V lại.
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
    [Tooltip("Khoảng cách tối đa (mét) để 2 icon hiện ra và cho phép tương tác")]
    public float triggerDistance = 3.2f;
    [Tooltip("Khoảng cách người chơi đi xa trước khi bài đọc thuyết minh tự động tắt hẳn")]
    public float maxAudibleDistance = 5.5f;

    [Header("4. Cài đặt Biểu tượng (Ngón tay & Loa)")]
    [Tooltip("Sprite ngón tay (tự nạp từ Assets/Resources/FingerTapIcon.png nếu để trống)")]
    public Sprite fingerSprite;
    [Tooltip("Sprite chiếc loa (tự nạp từ Assets/Resources/SpeakerIcon.png nếu để trống)")]
    public Sprite speakerSprite;
    [Tooltip("Kích thước hiển thị của 2 biểu tượng (0.032f: nhỏ gọn, tinh tế)")]
    public float fingerScale = 0.032f;
    [Tooltip("Khoảng cách tách rời giữa 2 biểu tượng từ tâm (0.16m = 2 icon cách nhau 32cm, không bị lồng vào nhau)")]
    public float iconSpacing = 0.16f;
    [Tooltip("Độ cao của icon so với cạnh dưới (đáy) của Cube (mét)")]
    public float heightFromBottom = 0.15f;
    [Tooltip("Độ nhô ra phía trước mặt bệ Cube theo hướng người chơi (mét)")]
    public float forwardOffset = 0.08f;
    [Tooltip("Độ cao biểu tượng lơ lửng phía trên mặt trên của Cube (cho phiên bản cũ)")]
    public float heightAboveCube = 0.15f;
    [Tooltip("Tốc độ nhấp nhô của ngón tay")]
    public float tapSpeed = 3.8f;
    [Tooltip("Độ nhấp nhô khi ngón tay nhấn xuống")]
    public float tapDepth = 0.015f;
    [Tooltip("Màu sắc và độ sáng của icon")]
    public Color iconColor = Color.white;

    [Header("5. Giọng đọc Thuyết minh (Audio Narration)")]
    [Tooltip("Gắn file ghi âm giọng đọc thuyết minh vào đây (Cube 72 tự động nạp ThuyetMinh_Cube72 nếu để trống)")]
    public AudioClip voiceNarration;

    // Thành phần nội bộ cho 2 icon
    private GameObject promptObj;               // Icon ngón tay
    private SpriteRenderer promptRenderer;
    private GameObject speakerPromptObj;        // Icon chiếc loa
    private SpriteRenderer speakerPromptRenderer;

    private AudioSource narrationAudioSource;
    private bool isNarrationPlaying = false;
    public bool IsNarrationPlaying => isNarrationPlaying;

    private Vector3 baseTopCenter;
    private float currentAlpha = 0f;
    private bool isPlayerNear = false;
    private Transform playerCam;

    void Awake()
    {
        ApplyIconSettings();
        EnsureFingerSprite();
        EnsureSpeakerSprite();
        EnsureVoiceNarration();
        CreatePromptObjects();
    }

    void Start()
    {
        ApplyIconSettings();
        EnsureFingerSprite();
        EnsureSpeakerSprite();
        EnsureVoiceNarration();
        UpdateCubeTopPosition();
        playerCam = GetActivePlayerCamera();
    }

    private void ApplyIconSettings()
    {
        if (fingerScale > 0.045f)
        {
            fingerScale = 0.032f;
        }
        if (iconSpacing < 0.12f)
        {
            iconSpacing = 0.16f;
        }
        if (heightFromBottom <= 0.01f)
        {
            heightFromBottom = 0.15f;
        }
        if (forwardOffset <= 0.01f)
        {
            forwardOffset = 0.08f;
        }
    }

    void OnEnable()
    {
        UpdateCubeTopPosition();
    }

    /// <summary>
    /// Nạp Sprite ngón tay.
    /// </summary>
    public void EnsureFingerSprite()
    {
        if (fingerSprite != null) return;

        fingerSprite = Resources.Load<Sprite>("FingerTapIcon");
        if (fingerSprite != null) return;

#if UNITY_EDITOR
        fingerSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/FingerTapIcon.png");
        if (fingerSprite != null) return;
#endif

        fingerSprite = GenerateFallbackFingerSprite();
    }

    /// <summary>
    /// Nạp Sprite chiếc loa phát thanh đã tách nền trong suốt.
    /// </summary>
    public void EnsureSpeakerSprite()
    {
        if (speakerSprite != null) return;

        speakerSprite = Resources.Load<Sprite>("SpeakerIcon");
        if (speakerSprite != null) return;

#if UNITY_EDITOR
        speakerSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/SpeakerIcon.png");
        if (speakerSprite != null) return;
#endif
    }

    /// <summary>
    /// Tự động nạp file thuyết minh âm thanh cho Cube (72).
    /// </summary>
    public void EnsureVoiceNarration()
    {
        if (voiceNarration != null) return;

        if (gameObject.name.Contains("72") || ethnicTitle.Contains("Giẻ Triêng") || ethnicTitle.Contains("Rơ Măm"))
        {
            voiceNarration = Resources.Load<AudioClip>("Audio/ThuyetMinh_Cube72");
            if (voiceNarration != null) return;

#if UNITY_EDITOR
            voiceNarration = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/ThuyetMinh_Cube72.mp3");
#endif
        }
    }

    /// <summary>
    /// Tạo sprite ngón tay trỏ dự phòng nếu file ảnh bị thiếu
    /// </summary>
    private static Sprite GenerateFallbackFingerSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = Color.clear;
        Color white = Color.white;

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;

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

        int fingerW = Mathf.RoundToInt(size * 0.18f);
        int fingerH = Mathf.RoundToInt(size * 0.42f);
        int fingerX = Mathf.RoundToInt(center.x) - fingerW / 2;
        int fingerY = Mathf.RoundToInt(center.y) - fingerH / 2;
        for (int y = fingerY; y < fingerY + fingerH && y < size; y++)
        {
            for (int x = fingerX; x < fingerX + fingerW && x < size; x++)
            {
                if (x >= 0 && y >= 0) pixels[y * size + x] = white;
            }
        }

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
    /// Tạo 2 GameObject con chứa SpriteRenderer cho Ngón tay và Loa phát thanh
    /// </summary>
    private void CreatePromptObjects()
    {
        if (globalPromptHolder == null)
        {
            GameObject holderGo = GameObject.Find("_ExhibitionFingerPrompts");
            if (holderGo == null)
            {
                holderGo = new GameObject("_ExhibitionFingerPrompts");
            }
            globalPromptHolder = holderGo.transform;
        }

        // 1. Tạo hoặc nạp lại Icon Ngón Tay
        if (promptObj == null)
        {
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

            // Gắn collider và tương tác cho ngón tay
            SphereCollider col = promptObj.GetComponent<SphereCollider>();
            if (col == null)
            {
                col = promptObj.AddComponent<SphereCollider>();
                col.isTrigger = true;
            }
            col.radius = 0.12f / Mathf.Max(fingerScale, 0.001f);

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

            promptObj.SetActive(false);
        }

        // 2. Tạo hoặc nạp lại Icon Chiếc Loa Thuyết Minh
        if (speakerPromptObj == null)
        {
            string speakerName = $"SpeakerPrompt_{gameObject.name}";
            Transform existingSpk = globalPromptHolder.Find(speakerName);
            if (existingSpk != null)
            {
                speakerPromptObj = existingSpk.gameObject;
                speakerPromptRenderer = speakerPromptObj.GetComponent<SpriteRenderer>();
            }
            else
            {
                speakerPromptObj = new GameObject(speakerName);
                speakerPromptObj.transform.SetParent(globalPromptHolder, false);
                speakerPromptRenderer = speakerPromptObj.AddComponent<SpriteRenderer>();
            }

            if (speakerSprite != null && speakerPromptRenderer != null)
            {
                speakerPromptRenderer.sprite = speakerSprite;
            }

            if (speakerPromptRenderer != null)
            {
                speakerPromptRenderer.sharedMaterial = GetFingerSpriteMaterial();
                speakerPromptRenderer.color = new Color(iconColor.r, iconColor.g, iconColor.b, 0f);
                speakerPromptRenderer.sortingOrder = 100;
            }

            // Gắn collider và tương tác cho icon loa (click laser VR)
            SphereCollider spkCol = speakerPromptObj.GetComponent<SphereCollider>();
            if (spkCol == null)
            {
                spkCol = speakerPromptObj.AddComponent<SphereCollider>();
                spkCol.isTrigger = true;
            }
            spkCol.radius = 0.12f / Mathf.Max(fingerScale, 0.001f);

            var spkInteractable = speakerPromptObj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            if (spkInteractable == null)
            {
                spkInteractable = speakerPromptObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            }
            spkInteractable.selectEntered.RemoveAllListeners();
            spkInteractable.selectEntered.AddListener((args) =>
            {
                PlayVoiceNarration();
            });

            speakerPromptObj.SetActive(false);
        }

        // 3. AudioSource phát thuyết minh
        if (narrationAudioSource == null)
        {
            narrationAudioSource = GetComponent<AudioSource>();
            if (narrationAudioSource == null)
            {
                narrationAudioSource = gameObject.AddComponent<AudioSource>();
            }
            narrationAudioSource.playOnAwake = false;
            narrationAudioSource.loop = false;
            narrationAudioSource.spatialBlend = 0f; // 2D âm thanh stereo trong trẻo hai tai, điều khiển âm lượng theo khoảng cách
        }
    }

    void OnDisable()
    {
        if (promptObj != null) promptObj.SetActive(false);
        if (speakerPromptObj != null) speakerPromptObj.SetActive(false);
        if (narrationAudioSource != null && narrationAudioSource.isPlaying)
        {
            narrationAudioSource.Stop();
            isNarrationPlaying = false;
        }
    }

    void OnDestroy()
    {
        if (promptObj != null) Destroy(promptObj);
        if (speakerPromptObj != null) Destroy(speakerPromptObj);
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
    /// </summary>
    private Transform GetActivePlayerCamera()
    {
        if (playerCam != null && playerCam.gameObject.activeInHierarchy)
        {
            return playerCam;
        }

        var fps = FindObjectOfType<FPSController>();
        if (fps != null && fps.gameObject.activeInHierarchy && fps.playerCamera != null && fps.playerCamera.gameObject.activeInHierarchy)
        {
            playerCam = fps.playerCamera;
            return playerCam;
        }

        var xr = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (xr != null && xr.gameObject.activeInHierarchy && xr.Camera != null && xr.Camera.gameObject.activeInHierarchy)
        {
            playerCam = xr.Camera.transform;
            return playerCam;
        }

        if (Camera.main != null && Camera.main.gameObject.activeInHierarchy)
        {
            playerCam = Camera.main.transform;
            return playerCam;
        }

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
    /// Kích hoạt mở bảng thuyết minh hình ảnh (gọi từ nút Shift hoặc tương tác VR)
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

    /// <summary>
    /// Bật hoặc tắt giọng đọc thuyết minh khi bấm phím V (hoặc tương tác VR).
    /// Nhấn V sẽ không hiển thị thêm bất kỳ giao diện nào.
    /// </summary>
    public void PlayVoiceNarration()
    {
        EnsureVoiceNarration();

        if (voiceNarration == null)
        {
            Debug.LogWarning($"[CubeExhibitionTrigger] Chưa có file ghi âm thuyết minh cho '{gameObject.name}' ({ethnicTitle})!");
            return;
        }

        if (narrationAudioSource == null)
        {
            narrationAudioSource = GetComponent<AudioSource>();
            if (narrationAudioSource == null) narrationAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (isNarrationPlaying && narrationAudioSource.isPlaying)
        {
            // Nhấn V lần nữa khi đang nghe -> Tắt
            narrationAudioSource.Stop();
            isNarrationPlaying = false;
        }
        else
        {
            // Bắt đầu phát giọng đọc thuyết minh
            narrationAudioSource.clip = voiceNarration;
            narrationAudioSource.volume = 1f;
            narrationAudioSource.Play();
            isNarrationPlaying = true;
            VRInputHelper.TriggerHapticBoth(0.35f, 0.08f);
        }
    }

    void Update()
    {
        playerCam = GetActivePlayerCamera();
        if (playerCam == null) return;

        if (baseTopCenter == Vector3.zero)
        {
            UpdateCubeTopPosition();
        }

        // Tính khoảng cách từ người chơi đến bệ
        Vector3 camPos = playerCam.position;
        float horizontalDist = Vector2.Distance(new Vector2(camPos.x, camPos.z), new Vector2(baseTopCenter.x, baseTopCenter.z));
        float verticalDist = Mathf.Abs(camPos.y - baseTopCenter.y);
        
        float effectiveDist = Vector3.Distance(camPos, baseTopCenter);
        isPlayerNear = (effectiveDist <= Mathf.Max(triggerDistance, 3.2f)) || (horizontalDist <= 2.8f && verticalDist <= 2.5f);

        // Hiệu ứng mờ dần (Fade In / Fade Out)
        float targetAlpha = isPlayerNear ? 1f : 0f;
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * 6f);

        // --- 1. HIỂN THỊ CẢ 2 BIỂU TƯỢNG (NGÓN TAY VÀ LOA NẰM TÁCH RỜI TẠI CẠNH DƯỚI CỦA CUBE) ---
        if (currentAlpha > 0.001f)
        {
            if (promptObj != null && !promptObj.activeSelf) promptObj.SetActive(true);
            if (speakerPromptObj != null && !speakerPromptObj.activeSelf) speakerPromptObj.SetActive(true);

            // Xác định toạ độ cạnh dưới của Cube
            float cubeBottomY;
            float cubeTopY;
            Vector3 cubeCenter;
            Renderer rend = GetComponent<Renderer>();
            Collider cubeCol = GetComponent<Collider>();

            if (rend != null)
            {
                cubeBottomY = rend.bounds.min.y;
                cubeTopY = rend.bounds.max.y;
                cubeCenter = rend.bounds.center;
            }
            else if (cubeCol != null)
            {
                cubeBottomY = cubeCol.bounds.min.y;
                cubeTopY = cubeCol.bounds.max.y;
                cubeCenter = cubeCol.bounds.center;
            }
            else
            {
                cubeBottomY = transform.position.y - transform.lossyScale.y * 0.5f;
                cubeTopY = transform.position.y + transform.lossyScale.y * 0.5f;
                cubeCenter = transform.position;
            }

            // Độ cao tại cạnh dưới của Cube (cách mặt đất / đáy một khoảng heightFromBottom)
            float targetY = cubeBottomY + heightFromBottom;
            float midY = (cubeBottomY + cubeTopY) * 0.5f;
            if (targetY > midY)
            {
                targetY = Mathf.Lerp(cubeBottomY, cubeTopY, 0.2f);
            }

            // Hướng từ tâm Cube tới camera người chơi (phương nằm ngang)
            Vector3 toCam = playerCam.position - cubeCenter;
            toCam.y = 0f;
            Vector3 dirToCam = toCam.sqrMagnitude > 0.001f ? toCam.normalized : transform.forward;

            // Điểm trên bề mặt cạnh dưới của Cube quay về phía người chơi
            Vector3 iconCenter;
            if (cubeCol != null)
            {
                Vector3 testPoint = new Vector3(playerCam.position.x, targetY, playerCam.position.z);
                Vector3 surfacePoint = cubeCol.ClosestPoint(testPoint);
                surfacePoint.y = targetY;
                iconCenter = surfacePoint + dirToCam * forwardOffset;
            }
            else if (rend != null)
            {
                float halfExt = Mathf.Max(rend.bounds.extents.x, rend.bounds.extents.z);
                iconCenter = new Vector3(cubeCenter.x, targetY, cubeCenter.z) + dirToCam * (halfExt + forwardOffset);
            }
            else
            {
                float halfExt = Mathf.Max(transform.lossyScale.x, transform.lossyScale.z) * 0.5f;
                iconCenter = new Vector3(cubeCenter.x, targetY, cubeCenter.z) + dirToCam * (halfExt + forwardOffset);
            }

            // Trục ngang vuông góc với hướng nhìn người chơi (song song với mặt đất)
            Vector3 camRight = playerCam.right;
            camRight.y = 0f;
            if (camRight.sqrMagnitude > 0.001f)
            {
                camRight.Normalize();
            }
            else
            {
                camRight = Vector3.Cross(Vector3.up, dirToCam).normalized;
            }

            // Tính toán chuyển động nhấp nhô của ngón tay
            float cycle = (Time.time * tapSpeed) % (Mathf.PI * 2f);
            float tapMotion = Mathf.Max(0f, Mathf.Sin(cycle));
            tapMotion = Mathf.Pow(tapMotion, 1.8f);
            float fingerAnimOffset = -(tapMotion * tapDepth);

            // Icon 1: Ngón tay trỏ nằm bên trái (cách tâm iconSpacing mét)
            if (promptObj != null)
            {
                promptObj.transform.position = iconCenter - camRight * iconSpacing + Vector3.up * fingerAnimOffset;
                promptObj.transform.rotation = playerCam.rotation;
                promptObj.transform.localScale = Vector3.one * fingerScale;

                if (promptRenderer != null)
                {
                    if (fingerSprite != null && promptRenderer.sprite != fingerSprite) promptRenderer.sprite = fingerSprite;
                    if (promptRenderer.sharedMaterial == null) promptRenderer.sharedMaterial = GetFingerSpriteMaterial();
                    promptRenderer.color = new Color(iconColor.r, iconColor.g, iconColor.b, currentAlpha);
                }
            }

            // Icon 2: Chiếc loa thuyết minh nằm bên phải (cách tâm iconSpacing mét)
            if (speakerPromptObj != null)
            {
                speakerPromptObj.transform.position = iconCenter + camRight * iconSpacing;
                speakerPromptObj.transform.rotation = playerCam.rotation;

                // Hiệu ứng nhịp đập nhẹ của loa khi đang phát âm thanh
                float pulse = (isNarrationPlaying && narrationAudioSource != null && narrationAudioSource.isPlaying) ?
                    (1f + Mathf.Sin(Time.time * 8f) * 0.08f) : 1f;
                speakerPromptObj.transform.localScale = Vector3.one * (fingerScale * pulse);

                if (speakerPromptRenderer != null)
                {
                    if (speakerSprite != null && speakerPromptRenderer.sprite != speakerSprite) speakerPromptRenderer.sprite = speakerSprite;
                    if (speakerPromptRenderer.sharedMaterial == null) speakerPromptRenderer.sharedMaterial = GetFingerSpriteMaterial();

                    // Khi đang phát, icon loa hơi sáng vàng nổi bật
                    Color spkColor = (isNarrationPlaying && narrationAudioSource != null && narrationAudioSource.isPlaying) ?
                        new Color(1f, 0.95f, 0.5f, currentAlpha) : new Color(iconColor.r, iconColor.g, iconColor.b, currentAlpha);
                    speakerPromptRenderer.color = spkColor;
                }
            }
        }
        else
        {
            if (promptObj != null && promptObj.activeSelf) promptObj.SetActive(false);
            if (speakerPromptObj != null && speakerPromptObj.activeSelf) speakerPromptObj.SetActive(false);
        }

        // --- 2. QUẢN LÝ ÂM LƯỢNG GIỌNG ĐỌC KHI ĐI XA / LẠI GẦN ---
        if (isNarrationPlaying && narrationAudioSource != null)
        {
            if (!narrationAudioSource.isPlaying)
            {
                // File âm thanh đã chạy hết tự nhiên
                isNarrationPlaying = false;
            }
            else
            {
                // Khi người chơi đi ra xa, âm lượng nhỏ dần
                float fadeStart = Mathf.Max(triggerDistance, 3.2f);
                float fadeEnd = Mathf.Max(maxAudibleDistance, fadeStart + 2.0f);

                if (effectiveDist <= fadeStart)
                {
                    narrationAudioSource.volume = 1f;
                }
                else if (effectiveDist <= fadeEnd)
                {
                    float t = (effectiveDist - fadeStart) / (fadeEnd - fadeStart);
                    narrationAudioSource.volume = Mathf.Clamp01(1f - t);
                }
                else
                {
                    // Đi ra quá xa -> TẮT HẲN!
                    narrationAudioSource.Stop();
                    isNarrationPlaying = false;
                    narrationAudioSource.volume = 1f; // reset volume cho lần nhấn sau
                    // LƯU Ý: isNarrationPlaying lúc này = false, nên khi lại gần sẽ KHÔNG tự to lại, người chơi phải bấm V lại.
                }
            }
        }

        // --- 3. PHÍM V: BẬT / TẮT GIỌNG THUYẾT MINH (KHÔNG HIỂN THỊ GÌ THÊM) ---
        bool audioAction = Input.GetKeyDown(KeyCode.V) || VRInputHelper.IsSecondaryButtonDown();
        if (isPlayerNear && audioAction)
        {
            if (IsClosestTriggerToPlayer())
            {
                PlayVoiceNarration();
            }
        }

        // --- 4. PHÍM SHIFT: MỞ POPUP CARD HÌNH ẢNH ---
        bool popupAction = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) ||
                           Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) ||
                           VRInputHelper.IsTriggerDown() || VRInputHelper.IsPrimaryButtonDown();

        if (MuseumPopupManager.Instance != null && !MuseumPopupManager.Instance.IsOpen)
        {
            if (isPlayerNear && popupAction)
            {
                if (Time.unscaledTime - MuseumPopupManager.lastToggleTime > 0.22f)
                {
                    if (IsClosestTriggerToPlayer())
                    {
                        TriggerInteraction();
                    }
                }
            }
        }
        else if (!isPlayerNear && MuseumPopupManager.Instance != null && MuseumPopupManager.Instance.IsOpen)
        {
            if (MuseumPopupManager.Instance.CurrentTrigger == this &&
                effectiveDist > triggerDistance * 1.5f)
            {
                MuseumPopupManager.Instance.ClosePopup();
            }
        }
    }

    /// <summary>
    /// Kiểm tra xem bệ này có phải là bệ gần camera người chơi nhất hay không
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

        Renderer rend = GetComponent<Renderer>();
        Collider col = GetComponent<Collider>();
        float cubeBottomY = rend != null ? rend.bounds.min.y : (col != null ? col.bounds.min.y : transform.position.y);
        Vector3 gizmoCenter = transform.position;
        gizmoCenter.y = cubeBottomY + heightFromBottom;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(gizmoCenter, 0.08f);
    }
}

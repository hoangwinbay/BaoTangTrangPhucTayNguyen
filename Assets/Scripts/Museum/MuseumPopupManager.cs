using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MuseumPopupManager : MonoBehaviour
{
    public static MuseumPopupManager Instance { get; private set; }

    [Header("Trạng thái hiện tại")]
    public bool IsOpen { get; private set; } = false;
    public CubeExhibitionTrigger CurrentTrigger { get; private set; }

    [Header("Cài đặt hiệu ứng")]
    public float animDuration = 0.22f;

    private Canvas popupCanvas;
    private CanvasGroup canvasGroup;
    private RectTransform cardContainer;
    private RawImage displayRawImage;
    private Text placeholderText;
    private Text hintText;
    private FPSController cachedFPSController;
    private Coroutine currentAnimRoutine;

    public static float lastToggleTime = -10f;
    public static int lastOpenFrame = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoInit()
    {
        if (Instance == null && FindObjectOfType<MuseumPopupManager>() == null)
        {
            GameObject go = new GameObject("MuseumPopupManager");
            go.AddComponent<MuseumPopupManager>();
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

        EnsurePopupUI();
    }

    void Start()
    {
        cachedFPSController = FindObjectOfType<FPSController>();
        AutoDetectExhibitionCubes();
    }

    private static readonly (string cubeName, string ethnicName, string baseFileName)[] DefaultCubeConfigs = new[]
    {
        ("Cube (56)", "Dân tộc Ê Đê", "03c4a4d6-db18-4535-b5ba-73cf309ba1f9"),
        ("Cube (58)", "Dân tộc Mạ", "058d668e-a1aa-4989-a44f-6466fbb623e9"),
        ("Cube (74)", "Dân tộc Xtiêng", "14804e7a-967b-4de9-874a-382bc4b43e2a"),
        ("Cube (76)", "Dân tộc Ba Na", "1dc2cf1f-804f-4c1c-aa44-407a9c40a64e"),
        ("Cube (78)", "Dân tộc Chư-Ru", "363eb253-0c6d-41a3-8ef1-1f420161e13a"),
        ("Cube (60)", "Dân tộc M'Nông", "9b7b7714-f545-4978-bb30-53f288981096"),
        ("Cube (64)", "Dân tộc Gia Rai", "9d5939b0-a28b-4845-bb7e-cd057c0eae57"),
        ("Cube (66)", "Dân tộc Brâu", "1e6aeefd-1896-4305-b7dd-b1c69ece1693"),
        ("Cube (68)", "Dân tộc Cơ Ho", "ebbd89d0-4dd7-4790-97e1-9a32a5c06a05"),
        ("Cube (70)", "Dân tộc Xơ Đăng", "a0904cda-3732-4c7b-955a-0f182ed8e3fd"),
        ("Cube (72)", "Dân tộc Giẻ Triêng", "e335c1b3-fe38-496b-97f1-f693b2896c37"),
        ("Cube (62)", "Dân tộc Rơ Măm", "314ca509-2fb7-487a-b3ba-7c9b2876e599")
    };

    public void AutoDetectExhibitionCubes()
    {
        string imageFolder = System.IO.Path.Combine(Application.dataPath, "3d/ảnh chi tiết");

        foreach (var item in DefaultCubeConfigs)
        {
            GameObject go = GameObject.Find(item.cubeName);
            if (go != null)
            {
                var trigger = go.GetComponent<CubeExhibitionTrigger>();
                if (trigger == null)
                {
                    trigger = go.AddComponent<CubeExhibitionTrigger>();
                }

                if (string.IsNullOrEmpty(trigger.ethnicTitle) || trigger.ethnicTitle.StartsWith("DÂN TỘC TÂY NGUYÊN"))
                {
                    trigger.ethnicTitle = item.ethnicName;
                }

                if (trigger.detailTexture == null && trigger.detailSprite == null)
                {
                    string fullPath = System.IO.Path.Combine(imageFolder, item.baseFileName + ".png");
                    if (!System.IO.File.Exists(fullPath))
                    {
                        fullPath = System.IO.Path.Combine(imageFolder, item.baseFileName + ".jpg");
                    }

                    if (System.IO.File.Exists(fullPath))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        if (tex.LoadImage(bytes))
                        {
                            tex.filterMode = FilterMode.Bilinear;
                            trigger.detailTexture = tex;
                        }
                    }
                }
            }
        }
    }

    private void EnsurePopupUI()
    {
        if (popupCanvas != null)
        {
            Transform oldBorder = popupCanvas.transform.Find("CardContainer/CardBorder");
            Transform oldTitle = popupCanvas.transform.Find("CardContainer/TitleText");
            if (oldBorder != null || oldTitle != null)
            {
                DestroyImmediate(popupCanvas.gameObject);
                popupCanvas = null;
            }
        }

        if (popupCanvas != null) return;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name == "MuseumPopupCanvas")
            {
                DestroyImmediate(child.gameObject);
            }
        }

        GameObject canvasObj = new GameObject("MuseumPopupCanvas");
        canvasObj.transform.SetParent(transform);
        popupCanvas = canvasObj.AddComponent<Canvas>();
        popupCanvas.sortingOrder = 999;

        var scaler = canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        if (canvasObj.GetComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>() == null)
        {
            canvasObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster>();
        }

        bool isVR = VRInputHelper.IsVREnvironment();
        if (isVR)
        {
            popupCanvas.renderMode = RenderMode.WorldSpace;
            popupCanvas.worldCamera = Camera.main;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = 1f;
            popupCanvas.transform.localScale = Vector3.one * 0.0016f;
        }
        else
        {
            popupCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        canvasGroup = canvasObj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        GameObject dimmerObj = new GameObject("DimmerOverlay");
        dimmerObj.transform.SetParent(canvasObj.transform, false);
        RectTransform dimmerRect = dimmerObj.AddComponent<RectTransform>();
        dimmerRect.anchorMin = Vector2.zero;
        dimmerRect.anchorMax = Vector2.one;
        dimmerRect.offsetMin = Vector2.zero;
        dimmerRect.offsetMax = Vector2.zero;

        Image dimmerImg = dimmerObj.AddComponent<Image>();
        dimmerImg.color = new Color(0f, 0f, 0f, 0.45f);

        Button dimmerBtn = dimmerObj.AddComponent<Button>();
        dimmerBtn.transition = Selectable.Transition.None;
        dimmerBtn.onClick.AddListener(ClosePopup);

        GameObject cardObj = new GameObject("CardContainer");
        cardObj.transform.SetParent(canvasObj.transform, false);
        cardContainer = cardObj.AddComponent<RectTransform>();
        cardContainer.anchorMin = new Vector2(0.5f, 0.5f);
        cardContainer.anchorMax = new Vector2(0.5f, 0.5f);
        cardContainer.pivot = new Vector2(0.5f, 0.5f);
        cardContainer.sizeDelta = new Vector2(1500, 1000);

        GameObject rawImgObj = new GameObject("DetailImage");
        rawImgObj.transform.SetParent(cardObj.transform, false);
        RectTransform rawRect = rawImgObj.AddComponent<RectTransform>();
        rawRect.anchorMin = Vector2.zero;
        rawRect.anchorMax = Vector2.one;
        rawRect.offsetMin = Vector2.zero;
        rawRect.offsetMax = Vector2.zero;

        displayRawImage = rawImgObj.AddComponent<RawImage>();
        var aspect = rawImgObj.AddComponent<AspectRatioFitter>();
        aspect.aspectRatio = 1.5f;
        aspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent;

        GameObject placeObj = new GameObject("PlaceholderText");
        placeObj.transform.SetParent(cardObj.transform, false);
        RectTransform placeRect = placeObj.AddComponent<RectTransform>();
        placeRect.anchorMin = Vector2.zero;
        placeRect.anchorMax = Vector2.one;
        placeRect.offsetMin = new Vector2(40, 40);
        placeRect.offsetMax = new Vector2(-40, -40);

        placeholderText = placeObj.AddComponent<Text>();
        placeholderText.font = defaultFont;
        placeholderText.fontSize = 26;
        placeholderText.alignment = TextAnchor.MiddleCenter;
        placeholderText.color = new Color(0.85f, 0.75f, 0.6f, 1f);
        placeholderText.text = "Chưa gán ảnh thuyết minh cho bệ này.\n(Kéo ảnh vào ô Detail Texture trong Unity Inspector)";
        placeholderText.gameObject.SetActive(false);

        GameObject hintObj = new GameObject("HintPill");
        hintObj.transform.SetParent(canvasObj.transform, false);
        RectTransform hintRect = hintObj.AddComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0.5f, 0f);
        hintRect.anchorMax = new Vector2(0.5f, 0f);
        hintRect.pivot = new Vector2(0.5f, 0f);
        hintRect.anchoredPosition = new Vector2(0, 10);
        hintRect.sizeDelta = new Vector2(560, 32);

        Image hintBg = hintObj.AddComponent<Image>();
        hintBg.color = new Color(0.12f, 0.10f, 0.08f, 0.90f);

        Button hintBtn = hintObj.AddComponent<Button>();
        hintBtn.onClick.AddListener(ClosePopup);

        GameObject hintTextObj = new GameObject("HintText");
        hintTextObj.transform.SetParent(hintObj.transform, false);
        RectTransform htRect = hintTextObj.AddComponent<RectTransform>();
        htRect.anchorMin = Vector2.zero;
        htRect.anchorMax = Vector2.one;
        htRect.offsetMin = Vector2.zero;
        htRect.offsetMax = Vector2.zero;

        hintText = hintTextObj.AddComponent<Text>();
        hintText.font = defaultFont;
        hintText.fontSize = 14;
        hintText.fontStyle = FontStyle.Bold;
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.color = new Color(0.95f, 0.88f, 0.72f, 0.95f);
        hintText.text = isVR ? "⟵ Bấm Cò trỏ [ Trigger ] hoặc nút [ A ] trên tay cầm để thu lại ⟶" : "⟵ Bấm phím [ SHIFT ] hoặc [ ESC ] để thu lại ⟶";

        popupCanvas.gameObject.SetActive(false);
    }

    public void TogglePopup(CubeExhibitionTrigger trigger)
    {
        if (IsOpen)
        {
            ClosePopup();
        }
        else
        {
            ShowPopup(trigger);
        }
    }

    public void ShowPopup(CubeExhibitionTrigger trigger)
    {
        if (trigger == null) return;
        EnsurePopupUI();

        CurrentTrigger = trigger;
        IsOpen = true;
        lastToggleTime = Time.unscaledTime;
        lastOpenFrame = Time.frameCount;

        Texture targetTex = null;
        if (trigger.detailTexture != null)
        {
            targetTex = trigger.detailTexture;
        }
        else if (trigger.detailSprite != null)
        {
            targetTex = trigger.detailSprite.texture;
        }

        if (targetTex != null)
        {
            displayRawImage.texture = targetTex;
            displayRawImage.gameObject.SetActive(true);
            placeholderText.gameObject.SetActive(false);
        }
        else
        {
            displayRawImage.gameObject.SetActive(false);
            placeholderText.text = $"⚠️ Chưa gán ảnh cho {trigger.gameObject.name}\n\nHãy kéo ảnh từ Assets/3d/ảnh chi tiết vào ô 'Detail Texture' của Cube này trong Unity Inspector.";
            placeholderText.gameObject.SetActive(true);
        }

        bool isVR = VRInputHelper.IsVREnvironment();
        if (isVR && popupCanvas != null)
        {
            Transform camTrans = Camera.main != null ? Camera.main.transform : transform;
            Vector3 forward = camTrans.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.01f) forward = camTrans.forward;
            forward.Normalize();

            Vector3 targetPos = camTrans.position + forward * 2.2f + Vector3.up * 0.05f;
            popupCanvas.transform.position = targetPos;
            popupCanvas.transform.rotation = Quaternion.LookRotation(forward);
            popupCanvas.transform.localScale = Vector3.one * 0.0016f;

            if (hintText != null)
            {
                hintText.text = "⟵ Bấm Cò trỏ [ Trigger ] hoặc nút [ A ] trên tay cầm để thu lại ⟶";
            }
        }
        else if (hintText != null)
        {
            hintText.text = "⟵ Bấm phím [ SHIFT ] hoặc [ ESC ] để thu lại ⟶";
        }

        if (cachedFPSController == null) cachedFPSController = FindObjectOfType<FPSController>();
        if (cachedFPSController != null)
        {
            cachedFPSController.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (currentAnimRoutine != null) StopCoroutine(currentAnimRoutine);
        currentAnimRoutine = StartCoroutine(AnimateOpen());
    }

    public void ClosePopup()
    {
        if (!IsOpen) return;
        IsOpen = false;
        CurrentTrigger = null;
        lastToggleTime = Time.unscaledTime;

        if (cachedFPSController == null) cachedFPSController = FindObjectOfType<FPSController>();
        if (cachedFPSController != null)
        {
            cachedFPSController.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentAnimRoutine != null) StopCoroutine(currentAnimRoutine);
        currentAnimRoutine = StartCoroutine(AnimateClose());
    }

    private IEnumerator AnimateOpen()
    {
        popupCanvas.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animDuration);
            float smooth = Mathf.Sin(t * Mathf.PI * 0.5f);

            canvasGroup.alpha = smooth;
            cardContainer.localScale = Vector3.one * Mathf.Lerp(0.85f, 1f, smooth);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        cardContainer.localScale = Vector3.one;
    }

    private IEnumerator AnimateClose()
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < animDuration * 0.8f)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / (animDuration * 0.8f));

            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            cardContainer.localScale = Vector3.one * Mathf.Lerp(1f, 0.88f, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        popupCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!IsOpen) return;

        if (Time.frameCount == lastOpenFrame) return;

        if (Time.unscaledTime - lastToggleTime < 0.22f) return;

        if (VRInputHelper.IsTriggerDown() || VRInputHelper.IsPrimaryButtonDown() || VRInputHelper.IsAnyVRActionTriggered())
        {
            VRInputHelper.TriggerHapticBoth(0.35f, 0.08f);
            ClosePopup();
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) ||
            Input.GetKeyDown(KeyCode.RightShift) ||
            Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            ClosePopup();
        }
    }
}

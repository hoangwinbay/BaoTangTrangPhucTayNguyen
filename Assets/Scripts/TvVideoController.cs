using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

/// <summary>
/// Quản lý âm thanh không gian 3D và ánh sáng động chiếu sáng phòng cho màn hình Tivi trong Bảo tàng.
/// 1. Âm thanh 3D (Spatial Audio): Đi lại gần Tivi nghe to, rõ; lùi ra xa âm lượng giảm dần và tắt khi ra khỏi phòng.
/// 2. Ánh sáng chiếu sáng phòng (Ambilight): Tạo nguồn sáng phát quang từ màn hình Tivi chiếu sáng sàn,
///    bàn ghế sofa và không gian xung quanh; tự động đổi màu sắc và cường độ theo từng khung hình video.
/// </summary>
[ExecuteAlways]
[SelectionBase]
public class TvVideoController : MonoBehaviour
{
    [Header("=== THÀNH PHẦN KẾT NỐI ===")]
    [Tooltip("VideoPlayer đang phát nội dung trên Tivi")]
    public VideoPlayer videoPlayer;

    [Tooltip("AudioSource phát âm thanh 3D cho Tivi")]
    public AudioSource audioSource;

    [Tooltip("Nguồn sáng mô phỏng ánh sáng từ màn hình Tivi chiếu ra phòng")]
    public Light screenLight;

    [Header("=== CÀI ĐẶT ÂM THANH 3D (SPATIAL AUDIO) ===")]
    [Tooltip("Khoảng cách gần Tivi nghe âm lượng tối đa 100% (mét)")]
    [Range(0.5f, 5f)]
    public float minAudioDistance = 2.5f;

    [Tooltip("Khoảng cách tối đa mà âm thanh tắt hẳn khi đi ra xa (mét)")]
    [Range(8f, 30f)]
    public float maxAudioDistance = 16.0f;

    [Tooltip("Âm lượng tối đa khi đứng gần Tivi")]
    [Range(0f, 1f)]
    public float maxVolume = 1.0f;

    [Tooltip("Tỷ lệ âm thanh không gian 3D (1 = 100% 3D định hướng theo tai người nghe)")]
    [Range(0f, 1f)]
    public float spatialBlend = 1.0f;

    [Header("=== CÀI ĐẶT ÁNH SÁNG CHIẾU SÁNG PHÒNG (AMBILIGHT) ===")]
    [Tooltip("Bật tính năng ánh sáng màn hình tự động làm sáng phòng")]
    public bool enableRoomLighting = true;

    [Tooltip("Bật chế độ đổi màu và độ sáng đèn theo nội dung video")]
    public bool enableDynamicAmbilight = true;

    [Tooltip("Phạm vi chiếu sáng của đèn trong phòng (mét)")]
    [Range(5f, 25f)]
    public float lightRange = 13.0f;

    [Tooltip("Độ sáng cơ bản của đèn")]
    [Range(0.5f, 8f)]
    public float baseIntensity = 3.0f;

    [Tooltip("Độ sáng tối thiểu khi video vào khung cảnh tối")]
    [Range(0.2f, 3f)]
    public float minIntensity = 1.2f;

    [Tooltip("Độ sáng tối đa khi video vào khung cảnh rực rỡ")]
    [Range(1f, 8f)]
    public float maxIntensity = 4.2f;

    [Tooltip("Khoảng cách đưa đèn ra phía trước màn hình vào phòng (mét)")]
    public float lightOffsetForward = 0.45f;

    [Tooltip("Khoảng cách nâng đèn lên tâm màn hình (mét)")]
    public float lightOffsetUp = 0.15f;

    [Tooltip("Màu ánh sáng mặc định khi chưa lấy mẫu được video")]
    public Color defaultLightColor = new Color(0.85f, 0.92f, 1.0f);

    [Tooltip("Tăng cường độ bão hòa màu của ánh sáng để phòng có sắc thái sinh động")]
    [Range(1f, 2.5f)]
    public float saturationBoost = 1.35f;

    [Tooltip("Tốc độ chuyển màu mượt mà")]
    public float colorLerpSpeed = 5.0f;

    [Tooltip("Tốc độ chuyển độ sáng mượt mà")]
    public float intensityLerpSpeed = 5.0f;

    [Tooltip("Tần suất lấy mẫu màu video (giây) - 0.1s tương đương ~10 lần/giây, tối ưu hiệu năng VR")]
    public float sampleInterval = 0.1f;

    // Biến nội bộ phục vụ lấy mẫu màu hiệu năng cao (1x1 downsampling)
    private RenderTexture tinySampleRT;
    private Texture2D samplePixelTex;
    private float nextSampleTime = 0f;
    private Color targetLightColor;
    private float targetIntensity;
    private bool isReadbackPending = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoEnsureOnSceneLoad()
    {
        // Tự động tìm đối tượng Tivi trong scene nếu chưa được gắn script
        GameObject tvGo = GameObject.Find("tivi");
        if (tvGo == null)
        {
            // Tìm theo VideoPlayer nếu tên không phải "tivi"
            VideoPlayer vp = Object.FindObjectOfType<VideoPlayer>();
            if (vp != null) tvGo = vp.gameObject;
        }

        if (tvGo != null)
        {
            TvVideoController controller = tvGo.GetComponent<TvVideoController>();
            if (controller == null)
            {
                controller = tvGo.AddComponent<TvVideoController>();
            }
            controller.SetupAllComponents();
        }
    }

    private void Awake()
    {
        SetupAllComponents();
    }

    private void Start()
    {
        SetupAllComponents();
        InitAmbilightSampler();

        // Đảm bảo video đang phát được đồng bộ âm thanh
        if (videoPlayer != null)
        {
            videoPlayer.errorReceived += OnVideoError;
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
    }

    private void OnEnable()
    {
        SetupAllComponents();
    }

    private void OnDisable()
    {
        CleanupAmbilightSampler();
    }

    private void OnDestroy()
    {
        CleanupAmbilightSampler();
        if (videoPlayer != null)
        {
            videoPlayer.errorReceived -= OnVideoError;
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }

    /// <summary>
    /// Cấu hình toàn bộ VideoPlayer, AudioSource 3D và Nguồn sáng làm sáng phòng
    /// </summary>
    public void SetupAllComponents()
    {
        SetupVideoPlayer();
        SetupAudioSource();
        SetupScreenLight();
    }

    /// <summary>
    /// 1. Cấu hình VideoPlayer kết nối tới AudioSource
    /// </summary>
    public void SetupVideoPlayer()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                videoPlayer = GetComponentInChildren<VideoPlayer>();
            }
        }

        if (videoPlayer == null) return;

        // Chuyển chế độ âm thanh của VideoPlayer sang AudioSource
        if (videoPlayer.audioOutputMode != VideoAudioOutputMode.AudioSource)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        }

        // Kích hoạt Track 0
        videoPlayer.EnableAudioTrack(0, true);

        // Gán target AudioSource nếu đã có
        if (audioSource != null)
        {
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
    }

    /// <summary>
    /// 2. Cấu hình AudioSource 3D chuẩn bảo tàng (to khi lại gần, giảm dần khi ở xa)
    /// </summary>
    public void SetupAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Thiết lập âm thanh 3D định hướng không gian
        audioSource.spatialBlend = spatialBlend;
        audioSource.volume = maxVolume;
        audioSource.minDistance = minAudioDistance;
        audioSource.maxDistance = maxAudioDistance;
        audioSource.dopplerLevel = 0f; // Không méo cao độ khi player bước đi
        audioSource.spread = 45f;      // Tạo hiệu ứng âm thanh nổi bao quanh màn hình
        audioSource.playOnAwake = false;
        audioSource.loop = true;

        // Cài đặt đường cong suy giảm âm lượng mượt mà (Custom Rolloff Curve)
        // 0m - 2.5m: 100% âm lượng
        // 5m: 75%
        // 8m: 45%
        // 12m: 18%
        // 16m: 0% (tắt hẳn khi rời phòng chiếu)
        audioSource.rolloffMode = AudioRolloffMode.Custom;
        AnimationCurve curve = new AnimationCurve(
            new Keyframe(0f, 1.0f),
            new Keyframe(minAudioDistance, 1.0f, 0f, -0.08f),
            new Keyframe(minAudioDistance + (maxAudioDistance - minAudioDistance) * 0.25f, 0.72f),
            new Keyframe(minAudioDistance + (maxAudioDistance - minAudioDistance) * 0.50f, 0.40f),
            new Keyframe(minAudioDistance + (maxAudioDistance - minAudioDistance) * 0.75f, 0.16f),
            new Keyframe(maxAudioDistance, 0.0f, -0.05f, 0f)
        );
        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);

        // Liên kết lại với VideoPlayer
        if (videoPlayer != null)
        {
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
    }

    /// <summary>
    /// 3. Cấu hình nguồn sáng Point Light chiếu sáng phòng khách
    /// </summary>
    public void SetupScreenLight()
    {
        if (screenLight == null)
        {
            Transform lightTr = transform.Find("TvScreenLight");
            if (lightTr == null)
            {
                GameObject lightGo = new GameObject("TvScreenLight");
                lightGo.transform.SetParent(transform, false);
                screenLight = lightGo.AddComponent<Light>();
            }
            else
            {
                screenLight = lightTr.GetComponent<Light>();
                if (screenLight == null)
                {
                    screenLight = lightTr.gameObject.AddComponent<Light>();
                }
            }
        }

        if (screenLight != null)
        {
            screenLight.type = LightType.Point;
            screenLight.range = lightRange;
            screenLight.intensity = baseIntensity;
            screenLight.color = defaultLightColor;
            screenLight.shadows = LightShadows.None; // Tối ưu hiệu năng 90 FPS
            screenLight.enabled = enableRoomLighting;

            // Đặt vị trí nguồn sáng phía trước màn hình Tivi chiếu ra phòng
            UpdateLightPosition();
        }

        targetLightColor = defaultLightColor;
        targetIntensity = baseIntensity;
    }

    /// <summary>
    /// Cập nhật tọa độ của đèn đặt ngay trước bề mặt màn hình Tivi
    /// </summary>
    public void UpdateLightPosition()
    {
        if (screenLight == null) return;

        // Tính tâm màn hình từ Renderer bounds nếu có
        Renderer rend = GetComponentInChildren<Renderer>();
        Vector3 center = (rend != null) ? rend.bounds.center : transform.position;

        // Đưa đèn về phía trước theo hướng forward của Tivi
        Vector3 forwardDir = transform.forward;
        Vector3 targetPos = center + forwardDir * lightOffsetForward + Vector3.up * lightOffsetUp;

        screenLight.transform.position = targetPos;
    }

    private void Update()
    {
        if (!enableRoomLighting || screenLight == null) return;

        if (Application.isPlaying && enableDynamicAmbilight)
        {
            UpdateAmbilightSampling();
        }

        // Nội suy mượt mà màu sắc và cường độ ánh sáng
        screenLight.color = Color.Lerp(screenLight.color, targetLightColor, Time.deltaTime * colorLerpSpeed);
        screenLight.intensity = Mathf.Lerp(screenLight.intensity, targetIntensity, Time.deltaTime * intensityLerpSpeed);
        screenLight.range = lightRange;
    }

    private void InitAmbilightSampler()
    {
        if (!Application.isPlaying) return;

        if (tinySampleRT == null)
        {
            tinySampleRT = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGB32);
            tinySampleRT.filterMode = FilterMode.Bilinear;
            tinySampleRT.Create();
        }

        if (samplePixelTex == null)
        {
            samplePixelTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        }
    }

    private void CleanupAmbilightSampler()
    {
        isReadbackPending = false;
        if (tinySampleRT != null)
        {
            if (tinySampleRT.IsCreated()) tinySampleRT.Release();
            DestroyImmediate(tinySampleRT);
            tinySampleRT = null;
        }

        if (samplePixelTex != null)
        {
            DestroyImmediate(samplePixelTex);
            samplePixelTex = null;
        }
    }

    /// <summary>
    /// Lấy mẫu màu trung bình và độ sáng của video qua GPU hardware downsampling 1x1 bất đồng bộ (Zero CPU Stall)
    /// </summary>
    private void UpdateAmbilightSampling()
    {
        if (isReadbackPending) return;

        // Tối ưu khoảng cách: Nếu người chơi ở quá xa (> 25m), giảm tần suất lấy mẫu
        Transform camTr = Camera.main != null ? Camera.main.transform : null;
        if (camTr != null)
        {
            float distSq = (camTr.position - transform.position).sqrMagnitude;
            if (distSq > 625f) // > 25m
            {
                if (Time.time < nextSampleTime) return;
                nextSampleTime = Time.time + 1.0f; // Ở xa chỉ lấy mẫu 1 lần/giây
            }
        }

        if (Time.time < nextSampleTime) return;
        nextSampleTime = Time.time + sampleInterval;

        RenderTexture currentVideoRT = GetActiveVideoRenderTexture();
        if (currentVideoRT == null || !currentVideoRT.IsCreated()) return;

        if (tinySampleRT == null)
        {
            InitAmbilightSampler();
            if (tinySampleRT == null) return;
        }

        try
        {
            // Downsample toàn bộ khung hình video về đúng 1 pixel (GPU bilinear filter tự động tính trung bình màu)
            Graphics.Blit(currentVideoRT, tinySampleRT);

            if (SystemInfo.supportsAsyncGPUReadback)
            {
                isReadbackPending = true;
                AsyncGPUReadback.Request(tinySampleRT, 0, (AsyncGPUReadbackRequest req) =>
                {
                    isReadbackPending = false;
                    if (this == null || !enabled || tinySampleRT == null) return;
                    if (req.hasError) return;

                    var data = req.GetData<Color32>();
                    if (data.Length > 0)
                    {
                        Color32 c = data[0];
                        Color rawColor = new Color(c.r / 255f, c.g / 255f, c.b / 255f, c.a / 255f);
                        ProcessSampledColor(rawColor);
                    }
                });
            }
            else
            {
                // Fallback dự phòng nếu thiết bị cũ không hỗ trợ AsyncGPUReadback
                if (samplePixelTex == null) samplePixelTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                RenderTexture prevActive = RenderTexture.active;
                RenderTexture.active = tinySampleRT;
                samplePixelTex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
                samplePixelTex.Apply();
                RenderTexture.active = prevActive;

                Color rawColor = samplePixelTex.GetPixel(0, 0);
                ProcessSampledColor(rawColor);
            }
        }
        catch
        {
            isReadbackPending = false;
        }
    }

    private void ProcessSampledColor(Color rawColor)
    {
        // Tính độ sáng (Luminance) của khung hình video
        float luminance = 0.299f * rawColor.r + 0.587f * rawColor.g + 0.114f * rawColor.b;

        // Tăng cường nhẹ độ bão hòa để ánh sáng hắt vào phòng có sắc thái ấm cúng, sống động
        Color.RGBToHSV(rawColor, out float h, out float s, out float v);
        s = Mathf.Clamp01(s * saturationBoost);
        v = Mathf.Max(v, 0.2f); // Đảm bảo không bị tối hoàn toàn
        Color vibrantColor = Color.HSVToRGB(h, s, v);

        // Hòa trộn nhẹ với màu trắng sáng tự nhiên để tạo cảm giác ánh sáng đèn phòng dịu mắt
        targetLightColor = Color.Lerp(vibrantColor, Color.white, 0.25f);

        // Điều tiết cường độ đèn theo độ sáng cảnh phim
        targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.Clamp01(luminance * 1.3f));
    }

    private RenderTexture GetActiveVideoRenderTexture()
    {
        if (videoPlayer != null && videoPlayer.targetTexture != null)
        {
            return videoPlayer.targetTexture;
        }
        return null;
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        if (audioSource != null && videoPlayer != null)
        {
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.LogWarning($"[TvVideoController] VideoPlayer thông báo: {message}");
    }

    private void OnValidate()
    {
        // Đồng bộ các thông số ngay lập tức khi người dùng chỉnh sửa trong Inspector
        if (audioSource != null)
        {
            audioSource.spatialBlend = spatialBlend;
            audioSource.volume = maxVolume;
            audioSource.minDistance = minAudioDistance;
            audioSource.maxDistance = maxAudioDistance;
        }

        if (screenLight != null)
        {
            screenLight.enabled = enableRoomLighting;
            screenLight.range = lightRange;
            if (!Application.isPlaying)
            {
                screenLight.intensity = baseIntensity;
                screenLight.color = defaultLightColor;
                UpdateLightPosition();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị trực quan bán kính âm thanh và ánh sáng trong cửa sổ Scene
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.7f);
        Gizmos.DrawWireSphere(transform.position, minAudioDistance);

        Gizmos.color = new Color(1f, 0.6f, 0.1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, maxAudioDistance);

        if (screenLight != null)
        {
            Gizmos.color = new Color(0.3f, 0.7f, 1.0f, 0.5f);
            Gizmos.DrawWireSphere(screenLight.transform.position, lightRange);
            Gizmos.DrawIcon(screenLight.transform.position, "Light Gizmo.png", true);
        }
    }
}

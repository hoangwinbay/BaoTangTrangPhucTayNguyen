using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

[ExecuteAlways]
[SelectionBase]
public class TvVideoController : MonoBehaviour
{
    [Header("=== THÀNH PHẦN KẾT NỐI ===")]
    [Tooltip("VideoPlayer đang phát nội dung trên Tivi")]
    public VideoPlayer videoPlayer;

    [Tooltip("Kéo file VideoClip mới vào đây nếu muốn đổi video phát trên Tivi qua Code/Inspector")]
    public VideoClip customVideoClip;

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

    private RenderTexture tinySampleRT;
    private Texture2D samplePixelTex;
    private float nextSampleTime = 0f;
    private Color targetLightColor;
    private float targetIntensity;
    private bool isReadbackPending = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoEnsureOnSceneLoad()
    {
        GameObject tvGo = GameObject.Find("tivi");
        if (tvGo == null)
        {
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

    public void SetupAllComponents()
    {
        SetupVideoPlayer();
        SetupAudioSource();
        SetupScreenLight();
    }

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

        if (customVideoClip != null && videoPlayer.clip != customVideoClip)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = customVideoClip;
        }

        if (videoPlayer.audioOutputMode != VideoAudioOutputMode.AudioSource)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        }

        videoPlayer.EnableAudioTrack(0, true);

        if (audioSource != null)
        {
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
    }

    public void ChangeVideo(VideoClip newClip)
    {
        if (videoPlayer == null) SetupVideoPlayer();
        if (videoPlayer == null || newClip == null) return;

        customVideoClip = newClip;
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = newClip;
        videoPlayer.Play();
    }

    public void ChangeVideoUrl(string url)
    {
        if (videoPlayer == null) SetupVideoPlayer();
        if (videoPlayer == null || string.IsNullOrEmpty(url)) return;

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = url;
        videoPlayer.Play();
    }

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

        audioSource.spatialBlend = spatialBlend;
        audioSource.volume = maxVolume;
        audioSource.minDistance = minAudioDistance;
        audioSource.maxDistance = maxAudioDistance;
        audioSource.dopplerLevel = 0f;
        audioSource.spread = 45f;
        audioSource.playOnAwake = false;
        audioSource.loop = true;

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

        if (videoPlayer != null)
        {
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
    }

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
            screenLight.shadows = LightShadows.None;
            screenLight.enabled = enableRoomLighting;

            UpdateLightPosition();
        }

        targetLightColor = defaultLightColor;
        targetIntensity = baseIntensity;
    }

    public void UpdateLightPosition()
    {
        if (screenLight == null) return;

        Renderer rend = GetComponentInChildren<Renderer>();
        Vector3 center = (rend != null) ? rend.bounds.center : transform.position;

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

    private void UpdateAmbilightSampling()
    {
        if (isReadbackPending) return;

        Transform camTr = Camera.main != null ? Camera.main.transform : null;
        if (camTr != null)
        {
            float distSq = (camTr.position - transform.position).sqrMagnitude;
            if (distSq > 625f)
            {
                if (Time.time < nextSampleTime) return;
                nextSampleTime = Time.time + 1.0f;
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
        float luminance = 0.299f * rawColor.r + 0.587f * rawColor.g + 0.114f * rawColor.b;

        Color.RGBToHSV(rawColor, out float h, out float s, out float v);
        s = Mathf.Clamp01(s * saturationBoost);
        v = Mathf.Max(v, 0.2f);
        Color vibrantColor = Color.HSVToRGB(h, s, v);

        targetLightColor = Color.Lerp(vibrantColor, Color.white, 0.25f);

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

using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MuseumAudioManager : MonoBehaviour
{
    private static MuseumAudioManager _instance;
    public static MuseumAudioManager Instance => _instance;

    [Header("1. Âm thanh nền (Background Music)")]
    [Tooltip("Kéo file nhạc nền Tây Nguyên vào đây (Nếu để trống sẽ tự nạp từ Assets/Resources/Audio/NhacNen_TayNguyen)")]
    public AudioClip backgroundMusicClip;

    [Range(0f, 1f)]
    [Tooltip("Âm lượng chuẩn khi dạo quanh bảo tàng (mặc định 0.22 - êm dịu, không chói tai)")]
    public float baseVolume = 0.22f;

    [Range(0f, 1f)]
    [Tooltip("Âm lượng nền khi có thuyết minh hiện vật đang phát (Audio Ducking)")]
    public float duckedVolume = 0.05f;

    [Tooltip("Tốc độ chuyển đổi âm lượng (fade in/out) mượt mà")]
    public float fadeSpeed = 2.0f;

    [Header("2. Phím tắt tiện ích")]
    [Tooltip("Phím tắt bàn phím để Bật/Tắt (Mute) âm thanh nền")]
    public KeyCode toggleMuteKey = KeyCode.M;

    private AudioSource bgmSource;
    private bool isMuted = false;
    private float targetVolume = 0.22f;

    private CubeExhibitionTrigger[] cachedTriggers;
    private float nextTriggerCheckTime = 0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        if (_instance == null)
        {
            MuseumAudioManager existing = FindObjectOfType<MuseumAudioManager>();
            if (existing == null)
            {
                GameObject audioManagerGO = new GameObject("[MuseumAudioManager]");
                _instance = audioManagerGO.AddComponent<MuseumAudioManager>();
                DontDestroyOnLoad(audioManagerGO);
                Debug.Log("[MuseumAudioManager] Đã tự động khởi tạo hệ thống âm thanh nền cho bảo tàng.");
            }
            else
            {
                _instance = existing;
            }
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSource();
    }

    private void Start()
    {
        if (backgroundMusicClip == null)
        {
            backgroundMusicClip = Resources.Load<AudioClip>("Audio/NhacNen_TayNguyen");
            if (backgroundMusicClip == null)
            {
                backgroundMusicClip = Resources.Load<AudioClip>("Audio/nhacnen_taynguyen");
            }
        }

        if (backgroundMusicClip != null)
        {
            bgmSource.clip = backgroundMusicClip;
            bgmSource.volume = baseVolume;
            targetVolume = baseVolume;
            bgmSource.Play();
            Debug.Log($"[MuseumAudioManager] Đang phát nhạc nền: '{backgroundMusicClip.name}' (Độ dài: {backgroundMusicClip.length:F1}s).");
        }
        else
        {
            Debug.LogWarning("[MuseumAudioManager] Không tìm thấy file âm thanh nền tại 'Resources/Audio/NhacNen_TayNguyen'!");
        }

        RefreshTriggerCache();
    }

    private void SetupAudioSource()
    {
        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.loop = true;
        bgmSource.playOnAwake = true;
        bgmSource.spatialBlend = 0f;
        bgmSource.priority = 64;
        bgmSource.volume = baseVolume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleMuteKey))
        {
            ToggleMute();
        }

        bool isNarrationActive = CheckIfAnyNarrationPlaying();

        if (isMuted)
        {
            targetVolume = 0f;
        }
        else if (isNarrationActive)
        {
            targetVolume = duckedVolume;
        }
        else
        {
            targetVolume = baseVolume;
        }

        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.volume = Mathf.MoveTowards(bgmSource.volume, targetVolume, fadeSpeed * Time.unscaledDeltaTime);
        }
    }

    private bool CheckIfAnyNarrationPlaying()
    {
        if (Time.unscaledTime >= nextTriggerCheckTime)
        {
            nextTriggerCheckTime = Time.unscaledTime + 0.35f;
            RefreshTriggerCache();
        }

        if (cachedTriggers != null)
        {
            for (int i = 0; i < cachedTriggers.Length; i++)
            {
                var trigger = cachedTriggers[i];
                if (trigger != null && trigger.IsNarrationPlaying)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void RefreshTriggerCache()
    {
        cachedTriggers = FindObjectsOfType<CubeExhibitionTrigger>();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        Debug.Log($"[MuseumAudioManager] Nhạc nền: {(isMuted ? "Đã tắt tiếng (Muted)" : "Đã bật tiếng (Unmuted)")}");
    }

    public void SetBaseVolume(float newVolume)
    {
        baseVolume = Mathf.Clamp01(newVolume);
    }
}

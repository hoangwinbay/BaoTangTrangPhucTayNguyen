using System.Collections;
using UnityEngine;

/// <summary>
/// Quản lý âm thanh nền (Background Music - BGM) cho Bảo tàng Không gian Văn hóa Tây Nguyên.
/// 
/// ĐẶC TÍNH:
/// 1. Tự động khởi tạo khi chạy game (Zero-Setup): Không cần phải kéo GameObject vào Scene trước.
/// 2. Tự nạp file âm thanh Tây Nguyên từ Resources/Audio/NhacNen_TayNguyen nếu chưa gán trong Inspector.
/// 3. Hỗ trợ Audio Ducking mượt mà: Tự động hạ nhỏ âm lượng nhạc nền khi khách tham quan bấm nghe
///    giọng đọc thuyết minh hiện vật (CubeExhibitionTrigger), và tự tăng lại khi thuyết minh kết thúc.
/// 4. Hỗ trợ phím tắt 'M' (hoặc nút Controller) để Mute/Unmute nhanh nhạc nền theo ý muốn người chơi.
/// </summary>
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

    // Component âm thanh
    private AudioSource bgmSource;
    private bool isMuted = false;
    private float targetVolume = 0.22f;

    // Bộ nhớ cache các CubeExhibitionTrigger trong bảo tàng để theo dõi thuyết minh
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
        // Tự nạp file âm thanh nếu chưa gán
        if (backgroundMusicClip == null)
        {
            backgroundMusicClip = Resources.Load<AudioClip>("Audio/NhacNen_TayNguyen");
            if (backgroundMusicClip == null)
            {
                // Thử nạp tên không phân biệt hoa thường hoặc tên thay thế
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

        // Cache danh sách trigger hiện vật ban đầu
        RefreshTriggerCache();
    }

    private void SetupAudioSource()
    {
        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.loop = true;              // Lặp vô tận
        bgmSource.playOnAwake = true;       // Tự phát khi vào bảo tàng
        bgmSource.spatialBlend = 0f;        // 2D Audio (nghe đều khắp không gian bảo tàng)
        bgmSource.priority = 64;            // Độ ưu tiên cao vừa phải
        bgmSource.volume = baseVolume;
    }

    private void Update()
    {
        // 1. Phím tắt phím 'M' để tắt/bật nhanh nhạc nền
        if (Input.GetKeyDown(toggleMuteKey))
        {
            ToggleMute();
        }

        // 2. Tự động kiểm tra xem có bài thuyết minh hiện vật nào đang phát không (Audio Ducking)
        bool isNarrationActive = CheckIfAnyNarrationPlaying();

        // 3. Tính toán âm lượng đích
        if (isMuted)
        {
            targetVolume = 0f;
        }
        else if (isNarrationActive)
        {
            targetVolume = duckedVolume; // Giảm nhỏ tiếng nhạc nền để nhường chỗ cho thuyết minh
        }
        else
        {
            targetVolume = baseVolume;   // Trở về âm lượng nền êm dịu bình thường
        }

        // 4. Fade âm lượng mượt mà (tránh giật tiếng)
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.volume = Mathf.MoveTowards(bgmSource.volume, targetVolume, fadeSpeed * Time.unscaledDeltaTime);
        }
    }

    /// <summary>
    /// Kiểm tra xem có bệ trưng bày nào đang phát giọng đọc thuyết minh hay không
    /// </summary>
    private bool CheckIfAnyNarrationPlaying()
    {
        if (Time.unscaledTime >= nextTriggerCheckTime)
        {
            nextTriggerCheckTime = Time.unscaledTime + 0.35f; // Quét mỗi 0.35s để tối ưu hiệu năng
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

    /// <summary>
    /// Bật/Tắt chế độ im lặng cho nhạc nền
    /// </summary>
    public void ToggleMute()
    {
        isMuted = !isMuted;
        Debug.Log($"[MuseumAudioManager] Nhạc nền: {(isMuted ? "Đã tắt tiếng (Muted)" : "Đã bật tiếng (Unmuted)")}");
    }

    /// <summary>
    /// Thay đổi âm lượng cơ bản của nhạc nền
    /// </summary>
    public void SetBaseVolume(float newVolume)
    {
        baseVolume = Mathf.Clamp01(newVolume);
    }
}

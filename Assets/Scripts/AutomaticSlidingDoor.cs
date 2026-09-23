using System.Collections;
using UnityEngine;

/// <summary>
/// Điều khiển hệ thống cửa kính tự động (Automatic Sliding Glass Doors) cho bảo tàng:
/// - Hai cánh cửa glass_partition_41_003 và glass_partition_41_004 trượt êm ái sang 2 bên khi người chơi đến gần.
/// - Tự động đóng lại mượt mà khi người chơi đã đi qua hoặc rời khỏi khu vực cửa.
/// - Tích hợp cảm biến kép (Trigger Collider + Khoảng cách Vector3) đảm bảo nhận diện 100% tin cậy trên cả PC và VR.
/// - Tích hợp cơ chế an toàn chống kẹp (Anti-pinch): nếu người chơi còn đứng giữa cửa sẽ không bao giờ đóng lại.
/// - Phát âm thanh trượt mở/đóng cửa êm dịu phong cách bảo tàng hiện đại.
/// </summary>
[SelectionBase]
[ExecuteAlways]
public class AutomaticSlidingDoor : MonoBehaviour
{
    public enum DoorState
    {
        Closed,
        Opening,
        Open,
        Closing
    }

    [Header("=== 1. HAI CÁNH CỬA KÍNH ===")]
    [Tooltip("Cánh cửa kính bên trái (glass_partition_41_004)")]
    public Transform doorLeft;

    [Tooltip("Cánh cửa kính bên phải (glass_partition_41_003)")]
    public Transform doorRight;

    [Header("=== 2. THÔNG SỐ TRƯỢT MỞ ===")]
    [Tooltip("Khoảng cách trượt của mỗi cánh cửa (mét)")]
    [Range(0.4f, 1.2f)]
    public float slideDistance = 0.78f;

    [Tooltip("Tốc độ trượt mở (càng lớn mở càng nhanh)")]
    [Range(0.5f, 5.0f)]
    public float openSpeed = 2.4f;

    [Tooltip("Tốc độ trượt đóng (êm ái, từ tốn)")]
    [Range(0.5f, 5.0f)]
    public float closeSpeed = 1.8f;

    [Tooltip("Thời gian chờ (giây) trước khi tự động đóng sau khi người chơi đã đi qua")]
    [Range(0.5f, 4.0f)]
    public float autoCloseDelay = 1.2f;

    [Header("=== 3. CẢM BIẾN NHẬN DIỆN NGƯỜI CHƠI ===")]
    [Tooltip("Tọa độ tâm lối vào giữa 2 cánh cửa (World Position)")]
    public Vector3 doorwayCenter = new Vector3(19.98f, 1.37f, 38.20f);

    [Tooltip("Khoảng cách nhận diện để cửa bắt đầu mở (mét)")]
    [Range(1.5f, 5.0f)]
    public float triggerDistance = 2.8f;

    [Tooltip("Phát âm thanh trượt khi mở và đóng")]
    public bool enableSoundEffects = true;

    [Header("=== TRẠNG THÁI HIỆN TẠI (DEBUG) ===")]
    [SerializeField] private DoorState currentState = DoorState.Closed;
    [SerializeField] [Range(0f, 1f)] private float openProgress = 0f;
    [SerializeField] private bool isPlayerNearby = false;

    // Vị trí cục bộ khi đóng và mở
    [SerializeField] private Vector3 closedLocalPosLeft;
    [SerializeField] private Vector3 openLocalPosLeft;
    [SerializeField] private Vector3 closedLocalPosRight;
    [SerializeField] private Vector3 openLocalPosRight;
    [SerializeField] private bool positionsInitialized = false;

    // Quản lý thời gian & âm thanh
    private float lastPlayerExitTime = -10f;
    private AudioSource audioSource;
    private AudioClip doorSlideSound;
    private Transform playerTransform;

    public DoorState CurrentState => currentState;
    public float OpenProgress => openProgress;
    public bool IsPlayerNearby => isPlayerNearby;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoEnsureInScene()
    {
        GameObject d3 = GameObject.Find("glass_partition_41_003");
        GameObject d4 = GameObject.Find("glass_partition_41_004");
        if (d3 == null || d4 == null) return;

        // Đảm bảo BoxCollider trên 2 cánh cửa để chặn vật lý
        if (d3.GetComponent<Collider>() == null) d3.AddComponent<BoxCollider>();
        if (d4.GetComponent<Collider>() == null) d4.AddComponent<BoxCollider>();

        // Tìm hoặc tạo AutomaticSlidingDoor
        var doorCtrl = Object.FindObjectOfType<AutomaticSlidingDoor>();
        if (doorCtrl == null)
        {
            GameObject parentGo = d3.transform.parent ? d3.transform.parent.gameObject : d3;
            doorCtrl = parentGo.AddComponent<AutomaticSlidingDoor>();
        }

        doorCtrl.doorLeft = d4.transform;
        doorCtrl.doorRight = d3.transform;
        doorCtrl.doorwayCenter = new Vector3(19.98f, 1.37f, 38.20f);
        doorCtrl.slideDistance = 0.78f;
        doorCtrl.openSpeed = 2.4f;
        doorCtrl.closeSpeed = 1.8f;
        doorCtrl.autoCloseDelay = 1.2f;
        doorCtrl.triggerDistance = 2.8f;
        doorCtrl.enableSoundEffects = true;
        doorCtrl.InitializeDoors(forceRecalculate: true);

        // Tạo hoặc kiểm tra vùng cảm biến
        GameObject parentObj = d3.transform.parent ? d3.transform.parent.gameObject : d3;
        Transform existingSensor = parentObj.transform.Find("DoorSensorTrigger");
        if (existingSensor == null)
        {
            GameObject sensorGo = new GameObject("DoorSensorTrigger");
            sensorGo.transform.SetParent(parentObj.transform, false);
            sensorGo.transform.position = doorCtrl.doorwayCenter;
            sensorGo.transform.rotation = Quaternion.identity;

            BoxCollider sensorCol = sensorGo.AddComponent<BoxCollider>();
            sensorCol.isTrigger = true;
            sensorCol.size = new Vector3(5.5f, 2.6f, 2.5f);
            sensorCol.center = Vector3.zero;

            DoorSensorTrigger triggerScript = sensorGo.AddComponent<DoorSensorTrigger>();
            triggerScript.slidingDoor = doorCtrl;
        }
    }

    void Awake()
    {
        InitializeDoors();
        SetupAudio();
    }

    void Start()
    {
        InitializeDoors();
        CachePlayer();
    }

    void OnEnable()
    {
        InitializeDoors();
    }

    /// <summary>
    /// Tìm kiếm và gán 2 cánh cửa nếu chưa được gán trong Inspector
    /// </summary>
    public void AutoFindDoors()
    {
        if (doorLeft == null)
        {
            var go4 = GameObject.Find("glass_partition_41_004");
            if (go4 != null) doorLeft = go4.transform;
        }

        if (doorRight == null)
        {
            var go3 = GameObject.Find("glass_partition_41_003");
            if (go3 != null) doorRight = go3.transform;
        }
    }

    /// <summary>
    /// Khởi tạo tọa độ đóng/mở chuẩn xác cho 2 cánh cửa
    /// </summary>
    public void InitializeDoors(bool forceRecalculate = false)
    {
        AutoFindDoors();

        if (doorLeft == null || doorRight == null) return;

        // Nếu đã tính toán và không bị ép tính lại thì giữ nguyên
        if (positionsInitialized && !forceRecalculate) return;

        // Lưu vị trí đóng ban đầu (khi hai cửa khép sát nhau ở giữa)
        // Nếu hiện tại cửa đang mở một phần, cần tính toán từ doorway center
        if (forceRecalculate || !positionsInitialized)
        {
            closedLocalPosLeft = doorLeft.localPosition;
            closedLocalPosRight = doorRight.localPosition;

            // Hướng trượt trong World space:
            // Door 004 nằm ở phía Z nhỏ hơn (Z ~ 37.79) -> Trượt về hướng -Z (Vector3.back)
            // Door 003 nằm ở phía Z lớn hơn (Z ~ 38.61) -> Trượt về hướng +Z (Vector3.forward)
            Vector3 worldSlideLeft = new Vector3(0f, 0f, -slideDistance);
            Vector3 worldSlideRight = new Vector3(0f, 0f, slideDistance);

            // Chuyển đổi sang local position của đối tượng cha
            if (doorLeft.parent != null)
            {
                openLocalPosLeft = doorLeft.parent.InverseTransformPoint(doorLeft.position + worldSlideLeft);
            }
            else
            {
                openLocalPosLeft = closedLocalPosLeft + worldSlideLeft;
            }

            if (doorRight.parent != null)
            {
                openLocalPosRight = doorRight.parent.InverseTransformPoint(doorRight.position + worldSlideRight);
            }
            else
            {
                openLocalPosRight = closedLocalPosRight + worldSlideRight;
            }

            positionsInitialized = true;
        }
    }

    private void CachePlayer()
    {
        if (Camera.main != null)
        {
            playerTransform = Camera.main.transform;
            return;
        }

        var fps = FindObjectOfType<FPSController>();
        if (fps != null)
        {
            playerTransform = fps.transform;
            return;
        }

        var playerTagObj = GameObject.FindGameObjectWithTag("Player");
        if (playerTagObj != null)
        {
            playerTransform = playerTagObj.transform;
        }
    }

    void Update()
    {
        // 1. Kiểm tra vị trí cửa
        if (!positionsInitialized || doorLeft == null || doorRight == null)
        {
            InitializeDoors();
            if (!positionsInitialized) return;
        }

        // 2. Nhận diện người chơi ở chế độ Play mode
        if (Application.isPlaying)
        {
            CheckPlayerPresence();
            UpdateDoorMotion();
        }
        else
        {
            // Trong Editor: Áp dụng vị trí theo slider openProgress
            ApplyDoorPositions(openProgress);
        }
    }

    /// <summary>
    /// Kiểm tra người chơi có đang ở gần khu vực cửa hay không
    /// </summary>
    private void CheckPlayerPresence()
    {
        if (playerTransform == null)
        {
            CachePlayer();
            if (playerTransform == null) return;
        }

        // Tính khoảng cách từ người chơi tới tâm lối đi (chỉ xét mặt phẳng ngang XZ)
        Vector3 playerPos = playerTransform.position;
        Vector3 doorPos = doorwayCenter;
        
        // Đo khoảng cách ngang (bỏ qua chênh lệch chiều cao sàn)
        float horizontalDist = Vector2.Distance(
            new Vector2(playerPos.x, playerPos.z),
            new Vector2(doorPos.x, doorPos.z)
        );

        // Kiểm tra chiều cao hợp lệ (người chơi cùng tầng)
        float heightDiff = Mathf.Abs(playerPos.y - doorPos.y);
        bool inHeightRange = heightDiff < 3.0f;

        bool wasNearby = isPlayerNearby;
        isPlayerNearby = (horizontalDist <= triggerDistance) && inHeightRange;

        // Khi người chơi vừa rời khỏi vùng nhận diện
        if (wasNearby && !isPlayerNearby)
        {
            lastPlayerExitTime = Time.time;
        }
    }

    /// <summary>
    /// Cập nhật tiến trình trượt mở / đóng cửa
    /// </summary>
    private void UpdateDoorMotion()
    {
        float targetProgress = 0f;

        if (isPlayerNearby)
        {
            // Người chơi ở gần: Lập tức mở ra
            targetProgress = 1f;
        }
        else
        {
            // Người chơi đã đi vào trong hoặc đi ra xa:
            // Chờ hết thời gian autoCloseDelay mới bắt đầu đóng
            if (Time.time - lastPlayerExitTime >= autoCloseDelay)
            {
                targetProgress = 0f;
            }
            else
            {
                // Vẫn đang trong thời gian chờ đóng: giữ nguyên trạng thái mở
                targetProgress = 1f;
            }
        }

        // Cập nhật trạng thái
        if (targetProgress > openProgress)
        {
            if (currentState != DoorState.Opening && openProgress < 0.99f)
            {
                currentState = DoorState.Opening;
                PlayDoorSound(true);
            }
            openProgress = Mathf.MoveTowards(openProgress, targetProgress, openSpeed * Time.deltaTime);
            if (openProgress >= 0.999f)
            {
                currentState = DoorState.Open;
            }
        }
        else if (targetProgress < openProgress)
        {
            if (currentState != DoorState.Closing && openProgress > 0.01f)
            {
                currentState = DoorState.Closing;
                PlayDoorSound(false);
            }
            openProgress = Mathf.MoveTowards(openProgress, targetProgress, closeSpeed * Time.deltaTime);
            if (openProgress <= 0.001f)
            {
                currentState = DoorState.Closed;
            }
        }

        // Áp dụng vị trí trượt với đường cong nội suy mượt mà (SmoothStep Easing)
        ApplyDoorPositions(openProgress);
    }

    /// <summary>
    /// Áp dụng vị trí localPosition cho 2 cánh cửa theo tiến trình [0..1]
    /// </summary>
    public void ApplyDoorPositions(float progress)
    {
        if (doorLeft == null || doorRight == null) return;

        // Nội suy mượt mà (Gia tốc lúc khởi đầu và hãm phanh êm ái khi dừng)
        float smoothT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress));

        doorLeft.localPosition = Vector3.Lerp(closedLocalPosLeft, openLocalPosLeft, smoothT);
        doorRight.localPosition = Vector3.Lerp(closedLocalPosRight, openLocalPosRight, smoothT);
    }

    /// <summary>
    /// Gọi từ Trigger Collider bên ngoài (nếu có gắn BoxCollider isTrigger)
    /// </summary>
    public void OnSensorTriggerEnter()
    {
        isPlayerNearby = true;
    }

    public void OnSensorTriggerExit()
    {
        isPlayerNearby = false;
        lastPlayerExitTime = Time.time;
    }

    /// <summary>
    /// Thiết lập AudioSource và tạo âm thanh trượt cửa êm ái
    /// </summary>
    private void SetupAudio()
    {
        if (!enableSoundEffects) return;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 1.0f; // Âm thanh 3D định hướng không gian
        audioSource.minDistance = 2.0f;
        audioSource.maxDistance = 15.0f;
        audioSource.volume = 0.45f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (doorSlideSound == null)
        {
            doorSlideSound = CreateProceduralSlidingSound();
        }
        audioSource.clip = doorSlideSound;
    }

    private void PlayDoorSound(bool opening)
    {
        if (!enableSoundEffects || audioSource == null || doorSlideSound == null) return;

        audioSource.pitch = opening ? 1.05f : 0.95f;
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// Tạo âm thanh trượt gió êm dịu (smooth motorized whoosh sound) chuẩn âm thanh bảo tàng
    /// </summary>
    private AudioClip CreateProceduralSlidingSound()
    {
        int sampleRate = 44100;
        float duration = 0.75f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            // Envelope hình quả chuông (fade in -> duy trì -> fade out)
            float envelope = Mathf.Sin(t * Mathf.PI);
            envelope = Mathf.Pow(envelope, 1.4f);

            // Tiếng rít gió êm dịu pha trộn tần số thấp
            float noise = (Random.value * 2f - 1f) * 0.12f;
            float lowHum = Mathf.Sin(2f * Mathf.PI * 180f * t) * 0.08f;
            float airWhoosh = Mathf.Sin(2f * Mathf.PI * (280f + 140f * (1f - t)) * t) * 0.15f;

            samples[i] = (noise + lowHum + airWhoosh) * envelope * 0.6f;
        }

        AudioClip clip = AudioClip.Create("GlassDoorSlideSound", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    void OnDrawGizmosSelected()
    {
        // Hiển thị trực quan khu vực cảm biến nhận diện trong cửa sổ Scene
        Gizmos.color = isPlayerNearby ? new Color(0.2f, 1f, 0.4f, 0.4f) : new Color(0.2f, 0.7f, 1f, 0.25f);
        Gizmos.DrawWireSphere(doorwayCenter, triggerDistance);

        // Hiển thị tâm cửa và hướng mở
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(doorwayCenter, new Vector3(0.1f, 2.35f, 1.64f));

        // Vẽ hướng trượt của 2 cánh
        if (doorLeft != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(doorLeft.position, doorLeft.position + new Vector3(0, 0, -slideDistance));
        }

        if (doorRight != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(doorRight.position, doorRight.position + new Vector3(0, 0, slideDistance));
        }
    }
}

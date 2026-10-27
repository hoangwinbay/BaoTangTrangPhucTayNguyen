using System.Collections;
using UnityEngine;

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

    [SerializeField] private Vector3 closedLocalPosLeft;
    [SerializeField] private Vector3 openLocalPosLeft;
    [SerializeField] private Vector3 closedLocalPosRight;
    [SerializeField] private Vector3 openLocalPosRight;
    [SerializeField] private bool positionsInitialized = false;

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

        if (d3.GetComponent<Collider>() == null) d3.AddComponent<BoxCollider>();
        if (d4.GetComponent<Collider>() == null) d4.AddComponent<BoxCollider>();

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

    public void InitializeDoors(bool forceRecalculate = false)
    {
        AutoFindDoors();

        if (doorLeft == null || doorRight == null) return;

        if (positionsInitialized && !forceRecalculate) return;

        if (forceRecalculate || !positionsInitialized)
        {
            closedLocalPosLeft = doorLeft.localPosition;
            closedLocalPosRight = doorRight.localPosition;

            Vector3 worldSlideLeft = new Vector3(0f, 0f, -slideDistance);
            Vector3 worldSlideRight = new Vector3(0f, 0f, slideDistance);

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
        if (!positionsInitialized || doorLeft == null || doorRight == null)
        {
            InitializeDoors();
            if (!positionsInitialized) return;
        }

        if (Application.isPlaying)
        {
            CheckPlayerPresence();
            UpdateDoorMotion();
        }
        else
        {
            ApplyDoorPositions(openProgress);
        }
    }

    private void CheckPlayerPresence()
    {
        if (playerTransform == null)
        {
            CachePlayer();
            if (playerTransform == null) return;
        }

        Vector3 playerPos = playerTransform.position;
        Vector3 doorPos = doorwayCenter;

        float horizontalDist = Vector2.Distance(
            new Vector2(playerPos.x, playerPos.z),
            new Vector2(doorPos.x, doorPos.z)
        );

        float heightDiff = Mathf.Abs(playerPos.y - doorPos.y);
        bool inHeightRange = heightDiff < 3.0f;

        bool wasNearby = isPlayerNearby;
        isPlayerNearby = (horizontalDist <= triggerDistance) && inHeightRange;

        if (wasNearby && !isPlayerNearby)
        {
            lastPlayerExitTime = Time.time;
        }
    }

    private void UpdateDoorMotion()
    {
        float targetProgress = 0f;

        if (isPlayerNearby)
        {
            targetProgress = 1f;
        }
        else
        {
            if (Time.time - lastPlayerExitTime >= autoCloseDelay)
            {
                targetProgress = 0f;
            }
            else
            {
                targetProgress = 1f;
            }
        }

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

        ApplyDoorPositions(openProgress);
    }

    public void ApplyDoorPositions(float progress)
    {
        if (doorLeft == null || doorRight == null) return;

        float smoothT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress));

        doorLeft.localPosition = Vector3.Lerp(closedLocalPosLeft, openLocalPosLeft, smoothT);
        doorRight.localPosition = Vector3.Lerp(closedLocalPosRight, openLocalPosRight, smoothT);
    }

    public void OnSensorTriggerEnter()
    {
        isPlayerNearby = true;
    }

    public void OnSensorTriggerExit()
    {
        isPlayerNearby = false;
        lastPlayerExitTime = Time.time;
    }

    private void SetupAudio()
    {
        if (!enableSoundEffects) return;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 1.0f;
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

    private AudioClip CreateProceduralSlidingSound()
    {
        int sampleRate = 44100;
        float duration = 0.75f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            float envelope = Mathf.Sin(t * Mathf.PI);
            envelope = Mathf.Pow(envelope, 1.4f);

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
        Gizmos.color = isPlayerNearby ? new Color(0.2f, 1f, 0.4f, 0.4f) : new Color(0.2f, 0.7f, 1f, 0.25f);
        Gizmos.DrawWireSphere(doorwayCenter, triggerDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(doorwayCenter, new Vector3(0.1f, 2.35f, 1.64f));

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

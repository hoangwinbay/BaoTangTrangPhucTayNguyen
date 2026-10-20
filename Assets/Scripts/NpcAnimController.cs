using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Điều khiển animation NPC theo tốc độ và trạng thái của NavMeshAgent:
/// - Khi di chuyển: Tự động kích hoạt animation Walking (Walk)
/// - Khi đứng yên (tham quan, dừng xem hiện vật): Tự động chuyển về animation Standing Idle (Idle)
/// - Tự động phát hiện parameter 'Speed' trong Animator Controller để Blend mượt mà
/// - Fallback CrossFade nếu Animator Controller không có parameter
/// - Luôn tắt applyRootMotion để tránh lỗi lún đất hoặc trôi vị trí khỏi NavMesh.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NpcAnimController : MonoBehaviour
{
    [Header("Tên Animation State")]
    [Tooltip("Tên state IDLE trong Animator Controller (mặc định: Idle)")]
    public string idleStateName = "Idle";

    [Tooltip("Tên state WALK trong Animator Controller (mặc định: Walk)")]
    public string walkStateName = "Walk";

    [Tooltip("Layer của Animator (thường là 0)")]
    public int animatorLayer = 0;

    [Header("Cấu hình Parameter")]
    [Tooltip("Sử dụng parameter để chuyển đổi animation mượt mà.")]
    public bool useParameter = true;

    [Tooltip("Tên parameter trong Animator Controller (mặc định: Speed).")]
    public string parameterName = "Speed";

    [Tooltip("Tick nếu parameter là Bool thay vì Float.")]
    public bool isBoolParameter = false;

    [Header("Trạng thái Hoạt động (Debug)")]
    [SerializeField] private string _currentState = "Idle";
    [SerializeField] private float  _speed = 0f;
    [SerializeField] private bool   _isMoving = false;
    [SerializeField] private string _log = "";

    private Animator anim;
    private NavMeshAgent agent;
    private bool isWalking = false;

    // Hashes
    private int speedParamHash = -1;
    private int boolParamHash = -1;
    private int idleStateHash = -1;
    private int walkStateHash = -1;
    private bool hasFloatSpeed = false;
    private bool hasBoolParam = false;
    private bool initialized = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim  = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        Init();
        if (anim != null)
        {
            Debug.Log($"[NpcAnim] {name}: Đã khởi động Animation! Controller='{(anim.runtimeAnimatorController != null ? anim.runtimeAnimatorController.name : "CHƯA GÁN")}', Avatar='{(anim.avatar != null ? anim.avatar.name : "CHƯA GÁN")}'");
        }
        else
        {
            Debug.LogWarning($"[NpcAnim] {name}: KHÔNG tìm thấy Animator component!");
        }
    }

    void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null)  anim  = GetComponentInChildren<Animator>();

        if (anim == null)
        {
            _log = "KHÔNG TÌM THẤY ANIMATOR!";
            return;
        }

        // Tắt Root Motion để NavMeshAgent toàn quyền quản lý tọa độ di chuyển
        anim.applyRootMotion = false;
        anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        idleStateHash = Animator.StringToHash(idleStateName);
        walkStateHash = Animator.StringToHash(walkStateName);

        // Kiểm tra controller, nạp fallback từ Resources nếu chưa gán
        if (anim.runtimeAnimatorController == null)
        {
            var fallbackCtrl = Resources.Load<RuntimeAnimatorController>("NPC_AnimatorController");
            if (fallbackCtrl != null)
            {
                anim.runtimeAnimatorController = fallbackCtrl;
            }
            else
            {
                _log = "ANIMATOR CONTROLLER CHƯA ĐƯỢC GÁN!";
                return;
            }
        }

        // Tự động nhận diện Parameters trong Animator
        hasFloatSpeed = false;
        hasBoolParam = false;

        string targetParam = string.IsNullOrEmpty(parameterName) ? "Speed" : parameterName;

        if (anim.parameters != null)
        {
            foreach (var p in anim.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Float &&
                    (p.name.Equals(targetParam, System.StringComparison.OrdinalIgnoreCase) || p.name.Equals("Speed", System.StringComparison.OrdinalIgnoreCase)))
                {
                    speedParamHash = p.nameHash;
                    parameterName = p.name;
                    hasFloatSpeed = true;
                    break;
                }
                else if (p.type == AnimatorControllerParameterType.Bool &&
                    (p.name.Equals(targetParam, System.StringComparison.OrdinalIgnoreCase) || p.name.Equals("isWalking", System.StringComparison.OrdinalIgnoreCase) || p.name.Equals("isMoving", System.StringComparison.OrdinalIgnoreCase)))
                {
                    boolParamHash = p.nameHash;
                    parameterName = p.name;
                    hasBoolParam = true;
                    break;
                }
            }
        }

        if (hasFloatSpeed)
        {
            _log = $"Float Param: '{parameterName}'";
        }
        else if (hasBoolParam)
        {
            _log = $"Bool Param: '{parameterName}'";
        }
        else
        {
            _log = $"Direct State: '{idleStateName}'/'{walkStateName}'";
        }

        initialized = true;
    }

    void Update()
    {
        if (agent == null) return;
        if (anim == null || !initialized)
        {
            anim = GetComponentInChildren<Animator>();
            if (anim == null) return;
            Init();
        }

        // Đảm bảo không bao giờ bật Root Motion gây giật hoặc lệch vị trí
        if (anim.applyRootMotion) anim.applyRootMotion = false;

        float currentSpeed = agent.velocity.magnitude;
        _speed = currentSpeed;

        // Xác định xem NPC có đang di chuyển hay không
        bool hasActivePath = !agent.isStopped && agent.hasPath && agent.remainingDistance > (agent.stoppingDistance + 0.08f);
        bool moving = currentSpeed > 0.08f || (hasActivePath && agent.pathStatus == NavMeshPathStatus.PathComplete);
        _isMoving = moving;

        // Tính toán tốc độ chuẩn hóa (0..1)
        float maxSpeed = Mathf.Max(agent.speed, 0.1f);
        float normSpeed = Mathf.Clamp01(currentSpeed / maxSpeed);

        // Khi đang di chuyển, đảm bảo Speed param >= 0.4f để vượt ngưỡng transition (> 0.1f)
        float targetSpeed = moving ? Mathf.Max(normSpeed, 0.4f) : 0f;

        // 3. Quản lý trạng thái và chuyển đổi animation
        if (moving != isWalking)
        {
            isWalking = moving;
            _currentState = moving ? walkStateName : idleStateName;

            // In log thông báo chuyển đổi trạng thái
            Debug.Log($"[NpcAnim] {name}: ➔ Chuyển trạng thái: {(_currentState == walkStateName ? "🏃 DI CHUYỂN (Walking)" : "🧍 ĐỨNG YÊN (Standing Idle)")} (speed={currentSpeed:F2})");

            // Set ngay lập tức giá trị Speed khi vừa đổi trạng thái để kích hoạt transition tức thì
            if (hasFloatSpeed)
            {
                anim.SetFloat(speedParamHash, targetSpeed);
            }

            if (!hasFloatSpeed && !hasBoolParam)
            {
                anim.CrossFadeInFixedTime(moving ? walkStateName : idleStateName, moving ? 0.2f : 0.25f);
            }
        }
        else
        {
            // Trong trạng thái ổn định, dùng damping nhẹ để chuyển động mượt mà
            if (hasFloatSpeed)
            {
                anim.SetFloat(speedParamHash, targetSpeed, 0.15f, Time.deltaTime);
            }
        }

        // Cập nhật Parameter Bool nếu có
        if (hasBoolParam)
        {
            anim.SetBool(boolParamHash, moving);
        }
    }
}

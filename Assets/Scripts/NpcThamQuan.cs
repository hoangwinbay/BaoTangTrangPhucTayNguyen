using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Điều khiển hành vi tham quan tự động của NPC trong bảo tàng:
/// 1. Tự động thu thập tất cả các điểm dừng tham quan:
///    - 12 Bệ trưng bày CubeExhibitionTrigger (Ê Đê, Gia Rai, Ba Na, Chư Ru, ...)
///    - Các mô hình tượng/trang phục dân tộc (B_rau_nam, Ba_na_nu, E_de_nam, ...)
///    - Các bệ trưng bày Cube khác trong bảo tàng
///    - TrangPhucInfo (nếu có)
///    - Điểm rải rác trên NavMesh làm fallback dự phòng.
/// 2. Đảm bảo NavMeshAgent luôn được đặt an toàn trên NavMesh bằng Warp.
/// 3. Khắc phục triệt để lỗi không di chuyển do:
///    - Race condition quãng đường (remainingDistance <= stoppingDistance) ngay khi vừa SetDestination.
///    - Bị kẹt hoặc điểm đến không hợp lệ (tự động chọn điểm khác sau thời gian chờ).
///    - Danh sách stops bị rỗng trước đây.
/// 4. Dừng quan sát tự nhiên: quay mặt về phía hiện vật, quan sát trong 4-8 giây rồi tiếp tục di chuyển.
/// </summary>
public class NpcThamQuan : MonoBehaviour
{
    [Header("Cài đặt Di chuyển")]
    [Tooltip("Tốc độ di chuyển của NPC (m/s)")]
    public float tocDo = 1.2f;

    [Tooltip("Khoảng cách dừng trước hiện vật")]
    public float khoangCachDung = 0.35f;

    [Header("Cài đặt Quan sát")]
    [Tooltip("Thời gian dừng xem tối thiểu (giây)")]
    public float minThoiGianXem = 4.0f;

    [Tooltip("Thời gian dừng xem tối đa (giây)")]
    public float maxThoiGianXem = 8.0f;

    [Header("Trạng thái (Debug)")]
    [SerializeField] private string diemDangDen = "";
    [SerializeField] private bool dangXem = false;
    [SerializeField] private float demNguocXem = 0f;

    public struct Stop
    {
        public Vector3 pos;
        public Vector3 lookDir;
        public string ten;
    }

    private static Stop[] sharedStops; // Chia sẻ danh sách stops giữa các NPC để tối ưu hiệu năng
    private NavMeshAgent agent;
    private int curIdx = -1;
    private int prevIdx = -1;
    private Vector3 lastMoveDir = Vector3.forward;
    private float destSetTime = 0f;
    private float stuckTimer = 0f;
    private Vector3 lastPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = tocDo;
            agent.angularSpeed = 220f;
            agent.acceleration = 3.5f;
            agent.stoppingDistance = khoangCachDung;
            agent.autoBraking = true;
            agent.avoidancePriority = Random.Range(20, 80); // Tránh va chạm mượt mà giữa các NPC
        }

        // Đảm bảo NPC đứng chuẩn trên NavMesh
        SnapToNavMesh();

        // Xây dựng danh sách điểm dừng nếu chưa có
        if (sharedStops == null || sharedStops.Length == 0)
        {
            BuildStops();
        }

        // Khởi động di chuyển với độ trễ ngẫu nhiên nhẹ (0.3s - 2.5s) để các NPC xuất phát lệch nhau tự nhiên
        float delay = Random.Range(0.3f, 2.5f);
        Invoke(nameof(KhoiDong), delay);
    }

    public void SnapToNavMesh()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (agent != null && !agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out var hit, 5.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    void BuildStops()
    {
        var list = new List<Stop>();
        var addedPos = new List<Vector3>();

        // 1. Quét các bệ CubeExhibitionTrigger (12 bệ thuyết minh chính của bảo tàng)
        var triggers = Object.FindObjectsByType<CubeExhibitionTrigger>(FindObjectsInactive.Exclude);
        foreach (var trig in triggers)
        {
            Vector3 cubePos = trig.transform.position;
            Vector3 fwd = trig.transform.forward;
            fwd.y = 0f;
            if (fwd.sqrMagnitude < 0.001f) fwd = Vector3.forward;
            fwd.Normalize();

            // Thử 4 hướng quanh bệ trưng bày để tìm vị trí đứng đẹp nhất trên NavMesh
            Vector3[] candidateOffsets = new Vector3[]
            {
                fwd * 1.35f,
                -fwd * 1.35f,
                Quaternion.Euler(0, 90, 0) * fwd * 1.35f,
                Quaternion.Euler(0, -90, 0) * fwd * 1.35f
            };

            foreach (var off in candidateOffsets)
            {
                Vector3 p = cubePos + off;
                if (NavMesh.SamplePosition(p, out var hit, 1.8f, NavMesh.AllAreas))
                {
                    Vector3 look = (cubePos - hit.position);
                    look.y = 0f;
                    if (look.sqrMagnitude > 0.01f && !IsTooCloseToExisting(hit.position, addedPos, 1.2f))
                    {
                        list.Add(new Stop
                        {
                            pos = hit.position,
                            lookDir = look.normalized,
                            ten = string.IsNullOrEmpty(trig.ethnicTitle) ? trig.gameObject.name : trig.ethnicTitle
                        });
                        addedPos.Add(hit.position);
                        break;
                    }
                }
            }
        }

        // 2. Quét các mô hình tượng trang phục dân tộc (các tượng _00001)
        var allGos = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude);
        foreach (var go in allGos)
        {
            if (go.name.EndsWith("_00001") && go.GetComponent<Renderer>() != null)
            {
                Vector3 statuePos = go.transform.position;
                Vector3 fwd = go.transform.forward;
                fwd.y = 0f;
                if (fwd.sqrMagnitude < 0.001f) fwd = Vector3.forward;
                fwd.Normalize();

                Vector3 p = statuePos + fwd * 1.35f;
                if (NavMesh.SamplePosition(p, out var hit, 2.0f, NavMesh.AllAreas))
                {
                    Vector3 look = statuePos - hit.position;
                    look.y = 0f;
                    if (look.sqrMagnitude > 0.01f && !IsTooCloseToExisting(hit.position, addedPos, 1.2f))
                    {
                        list.Add(new Stop
                        {
                            pos = hit.position,
                            lookDir = look.normalized,
                            ten = "Tượng " + go.name.Replace("_00001", "")
                        });
                        addedPos.Add(hit.position);
                    }
                }
            }
        }

        // 3. Quét các bệ Cube phụ khác (Cube (33), Cube (34), Cube (35), Cube (37), Cube (38), ...)
        for (int i = 1; i <= 90; i++)
        {
            string cName = i == 0 ? "Cube" : $"Cube ({i})";
            GameObject cubeObj = GameObject.Find(cName);
            if (cubeObj != null && cubeObj.GetComponent<CubeExhibitionTrigger>() == null)
            {
                // Chỉ lấy các Cube bệ trưng bày có kích thước đáng kể
                if (cubeObj.transform.lossyScale.magnitude > 0.8f)
                {
                    Vector3 p = cubeObj.transform.position + Vector3.forward * 1.3f;
                    if (NavMesh.SamplePosition(p, out var hit, 1.8f, NavMesh.AllAreas))
                    {
                        Vector3 look = cubeObj.transform.position - hit.position;
                        look.y = 0f;
                        if (look.sqrMagnitude > 0.01f && !IsTooCloseToExisting(hit.position, addedPos, 2.0f))
                        {
                            list.Add(new Stop
                            {
                                pos = hit.position,
                                lookDir = look.normalized,
                                ten = "Hiện vật " + cName
                            });
                            addedPos.Add(hit.position);
                        }
                    }
                }
            }
        }

        // 4. Quét TrangPhucInfo (nếu có trong scene)
        var infos = Object.FindObjectsByType<TrangPhucInfo>(FindObjectsInactive.Exclude);
        foreach (var inf in infos)
        {
            Transform targetTr = inf.bangMoTa != null ? inf.bangMoTa.transform : inf.transform;
            Vector3 fwd = targetTr.forward; fwd.y = 0f;
            if (fwd.sqrMagnitude < 0.001f) fwd = Vector3.forward;
            fwd.Normalize();
            Vector3 p = targetTr.position + fwd * 1.35f;
            if (NavMesh.SamplePosition(p, out var hit, 2.5f, NavMesh.AllAreas))
            {
                Vector3 look = targetTr.position - hit.position;
                look.y = 0f;
                if (look.sqrMagnitude > 0.01f && !IsTooCloseToExisting(hit.position, addedPos, 1.2f))
                {
                    list.Add(new Stop
                    {
                        pos = hit.position,
                        lookDir = look.normalized,
                        ten = inf.name
                    });
                    addedPos.Add(hit.position);
                }
            }
        }

        // 5. Dự phòng (Fallback): Nếu danh sách vẫn ít hơn 5 điểm, lấy các điểm trên NavMesh
        if (list.Count < 5)
        {
            var tri = NavMesh.CalculateTriangulation();
            if (tri.vertices != null && tri.vertices.Length > 0)
            {
                int step = Mathf.Max(1, tri.vertices.Length / 12);
                for (int i = 0; i < tri.vertices.Length && list.Count < 15; i += step)
                {
                    Vector3 v = tri.vertices[i];
                    if (NavMesh.SamplePosition(v, out var hit, 1.0f, NavMesh.AllAreas))
                    {
                        if (!IsTooCloseToExisting(hit.position, addedPos, 2.5f))
                        {
                            list.Add(new Stop
                            {
                                pos = hit.position,
                                lookDir = (Vector3.forward + Vector3.right).normalized,
                                ten = $"Khu vực bảo tàng #{list.Count + 1}"
                            });
                            addedPos.Add(hit.position);
                        }
                    }
                }
            }
        }

        sharedStops = list.ToArray();
        Debug.Log($"[NPC] Đã khởi tạo thành công {sharedStops.Length} điểm dừng tham quan trong bảo tàng.");
    }

    private static bool IsTooCloseToExisting(Vector3 pos, List<Vector3> existing, float minDist)
    {
        float minDistSq = minDist * minDist;
        for (int i = 0; i < existing.Count; i++)
        {
            Vector3 d = existing[i] - pos; d.y = 0f;
            if (d.sqrMagnitude < minDistSq) return true;
        }
        return false;
    }

    void KhoiDong()
    {
        if (sharedStops == null || sharedStops.Length == 0)
        {
            BuildStops();
            if (sharedStops == null || sharedStops.Length == 0)
            {
                Debug.LogWarning($"[NPC] {gameObject.name}: Không tìm thấy điểm dừng nào trên NavMesh.");
                return;
            }
        }

        SnapToNavMesh();
        GoTo(PickNext(-1, transform.forward));
    }

    int PickNext(int excludeIdx, Vector3 fwd)
    {
        int n = sharedStops.Length;
        if (n == 0) return -1;
        if (n == 1) return 0;

        fwd.y = 0f;
        if (fwd.sqrMagnitude < 0.001f) fwd = Vector3.forward;
        fwd.Normalize();

        float[] score = new float[n];
        for (int i = 0; i < n; i++)
        {
            if (i == excludeIdx || i == prevIdx)
            {
                score[i] = -9999f;
                continue;
            }

            Vector3 d = sharedStops[i].pos - transform.position;
            d.y = 0f;
            float dist = Mathf.Max(d.magnitude, 0.05f);
            float dot = Vector3.Dot(d / dist, fwd);

            // Ưu tiên các điểm ở phía trước, khoảng cách vừa phải (không quá xa, không quá sát)
            score[i] = dot * 2.5f - (dist * 0.04f) + Random.Range(0f, 0.8f);
        }

        int[] order = new int[n];
        for (int i = 0; i < n; i++) order[i] = i;
        System.Array.Sort(order, (a, b) => score[b].CompareTo(score[a]));

        int top = Mathf.Min(4, n);
        return order[Random.Range(0, top)];
    }

    void GoTo(int idx)
    {
        if (idx < 0 || sharedStops == null || idx >= sharedStops.Length || agent == null) return;

        SnapToNavMesh();
        if (!agent.isOnNavMesh) return;

        curIdx = idx;
        dangXem = false;
        diemDangDen = sharedStops[idx].ten;

        agent.updateRotation = true;
        agent.isStopped = false;
        agent.SetDestination(sharedStops[idx].pos);

        destSetTime = Time.time;
        stuckTimer = 0f;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (sharedStops == null || sharedStops.Length == 0 || agent == null || curIdx < 0) return;
        if (!agent.isOnNavMesh)
        {
            SnapToNavMesh();
            return;
        }

        if (!dangXem)
        {
            // Tránh bẫy race-condition: chỉ kiểm tra đã đến đích sau ít nhất 0.35 giây tính từ lúc SetDestination
            float elapsedSinceSet = Time.time - destSetTime;
            if (elapsedSinceSet > 0.35f)
            {
                // Bộ đếm phát hiện kẹt: nếu quá 2 giây mà NPC không nhích được thì đổi điểm khác
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= 2.0f)
                {
                    if (Vector3.Distance(transform.position, lastPosition) < 0.1f && !agent.pathPending && agent.velocity.sqrMagnitude < 0.02f)
                    {
                        // Đang bị tắc đường hoặc kẹt góc tường, lập tức chuyển hướng sang điểm khác
                        prevIdx = curIdx;
                        GoTo(PickNext(curIdx, transform.forward));
                        return;
                    }
                    lastPosition = transform.position;
                    stuckTimer = 0f;
                }

                // Kiểm tra xem đã đến đích chưa
                bool reachedDestination = !agent.pathPending &&
                    (agent.remainingDistance <= agent.stoppingDistance + 0.15f || !agent.hasPath);

                if (reachedDestination)
                {
                    dangXem = true;
                    demNguocXem = Random.Range(minThoiGianXem, maxThoiGianXem);
                    agent.isStopped = true;
                    agent.updateRotation = false;
                }
                else if (agent.velocity.sqrMagnitude > 0.02f)
                {
                    lastMoveDir = agent.velocity.normalized;
                }
            }
        }
        else
        {
            // Chế độ dừng xem: xoay người nhẹ nhàng hướng mắt vào hiện vật
            if (curIdx >= 0 && curIdx < sharedStops.Length)
            {
                Vector3 look = sharedStops[curIdx].lookDir;
                look.y = 0f;
                if (look.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(look, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3.5f);
                }
            }

            demNguocXem -= Time.deltaTime;
            if (demNguocXem <= 0f)
            {
                dangXem = false;
                prevIdx = curIdx;
                GoTo(PickNext(curIdx, lastMoveDir));
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (sharedStops != null && curIdx >= 0 && curIdx < sharedStops.Length)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, sharedStops[curIdx].pos);
            Gizmos.DrawWireSphere(sharedStops[curIdx].pos, 0.4f);
        }
    }
}

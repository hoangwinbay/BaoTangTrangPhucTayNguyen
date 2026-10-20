using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Tự động sửa các lỗi render phổ biến trong bảo tàng VR:
/// 1. Z-Fighting: object/tường/sàn nhấp nháy do mesh chồng nhau
/// 2. Far Clip Plane quá gần: object biến mất khi ở xa
/// 3. Occlusion Culling lỗi: object biến mất dù đứng gần
/// 
/// Script này tự chạy khi scene load, không cần gắn tay lên bất kỳ GameObject nào.
/// </summary>
[DefaultExecutionOrder(-100)]
public class MuseumRenderFixer : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Far Clip Plane của camera (m). 80m bao trọn toàn bộ khuôn viên bảo tàng mà không vẽ thừa không gian vô tận.")]
    public float cameraFarClipPlane = 80f;

    [Tooltip("Near Clip Plane của camera (m). 0.015m giúp khi đứng sát vào tủ kính/hiện vật không bị cắt lẹm.")]
    public float cameraNearClipPlane = 0.015f;

    [Tooltip("Tắt Occlusion Culling vì dữ liệu bake bị lỗi với vách kính/hộp kính, tránh hiện tượng vật thể biến mất/nhấp nháy.")]
    public bool disableOcclusionCulling = true;

    [Header("Z-Fighting Fix")]
    [Tooltip("Bật sửa Z-Fighting bằng cách tách nhẹ mesh vật lý để sàn/tường không bị nhấp nháy chồng lấn.")]
    public bool fixZFighting = true;

    [Tooltip("Danh sách tên layer (chứa tường/sàn) cần kiểm tra Z-Fighting. Để trống = kiểm tra tất cả.")]
    public string[] layersToCheck = new string[] { "Default" };

    [Header("Lighting Fix")]
    [Tooltip("Tự động kiểm tra và bật toàn bộ đèn trong bảo tàng, sửa các đèn bị tắt, ẩn hoặc thiếu sáng.")]
    public bool fixMuseumLights = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoInit()
    {
        // Tự động tạo nếu chưa có trong scene
        if (FindObjectOfType<MuseumRenderFixer>() == null)
        {
            GameObject go = new GameObject("[MuseumRenderFixer]");
            go.AddComponent<MuseumRenderFixer>();
            DontDestroyOnLoad(go);
            Debug.Log("[MuseumRenderFixer] Đã tự động khởi tạo.");
        }
    }

    IEnumerator Start()
    {
        // Chờ 1 frame để toàn bộ scene load xong (kể cả XR Rig)
        yield return null;
        yield return null;

        ApplyAllFixes();
    }

    /// <summary>
    /// Áp dụng toàn bộ các bản sửa lỗi render
    /// </summary>
    public void ApplyAllFixes()
    {
        int fixCount = 0;

        fixCount += FixAllCameras();

        if (fixZFighting)
            fixCount += FixZFightingObjects();

        if (fixMuseumLights)
            fixCount += FixAllLights();

        Debug.Log($"[MuseumRenderFixer] Đã áp dụng {fixCount} bản sửa lỗi render.");
    }

    /// <summary>
    /// Sửa tất cả Camera trong scene:
    /// - Tối ưu Far Clip Plane (80m)
    /// - Tối ưu Near Clip Plane (0.015m tránh cắt vật thể gần)
    /// - Tắt Occlusion Culling để tránh lỗi biến mất vật thể sau kính/tủ
    /// </summary>
    private int FixAllCameras()
    {
        Camera[] allCameras = FindObjectsOfType<Camera>(true); // true = include inactive
        int count = 0;

        foreach (Camera cam in allCameras)
        {
            // Bỏ qua security camera (không phải main camera)
            if (cam.name.ToLower().Contains("security") || cam.targetTexture != null)
                continue;

            bool changed = false;

            // Sửa Far Clip Plane (80m bao trọn bảo tàng, giảm tải overdraw)
            if (Mathf.Abs(cam.farClipPlane - cameraFarClipPlane) > 0.5f)
            {
                Debug.Log($"[MuseumRenderFixer] Camera '{cam.name}': Far Clip {cam.farClipPlane} → {cameraFarClipPlane}");
                cam.farClipPlane = cameraFarClipPlane;
                changed = true;
            }

            // Sửa Near Clip Plane (0.015m giúp đứng sát tủ kính/tranh không bị cắt lẹm)
            if (Mathf.Abs(cam.nearClipPlane - cameraNearClipPlane) > 0.005f)
            {
                Debug.Log($"[MuseumRenderFixer] Camera '{cam.name}': Near Clip {cam.nearClipPlane} → {cameraNearClipPlane}");
                cam.nearClipPlane = cameraNearClipPlane;
                changed = true;
            }

            // Tắt Occlusion Culling để tránh vật thể sau vách kính/tủ kính bị biến mất nhầm
            if (disableOcclusionCulling && cam.useOcclusionCulling)
            {
                Debug.Log($"[MuseumRenderFixer] Camera '{cam.name}': Đã tắt Occlusion Culling để tránh ẩn vật thể nhầm.");
                cam.useOcclusionCulling = false;
                changed = true;
            }

            if (changed) count++;
        }

        if (count == 0)
            Debug.Log("[MuseumRenderFixer] Tất cả camera đã có cấu hình tốt, không cần sửa.");

        return count;
    }

    /// <summary>
    /// Phát hiện và sửa Z-Fighting:
    /// Tìm các Renderer nằm chồng lên nhau (cùng vị trí Y), tách ra một chút.
    /// Áp dụng OffsetFactor/OffsetUnits trên material để giảm Z-Fighting.
    /// </summary>
    private int FixZFightingObjects()
    {
        // Lấy tất cả MeshRenderer trong scene
        MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>(true);
        int count = 0;

        // Nhóm các renderer theo vị trí Y (làm tròn 2 chữ số thập phân)
        Dictionary<float, List<MeshRenderer>> byY = new Dictionary<float, List<MeshRenderer>>();

        foreach (var r in renderers)
        {
            if (r == null) continue;
            float yKey = Mathf.Round(r.transform.position.y * 100f) / 100f;
            if (!byY.ContainsKey(yKey))
                byY[yKey] = new List<MeshRenderer>();
            byY[yKey].Add(r);
        }

        // Với mỗi nhóm có 2+ renderer cùng Y → nguy cơ Z-Fighting
        foreach (var pair in byY)
        {
            if (pair.Value.Count < 2) continue;

            List<MeshRenderer> group = pair.Value;

            // Kiểm tra xem có 2 object nào overlap XZ không
            for (int i = 0; i < group.Count; i++)
            {
                for (int j = i + 1; j < group.Count; j++)
                {
                    if (group[i] == null || group[j] == null) continue;

                    Bounds a = group[i].bounds;
                    Bounds b = group[j].bounds;

                    // Nếu 2 bounds giao nhau trên XZ và cùng Y → Z-Fighting
                    if (BoundsOverlapXZ(a, b) && Mathf.Abs(a.center.y - b.center.y) < 0.005f)
                    {
                        // Sửa bằng cách dịch nhẹ object nhỏ hơn lên 1.5mm (vừa hết nhấp nháy, vừa giữ nguyên Material để SRP Batcher hoạt động)
                        MeshRenderer smaller = a.size.magnitude <= b.size.magnitude ? group[i] : group[j];

                        if (!IsChildOf(group[i].transform, group[j].transform))
                        {
                            Vector3 pos = smaller.transform.position;
                            pos.y += 0.0015f; // Tách ra 1.5mm
                            smaller.transform.position = pos;

                            Debug.Log($"[MuseumRenderFixer] Z-Fighting: Dịch '{smaller.gameObject.name}' lên 0.0015m (Bảo toàn SRP Batcher)");
                            count++;
                        }
                    }
                }
            }
        }

        if (count == 0)
            Debug.Log("[MuseumRenderFixer] Không phát hiện Z-Fighting.");

        return count;
    }

    private bool BoundsOverlapXZ(Bounds a, Bounds b)
    {
        return Mathf.Abs(a.center.x - b.center.x) < (a.extents.x + b.extents.x) &&
               Mathf.Abs(a.center.z - b.center.z) < (a.extents.z + b.extents.z);
    }

    private bool IsStaticObject(GameObject go)
    {
        return go.isStatic;
    }

    private bool IsChildOf(Transform a, Transform b)
    {
        return a.IsChildOf(b) || b.IsChildOf(a);
    }

    /// <summary>
    /// Kiểm tra và sửa toàn bộ hệ thống đèn trong bảo tàng:
    /// - Đảm bảo QualitySettings.pixelLightCount = 8
    /// - Kích hoạt các GameObject đèn bị ẩn
    /// - Bật các component Light bị tắt
    /// - Sửa các đèn bị yếu sáng (Gallery Spot 5 (12), Point Light (1))
    /// - Đảm bảo tầm chiếu sáng và CullingMask chính xác
    /// </summary>
    private int FixAllLights()
    {
        // Đặt mức tối đa pixel lights cho URP/Quality
        QualitySettings.pixelLightCount = 8;

        Light[] allLights = FindObjectsOfType<Light>(true);
        int fixedCount = 0;

        foreach (Light l in allLights)
        {
            if (l == null) continue;

            bool changed = false;

            // 1. Kích hoạt GameObject nếu bị tắt
            if (!l.gameObject.activeSelf)
            {
                Debug.Log($"[MuseumRenderFixer] Kích hoạt GameObject đèn: '{l.gameObject.name}'");
                l.gameObject.SetActive(true);
                changed = true;
            }

            // 2. Kích hoạt component Light nếu bị tắt
            if (!l.enabled)
            {
                Debug.Log($"[MuseumRenderFixer] Bật component Light: '{l.name}'");
                l.enabled = true;
                changed = true;
            }

            // 3. Khôi phục cường độ nếu đèn bị yếu hoặc tắt (intensity <= 0.05 hoặc thấp bất thường)
            if (l.type == LightType.Spot)
            {
                if (l.intensity < 4.0f)
                {
                    Debug.Log($"[MuseumRenderFixer] Khôi phục độ sáng Spot Light '{l.name}': {l.intensity} → 6.0");
                    l.intensity = 6.0f;
                    changed = true;
                }
                if (l.range < 5.0f)
                {
                    l.range = 9.0f;
                    changed = true;
                }
            }
            else if (l.type == LightType.Point)
            {
                // Bỏ qua đèn viền LED và đèn màn hình TV vì có thiết lập riêng
                bool isSpecial = l.name.Contains("Border") || l.name.Contains("TvScreen");
                if (!isSpecial && l.intensity < 5.0f)
                {
                    Debug.Log($"[MuseumRenderFixer] Khôi phục độ sáng Point Light '{l.name}': {l.intensity} → 10.0");
                    l.intensity = 10.0f;
                    changed = true;
                }
                if (!isSpecial && l.range < 50.0f)
                {
                    l.range = 1000.0f;
                    changed = true;
                }
            }
            else if (l.type == LightType.Directional)
            {
                if (l.intensity < 0.5f)
                {
                    Debug.Log($"[MuseumRenderFixer] Khôi phục độ sáng Sun Light: {l.intensity} → 1.1");
                    l.intensity = 1.1f;
                    changed = true;
                }
            }

            // 4. Đảm bảo CullingMask chiếu vào Default layer
            int defaultLayer = LayerMask.NameToLayer("Default");
            if (defaultLayer >= 0 && (l.cullingMask & (1 << defaultLayer)) == 0)
            {
                l.cullingMask |= (1 << defaultLayer);
                changed = true;
            }

            if (changed) fixedCount++;
        }

        Debug.Log($"[MuseumRenderFixer] Hệ thống đèn: Đã kiểm tra {allLights.Length} đèn ({fixedCount} đèn được tinh chỉnh).");
        return fixedCount;
    }

    // ===== Editor Helper =====
#if UNITY_EDITOR
    [ContextMenu("Chạy sửa lỗi render ngay bây giờ")]
    void RunFixesInEditor()
    {
        ApplyAllFixes();
    }
#endif
}

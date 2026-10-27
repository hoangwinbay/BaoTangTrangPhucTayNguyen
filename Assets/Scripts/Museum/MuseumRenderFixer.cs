using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        yield return null;
        yield return null;

        ApplyAllFixes();
    }

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

    private int FixAllCameras()
    {
        Camera[] allCameras = FindObjectsOfType<Camera>(true);
        int count = 0;

        foreach (Camera cam in allCameras)
        {
            if (cam.name.ToLower().Contains("security") || cam.targetTexture != null)
                continue;

            bool changed = false;

            if (Mathf.Abs(cam.farClipPlane - cameraFarClipPlane) > 0.5f)
            {
                Debug.Log($"[MuseumRenderFixer] Camera '{cam.name}': Far Clip {cam.farClipPlane} → {cameraFarClipPlane}");
                cam.farClipPlane = cameraFarClipPlane;
                changed = true;
            }

            if (Mathf.Abs(cam.nearClipPlane - cameraNearClipPlane) > 0.005f)
            {
                Debug.Log($"[MuseumRenderFixer] Camera '{cam.name}': Near Clip {cam.nearClipPlane} → {cameraNearClipPlane}");
                cam.nearClipPlane = cameraNearClipPlane;
                changed = true;
            }

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

    private int FixZFightingObjects()
    {
        MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>(true);
        int count = 0;

        Dictionary<float, List<MeshRenderer>> byY = new Dictionary<float, List<MeshRenderer>>();

        foreach (var r in renderers)
        {
            if (r == null) continue;
            float yKey = Mathf.Round(r.transform.position.y * 100f) / 100f;
            if (!byY.ContainsKey(yKey))
                byY[yKey] = new List<MeshRenderer>();
            byY[yKey].Add(r);
        }

        foreach (var pair in byY)
        {
            if (pair.Value.Count < 2) continue;

            List<MeshRenderer> group = pair.Value;

            for (int i = 0; i < group.Count; i++)
            {
                for (int j = i + 1; j < group.Count; j++)
                {
                    if (group[i] == null || group[j] == null) continue;

                    Bounds a = group[i].bounds;
                    Bounds b = group[j].bounds;

                    if (BoundsOverlapXZ(a, b) && Mathf.Abs(a.center.y - b.center.y) < 0.005f)
                    {
                        MeshRenderer smaller = a.size.magnitude <= b.size.magnitude ? group[i] : group[j];

                        if (!IsChildOf(group[i].transform, group[j].transform))
                        {
                            Vector3 pos = smaller.transform.position;
                            pos.y += 0.0015f;
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

    private int FixAllLights()
    {
        QualitySettings.pixelLightCount = 8;

        Light[] allLights = FindObjectsOfType<Light>(true);
        int fixedCount = 0;

        foreach (Light l in allLights)
        {
            if (l == null) continue;

            bool changed = false;

            if (!l.gameObject.activeSelf)
            {
                Debug.Log($"[MuseumRenderFixer] Kích hoạt GameObject đèn: '{l.gameObject.name}'");
                l.gameObject.SetActive(true);
                changed = true;
            }

            if (!l.enabled)
            {
                Debug.Log($"[MuseumRenderFixer] Bật component Light: '{l.name}'");
                l.enabled = true;
                changed = true;
            }

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

#if UNITY_EDITOR
    [ContextMenu("Chạy sửa lỗi render ngay bây giờ")]
    void RunFixesInEditor()
    {
        ApplyAllFixes();
    }
#endif
}

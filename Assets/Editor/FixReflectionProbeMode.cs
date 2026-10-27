using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;

public static class FixReflectionProbeMode
{
    private const string MenuPrefix = "Bảo Tàng/";

    [MenuItem(MenuPrefix + "5. Khắc Phục Lỗi Reflection Probe (Realtime -> Baked)", false, 5)]
    [MenuItem("Tools/Lighting/Fix Reflection Probe Realtime Error", false, 100)]
    public static void FixProbes()
    {
        ReflectionProbe[] probes = UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int fixedCount = 0;

        foreach (var probe in probes)
        {
            if (probe.mode == ReflectionProbeMode.Realtime)
            {
                Undo.RecordObject(probe, "Fix Reflection Probe Mode to Baked");
                probe.mode = ReflectionProbeMode.Baked;
                EditorUtility.SetDirty(probe);
                fixedCount++;
                Debug.Log($"[FixReflectionProbe] Đã chuyển đổi Reflection Probe '{probe.gameObject.name}' từ Realtime sang Baked thành công.");
            }
        }

        if (fixedCount > 0)
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                EditorSceneManager.SaveOpenScenes();
            }
            AssetDatabase.SaveAssets();

            string msg = $"Đã tự động chuyển đổi thành công {fixedCount}/{probes.Length} Reflection Probe từ chế độ 'Realtime' (mode 1) sang 'Baked' (mode 0)!\n\n" +
                         $"✓ Đã lưu thay đổi vào Scene và Assets.\n" +
                         $"✓ Khắc phục triệt để lỗi 'Unknown reflection probe mode: 1'.\n" +
                         $"✓ Tối ưu hóa FPS cho kính VR / Museum (loại bỏ chi phí render cubemap liên tục).\n\n" +
                         $"Bây giờ bạn có thể bấm 'Generate Lighting' hoặc nướng Reflection Probes bình thường!";

            EditorUtility.DisplayDialog("Sửa Lỗi Reflection Probe Thành Công", msg, "Hoàn tất");
            Debug.Log($"[FixReflectionProbe] {msg}");
        }
        else
        {
            EditorUtility.DisplayDialog("Kiểm Tra Reflection Probe",
                $"Tất cả {probes.Length} Reflection Probe trong Scene hiện tại đều đã ở chế độ Baked hoặc Custom hợp lệ.\n" +
                $"Không phát hiện Reflection Probe nào ở chế độ Realtime (mode 1).", "OK");
        }
    }

    public static int FixProbesInternal()
    {
        ReflectionProbe[] probes = UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int fixedCount = 0;

        foreach (var probe in probes)
        {
            if (probe.mode == ReflectionProbeMode.Realtime)
            {
                Undo.RecordObject(probe, "Fix Reflection Probe Mode to Baked");
                probe.mode = ReflectionProbeMode.Baked;
                EditorUtility.SetDirty(probe);
                fixedCount++;
                Debug.Log($"[FixReflectionProbe] Đã chuyển đổi probe '{probe.gameObject.name}' sang Baked.");
            }
        }

        if (fixedCount > 0)
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                EditorSceneManager.SaveOpenScenes();
            }
            AssetDatabase.SaveAssets();
        }
        return fixedCount;
    }
}

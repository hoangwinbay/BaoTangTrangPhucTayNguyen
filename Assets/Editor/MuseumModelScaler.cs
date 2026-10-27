using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

static class MuseumModelScaler
{
    const float TargetHeight = 1.7f;
    const float FloorY = 0f;
    const string ModelFolder = "Assets/3d";

    static readonly HashSet<Object> processed = new HashSet<Object>();
    static HashSet<string> knownModelNames;
    static bool markOnlyNextScan = true;

    static void Init()
    {
    }

    static void QueueScan()
    {
        EditorApplication.delayCall -= Scan;
        EditorApplication.delayCall += Scan;
    }

    static void Scan()
    {
        bool markOnly = markOnlyNextScan;
        markOnlyNextScan = false;

        if (!markOnly && knownModelNames == null)
        {
            knownModelNames = new HashSet<string>();
            foreach (var guid in AssetDatabase.FindAssets("", new[] { ModelFolder }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(".glb", System.StringComparison.OrdinalIgnoreCase))
                    knownModelNames.Add(System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }

        if (!markOnly && (knownModelNames == null || knownModelNames.Count == 0)) return;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            foreach (var root in SceneManager.GetSceneAt(i).GetRootGameObjects())
            {
                if (!processed.Add(root)) continue;
                if (markOnly || !knownModelNames.Contains(root.name)) continue;
                Normalize(root, snapFeetToFloor: false, log: true);
            }
        }
    }

    static void Normalize(GameObject go, bool snapFeetToFloor, bool log)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            if (log) Debug.LogWarning($"[Bảo tàng] '{go.name}' không có Renderer nào để đo.", go);
            return;
        }

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
        if (b.size.y <= 0.0001f) return;

        float scale = TargetHeight / b.size.y;
        if (Mathf.Abs(scale - 1f) < 0.005f) return;

        var t = go.transform;
        Undo.RecordObject(t, "Chuẩn hóa kích thước theo player");
        Vector3 pivot = t.position;
        float newMinY = pivot.y + (b.min.y - pivot.y) * scale;

        t.localScale *= scale;
        if (snapFeetToFloor)
            t.position += Vector3.up * (FloorY - newMinY);

        EditorSceneManager.MarkSceneDirty(go.scene);
        if (log)
            Debug.Log($"[Bảo tàng] '{go.name}': {b.size.y:0.00}m → {TargetHeight}m (scale ×{scale:0.###})" +
                      (snapFeetToFloor ? ", chân chạm sàn y=" + FloorY : ""), go);
    }

    [MenuItem("Tools/Bảo tàng/Mô hình — chuẩn hóa theo player &#n")]
    static void NormalizeSelected()
    {
        foreach (var go in Selection.gameObjects) Normalize(go, false, true);
    }

    [MenuItem("Tools/Bảo tàng/Mô hình — chuẩn hóa + đặt chân chạm sàn &#m")]
    static void NormalizeSelectedAndSnap()
    {
        foreach (var go in Selection.gameObjects) Normalize(go, true, true);
    }
}

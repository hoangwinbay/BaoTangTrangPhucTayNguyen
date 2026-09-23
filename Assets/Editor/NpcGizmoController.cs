#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class NpcGizmoController
{
    // ClassID của NavMeshAgent trong Unity là 195
    private const int NavMeshAgentClassID = 195;

    static NpcGizmoController()
    {
        // Tự động tắt Gizmo và đường quỹ đạo màu xanh của NavMeshAgent khi khởi động / compile
        EditorApplication.delayCall += () =>
        {
            SetGizmoState(NavMeshAgentClassID, false);
        };
    }

    [MenuItem("Tools/NPC/Ẩn Quỹ Đạo NPC (Tắt Gizmo NavMeshAgent)")]
    public static void DisableNavMeshAgentGizmo()
    {
        SetGizmoState(NavMeshAgentClassID, false);
        Debug.Log("[NPC] Đã ẩn vạch quỹ đạo di chuyển màu xanh của NavMeshAgent.");
    }

    [MenuItem("Tools/NPC/Hiện Quỹ Đạo NPC (Bật Gizmo NavMeshAgent)")]
    public static void EnableNavMeshAgentGizmo()
    {
        SetGizmoState(NavMeshAgentClassID, true);
        Debug.Log("[NPC] Đã bật lại vạch quỹ đạo di chuyển của NavMeshAgent.");
    }

    public static void SetGizmoState(int classID, bool enabled)
    {
        try
        {
            Type annotationUtility = Type.GetType("UnityEditor.AnnotationUtility, UnityEditor");
            if (annotationUtility != null)
            {
                MethodInfo setGizmoEnabled = annotationUtility.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                if (setGizmoEnabled != null)
                {
                    setGizmoEnabled.Invoke(null, new object[] { classID, "", enabled ? 1 : 0 });
                }

                MethodInfo setIconEnabled = annotationUtility.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                if (setIconEnabled != null)
                {
                    setIconEnabled.Invoke(null, new object[] { classID, "", enabled ? 1 : 0 });
                }

                SceneView.RepaintAll();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[NPC] Không thể thay đổi Gizmo qua AnnotationUtility: " + ex.Message);
        }
    }
}
#endif

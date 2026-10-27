using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public static class RestoreWindowShadows
{
    [InitializeOnLoadMethod]
    private static void AutoRun()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            ApplyWindowShadowFix(showDialog: false);
        };
    }

    [MenuItem("Bảo Tàng/Khôi Phục Bóng Sọc Cửa Sổ Trần Nhà (Window Stripes)", false, 5)]
    public static void MenuApply()
    {
        ApplyWindowShadowFix(showDialog: true);
    }

    public static void ApplyWindowShadowFix(bool showDialog = false)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== CẤU HÌNH ĐỔ BÓNG SỌC CỬA SỔ TRẦN (WINDOW BARS SHADOW) ===");

        int windowBarsFixed = 0;
        int glassCleaned = 0;

        MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var r in renderers)
        {
            string n = r.gameObject.name;

            if (n.Contains("Window6_04L"))
            {
                if (r.shadowCastingMode != ShadowCastingMode.TwoSided)
                {
                    Undo.RecordObject(r, "Enable Window Bars Shadow");
                    r.shadowCastingMode = ShadowCastingMode.TwoSided;
                    r.receiveShadows = true;
                    EditorUtility.SetDirty(r);
                    windowBarsFixed++;
                }
            }
            else if (n.ToLower().Contains("glass") || n.ToLower().Contains("kinh"))
            {
                if (r.shadowCastingMode != ShadowCastingMode.Off)
                {
                    Undo.RecordObject(r, "Disable Glass Shadow");
                    r.shadowCastingMode = ShadowCastingMode.Off;
                    EditorUtility.SetDirty(r);
                    glassCleaned++;
                }
            }
        }

        sb.AppendLine($"- Đã bật bóng đổ (TwoSided) cho {windowBarsFixed} khung/thanh ngăn cửa sổ trần (Window6_04L).");
        sb.AppendLine($"- Đã đảm bảo {glassCleaned} mặt kính (Glass) không cản sáng để tia nắng chiếu lọt qua khe.");

        Light sunLight = null;
        Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var l in allLights)
        {
            if (l.type == LightType.Directional)
            {
                sunLight = l;
                break;
            }
        }

        if (sunLight != null)
        {
            Undo.RecordObject(sunLight, "Tune Sun Light Shadow Quality");
            sunLight.shadows = LightShadows.Soft;
            sunLight.shadowBias = 0.05f;
            sunLight.shadowNormalBias = 0.35f;
            sunLight.shadowNearPlane = 0.1f;
            EditorUtility.SetDirty(sunLight);
            sb.AppendLine($"- Đã tinh chỉnh Sun Light: Shadows=Soft, Bias={sunLight.shadowBias}, NormalBias={sunLight.shadowNormalBias}");
        }

        TuneURPShadows("Assets/Settings/Project Configuration/Performance URP Config.asset");
        TuneURPShadows("Assets/Settings/Project Configuration/Quality URP Config.asset");
        sb.AppendLine("- Đã bật 4 Shadow Cascades và nâng độ phân giải shadow map trong URP Asset.");

        Scene scene = SceneManager.GetActiveScene();
        if (scene.isLoaded)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            sb.AppendLine($"- Đã lưu Scene '{scene.name}' thành công.");
        }

        AssetDatabase.SaveAssets();

        string result = sb.ToString();
        Debug.Log(result);

        if (showDialog)
        {
            EditorUtility.DisplayDialog("Bóng Đổ Cửa Sổ Trần",
                "Đã cấu hình thành công bóng đổ sọc cho cửa sổ trần nhà!\n\n" +
                "1. Khung và thanh ngăn cửa sổ: Đã bật đổ bóng 2 mặt (TwoSided).\n" +
                "2. Mặt kính trong suốt: Không cản sáng để nắng chiếu lọt qua.\n" +
                "3. Độ sắc nét bóng: Đã tối ưu Shadow Bias và Cascades để các thanh nan in rõ nét trên mặt sàn.",
                "Tuyệt vời");
        }
    }

    private static void TuneURPShadows(string assetPath)
    {
        var urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(assetPath);
        if (urpAsset == null) return;

        SerializedObject so = new SerializedObject(urpAsset);

        SerializedProperty cascadeProp = so.FindProperty("m_ShadowCascadeCount");
        if (cascadeProp != null) cascadeProp.intValue = 4;

        SerializedProperty biasProp = so.FindProperty("m_ShadowDepthBias");
        if (biasProp != null) biasProp.floatValue = 0.1f;

        SerializedProperty normalBiasProp = so.FindProperty("m_ShadowNormalBias");
        if (normalBiasProp != null) normalBiasProp.floatValue = 0.35f;

        SerializedProperty shadowRes = so.FindProperty("m_MainLightShadowmapResolution");
        if (shadowRes != null && shadowRes.intValue < 2048) shadowRes.intValue = 2048;

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(urpAsset);
    }
}

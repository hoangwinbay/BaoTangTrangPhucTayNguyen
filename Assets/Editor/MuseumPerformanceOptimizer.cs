using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class MuseumPerformanceOptimizer
{
    private const string MenuPrefix = "Bảo Tàng/";

    [InitializeOnLoadMethod]
    private static void AutoInit()
    {
        EditorApplication.delayCall += () =>
        {
            RunDiagnostic(showDialog: false);

            int standardCount = CountStandardMaterials();
            if (standardCount > 0)
            {
                Debug.Log($"[MuseumPerformanceOptimizer] Phát hiện {standardCount} Material dùng shader Standard cũ làm vỡ SRP Batcher. Đang tự động nâng cấp sang URP Lit...");
                UpgradeAllMaterialsToURPLit();
            }
        };
    }

    [MenuItem(MenuPrefix + "1. Tối Ưu Hiệu Năng Toàn Cảnh (Khắc Phục Lag Bao Quát)", false, 1)]
    public static void MenuOptimize()
    {
        OptimizeAll(showDialog: true);
    }

    [MenuItem(MenuPrefix + "2. Chẩn Đoán Hiệu Năng Chi Tiết (Performance Diagnostic)", false, 2)]
    public static void MenuDiagnostic()
    {
        RunDiagnostic(showDialog: true);
    }

    [MenuItem(MenuPrefix + "3. Khôi Phục Đổ Bóng Tất Cả Đèn (Nghiệm Thu)", false, 3)]
    public static void MenuRestoreLightShadows()
    {
        RestoreAllLightShadows();
    }

    [MenuItem(MenuPrefix + "4. Nướng Occlusion Culling (Tối Ưu Ẩn Phòng Bị Che Khuất)", false, 4)]
    public static void MenuBakeOcclusion()
    {
        BakeOcclusion();
    }

    public static void OptimizeAll(bool showDialog = true)
    {
        int lightsOptimized = 0;
        int materialsInstanced = 0;
        int materialsUpgraded = 0;
        int shadowCastersOff = 0;
        int staticObjectsTagged = 0;

        Light[] lights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var l in lights)
        {
            if (l.type == LightType.Directional)
            {
                if (l.shadows == LightShadows.None)
                {
                    Undo.RecordObject(l, "Enable Directional Shadow");
                    l.shadows = LightShadows.Soft;
                    EditorUtility.SetDirty(l);
                }
            }
            else
            {
                if (l.shadows != LightShadows.None)
                {
                    Undo.RecordObject(l, "Disable Additional Light Shadow");
                    l.shadows = LightShadows.None;
                    EditorUtility.SetDirty(l);
                    lightsOptimized++;
                }
            }
        }

        materialsUpgraded = UpgradeAllMaterialsToURPLit();

        string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        foreach (var guid in matGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null && !mat.enableInstancing)
            {
                mat.enableInstancing = true;
                EditorUtility.SetDirty(mat);
                materialsInstanced++;
            }
        }

        MeshRenderer[] renderers = UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var r in renderers)
        {
            string n = r.gameObject.name.ToLower();
            if (n.Contains("light_") || n.Contains("led_light") || n.Contains("vent") ||
                n.Contains("glass") || n.Contains("kinh"))
            {
                if (r.shadowCastingMode != ShadowCastingMode.Off)
                {
                    Undo.RecordObject(r, "Disable Small Props Shadow");
                    r.shadowCastingMode = ShadowCastingMode.Off;
                    EditorUtility.SetDirty(r);
                    shadowCastersOff++;
                }
            }
        }

        staticObjectsTagged = TagStaticArchitecture();

        OptimizeURPConfig("Assets/Settings/Project Configuration/Performance URP Config.asset", 2048, 8, 35f);
        OptimizeURPConfig("Assets/Settings/Project Configuration/Quality URP Config.asset", 2048, 8, 40f);

        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.isLoaded)
        {
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveOpenScenes();
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RunDiagnostic(showDialog: false);

        string summary = $"[Bảo Tàng] Tối ưu hiệu năng toàn cảnh hoàn tất!\n" +
                         $"- Nâng cấp Material sang URP Lit (tương thích SRP Batcher): {materialsUpgraded}\n" +
                         $"- Tắt Cast Shadows vật thể trần/phụ (Light_09, Vent, Glass): {shadowCastersOff}\n" +
                         $"- Số Material đã bật GPU Instancing: {materialsInstanced}\n" +
                         $"- Đèn phụ nội thất đã tắt đổ bóng: {lightsOptimized}\n" +
                         $"- Vật thể kiến trúc đã gắn Static: {staticObjectsTagged}\n" +
                         $"100% mô hình và tài nguyên được bảo toàn nguyên vẹn!";

        Debug.Log(summary);

        if (showDialog)
        {
            EditorUtility.DisplayDialog("Tối Ưu Hiệu Năng Hoàn Tất",
                $"Đã hoàn thành tối ưu hóa chuyên sâu toàn bộ dự án!\n\n" +
                $"• Material nâng cấp sang URP Lit: {materialsUpgraded} vật liệu (Khắc phục vỡ batch)\n" +
                $"• Vật thể trần/phụ tắt đổ bóng thừa: {shadowCastersOff} vật thể (Giảm tải đổ bóng mặt trời)\n" +
                $"• Material bật GPU Instancing: {materialsInstanced} vật liệu\n" +
                $"• Cấu hình URP: Đã tối ưu Shadow Distance, SRP Batcher & Dynamic Batching\n\n" +
                $"Mọi mô hình được giữ nguyên vẹn 100%.\n" +
                $"Mẹo bổ sung: Bạn hãy bấm 'Bảo Tàng > 4. Nướng Occlusion Culling' để ẩn các phòng bị che khuất khi nhìn từ xa!",
                "Đồng Ý");
        }
    }

    private static int CountStandardMaterials()
    {
        int count = 0;
        string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        foreach (var guid in matGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null && mat.shader != null)
            {
                if (mat.shader.name == "Standard" || mat.shader.name == "Standard (Specular setup)" || mat.shader.name.StartsWith("Legacy Shaders/"))
                    count++;
            }
        }
        return count;
    }

    private static int UpgradeAllMaterialsToURPLit()
    {
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null) return 0;

        int upgraded = 0;
        string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        foreach (var guid in matGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null || mat.shader == null) continue;

            if (mat.shader.name == "Standard" || mat.shader.name == "Standard (Specular setup)" || mat.shader.name.StartsWith("Legacy Shaders/"))
            {
                Texture mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;
                Color baseColor = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;
                Texture bumpMap = mat.HasProperty("_BumpMap") ? mat.GetTexture("_BumpMap") : null;
                float metallic = mat.HasProperty("_Metallic") ? mat.GetFloat("_Metallic") : 0f;
                float smoothness = mat.HasProperty("_Glossiness") ? mat.GetFloat("_Glossiness") : 0.5f;

                Undo.RecordObject(mat, "Upgrade Material to URP Lit");
                mat.shader = urpLit;

                if (mainTex != null) mat.SetTexture("_BaseMap", mainTex);
                mat.SetColor("_BaseColor", baseColor);
                if (bumpMap != null) mat.SetTexture("_BumpMap", bumpMap);
                mat.SetFloat("_Metallic", metallic);
                mat.SetFloat("_Smoothness", smoothness);
                mat.enableInstancing = true;

                EditorUtility.SetDirty(mat);
                upgraded++;
            }
        }
        return upgraded;
    }

    private static int TagStaticArchitecture()
    {
        int staticObjectsTagged = 0;
        MeshRenderer[] renderers = UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var r in renderers)
        {
            GameObject go = r.gameObject;

            if (go.GetComponentInParent<Animator>() != null ||
                go.GetComponentInParent<Animation>() != null ||
                go.GetComponentInParent<Rigidbody>() != null ||
                go.name.Contains("NPC") || go.name.Contains("XR") || go.name.Contains("Camera") ||
                go.name.Contains("glass_partition_41_003") || go.name.Contains("glass_partition_41_004") ||
                go.GetComponentInParent<AutomaticSlidingDoor>() != null)
            {
                continue;
            }

            StaticEditorFlags currentFlags = GameObjectUtility.GetStaticEditorFlags(go);
            StaticEditorFlags targetFlags = currentFlags |
                                           StaticEditorFlags.BatchingStatic |
                                           StaticEditorFlags.OccluderStatic |
                                           StaticEditorFlags.OccludeeStatic;

            if (currentFlags != targetFlags)
            {
                Undo.RecordObject(go, "Set Static Flags");
                GameObjectUtility.SetStaticEditorFlags(go, targetFlags);
                EditorUtility.SetDirty(go);
                staticObjectsTagged++;
            }
        }
        return staticObjectsTagged;
    }

    private static void BakeOcclusion()
    {
        if (StaticOcclusionCulling.isRunning)
        {
            EditorUtility.DisplayDialog("Occlusion Culling", "Quá trình nướng Occlusion Culling đang diễn ra trong nền. Vui lòng chờ vài giây!", "Đồng Ý");
            return;
        }

        TagStaticArchitecture();

        StaticOcclusionCulling.smallestHole = 0.5f;
        StaticOcclusionCulling.smallestOccluder = 1.0f;
        StaticOcclusionCulling.backfaceThreshold = 100f;
        StaticOcclusionCulling.Compute();

        Debug.Log("[Bảo Tàng] Đang tiến hành nướng Occlusion Culling...");
        EditorUtility.DisplayDialog("Occlusion Culling",
            "Đang tiến hành nướng Occlusion Culling!\n\n" +
            "Khi hoàn tất, các gian phòng và đồ vật nằm phía sau bức tường sẽ tự động được ẩn đi khi nhìn từ ngoài vào, giúp tăng vọt FPS khi di chuyển camera.", "Đồng Ý");
    }

    private static void OptimizeURPConfig(string assetPath, int mainShadowRes, int perObjectLightLimit, float shadowDistance)
    {
        UniversalRenderPipelineAsset urp = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(assetPath);
        if (urp == null) return;

        SerializedObject so = new SerializedObject(urp);
        so.Update();

        SerializedProperty pMainRes = so.FindProperty("m_MainLightShadowmapResolution");
        if (pMainRes != null) pMainRes.intValue = mainShadowRes;

        SerializedProperty pAddLightsLimit = so.FindProperty("m_AdditionalLightsPerObjectLimit");
        if (pAddLightsLimit != null) pAddLightsLimit.intValue = perObjectLightLimit;

        SerializedProperty pAddLightsShadows = so.FindProperty("m_AdditionalLightShadowsSupported");
        if (pAddLightsShadows != null) pAddLightsShadows.boolValue = false;

        SerializedProperty pShadowDist = so.FindProperty("m_ShadowDistance");
        if (pShadowDist != null) pShadowDist.floatValue = shadowDistance;

        SerializedProperty pDynamicBatch = so.FindProperty("m_SupportsDynamicBatching");
        if (pDynamicBatch != null) pDynamicBatch.boolValue = true;

        SerializedProperty pUseSRP = so.FindProperty("m_UseSRPBatcher");
        if (pUseSRP != null) pUseSRP.boolValue = true;

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(urp);
    }

    private static void RestoreURPConfig(string assetPath)
    {
        UniversalRenderPipelineAsset urp = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(assetPath);
        if (urp == null) return;

        SerializedObject so = new SerializedObject(urp);
        so.Update();

        SerializedProperty pMainRes = so.FindProperty("m_MainLightShadowmapResolution");
        if (pMainRes != null) pMainRes.intValue = 4096;

        SerializedProperty pMainLightShadows = so.FindProperty("m_MainLightShadowsSupported");
        if (pMainLightShadows != null) pMainLightShadows.boolValue = true;

        SerializedProperty pAddLightsMode = so.FindProperty("m_AdditionalLightsRenderingMode");
        if (pAddLightsMode != null) pAddLightsMode.intValue = 1;

        SerializedProperty pAddLightsLimit = so.FindProperty("m_AdditionalLightsPerObjectLimit");
        if (pAddLightsLimit != null) pAddLightsLimit.intValue = 8;

        SerializedProperty pAddLightsShadows = so.FindProperty("m_AdditionalLightShadowsSupported");
        if (pAddLightsShadows != null) pAddLightsShadows.boolValue = true;

        SerializedProperty pAddLightsRes = so.FindProperty("m_AdditionalLightsShadowmapResolution");
        if (pAddLightsRes != null) pAddLightsRes.intValue = 2048;

        SerializedProperty pShadowDist = so.FindProperty("m_ShadowDistance");
        if (pShadowDist != null) pShadowDist.floatValue = 50f;

        SerializedProperty pDynamicBatch = so.FindProperty("m_SupportsDynamicBatching");
        if (pDynamicBatch != null) pDynamicBatch.boolValue = true;

        SerializedProperty pUseSRP = so.FindProperty("m_UseSRPBatcher");
        if (pUseSRP != null) pUseSRP.boolValue = true;

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(urp);
    }

    private static void RestoreActivePipelineURP()
    {
        if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset currentUrp)
        {
            string path = AssetDatabase.GetAssetPath(currentUrp);
            if (!string.IsNullOrEmpty(path)) RestoreURPConfig(path);
        }
        if (QualitySettings.renderPipeline is UniversalRenderPipelineAsset qualityUrp)
        {
            string path = AssetDatabase.GetAssetPath(qualityUrp);
            if (!string.IsNullOrEmpty(path)) RestoreURPConfig(path);
        }
    }

    public static void RestoreAllLightShadows()
    {
        RestoreURPConfig("Assets/Settings/Project Configuration/Performance URP Config.asset");
        RestoreURPConfig("Assets/Settings/Project Configuration/Quality URP Config.asset");
        RestoreActivePipelineURP();

        Light[] lights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int restoredShadowCount = 0;
        int activatedLightCount = 0;
        int activatedGoCount = 0;
        int restoredIntensityCount = 0;

        foreach (var l in lights)
        {
            if (!l.gameObject.activeSelf)
            {
                Undo.RecordObject(l.gameObject, "Activate Light GameObject");
                l.gameObject.SetActive(true);
                EditorUtility.SetDirty(l.gameObject);
                activatedGoCount++;
            }

            if (!l.enabled)
            {
                Undo.RecordObject(l, "Enable Light Component");
                l.enabled = true;
                EditorUtility.SetDirty(l);
                activatedLightCount++;
            }

            if (l.shadows != LightShadows.Soft)
            {
                Undo.RecordObject(l, "Restore Light Shadow");
                l.shadows = LightShadows.Soft;
                EditorUtility.SetDirty(l);
                restoredShadowCount++;
            }

            if (l.intensity <= 0.05f)
            {
                Undo.RecordObject(l, "Restore Light Intensity");
                if (l.type == LightType.Directional) l.intensity = 1.1f;
                else if (l.type == LightType.Point) l.intensity = (l.name == "TvScreenLight") ? 3.0f : 5.0f;
                else if (l.type == LightType.Spot) l.intensity = 6.0f;
                EditorUtility.SetDirty(l);
                restoredIntensityCount++;
            }
        }

        int restoredPropsShadowCount = 0;
        MeshRenderer[] renderers = UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var r in renderers)
        {
            if (r.shadowCastingMode == ShadowCastingMode.Off)
            {
                string n = r.gameObject.name.ToLower();
                if (n.Contains("light_") || n.Contains("led_light") || n.Contains("vent") ||
                    n.Contains("glass") || n.Contains("kinh") || n.Contains("cube") || n.Contains("window"))
                {
                    Undo.RecordObject(r, "Restore Props Cast Shadow");
                    r.shadowCastingMode = ShadowCastingMode.On;
                    EditorUtility.SetDirty(r);
                    restoredPropsShadowCount++;
                }
            }
        }

        if (!EditorApplication.isPlaying)
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                EditorSceneManager.SaveOpenScenes();
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        SceneView.RepaintAll();

        RunDiagnostic(showDialog: false);

        string summary = $"[Bảo Tàng] Khôi phục toàn bộ hệ thống đèn & bóng đổ hoàn tất!\n" +
                         $"- Tổng số bóng đèn đã xử lý: {lights.Length}\n" +
                         $"- Bật đổ bóng Soft Shadows: {restoredShadowCount} đèn\n" +
                         $"- Bật lại component Light bị tắt: {activatedLightCount} đèn\n" +
                         $"- Kích hoạt GameObject bị ẩn: {activatedGoCount} đối tượng\n" +
                         $"- Cấu hình URP: Đã bật lại Additional Light Shadows, Limit 8 đèn, Shadow Distance 50m\n" +
                         $"- Khôi phục đổ bóng vật thể nội thất: {restoredPropsShadowCount} vật thể";

        Debug.Log(summary);

        EditorUtility.DisplayDialog("Khôi Phục Đổ Bóng & Đèn Thành Công",
            $"Đã khôi phục hoàn chỉnh hệ thống đèn & bóng đổ:\n\n" +
            $"• Bật đổ bóng Soft Shadows: {restoredShadowCount} đèn\n" +
            $"• Bật lại component/GameObject bị tắt: {activatedLightCount + activatedGoCount} đèn\n" +
            $"• Kích hoạt lại Additional Light Shadows trong cấu hình URP\n" +
            $"• Nâng giới hạn đèn trên mỗi vật thể lên 8 đèn (URP)\n" +
            $"• Khôi phục tầm chiếu bóng đổ Shadow Distance: 50m\n" +
            $"• Khôi phục đổ bóng vật thể trần/phụ: {restoredPropsShadowCount} vật thể",
            "Đồng Ý");
    }

    public static void RunDiagnostic(bool showDialog = false)
    {
        try
        {
            var scene = EditorSceneManager.GetActiveScene();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=================================================================");
            sb.AppendLine("        BÁO CÁO PHÂN TÍCH CHUYÊN SÂU HIỆU NĂNG BẢO TÀNG         ");
            sb.AppendLine("=================================================================");
            sb.AppendLine($"Thời gian: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Scene: {scene.name} (Đường dẫn: {scene.path}, IsLoaded: {scene.isLoaded})");

            ReflectionProbe[] probes = UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            sb.AppendLine($"\n--- 1. REFLECTION PROBES ({probes.Length} probe) ---");
            foreach (var p in probes)
            {
                sb.AppendLine($"  • '{p.name}': Mode={p.mode}, RefreshMode={p.refreshMode}, TimeSlicing={p.timeSlicingMode}, Resolution={p.resolution}, BoxSize={p.size}");
            }

            Camera[] cameras = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            sb.AppendLine($"\n--- 2. CAMERAS ({cameras.Length} camera) ---");
            foreach (var c in cameras)
            {
                sb.AppendLine($"  • '{c.name}': FarClip={c.farClipPlane}, FOV={c.fieldOfView}, OcclusionCulling={c.useOcclusionCulling}, ClearFlags={c.clearFlags}");
            }

            Light[] lights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            sb.AppendLine($"\n--- 3. HỆ THỐNG ĐÈN ({lights.Length} đèn) ---");
            int dirCount = 0, pointCount = 0, spotCount = 0, shadowCount = 0;
            foreach (var l in lights)
            {
                if (l.type == LightType.Directional) dirCount++;
                else if (l.type == LightType.Point) pointCount++;
                else if (l.type == LightType.Spot) spotCount++;
                if (l.shadows != LightShadows.None) shadowCount++;

                sb.AppendLine($"  • '{l.name}' (Parent: {(l.transform.parent ? l.transform.parent.name : "root")}): Type={l.type}, Mode={l.lightmapBakeType}, Shadows={l.shadows}, Range={l.range}, Intensity={l.intensity}");
            }
            sb.AppendLine($"  => Tổng Directional: {dirCount}, Point: {pointCount}, Spot: {spotCount}, Đang đổ bóng: {shadowCount}");

            MeshRenderer[] mrs = UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            SkinnedMeshRenderer[] smrs = UnityEngine.Object.FindObjectsByType<SkinnedMeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            long totalTris = 0;
            long totalVerts = 0;
            int staticBatchCount = 0;

            List<(string name, string meshName, int tris, int verts, string parent)> meshList = new List<(string, string, int, int, string)>();

            foreach (var r in mrs)
            {
                if ((GameObjectUtility.GetStaticEditorFlags(r.gameObject) & StaticEditorFlags.BatchingStatic) != 0)
                    staticBatchCount++;

                var mf = r.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    int tris = mf.sharedMesh.triangles.Length / 3;
                    int verts = mf.sharedMesh.vertexCount;
                    totalTris += tris;
                    totalVerts += verts;
                    meshList.Add((r.gameObject.name, mf.sharedMesh.name, tris, verts, r.transform.parent ? r.transform.parent.name : "root"));
                }
            }

            foreach (var s in smrs)
            {
                if (s.sharedMesh != null)
                {
                    int tris = s.sharedMesh.triangles.Length / 3;
                    int verts = s.sharedMesh.vertexCount;
                    totalTris += tris;
                    totalVerts += verts;
                    meshList.Add(("[SKINNED] " + s.gameObject.name, s.sharedMesh.name, tris, verts, s.transform.parent ? s.transform.parent.name : "root"));
                }
            }

            sb.AppendLine($"\n--- 4. TỔNG QUAN HÌNH HỌC (GEOMETRY) ---");
            sb.AppendLine($"  • Tổng MeshRenderer: {mrs.Length}");
            sb.AppendLine($"  • Tổng SkinnedMeshRenderer: {smrs.Length}");
            sb.AppendLine($"  • Tổng Tam Giác (Triangles): {totalTris:N0}");
            sb.AppendLine($"  • Tổng Đỉnh (Vertices): {totalVerts:N0}");
            sb.AppendLine($"  • Đã gắn Static Batching: {staticBatchCount}/{mrs.Length}");

            meshList.Sort((a, b) => b.tris.CompareTo(a.tris));
            sb.AppendLine($"\n--- 5. TOP 25 MÔ HÌNH NẶNG NHẤT (CAO TRIANGLE NHẤT) ---");
            int topCount = Math.Min(25, meshList.Count);
            for (int i = 0; i < topCount; i++)
            {
                var item = meshList[i];
                sb.AppendLine($"  {i + 1,2}. '{item.name}' (Mesh: '{item.meshName}', Parent: '{item.parent}'): {item.tris:N0} tris, {item.verts:N0} verts");
            }

            HashSet<Shader> shaders = new HashSet<Shader>();
            foreach (var r in mrs)
            {
                foreach (var m in r.sharedMaterials)
                {
                    if (m != null && m.shader != null) shaders.Add(m.shader);
                }
            }
            sb.AppendLine($"\n--- 6. CÁC SHADER ĐANG DÙNG ({shaders.Count} shader) ---");
            foreach (var s in shaders)
            {
                sb.AppendLine($"  • {s.name}");
            }

            string repDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../Docs/Reports"));
            if (!Directory.Exists(repDir)) Directory.CreateDirectory(repDir);
            string path = Path.Combine(repDir, "SceneDeepAnalysis.txt");
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            Debug.Log($"[MuseumPerformanceOptimizer] Đã ghi phân tích chuyên sâu tại: {path}");

            if (showDialog)
            {
                EditorUtility.DisplayDialog("Phân Tích Hiệu Năng",
                    $"Tổng Triangles: {totalTris:N0}\n" +
                    $"Tổng Đèn: {lights.Length} (Shadow Casters: {shadowCount})\n" +
                    $"Reflection Probes: {probes.Length}\n" +
                    $"MeshRenderers: {mrs.Length} (Static: {staticBatchCount})\n\n" +
                    $"Chi tiết đã xuất ra file Docs/Reports/SceneDeepAnalysis.txt", "Đóng");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[MuseumPerformanceOptimizer] Lỗi RunDiagnostic: {ex}");
        }
    }
}

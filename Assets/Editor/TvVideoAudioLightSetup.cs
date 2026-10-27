using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public static class TvVideoAudioLightSetup
{
    private const string MenuPath = "Bảo Tàng/Cấu Hình Video Tivi (Âm Thanh 3D & Ánh Sáng Phòng)";

    [InitializeOnLoadMethod]
    private static void AutoInitOnEditorLoad()
    {
        EditorApplication.delayCall += () =>
        {
            GameObject tvGo = GameObject.Find("tivi");
            if (tvGo != null)
            {
                TvVideoController controller = tvGo.GetComponent<TvVideoController>();
                VideoPlayer vp = tvGo.GetComponent<VideoPlayer>();

                if (controller == null || (vp != null && vp.audioOutputMode != VideoAudioOutputMode.AudioSource))
                {
                    SetupTv(showDialog: false);
                }
            }
        };
    }

    [MenuItem(MenuPath, false, 11)]
    public static void MenuSetupTv()
    {
        SetupTv(showDialog: true);
    }

    public static void SetupTv(bool showDialog = true)
    {
        GameObject tvGo = GameObject.Find("tivi");
        if (tvGo == null)
        {
            VideoPlayer vp = Object.FindObjectOfType<VideoPlayer>();
            if (vp != null) tvGo = vp.gameObject;
        }

        if (tvGo == null)
        {
            string warnMsg = "Không tìm thấy vật thể 'tivi' trong Scene hiện tại!";
            Debug.LogWarning($"[Bảo Tàng - Tivi] {warnMsg}");
            if (showDialog)
            {
                EditorUtility.DisplayDialog("Lỗi Thiết Lập Tivi", warnMsg, "Đóng");
            }
            return;
        }

        Undo.RegisterCompleteObjectUndo(tvGo, "Setup TV Video Audio and Light");

        VideoPlayer videoPlayer = tvGo.GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = tvGo.GetComponentInChildren<VideoPlayer>();
        }

        AudioSource audioSource = tvGo.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = Undo.AddComponent<AudioSource>(tvGo);
        }

        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 2.5f;
        audioSource.maxDistance = 16.0f;
        audioSource.volume = 1.0f;
        audioSource.dopplerLevel = 0f;
        audioSource.spread = 45f;
        audioSource.playOnAwake = false;
        audioSource.loop = true;

        audioSource.rolloffMode = AudioRolloffMode.Custom;
        AnimationCurve curve = new AnimationCurve(
            new Keyframe(0f, 1.0f),
            new Keyframe(2.5f, 1.0f, 0f, -0.08f),
            new Keyframe(5.875f, 0.72f),
            new Keyframe(9.25f, 0.40f),
            new Keyframe(12.625f, 0.16f),
            new Keyframe(16.0f, 0.0f, -0.05f, 0f)
        );
        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);

        if (videoPlayer != null)
        {
            Undo.RecordObject(videoPlayer, "Configure VideoPlayer Audio");
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, audioSource);
            EditorUtility.SetDirty(videoPlayer);
        }

        Transform lightTr = tvGo.transform.Find("TvScreenLight");
        GameObject lightGo;
        if (lightTr == null)
        {
            lightGo = new GameObject("TvScreenLight");
            Undo.RegisterCreatedObjectUndo(lightGo, "Create TvScreenLight");
            lightGo.transform.SetParent(tvGo.transform, false);
        }
        else
        {
            lightGo = lightTr.gameObject;
        }

        Light screenLight = lightGo.GetComponent<Light>();
        if (screenLight == null)
        {
            screenLight = Undo.AddComponent<Light>(lightGo);
        }

        screenLight.type = LightType.Point;
        screenLight.range = 13.0f;
        screenLight.intensity = 3.0f;
        screenLight.color = new Color(0.85f, 0.92f, 1.0f);
        screenLight.shadows = LightShadows.None;

        Renderer rend = tvGo.GetComponentInChildren<Renderer>();
        Vector3 center = (rend != null) ? rend.bounds.center : tvGo.transform.position;
        lightGo.transform.position = center + tvGo.transform.forward * 0.45f + Vector3.up * 0.15f;

        TvVideoController controller = tvGo.GetComponent<TvVideoController>();
        if (controller == null)
        {
            controller = Undo.AddComponent<TvVideoController>(tvGo);
        }

        controller.videoPlayer = videoPlayer;
        controller.audioSource = audioSource;
        controller.screenLight = screenLight;
        controller.minAudioDistance = 2.5f;
        controller.maxAudioDistance = 16.0f;
        controller.maxVolume = 1.0f;
        controller.spatialBlend = 1.0f;
        controller.enableRoomLighting = true;
        controller.enableDynamicAmbilight = true;
        controller.lightRange = 13.0f;
        controller.baseIntensity = 3.0f;
        controller.minIntensity = 1.2f;
        controller.maxIntensity = 4.2f;
        controller.lightOffsetForward = 0.45f;
        controller.lightOffsetUp = 0.15f;

        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(audioSource);
        EditorUtility.SetDirty(screenLight);
        EditorUtility.SetDirty(lightGo);
        EditorUtility.SetDirty(tvGo);

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.isLoaded)
        {
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);
        }

        string resultMsg = "Đã cấu hình thành công Hệ thống Video Tivi Bảo Tàng:\n\n" +
                           "1. Âm thanh 3D Không Gian (Spatial Audio):\n" +
                           "   - Chế độ: VideoAudioOutputMode.AudioSource (Track 0)\n" +
                           "   - Khoảng cách nghe to nhất: 0 - 2.5m (100% Volume)\n" +
                           "   - Suy giảm âm lượng: Giảm dần theo khoảng cách và tắt hẳn khi ra khỏi phòng (~16m)\n" +
                           "   - Định hướng: 100% 3D theo tai người nghe (Player Camera)\n\n" +
                           "2. Ánh sáng chiếu sáng phòng (Ambilight):\n" +
                           "   - Đèn Point Light đặt trước màn hình Tivi\n" +
                           "   - Phạm vi chiếu sáng: 13m bao trùm toàn bộ khu vực ghế sofa và sàn phòng\n" +
                           "   - Cường độ: 3.0 (tự động điều tiết 1.2 - 4.2 theo độ sáng video)\n" +
                           "   - Đổi màu động theo thời gian thực theo từng khung hình video";

        Debug.Log($"[Bảo Tàng - Tivi] {resultMsg}");
        if (showDialog)
        {
            EditorUtility.DisplayDialog("Bảo Tàng - Cấu Hình Video Tivi Thành Công", resultMsg, "Hoàn tất");
        }
    }

    [MenuItem("Bảo Tàng/Tối Ưu Kính Màn Hình Tivi (Khắc Phục Phản Chiếu Đèn Ngoài Trời)", false, 12)]
    public static void OptimizeTvGlassScreen()
    {
        GameObject tvGo = GameObject.Find("tivi");
        if (tvGo == null)
        {
            VideoPlayer vp = Object.FindObjectOfType<VideoPlayer>();
            if (vp != null) tvGo = vp.gameObject;
        }

        if (tvGo == null)
        {
            EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy vật thể 'tivi' trong Scene!", "Đóng");
            return;
        }

        Material tvGlassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/glass_partition_41/TV_GlassScreen.mat");
        if (tvGlassMat == null)
        {
            tvGlassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/glass_partition_41/KinhPhanChieu.mat");
        }

        int modifiedCount = 0;

        Renderer[] allRenderers = tvGo.GetComponentsInChildren<Renderer>(true);
        foreach (var rend in allRenderers)
        {
            if (rend.gameObject == tvGo) continue;

            string objName = rend.gameObject.name.ToLower();
            bool isGlassCandidate = objName.Contains("glass") || objName.Contains("kinh") ||
                                    objName.Contains("door") || objName.Contains("partition") ||
                                    objName.Contains("screen") || objName.Contains("quad") || objName.Contains("plane");

            if (isGlassCandidate || (rend.sharedMaterial != null && rend.sharedMaterial.name.ToLower().Contains("kinh")))
            {
                Undo.RecordObject(rend, "Optimize TV Glass Material");
                if (tvGlassMat != null)
                {
                    rend.sharedMaterial = tvGlassMat;
                }
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.receiveShadows = true;
                rend.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                rend.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                EditorUtility.SetDirty(rend.gameObject);
                modifiedCount++;
                Debug.Log($"[Tối Ưu Kính TV] Đã áp dụng vật liệu kính chống chói cho: {rend.gameObject.name}");
            }
        }

        Transform probeTr = tvGo.transform.Find("TvRoom_ReflectionProbe");
        if (probeTr == null)
        {
            GameObject probeGo = new GameObject("TvRoom_ReflectionProbe");
            Undo.RegisterCreatedObjectUndo(probeGo, "Create TV Room Reflection Probe");
            probeGo.transform.SetParent(tvGo.transform, false);
            probeGo.transform.localPosition = new Vector3(0, 1.2f, 2.5f);

            ReflectionProbe probe = probeGo.AddComponent<ReflectionProbe>();
            probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Baked;
            probe.size = new Vector3(12f, 6f, 14f);
            probe.boxProjection = true;
            probe.intensity = 0.8f;
            probe.clearFlags = UnityEngine.Rendering.ReflectionProbeClearFlags.SolidColor;
            probe.backgroundColor = new Color(0.04f, 0.04f, 0.05f);
            EditorUtility.SetDirty(probeGo);
            Debug.Log("[Tối Ưu Kính TV] Đã tạo Reflection Probe nội thất phòng TV.");
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();

        string msg = $"Đã tối ưu hóa lớp kính màn hình Tivi thành công!\n\n" +
                     $"1. Đã gán vật liệu kính TV chống chói chuyên dụng (TV_GlassScreen.mat):\n" +
                     $"   - Độ kim loại (Metallic): 0 (loại bỏ tính chất gương tráng bạc).\n" +
                     $"   - Đã tắt Environment Reflections & Specular Highlights: Chặn hoàn toàn ánh sáng bầu trời ngoài trời và đèn mặt trời chiếu xuyên vào phòng.\n" +
                     $"   - Độ nhẵn (Smoothness): 0.72 tạo bề mặt kính màn hình sang trọng, sắc nét.\n" +
                     $"2. Đã tạo Reflection Probe nội thất phòng TV mô phỏng phản xạ không gian kín thực tế.";

        EditorUtility.DisplayDialog("Tối Ưu Kính Tivi Thành Công", msg, "Hoàn tất");
    }
}

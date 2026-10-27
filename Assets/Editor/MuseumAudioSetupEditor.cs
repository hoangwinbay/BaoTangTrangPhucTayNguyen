using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MuseumAudioSetupEditor
{
    [MenuItem("Tools/Bảo Tàng/Âm Thanh/Cài đặt Âm thanh nền (BGM)", false, 1)]
    public static void SetupMuseumBackgroundMusic()
    {
        AudioClip bgmClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources/Audio/NhacNen_TayNguyen.mp3");
        if (bgmClip == null)
        {
            bgmClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/NhacNen_TayNguyen.mp3");
        }

        if (bgmClip == null)
        {
            EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy file 'NhacNen_TayNguyen.mp3' trong Assets/Audio hoặc Assets/Resources/Audio!", "OK");
            return;
        }

        string clipPath = AssetDatabase.GetAssetPath(bgmClip);
        AudioImporter importer = AssetImporter.GetAtPath(clipPath) as AudioImporter;
        if (importer != null)
        {
            var defaultSettings = importer.defaultSampleSettings;
            bool changed = false;

            if (defaultSettings.loadType != AudioClipLoadType.Streaming)
            {
                defaultSettings.loadType = AudioClipLoadType.Streaming;
                changed = true;
            }
            if (defaultSettings.compressionFormat != AudioCompressionFormat.Vorbis)
            {
                defaultSettings.compressionFormat = AudioCompressionFormat.Vorbis;
                changed = true;
            }
            if (importer.loadInBackground == false)
            {
                importer.loadInBackground = true;
                changed = true;
            }

            if (changed)
            {
                importer.defaultSampleSettings = defaultSettings;
                importer.SaveAndReimport();
                Debug.Log("[MuseumAudioSetup] Đã tối ưu cấu hình Streaming cho file âm thanh nền.");
            }
        }

        MuseumAudioManager audioMgr = Object.FindObjectOfType<MuseumAudioManager>();
        if (audioMgr == null)
        {
            GameObject go = new GameObject("[MuseumAudioManager]");
            audioMgr = go.AddComponent<MuseumAudioManager>();
            Undo.RegisterCreatedObjectUndo(go, "Create MuseumAudioManager");
        }

        Undo.RecordObject(audioMgr, "Configure MuseumAudioManager");
        audioMgr.backgroundMusicClip = bgmClip;
        audioMgr.baseVolume = 0.22f;
        audioMgr.duckedVolume = 0.05f;
        audioMgr.fadeSpeed = 2.0f;
        audioMgr.toggleMuteKey = KeyCode.M;

        AudioSource src = audioMgr.GetComponent<AudioSource>();
        if (src == null) src = audioMgr.gameObject.AddComponent<AudioSource>();

        Undo.RecordObject(src, "Configure AudioSource");
        src.clip = bgmClip;
        src.loop = true;
        src.playOnAwake = true;
        src.spatialBlend = 0f;
        src.volume = 0.22f;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = audioMgr.gameObject;

        EditorUtility.DisplayDialog("Thành Công",
            $"Đã cài đặt âm thanh nền thành công!\n\n" +
            $"- Tên bài hát: {bgmClip.name}\n" +
            $"- Thời lượng: {bgmClip.length:F1}s\n" +
            $"- Chế độ phát: Lặp lại vô tận (Loop), Âm lượng: 22%\n" +
            $"- Tối ưu RAM: Bật Streaming (Gần như 0 MB RAM)\n" +
            $"- Phím M: Tắt / Bật tiếng nhanh\n" +
            $"- Audio Ducking: Tự động hạ nhỏ khi nghe thuyết minh hiện vật.",
            "Tuyệt vời");

        Debug.Log("[MuseumAudioSetup] Hoàn tất thiết lập âm thanh nền cho bảo tàng.");
    }
}

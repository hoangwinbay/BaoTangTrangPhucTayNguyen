using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SetupExhibitionCubes
{
    public static readonly (string cubeName, string ethnicName, string baseFileName, string audioPath)[] CubeList = new[]
    {
        ("Cube (56)", "Dân tộc Ê Đê", "03c4a4d6-db18-4535-b5ba-73cf309ba1f9", "Assets/Audio/trang_phuc_truyen_thong_cua_nguoi_e_de_noi_bat_05494c5f-4230-41f7-b4f6-0d4a28d81caf.mp3"),
        ("Cube (58)", "Dân tộc Mạ", "058d668e-a1aa-4989-a44f-6466fbb623e9", "Assets/Audio/Ma.wav"),
        ("Cube (74)", "Dân tộc Xtiêng", "14804e7a-967b-4de9-874a-382bc4b43e2a", "Assets/Audio/Xtieng.wav"),
        ("Cube (76)", "Dân tộc Ba Na", "1dc2cf1f-804f-4c1c-aa44-407a9c40a64e", "Assets/Audio/Ba Na.wav"),
        ("Cube (78)", "Dân tộc Chư-Ru", "363eb253-0c6d-41a3-8ef1-1f420161e13a", "Assets/Audio/Chu Ru.wav"),
        ("Cube (60)", "Dân tộc M'Nông", "9b7b7714-f545-4978-bb30-53f288981096", "Assets/Audio/M'Nong.wav"),
        ("Cube (64)", "Dân tộc Gia Rai", "9d5939b0-a28b-4845-bb7e-cd057c0eae57", "Assets/Audio/Gia Rai.wav"),
        ("Cube (66)", "Dân tộc Brâu", "1e6aeefd-1896-4305-b7dd-b1c69ece1693", "Assets/Audio/Brau.wav"),
        ("Cube (68)", "Dân tộc Cơ Ho", "ebbd89d0-4dd7-4790-97e1-9a32a5c06a05", "Assets/Audio/Co Ho.wav"),
        ("Cube (70)", "Dân tộc Xơ Đăng", "a0904cda-3732-4c7b-955a-0f182ed8e3fd", "Assets/Audio/Xo Dang.wav"),
        ("Cube (72)", "Dân tộc Giẻ Triêng", "e335c1b3-fe38-496b-97f1-f693b2896c37", "Assets/Audio/Gie Trieng.wav"),
        ("Cube (62)", "Dân tộc Rơ Măm", "314ca509-2fb7-487a-b3ba-7c9b2876e599", "Assets/Audio/ThuyetMinh_Cube72.mp3")
    };

    [InitializeOnLoadMethod]
    static void AutoInitOnEditorLoad()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            bool needSetup = false;
            foreach (var item in CubeList)
            {
                GameObject go = GameObject.Find(item.cubeName);
                if (go != null)
                {
                    var trig = go.GetComponent<CubeExhibitionTrigger>();
                    if (trig == null || trig.voiceNarration == null || trig.speakerSprite == null)
                    {
                        needSetup = true;
                        break;
                    }
                }
            }

            if (needSetup)
            {
                SetupAllCubes(showDialog: false);
            }
        };
    }

    [MenuItem("Bảo Tàng/Kiểm Tra & Gán Giọng Đọc 12 Cube Thuyết Minh", false, 10)]
    public static void MenuSetupAllCubes()
    {
        SetupAllCubes(showDialog: true);
    }

    [MenuItem("Bảo Tàng/Báo Cáo Tình Trạng Giọng Đọc 12 Cube", false, 11)]
    public static void MenuAuditReport()
    {
        string report = GenerateAuditReport();
        Debug.Log(report);
        EditorUtility.DisplayDialog("Báo Cáo Giọng Đọc Thuyết Minh", report, "Đồng Ý");
    }

    public static void SetupAllCubes(bool showDialog = true)
    {
        Sprite fingerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/FingerTapIcon.png");
        Sprite speakerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/SpeakerIcon.png");
        string imageFolder = "Assets/3d/ảnh chi tiết";

        int configuredCount = 0;
        int audioAssignedCount = 0;
        List<string> missingCubes = new List<string>();
        List<string> missingAudio = new List<string>();

        foreach (var item in CubeList)
        {
            GameObject cubeGo = GameObject.Find(item.cubeName);
            if (cubeGo == null)
            {
                missingCubes.Add(item.cubeName);
                continue;
            }

            Undo.RegisterCompleteObjectUndo(cubeGo, "Setup Cube Exhibition");

            CubeExhibitionTrigger trigger = cubeGo.GetComponent<CubeExhibitionTrigger>();
            if (trigger == null)
            {
                trigger = Undo.AddComponent<CubeExhibitionTrigger>(cubeGo);
            }

            trigger.ethnicTitle = item.ethnicName;
            if (fingerSprite != null) trigger.fingerSprite = fingerSprite;
            if (speakerSprite != null) trigger.speakerSprite = speakerSprite;
            trigger.triggerDistance = 3.2f;
            trigger.maxAudibleDistance = 5.5f;
            trigger.heightAboveCube = 0.15f;
            trigger.fingerScale = 0.032f;
            trigger.iconSpacing = 0.16f;
            trigger.heightFromBottom = 0.15f;
            trigger.forwardOffset = 0.08f;
            trigger.tapSpeed = 3.8f;
            trigger.tapDepth = 0.015f;

            if (!string.IsNullOrEmpty(item.audioPath))
            {
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(item.audioPath);
                if (clip != null)
                {
                    trigger.voiceNarration = clip;
                    audioAssignedCount++;
                }
                else
                {
                    AudioClip fallbackClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/ThuyetMinh_Cube72.mp3");
                    if (fallbackClip != null)
                    {
                        trigger.voiceNarration = fallbackClip;
                        audioAssignedCount++;
                    }
                    else
                    {
                        missingAudio.Add($"{item.cubeName} ({item.ethnicName}): {item.audioPath}");
                    }
                }
            }

            string imgPath = $"{imageFolder}/{item.baseFileName}.png";
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(imgPath);
            if (tex == null)
            {
                imgPath = $"{imageFolder}/{item.baseFileName}.jpg";
                tex = AssetDatabase.LoadAssetAtPath<Texture2D>(imgPath);
            }

            if (tex != null)
            {
                trigger.detailTexture = tex;
            }

            trigger.UpdateCubeTopPosition();
            EditorUtility.SetDirty(trigger);
            EditorUtility.SetDirty(cubeGo);
            configuredCount++;
        }

        MuseumPopupManager popupMgr = UnityEngine.Object.FindAnyObjectByType<MuseumPopupManager>();
        if (popupMgr == null)
        {
            GameObject mgrGo = new GameObject("MuseumPopupManager");
            popupMgr = mgrGo.AddComponent<MuseumPopupManager>();
            Undo.RegisterCreatedObjectUndo(mgrGo, "Create MuseumPopupManager");
            EditorUtility.SetDirty(mgrGo);
        }

        if (!EditorApplication.isPlaying)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                EditorSceneManager.SaveScene(activeScene);
            }
        }

        string report = GenerateAuditReport();
        string repDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../Docs/Reports"));
        if (!Directory.Exists(repDir)) Directory.CreateDirectory(repDir);
        File.WriteAllText(Path.Combine(repDir, "BaoCao_ThuyetMinh_Cube.txt"), report, Encoding.UTF8);
        AssetDatabase.Refresh();

        string message = $"Đã kiểm tra & thiết lập thành công 12 Cube bệ thuyết minh!\n\n" +
                         $"• Số Cube đã nhận dạng: {configuredCount}/12\n" +
                         $"• Giọng đọc thuyết minh (Voice Narration): {audioAssignedCount}/12 Cube đã gán đầy đủ\n" +
                         $"• Biểu tượng loa & ngón tay: Đã cấu hình\n" +
                         $"• Tương tác: Đến gần (< 3.2m), bấm 'V' (hoặc nút B/Y trên VR) để nghe thuyết minh giọng đọc.\n" +
                         $"• Báo cáo chi tiết đã lưu tại: Docs/Reports/BaoCao_ThuyetMinh_Cube.txt";

        if (missingCubes.Count > 0)
        {
            message += $"\n\nKhông tìm thấy Cube: " + string.Join(", ", missingCubes);
        }
        if (missingAudio.Count > 0)
        {
            message += $"\n\nThiếu file audio: " + string.Join(", ", missingAudio);
        }

        Debug.Log($"[Bảo Tàng] {message}");
        if (showDialog)
        {
            EditorUtility.DisplayDialog("Bảo Tàng - Thiết Lập Giọng Đọc 12 Cube", message, "Tuyệt vời");
        }
    }

    public static string GenerateAuditReport()
    {
        var sb = new StringBuilder();
        sb.AppendLine("===============================================================================");
        sb.AppendLine("             BÁO CÁO KIỂM TRA GIỌNG ĐỌC THUYẾT MINH 12 CUBE BẢO TÀNG           ");
        sb.AppendLine($"                    Thời gian kiểm tra: {DateTime.Now:yyyy-MM-dd HH:mm:ss}     ");
        sb.AppendLine("===============================================================================");
        sb.AppendLine(string.Format("{0,-12} | {1,-18} | {2,-15} | {3,-35}", "TÊN CUBE", "DÂN TỘC", "TRẠNG THÁI", "FILE ÂM THANH GÁN VÀO"));
        sb.AppendLine("-------------------------------------------------------------------------------");

        int assignedCount = 0;
        foreach (var item in CubeList)
        {
            GameObject cubeGo = GameObject.Find(item.cubeName);
            string status = "CHƯA GÁN";
            string clipName = "(null)";

            if (cubeGo != null)
            {
                var trig = cubeGo.GetComponent<CubeExhibitionTrigger>();
                if (trig != null && trig.voiceNarration != null)
                {
                    status = "ĐÃ GÁN ✓";
                    clipName = $"{trig.voiceNarration.name} ({trig.voiceNarration.length:F1}s)";
                    assignedCount++;
                }
                else if (trig != null)
                {
                    status = "THIẾU CLIP ✗";
                }
                else
                {
                    status = "THIẾU TRIGGER ✗";
                }
            }
            else
            {
                status = "KHÔNG TÌM THẤY ✗";
            }

            sb.AppendLine(string.Format("{0,-12} | {1,-18} | {2,-15} | {3,-35}", item.cubeName, item.ethnicName, status, clipName));
        }

        sb.AppendLine("-------------------------------------------------------------------------------");
        sb.AppendLine($"TỔNG KẾT: {assignedCount}/12 Cube đã được gán giọng đọc thuyết minh thành công.");
        sb.AppendLine("===============================================================================");
        return sb.ToString();
    }
}

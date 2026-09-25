using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu Editor tự động thiết lập 12 bệ Cube thuyết minh trong bảo tàng.
/// Gắn script CubeExhibitionTrigger, cài đặt biểu tượng ngón tay nhấn, nạp sẵn ảnh thuyết minh
/// và cấu hình MuseumPopupManager để bấm Shift là hiện ảnh pop-up.
/// </summary>
public static class SetupExhibitionCubes
{
    // Cấu hình 12 Cube bệ trưng bày tương ứng với 12 dân tộc và 12 ảnh thuyết minh
    private static readonly (string cubeName, string ethnicName, string baseFileName)[] CubeList = new[]
    {
        ("Cube (56)", "Dân tộc Ê Đê", "03c4a4d6-db18-4535-b5ba-73cf309ba1f9"),
        ("Cube (58)", "Dân tộc Mạ", "058d668e-a1aa-4989-a44f-6466fbb623e9"),
        ("Cube (74)", "Dân tộc Xtiêng", "14804e7a-967b-4de9-874a-382bc4b43e2a"),
        ("Cube (76)", "Dân tộc Ba Na", "1dc2cf1f-804f-4c1c-aa44-407a9c40a64e"),
        ("Cube (78)", "Dân tộc Chư-Ru", "363eb253-0c6d-41a3-8ef1-1f420161e13a"),
        ("Cube (60)", "Dân tộc M'Nông", "9b7b7714-f545-4978-bb30-53f288981096"),
        ("Cube (64)", "Dân tộc Gia Rai", "9d5939b0-a28b-4845-bb7e-cd057c0eae57"),
        ("Cube (66)", "Dân tộc Brâu", "1e6aeefd-1896-4305-b7dd-b1c69ece1693"),
        ("Cube (68)", "Dân tộc Cơ Ho", "ebbd89d0-4dd7-4790-97e1-9a32a5c06a05"),
        ("Cube (70)", "Dân tộc Xơ Đăng", "a0904cda-3732-4c7b-955a-0f182ed8e3fd"),
        ("Cube (72)", "Dân tộc Giẻ Triêng", "e335c1b3-fe38-496b-97f1-f693b2896c37"),
        ("Cube (62)", "Dân tộc Rơ Măm", "314ca509-2fb7-487a-b3ba-7c9b2876e599")
    };

    [InitializeOnLoadMethod]
    static void AutoInitOnEditorLoad()
    {
        EditorApplication.delayCall += () =>
        {
            GameObject c56 = GameObject.Find("Cube (56)");
            if (c56 != null)
            {
                var trig = c56.GetComponent<CubeExhibitionTrigger>();
                if (trig == null || trig.fingerScale > 0.08f || trig.speakerSprite == null)
                {
                    SetupAllCubes(showDialog: false);
                }
            }
        };
    }

    [MenuItem("Bảo Tàng/Thiết lập 12 Bệ Cube Thuyết Minh", false, 10)]
    public static void MenuSetupAllCubes()
    {
        SetupAllCubes(showDialog: true);
    }

    public static void SetupAllCubes(bool showDialog = true)
    {
        Sprite fingerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/FingerTapIcon.png");
        Sprite speakerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/SpeakerIcon.png");
        AudioClip cube72Audio = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/ThuyetMinh_Cube72.mp3");
        string imageFolder = "Assets/3d/ảnh chi tiết";

        int configuredCount = 0;
        List<string> missingCubes = new List<string>();

        // 1. Cấu hình từng Cube
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
            trigger.heightAboveCube = 0.15f;
            trigger.fingerScale = 0.06f; // Nhỏ bằng một nửa (gấp đôi độ nhỏ)
            trigger.tapSpeed = 3.8f;
            trigger.tapDepth = 0.025f;

            // Gán file ghi âm thuyết minh cho Cube (72)
            if (item.cubeName == "Cube (72)" && cube72Audio != null)
            {
                trigger.voiceNarration = cube72Audio;
            }

            // Nạp Texture ảnh thuyết minh mặc định (ưu tiên .png)
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
            EditorUtility.SetDirty(cubeGo);
            configuredCount++;
        }

        // 2. Đảm bảo MuseumPopupManager có mặt trong scene
        MuseumPopupManager popupMgr = Object.FindObjectOfType<MuseumPopupManager>();
        if (popupMgr == null)
        {
            GameObject mgrGo = new GameObject("MuseumPopupManager");
            popupMgr = mgrGo.AddComponent<MuseumPopupManager>();
            Undo.RegisterCreatedObjectUndo(mgrGo, "Create MuseumPopupManager");
            EditorUtility.SetDirty(mgrGo);
        }

        // 3. Đánh dấu và lưu Scene an toàn
        if (!EditorApplication.isPlaying)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                EditorSceneManager.SaveScene(activeScene);
            }
        }

        string message = $"Đã thiết lập thành công {configuredCount}/12 Cube bệ thuyết minh!\n\n" +
                         $"- Biểu tượng ngón tay nhấn: Đã gắn\n" +
                         $"- Tương tác: Đến gần (< 2.8m) ngón tay hiện ra, nhấn Shift để mở bảng pop-up, nhấn Shift lần nữa để thu lại\n" +
                         $"- Quản lý Popup: Đã cấu hình MuseumPopupManager trong Scene\n" +
                         $"- Tùy chỉnh ảnh: Bạn có thể chọn từng Cube trong Hierarchy và kéo đổi ảnh trong mục 'Detail Texture' của Inspector.";

        if (missingCubes.Count > 0)
        {
            message += $"\n\nKhông tìm thấy {missingCubes.Count} Cube: " + string.Join(", ", missingCubes);
        }

        Debug.Log($"[Bảo Tàng] {message}");
        if (showDialog)
        {
            EditorUtility.DisplayDialog("Bảo Tàng - Thiết Lập Cube Thuyết Minh", message, "Tuyệt vời");
        }
    }
}

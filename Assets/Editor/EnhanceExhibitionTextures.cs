using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Menu Editor tự động tối ưu và làm nét toàn bộ ảnh trưng bày trong bảo tàng:
/// 1. Khắc phục triệt để lỗi "khi đi ra xa thì hình ảnh trên cube bị mờ":
///    - Tắt Mipmap (enableMipMap = false) hoặc gán Mipmap Bias = -1.5f để ảnh không bị downsample khi lùi xa.
///    - Bật Lọc bất đẳng hướng tối đa 16x (Anisotropic Filtering Level = 16) chống mờ ở góc nhìn nghiêng.
///    - Chuyển chế độ lọc sang Trilinear (mịn và sắc nét tối đa).
///    - Mở rộng kích thước tối đa lên 4096 (giữ nguyên độ phân giải gốc 2816x2036 của ảnh infographics).
///    - Tắt nén giảm chất lượng (Texture Compression = Uncompressed / HighQuality) để chữ và chi tiết sắc nét 100%.
/// 2. Ép bật Anisotropic Filtering toàn cục trong QualitySettings (ForceEnable).
/// 3. Hỗ trợ 2 chế độ:
///    - Chế độ 1 (Khuyên dùng): Siêu nét chống mờ từ xa (Tắt Mipmaps) -> Đứng xa bao nhiêu cũng nét căng.
///    - Chế độ 2: Nét mượt mà (Bật Mipmaps + Bias -1.5f + Aniso 16).
/// </summary>
public class EnhanceExhibitionTexturesWindow : EditorWindow
{
    private static readonly string[] TargetFolders = new[]
    {
        "Assets/3d/ảnh tường",
        "Assets/3d/ảnh gth",
        "Assets/3d/3d",
        "Assets/3d/nội thất",
        "Assets/3d/ảnh chi tiết"
    };

    private static readonly string[] TargetCubes = new[]
    {
        "Cube (32)", "Cube (45)", "Cube (46)", "Cube (47)", "Cube (48)",
        "Cube (79)", "Cube (80)", "Cube (50)", "Cube (51)", "Cube (52)",
        "Cube (53)", "Cube (54)"
    };

    [MenuItem("Bảo Tàng/Làm Nét Ảnh Trưng Bày (Chống Mờ Khi Đi Ra Xa)", false, 11)]
    public static void ShowWindow()
    {
        var window = GetWindow<EnhanceExhibitionTexturesWindow>("Làm Nét Ảnh Bảo Tàng");
        window.minSize = new Vector2(520, 420);
        window.Show();
    }

    [InitializeOnLoadMethod]
    private static void AutoInit()
    {
        EditorApplication.delayCall += () =>
        {
            // Kiểm tra nếu QualitySettings chưa bật ForceEnable
            if (QualitySettings.anisotropicFiltering != AnisotropicFiltering.ForceEnable)
            {
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                Debug.Log("[EnhanceExhibitionTextures] Đã tự động kích hoạt AnisotropicFiltering.ForceEnable trong QualitySettings.");
            }
        };
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        GUILayout.Label("CÔNG CỤ LÀM NÉT ẢNH TRƯNG BÀY BẢO TÀNG", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Khắc phục triệt để hiện tượng ảnh trên các Cube bị mờ khi người chơi đi ra xa.\n" +
            "Áp dụng cho 12 ảnh tường dân tộc (2816x2036), ảnh giới thiệu, tranh 3D và bệ trưng bày.",
            MessageType.Info);

        EditorGUILayout.Space(10);
        GUILayout.Label("1. Trạng Thái Hiện Tại:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Anisotropic Filtering toàn cục:", QualitySettings.anisotropicFiltering.ToString());

        EditorGUILayout.Space(15);
        GUILayout.Label("2. Chọn Chế Độ Tối Ưu Làm Nét:", EditorStyles.boldLabel);

        // Chế độ 1: Khuyên dùng
        GUI.backgroundColor = new Color(0.3f, 0.85f, 0.4f);
        if (GUILayout.Button("★ CHẾ ĐỘ 1: SIÊU NÉT CHỐNG MỜ TỪ XA (TẮT MIPMAPS - KHUYÊN DÙNG)\nĐảm bảo đi ra xa bao nhiêu cũng giữ nguyên độ phân giải 100%, không bao giờ mờ", GUILayout.Height(55)))
        {
            ApplyEnhancement(disableMipmaps: true);
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(8);

        // Chế độ 2
        GUI.backgroundColor = new Color(0.4f, 0.7f, 1f);
        if (GUILayout.Button("CHẾ ĐỘ 2: LÀM NÉT CÂN BẰNG (BẬT MIPMAPS + BIAS -1.5f + ANISO 16x)\nGiữ Mipmaps nhưng đẩy xa gấp 3 lần khoảng cách làm mờ + 16x Anisotropic", GUILayout.Height(45)))
        {
            ApplyEnhancement(disableMipmaps: false);
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(15);
        GUILayout.Label("3. Kiểm Tra 12 Cube Trưng Bày Trong Scene:", EditorStyles.boldLabel);
        if (GUILayout.Button("Kiểm Tra & Đồng Bộ Vật Liệu 12 Cube", GUILayout.Height(32)))
        {
            CheckAndSyncCubes();
        }
    }

    public static void ApplyEnhancement(bool disableMipmaps)
    {
        // 1. Ép bật Anisotropic Filtering trong QualitySettings
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;

        // 2. Tìm tất cả ảnh trong các thư mục mục tiêu
        List<string> texturePaths = new List<string>();
        foreach (string folder in TargetFolders)
        {
            if (!Directory.Exists(folder)) continue;

            string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                {
                    string unityPath = file.Replace("\\", "/");
                    texturePaths.Add(unityPath);
                }
            }
        }

        int count = 0;
        try
        {
            AssetDatabase.StartAssetEditing();

            for (int i = 0; i < texturePaths.Count; i++)
            {
                string path = texturePaths[i];
                EditorUtility.DisplayProgressBar("Đang tối ưu ảnh bảo tàng...", Path.GetFileName(path), (float)i / texturePaths.Count);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool changed = false;

                if (disableMipmaps)
                {
                    if (importer.mipmapEnabled)
                    {
                        importer.mipmapEnabled = false;
                        changed = true;
                    }
                }
                else
                {
                    if (!importer.mipmapEnabled)
                    {
                        importer.mipmapEnabled = true;
                        changed = true;
                    }
                    if (Mathf.Abs(importer.mipMapBias - (-1.5f)) > 0.01f)
                    {
                        importer.mipMapBias = -1.5f;
                        changed = true;
                    }
                }

                if (importer.anisoLevel != 16)
                {
                    importer.anisoLevel = 16;
                    changed = true;
                }

                if (importer.filterMode != FilterMode.Trilinear)
                {
                    importer.filterMode = FilterMode.Trilinear;
                    changed = true;
                }

                if (importer.maxTextureSize < 4096)
                {
                    importer.maxTextureSize = 4096;
                    changed = true;
                }

                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    changed = true;
                }

                // Cấu hình Default platform
                TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
                if (defaultSettings.maxTextureSize < 4096 || defaultSettings.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    defaultSettings.maxTextureSize = 4096;
                    defaultSettings.textureCompression = TextureImporterCompression.Uncompressed;
                    defaultSettings.compressionQuality = 100;
                    importer.SetPlatformTextureSettings(defaultSettings);
                    changed = true;
                }

                // Cấu hình Standalone platform
                TextureImporterPlatformSettings pcSettings = importer.GetPlatformTextureSettings("Standalone");
                if (pcSettings != null && pcSettings.overridden)
                {
                    pcSettings.maxTextureSize = 4096;
                    pcSettings.textureCompression = TextureImporterCompression.Uncompressed;
                    pcSettings.compressionQuality = 100;
                    importer.SetPlatformTextureSettings(pcSettings);
                    changed = true;
                }

                if (changed)
                {
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                    count++;
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string modeName = disableMipmaps ? "Siêu Nét Chống Mờ Từ Xa (Tắt Mipmaps)" : "Nét Cân Bằng (Mipmaps + Bias -1.5f + Aniso 16)";
        string msg = $"Đã áp dụng thành công chế độ '{modeName}' cho {count}/{texturePaths.Count} ảnh trưng bày trong bảo tàng!\n\n" +
                     $"• Anisotropic Filtering: ForceEnable (16x)\n" +
                     $"• Filter Mode: Trilinear\n" +
                     $"• Max Texture Size: 4096 (giữ nguyên độ phân giải gốc)\n" +
                     $"• Compression: Uncompressed (không méo hình / nhòe chữ)";

        Debug.Log($"[EnhanceExhibitionTextures] {msg}");
        EditorUtility.DisplayDialog("Làm Nét Ảnh Bảo Tàng Thành Công", msg, "Đồng Ý");
    }

    public static void CheckAndSyncCubes()
    {
        int found = 0;
        List<string> details = new List<string>();

        foreach (string cubeName in TargetCubes)
        {
            GameObject go = GameObject.Find(cubeName);
            if (go == null)
            {
                details.Add($"❌ {cubeName}: Không tìm thấy trong Scene hiện tại.");
                continue;
            }

            found++;
            Renderer rend = go.GetComponent<Renderer>();
            if (rend == null || rend.sharedMaterial == null)
            {
                details.Add($"⚠️ {cubeName}: Không có Renderer hoặc Material.");
                continue;
            }

            Material mat = rend.sharedMaterial;
            Texture mainTex = mat.HasProperty("_BaseMap") ? mat.GetTexture("_BaseMap") : (mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null);

            string texName = mainTex != null ? mainTex.name : "Không có Texture";
            string res = mainTex != null ? $"{mainTex.width}x{mainTex.height}" : "N/A";
            string filter = mainTex != null ? mainTex.filterMode.ToString() : "N/A";
            int aniso = mainTex != null ? mainTex.anisoLevel : 0;

            details.Add($"✓ {cubeName}: Mat='{mat.name}' | Tex='{texName}' ({res}) | Filter={filter}, Aniso={aniso}");

            // Đảm bảo material không bị bóng lóa che khuất chữ
            if (mat.HasProperty("_Smoothness") && mat.GetFloat("_Smoothness") > 0.5f)
            {
                mat.SetFloat("_Smoothness", 0.2f);
                EditorUtility.SetDirty(mat);
            }
        }

        string result = $"Tìm thấy {found}/{TargetCubes.Length} Cube trưng bày:\n\n" + string.Join("\n", details);
        Debug.Log($"[EnhanceExhibitionTextures]\n{result}");
        EditorUtility.DisplayDialog("Kết Quả Kiểm Tra 12 Cube", result, "Đóng");
    }
}

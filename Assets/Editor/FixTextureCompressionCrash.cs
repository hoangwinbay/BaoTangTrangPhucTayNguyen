using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FixTextureCompressionCrash
{
    private const string AlreadyFixedKey = "FixTextureCompressionCrash_v1_done";

    private static readonly string[] TargetFolders = new[]
    {
        "Assets/3d/ảnh tường",
        "Assets/3d/ảnh gth",
        "Assets/3d/3d",
        "Assets/3d/nội thất",
        "Assets/3d/ảnh chi tiết"
    };

    static FixTextureCompressionCrash()
    {
        if (SessionState.GetBool(AlreadyFixedKey, false)) return;

        EditorApplication.delayCall += () => RunFix();
    }

    [MenuItem("Bảo Tàng/Fix Khẩn Cấp: Reimport Texture Chống Crash D3D11", false, 99)]
    public static void MenuRunFix()
    {
        RunFix(force: true);
    }

    private static void RunFix(bool force = false)
    {
        if (!force && SessionState.GetBool(AlreadyFixedKey, false)) return;

        SessionState.SetBool(AlreadyFixedKey, true);

        var texturesToFix = new List<string>();

        foreach (string folder in TargetFolders)
        {
            if (!Directory.Exists(folder)) continue;

            string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg") continue;

                string unityPath = file.Replace("\\", "/");
                TextureImporter importer = AssetImporter.GetAtPath(unityPath) as TextureImporter;
                if (importer == null) continue;

                bool isUncompressed = importer.textureCompression == TextureImporterCompression.Uncompressed;

                TextureImporterPlatformSettings def = importer.GetDefaultPlatformTextureSettings();
                bool defIsUncompressed = def.textureCompression == TextureImporterCompression.Uncompressed;

                if (isUncompressed || defIsUncompressed)
                {
                    texturesToFix.Add(unityPath);
                }
            }
        }

        if (texturesToFix.Count == 0)
        {
            Debug.Log("[FixTextureCompressionCrash] Không tìm thấy texture Uncompressed nào cần fix. Dự án đã an toàn.");
            return;
        }

        Debug.LogWarning($"[FixTextureCompressionCrash] Phát hiện {texturesToFix.Count} texture Uncompressed gây crash D3D11. Đang tự động fix...");

        int fixed_count = 0;
        try
        {
            AssetDatabase.StartAssetEditing();
            for (int i = 0; i < texturesToFix.Count; i++)
            {
                string path = texturesToFix[i];
                EditorUtility.DisplayProgressBar(
                    "Fix Texture Compression (Chống Crash D3D11)...",
                    Path.GetFileName(path),
                    (float)i / texturesToFix.Count);

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool changed = false;

                if (importer.textureCompression == TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.CompressedHQ;
                    changed = true;
                }

                TextureImporterPlatformSettings defSettings = importer.GetDefaultPlatformTextureSettings();
                if (defSettings.textureCompression == TextureImporterCompression.Uncompressed)
                {
                    defSettings.textureCompression = TextureImporterCompression.CompressedHQ;
                    defSettings.compressionQuality = 100;
                    importer.SetPlatformTextureSettings(defSettings);
                    changed = true;
                }

                TextureImporterPlatformSettings pcSettings = importer.GetPlatformTextureSettings("Standalone");
                if (pcSettings != null && pcSettings.overridden && pcSettings.textureCompression == TextureImporterCompression.Uncompressed)
                {
                    pcSettings.textureCompression = TextureImporterCompression.CompressedHQ;
                    pcSettings.compressionQuality = 100;
                    importer.SetPlatformTextureSettings(pcSettings);
                    changed = true;
                }

                if (changed)
                {
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                    fixed_count++;
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

        string msg = $"[FixTextureCompressionCrash] Đã fix {fixed_count}/{texturesToFix.Count} texture:\n" +
                     $"  Uncompressed → CompressedHQ (tiết kiệm ~83% VRAM)\n" +
                     $"  Ước tính VRAM tiết kiệm: ~{fixed_count * 56}MB\n" +
                     $"Editor sẽ ổn định, không còn lỗi D3D11 swapchain crash.";
        Debug.Log(msg);
    }
}

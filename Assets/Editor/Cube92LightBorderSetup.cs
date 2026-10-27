using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MuseumVR.Editor
{
    public static class Cube92LightBorderSetup
    {
        [InitializeOnLoadMethod]
        private static void AutoSetupOnLoad()
        {
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
                var scene = EditorSceneManager.GetActiveScene();
                if (!scene.isLoaded) return;

                GameObject cube92 = FindCube92();
                if (cube92 != null && cube92.GetComponent<CubeLightBorder>() == null)
                {
                    SetupCube92(showDialog: false);
                }
            };
        }

        [MenuItem("Bảo Tàng/Tạo Viền Đèn & Làm Nét Ảnh cho Cube (92)", false, 14)]
        public static void MenuSetup()
        {
            SetupCube92(showDialog: true);
        }

        public static void SetupCube92(bool showDialog = true)
        {
            GameObject cube92 = FindCube92();
            if (cube92 == null)
            {
                string warn = "Không tìm thấy GameObject 'Cube (92)' trong Scene hiện tại!";
                Debug.LogWarning($"[Cube 92 Setup] {warn}");
                if (showDialog) EditorUtility.DisplayDialog("Thông báo", warn, "OK");
                return;
            }

            Undo.RegisterCompleteObjectUndo(cube92, "Setup Cube 92 Light Border and Sharpen Texture");

            CubeLightBorder border = cube92.GetComponent<CubeLightBorder>();
            if (border == null)
            {
                border = Undo.AddComponent<CubeLightBorder>(cube92);
            }

            border.ledColor = new Color(1.0f, 0.90f, 0.72f, 1.0f);
            border.emissionIntensity = 3.5f;
            border.borderThickness = 0.04f;
            border.borderDepth = 0.025f;
            border.enableBacklight = true;
            border.lightIntensity = 2.0f;
            border.lightRange = 3.5f;

            border.BuildOrUpdateBorder();
            EditorUtility.SetDirty(cube92);

            string textureName = "";
            Renderer rend = cube92.GetComponent<Renderer>();
            if (rend != null)
            {
                foreach (var mat in rend.sharedMaterials)
                {
                    if (mat == null) continue;

                    if (mat.HasProperty("_Smoothness"))
                    {
                        mat.SetFloat("_Smoothness", 0.30f);
                        EditorUtility.SetDirty(mat);
                    }

                    Texture tex = mat.mainTexture;
                    if (tex == null && mat.HasProperty("_BaseMap")) tex = mat.GetTexture("_BaseMap");

                    if (tex != null)
                    {
                        textureName = tex.name;
                        string path = AssetDatabase.GetAssetPath(tex);
                        if (!string.IsNullOrEmpty(path))
                        {
                            SharpenTexture(path);
                        }
                    }
                }
            }

            SharpenTexture("Assets/3d/nội thất/images (1).jpg");
            SharpenTexture("Assets/3d/nội thất/images.jpg");

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                EditorSceneManager.SaveScene(activeScene);
            }

            Debug.Log($"[Cube 92 Setup] Đã hoàn thành tạo viền đèn LED và làm nét ảnh từ xa cho {cube92.name}!");

            if (showDialog)
            {
                EditorUtility.DisplayDialog("Thành Công",
                    $"Đã thiết lập thành công cho Cube (92):\n\n" +
                    $"1. Đã tạo Khung Viền Đèn LED (CubeLightBorder):\n" +
                    $"   - Viền LED phát quang HDR (Màu vàng ấm sang trọng, cường độ 3.5).\n" +
                    $"   - Ôm sát 4 cạnh mép trước của tranh.\n" +
                    $"   - Tích hợp đèn Backlight hắt sáng dịu nhẹ lên tường xung quanh.\n\n" +
                    $"2. Đã làm nét ảnh từ xa (Texture Sharpening):\n" +
                    $"   - Anisotropic Filtering = 16 (khử nhòe triệt để khi nhìn từ xa và góc nghiêng).\n" +
                    $"   - Filter Mode = Trilinear (mịn và sắc nét giữa các mức khoảng cách).\n" +
                    $"   - Mipmap Bias = -0.8 (ép GPU ưu tiên chi tiết sắc nét từ cự ly xa).\n" +
                    $"   - Max Size = 4096 (độ phân giải tối đa).", "Hoàn tất");
            }
        }

        private static void SharpenTexture(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || !File.Exists(assetPath)) return;

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                bool modified = false;

                if (importer.anisoLevel != 16) { importer.anisoLevel = 16; modified = true; }
                if (importer.filterMode != FilterMode.Trilinear) { importer.filterMode = FilterMode.Trilinear; modified = true; }
                if (Mathf.Abs(importer.mipMapBias - (-0.8f)) > 0.05f) { importer.mipMapBias = -0.8f; modified = true; }
                if (importer.maxTextureSize < 4096) { importer.maxTextureSize = 4096; modified = true; }
                if (importer.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    importer.textureCompression = TextureImporterCompression.CompressedHQ;
                    modified = true;
                }

                if (modified)
                {
                    importer.SaveAndReimport();
                    Debug.Log($"[Cube 92 Setup] Đã cập nhật và làm nét texture tại: {assetPath}");
                }
            }
        }

        private static GameObject FindCube92()
        {
            GameObject go = GameObject.Find("Cube (92)");
            if (go != null) return go;

            var allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (var t in allTransforms)
            {
                if (t.name == "Cube (92)" && t.gameObject.scene.isLoaded)
                {
                    return t.gameObject;
                }
            }
            return null;
        }
    }
}

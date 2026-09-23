using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public static class ApplyDaVanMaterial
{
    private const string AlbedoPath = "Assets/material/DaVan_Albedo.png";
    private const string NormalPath = "Assets/material/DaVan_Normal.png";
    private const string MaterialPath = "Assets/material/DaVan_Stone.mat";

    [InitializeOnLoadMethod]
    public static void AutoRun()
    {
        EditorApplication.delayCall += () =>
        {
            Apply(showDialog: false);
        };
    }

    [MenuItem("Bảo Tàng/Áp dụng màu đá vân cho Cube 33, 34, 35, 37, 38", false, 15)]
    public static void MenuApply()
    {
        Apply(showDialog: true);
    }

    public static void Apply(bool showDialog = false)
    {
        // 1. Configure texture import settings
        ConfigureTextures();

        // 2. Create or load the URP Lit material
        Material mat = CreateOrUpdateMaterial();
        if (mat == null)
        {
            Debug.LogError("[DaVan] Failed to create or load DaVan_Stone.mat");
            return;
        }

        // 3. Target cubes
        string[] targetCubes = new string[]
        {
            "Cube (33)",
            "Cube (34)",
            "Cube (35)",
            "Cube (37)",
            "Cube (38)"
        };

        int count = 0;
        foreach (string name in targetCubes)
        {
            GameObject go = GameObject.Find(name);
            if (go == null)
            {
                // Try finding among inactive objects as well
                GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var obj in all)
                {
                    if (obj.name == name && obj.scene.isLoaded)
                    {
                        go = obj;
                        break;
                    }
                }
            }

            if (go != null)
            {
                MeshRenderer mr = go.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Undo.RecordObject(mr, "Apply Da Van Material");
                    mr.sharedMaterial = mat;
                    EditorUtility.SetDirty(mr);
                    EditorUtility.SetDirty(go);
                    count++;
                    Debug.Log($"[DaVan] Applied DaVan_Stone to {name}");
                }
                else
                {
                    Debug.LogWarning($"[DaVan] {name} has no MeshRenderer!");
                }
            }
            else
            {
                Debug.LogWarning($"[DaVan] Could not find GameObject {name}!");
            }
        }

        // 4. Save active scene
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.isLoaded)
        {
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);
            Debug.Log($"[DaVan] Scene {activeScene.name} saved successfully.");
        }

        // 5. Render a preview screenshot to check result
        CapturePreviewScreenshot();

        string message = $"Đã áp dụng thành công vật liệu Đá Vân (DaVan_Stone) cho {count}/5 bệ Cube (33, 34, 35, 37, 38)!\n\n" +
                         $"- Texture Albedo: {AlbedoPath}\n" +
                         $"- Normal Map: {NormalPath}\n" +
                         $"- Material: {MaterialPath} (URP Lit)\n" +
                         $"- Độ bóng (Smoothness): 0.55 (phản xạ đá mài tự nhiên)\n" +
                         $"- Tiling: (2, 1) lặp vân đá mượt mà, không bị kéo dãn\n" +
                         $"- Scene đã được lưu tự động.";

        Debug.Log($"[DaVan] {message}");
        if (showDialog)
        {
            EditorUtility.DisplayDialog("Bảo Tàng - Thiết Lập Đá Vân", message, "Tuyệt vời");
        }
    }

    private static void ConfigureTextures()
    {
        AssetDatabase.ImportAsset(AlbedoPath, ImportAssetOptions.ForceUpdate);
        TextureImporter albedoImporter = AssetImporter.GetAtPath(AlbedoPath) as TextureImporter;
        if (albedoImporter != null)
        {
            albedoImporter.textureType = TextureImporterType.Default;
            albedoImporter.wrapMode = TextureWrapMode.Repeat;
            albedoImporter.filterMode = FilterMode.Bilinear;
            albedoImporter.anisoLevel = 8;
            albedoImporter.SaveAndReimport();
        }

        AssetDatabase.ImportAsset(NormalPath, ImportAssetOptions.ForceUpdate);
        TextureImporter normalImporter = AssetImporter.GetAtPath(NormalPath) as TextureImporter;
        if (normalImporter != null)
        {
            normalImporter.textureType = TextureImporterType.NormalMap;
            normalImporter.wrapMode = TextureWrapMode.Repeat;
            normalImporter.filterMode = FilterMode.Bilinear;
            normalImporter.anisoLevel = 8;
            normalImporter.SaveAndReimport();
        }

        AssetDatabase.Refresh();
    }

    private static Material CreateOrUpdateMaterial()
    {
        Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
        if (litShader == null)
        {
            litShader = Shader.Find("Standard");
        }

        Material mat = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (mat == null)
        {
            mat = new Material(litShader);
            AssetDatabase.CreateAsset(mat, MaterialPath);
        }
        else
        {
            mat.shader = litShader;
        }

        Texture2D albedoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(AlbedoPath);
        Texture2D normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(NormalPath);

        if (albedoTex != null)
        {
            mat.SetTexture("_BaseMap", albedoTex);
            mat.SetTexture("_MainTex", albedoTex);
        }

        if (normalTex != null)
        {
            mat.EnableKeyword("_NORMALMAP");
            mat.SetTexture("_BumpMap", normalTex);
            mat.SetFloat("_BumpScale", 0.85f);
        }

        // Realistic polished stone settings
        mat.SetFloat("_Smoothness", 0.55f);
        mat.SetFloat("_Metallic", 0.0f);
        mat.SetColor("_BaseColor", Color.white);

        // Subtly tile texture so it doesn't stretch
        mat.SetTextureScale("_BaseMap", new Vector2(2f, 1f));
        mat.SetTextureScale("_BumpMap", new Vector2(2f, 1f));

        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();

        return mat;
    }

    private static void CapturePreviewScreenshot()
    {
        try
        {
            GameObject target = GameObject.Find("Cube (33)");
            if (target == null) return;

            GameObject camGo = new GameObject("TempCaptureCam");
            Camera cam = camGo.AddComponent<Camera>();
            cam.fieldOfView = 60;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 100f;

            // Position camera looking at Cube (33) and nearby cubes
            cam.transform.position = target.transform.position + new Vector3(-4f, 1.5f, -3f);
            cam.transform.LookAt(target.transform.position);

            int width = 1280;
            int height = 720;
            RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShot.Apply();

            cam.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(camGo);

            byte[] bytes = screenShot.EncodeToPNG();
            Object.DestroyImmediate(screenShot);

            string outPath = "D:/Dự án bảo tàng/temp_cubes_preview.png";
            File.WriteAllBytes(outPath, bytes);
            Debug.Log($"[DaVan] Preview saved to {outPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[DaVan] Could not capture preview screenshot: {e.Message}");
        }
    }
}

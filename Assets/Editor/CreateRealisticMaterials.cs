using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using System.IO;

public class CreateRealisticMaterials : EditorWindow
{
    [MenuItem("Assets/Museum/Create Realistic Wall & Ceiling Materials")]
    public static void CreateMaterials()
    {
        string materialsPath = "Assets/LowPolyInterior2/Materials";
        string texturesPath = "Assets/LowPolyInterior2/Textures";
        string floorTexturesPath = "Assets/Floor";

        if (!Directory.Exists(materialsPath))
            Directory.CreateDirectory(materialsPath);

        Texture2D floorAlbedo = AssetDatabase.LoadAssetAtPath<Texture2D>($"{floorTexturesPath}/images.jpg");
        Texture2D floorNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{floorTexturesPath}/images_Normal.png");
        Texture2D interiorTex01 = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesPath}/InteriorTexture_01.png");
        Texture2D interiorTex02 = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesPath}/InteriorTexture_02.png");

        Texture2D wallPlasterNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesPath}/Wall_Plaster_Normal.png");
        Texture2D ceilingPlasterNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesPath}/Ceiling_Plaster_Normal.png");
        Texture2D wallConcreteNormal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{texturesPath}/Wall_Concrete_Normal.png");

        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null)
        {
            Debug.LogError("URP Lit shader not found! Make sure URP is installed.");
            return;
        }

        Material wallMat = new Material(urpLit)
        {
            name = "MuseumWall_Realistic"
        };

        wallMat.SetColor("_BaseColor", new Color(0.92f, 0.88f, 0.82f, 1f));

        if (interiorTex02 != null)
        {
            wallMat.SetTexture("_BaseMap", interiorTex02);
            wallMat.SetTextureScale("_BaseMap", new Vector2(4f, 4f));
            wallMat.SetTextureOffset("_BaseMap", Vector2.zero);
        }

        Texture2D wallNormalToUse = wallPlasterNormal ?? floorNormal;
        if (wallNormalToUse != null)
        {
            wallMat.SetTexture("_BumpMap", wallNormalToUse);
            wallMat.SetTextureScale("_BumpMap", new Vector2(2f, 2f));
            wallMat.SetFloat("_BumpScale", 0.8f);
        }

        wallMat.SetFloat("_Smoothness", 0.05f);
        wallMat.SetFloat("_Metallic", 0f);

        wallMat.SetFloat("_SmoothnessTextureChannel", 1);

        wallMat.EnableKeyword("_SPECULARHIGHLIGHTS_ON");
        wallMat.SetFloat("_SpecularHighlights", 1f);

        wallMat.EnableKeyword("_ENVIRONMENTREFLECTIONS_ON");
        wallMat.SetFloat("_EnvironmentReflections", 0.15f);
        wallMat.SetFloat("_GlossyReflections", 0.05f);

        wallMat.EnableKeyword("_OCCLUSIONMAP_ON");
        wallMat.SetFloat("_OcclusionStrength", 1f);

        wallMat.SetFloat("_Surface", 0f);
        wallMat.SetFloat("_Cull", 0f);
        wallMat.SetFloat("_ReceiveShadows", 1f);
        wallMat.SetFloat("_DoubleSidedGI", 1f);

        wallMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
        wallMat.enableInstancing = false;
        wallMat.doubleSidedGI = true;

        string wallMatPath = $"{materialsPath}/MuseumWall_Realistic.mat";
        AssetDatabase.CreateAsset(wallMat, wallMatPath);
        Debug.Log($"Created wall material: {wallMatPath}");

        Material ceilingMat = new Material(urpLit)
        {
            name = "MuseumCeiling_Realistic"
        };

        ceilingMat.SetColor("_BaseColor", new Color(0.96f, 0.95f, 0.93f, 1f));

        if (interiorTex01 != null)
        {
            ceilingMat.SetTexture("_BaseMap", interiorTex01);
            ceilingMat.SetTextureScale("_BaseMap", new Vector2(6f, 6f));
            ceilingMat.SetTextureOffset("_BaseMap", Vector2.zero);
        }

        Texture2D ceilingNormalToUse = ceilingPlasterNormal ?? floorNormal;
        if (ceilingNormalToUse != null)
        {
            ceilingMat.SetTexture("_BumpMap", ceilingNormalToUse);
            ceilingMat.SetTextureScale("_BumpMap", new Vector2(3f, 3f));
            ceilingMat.SetFloat("_BumpScale", 0.5f);
        }

        ceilingMat.SetFloat("_Smoothness", 0.02f);
        ceilingMat.SetFloat("_Metallic", 0f);
        ceilingMat.SetFloat("_SmoothnessTextureChannel", 1);
        ceilingMat.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
        ceilingMat.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");
        ceilingMat.EnableKeyword("_OCCLUSIONMAP_ON");
        ceilingMat.SetFloat("_OcclusionStrength", 1f);

        ceilingMat.SetFloat("_Surface", 0f);
        ceilingMat.SetFloat("_Cull", 0f);
        ceilingMat.SetFloat("_ReceiveShadows", 1f);
        ceilingMat.SetFloat("_DoubleSidedGI", 1f);

        ceilingMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
        ceilingMat.enableInstancing = false;
        ceilingMat.doubleSidedGI = true;

        string ceilingMatPath = $"{materialsPath}/MuseumCeiling_Realistic.mat";
        AssetDatabase.CreateAsset(ceilingMat, ceilingMatPath);
        Debug.Log($"Created ceiling material: {ceilingMatPath}");

        Material floorMat = new Material(urpLit)
        {
            name = "MuseumFloor_Realistic"
        };

        if (floorAlbedo != null)
        {
            floorMat.SetTexture("_BaseMap", floorAlbedo);
            floorMat.SetTextureScale("_BaseMap", new Vector2(2.5f, 2.5f));
        }
        floorMat.SetColor("_BaseColor", Color.white);

        if (floorNormal != null)
        {
            floorMat.SetTexture("_BumpMap", floorNormal);
            floorMat.SetTextureScale("_BumpMap", new Vector2(2.5f, 2.5f));
            floorMat.SetFloat("_BumpScale", 0.8f);
        }

        floorMat.SetFloat("_Smoothness", 0.15f);
        floorMat.SetFloat("_Metallic", 0f);
        floorMat.EnableKeyword("_SPECULARHIGHLIGHTS_ON");
        floorMat.EnableKeyword("_ENVIRONMENTREFLECTIONS_ON");
        floorMat.SetFloat("_EnvironmentReflections", 0.3f);

        floorMat.SetFloat("_Surface", 0f);
        floorMat.SetFloat("_Cull", 2f);
        floorMat.SetFloat("_ReceiveShadows", 1f);

        floorMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
        floorMat.enableInstancing = true;
        floorMat.doubleSidedGI = false;

        string floorMatPath = $"{materialsPath}/MuseumFloor_Realistic.mat";
        AssetDatabase.CreateAsset(floorMat, floorMatPath);
        Debug.Log($"Created floor material: {floorMatPath}");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Materials Created",
            "Realistic materials created:\n" +
            "- MuseumWall_Realistic.mat\n" +
            "- MuseumCeiling_Realistic.mat\n" +
            "- MuseumFloor_Realistic.mat\n\n" +
            "Next: Run 'Apply Materials to Prefabs' to update all wall/ceiling prefabs.",
            "OK");
    }
}

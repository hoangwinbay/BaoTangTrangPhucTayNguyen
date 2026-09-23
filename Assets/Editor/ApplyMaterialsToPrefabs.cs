using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Applies realistic materials to all wall and ceiling prefabs.
/// Run from Unity Editor: Assets > Museum > Apply Realistic Materials to Prefabs
/// </summary>
public class ApplyMaterialsToPrefabs : EditorWindow
{
    [MenuItem("Assets/Museum/Apply Realistic Materials to Prefabs")]
    public static void ApplyMaterials()
    {
        // Load the new materials
        Material wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/LowPolyInterior2/Materials/MuseumWall_Realistic.mat");
        Material ceilingMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/LowPolyInterior2/Materials/MuseumCeiling_Realistic.mat");
        Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/LowPolyInterior2/Materials/MuseumFloor_Realistic.mat");

        if (wallMat == null || ceilingMat == null)
        {
            EditorUtility.DisplayDialog("Error", "Materials not found! Run 'Create Realistic Materials' first.", "OK");
            return;
        }

        int wallCount = 0;
        int ceilingCount = 0;
        int floorCount = 0;

        // Process Wall prefabs
        string wallsPath = "Assets/LowPolyInterior2/Prefabs/Exterior/Walls";
        if (Directory.Exists(wallsPath))
        {
            string[] wallPrefabs = Directory.GetFiles(wallsPath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in wallPrefabs)
            {
                if (ApplyMaterialToPrefab(prefabPath, wallMat))
                    wallCount++;
            }
        }

        // Process Ceiling prefabs
        string ceilingPath = "Assets/LowPolyInterior2/Prefabs/Exterior/Ceiling";
        if (Directory.Exists(ceilingPath))
        {
            string[] ceilingPrefabs = Directory.GetFiles(ceilingPath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in ceilingPrefabs)
            {
                if (ApplyMaterialToPrefab(prefabPath, ceilingMat))
                    ceilingCount++;
            }
        }

        // Process Floor prefabs
        string floorPath = "Assets/LowPolyInterior2/Prefabs/Exterior/Floor";
        if (Directory.Exists(floorPath) && floorMat != null)
        {
            string[] floorPrefabs = Directory.GetFiles(floorPath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in floorPrefabs)
            {
                if (ApplyMaterialToPrefab(prefabPath, floorMat))
                    floorCount++;
            }
        }

        // Also process any wall/ceiling objects in the main scene
        ApplyToSceneObjects(wallMat, ceilingMat, floorMat);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Materials Applied", 
            $"Updated prefabs:\n" +
            $"- Walls: {wallCount}\n" +
            $"- Ceilings: {ceilingCount}\n" +
            $"- Floors: {floorCount}\n\n" +
            "Scene objects have also been updated.", 
            "OK");
    }

    static bool ApplyMaterialToPrefab(string prefabPath, Material newMaterial)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) return false;

        bool modified = false;
        MeshRenderer[] renderers = prefab.GetComponentsInChildren<MeshRenderer>(true);
        
        foreach (MeshRenderer renderer in renderers)
        {
            Material[] mats = renderer.sharedMaterials;
            bool rendererModified = false;
            
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] != null && mats[i] != newMaterial)
                {
                    // Check if this material should be replaced
                    string matName = mats[i].name.ToLower();
                    if (matName.Contains("wall") || matName.Contains("museumwall") || 
                        matName.Contains("museumceiling") || matName.Contains("lowpolyinterior2_main") ||
                        matName.Contains("interiortexture"))
                    {
                        mats[i] = newMaterial;
                        rendererModified = true;
                        modified = true;
                    }
                }
            }
            
            if (rendererModified)
            {
                renderer.sharedMaterials = mats;
                EditorUtility.SetDirty(renderer);
            }
        }

        if (modified)
        {
            EditorUtility.SetDirty(prefab);
            // PrefabUtility.SavePrefabAsset(prefab); // This saves to disk
            Debug.Log($"Updated: {prefabPath}");
            return true;
        }

        return false;
    }

    static void ApplyToSceneObjects(Material wallMat, Material ceilingMat, Material floorMat)
    {
        // Find all MeshRenderers in the scene
        MeshRenderer[] allRenderers = FindObjectsOfType<MeshRenderer>();
        
        foreach (MeshRenderer renderer in allRenderers)
        {
            Material[] mats = renderer.sharedMaterials;
            bool modified = false;
            
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] != null)
                {
                    string matName = mats[i].name.ToLower();
                    Material replacement = null;
                    
                    if (matName.Contains("wall") || matName.Contains("museumwall"))
                        replacement = wallMat;
                    else if (matName.Contains("ceiling") || matName.Contains("museumceiling"))
                        replacement = ceilingMat;
                    else if (matName.Contains("floor") || matName.Contains("museumfloor") || matName.Contains("images"))
                        replacement = floorMat;
                    
                    if (replacement != null && mats[i] != replacement)
                    {
                        mats[i] = replacement;
                        modified = true;
                    }
                }
            }
            
            if (modified)
            {
                renderer.sharedMaterials = mats;
                EditorUtility.SetDirty(renderer);
            }
        }
        
        // Save scene
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
    }
    
    [MenuItem("Assets/Museum/Revert Material Changes (Undo)")]
    public static void RevertMaterials()
    {
        // Load original materials
        Material origWallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/LowPolyInterior2/Materials/MuseumWallMatte.mat");
        Material origCeilingMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/LowPolyInterior2/Materials/MuseumCeilingMatte.mat");
        Material origFloorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Floor/Materials/images.mat");

        if (origWallMat == null || origCeilingMat == null)
        {
            EditorUtility.DisplayDialog("Error", "Original materials not found!", "OK");
            return;
        }

        int count = 0;

        // Revert Wall prefabs
        string wallsPath = "Assets/LowPolyInterior2/Prefabs/Exterior/Walls";
        if (Directory.Exists(wallsPath))
        {
            string[] wallPrefabs = Directory.GetFiles(wallsPath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in wallPrefabs)
            {
                if (RevertPrefabMaterial(prefabPath, origWallMat))
                    count++;
            }
        }

        // Revert Ceiling prefabs
        string ceilingPath = "Assets/LowPolyInterior2/Prefabs/Exterior/Ceiling";
        if (Directory.Exists(ceilingPath))
        {
            string[] ceilingPrefabs = Directory.GetFiles(ceilingPath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in ceilingPrefabs)
            {
                if (RevertPrefabMaterial(prefabPath, origCeilingMat))
                    count++;
            }
        }

        // Revert Floor prefabs
        string floorPath = "Assets/LowPolyInterior2/Prefabs/Exterior/Floor";
        if (Directory.Exists(floorPath) && origFloorMat != null)
        {
            string[] floorPrefabs = Directory.GetFiles(floorPath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in floorPrefabs)
            {
                if (RevertPrefabMaterial(prefabPath, origFloorMat))
                    count++;
            }
        }

        // Revert scene objects
        RevertSceneObjects(origWallMat, origCeilingMat, origFloorMat);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Reverted", $"Reverted {count} prefabs to original materials.", "OK");
    }

    static bool RevertPrefabMaterial(string prefabPath, Material originalMaterial)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) return false;

        bool modified = false;
        MeshRenderer[] renderers = prefab.GetComponentsInChildren<MeshRenderer>(true);
        
        foreach (MeshRenderer renderer in renderers)
        {
            Material[] mats = renderer.sharedMaterials;
            bool rendererModified = false;
            
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] != null && mats[i] != originalMaterial)
                {
                    string matName = mats[i].name.ToLower();
                    if (matName.Contains("museumwall") || matName.Contains("museumceiling") || 
                        matName.Contains("museumfloor") || matName.Contains("realistic"))
                    {
                        mats[i] = originalMaterial;
                        rendererModified = true;
                        modified = true;
                    }
                }
            }
            
            if (rendererModified)
            {
                renderer.sharedMaterials = mats;
                EditorUtility.SetDirty(renderer);
            }
        }

        if (modified)
        {
            EditorUtility.SetDirty(prefab);
            return true;
        }

        return false;
    }

    static void RevertSceneObjects(Material wallMat, Material ceilingMat, Material floorMat)
    {
        MeshRenderer[] allRenderers = FindObjectsOfType<MeshRenderer>();
        
        foreach (MeshRenderer renderer in allRenderers)
        {
            Material[] mats = renderer.sharedMaterials;
            bool modified = false;
            
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] != null)
                {
                    string matName = mats[i].name.ToLower();
                    Material replacement = null;
                    
                    if (matName.Contains("museumwall") || matName.Contains("realistic"))
                        replacement = wallMat;
                    else if (matName.Contains("museumceiling") || matName.Contains("realistic"))
                        replacement = ceilingMat;
                    else if (matName.Contains("museumfloor") || matName.Contains("realistic"))
                        replacement = floorMat;
                    
                    if (replacement != null && mats[i] != replacement)
                    {
                        mats[i] = replacement;
                        modified = true;
                    }
                }
            }
            
            if (modified)
            {
                renderer.sharedMaterials = mats;
                EditorUtility.SetDirty(renderer);
            }
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
    }
}
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Generates procedural normal maps for realistic plaster/stucco walls and ceilings.
/// </summary>
public class GenerateProceduralNormals : EditorWindow
{
    [MenuItem("Assets/Museum/Generate Procedural Normal Maps")]
    public static void GenerateNormals()
    {
        string texturesPath = "Assets/LowPolyInterior2/Textures";
        if (!Directory.Exists(texturesPath))
            Directory.CreateDirectory(texturesPath);

        // Generate plaster normal map for walls
        Texture2D wallNormal = GeneratePlasterNormal(1024, 1024, 0.08f, "Plaster Wall Normal");
        SaveTexture(wallNormal, $"{texturesPath}/Wall_Plaster_Normal.png", true);

        // Generate smoother ceiling normal map
        Texture2D ceilingNormal = GeneratePlasterNormal(1024, 1024, 0.04f, "Plaster Ceiling Normal");
        SaveTexture(ceilingNormal, $"{texturesPath}/Ceiling_Plaster_Normal.png", true);

        // Generate subtle concrete/breeze block normal for variety
        Texture2D concreteNormal = GenerateConcreteNormal(1024, 1024, "Concrete Normal");
        SaveTexture(concreteNormal, $"{texturesPath}/Wall_Concrete_Normal.png", true);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Normal Maps Generated", 
            "Created procedural normal maps:\n" +
            "- Wall_Plaster_Normal.png\n" +
            "- Ceiling_Plaster_Normal.png\n" +
            "- Wall_Concrete_Normal.png\n\n" +
            "Now run 'Create Realistic Materials' again to use these.", 
            "OK");
    }

    static Texture2D GeneratePlasterNormal(int width, int height, float intensity, string name)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, true, true)
        {
            name = name,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Repeat
        };

        Color[] pixels = new Color[width * height];
        
        // Multi-octave noise for realistic plaster texture
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = (float)x / width;
                float ny = (float)y / height;
                
                // Multiple octaves of Perlin noise for natural plaster look
                float noise = 0f;
                float amplitude = 1f;
                float frequency = 1f;
                float maxAmplitude = 0f;
                
                for (int octave = 0; octave < 5; octave++)
                {
                    float sampleX = nx * frequency * 8f;
                    float sampleY = ny * frequency * 8f;
                    noise += amplitude * Mathf.PerlinNoise(sampleX, sampleY);
                    maxAmplitude += amplitude;
                    amplitude *= 0.5f;
                    frequency *= 2f;
                }
                noise /= maxAmplitude;
                
                // Add some directional variation (trowel marks)
                float trowel = Mathf.PerlinNoise(nx * 20f, ny * 20f) * 
                              Mathf.Sin(nx * 30f) * Mathf.Cos(ny * 30f) * 0.3f;
                noise += trowel;
                
                // Add fine grain noise
                float grain = (Mathf.PerlinNoise(nx * 100f, ny * 100f) - 0.5f) * 0.15f;
                noise += grain;
                
                noise = Mathf.Clamp01(noise);
                
                // Convert to normal map (tangent space)
                // Use Sobel-like derivative for normal
                float h = intensity * noise;
                
                // Calculate derivatives using neighboring pixels (approximate)
                float dx = 0, dy = 0;
                if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
                {
                    // We'll use a simpler approach - encode height as normal
                    float hL = intensity * GetPlasterHeight(nx - 1f/width, ny, width, height);
                    float hR = intensity * GetPlasterHeight(nx + 1f/width, ny, width, height);
                    float hD = intensity * GetPlasterHeight(nx, ny - 1f/height, width, height);
                    float hU = intensity * GetPlasterHeight(nx, ny + 1f/height, width, height);
                    
                    dx = (hR - hL) * 0.5f;
                    dy = (hU - hD) * 0.5f;
                }
                
                // Normal vector in tangent space
                Vector3 normal = new Vector3(-dx, -dy, 1f).normalized;
                
                // Encode in RGB (normal * 0.5 + 0.5)
                pixels[y * width + x] = new Color(
                    normal.x * 0.5f + 0.5f,
                    normal.y * 0.5f + 0.5f,
                    normal.z * 0.5f + 0.5f,
                    1f
                );
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply(true, true);
        return tex;
    }
    
    static float GetPlasterHeight(float nx, float ny, int width, int height)
    {
        // Wrap coordinates
        nx = Mathf.Repeat(nx, 1f);
        ny = Mathf.Repeat(ny, 1f);
        
        float noise = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxAmplitude = 0f;
        
        for (int octave = 0; octave < 5; octave++)
        {
            noise += amplitude * Mathf.PerlinNoise(nx * frequency * 8f, ny * frequency * 8f);
            maxAmplitude += amplitude;
            amplitude *= 0.5f;
            frequency *= 2f;
        }
        noise /= maxAmplitude;
        
        float trowel = Mathf.PerlinNoise(nx * 20f, ny * 20f) * 
                      Mathf.Sin(nx * 30f) * Mathf.Cos(ny * 30f) * 0.3f;
        noise += trowel;
        
        float grain = (Mathf.PerlinNoise(nx * 100f, ny * 100f) - 0.5f) * 0.15f;
        noise += grain;
        
        return Mathf.Clamp01(noise);
    }

    static Texture2D GenerateConcreteNormal(int width, int height, string name)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, true, true)
        {
            name = name,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Repeat
        };

        Color[] pixels = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = (float)x / width;
                float ny = (float)y / height;
                
                // Concrete has larger aggregate patterns
                float noise = 0f;
                float amplitude = 1f;
                float frequency = 1f;
                float maxAmplitude = 0f;
                
                for (int octave = 0; octave < 4; octave++)
                {
                    noise += amplitude * Mathf.PerlinNoise(nx * frequency * 4f, ny * frequency * 4f);
                    maxAmplitude += amplitude;
                    amplitude *= 0.5f;
                    frequency *= 2f;
                }
                noise /= maxAmplitude;
                
                // Add aggregate "stones"
                float aggregate = 0f;
                for (int i = 0; i < 3; i++)
                {
                    float ax = nx * 50f + i * 17.3f;
                    float ay = ny * 50f + i * 23.7f;
                    float dist = Mathf.Abs(Mathf.PerlinNoise(ax, ay) - 0.5f) * 2f;
                    aggregate += Mathf.SmoothStep(0f, 1f, 1f - dist * 5f) * 0.15f;
                }
                noise += aggregate;
                
                // Fine pores
                float pores = (Mathf.PerlinNoise(nx * 200f, ny * 200f) - 0.5f) * 0.1f;
                noise += pores;
                
                noise = Mathf.Clamp01(noise);
                
                // Simple normal from height
                float h = 0.06f * noise;
                
                // Approximate derivatives
                float eps = 1f / width;
                float hL = 0.06f * GetConcreteHeight(nx - eps, ny);
                float hR = 0.06f * GetConcreteHeight(nx + eps, ny);
                float hD = 0.06f * GetConcreteHeight(nx, ny - eps);
                float hU = 0.06f * GetConcreteHeight(nx, ny + eps);
                
                float dx = (hR - hL) * 0.5f;
                float dy = (hU - hD) * 0.5f;
                
                Vector3 normal = new Vector3(-dx, -dy, 1f).normalized;
                
                pixels[y * width + x] = new Color(
                    normal.x * 0.5f + 0.5f,
                    normal.y * 0.5f + 0.5f,
                    normal.z * 0.5f + 0.5f,
                    1f
                );
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply(true, true);
        return tex;
    }
    
    static float GetConcreteHeight(float nx, float ny)
    {
        nx = Mathf.Repeat(nx, 1f);
        ny = Mathf.Repeat(ny, 1f);
        
        float noise = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxAmplitude = 0f;
        
        for (int octave = 0; octave < 4; octave++)
        {
            noise += amplitude * Mathf.PerlinNoise(nx * frequency * 4f, ny * frequency * 4f);
            maxAmplitude += amplitude;
            amplitude *= 0.5f;
            frequency *= 2f;
        }
        noise /= maxAmplitude;
        
        float aggregate = 0f;
        for (int i = 0; i < 3; i++)
        {
            float ax = nx * 50f + i * 17.3f;
            float ay = ny * 50f + i * 23.7f;
            float dist = Mathf.Abs(Mathf.PerlinNoise(ax, ay) - 0.5f) * 2f;
            aggregate += Mathf.SmoothStep(0f, 1f, 1f - dist * 5f) * 0.15f;
        }
        noise += aggregate;
        
        float pores = (Mathf.PerlinNoise(nx * 200f, ny * 200f) - 0.5f) * 0.1f;
        noise += pores;
        
        return Mathf.Clamp01(noise);
    }

    static void SaveTexture(Texture2D tex, string path, bool isNormalMap)
    {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        
        // Import settings
        AssetDatabase.ImportAsset(path);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            if (isNormalMap)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.convertToNormalmap = false; // Already in normal map format
            }
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = true;
            importer.sRGBTexture = !isNormalMap; // Normal maps should be linear
            importer.SaveAndReimport();
        }
        
        Debug.Log($"Saved texture: {path}");
    }
}
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class InspectCube92
{
    [InitializeOnLoadMethod]
    private static void AutoInspect()
    {
        EditorApplication.delayCall += Inspect;
    }

    [MenuItem("Bảo Tàng/Debug/Kiểm tra Cube (92)")]
    public static void Inspect()
    {
        GameObject go = GameObject.Find("Cube (92)");
        if (go == null)
        {
            Debug.LogWarning("[InspectCube92] Không tìm thấy GameObject 'Cube (92)' bằng GameObject.Find, đang tìm kiếm trong toàn bộ hierarchy...");
            var allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (var t in allTransforms)
            {
                if (t.name == "Cube (92)")
                {
                    go = t.gameObject;
                    break;
                }
            }
        }

        if (go == null)
        {
            Debug.LogError("[InspectCube92] Vẫn không tìm thấy 'Cube (92)'!");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"=== THÔNG TIN VẬT THỂ: {go.name} ===");
        sb.AppendLine($"Parent: {(go.transform.parent != null ? go.transform.parent.name : "None (Root)")}");
        sb.AppendLine($"Position: {go.transform.position}");
        sb.AppendLine($"Rotation: {go.transform.eulerAngles}");
        sb.AppendLine($"LossyScale: {go.transform.lossyScale}");

        var rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            sb.AppendLine($"Renderer Type: {rend.GetType().Name}");
            sb.AppendLine($"Bounds Center: {rend.bounds.center}, Size: {rend.bounds.size}");
            sb.AppendLine($"Số lượng Materials: {rend.sharedMaterials.Length}");

            for (int i = 0; i < rend.sharedMaterials.Length; i++)
            {
                var mat = rend.sharedMaterials[i];
                if (mat != null)
                {
                    string matPath = AssetDatabase.GetAssetPath(mat);
                    sb.AppendLine($"Material [{i}]: {mat.name} (Shader: {mat.shader.name}, Path: {matPath})");

                    // Liệt kê các Texture gán vào material
                    string[] texPropNames = mat.GetTexturePropertyNames();
                    foreach (var prop in texPropNames)
                    {
                        Texture tex = mat.GetTexture(prop);
                        if (tex != null)
                        {
                            string texPath = AssetDatabase.GetAssetPath(tex);
                            sb.AppendLine($"   -> Texture '{prop}': {tex.name} (Size: {tex.width}x{tex.height}, Path: {texPath})");

                            TextureImporter importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
                            if (importer != null)
                            {
                                sb.AppendLine($"      Importer: MaxSize={importer.maxTextureSize}, FilterMode={importer.filterMode}, Aniso={importer.anisoLevel}, Mipmap={importer.mipmapEnabled}, MipmapBias={importer.mipMapBias}, Compression={importer.textureCompression}");
                            }
                        }
                    }
                }
            }
        }
        else
        {
            sb.AppendLine("Không có Renderer!");
        }

        string report = sb.ToString();
        Debug.Log(report);
        File.WriteAllText("Assets/Cube92_Report.txt", report);
    }
}

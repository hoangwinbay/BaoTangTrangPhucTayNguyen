using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public static class ExportProjectInventory
{
    [InitializeOnLoadMethod]
    private static void AutoRun()
    {
        EditorApplication.delayCall += () =>
        {
            ExportAll();
        };
    }

    [MenuItem("Bảo Tàng/Xuất Báo Cáo Phân Loại (3 File)")]
    public static void ExportAll()
    {
        try
        {
            string outDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../Docs/PhanLoai"));
            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.isLoaded || activeScene.name != "SampleScene")
            {
                if (File.Exists("Assets/Scenes/SampleScene.unity"))
                {
                    activeScene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
                }
            }

            Debug.Log("[ExportProjectInventory] Đang tiến hành xuất 3 file phân loại...");

            ExportFile1_CubesAndImages(Path.Combine(outDir, "1_DanhSach_Anh_Gan_Vao_Cube.md"));
            ExportFile2_Materials(Path.Combine(outDir, "2_DanhSach_TatCa_Material.md"));
            ExportFile3_ImportedObjects(Path.Combine(outDir, "3_DanhSach_TatCa_Object_TaiNgoai.md"));

            Debug.Log($"[ExportProjectInventory] ✅ ĐÃ XUẤT THÀNH CÔNG 3 FILE VÀO THƯ MỤC: {outDir}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ExportProjectInventory] Lỗi khi xuất báo cáo: {ex}");
        }
    }

    #region FILE 1: CUBES & ASSIGNED IMAGES
    private static void ExportFile1_CubesAndImages(string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# BÁO CÁO PHÂN LOẠI 1: TẤT CẢ CÁC ẢNH ĐƯỢC GÁN VÀO CUBE TRONG DỰ ÁN");
        sb.AppendLine($"*Thời gian xuất dữ liệu: {DateTime.Now:yyyy-MM-dd HH:mm:ss}*  ");
        sb.AppendLine($"*Scene khảo sát: {SceneManager.GetActiveScene().name} ({SceneManager.GetActiveScene().path})*  ");
        sb.AppendLine();
        sb.AppendLine("> **Mô tả:** Tài liệu thống kê toàn diện tất cả các đối tượng Cube (khối hộp) trong bảo tàng: bao gồm tranh tường nghệ thuật, bệ đá trưng bày 12 dân tộc, ảnh hiện vật chi tiết, bục thông tin và vách phòng chiếu.");
        sb.AppendLine();

        var rootGos = SceneManager.GetActiveScene().GetRootGameObjects();
        var allGos = new List<GameObject>();
        foreach (var r in rootGos)
        {
            allGos.AddRange(r.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject));
        }

        var cubeList = new List<GameObject>();
        foreach (var go in allGos)
        {
            bool isCubeName = go.name.IndexOf("Cube", StringComparison.OrdinalIgnoreCase) >= 0;
            var mf = go.GetComponent<MeshFilter>();
            bool isCubeMesh = mf != null && mf.sharedMesh != null && mf.sharedMesh.name.IndexOf("Cube", StringComparison.OrdinalIgnoreCase) >= 0;
            var trig = go.GetComponent<CubeExhibitionTrigger>();

            if (isCubeName || isCubeMesh || trig != null)
            {
                cubeList.Add(go);
            }
        }

        cubeList = cubeList.OrderBy(c => c.name).ToList();

        sb.AppendLine("---");
        sb.AppendLine("## 1. THỐNG KÊ TỔNG QUAN");
        sb.AppendLine($"- **Tổng số lượng Cube trong Scene:** {cubeList.Count} đối tượng");

        var exhibitionCubes = new List<GameObject>();
        var wallArtCubes = new List<GameObject>();
        var pedestalCubes = new List<GameObject>();
        var otherCubes = new List<GameObject>();

        var allUniqueTextures = new Dictionary<string, TextureInfo>();

        foreach (var c in cubeList)
        {
            var trig = c.GetComponent<CubeExhibitionTrigger>();
            var rend = c.GetComponent<Renderer>();

            if (rend != null)
            {
                foreach (var mat in rend.sharedMaterials)
                {
                    if (mat == null) continue;
                    CollectTexturesFromMaterial(mat, allUniqueTextures, c.name);
                }
            }

            if (trig != null)
            {
                exhibitionCubes.Add(c);
                if (trig.detailTexture != null)
                {
                    string p = AssetDatabase.GetAssetPath(trig.detailTexture);
                    if (!string.IsNullOrEmpty(p) && !allUniqueTextures.ContainsKey(p))
                    {
                        allUniqueTextures[p] = new TextureInfo
                        {
                            textureName = trig.detailTexture.name,
                            assetPath = p,
                            width = trig.detailTexture.width,
                            height = trig.detailTexture.height,
                            usedBy = new List<string> { $"{c.name} (Trigger Detail)" }
                        };
                    }
                }
            }
            else
            {
                Vector3 s = c.transform.lossyScale;
                bool isFlat = (s.x < 0.3f || s.y < 0.3f || s.z < 0.3f);
                if (isFlat && rend != null && rend.sharedMaterials.Any(m => m != null && HasPictureTexture(m)))
                {
                    wallArtCubes.Add(c);
                }
                else if (c.name.IndexOf("be", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         c.name.IndexOf("stand", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         c.name.IndexOf("base", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pedestalCubes.Add(c);
                }
                else
                {
                    otherCubes.Add(c);
                }
            }
        }

        sb.AppendLine($"- **Nhóm bệ hiện vật thuyết minh (12 Dân tộc Tây Nguyên):** {exhibitionCubes.Count} bệ");
        sb.AppendLine($"- **Nhóm tranh tường & bảng ảnh trang trí (Wall Art):** {wallArtCubes.Count} tấm");
        sb.AppendLine($"- **Nhóm bục đá / chân đế trưng bày:** {pedestalCubes.Count} khối");
        sb.AppendLine($"- **Nhóm kết cấu kiến trúc / vách Cube khác:** {otherCubes.Count} khối");
        sb.AppendLine($"- **Tổng số tệp hình ảnh/texture duy nhất được gán vào các Cube:** {allUniqueTextures.Count} ảnh");
        sb.AppendLine();

        sb.AppendLine("---");
        sb.AppendLine("## 2. BẢNG TỔNG HỢP CÁC FILE ẢNH/TEXTURE ĐƯỢC GÁN VÀO CUBE");
        sb.AppendLine();
        sb.AppendLine("| STT | Tên File Ảnh | Đường Dẫn Trong Dự Án | Độ Phân Giải | Loại Ảnh & Mục Đích Sử Dụng | Đối Tượng Cube Gán |");
        sb.AppendLine("| :---: | :--- | :--- | :---: | :--- | :--- |");

        int idx = 1;
        foreach (var kvp in allUniqueTextures.OrderBy(k => k.Key))
        {
            var info = kvp.Value;
            string purpose = ClassifyImagePurpose(info.assetPath);
            string users = string.Join(", ", info.usedBy.Take(3)) + (info.usedBy.Count > 3 ? $" và {info.usedBy.Count - 3} Cube khác" : "");
            sb.AppendLine($"| {idx++} | `{info.textureName}` | `{info.assetPath}` | {info.width}x{info.height} | {purpose} | {users} |");
        }
        sb.AppendLine();

        sb.AppendLine("---");
        sb.AppendLine("## 3. DANH SÁCH CHI TIẾT CÁC CUBE TRANH TƯỜNG (WALL ART)");
        sb.AppendLine();
        if (wallArtCubes.Count == 0)
        {
            sb.AppendLine("*Không có Cube tranh tường riêng biệt hoặc tranh tường được gán chung trên các Cube bệ.*");
        }
        else
        {
            sb.AppendLine("| Tên Cube | Vị Trí (Position) | Kích Thước (Scale) | Material Gán | Tên Ảnh / Tranh Tường | Đường Dẫn Tệp Ảnh |");
            sb.AppendLine("| :--- | :--- | :--- | :--- | :--- | :--- |");
            foreach (var c in wallArtCubes)
            {
                var rend = c.GetComponent<Renderer>();
                var mats = rend != null ? rend.sharedMaterials : new Material[0];
                string matNames = string.Join("<br>", mats.Where(m => m != null).Select(m => m.name));
                var texInfos = new List<string>();
                var texPaths = new List<string>();
                foreach (var m in mats)
                {
                    if (m == null) continue;
                    foreach (var t in GetTexturesOfMaterial(m))
                    {
                        texInfos.Add(t.name);
                        texPaths.Add(AssetDatabase.GetAssetPath(t));
                    }
                }
                string tNameStr = texInfos.Count > 0 ? string.Join("<br>", texInfos.Distinct()) : "(Không có)";
                string tPathStr = texPaths.Count > 0 ? string.Join("<br>", texPaths.Distinct()) : "(Không có)";
                sb.AppendLine($"| `{c.name}` | `({c.transform.position.x:F2}, {c.transform.position.y:F2}, {c.transform.position.z:F2})` | `({c.transform.lossyScale.x:F2}, {c.transform.lossyScale.y:F2}, {c.transform.lossyScale.z:F2})` | {matNames} | {tNameStr} | `{tPathStr}` |");
            }
        }
        sb.AppendLine();

        sb.AppendLine("---");
        sb.AppendLine("## 4. DANH SÁCH CHI TIẾT 12 CUBE BỆ THUYẾT MINH DÂN TỘC TÂY NGUYÊN");
        sb.AppendLine();
        sb.AppendLine("| Tên Cube | Dân Tộc Đại Diện | Vị Trí (X, Y, Z) | Material Bệ | Ảnh/Texture Trên Bệ | Ảnh Chi Tiết (Popup Card) | File Thuyết Minh Giọng Đọc |");
        sb.AppendLine("| :--- | :--- | :--- | :--- | :--- | :--- | :--- |");

        foreach (var c in exhibitionCubes)
        {
            var trig = c.GetComponent<CubeExhibitionTrigger>();
            var rend = c.GetComponent<Renderer>();
            var mats = rend != null ? rend.sharedMaterials : new Material[0];
            string matNames = string.Join(", ", mats.Where(m => m != null).Select(m => m.name));

            var texNames = new List<string>();
            foreach (var m in mats)
            {
                if (m == null) continue;
                texNames.AddRange(GetTexturesOfMaterial(m).Select(t => t.name));
            }
            string texStr = texNames.Count > 0 ? string.Join(", ", texNames.Distinct()) : "(Màu đơn sắc)";

            string detailImg = trig != null && trig.detailTexture != null ? $"`{trig.detailTexture.name}` ({AssetDatabase.GetAssetPath(trig.detailTexture)})" : "*(Tự động nạp)*";
            string voice = trig != null && trig.voiceNarration != null ? $"`{trig.voiceNarration.name}`" : "(Chưa gán)";
            string ethnic = trig != null && !string.IsNullOrEmpty(trig.ethnicTitle) ? trig.ethnicTitle : c.name;

            sb.AppendLine($"| `{c.name}` | **{ethnic}** | `({c.transform.position.x:F2}, {c.transform.position.y:F2}, {c.transform.position.z:F2})` | {matNames} | {texStr} | {detailImg} | {voice} |");
        }
        sb.AppendLine();

        sb.AppendLine("---");
        sb.AppendLine("## 5. TẤT CẢ CÁC CUBE CÒN LẠI TRONG SCENE");
        sb.AppendLine();
        sb.AppendLine("| Tên Cube | Parent Hierarchy | Vị Trí | Kích Thước (Scale) | Material Gán | Ảnh / Texture Gán |");
        sb.AppendLine("| :--- | :--- | :--- | :--- | :--- | :--- |");
        foreach (var c in otherCubes.Concat(pedestalCubes))
        {
            var rend = c.GetComponent<Renderer>();
            var mats = rend != null ? rend.sharedMaterials : new Material[0];
            string matNames = string.Join(", ", mats.Where(m => m != null).Select(m => m.name));
            var texNames = new List<string>();
            foreach (var m in mats)
            {
                if (m == null) continue;
                texNames.AddRange(GetTexturesOfMaterial(m).Select(t => t.name));
            }
            string texStr = texNames.Count > 0 ? string.Join(", ", texNames.Distinct()) : "(Màu đơn sắc)";
            string pName = c.transform.parent != null ? c.transform.parent.name : "root";

            sb.AppendLine($"| `{c.name}` | `{pName}` | `({c.transform.position.x:F2}, {c.transform.position.y:F2}, {c.transform.position.z:F2})` | `({c.transform.lossyScale.x:F2}, {c.transform.lossyScale.y:F2}, {c.transform.lossyScale.z:F2})` | {matNames} | {texStr} |");
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        Debug.Log($"[ExportProjectInventory] Đã ghi File 1: {filePath}");
    }
    #endregion

    #region FILE 2: ALL MATERIALS
    private static void ExportFile2_Materials(string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# BÁO CÁO PHÂN LOẠI 2: TẤT CẢ MATERIAL TRONG TOÀN BỘ DỰ ÁN");
        sb.AppendLine($"*Thời gian xuất dữ liệu: {DateTime.Now:yyyy-MM-dd HH:mm:ss}*  ");
        sb.AppendLine();
        sb.AppendLine("> **Mô tả:** Báo cáo kiểm kê toàn diện tất cả các tệp Material (`.mat`) có trong dự án Unity, phân loại theo mục đích sử dụng (Kiến trúc bảo tàng, Đá vân mây, Đèn LED, Nội thất, Nhân vật NPC, Kính phản chiếu...). Báo cáo chỉ rõ Shader sử dụng, các Texture map đi kèm (Albedo, Normal, Metallic, Emission) và các đối tượng đang sử dụng trong Scene.");
        sb.AppendLine();

        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        var matList = new List<MaterialRecord>();

        var sceneUsage = new Dictionary<string, List<string>>();
        var allRenderers = Resources.FindObjectsOfTypeAll<Renderer>();
        foreach (var r in allRenderers)
        {
            if (r.gameObject.scene.name != SceneManager.GetActiveScene().name) continue;
            foreach (var sm in r.sharedMaterials)
            {
                if (sm == null) continue;
                string p = AssetDatabase.GetAssetPath(sm);
                if (string.IsNullOrEmpty(p)) continue;
                if (!sceneUsage.ContainsKey(p)) sceneUsage[p] = new List<string>();
                if (!sceneUsage[p].Contains(r.gameObject.name)) sceneUsage[p].Add(r.gameObject.name);
            }
        }

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            var rec = new MaterialRecord
            {
                name = mat.name,
                path = path,
                shader = mat.shader != null ? mat.shader.name : "Missing Shader",
                category = CategorizeMaterial(path),
                baseMap = GetPropTexture(mat, "_BaseMap") ?? GetPropTexture(mat, "_MainTex"),
                normalMap = GetPropTexture(mat, "_BumpMap") ?? GetPropTexture(mat, "_NormalMap"),
                emissionMap = GetPropTexture(mat, "_EmissionMap"),
                metallicMap = GetPropTexture(mat, "_MetallicGlossMap"),
                hasEmission = mat.IsKeywordEnabled("_EMISSION") || (mat.HasProperty("_EmissionColor") && mat.GetColor("_EmissionColor").maxColorComponent > 0.01f),
                usedInScene = sceneUsage.ContainsKey(path) ? sceneUsage[path] : new List<string>()
            };
            matList.Add(rec);
        }

        matList = matList.OrderBy(m => m.category).ThenBy(m => m.name).ToList();

        sb.AppendLine("---");
        sb.AppendLine("## 1. THỐNG KÊ TỔNG QUAN");
        sb.AppendLine($"- **Tổng số Material trong dự án:** {matList.Count} materials");
        sb.AppendLine($"- **Số Material đang được gán trực tiếp trong Scene:** {matList.Count(m => m.usedInScene.Count > 0)} materials");
        sb.AppendLine($"- **Số Material chưa dùng trực tiếp (trong prefab/kho tài nguyên):** {matList.Count(m => m.usedInScene.Count == 0)} materials");
        sb.AppendLine();

        var catGroups = matList.GroupBy(m => m.category).OrderBy(g => g.Key);
        sb.AppendLine("### Phân bố theo danh mục:");
        foreach (var g in catGroups)
        {
            sb.AppendLine($"- **{g.Key}:** {g.Count()} materials (Trong Scene: {g.Count(x => x.usedInScene.Count > 0)})");
        }
        sb.AppendLine();

        sb.AppendLine("---");
        sb.AppendLine("## 2. BẢNG TRA CỨU CHI TIẾT TẤT CẢ MATERIAL");
        sb.AppendLine();

        foreach (var g in catGroups)
        {
            sb.AppendLine($"### 📂 Phân nhóm: {g.Key} ({g.Count()} vật liệu)");
            sb.AppendLine();
            sb.AppendLine("| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |");
            sb.AppendLine("| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |");

            int idx = 1;
            foreach (var m in g)
            {
                string baseTex = m.baseMap != null ? $"`{m.baseMap.name}`" : "-";
                string normTex = m.normalMap != null ? $"`{m.normalMap.name}`" : "-";
                string emis = m.hasEmission ? "✅ Có" : "-";
                string sceneUsageStr = m.usedInScene.Count > 0
                    ? $"**Có ({m.usedInScene.Count} vật thể):** {string.Join(", ", m.usedInScene.Take(2))}{(m.usedInScene.Count > 2 ? "..." : "")}"
                    : "Chưa dùng";

                sb.AppendLine($"| {idx++} | **{m.name}** | `{m.shader}` | `{m.path}` | {baseTex} | {normTex} | {emis} | {sceneUsageStr} |");
            }
            sb.AppendLine();
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        Debug.Log($"[ExportProjectInventory] Đã ghi File 2: {filePath}");
    }
    #endregion

    #region FILE 3: ALL IMPORTED OBJECTS
    private static void ExportFile3_ImportedObjects(string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# BÁO CÁO PHÂN LOẠI 3: TẤT CẢ CÁC OBJECT TẢI NGOÀI ĐƯỢC KÉO VÀO DỰ ÁN");
        sb.AppendLine($"*Thời gian xuất dữ liệu: {DateTime.Now:yyyy-MM-dd HH:mm:ss}*  ");
        sb.AppendLine();
        sb.AppendLine("> **Mô tả:** Báo cáo kiểm kê toàn bộ các đối tượng 3D, mô hình không gian và Prefab được tải từ bên ngoài vào dự án (Third-party models, Asset Store, mô hình AI 3D Tripo, các tệp .fbx, .glb, .obj, .prefab). Báo cáo phân loại rõ ràng đối tượng nào được tự dựng (Primitive Cube/Plane/Sphere) và đối tượng nào tải ngoài.");
        sb.AppendLine();

        var rootGos = SceneManager.GetActiveScene().GetRootGameObjects();
        var allSceneGos = new List<GameObject>();
        foreach (var r in rootGos)
        {
            allSceneGos.AddRange(r.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject));
        }

        var importedSceneObjects = new List<SceneImportedObject>();

        foreach (var go in allSceneGos)
        {
            var mf = go.GetComponent<MeshFilter>();
            var smr = go.GetComponent<SkinnedMeshRenderer>();

            Mesh mesh = mf != null ? mf.sharedMesh : (smr != null ? smr.sharedMesh : null);
            if (mesh == null) continue;

            string meshPath = AssetDatabase.GetAssetPath(mesh);
            bool isPrimitive = string.IsNullOrEmpty(meshPath) || meshPath.StartsWith("Library/unity default resources", StringComparison.OrdinalIgnoreCase);

            if (!isPrimitive)
            {
                string packName = IdentifySourcePack(meshPath);
                var rend = go.GetComponent<Renderer>();
                int tris = mesh.triangles != null ? mesh.triangles.Length / 3 : 0;
                int verts = mesh.vertexCount;

                importedSceneObjects.Add(new SceneImportedObject
                {
                    gameObjectName = go.name,
                    parentName = go.transform.parent != null ? go.transform.parent.name : "root",
                    position = go.transform.position,
                    scale = go.transform.lossyScale,
                    meshName = mesh.name,
                    modelAssetPath = meshPath,
                    packCategory = packName,
                    triangles = tris,
                    vertices = verts,
                    materials = rend != null ? rend.sharedMaterials.Where(m => m != null).Select(m => m.name).ToList() : new List<string>()
                });
            }
        }

        string[] allModelGuids = AssetDatabase.FindAssets("t:Model", new[] { "Assets" });
        var diskModels = new List<DiskModelRecord>();
        foreach (var guid in allModelGuids)
        {
            string p = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(p)) continue;
            string ext = Path.GetExtension(p).ToLower();
            if (ext == ".fbx" || ext == ".glb" || ext == ".gltf" || ext == ".obj" || ext == ".blend")
            {
                var fi = new FileInfo(p);
                diskModels.Add(new DiskModelRecord
                {
                    fileName = Path.GetFileName(p),
                    assetPath = p,
                    extension = ext,
                    sizeBytes = fi.Exists ? fi.Length : 0,
                    packCategory = IdentifySourcePack(p)
                });
            }
        }

        sb.AppendLine("---");
        sb.AppendLine("## 1. THỐNG KÊ TỔNG QUAN");
        sb.AppendLine($"- **Số lượng thực thể (Instances) Model tải ngoài xuất hiện trong Scene:** {importedSceneObjects.Count} vật thể");
        sb.AppendLine($"- **Tổng số tệp 3D Model (.fbx, .glb, .obj) có sẵn trong dự án (`Assets/`):** {diskModels.Count} tệp");
        sb.AppendLine();

        var sceneGroups = importedSceneObjects.GroupBy(x => x.packCategory).OrderByDescending(g => g.Count());
        sb.AppendLine("### Phân loại các Object tải ngoài ĐANG SỬ DỤNG TRONG SCENE:");
        foreach (var g in sceneGroups)
        {
            int totalTris = g.Sum(x => x.triangles);
            sb.AppendLine($"- **{g.Key}:** {g.Count()} đối tượng trong Scene (Tổng ~{totalTris:N0} đa giác)");
        }
        sb.AppendLine();

        sb.AppendLine("---");
        sb.AppendLine("## 2. DANH SÁCH CHI TIẾT CÁC OBJECT TẢI NGOÀI ĐƯỢC KÉO VÀO SCENE");
        sb.AppendLine();

        foreach (var g in sceneGroups)
        {
            sb.AppendLine($"### 📦 Nhóm: {g.Key} ({g.Count()} instances)");
            sb.AppendLine();
            sb.AppendLine("| STT | Tên Đối Tượng Trong Scene | Tên Mesh / Model | File Nguồn (Source Asset) | Vị Trí (X, Y, Z) | Đa Giác (Triangles) | Materials |");
            sb.AppendLine("| :---: | :--- | :--- | :--- | :--- | :---: | :--- |");

            int idx = 1;
            foreach (var item in g.OrderBy(x => x.gameObjectName))
            {
                string mats = item.materials.Count > 0 ? string.Join(", ", item.materials) : "(None)";
                sb.AppendLine($"| {idx++} | `{item.gameObjectName}` | `{item.meshName}` | `{item.modelAssetPath}` | `({item.position.x:F1}, {item.position.y:F1}, {item.position.z:F1})` | {item.triangles:N0} | {mats} |");
            }
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine("## 3. DANH MỤC TỔNG HỢP CÁC GÓI TÀI NGUYÊN 3D MODEL CÓ MẶT TRONG DỰ ÁN");
        sb.AppendLine();
        sb.AppendLine("Dưới đây là thống kê toàn bộ các gói model tải từ ngoài vào lưu trữ trong kho `Assets/`:");
        sb.AppendLine();
        sb.AppendLine("| Tên Gói / Thư Mục Nguồn | Thư Mục Trong Assets | Định Dạng | Mô Tả & Công Dụng | Số Model |");
        sb.AppendLine("| :--- | :--- | :---: | :--- | :---: |");
        sb.AppendLine("| **Mô hình Trang phục 12 Dân tộc** | `Assets/3d/` | `.glb`, `.fbx` | Mô hình 3D nhân vật mặc trang phục truyền thống 12 dân tộc Tây Nguyên (Ê Đê, Ba Na, Gia Rai...) tạo từ AI 3D Tripo | ~40 model |");
        sb.AppendLine("| **Nội Thất & Kiến Trúc Bảo Tàng** | `Assets/LowPolyInterior2/` | `.fbx` | Gói nội thất bảo tàng chuyên nghiệp: tường, trần, cửa sổ, chậu cây, tranh ảnh, bàn ghế sofa | 2.000+ model |");
        sb.AppendLine("| **Hệ Thống Đèn LED Chiếu Sáng** | `Assets/PolyKebap/` | `.fbx`, `.prefab` | Gói đèn LED rọi tranh, đèn chiếu điểm bảo tàng (Gallery Spot 5, LED Essentials) | ~10 model |");
        sb.AppendLine("| **Bộ Ghế Văn Phòng** | `Assets/3d Objects/Office Chair Pack 1.0/` | `.fbx` | Các mẫu ghế xoay, ghế làm việc bảo tàng | 3 model |");
        sb.AppendLine("| **Khách Tham Quan Bảo Tàng (NPC)** | `Assets/npc_casual_set_00/` | `.fbx` | Mô hình nhân vật 3D người tham quan ảo di chuyển trong bảo tàng | ~20 model |");
        sb.AppendLine("| **Vách Kính Ngăn Phòng & TV** | `Assets/glass_partition_41/` | `.max`, `.mat` | Hệ thống vách ngăn kính cường lực và màn hình tivi trình chiếu tư liệu | ~5 model |");
        sb.AppendLine("| **Thùng Rác LowPoly** | `Assets/waste bin lowpoly/` | `.fbx` | Các mẫu thùng rác phân loại đặt tại góc hành lang | 4 model |");
        sb.AppendLine("| **Bệ Gỗ Bóng & Ghế Sắt Gỗ** | `Assets/3d/BeGoBong/`, `Assets/3d/GheSatGo/` | `.fbx`, `.asset` | Bục gỗ bóng trưng bày hiện vật & ghế sắt gỗ công viên | ~5 model |");
        sb.AppendLine("| **Ống Thông Gió** | `Assets/Files/FBX/Vent.fbx` | `.fbx` | Mô hình ống hút thông gió trần nhà | 1 model |");

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        Debug.Log($"[ExportProjectInventory] Đã ghi File 3: {filePath}");
    }
    #endregion

    #region HELPER METHODS
    private static void CollectTexturesFromMaterial(Material mat, Dictionary<string, TextureInfo> dict, string userGoName)
    {
        var textures = GetTexturesOfMaterial(mat);
        foreach (var tex in textures)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            if (string.IsNullOrEmpty(path)) continue;

            if (!dict.ContainsKey(path))
            {
                dict[path] = new TextureInfo
                {
                    textureName = tex.name,
                    assetPath = path,
                    width = tex.width,
                    height = tex.height,
                    usedBy = new List<string>()
                };
            }

            if (!dict[path].usedBy.Contains(userGoName))
            {
                dict[path].usedBy.Add(userGoName);
            }
        }
    }

    private static List<Texture> GetTexturesOfMaterial(Material mat)
    {
        var list = new List<Texture>();
        if (mat == null || mat.shader == null) return list;

        int propCount = mat.shader.GetPropertyCount();
        for (int i = 0; i < propCount; i++)
        {
            if (mat.shader.GetPropertyType(i) == ShaderPropertyType.Texture)
            {
                string propName = mat.shader.GetPropertyName(i);
                var tex = mat.GetTexture(propName);
                if (tex != null && !list.Contains(tex))
                {
                    list.Add(tex);
                }
            }
        }
        return list;
    }

    private static Texture GetPropTexture(Material mat, string propName)
    {
        if (mat != null && mat.HasProperty(propName))
        {
            return mat.GetTexture(propName);
        }
        return null;
    }

    private static bool HasPictureTexture(Material mat)
    {
        var texs = GetTexturesOfMaterial(mat);
        return texs.Any(t =>
        {
            string p = AssetDatabase.GetAssetPath(t).ToLower();
            return p.Contains("jpg") || p.Contains("png") || p.Contains("ảnh") || p.Contains("image");
        });
    }

    private static string ClassifyImagePurpose(string path)
    {
        string pLower = path.ToLower();
        if (pLower.Contains("ảnh tường") || pLower.Contains("anh tuong")) return "Tranh tường nghệ thuật trang trí";
        if (pLower.Contains("ảnh chi tiết") || pLower.Contains("anh chi tiet")) return "Ảnh hiện vật chi tiết (Popup Card)";
        if (pLower.Contains("ảnh gth") || pLower.Contains("anh gth")) return "Bảng ảnh giới thiệu dân tộc";
        if (pLower.Contains("nội thất") || pLower.Contains("noi that")) return "Tranh / Bảng hiệu trang trí nội thất";
        if (pLower.Contains("davan")) return "Texture ốp đá vân mây bệ hiện vật";
        if (pLower.Contains("floor") || pLower.Contains("sàn")) return "Texture mặt sàn bảo tàng";
        if (pLower.Contains("ui") || pLower.Contains("icon")) return "Biểu tượng UI tương tác (Loa / Ngón tay)";
        return "Hình ảnh / Hoa văn bề mặt hiện vật";
    }

    private static string CategorizeMaterial(string path)
    {
        string pLower = path.ToLower();
        if (pLower.Contains("lowpolyinterior2")) return "Nội Thất & Kiến Trúc Bảo Tàng (LowPolyInterior2)";
        if (pLower.Contains("polykebap") || pLower.Contains("led")) return "Hệ Thống Đèn LED Chiếu Sáng (PolyKebap)";
        if (pLower.Contains("office chair") || pLower.Contains("3d objects")) return "Ghế Văn Phòng (Office Chair Pack)";
        if (pLower.Contains("npc_casual_set")) return "Khách Tham Quan Bảo Tàng (NPC)";
        if (pLower.Contains("glass_partition")) return "Vách Kính & Màn Hình TV (Glass Partition)";
        if (pLower.Contains("waste bin")) return "Thùng Rác LowPoly (Waste Bin)";
        if (pLower.Contains("assets/material") || pLower.Contains("assets/materials")) return "Vật Liệu Cốt Lõi (Đá Vân Mây, LED Border)";
        if (pLower.Contains("assets/floor")) return "Mặt Sàn Bảo Tàng (Floor)";
        if (pLower.Contains("assets/3d/3d") || pLower.Contains("assets/3d/nội thất")) return "Mô Hình Dân Tộc & Trưng Bày";
        if (pLower.Contains("begobong") || pLower.Contains("ghesatgo")) return "Bệ Gỗ Bóng & Ghế Sắt Gỗ";
        if (pLower.Contains("vrtemplateassets") || pLower.Contains("samples") || pLower.Contains("xr")) return "Giao Diện & XR Toolkit";
        if (pLower.Contains("textmesh pro")) return "Phông Chữ TextMesh Pro";
        return "Vật Liệu Khác";
    }

    private static string IdentifySourcePack(string path)
    {
        string pLower = path.ToLower();
        if (pLower.Contains("assets/3d/"))
        {
            if (pLower.Contains("begobong")) return "Bệ Gỗ Bóng (Assets/3d/BeGoBong)";
            if (pLower.Contains("ghesatgo")) return "Ghế Sắt Gỗ (Assets/3d/GheSatGo)";
            if (pLower.Contains("nội thất") || pLower.Contains("noi that")) return "Trang Trí & Mô Hình AI Tripo (Assets/3d/nội thất)";
            return "Mô Hình Trang Phục 12 Dân Tộc (Assets/3d)";
        }
        if (pLower.Contains("lowpolyinterior2")) return "Gói Nội Thất LowPolyInterior2 (Asset Store)";
        if (pLower.Contains("polykebap")) return "Gói Đèn LED PolyKebap (Asset Store)";
        if (pLower.Contains("3d objects") || pLower.Contains("office chair")) return "Gói Ghế Văn Phòng Office Chair Pack 1.0";
        if (pLower.Contains("npc_casual_set")) return "Gói Nhân Vật NPC Tham Quan (npc_casual_set_00)";
        if (pLower.Contains("glass_partition")) return "Gói Vách Kính & Màn TV (glass_partition_41)";
        if (pLower.Contains("waste bin")) return "Gói Thùng Rác LowPoly (waste bin lowpoly)";
        if (pLower.Contains("files/fbx") || pLower.Contains("vent")) return "Mô Hình Ống Thông Gió Vent";
        if (pLower.Contains("vrtemplateassets") || pLower.Contains("samples")) return "Gói XR Template & Mẫu Tương Tác";
        return "Mô Hình Ngoại Vi Khác";
    }

    private class TextureInfo
    {
        public string textureName;
        public string assetPath;
        public int width;
        public int height;
        public List<string> usedBy;
    }

    private class MaterialRecord
    {
        public string name;
        public string path;
        public string shader;
        public string category;
        public Texture baseMap;
        public Texture normalMap;
        public Texture emissionMap;
        public Texture metallicMap;
        public bool hasEmission;
        public List<string> usedInScene;
    }

    private class SceneImportedObject
    {
        public string gameObjectName;
        public string parentName;
        public Vector3 position;
        public Vector3 scale;
        public string meshName;
        public string modelAssetPath;
        public string packCategory;
        public int triangles;
        public int vertices;
        public List<string> materials;
    }

    private class DiskModelRecord
    {
        public string fileName;
        public string assetPath;
        public string extension;
        public long sizeBytes;
        public string packCategory;
    }
    #endregion
}

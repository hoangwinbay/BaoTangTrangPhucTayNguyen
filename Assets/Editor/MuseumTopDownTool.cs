using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Công cụ chuyên dụng: Ẩn/Hiện trần nhà và cửa sổ trần để chụp ảnh Top-Down bảo tàng.
/// Tuyệt đối KHÔNG xóa bất kỳ vật thể nào - chỉ tắt bật hiển thị (SetActive).
/// </summary>
public class MuseumTopDownTool : EditorWindow
{
    private const string PrefKeyCeilingHidden = "Museum_TopDown_IsCeilingHidden";
    private const string PrefKeyHideCeilingLights = "Museum_TopDown_HideCeilingLights";

    // Danh sách các Cube được bảo vệ tuyệt đối (bệ trưng bày thuyết minh, tranh tường, ghế, TV)
    private static readonly HashSet<string> ProtectedCubeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        // 12 bệ trưng bày thuyết minh
        "Cube (56)", "Cube (58)", "Cube (60)", "Cube (62)", "Cube (64)", "Cube (66)",
        "Cube (68)", "Cube (70)", "Cube (72)", "Cube (74)", "Cube (76)", "Cube (78)",
        // 12 tranh ảnh infographics tường
        "Cube (32)", "Cube (45)", "Cube (46)", "Cube (47)", "Cube (48)", "Cube (50)",
        "Cube (51)", "Cube (52)", "Cube (53)", "Cube (54)", "Cube (79)", "Cube (80)",
        // 5 bệ ghế đá vân
        "Cube (33)", "Cube (34)", "Cube (35)", "Cube (37)", "Cube (38)",
        // 1 màn hình TV LED
        "Cube (92)"
    };

    [InitializeOnLoadMethod]
    private static void AutoRunOnLoad()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            // Tự động thực thi ẩn trần nhà và cửa sổ trần theo yêu cầu người dùng
            ExecuteHide(hide: true, hideCeilingLights: false, showDialog: false);
        };
    }

    [MenuItem("Bảo Tàng/Ẩn Trần & Cửa Sổ Trần (Chụp Top-Down)", false, 20)]
    public static void MenuHideCeiling()
    {
        bool hideLights = EditorPrefs.GetBool(PrefKeyHideCeilingLights, false);
        ExecuteHide(hide: true, hideCeilingLights: hideLights, showDialog: true);
    }

    [MenuItem("Bảo Tàng/Hiện Lại Trần & Cửa Sổ Trần", false, 21)]
    public static void MenuRestoreCeiling()
    {
        ExecuteHide(hide: false, hideCeilingLights: true, showDialog: true);
    }

    [MenuItem("Bảo Tàng/Cửa Sổ Điều Khiển Chụp Top-Down...", false, 22)]
    public static void ShowWindow()
    {
        var window = GetWindow<MuseumTopDownTool>("Chụp Top-Down Bảo Tàng");
        window.minSize = new Vector2(480, 520);
        window.Show();
    }

    private void OnGUI()
    {
        bool isHidden = EditorPrefs.GetBool(PrefKeyCeilingHidden, false);
        bool hideLights = EditorPrefs.GetBool(PrefKeyHideCeilingLights, false);

        EditorGUILayout.Space(10);
        GUILayout.Label("CÔNG CỤ HỖ TRỢ CHỤP ẢNH TOP-DOWN BẢO TÀNG", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Ẩn toàn bộ Cube trần nhà và các cửa sổ trần (Window6_04L) để nhìn thấu toàn bộ không gian phòng ốc từ trên cao.\n" +
            "Toàn bộ mô hình được BẢO TOÀN NGUYÊN VẸN 100% (không xóa bất kỳ đối tượng nào).",
            MessageType.Info);

        EditorGUILayout.Space(10);
        GUILayout.Label("1. Trạng Thái Hiện Tại:", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Trần & Cửa sổ trần:", GUILayout.Width(150));
        if (isHidden)
        {
            GUI.color = new Color(1f, 0.4f, 0.4f);
            GUILayout.Label("● ĐANG ĐƯỢC ẨN (Sẵn sàng chụp Top-Down)", EditorStyles.boldLabel);
        }
        else
        {
            GUI.color = new Color(0.3f, 0.9f, 0.4f);
            GUILayout.Label("● ĐANG HIỂN THỊ ĐẦY ĐỦ", EditorStyles.boldLabel);
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);
        bool newHideLights = EditorGUILayout.ToggleLeft(" Ẩn luôn đèn treo trần (Light_09) và ống gió (Vent) để góc nhìn phẳng sạch nhất", hideLights);
        if (newHideLights != hideLights)
        {
            EditorPrefs.SetBool(PrefKeyHideCeilingLights, newHideLights);
            hideLights = newHideLights;
        }

        EditorGUILayout.Space(15);
        GUILayout.Label("2. Thao Tác Ẩn / Hiện:", EditorStyles.boldLabel);

        GUI.backgroundColor = new Color(0.3f, 0.85f, 0.4f);
        if (GUILayout.Button("★ ẨN CUBE TRẦN & CỬA SỔ TRẦN (CHỤP TOP-DOWN)", GUILayout.Height(45)))
        {
            ExecuteHide(hide: true, hideCeilingLights: hideLights, showDialog: true);
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        GUI.backgroundColor = new Color(0.4f, 0.7f, 1f);
        if (GUILayout.Button("HIỆN LẠI TRẦN & CỬA SỔ TRẦN (MẶC ĐỊNH)", GUILayout.Height(35)))
        {
            ExecuteHide(hide: false, hideCeilingLights: true, showDialog: true);
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(20);
        GUILayout.Label("3. Hỗ Trợ Góc Chụp & Xuất Ảnh:", EditorStyles.boldLabel);

        if (GUILayout.Button("Đặt Góc Camera SceneView Thẳng Từ Trên Xuống (Phối Cảnh Perspective)", GUILayout.Height(38)))
        {
            AlignSceneViewTopDown(orthographic: false);
        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Đặt Góc Camera Trực Giao Bản Vẽ Mặt Bằng (Orthographic Floorplan)", GUILayout.Height(35)))
        {
            AlignSceneViewTopDown(orthographic: true);
        }

        EditorGUILayout.Space(5);

        GUI.backgroundColor = new Color(1f, 0.75f, 0.2f);
        if (GUILayout.Button("📸 Chụp & Lưu Ảnh Top-Down Độ Phân Giải Cao (4K PNG)", GUILayout.Height(40)))
        {
            CaptureTopDownScreenshot(3840, 2160);
        }
        GUI.backgroundColor = Color.white;
    }

    /// <summary>
    /// Thực thi ẩn hoặc hiện trần nhà và cửa sổ trần
    /// </summary>
    public static void ExecuteHide(bool hide, bool hideCeilingLights, bool showDialog = false)
    {
        var targetObjects = FindCeilingAndSkylightObjects(hideCeilingLights);

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName(hide ? "Hide Ceiling For TopDown" : "Restore Ceiling");

        int hiddenCount = 0;
        int restoredCount = 0;

        foreach (var go in targetObjects)
        {
            if (go == null) continue;

            if (hide)
            {
                if (go.activeSelf)
                {
                    Undo.RecordObject(go, "Hide Ceiling Object");
                    go.SetActive(false);
                    EditorUtility.SetDirty(go);
                    hiddenCount++;
                }
            }
            else
            {
                if (!go.activeSelf)
                {
                    Undo.RecordObject(go, "Restore Ceiling Object");
                    go.SetActive(true);
                    EditorUtility.SetDirty(go);
                    restoredCount++;
                }
            }
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorPrefs.SetBool(PrefKeyCeilingHidden, hide);

        // Đánh dấu bẩn scene và lưu lại
        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.isLoaded)
        {
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveOpenScenes();
        }

        string msg = hide
            ? $"Đã ẩn thành công {hiddenCount} vật thể trần nhà và cửa sổ trần!\nBảo tàng hiện đã thông thoáng để bạn chụp ảnh top-down toàn cảnh.\n(Dùng menu 'Bảo Tàng > Hiện Lại...' để bật lại bất cứ lúc nào)."
            : $"Đã khôi phục hiển thị thành công {restoredCount} vật thể trần nhà và cửa sổ trần!";

        Debug.Log($"[MuseumTopDown] {msg}");

        if (showDialog)
        {
            EditorUtility.DisplayDialog("Top-Down Bảo Tàng", msg, "Đồng Ý");
        }
    }

    /// <summary>
    /// Tìm kiếm toàn bộ Cube trần nhà, Cửa sổ trần và các chi tiết trần trong Scene
    /// </summary>
    private static List<GameObject> FindCeilingAndSkylightObjects(bool includeCeilingLights)
    {
        var result = new HashSet<GameObject>();
        var allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var go in allGameObjects)
        {
            // Bỏ qua object không thuộc Scene đang mở
            if (go.hideFlags != HideFlags.None || !go.scene.isLoaded) continue;

            string name = go.name;
            string lowerName = name.ToLower();

            // 1. CỬA SỔ TRẦN (Window6_04L)
            if (name.Contains("Window6_04L") || lowerName.Contains("skylight") || lowerName.Contains("cuaso_tran"))
            {
                GameObject rootWindow = GetTopWindowContainer(go);
                result.Add(rootWindow);
                continue;
            }

            // 2. CÁC ĐỐI TƯỢNG CÓ TÊN RÕ RÀNG VỀ TRẦN NHÀ
            if (lowerName.Contains("ceiling") || lowerName.Contains("tran_nha") || lowerName.Contains("mái") || lowerName.Contains("roof"))
            {
                result.Add(go);
                continue;
            }

            // 3. ĐÈN TREO TRẦN & ỐNG THÔNG GIÓ (NẾU ĐƯỢC CHỌN)
            if (includeCeilingLights)
            {
                if (name.StartsWith("Light_09", StringComparison.OrdinalIgnoreCase) || 
                    name.StartsWith("Vent", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(go);
                    continue;
                }
            }

            // 4. CÁC KHỐI CUBE TRẦN NHÀ
            if (name.StartsWith("Cube", StringComparison.OrdinalIgnoreCase))
            {
                // Bỏ qua nếu thuộc danh sách các Cube được bảo vệ
                if (ProtectedCubeNames.Contains(name)) continue;

                // Bỏ qua nếu có script thuyết minh
                if (go.GetComponent("CubeExhibitionTrigger") != null) continue;

                var rend = go.GetComponent<Renderer>();
                if (rend != null)
                {
                    // A. Kiểm tra qua Vật Liệu: Nếu dùng vật liệu trần
                    bool isCeilingMat = false;
                    foreach (var mat in rend.sharedMaterials)
                    {
                        if (mat != null)
                        {
                            string matName = mat.name.ToLower();
                            if (matName.Contains("ceiling") || matName.Contains("tran") || matName.Contains("museumceiling"))
                            {
                                isCeilingMat = true;
                                break;
                            }
                        }
                    }

                    if (isCeilingMat)
                    {
                        result.Add(go);
                        continue;
                    }

                    // B. Kiểm tra qua Vị Trí & Kích Thước (Tọa độ không gian 3D):
                    Bounds b = rend.bounds;

                    // Nếu đối tượng nằm ở tầng trần nhà (Y >= 3.0m hoặc tâm Y >= 3.4m)
                    // Và KHÔNG PHẢI là bức tường dọc từ sàn lên trần (tường thì b.min.y <= 0.6m)
                    bool isAtCeilingLevel = b.min.y >= 3.0f || b.center.y >= 3.4f;
                    bool isVerticalWall = b.min.y <= 0.6f && b.size.y >= 2.0f;
                    bool isFloor = b.max.y <= 0.5f;

                    if (isAtCeilingLevel && !isVerticalWall && !isFloor)
                    {
                        result.Add(go);
                        continue;
                    }
                }
                else
                {
                    // Không có renderer: kiểm tra Transform position
                    if (go.transform.position.y >= 3.4f)
                    {
                        result.Add(go);
                        continue;
                    }
                }
            }
        }

        return result.ToList();
    }

    private static GameObject GetTopWindowContainer(GameObject go)
    {
        Transform curr = go.transform;
        while (curr.parent != null && (curr.parent.name.Contains("Window6_04L") || curr.parent.name.ToLower().Contains("skylight")))
        {
            curr = curr.parent;
        }
        return curr.gameObject;
    }

    /// <summary>
    /// Tự động căn chỉnh Camera trong SceneView nhìn thẳng từ trên xuống trung tâm bảo tàng
    /// </summary>
    public static void AlignSceneViewTopDown(bool orthographic)
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view == null && SceneView.sceneViews.Count > 0)
        {
            view = (SceneView)SceneView.sceneViews[0];
        }

        if (view == null)
        {
            EditorUtility.DisplayDialog("Thông báo", "Vui lòng mở tab Scene trong Unity để căn góc nhìn!", "Đồng Ý");
            return;
        }

        Bounds bounds = CalculateVisibleBounds();
        Vector3 center = bounds.center;
        center.y = 0; // Chiếu về mặt phẳng sàn

        float size = Mathf.Max(bounds.size.x, bounds.size.z);
        if (size < 10f) size = 50f;

        view.orthographic = orthographic;
        view.rotation = Quaternion.Euler(90f, 0f, 0f); // Nhìn thẳng 90 độ xuống sàn
        view.pivot = center;
        view.size = size * 0.65f;
        view.Repaint();

        Debug.Log($"[MuseumTopDown] Đã căn góc SceneView Top-Down tại tâm: {center}, Độ rộng khung hình: {view.size}, Chế độ Trực giao: {orthographic}");
    }

    /// <summary>
    /// Chụp ảnh Top-Down độ phân giải cao và lưu trực tiếp thành file PNG trong dự án
    /// </summary>
    public static void CaptureTopDownScreenshot(int width, int height)
    {
        Bounds bounds = CalculateVisibleBounds();
        Vector3 center = bounds.center;
        float maxDim = Mathf.Max(bounds.size.x, bounds.size.z);
        if (maxDim < 10f) maxDim = 50f;

        // Tạo Camera tạm thời nhìn từ trên cao xuống
        GameObject camGo = new GameObject("Temp_TopDown_Camera");
        Camera cam = camGo.AddComponent<Camera>();

        try
        {
            cam.orthographic = true;
            cam.orthographicSize = maxDim * 0.55f;
            cam.transform.position = new Vector3(center.x, 60f, center.z);
            cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 120f;
            cam.clearFlags = CameraClearFlags.Color;
            cam.backgroundColor = new Color(0.12f, 0.13f, 0.15f, 1f); // Nền xám đen sang trọng cho bản vẽ mặt bằng

            RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShot.Apply();

            byte[] bytes = screenShot.EncodeToPNG();
            string filename = $"Assets/Anh_TopDown_BaoTang_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            File.WriteAllBytes(filename, bytes);

            cam.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt);
            DestroyImmediate(screenShot);

            AssetDatabase.Refresh();
            var savedObj = AssetDatabase.LoadAssetAtPath<Texture2D>(filename);
            if (savedObj != null)
            {
                EditorGUIUtility.PingObject(savedObj);
            }

            Debug.Log($"[MuseumTopDown] Đã chụp và lưu thành công ảnh Top-Down tại: {filename}");
            EditorUtility.DisplayDialog("Chụp Top-Down Thành Công!",
                $"Ảnh chụp top-down độ phân giải cao ({width}x{height}) đã được lưu tại:\n{filename}\n\nBạn có thể xem ảnh ngay trong tab Project của Unity!",
                "Xem Ảnh");
        }
        finally
        {
            DestroyImmediate(camGo);
        }
    }

    /// <summary>
    /// Tính toán kích thước bao quát tổng thể của toàn bộ bảo tàng đang hiển thị
    /// </summary>
    private static Bounds CalculateVisibleBounds()
    {
        var renderers = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Bounds b = new Bounds();
        bool hasInit = false;

        foreach (var r in renderers)
        {
            if (r.gameObject.name.Contains("XR") || r.gameObject.name.Contains("Camera")) continue;

            if (!hasInit)
            {
                b = r.bounds;
                hasInit = true;
            }
            else
            {
                b.Encapsulate(r.bounds);
            }
        }

        if (!hasInit)
        {
            b = new Bounds(new Vector3(30f, 0f, 25f), new Vector3(50f, 10f, 50f));
        }

        return b;
    }
}

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.XR.CoreUtils;

/// <summary>
/// Công cụ thiết lập nhanh XR Hands (Hand Tracking) cho dự án Bảo tàng Tây Nguyên.
/// </summary>
public static class MuseumXRHandSetup
{
    private const string HandsRigPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Prefabs/XR Origin Hands (XR Rig).prefab";
    private const string OldRigPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";

    [MenuItem("Tools/Bảo Tàng/XR Hands/1. Cài đặt XR Origin Hands vào Scene", false, 1)]
    public static void SetupXRHandRigInScene()
    {
        // 1. Tải Prefab XR Origin Hands
        GameObject handRigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(HandsRigPrefabPath);
        if (handRigPrefab == null)
        {
            EditorUtility.DisplayDialog("Lỗi", $"Không tìm thấy prefab tại đường dẫn:\n{HandsRigPrefabPath}", "OK");
            return;
        }

        // 2. Tìm vị trí người chơi hiện tại (từ Rig cũ hoặc FPSController hoặc Camera)
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        XROrigin existingOrigin = Object.FindObjectOfType<XROrigin>();
        if (existingOrigin != null)
        {
            spawnPos = existingOrigin.transform.position;
            spawnRot = existingOrigin.transform.rotation;

            // Xóa Rig cũ để thay bằng Hand Rig
            Undo.DestroyObjectImmediate(existingOrigin.gameObject);
            Debug.Log("[XR Hands Setup] Đã gỡ bỏ XR Origin cũ để nâng cấp lên XR Origin Hands.");
        }
        else
        {
            FPSController fps = Object.FindObjectOfType<FPSController>();
            if (fps != null)
            {
                spawnPos = fps.transform.position;
                spawnRot = fps.transform.rotation;
            }
            else if (Camera.main != null)
            {
                spawnPos = Camera.main.transform.position;
                spawnRot = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            }
        }

        // 3. Khởi tạo XR Origin Hands (XR Rig) vào Scene
        GameObject newRig = (GameObject)PrefabUtility.InstantiatePrefab(handRigPrefab);
        newRig.name = "XR Origin Hands (XR Rig)";
        newRig.transform.position = spawnPos;
        newRig.transform.rotation = spawnRot;
        Undo.RegisterCreatedObjectUndo(newRig, "Instantiate XR Origin Hands");

        // Đảm bảo tag MainCamera
        XROrigin newOrigin = newRig.GetComponent<XROrigin>();
        if (newOrigin != null && newOrigin.Camera != null)
        {
            newOrigin.Camera.tag = "MainCamera";
        }

        // Đánh dấu Scene đã thay đổi để lưu
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = newRig;

        EditorUtility.DisplayDialog("Thành Công",
            "Đã cài đặt 'XR Origin Hands (XR Rig)' vào Scene thành công!\n\n" +
            "Các tính năng bàn tay ảo đã được tích hợp:\n" +
            "✔ Left Hand & Right Hand Visualizer (Khớp xương 3D)\n" +
            "✔ Poke Interactor (Chạm ngón tay vào bảng/nút)\n" +
            "✔ Hand Ray Interactor (Tia bắn từ ngón tay/lòng bàn tay)\n" +
            "✔ Pinch Gesture (Chụm ngón trỏ và ngón cái để tương tác)\n\n" +
            "LƯU Ý KHI BẤM PLAY:\n" +
            "1. Nếu cắm kính Quest (Quest Link): Buông tay cầm xuống bàn, đưa 2 bàn tay ra trước kính là tay ảo sẽ xuất hiện ngay!\n" +
            "2. Nếu test trên PC không có kính: Bàn tay sẽ ẩn vì không có camera nhận diện tay thật (xem hướng dẫn ở Menu số 2).",
            "OK");

        Debug.Log("[XR Hands Setup] Hoàn tất cài đặt XR Origin Hands.");
    }

    [MenuItem("Tools/Bảo Tàng/XR Hands/2. Hướng dẫn Test Hand Tracking khi Play", false, 2)]
    public static void ShowTestingGuide()
    {
        EditorUtility.DisplayDialog("Cách Test Bàn Tay Ảo Khi Bấm Play",
            "1. CẮM KÍNH META QUEST (Khuyên dùng):\n" +
            " - Cắm cáp Link hoặc bật Air Link trên kính Quest.\n" +
            " - Trong kính, bật: Cài đặt -> Movement tracking -> Hand tracking.\n" +
            " - Đặt 2 tay cầm xuống bàn, đưa 2 bàn tay thật lên.\n" +
            " - Bấm PLAY trong Unity Editor -> Bàn tay 3D sẽ xuất hiện và cử động theo tay thật của bạn!\n\n" +
            "2. TEST TRÊN MÁY TÍNH (Không cần kính):\n" +
            " - Bật XR Device Simulator trong Project Settings -> XR Plug-in Management -> XR Interaction Toolkit.\n" +
            " - Bấm Play, dùng phím Space hoặc chuột để điều khiển tay ảo mô phỏng.",
            "Đã Hiểu");
    }
}

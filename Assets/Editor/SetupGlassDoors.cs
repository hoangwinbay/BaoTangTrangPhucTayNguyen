using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu Editor tự động cấu hình và thiết lập hệ thống Cửa Kính Tự Động (Automatic Sliding Doors)
/// cho 2 cánh cửa kính glass_partition_41_003 và glass_partition_41_004 trong bảo tàng:
/// 1. Tự động tìm kiếm 2 cánh cửa trong hierarchy.
/// 2. Gỡ bỏ cờ BatchingStatic và OccluderStatic để 2 cánh cửa có thể di chuyển/animation tại runtime.
/// 3. Gắn BoxCollider vào mỗi cánh cửa để chặn vật lý khi cửa đóng (người chơi không đi xuyên qua kính).
/// 4. Gắn và cấu hình script AutomaticSlidingDoor điều khiển trượt êm ái sang 2 bên.
/// 5. Tạo vùng cảm biến Trigger Collider đa chiều phát hiện người chơi đi đến gần.
/// 6. Thiết lập âm thanh trượt 3D (Spatial Audio).
/// 7. Tự động lưu Scene an toàn.
/// </summary>
public static class SetupGlassDoors
{
    private const string MenuPath = "Bảo Tàng/Cấu Hình Cửa Kính Tự Động (003 & 004)";

    [InitializeOnLoadMethod]
    private static void AutoInit()
    {
        EditorApplication.delayCall += () =>
        {
            GameObject d3 = GameObject.Find("glass_partition_41_003");
            GameObject d4 = GameObject.Find("glass_partition_41_004");

            if (d3 != null && d4 != null)
            {
                // Kiểm tra nếu cửa chưa có controller hoặc còn mang cờ BatchingStatic
                var staticFlags3 = GameObjectUtility.GetStaticEditorFlags(d3);
                var staticFlags4 = GameObjectUtility.GetStaticEditorFlags(d4);
                bool hasBatching = (staticFlags3 & StaticEditorFlags.BatchingStatic) != 0 ||
                                  (staticFlags4 & StaticEditorFlags.BatchingStatic) != 0;

                var controller = Object.FindObjectOfType<AutomaticSlidingDoor>();
                if (controller == null || hasBatching || d3.GetComponent<Collider>() == null)
                {
                    Setup(showDialog: false);
                }
            }
        };
    }

    [MenuItem(MenuPath, false, 12)]
    public static void MenuSetup()
    {
        Setup(showDialog: true);
    }

    public static void Setup(bool showDialog = true)
    {
        GameObject doorGo3 = GameObject.Find("glass_partition_41_003");
        GameObject doorGo4 = GameObject.Find("glass_partition_41_004");

        if (doorGo3 == null || doorGo4 == null)
        {
            string err = "Không tìm thấy đủ 2 cánh cửa kính 'glass_partition_41_003' và 'glass_partition_41_004' trong Scene hiện tại!";
            Debug.LogWarning($"[SetupGlassDoors] {err}");
            if (showDialog)
            {
                EditorUtility.DisplayDialog("Lỗi Thiết Lập Cửa Kính", err, "Đóng");
            }
            return;
        }

        Undo.RegisterCompleteObjectUndo(doorGo3, "Setup Glass Door 003");
        Undo.RegisterCompleteObjectUndo(doorGo4, "Setup Glass Door 004");

        // 1. GỠ BỎ STATIC BATCHING & OCCLUDER STATIC ĐỂ CỬA CÓ THỂ DI CHUYỂN RUNTIME
        // (Nếu bật BatchingStatic, Unity sẽ gộp mesh vào Static Batch khiến cửa không thể di chuyển)
        StaticEditorFlags targetFlags = StaticEditorFlags.OccludeeStatic; // Chỉ giữ OccludeeStatic
        GameObjectUtility.SetStaticEditorFlags(doorGo3, targetFlags);
        GameObjectUtility.SetStaticEditorFlags(doorGo4, targetFlags);
        EditorUtility.SetDirty(doorGo3);
        EditorUtility.SetDirty(doorGo4);

        // 2. GẮN BOXCOLLIDER VÀO MỖI CÁNH CỬA (Ngăn người chơi đi xuyên kính khi cửa đóng)
        BoxCollider col3 = doorGo3.GetComponent<BoxCollider>();
        if (col3 == null) col3 = Undo.AddComponent<BoxCollider>(doorGo3);
        col3.isTrigger = false;
        EditorUtility.SetDirty(col3);

        BoxCollider col4 = doorGo4.GetComponent<BoxCollider>();
        if (col4 == null) col4 = Undo.AddComponent<BoxCollider>(doorGo4);
        col4.isTrigger = false;
        EditorUtility.SetDirty(col4);

        // 3. TẠO HOẶC LẤY ĐỐI TƯỢNG QUẢN LÝ CỬA
        GameObject parentGo = doorGo3.transform.parent ? doorGo3.transform.parent.gameObject : doorGo3;
        AutomaticSlidingDoor doorCtrl = parentGo.GetComponent<AutomaticSlidingDoor>();
        if (doorCtrl == null)
        {
            doorCtrl = Undo.AddComponent<AutomaticSlidingDoor>(parentGo);
        }

        doorCtrl.doorLeft = doorGo4.transform;   // Cánh trái (trượt về -Z)
        doorCtrl.doorRight = doorGo3.transform;  // Cánh phải (trượt về +Z)
        doorCtrl.doorwayCenter = new Vector3(19.98f, 1.37f, 38.20f);
        doorCtrl.slideDistance = 0.78f;
        doorCtrl.openSpeed = 2.4f;
        doorCtrl.closeSpeed = 1.8f;
        doorCtrl.autoCloseDelay = 1.2f;
        doorCtrl.triggerDistance = 2.8f;
        doorCtrl.enableSoundEffects = true;
        doorCtrl.InitializeDoors(forceRecalculate: true);
        EditorUtility.SetDirty(doorCtrl);

        // 4. TẠO VÙNG CẢM BIẾN TRIGGER COLLIDER (SENSOR ZONE)
        Transform existingSensor = parentGo.transform.Find("DoorSensorTrigger");
        GameObject sensorGo;
        if (existingSensor == null)
        {
            sensorGo = new GameObject("DoorSensorTrigger");
            Undo.RegisterCreatedObjectUndo(sensorGo, "Create DoorSensorTrigger");
            sensorGo.transform.SetParent(parentGo.transform, false);
        }
        else
        {
            sensorGo = existingSensor.gameObject;
        }

        // Đặt tọa độ cảm biến tại đúng tâm lối đi
        sensorGo.transform.position = doorCtrl.doorwayCenter;
        sensorGo.transform.rotation = Quaternion.identity;

        BoxCollider sensorCol = sensorGo.GetComponent<BoxCollider>();
        if (sensorCol == null) sensorCol = Undo.AddComponent<BoxCollider>(sensorGo);
        sensorCol.isTrigger = true;
        // Kích thước vùng cảm biến: Dài 5.5m (dọc lối đi trục X), Cao 2.6m (trục Y), Rộng 2.5m (ngang cửa trục Z)
        sensorCol.size = new Vector3(5.5f, 2.6f, 2.5f);
        sensorCol.center = Vector3.zero;
        EditorUtility.SetDirty(sensorCol);

        DoorSensorTrigger triggerScript = sensorGo.GetComponent<DoorSensorTrigger>();
        if (triggerScript == null) triggerScript = Undo.AddComponent<DoorSensorTrigger>(sensorGo);
        triggerScript.slidingDoor = doorCtrl;
        EditorUtility.SetDirty(triggerScript);
        EditorUtility.SetDirty(sensorGo);

        // 5. LƯU SCENE AN TOÀN
        if (!EditorApplication.isPlaying)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                EditorSceneManager.SaveScene(activeScene);
            }
        }

        string result = "Đã cấu hình thành công Hệ thống Cửa Kính Tự Động 2 Cánh:\n\n" +
                        "1. Cánh cửa kính bên trái: 'glass_partition_41_004' (Trượt sang trái -Z khi mở)\n" +
                        "2. Cánh cửa kính bên phải: 'glass_partition_41_003' (Trượt sang phải +Z khi mở)\n" +
                        "3. Khoảng cách trượt mỗi bên: 0.78m (Lối đi rộng 1.56m thông thoáng)\n" +
                        "4. Đã gỡ bỏ BatchingStatic để 2 cánh cửa di chuyển mượt mà tại Runtime\n" +
                        "5. Đã gắn BoxCollider trên 2 cánh cửa (Chặn vật lý khi cửa đóng)\n" +
                        "6. Vùng cảm biến phát hiện người chơi (Bán kính 2.8m, hộp cảm biến 5.5m x 2.6m)\n" +
                        "7. Tự động đóng êm sau 1.2 giây khi người chơi đã đi qua hoặc rời đi\n" +
                        "8. Tích hợp âm thanh trượt cửa 3D không gian";

        Debug.Log($"[Bảo Tàng - Cửa Kính] {result}");
        if (showDialog)
        {
            EditorUtility.DisplayDialog("Bảo Tàng - Cửa Kính Tự Động Hoàn Tất", result, "Tuyệt vời");
        }
    }
}

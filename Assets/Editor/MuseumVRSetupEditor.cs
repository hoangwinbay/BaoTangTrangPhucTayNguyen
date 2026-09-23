using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

namespace MuseumVR.Editor
{
    public static class MuseumVRSetupEditor
    {
        private const string StarterRigPath = "Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";
        private const string InputActionsPath = "Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/XRI Default Input Actions.inputactions";
        private const string ResourcesDir = "Assets/Resources/VR";
        private const string ResourcePrefabPath = "Assets/Resources/VR/XROriginRig.prefab";

        [InitializeOnLoadMethod]
        private static void OnEditorCompile()
        {
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
                var activeScene = EditorSceneManager.GetActiveScene();
                if (!activeScene.isLoaded) return;

                // Nếu trong scene chưa có XR Origin, tự động cấu hình ngay
                if (Object.FindObjectOfType<XROrigin>() == null)
                {
                    SetupVROriginInActiveScene(false);
                }
            };
        }

        [MenuItem("Bảo Tàng VR/⚡ Thiết lập Người chơi VR (XR Origin vào Scene)", false, 2)]
        public static void SetupVROriginFromMenu()
        {
            SetupVROriginInActiveScene(true);
        }

        public static void SetupVROriginInActiveScene(bool showDialog = true)
        {
            EnsureResourcesPrefab();

            // 1. Đảm bảo có XRInteractionManager
            XRInteractionManager interactionManager = Object.FindObjectOfType<XRInteractionManager>();
            if (interactionManager == null)
            {
                GameObject managerObj = new GameObject("XR Interaction Manager");
                interactionManager = managerObj.AddComponent<XRInteractionManager>();
                Undo.RegisterCreatedObjectUndo(managerObj, "Create XR Interaction Manager");
                Debug.Log("[VR Setup] Đã tạo mới XR Interaction Manager.");
            }

            // 2. Tìm điểm xuất phát từ FPSController máy tính
            FPSController pcPlayer = Object.FindObjectOfType<FPSController>();
            Vector3 spawnPosition = Vector3.zero;
            Quaternion spawnRotation = Quaternion.identity;

            if (pcPlayer != null)
            {
                spawnPosition = pcPlayer.transform.position;
                spawnRotation = pcPlayer.transform.rotation;
            }
            else
            {
                Camera existingCam = Camera.main;
                if (existingCam != null)
                {
                    spawnPosition = existingCam.transform.position;
                    spawnRotation = Quaternion.Euler(0, existingCam.transform.eulerAngles.y, 0);
                }
            }

            // 3. Tìm hoặc tạo XR Origin
            XROrigin xrOrigin = Object.FindObjectOfType<XROrigin>();
            GameObject vrRigInstance = null;

            if (xrOrigin == null)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(StarterRigPath);
                if (prefab == null)
                {
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ResourcePrefabPath);
                }

                if (prefab != null)
                {
                    vrRigInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    vrRigInstance.name = "XR Origin (XR Rig)";
                    vrRigInstance.transform.position = spawnPosition;
                    vrRigInstance.transform.rotation = spawnRotation;
                    xrOrigin = vrRigInstance.GetComponent<XROrigin>();
                    Undo.RegisterCreatedObjectUndo(vrRigInstance, "Instantiate XR Origin Rig");
                    Debug.Log($"[VR Setup] Đã khởi tạo XR Origin (XR Rig) tại tọa độ: {spawnPosition}");
                }
                else
                {
                    Debug.LogError("[VR Setup] Không tìm thấy XR Origin prefab!");
                    EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy prefab XR Origin (XR Rig). Vui lòng kiểm tra package XR Interaction Toolkit!", "OK");
                    return;
                }
            }
            else
            {
                vrRigInstance = xrOrigin.gameObject;
            }

            // 4. Cấu hình Di chuyển (Joystick TRÁI theo hướng quay đầu của kính)
            var moveProvider = vrRigInstance.GetComponentInChildren<ContinuousMoveProvider>();
            if (moveProvider != null)
            {
                moveProvider.moveSpeed = 2.5f;
                moveProvider.enableFly = false;
                moveProvider.useGravity = true;

                if (xrOrigin.Camera != null)
                {
                    moveProvider.forwardSource = xrOrigin.Camera.transform;
                }
                EditorUtility.SetDirty(moveProvider);
            }

            // 5. Cấu hình Xoay góc nhìn: TẮT joystick phải, người chơi chỉ cần QUAY ĐẦU tự nhiên 100%
            var snapTurn = vrRigInstance.GetComponentInChildren<SnapTurnProvider>();
            if (snapTurn != null)
            {
                snapTurn.enabled = false;
                EditorUtility.SetDirty(snapTurn);
            }

            var continuousTurn = vrRigInstance.GetComponentInChildren<ContinuousTurnProvider>();
            if (continuousTurn != null)
            {
                continuousTurn.enabled = false;
                EditorUtility.SetDirty(continuousTurn);
            }

            Transform turnChild = vrRigInstance.transform.Find("Locomotion/Turn");
            if (turnChild != null)
            {
                turnChild.gameObject.SetActive(false);
                EditorUtility.SetDirty(turnChild.gameObject);
            }

            // 6. Gán InputActionManager với bộ asset input actions mặc định
            var inputActionMgr = vrRigInstance.GetComponent<InputActionManager>();
            if (inputActionMgr == null)
            {
                inputActionMgr = vrRigInstance.AddComponent<InputActionManager>();
            }

            var inputActionAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
            if (inputActionAsset != null)
            {
                if (inputActionMgr.actionAssets == null)
                {
                    inputActionMgr.actionAssets = new System.Collections.Generic.List<InputActionAsset>();
                }
                if (!inputActionMgr.actionAssets.Contains(inputActionAsset))
                {
                    inputActionMgr.actionAssets.Add(inputActionAsset);
                }
                EditorUtility.SetDirty(inputActionMgr);
            }

            // 7. Gắn XRSimpleInteractable cho 12 bệ hiện vật
            MuseumVRRigManager.ConfigureExhibitionCubeInteractables();

            // 8. Đảm bảo MuseumPopupManager và MuseumVRRigManager có mặt trong scene
            if (Object.FindObjectOfType<MuseumPopupManager>() == null)
            {
                GameObject mgr = new GameObject("MuseumPopupManager");
                mgr.AddComponent<MuseumPopupManager>();
                Undo.RegisterCreatedObjectUndo(mgr, "Create MuseumPopupManager");
            }

            if (Object.FindObjectOfType<MuseumVRRigManager>() == null)
            {
                GameObject vrMgr = new GameObject("MuseumVRRigManager");
                vrMgr.AddComponent<MuseumVRRigManager>();
                Undo.RegisterCreatedObjectUndo(vrMgr, "Create MuseumVRRigManager");
            }

            // Lưu scene hiện tại
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();

            Debug.Log("[VR Setup] Thiết lập XR Origin cho Meta Quest 2 hoàn tất thành công!");
            if (showDialog)
            {
                EditorUtility.DisplayDialog("Thành công", 
                    "Đã cấu hình thành công hệ thống XR Origin cho Meta Quest 2:\n\n" +
                    "✓ Cần Joystick TRÁI: Đi bộ trong bảo tàng theo hướng nhìn của đầu.\n" +
                    "✓ Nhìn quanh: Quay đầu tự nhiên 100% (6DoF Head Tracking), đã tắt xoay nhân tạo ở joystick phải.\n" +
                    "✓ Tay cầm: Laser Raycast và Cò trỏ (Index Trigger) / nút A để mở bảng thuyết minh hiện vật.\n" +
                    "✓ Popup: Hiển thị 3D World Space sắc nét lơ lửng trước mắt trong VR.", "OK");
            }
        }

        public static void EnsureResourcesPrefab()
        {
            if (!Directory.Exists(ResourcesDir))
            {
                Directory.CreateDirectory(ResourcesDir);
                AssetDatabase.Refresh();
            }

            if (!File.Exists(ResourcePrefabPath))
            {
                if (File.Exists(StarterRigPath))
                {
                    AssetDatabase.CopyAsset(StarterRigPath, ResourcePrefabPath);
                    AssetDatabase.Refresh();
                    Debug.Log($"[VR Setup] Đã sao chép XR Origin prefab vào {ResourcePrefabPath}");
                }
            }
        }
    }
}

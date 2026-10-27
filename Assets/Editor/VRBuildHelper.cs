using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

namespace MuseumVR.Editor
{
    public class VRBuildHelper : EditorWindow
    {
        private Vector2 scrollPosition;
        private string apkOutputPath = "Builds/BaoTangTayNguyen_VR.apk";

        [MenuItem("Bảo Tàng VR/Cài đặt & Build APK cho Kính VR", false, 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<VRBuildHelper>("VR Build Helper");
            window.minSize = new Vector2(520, 600);
            window.Show();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(10);
            GUILayout.Label("CÔNG CỤ HỖ TRỢ BUILD APK CHO KÍNH VR (META QUEST)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Công cụ này giúp kiểm tra, tối ưu hóa cấu hình và xuất file APK chạy độc lập trên kính VR (Meta Quest 2, Quest 3, Quest Pro, Quest 3S).", MessageType.Info);

            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            if (HasNonAscii(projectRoot))
            {
                EditorGUILayout.HelpBox(
                    $"❌ LỖI ĐƯỜNG DẪN CÓ DẤU TIẾNG VIỆT:\n\"{projectRoot}\"\n\n" +
                    "Android Build Tools (Gradle, AAPT2, NDK) chặn tuyệt đối đường dẫn có dấu.\n\n" +
                    "ĐÃ TẠO SẴN THƯ MỤC LIÊN KẾT: \"D:\\DuAnBaoTang\"\n\n" +
                    "👉 CÁCH XỬ LÝ (MẤT 30 GIÂY):\n" +
                    "1. Lưu scene (Ctrl + S) và TẮT cửa sổ Unity Editor này.\n" +
                    "2. Mở Unity Hub -> Nhấn nút 'Open' (hoặc 'Add project from disk').\n" +
                    "3. Chọn thư mục 'D:\\DuAnBaoTang'.\n" +
                    "4. Mở lại menu 'Bảo Tàng VR' và bấm Build APK là thành công 100%!",
                    MessageType.Error);
            }

            GUILayout.Space(10);

            DrawAndroidModuleCheck();

            GUILayout.Space(10);

            DrawSettingsCheck();

            GUILayout.Space(15);

            DrawActionButtons();

            GUILayout.Space(15);

            DrawVRGuide();

            EditorGUILayout.EndScrollView();
        }

        private void DrawAndroidModuleCheck()
        {
            GUILayout.Label("1. Trạng thái Unity Android Module", EditorStyles.boldLabel);

            bool isAndroidSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android);

            if (isAndroidSupported)
            {
                EditorGUILayout.HelpBox("✓ Unity đã có Android Build Support! Đã sẵn sàng để build APK.", MessageType.None);
            }
            else
            {
                EditorGUILayout.HelpBox("⚠ Unity 6 hiện TỰA CÀI module 'Android Build Support'.\n\nCách cài đặt nhanh:\n1. Mở Unity Hub.\n2. Vào tab 'Installs' -> Tìm phiên bản Unity 6000.5.9f1.\n3. Nhấn biểu tượng Bánh răng (⚙) -> Chọn 'Add modules'.\n4. Tích chọn: 'Android Build Support' (kèm Android SDK & NDK Tools và OpenJDK).\n5. Bấm Continue để cài đặt.", MessageType.Error);

                if (GUILayout.Button("Mở hướng dẫn tải Android Module trong Unity Hub", GUILayout.Height(30)))
                {
                    Application.OpenURL("https://docs.unity3d.com/hub/manual/AddModules.html");
                }
            }
        }

        private void DrawSettingsCheck()
        {
            GUILayout.Label("2. Kiểm tra thông số VR Meta Quest", EditorStyles.boldLabel);

            bool isColorSpaceLinear = PlayerSettings.colorSpace == ColorSpace.Linear;
            bool isIL2CPP = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) == ScriptingImplementation.IL2CPP;
            bool isARM64 = (PlayerSettings.Android.targetArchitectures & AndroidArchitecture.ARM64) != 0;
            bool isMinSdkValid = (int)PlayerSettings.Android.minSdkVersion <= 32;

            DrawCheckItem("Hệ màu Linear Color Space", isColorSpaceLinear);
            DrawCheckItem("Scripting Backend: IL2CPP (Bắt buộc cho Quest)", isIL2CPP);
            DrawCheckItem("Target Architecture: ARM64 (Bắt buộc cho Quest)", isARM64);
            DrawCheckItem("Min SDK <= Android 12L / API 32 (Khuyến nghị API 29)", isMinSdkValid,
                $"Hiện tại: {(int)PlayerSettings.Android.minSdkVersion}");
            DrawCheckItem("Tạo file APK (Tắt Build App Bundle / AAB)", !EditorUserBuildSettings.buildAppBundle);

            GUILayout.Space(5);
            if (GUILayout.Button("⚡ Tự động chuẩn hóa toàn bộ cấu hình VR Quest", GUILayout.Height(32)))
            {
                ApplyQuestOptimizations();
            }

            GUILayout.Space(10);
            GUILayout.Label("3. Cấu hình Người chơi VR (Meta Quest 2)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Tự động cấu hình XR Origin: Joystick TRÁI di chuyển theo hướng nhìn của đầu, QUAY ĐẦU tự nhiên 100% để nhìn quanh, và tay cầm tương tác bệ hiện vật.", MessageType.Info);
            if (GUILayout.Button("⚡ Thiết lập XR Origin & Tay Cầm VR vào Scene", GUILayout.Height(36)))
            {
                MuseumVRSetupEditor.SetupVROriginInActiveScene();
            }
        }

        private void DrawCheckItem(string title, bool passed, string detail = null)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = passed ? Color.green : Color.red;
            GUILayout.Label(passed ? "✓" : "✗", GUILayout.Width(20));
            GUI.contentColor = Color.white;
            GUILayout.Label(title);
            if (!string.IsNullOrEmpty(detail))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label($"({detail})", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawActionButtons()
        {
            GUILayout.Label("4. Xuất file APK", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            apkOutputPath = EditorGUILayout.TextField("Đường dẫn lưu APK:", apkOutputPath);
            if (GUILayout.Button("Chọn...", GUILayout.Width(60)))
            {
                string path = EditorUtility.SaveFilePanel("Chọn nơi lưu APK", "Builds", "BaoTangTayNguyen_VR.apk", "apk");
                if (!string.IsNullOrEmpty(path))
                {
                    apkOutputPath = path;
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            bool isAndroidSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android);

            GUI.enabled = isAndroidSupported;
            if (GUILayout.Button("🚀 TIẾN HÀNH BUILD FILE APK CHO KÍNH VR", GUILayout.Height(45)))
            {
                EditorApplication.delayCall += () => BuildQuestApk(false);
            }

            GUILayout.Space(5);

            if (GUILayout.Button("📲 Build & Cài đặt trực tiếp lên kính VR (Cần cắm cáp USB)", GUILayout.Height(35)))
            {
                EditorApplication.delayCall += () => BuildQuestApk(true);
            }
            GUI.enabled = true;

            GUILayout.Space(5);
            if (GUILayout.Button("📂 Mở thư mục chứa file APK (Builds)", GUILayout.Height(28)))
            {
                string outDir = Path.GetFullPath("Builds");
                if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);
                EditorUtility.RevealInFinder(outDir);
            }

            if (!isAndroidSupported)
            {
                EditorGUILayout.HelpBox("Nút Build bị vô hiệu hóa vì Unity chưa cài Android Build Support. Vui lòng cài qua Unity Hub theo hướng dẫn ở mục 1.", MessageType.Warning);
            }
        }

        private void DrawVRGuide()
        {
            GUILayout.Label("4. Hướng dẫn cài đặt APK vào kính VR (Meta Quest)", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Cách 1: Dùng SideQuest (Đơn giản nhất):\n" +
                "1. Tải phần mềm SideQuest trên PC (Advanced Installer).\n" +
                "2. Cắm kính Quest vào máy tính qua cáp Type-C.\n" +
                "3. Trong kính Quest: Chọn 'Cho phép gỡ lỗi USB' (Allow USB Debugging).\n" +
                "4. Kéo thả file .apk vừa build vào SideQuest để cài tự động.\n\n" +
                "Cách 2: Chạy trực tiếp từ PC (Quest Link / Air Link):\n" +
                "Nếu không muốn cài APK vào kính, bạn chỉ cần cắm cáp Quest Link hoặc bật Air Link, sau đó nhấn nút Play trong Unity Editor để trải nghiệm trực tiếp với đồ họa của máy tính!",
                MessageType.Info);
        }

        public static void ApplyQuestOptimizations()
        {
            PlayerSettings.colorSpace = ColorSpace.Linear;

            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34;

            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

            if (string.IsNullOrEmpty(PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android)) ||
                PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android) == "com.DefaultCompany.VRTemplate")
            {
                PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.BaoTangTayNguyen.VRApp");
            }

            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan, GraphicsDeviceType.OpenGLES3 });

            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[VRBuildHelper] Đã áp dụng các thiết lập tối ưu cho kính Meta Quest thành công!");
            EditorUtility.DisplayDialog("Thành công", "Đã chuẩn hóa toàn bộ cấu hình Project cho kính VR Meta Quest!", "OK");
        }

        public static void BuildQuestApk(bool autoRun)
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
            {
                EditorUtility.DisplayDialog("Lỗi", "Chưa cài đặt Android Build Support trong Unity. Vui lòng mở Unity Hub -> Installs -> Add Modules để cài đặt.", "OK");
                return;
            }

            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            if (HasNonAscii(projectRoot))
            {
                EditorUtility.DisplayDialog("Lỗi đường dẫn chứa tiếng Việt (Non-ASCII)",
                    $"Đường dẫn thư mục dự án hiện tại là:\n{projectRoot}\n\n" +
                    "Android Tools (Gradle, AAPT2, NDK) không thể biên dịch trên đường dẫn có dấu tiếng Việt.\n\n" +
                    "GIẢI PHÁP ĐÃ TẠO SẴN:\n" +
                    "1. Lưu project (Ctrl + S) và ĐÓNG Unity Editor.\n" +
                    "2. Mở Unity Hub -> Bấm 'Open' -> Chọn thư mục 'D:\\DuAnBaoTang' (thư mục liên kết không dấu đã tạo sẵn).\n" +
                    "3. Mở lại menu 'Bảo Tàng VR' và bấm Build APK.", "Tôi đã hiểu");
                return;
            }

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                bool switchTarget = EditorUtility.DisplayDialog("Chuyển Platform", "Project cần chuyển Build Target sang Android trước khi build. Quá trình này có thể mất vài phút. Tiếp tục?", "Đồng ý", "Hủy");
                if (!switchTarget) return;

                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }

            var scenes = EditorBuildSettings.scenes;
            int enabledScenesCount = 0;
            foreach (var scene in scenes)
            {
                if (scene.enabled) enabledScenesCount++;
            }

            if (enabledScenesCount == 0)
            {
                EditorUtility.DisplayDialog("Lỗi", "Không có Scene nào được chọn trong Build Settings! Vui lòng vào File -> Build Settings để thêm Scene.", "OK");
                return;
            }

            string outDirectory = Path.GetFullPath("Builds");
            if (!Directory.Exists(outDirectory))
            {
                Directory.CreateDirectory(outDirectory);
            }

            string apkPath = Path.Combine(outDirectory, "BaoTangTayNguyen_VR.apk");

            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettingsScene.GetActiveSceneList(scenes),
                locationPathName = apkPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = autoRun ? (BuildOptions.AutoRunPlayer | BuildOptions.Development) : BuildOptions.None
            };

            Debug.Log($"[VRBuildHelper] Bắt đầu build APK tại: {apkPath}");
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[VRBuildHelper] Build thành công! Kích thước: {summary.totalSize / (1024 * 1024):F2} MB. Thời gian: {summary.totalTime.TotalSeconds:F0}s");
                EditorUtility.RevealInFinder(apkPath);
                EditorUtility.DisplayDialog("Thành công", $"Đã xuất file APK thành công!\n\nĐường dẫn: {apkPath}\nKích thước: {summary.totalSize / (1024 * 1024):F2} MB", "Mở thư mục");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError($"[VRBuildHelper] Build thất bại với {summary.totalErrors} lỗi. Vui lòng kiểm tra tab Console.");
                EditorUtility.DisplayDialog("Thất bại", $"Quá trình build gặp lỗi ({summary.totalErrors} lỗi). Hãy mở tab Console trong Unity để xem chi tiết.", "OK");
            }
        }

        private static bool HasNonAscii(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            foreach (char c in text)
            {
                if (c > 127) return true;
            }
            return false;
        }
    }
}

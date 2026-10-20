#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class NpcAnimationSetup
{
    public const string WALK_FBX_PATH = "Assets/Animation/Walking.fbx";
    public const string IDLE_FBX_PATH = "Assets/Animation/Standing Idle.fbx";
    public const string CTRL_PATH     = "Assets/Animation/NPC_AnimatorController.controller";
    public const string RES_CTRL_PATH = "Assets/Resources/NPC_AnimatorController.controller";

    static NpcAnimationSetup()
    {
        EditorApplication.delayCall += () =>
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Run(showDialog: false);
            }
        };
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnPlaymodeSceneLoaded()
    {
        RuntimeEnsureNpcAnimation();
    }

    [MenuItem("Tools/NPC/Setup Animation Walking + Standing Idle", false, 1)]
    [MenuItem("Tools/Bao Tang/NPC/🎬 Setup Animation Walking & Standing Idle (Hoàn Chỉnh)", false, 1)]
    public static void MenuRun()
    {
        Run(showDialog: true);
    }

    public static void Run(bool showDialog = false)
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("=== BÁO CÁO THIẾT LẬP ANIMATION NPC BẢO TÀNG ===");
        report.AppendLine($"Thời gian: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        // 1. Cấu hình ModelImporter cho 2 file FBX từ take mixamo.com thực tế
        ConfigureFbxWithRealTake(WALK_FBX_PATH, "Walking", report);
        ConfigureFbxWithRealTake(IDLE_FBX_PATH, "Standing Idle", report);

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        // 2. Load Animation Clips thực tế
        AnimationClip walkClip = GetFirstClip(WALK_FBX_PATH);
        AnimationClip idleClip = GetFirstClip(IDLE_FBX_PATH);

        report.AppendLine($"Walk Clip: {(walkClip != null ? walkClip.name + $" (humanoid={walkClip.isHumanMotion}, loop={walkClip.isLooping}, length={walkClip.length:F2}s)" : "NULL!")}");
        report.AppendLine($"Idle Clip: {(idleClip != null ? idleClip.name + $" (humanoid={idleClip.isHumanMotion}, loop={idleClip.isLooping}, length={idleClip.length:F2}s)" : "NULL!")}");

        // 3. Tạo / Cập nhật Controller
        var ctrl = SetupController(CTRL_PATH, idleClip, walkClip, report);

        // Copy vào Resources để Runtime luôn sẵn sàng nạp
        try
        {
            if (!Directory.Exists("Assets/Resources")) Directory.CreateDirectory("Assets/Resources");
            AssetDatabase.CopyAsset(CTRL_PATH, RES_CTRL_PATH);
            AssetDatabase.SaveAssets();
        }
        catch { }

        // 4. Tìm Avatar tương thích
        Avatar avatar = FindBestAvatar(report);

        // 5. Cấu hình toàn bộ NPC trong Scene
        int count = 0;
        var scene = SceneManager.GetActiveScene();
        report.AppendLine($"\nActive Scene: '{scene.name}' (isLoaded={scene.isLoaded})");

        var agents = Object.FindObjectsByType<NavMeshAgent>(FindObjectsInactive.Include);
        report.AppendLine($"Tìm thấy {agents.Length} NavMeshAgent trong scene:");

        foreach (var agent in agents)
        {
            var go = agent.gameObject;
            if (go.GetComponentInChildren<SkinnedMeshRenderer>() == null)
            {
                report.AppendLine($"  - Bỏ qua '{go.name}' (không có SkinnedMeshRenderer)");
                continue;
            }

            // Gỡ bỏ script hỏng nếu có
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

            // Tìm Animator (ưu tiên trên root hoặc children có bones)
            var animators = go.GetComponentsInChildren<Animator>(true);
            Animator anim = null;
            if (animators.Length > 0)
            {
                // Chọn animator có avatar hợp lệ hoặc animator đầu tiên
                foreach (var a in animators)
                {
                    if (a.avatar != null && a.avatar.isValid) { anim = a; break; }
                }
                if (anim == null) anim = animators[0];
            }
            else
            {
                anim = go.AddComponent<Animator>();
            }

            anim.enabled = true;
            anim.applyRootMotion = false;
            anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            if (ctrl != null)
            {
                anim.runtimeAnimatorController = ctrl;
            }

            if ((anim.avatar == null || !anim.avatar.isValid) && avatar != null)
            {
                anim.avatar = avatar;
            }

            // Gán NpcAnimController trên root NPC
            var animCtrl = go.GetComponent<NpcAnimController>();
            if (animCtrl == null)
            {
                animCtrl = go.AddComponent<NpcAnimController>();
            }

            animCtrl.idleStateName = "Idle";
            animCtrl.walkStateName = "Walk";
            animCtrl.useParameter = true;
            animCtrl.parameterName = "Speed";
            animCtrl.isBoolParameter = false;
            animCtrl.Init();

            // Cấu hình NavMeshAgent
            agent.speed = Mathf.Clamp(agent.speed, 0.9f, 1.4f);
            agent.stoppingDistance = 0.35f;

            EditorUtility.SetDirty(go);
            if (anim.gameObject != go) EditorUtility.SetDirty(anim.gameObject);

            count++;
            report.AppendLine($"  + Đã cấu hình: '{go.name}' (Animator on '{anim.gameObject.name}', Avatar='{(anim.avatar != null ? anim.avatar.name : "null")}', Controller='{(anim.runtimeAnimatorController != null ? anim.runtimeAnimatorController.name : "null")}')");
        }

        // 6. Lưu Scene và Asset
        if (scene.isLoaded)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            report.AppendLine("\nĐã lưu Scene thành công.");
        }
        AssetDatabase.SaveAssets();

        File.WriteAllText("Assets/NpcAnimationExecutionReport.txt", report.ToString(), Encoding.UTF8);
        Debug.Log($"[NpcAnimationSetup] Hoàn tất cấu hình animation cho {count} NPC. Chi tiết tại Assets/NpcAnimationExecutionReport.txt");

        if (showDialog)
        {
            EditorUtility.DisplayDialog(
                "✅ Hoàn Tất Gán Animation NPC",
                $"Đã gán thành công Animation Walking & Standing Idle cho {count} NPC!\n\n" +
                $"- Walk Clip: {(walkClip != null ? walkClip.name + $" ({walkClip.length:F2}s)" : "Chưa tải")}\n" +
                $"- Idle Clip: {(idleClip != null ? idleClip.name + $" ({idleClip.length:F2}s)" : "Chưa tải")}\n" +
                $"- Controller: NPC_AnimatorController.controller\n" +
                $"- Scene đã được lưu tự động.\n\n" +
                $"Nhấn Play ▶ để kiểm tra hoạt cảnh!",
                "OK"
            );
        }
    }

    private static void RuntimeEnsureNpcAnimation()
    {
        var ctrl = Resources.Load<RuntimeAnimatorController>("NPC_AnimatorController");
        if (ctrl == null)
            ctrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(CTRL_PATH);

        Avatar avatar = null;
        string[] searchPaths = {
            "Assets/npc_casual_set_00/Mesh/npc_hmn_01m.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_hmn_01f.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_casual_set_00.fbx"
        };
        foreach (var p in searchPaths)
        {
            foreach (var a in AssetDatabase.LoadAllAssetsAtPath(p))
            {
                if (a is Avatar av && av.isValid)
                {
                    avatar = av;
                    break;
                }
            }
            if (avatar != null) break;
        }

        foreach (var agent in Object.FindObjectsByType<NavMeshAgent>(FindObjectsInactive.Exclude))
        {
            var anim = agent.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.enabled = true;
                anim.applyRootMotion = false;
                anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                if (ctrl != null && anim.runtimeAnimatorController == null)
                    anim.runtimeAnimatorController = ctrl;
                if (avatar != null && (anim.avatar == null || !anim.avatar.isValid))
                    anim.avatar = avatar;
            }

            var animCtrl = agent.GetComponent<NpcAnimController>();
            if (animCtrl != null)
            {
                animCtrl.Init();
            }
        }
        Debug.Log("[NpcAnimationSetup] RuntimeEnsureNpcAnimation đã chạy khi vào Play mode.");
    }

    private static void ConfigureFbxWithRealTake(string path, string desiredClipName, StringBuilder report)
    {
        var importer = AssetImporter.GetAtPath(path) as ModelImporter;
        if (importer == null)
        {
            report.AppendLine($"[ERROR] Không tìm thấy ModelImporter: {path}");
            return;
        }

        importer.animationType = ModelImporterAnimationType.Human;
        importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

        // Reset clipAnimations để đọc trực tiếp take gốc từ FBX
        importer.clipAnimations = new ModelImporterClipAnimation[0];
        importer.SaveAndReimport();

        var defClips = importer.defaultClipAnimations;
        if (defClips != null && defClips.Length > 0)
        {
            var clip = defClips[0];
            clip.name = desiredClipName;
            clip.loopTime = true;
            clip.loopPose = true;
            clip.lockRootRotation = true;
            clip.lockRootHeightY = true;
            clip.lockRootPositionXZ = true;
            clip.keepOriginalOrientation = true;
            clip.keepOriginalPositionY = true;
            clip.keepOriginalPositionXZ = true;

            importer.clipAnimations = new ModelImporterClipAnimation[] { clip };
            importer.SaveAndReimport();
            report.AppendLine($"[FBX] Đã tạo clip '{desiredClipName}' từ take '{clip.takeName}' (frames {clip.firstFrame} đến {clip.lastFrame}): {path}");
        }
        else
        {
            report.AppendLine($"[FBX] Không tìm thấy defaultClipAnimations trong: {path}");
        }
    }

    private static AnimationClip GetFirstClip(string fbxPath)
    {
        foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(fbxPath))
        {
            if (asset is AnimationClip clip && !clip.name.StartsWith("__"))
            {
                return clip;
            }
        }
        return null;
    }

    private static RuntimeAnimatorController SetupController(string path, AnimationClip idleClip, AnimationClip walkClip, StringBuilder report)
    {
        AssetDatabase.DeleteAsset(path);
        var ctrl = AnimatorController.CreateAnimatorControllerAtPath(path);

        ctrl.AddParameter("Speed", AnimatorControllerParameterType.Float);

        var sm = ctrl.layers[0].stateMachine;

        var idleState = sm.AddState("Idle");
        if (idleClip != null) idleState.motion = idleClip;

        var walkState = sm.AddState("Walk");
        if (walkClip != null) walkState.motion = walkClip;

        sm.defaultState = idleState;

        // Transition: Idle -> Walk (Speed > 0.1)
        var t1 = idleState.AddTransition(walkState);
        t1.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
        t1.duration = 0.2f;
        t1.hasExitTime = false;

        // Transition: Walk -> Idle (Speed < 0.05)
        var t2 = walkState.AddTransition(idleState);
        t2.AddCondition(AnimatorConditionMode.Less, 0.05f, "Speed");
        t2.duration = 0.25f;
        t2.hasExitTime = false;

        AssetDatabase.SaveAssets();
        report.AppendLine($"[Controller] Đã tạo Animator Controller: {path}");
        return ctrl;
    }

    private static Avatar FindBestAvatar(StringBuilder report)
    {
        string[] searchPaths = {
            "Assets/npc_casual_set_00/Mesh/npc_hmn_01m.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_hmn_01f.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_casual_set_00.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_csl_00_character_01m.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_csl_00_character_01f.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_csl_00_character_02m.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_csl_00_character_02f.fbx"
        };

        foreach (var path in searchPaths)
        {
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (asset is Avatar av && av.isValid)
                {
                    report.AppendLine($"[Avatar] Đã tìm thấy Avatar: '{av.name}' (isHuman={av.isHuman}) từ '{path}'");
                    return av;
                }
            }
        }

        report.AppendLine("[WARNING] Không tìm thấy Avatar nào trong các file mesh!");
        return null;
    }
}
#endif

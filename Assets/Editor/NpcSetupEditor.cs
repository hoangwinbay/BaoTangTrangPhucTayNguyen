#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public static class NpcSetupEditor
{
    private struct NpcSpawnInfo
    {
        public int id;
        public string prefabPath;
        public Vector3 position;
        public string description;
    }

    private static readonly NpcSpawnInfo[] NewNpcConfig = new NpcSpawnInfo[]
    {
        new NpcSpawnInfo {
            id = 4,
            prefabPath = "Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_01f_01.prefab",
            position = new Vector3(21.5f, 0.25f, 18.5f),
            description = "Khu vực trang phục Brâu & Xơ Đăng"
        },
        new NpcSpawnInfo {
            id = 5,
            prefabPath = "Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_01m_01.prefab",
            position = new Vector3(27.5f, 0.25f, 17.5f),
            description = "Khu vực trang phục Mạ & M'Nông"
        },
        new NpcSpawnInfo {
            id = 6,
            prefabPath = "Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_01f_02.prefab",
            position = new Vector3(35.0f, 0.25f, 18.0f),
            description = "Khu vực trang phục Rơ Măm & Cơ Ho"
        },
        new NpcSpawnInfo {
            id = 7,
            prefabPath = "Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_02m_01.prefab",
            position = new Vector3(36.0f, 0.25f, 26.0f),
            description = "Khu vực trang phục Giẻ Triêng & Ba Na"
        },
        new NpcSpawnInfo {
            id = 8,
            prefabPath = "Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_02f_01.prefab",
            position = new Vector3(25.0f, 0.25f, 28.0f),
            description = "Gian sảnh chính trung tâm bảo tàng"
        },
        new NpcSpawnInfo {
            id = 9,
            prefabPath = "Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_01m_02.prefab",
            position = new Vector3(32.0f, 0.25f, 33.0f),
            description = "Cánh đông bắc gần khu trưng bày Chư-Ru"
        },
        new NpcSpawnInfo {
            id = 10,
            prefabPath = "Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_01f_03.prefab",
            position = new Vector3(20.5f, 0.25f, 34.0f),
            description = "Hành lang tây bắc bảo tàng"
        }
    };

    [InitializeOnLoadMethod]
    private static void AutoInit()
    {
        EditorApplication.delayCall += () =>
        {
            SetupAllNpcs(showDialog: false);
        };
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoEnsureOnPlay()
    {
        SetupAllNpcs(showDialog: false);
    }

    [MenuItem("Bao Tang VR/Cap Nhat 10 NPC Tham Quan (Fix Animation)", false, 5)]
    [MenuItem("Tools/Bao Tang/NPC/Setup 10 NPC + Fix Animation", false, 10)]
    public static void MenuSetupNpcs()
    {
        SetupAllNpcs(showDialog: true);
    }

    public static void SetupAllNpcs(bool showDialog)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.isLoaded) return;

        Debug.Log("[NPC Setup] Bắt đầu kiểm tra và cấu hình hệ thống NPC...");
        NpcAnimationSetup.Run(showDialog: false);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=================================================");
        sb.AppendLine("     BÁO CÁO THIẾT LẬP HỆ THỐNG 10 NPC BẢO TÀNG  ");
        sb.AppendLine("=================================================");
        sb.AppendLine($"Thời gian: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        List<GameObject> existingNpcs = new List<GameObject>();
        for (int i = 1; i <= 10; i++)
        {
            GameObject npc = GameObject.Find($"NPC_{i}");
            if (npc != null)
            {
                existingNpcs.Add(npc);
                ConfigureNpcComponents(npc, i);
                sb.AppendLine($"• Đã cấu hình NPC_{i} tại vị trí: {npc.transform.position}");
            }
        }

        int createdCount = 0;
        foreach (var info in NewNpcConfig)
        {
            string npcName = $"NPC_{info.id}";
            GameObject npcObj = GameObject.Find(npcName);
            if (npcObj == null)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(info.prefabPath);
                if (prefab == null)
                {
                    string fallbackPath = "Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_01m_01.prefab";
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fallbackPath);
                }

                if (prefab != null)
                {
                    npcObj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    if (npcObj != null)
                    {
                        npcObj.name = npcName;
                        Undo.RegisterCreatedObjectUndo(npcObj, $"Create {npcName}");

                        Vector3 spawnPos = info.position;
                        if (NavMesh.SamplePosition(info.position, out var hit, 3.5f, NavMesh.AllAreas))
                        {
                            spawnPos = hit.position;
                        }

                        npcObj.transform.position = spawnPos;
                        npcObj.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                        npcObj.transform.localScale = Vector3.one * 0.887f;

                        ConfigureNpcComponents(npcObj, info.id);
                        createdCount++;
                        sb.AppendLine($"• [MỚI] Đã tạo {npcName} ({info.description}) tại: {spawnPos}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[NPC Setup] Không thể tải prefab: {info.prefabPath}");
                }
            }
        }

        var allNpcs = Object.FindObjectsByType<NpcThamQuan>(FindObjectsInactive.Exclude);
        sb.AppendLine("-------------------------------------------------");
        sb.AppendLine($"Tổng số NPC hoạt động trong bảo tàng: {allNpcs.Length} (Mới tạo: {createdCount})");
        Debug.Log($"[NPC Setup] Hoàn tất: Tổng {allNpcs.Length} NPC trong scene (Mới tạo: {createdCount}).");

        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);
            AssetDatabase.SaveAssets();
            sb.AppendLine("Trạng thái Scene: Đã lưu tự động thành công.");
        }

        try
        {
            string repDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../Docs/Reports"));
            if (!Directory.Exists(repDir)) Directory.CreateDirectory(repDir);
            File.WriteAllText(Path.Combine(repDir, "NpcSetupReport.txt"), sb.ToString(), Encoding.UTF8);
        }
        catch { }

        if (showDialog)
        {
            string msg = $"Đã thiết lập thành công hệ thống 10 NPC tham quan bảo tàng!\n\n" +
                         $"- Tổng số NPC: {allNpcs.Length}\n" +
                         $"- NPC mới tạo: {createdCount}\n" +
                         $"- Đã khắc phục lỗi không di chuyển khi Play.\n" +
                         $"- Các NPC được rải đều tại các gian trưng bày và hành lang.\n" +
                         $"- Scene đã được lưu tự động.";
            EditorUtility.DisplayDialog("Bảo Tàng - Hệ Thống NPC Tham Quan", msg, "Đồng ý");
        }
    }

    private static void ConfigureNpcComponents(GameObject npc, int id)
    {
        var agent = npc.GetComponent<NavMeshAgent>();
        if (agent == null) agent = Undo.AddComponent<NavMeshAgent>(npc);
        agent.speed = 1.15f + Random.Range(-0.1f, 0.15f);
        agent.angularSpeed = 220f;
        agent.acceleration = 3.5f;
        agent.stoppingDistance = 0.35f;
        agent.radius = 0.32f;
        agent.height = 1.75f;
        agent.baseOffset = 0f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(20, 80);
        agent.autoBraking = true;

        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(npc.transform.position, out var hit, 5.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }

        var thamQuan = npc.GetComponent<NpcThamQuan>();
        if (thamQuan == null) thamQuan = Undo.AddComponent<NpcThamQuan>(npc);
        thamQuan.tocDo = agent.speed;
        thamQuan.khoangCachDung = 0.35f;
        thamQuan.minThoiGianXem = 4.0f;
        thamQuan.maxThoiGianXem = 8.0f;

        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(npc);

        var animCtrl = npc.GetComponent<NpcAnimController>();
        if (animCtrl == null) animCtrl = Undo.AddComponent<NpcAnimController>(npc);
        animCtrl.idleStateName = "Idle";
        animCtrl.walkStateName = "Walk";
        animCtrl.useParameter = true;
        animCtrl.parameterName = "Speed";
        animCtrl.isBoolParameter = false;
        animCtrl.Init();

        const string CTRL_PATH = "Assets/Animation/NPC_AnimatorController.controller";
        var runtimeCtrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(CTRL_PATH);

        Avatar avatar = null;
        string[] avatarSearchPaths = {
            "Assets/npc_casual_set_00/Mesh/npc_csl_00_character_01m.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_csl_00_character_01f.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_csl_00_character_02m.fbx",
            "Assets/npc_casual_set_00/Mesh/npc_csl_00_character_02f.fbx",
        };
        foreach (var avatarPath in avatarSearchPaths)
        {
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(avatarPath))
            {
                if (asset is Avatar av) { avatar = av; break; }
            }
            if (avatar != null) break;
        }

        var animators = npc.GetComponentsInChildren<Animator>(true);
        foreach (var anim in animators)
        {
            if (anim == null) continue;
            anim.enabled = true;
            anim.applyRootMotion = false;
            anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            if (runtimeCtrl != null)
                anim.runtimeAnimatorController = runtimeCtrl;
            if (avatar != null && anim.avatar == null)
                anim.avatar = avatar;
        }

        EditorUtility.SetDirty(npc);
    }
}
#endif

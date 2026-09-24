using UnityEngine;

/// <summary>
/// Tự động tạo và quản lý khung viền đèn LED phát sáng (Light Border / Illuminated Frame)
/// cho tranh, bảng thông tin hoặc khối Cube trưng bày trong bảo tàng.
/// Tự động co giãn theo kích thước của vật thể và cung cấp ánh sáng hắt sáng viền cao cấp.
/// </summary>
[ExecuteAlways]
[SelectionBase]
public class CubeLightBorder : MonoBehaviour
{
    [Header("=== CÀI ĐẶT MÀU SẮC & ÁNH SÁNG LED ===")]
    [Tooltip("Màu sắc của viền đèn LED (Mặc định: Vàng ấm sang trọng)")]
    public Color ledColor = new Color(1.0f, 0.90f, 0.72f, 1.0f);

    [Tooltip("Cường độ phát sáng HDR của viền đèn")]
    [Range(1.0f, 10.0f)]
    public float emissionIntensity = 3.5f;

    [Header("=== KÍCH THƯỚC KHUNG VIỀN ===")]
    [Tooltip("Độ dày của thanh viền LED (mét)")]
    [Range(0.01f, 0.20f)]
    public float borderThickness = 0.035f;

    [Tooltip("Độ lồi thanh viền ra phía trước mặt tranh (mét)")]
    [Range(0.005f, 0.15f)]
    public float borderDepth = 0.025f;

    [Tooltip("Độ thụt/nhô viền ra mép ngoài")]
    [Range(-0.05f, 0.10f)]
    public float borderPadding = 0.01f;

    [Header("=== ĐÈN HẮT SÁNG NỘI THẤT (BACKLIGHT) ===")]
    [Tooltip("Bật đèn hắt sáng mềm xung quanh bức tranh")]
    public bool enableBacklight = true;

    [Tooltip("Cường độ của đèn hắt sáng")]
    [Range(0.2f, 6.0f)]
    public float lightIntensity = 2.0f;

    [Tooltip("Phạm vi lan tỏa của đèn (mét)")]
    [Range(1.0f, 8.0f)]
    public float lightRange = 3.5f;

    [Header("=== VẬT LIỆU VIỀN ĐÈN ===")]
    public Material customBorderMaterial;

    private const string BorderHolderName = "_LED_LightBorder_Holder";
    private Material runtimeMaterial;

    void OnEnable()
    {
        BuildOrUpdateBorder();
    }

    void Start()
    {
        BuildOrUpdateBorder();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // QUAN TRỌNG: Không gọi DestroyImmediate/CreatePrimitive trực tiếp trong OnValidate.
        // Dùng delayCall để hoãn sang frame tiếp theo (an toàn, không gây crash).
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return; // Component có thể bị xoá trước khi callback chạy
            BuildOrUpdateBorder();
        };
    }
#endif

    /// <summary>
    /// Tạo mới hoặc làm mới khung viền đèn ôm sát mép Cube
    /// </summary>
    [ContextMenu("Tạo / Làm Mới Viền Đèn LED")]
    public void BuildOrUpdateBorder()
    {
        Transform existingHolder = transform.Find(BorderHolderName);
        GameObject holderGo;

        if (existingHolder == null)
        {
            holderGo = new GameObject(BorderHolderName);
            holderGo.transform.SetParent(transform, false);
            holderGo.transform.localPosition = Vector3.zero;
            holderGo.transform.localRotation = Quaternion.identity;
            holderGo.transform.localScale = Vector3.one;
        }
        else
        {
            holderGo = existingHolder.gameObject;
        }

        // Đảm bảo có vật liệu phát sáng LED
        EnsureMaterial();

        // Xác định mặt phẳng chính của Cube (trục mỏng nhất là chiều sâu)
        Vector3 lossy = transform.lossyScale;
        Vector3 localScale = transform.localScale;

        // Đối với mesh Cube mặc định 1x1x1 trong local space
        float absX = Mathf.Abs(lossy.x);
        float absY = Mathf.Abs(lossy.y);
        float absZ = Mathf.Abs(lossy.z);

        // Tìm trục mỏng nhất (trục chiều sâu / chiều pháp tuyến của mặt tranh)
        int normalAxis = 2; // 0=X, 1=Y, 2=Z (mặc định đa số tranh/bảng dùng Z mỏng)
        if (absX < absY && absX < absZ) normalAxis = 0;
        else if (absY < absX && absY < absZ) normalAxis = 1;

        // Xóa các thanh viền cũ để vẽ lại đúng kích thước
        for (int i = holderGo.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = holderGo.transform.GetChild(i);
            if (child.name.StartsWith("BorderStrip_") || child.name == "BorderPointLight")
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) DestroyImmediate(child.gameObject);
                else Destroy(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }
        }

        // Tạo 4 thanh viền LED dựa trên trục mặt phẳng
        CreateBorderSegments(holderGo, normalAxis, localScale);

        // Tạo hoặc cập nhật đèn Point Light hắt sáng
        if (enableBacklight)
        {
            GameObject lightObj = new GameObject("BorderPointLight");
            lightObj.transform.SetParent(holderGo.transform, false);

            Vector3 lightPos = Vector3.zero;
            if (normalAxis == 2) lightPos.z = 0.5f + (borderDepth * 1.5f / Mathf.Max(0.001f, localScale.z));
            else if (normalAxis == 0) lightPos.x = 0.5f + (borderDepth * 1.5f / Mathf.Max(0.001f, localScale.x));
            else lightPos.y = 0.5f + (borderDepth * 1.5f / Mathf.Max(0.001f, localScale.y));

            lightObj.transform.localPosition = lightPos;

            Light ptLight = lightObj.AddComponent<Light>();
            ptLight.type = LightType.Point;
            ptLight.color = ledColor;
            ptLight.intensity = lightIntensity;
            ptLight.range = lightRange;
            ptLight.shadows = LightShadows.None;
        }
    }

    private void EnsureMaterial()
    {
        if (customBorderMaterial != null)
        {
            runtimeMaterial = customBorderMaterial;
            Color finalHdrColor = ledColor * Mathf.Pow(2f, emissionIntensity - 1f);
            if (runtimeMaterial.HasProperty("_EmissionColor"))
            {
                runtimeMaterial.SetColor("_EmissionColor", finalHdrColor);
            }
            if (runtimeMaterial.HasProperty("_BaseColor"))
            {
                runtimeMaterial.SetColor("_BaseColor", ledColor);
            }
            return;
        }

#if UNITY_EDITOR
        customBorderMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Cube92_LED_Border.mat");
        if (customBorderMaterial != null)
        {
            EnsureMaterial();
            return;
        }
#endif

        if (runtimeMaterial == null)
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");
            runtimeMaterial = new Material(litShader);
            runtimeMaterial.name = "Runtime_LED_BorderMat";
            runtimeMaterial.EnableKeyword("_EMISSION");
        }

        Color hdrColor = ledColor * Mathf.Pow(2f, emissionIntensity - 1f);
        if (runtimeMaterial.HasProperty("_EmissionColor"))
        {
            runtimeMaterial.SetColor("_EmissionColor", hdrColor);
        }
        if (runtimeMaterial.HasProperty("_BaseColor"))
        {
            runtimeMaterial.SetColor("_BaseColor", ledColor);
        }
    }

    private void CreateBorderSegments(GameObject holder, int normalAxis, Vector3 parentScale)
    {
        // Tính độ dày chuyển đổi sang Local Scale của Cube
        float thX = borderThickness / Mathf.Max(0.01f, parentScale.x);
        float thY = borderThickness / Mathf.Max(0.01f, parentScale.y);
        float thZ = borderThickness / Mathf.Max(0.01f, parentScale.z);

        float depX = borderDepth / Mathf.Max(0.01f, parentScale.x);
        float depY = borderDepth / Mathf.Max(0.01f, parentScale.y);
        float depZ = borderDepth / Mathf.Max(0.01f, parentScale.z);

        float padX = borderPadding / Mathf.Max(0.01f, parentScale.x);
        float padY = borderPadding / Mathf.Max(0.01f, parentScale.y);
        float padZ = borderPadding / Mathf.Max(0.01f, parentScale.z);

        // Trường hợp thông dụng nhất: Mặt tranh nằm trên trục Z (trục Z mỏng)
        if (normalAxis == 2)
        {
            float zFront = 0.5f + depZ * 0.5f;
            float w = 1.0f + (padX * 2f);
            float h = 1.0f + (padY * 2f);

            // 1. Thanh viền Trên (Top)
            CreateCubeSegment(holder, "BorderStrip_Top", 
                new Vector3(0f, 0.5f + padY - (thY * 0.5f), zFront), 
                new Vector3(w, thY, depZ));

            // 2. Thanh viền Dưới (Bottom)
            CreateCubeSegment(holder, "BorderStrip_Bottom", 
                new Vector3(0f, -0.5f - padY + (thY * 0.5f), zFront), 
                new Vector3(w, thY, depZ));

            // 3. Thanh viền Trái (Left)
            CreateCubeSegment(holder, "BorderStrip_Left", 
                new Vector3(-0.5f - padX + (thX * 0.5f), 0f, zFront), 
                new Vector3(thX, h, depZ));

            // 4. Thanh viền Phải (Right)
            CreateCubeSegment(holder, "BorderStrip_Right", 
                new Vector3(0.5f + padX - (thX * 0.5f), 0f, zFront), 
                new Vector3(thX, h, depZ));
        }
        else if (normalAxis == 0) // Trục X mỏng
        {
            float xFront = 0.5f + depX * 0.5f;
            float h = 1.0f + (padY * 2f);
            float d = 1.0f + (padZ * 2f);

            CreateCubeSegment(holder, "BorderStrip_Top", new Vector3(xFront, 0.5f + padY - (thY * 0.5f), 0f), new Vector3(depX, thY, d));
            CreateCubeSegment(holder, "BorderStrip_Bottom", new Vector3(xFront, -0.5f - padY + (thY * 0.5f), 0f), new Vector3(depX, thY, d));
            CreateCubeSegment(holder, "BorderStrip_Left", new Vector3(xFront, 0f, -0.5f - padZ + (thZ * 0.5f)), new Vector3(depX, h, thZ));
            CreateCubeSegment(holder, "BorderStrip_Right", new Vector3(xFront, 0f, 0.5f + padZ - (thZ * 0.5f)), new Vector3(depX, h, thZ));
        }
        else // Trục Y mỏng
        {
            float yFront = 0.5f + depY * 0.5f;
            float w = 1.0f + (padX * 2f);
            float d = 1.0f + (padZ * 2f);

            CreateCubeSegment(holder, "BorderStrip_Top", new Vector3(0f, yFront, 0.5f + padZ - (thZ * 0.5f)), new Vector3(w, depY, thZ));
            CreateCubeSegment(holder, "BorderStrip_Bottom", new Vector3(0f, yFront, -0.5f - padZ + (thZ * 0.5f)), new Vector3(w, depY, thZ));
            CreateCubeSegment(holder, "BorderStrip_Left", new Vector3(-0.5f - padX + (thX * 0.5f), yFront, 0f), new Vector3(thX, depY, d));
            CreateCubeSegment(holder, "BorderStrip_Right", new Vector3(0.5f + padX - (thX * 0.5f), yFront, 0f), new Vector3(thX, depY, d));
        }
    }

    private void CreateCubeSegment(GameObject parent, string name, Vector3 localPos, Vector3 localScale)
    {
        GameObject strip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        strip.name = name;
        strip.transform.SetParent(parent.transform, false);
        strip.transform.localPosition = localPos;
        strip.transform.localRotation = Quaternion.identity;
        strip.transform.localScale = localScale;

        // Xóa Collider của thanh viền đèn để không ảnh hưởng raycast hay va chạm vật lý
        Collider col = strip.GetComponent<Collider>();
        if (col != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(col);
            else Destroy(col);
#else
            Destroy(col);
#endif
        }

        // Gán vật liệu phát sáng
        Renderer rend = strip.GetComponent<Renderer>();
        if (rend != null && runtimeMaterial != null)
        {
            rend.sharedMaterial = runtimeMaterial;
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            rend.receiveShadows = false;
        }
    }
}

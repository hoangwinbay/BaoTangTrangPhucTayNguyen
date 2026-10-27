# BÁO CÁO PHÂN LOẠI 2: TẤT CẢ MATERIAL TRONG TOÀN BỘ DỰ ÁN
*Thời gian xuất dữ liệu: 2026-10-27 13:03:43*  

> **Mô tả:** Báo cáo kiểm kê toàn diện tất cả các tệp Material (`.mat`) có trong dự án Unity, phân loại theo mục đích sử dụng (Kiến trúc bảo tàng, Đá vân mây, Đèn LED, Nội thất, Nhân vật NPC, Kính phản chiếu...). Báo cáo chỉ rõ Shader sử dụng, các Texture map đi kèm (Albedo, Normal, Metallic, Emission) và các đối tượng đang sử dụng trong Scene.

---
## 1. THỐNG KÊ TỔNG QUAN
- **Tổng số Material trong dự án:** 411 materials
- **Số Material đang được gán trực tiếp trong Scene:** 159 materials
- **Số Material chưa dùng trực tiếp (trong prefab/kho tài nguyên):** 252 materials

### Phân bố theo danh mục:
- **Bệ Gỗ Bóng & Ghế Sắt Gỗ:** 3 materials (Trong Scene: 2)
- **Ghế Văn Phòng (Office Chair Pack):** 13 materials (Trong Scene: 4)
- **Giao Diện & XR Toolkit:** 47 materials (Trong Scene: 0)
- **Hệ Thống Đèn LED Chiếu Sáng (PolyKebap):** 23 materials (Trong Scene: 7)
- **Khách Tham Quan Bảo Tàng (NPC):** 96 materials (Trong Scene: 46)
- **Mặt Sàn Bảo Tàng (Floor):** 1 materials (Trong Scene: 0)
- **Mô Hình Dân Tộc & Trưng Bày:** 66 materials (Trong Scene: 59)
- **Nội Thất & Kiến Trúc Bảo Tàng (LowPolyInterior2):** 24 materials (Trong Scene: 6)
- **Phông Chữ TextMesh Pro:** 9 materials (Trong Scene: 0)
- **Thùng Rác LowPoly (Waste Bin):** 4 materials (Trong Scene: 1)
- **Vách Kính & Màn Hình TV (Glass Partition):** 4 materials (Trong Scene: 2)
- **Vật Liệu Cốt Lõi (Đá Vân Mây, LED Border):** 90 materials (Trong Scene: 5)
- **Vật Liệu Khác:** 31 materials (Trong Scene: 27)

---
## 2. BẢNG TRA CỨU CHI TIẾT TẤT CẢ MATERIAL

### 📂 Phân nhóm: Bệ Gỗ Bóng & Ghế Sắt Gỗ (3 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **BeGoBong_Go** | `Universal Render Pipeline/Lit` | `Assets/3d/BeGoBong/BeGoBong_Go.mat` | `Be_Go` | - | - | Chưa dùng |
| 2 | **GheSatGo_Go** | `Universal Render Pipeline/Lit` | `Assets/3d/GheSatGo/GheSatGo_Go.mat` | - | - | - | **Có (3 vật thể):** GheSatGo (1), GheSatGo (2)... |
| 3 | **GheSatGo_Sat** | `Universal Render Pipeline/Lit` | `Assets/3d/GheSatGo/GheSatGo_Sat.mat` | - | - | - | **Có (3 vật thể):** GheSatGo (1), GheSatGo (2)... |

### 📂 Phân nhóm: Ghế Văn Phòng (Office Chair Pack) (13 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **ambientCG_Fabric023_1K-JPG** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair3/Materials/ambientCG_Fabric023_1K-JPG.mat` | `Fabric023_1K_BlackColor` | - | - | Chưa dùng |
| 2 | **ambientCG_Fabric042_1K-JPG.001** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair2/Materials/ambientCG_Fabric042_1K-JPG.001.mat` | `Fabric042_1K_Color` | - | - | Chưa dùng |
| 3 | **ambientCG_Leather026_1K-JPG.002** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair2/Materials/ambientCG_Leather026_1K-JPG.002.mat` | `Leather026_1K_Color` | - | - | Chưa dùng |
| 4 | **ambientCG_Leather027_2K-JPG** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair3/Materials/ambientCG_Leather027_2K-JPG.mat` | `Leather027_2K_Color` | - | - | Chưa dùng |
| 5 | **ambientCG_Leather027_2K-JPG.001** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair3/Materials/ambientCG_Leather027_2K-JPG.001.mat` | - | - | - | Chưa dùng |
| 6 | **ambientCG_Metal041A_1K-JPG** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair1/Materials/ambientCG_Metal041A_1K-JPG.mat` | `Metal041A_1K_Color` | - | - | **Có (1 vật thể):** Arm-Metal |
| 7 | **ambientCG_Metal041A_1K-JPG** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair3/Materials/ambientCG_Metal041A_1K-JPG.mat` | `Metal041A_1K_Color` | - | - | Chưa dùng |
| 8 | **ambientCG_Metal046B_1K-JPG** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair2/Materials/ambientCG_Metal046B_1K-JPG.mat` | `Metal046B_1K_Color` | - | - | Chưa dùng |
| 9 | **ambientCG_Metal046B_1K-JPG.001** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair2/Materials/ambientCG_Metal046B_1K-JPG.001.mat` | `Fabric042_1K_Color` | - | - | Chưa dùng |
| 10 | **ambientCG_Plastic006_1K-JPG** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair2/Materials/ambientCG_Plastic006_1K-JPG.mat` | `Plastic006_1K_Color` | - | - | Chưa dùng |
| 11 | **Materiais** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair1/Materials/Materiais.mat` | `Metal012_1K_Color` | - | - | **Có (6 vật thể):** Chair Foot 5, Chair Foot 4... |
| 12 | **Materiais.007** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair1/Materials/Materiais.007.mat` | `Plastic006_1K_Color` | - | - | **Có (9 vật thể):** Seat Mechanism, Seat Adjustment... |
| 13 | **Materiais.008** | `Universal Render Pipeline/Lit` | `Assets/3d Objects/Office Chair Pack 1.0/Chair1/Materials/Materiais.008.mat` | `Cadeira` | - | - | **Có (2 vật thể):** Seat and backrest, Arm-Leather |

### 📂 Phân nhóm: Giao Diện & XR Toolkit (47 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **accentcolor** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/DemoAssets/Models/PushButton.fbx` | - | - | - | Chưa dùng |
| 2 | **aiAmbientOcclusion1** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Models/AnchorBase.fbx` | - | - | - | Chưa dùng |
| 3 | **aiAmbientOcclusion1** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Models/Anchor/AnchorBase.fbx` | - | - | - | Chưa dùng |
| 4 | **aiAmbientOcclusion1** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Models/Blink/BlinkVisual.fbx` | - | - | - | Chưa dùng |
| 5 | **aiAmbientOcclusion1** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Models/Cursors/cursor.fbx` | - | - | - | Chưa dùng |
| 6 | **aiAmbientOcclusion1** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Models/UI/Spatial Panel Manipulator Model.fbx` | - | - | - | Chưa dùng |
| 7 | **aiStandardSurface1** | `Shader Graphs/ArnoldStandardSurface` | `Assets/VRTemplateAssets/Models/Environment/Template Environment.fbx` | - | - | - | Chưa dùng |
| 8 | **chess_board_square_black** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Models/Chess/Chess_Board.fbx` | - | - | - | Chưa dùng |
| 9 | **DepthOnly** | `Unlit/DepthOnly` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Materials/DepthOnly.mat` | - | - | - | Chưa dùng |
| 10 | **Font Material** | `GUI/Text Shader` | `Assets/VRTemplateAssets/Fonts/Inter/Inter-Regular.ttf` | `Font Texture` | - | - | Chưa dùng |
| 11 | **HandsDefaultMaterial** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Hands/1.8.1/HandVisualizer/Materials/HandsDefaultMaterial.mat` | - | - | ✅ Có | Chưa dùng |
| 12 | **HighFidelity** | `Universal Render Pipeline/Unlit` | `Assets/Samples/XR Hands/1.8.1/HandVisualizer/Materials/HighFidelity.mat` | `HighFidelity` | - | - | Chưa dùng |
| 13 | **Interactable** | `Shader Graphs/Interactable` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Shaders/Interactable.shadergraph` | - | - | - | Chưa dùng |
| 14 | **InteractablePrimitive** | `Shader Graphs/InteractablePrimitive` | `Assets/VRTemplateAssets/Shaders/InteractablePrimitive.shadergraph` | - | - | - | Chưa dùng |
| 15 | **Inter-Regular SDF Material** | `TextMeshPro/Distance Field` | `Assets/VRTemplateAssets/Fonts/Inter/Inter-Regular SDF NoBackfaceCulling.asset` | `Inter-Regular SDF Atlas` | - | - | Chưa dùng |
| 16 | **Inter-Regular SDF Material** | `TextMeshPro/Distance Field` | `Assets/VRTemplateAssets/Fonts/Inter/Inter-Regular SDF.asset` | `Inter-Regular SDF Atlas` | - | - | Chưa dùng |
| 17 | **Inter-Regular SDF Material XRay Blue** | `TextMeshPro/Distance Field` | `Assets/VRTemplateAssets/Fonts/Inter/Inter-Regular SDF Material XRay Blue.mat` | `Inter-Regular SDF Atlas` | - | - | Chưa dùng |
| 18 | **Inter-Regular SDF Overlay Material** | `TextMeshPro/Distance Field Overlay` | `Assets/VRTemplateAssets/Fonts/Inter/Inter-Regular SDF Overlay Material.mat` | `Inter-Regular SDF Atlas` | - | - | Chưa dùng |
| 19 | **Inter-Regular SDF Overlay Outline Material** | `TextMeshPro/Distance Field Overlay` | `Assets/VRTemplateAssets/Fonts/Inter/Inter-Regular SDF Overlay Outline Material.mat` | `Inter-Regular SDF Atlas` | - | - | Chưa dùng |
| 20 | **Inter-Regular SDF Overlay Outline Thick Material** | `TextMeshPro/Distance Field Overlay` | `Assets/VRTemplateAssets/Fonts/Inter/Inter-Regular SDF Overlay Outline Thick Material.mat` | `Inter-Regular SDF Atlas` | - | - | Chưa dùng |
| 21 | **JoystickUISprite** | `Universal Render Pipeline/2D/Sprite-Lit-Default` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Materials/JoystickUISprite.mat` | - | - | - | Chưa dùng |
| 22 | **lambert1** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Models/Chess/Chess_Pawn.fbx` | - | - | - | Chưa dùng |
| 23 | **lambert1** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Models/VirtualTabletop.fbx` | - | - | - | Chưa dùng |
| 24 | **lambert1** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Models/Poke/PokePointer.fbx` | - | - | - | Chưa dùng |
| 25 | **lambert2** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Models/Frame.fbx` | - | - | - | Chưa dùng |
| 26 | **lambert2** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Models/Environment/Totem1.fbx` | - | - | - | Chưa dùng |
| 27 | **lambert2** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Models/Environment/Totem2.fbx` | - | - | - | Chưa dùng |
| 28 | **Marks** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Models/Environment/Arrows.fbx` | - | - | ✅ Có | Chưa dùng |
| 29 | **RimLight** | `Shader Graphs/RimLight` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Shaders/RimLight.shadergraph` | - | - | - | Chưa dùng |
| 30 | **TeleportNavigatorUI** | `Unlit/Transparent` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Materials/TeleportNavigatorUI.mat` | `TeleportNavigatorUI` | - | - | Chưa dùng |
| 31 | **TransparentRed** | `Universal Render Pipeline/Unlit` | `Assets/Samples/XR Hands/1.8.1/HandVisualizer/Materials/TransparentRed.mat` | `TransparentRed` | - | - | Chưa dùng |
| 32 | **TunnelingVignette** | `VR/TunnelingVignette` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/TunnelingVignette/TunnelingVignette.mat` | - | - | - | Chưa dùng |
| 33 | **TunnelingVignetteSG** | `Shader Graphs/TunnelingVignetteSG` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/TunnelingVignette/TunnelingVignetteSG.shadergraph` | - | - | - | Chưa dùng |
| 34 | **Unity_Hand** | `Shader Graphs/Unity_Hand` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Shaders/Unity_Hand.shadergraph` | - | - | - | Chưa dùng |
| 35 | **Unity_Hand_Dark** | `Shader Graphs/Unity_Hand_Noise` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Materials/Unity_Hand_Dark.mat` | - | - | - | Chưa dùng |
| 36 | **Unity_Hand_HDR** | `Shader Graphs/Unity_Hand_HDR` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Shaders/Unity_Hand_HDR.shadergraph` | - | - | - | Chưa dùng |
| 37 | **Unity_Hand_Light** | `Shader Graphs/Unity_Hand_Noise` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Materials/Unity_Hand_Light.mat` | - | - | - | Chưa dùng |
| 38 | **Unity_Hand_Medium** | `Shader Graphs/Unity_Hand_Noise` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Materials/Unity_Hand_Medium.mat` | - | - | - | Chưa dùng |
| 39 | **Unity_Hand_Noise** | `Shader Graphs/Unity_Hand_Noise` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Shaders/Unity_Hand_Noise.shadergraph` | - | - | - | Chưa dùng |
| 40 | **Unity_Hand_Noise_HDR** | `Shader Graphs/Unity_Hand_Noise_HDR` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/Shaders/Unity_Hand_Noise_HDR.shadergraph` | - | - | - | Chưa dùng |
| 41 | **unityblue** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Models/AnchorArrow.fbx` | - | - | - | Chưa dùng |
| 42 | **unityblue** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Models/Anchor/AnchorArrow.fbx` | - | - | - | Chưa dùng |
| 43 | **Unlit_Fresnel** | `Shader Graphs/Unlit_Fresnel` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Shaders/Unlit_Fresnel.shadergraph` | - | - | - | Chưa dùng |
| 44 | **Unlit_ShaderGraph** | `Shader Graphs/Unlit_ShaderGraph` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Shaders/Unlit_ShaderGraph.shadergraph` | - | - | - | Chưa dùng |
| 45 | **White** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Models/AnchorFade.fbx` | - | - | - | Chưa dùng |
| 46 | **White** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Models/Anchor/AnchorFade.fbx` | - | - | - | Chưa dùng |
| 47 | **YELLOW** | `Shader Graphs/ArnoldStandardSurface` | `Assets/VRTemplateAssets/Models/Marks/Marks.fbx` | - | - | - | Chưa dùng |

### 📂 Phân nhóm: Hệ Thống Đèn LED Chiếu Sáng (PolyKebap) (23 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **LedLightBase** | `Universal Render Pipeline/Lit` | `Assets/PolyKebap/LED Light Essentials Pack/Materials/LedLightBase.mat` | - | - | - | **Có (1 vật thể):** ledbase_circle |
| 2 | **LedLightBlue** | `Universal Render Pipeline/Lit` | `Assets/PolyKebap/LED Light Essentials Pack/Materials/LedLightBlue.mat` | - | - | ✅ Có | Chưa dùng |
| 3 | **LedLightGlass** | `Universal Render Pipeline/Lit` | `Assets/PolyKebap/LED Light Essentials Pack/Materials/LedLightGlass.mat` | - | - | - | **Có (1 vật thể):** ledbase_circle_glass |
| 4 | **LedLightRed** | `Universal Render Pipeline/Lit` | `Assets/PolyKebap/LED Light Essentials Pack/Materials/LedLightRed.mat` | - | - | ✅ Có | Chưa dùng |
| 5 | **LedLightWhite** | `Universal Render Pipeline/Lit` | `Assets/PolyKebap/LED Light Essentials Pack/Materials/LedLightWhite.mat` | - | - | ✅ Có | **Có (1 vật thể):** light_ON |
| 6 | **LedLightYellow** | `Universal Render Pipeline/Lit` | `Assets/PolyKebap/LED Light Essentials Pack/Materials/LedLightYellow.mat` | - | - | ✅ Có | Chưa dùng |
| 7 | **Mat_Cube92_LED_Border** | `Universal Render Pipeline/Lit` | `Assets/Materials/Mat_Cube92_LED_Border.mat` | - | - | ✅ Có | **Có (4 vật thể):** BorderStrip_Right, BorderStrip_Left... |
| 8 | **mtl_npc_csl_shirtopenrolled_00f_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shirtopenrolled_00f_01_01.mat` | `npc_csl_shirtopenrolled_00f_01_01_c` | `npc_csl_shirtopenrolled_00f_01_01_n` | - | Chưa dùng |
| 9 | **mtl_npc_csl_shirtopenrolled_00f_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shirtopenrolled_00f_01_01.mat` | `npc_csl_shirtopenrolled_00f_01_01_c` | `npc_csl_shirtopenrolled_00f_01_01_n` | - | Chưa dùng |
| 10 | **mtl_npc_csl_shirtopenrolled_00f_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shirtopenrolled_00f_01_01bw.mat` | `npc_csl_shirtopenrolled_00f_01_01bw_c` | `npc_csl_shirtopenrolled_00f_01_01_n` | - | Chưa dùng |
| 11 | **mtl_npc_csl_shirtopenrolled_00f_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shirtopenrolled_00f_01_01bw.mat` | `npc_csl_shirtopenrolled_00f_01_01bw_c` | `npc_csl_shirtopenrolled_00f_01_01_n` | - | Chưa dùng |
| 12 | **mtl_npc_csl_shirtopenrolled_00f_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shirtopenrolled_00f_01_02.mat` | `npc_csl_shirtopenrolled_00f_01_02_c` | `npc_csl_shirtopenrolled_00f_01_01_n` | - | Chưa dùng |
| 13 | **mtl_npc_csl_shirtopenrolled_00f_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shirtopenrolled_00f_01_02.mat` | `npc_csl_shirtopenrolled_00f_01_02_c` | `npc_csl_shirtopenrolled_00f_01_01_n` | - | Chưa dùng |
| 14 | **mtl_npc_csl_shirtopenrolled_00f_01_02bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shirtopenrolled_00f_01_02bw.mat` | `npc_csl_shirtopenrolled_00f_01_02bw_c` | `npc_csl_shirtopenrolled_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_shirtopenrolled_00f_lod4, npc_csl_shirtopenrolled_00f_lod3... |
| 15 | **mtl_npc_csl_shirtopenrolled_00f_01_02bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shirtopenrolled_00f_01_02bw.mat` | `npc_csl_shirtopenrolled_00f_01_02bw_c` | `npc_csl_shirtopenrolled_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_shirtopenrolled_00f_lod4, npc_csl_shirtopenrolled_00f_lod3... |
| 16 | **mtl_npc_csl_shirtopenrolled_00m_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shirtopenrolled_00m_01_01.mat` | `npc_csl_shirtopenrolled_00m_01_01_c` | `npc_csl_shirtopenrolled_00m_01_01_n` | - | Chưa dùng |
| 17 | **mtl_npc_csl_shirtopenrolled_00m_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shirtopenrolled_00m_01_01.mat` | `npc_csl_shirtopenrolled_00m_01_01_c` | `npc_csl_shirtopenrolled_00m_01_01_n` | - | Chưa dùng |
| 18 | **mtl_npc_csl_shirtopenrolled_00m_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shirtopenrolled_00m_01_01bw.mat` | `npc_csl_shirtopenrolled_00m_01_01bw_c` | `npc_csl_shirtopenrolled_00m_01_01_n` | - | **Có (5 vật thể):** npc_csl_shirtopenrolled_00m_lod4, npc_csl_shirtopenrolled_00m_lod3... |
| 19 | **mtl_npc_csl_shirtopenrolled_00m_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shirtopenrolled_00m_01_01bw.mat` | `npc_csl_shirtopenrolled_00m_01_01bw_c` | `npc_csl_shirtopenrolled_00m_01_01_n` | - | Chưa dùng |
| 20 | **mtl_npc_csl_shirtopenrolled_00m_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shirtopenrolled_00m_01_02.mat` | `npc_csl_shirtopenrolled_00m_01_02_c` | `npc_csl_shirtopenrolled_00m_01_01_n` | - | Chưa dùng |
| 21 | **mtl_npc_csl_shirtopenrolled_00m_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shirtopenrolled_00m_01_02.mat` | `npc_csl_shirtopenrolled_00m_01_02_c` | `npc_csl_shirtopenrolled_00m_01_01_n` | - | Chưa dùng |
| 22 | **mtl_npc_csl_shirtopenrolled_00m_01_02bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shirtopenrolled_00m_01_02bw.mat` | `npc_csl_shirtopenrolled_00m_01_02bw_c` | `npc_csl_shirtopenrolled_00m_01_01_n` | - | Chưa dùng |
| 23 | **mtl_npc_csl_shirtopenrolled_00m_01_02bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shirtopenrolled_00m_01_02bw.mat` | `npc_csl_shirtopenrolled_00m_01_02bw_c` | `npc_csl_shirtopenrolled_00m_01_01_n` | - | Chưa dùng |

### 📂 Phân nhóm: Khách Tham Quan Bảo Tàng (NPC) (96 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **mtl_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Mesh/Mesh_Parts/npc_csl_tshirt_00m_sleeves.fbx` | - | - | - | Chưa dùng |
| 2 | **mtl_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Mesh/npc_csl_shoe_02.fbx` | - | - | - | Chưa dùng |
| 3 | **mtl_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Mesh/npc_csl_shoe_03.fbx` | - | - | - | Chưa dùng |
| 4 | **mtl_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Mesh/npc_csl_shoe_04.fbx` | - | - | - | Chưa dùng |
| 5 | **mtl_npc_csl_pants_00f_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_pants_00f_01_01.mat` | `npc_csl_pants_00f_01_01_c` | `npc_csl_pants_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_pants_00f_lod4, npc_csl_pants_00f_lod3... |
| 6 | **mtl_npc_csl_pants_00f_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_pants_00f_01_01.mat` | `npc_csl_pants_00f_01_01_c` | `npc_csl_pants_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_pants_00f_lod4, npc_csl_pants_00f_lod3... |
| 7 | **mtl_npc_csl_pants_00f_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_pants_00f_01_01bw.mat` | `npc_csl_pants_00f_01_01bw_c` | `npc_csl_pants_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_pants_00f_lod4, npc_csl_pants_00f_lod3... |
| 8 | **mtl_npc_csl_pants_00f_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_pants_00f_01_01bw.mat` | `npc_csl_pants_00f_01_01bw_c` | `npc_csl_pants_00f_01_01_n` | - | Chưa dùng |
| 9 | **mtl_npc_csl_pants_00f_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_pants_00f_01_02.mat` | `npc_csl_pants_00f_01_02_c` | `npc_csl_pants_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_pants_00f_lod4, npc_csl_pants_00f_lod3... |
| 10 | **mtl_npc_csl_pants_00f_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_pants_00f_01_02.mat` | `npc_csl_pants_00f_01_02_c` | `npc_csl_pants_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_pants_00f_lod4, npc_csl_pants_00f_lod3... |
| 11 | **mtl_npc_csl_pants_00f_01_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_pants_00f_01_03.mat` | `npc_csl_pants_00f_01_03_c` | `npc_csl_pants_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_pants_00f_lod4, npc_csl_pants_00f_lod3... |
| 12 | **mtl_npc_csl_pants_00f_01_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_pants_00f_01_03.mat` | `npc_csl_pants_00f_01_03_c` | `npc_csl_pants_00f_01_01_n` | - | Chưa dùng |
| 13 | **mtl_npc_csl_pants_00m_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_pants_00m_01_01.mat` | `npc_csl_pants_00m_01_01_c` | `npc_csl_pants_00m_01_01_n` | - | **Có (5 vật thể):** npc_csl_pants_00m_lod4, npc_csl_pants_00m_lod3... |
| 14 | **mtl_npc_csl_pants_00m_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_pants_00m_01_01.mat` | `npc_csl_pants_00m_01_01_c` | `npc_csl_pants_00m_01_01_n` | - | **Có (5 vật thể):** npc_csl_pants_00m_lod4, npc_csl_pants_00m_lod3... |
| 15 | **mtl_npc_csl_pants_00m_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_pants_00m_01_01bw.mat` | `npc_csl_pants_00m_01_01bw_c` | `npc_csl_pants_00m_01_01_n` | - | Chưa dùng |
| 16 | **mtl_npc_csl_pants_00m_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_pants_00m_01_01bw.mat` | `npc_csl_pants_00m_01_01bw_c` | `npc_csl_pants_00m_01_01_n` | - | Chưa dùng |
| 17 | **mtl_npc_csl_pants_00m_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_pants_00m_01_02.mat` | `npc_csl_pants_00m_01_02_c` | `npc_csl_pants_00m_01_01_n` | - | **Có (5 vật thể):** npc_csl_pants_00m_lod4, npc_csl_pants_00m_lod3... |
| 18 | **mtl_npc_csl_pants_00m_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_pants_00m_01_02.mat` | `npc_csl_pants_00m_01_02_c` | `npc_csl_pants_00m_01_01_n` | - | Chưa dùng |
| 19 | **mtl_npc_csl_pants_00m_01_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_pants_00m_01_03.mat` | `npc_csl_pants_00m_01_03_c` | `npc_csl_pants_00m_01_01_n` | - | Chưa dùng |
| 20 | **mtl_npc_csl_pants_00m_01_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_pants_00m_01_03.mat` | `npc_csl_pants_00m_01_03_c` | `npc_csl_pants_00m_01_01_n` | - | Chưa dùng |
| 21 | **mtl_npc_csl_shoe_01_00_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shoe_01_00_01.mat` | `npc_csl_shoe_01_00_01_c` | `npc_csl_shoe_01_00_01_n` | - | **Có (5 vật thể):** npc_csl_shoe_01_lod4, npc_csl_shoe_01_lod3... |
| 22 | **mtl_npc_csl_shoe_01_00_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shoe_01_00_01.mat` | `npc_csl_shoe_01_00_01_c` | `npc_csl_shoe_01_00_01_n` | - | **Có (5 vật thể):** npc_csl_shoe_01_lod4, npc_csl_shoe_01_lod3... |
| 23 | **mtl_npc_csl_shoe_01_00_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shoe_01_00_01bw.mat` | `npc_csl_shoe_01_00_01bw_c` | `npc_csl_shoe_01_00_01_n` | - | **Có (5 vật thể):** npc_csl_shoe_01_lod4, npc_csl_shoe_01_lod3... |
| 24 | **mtl_npc_csl_shoe_01_00_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shoe_01_00_01bw.mat` | `npc_csl_shoe_01_00_01bw_c` | `npc_csl_shoe_01_00_01_n` | - | Chưa dùng |
| 25 | **mtl_npc_csl_shoe_01_00_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shoe_01_00_02.mat` | `npc_csl_shoe_01_00_02_c` | `npc_csl_shoe_01_00_01_n` | - | **Có (5 vật thể):** npc_csl_shoe_01_lod4, npc_csl_shoe_01_lod3... |
| 26 | **mtl_npc_csl_shoe_01_00_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shoe_01_00_02.mat` | `npc_csl_shoe_01_00_02_c` | `npc_csl_shoe_01_00_01_n` | - | **Có (5 vật thể):** npc_csl_shoe_01_lod4, npc_csl_shoe_01_lod3... |
| 27 | **mtl_npc_csl_shoe_01_00_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_shoe_01_00_03.mat` | `npc_csl_shoe_01_00_03_c` | `npc_csl_shoe_01_00_01_n` | - | **Có (5 vật thể):** npc_csl_shoe_01_lod4, npc_csl_shoe_01_lod3... |
| 28 | **mtl_npc_csl_shoe_01_00_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_shoe_01_00_03.mat` | `npc_csl_shoe_01_00_03_c` | `npc_csl_shoe_01_00_01_n` | - | Chưa dùng |
| 29 | **mtl_npc_csl_tshirt_00f_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_tshirt_00f_01_01.mat` | `npc_csl_tshirt_00f_01_01_c` | `npc_csl_tshirt_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_tshirt_00f_lod4, npc_csl_tshirt_00f_lod3... |
| 30 | **mtl_npc_csl_tshirt_00f_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_tshirt_00f_01_01.mat` | `npc_csl_tshirt_00f_01_01_c` | `npc_csl_tshirt_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_tshirt_00f_lod4, npc_csl_tshirt_00f_lod3... |
| 31 | **mtl_npc_csl_tshirt_00f_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_tshirt_00f_01_01bw.mat` | `npc_csl_tshirt_00f_01_01bw_c` | `npc_csl_tshirt_00f_01_01_n` | - | Chưa dùng |
| 32 | **mtl_npc_csl_tshirt_00f_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_tshirt_00f_01_01bw.mat` | `npc_csl_tshirt_00f_01_01bw_c` | `npc_csl_tshirt_00f_01_01_n` | - | Chưa dùng |
| 33 | **mtl_npc_csl_tshirt_00f_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_tshirt_00f_01_02.mat` | `npc_csl_tshirt_00f_01_02_c` | `npc_csl_tshirt_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_tshirt_00f_lod4, npc_csl_tshirt_00f_lod3... |
| 34 | **mtl_npc_csl_tshirt_00f_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_tshirt_00f_01_02.mat` | `npc_csl_tshirt_00f_01_02_c` | `npc_csl_tshirt_00f_01_01_n` | - | Chưa dùng |
| 35 | **mtl_npc_csl_tshirt_00f_01_02bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_tshirt_00f_01_02bw.mat` | `npc_csl_tshirt_00f_01_02bw` | `npc_csl_tshirt_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_tshirt_00f_lod4, npc_csl_tshirt_00f_lod3... |
| 36 | **mtl_npc_csl_tshirt_00f_01_02bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_tshirt_00f_01_02bw.mat` | `npc_csl_tshirt_00f_01_02bw` | `npc_csl_tshirt_00f_01_01_n` | - | Chưa dùng |
| 37 | **mtl_npc_csl_tshirt_00f_01_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_tshirt_00f_01_03.mat` | `npc_csl_tshirt_00f_01_03_c` | `npc_csl_tshirt_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_tshirt_00f_frontal_lod4, npc_csl_tshirt_00f_frontal_lod3... |
| 38 | **mtl_npc_csl_tshirt_00f_01_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_tshirt_00f_01_03.mat` | `npc_csl_tshirt_00f_01_03_c` | `npc_csl_tshirt_00f_01_01_n` | - | **Có (5 vật thể):** npc_csl_tshirt_00f_frontal_lod4, npc_csl_tshirt_00f_frontal_lod3... |
| 39 | **mtl_npc_csl_tshirt_00m_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_tshirt_00m_01_01.mat` | `npc_csl_tshirt_00m_01_01_c` | `npc_csl_tshirt_00m_01_01_n` | - | Chưa dùng |
| 40 | **mtl_npc_csl_tshirt_00m_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_tshirt_00m_01_01.mat` | `npc_csl_tshirt_00m_01_01_c` | `npc_csl_tshirt_00m_01_01_n` | - | Chưa dùng |
| 41 | **mtl_npc_csl_tshirt_00m_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_tshirt_00m_01_01bw.mat` | `npc_csl_tshirt_00m_01_01bw_c` | `npc_csl_tshirt_00m_01_01_n` | - | **Có (5 vật thể):** npc_csl_tshirt_00m_lod4, npc_csl_tshirt_00m_lod3... |
| 42 | **mtl_npc_csl_tshirt_00m_01_01bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_tshirt_00m_01_01bw.mat` | `npc_csl_tshirt_00m_01_01bw_c` | `npc_csl_tshirt_00m_01_01_n` | - | **Có (5 vật thể):** npc_csl_tshirt_00m_lod4, npc_csl_tshirt_00m_lod3... |
| 43 | **mtl_npc_csl_tshirt_00m_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_tshirt_00m_01_02.mat` | `npc_csl_tshirt_00m_01_02_c` | `npc_csl_tshirt_00m_01_01_n` | - | Chưa dùng |
| 44 | **mtl_npc_csl_tshirt_00m_01_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_tshirt_00m_01_02.mat` | `npc_csl_tshirt_00m_01_02_c` | `npc_csl_tshirt_00m_01_01_n` | - | Chưa dùng |
| 45 | **mtl_npc_csl_tshirt_00m_01_02bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_tshirt_00m_01_02bw.mat` | `npc_csl_tshirt_00m_01_02bw_c` | `npc_csl_tshirt_00m_01_01_n` | - | **Có (5 vật thể):** npc_csl_tshirt_00m_frontal_lod4, npc_csl_tshirt_00m_frontal_lod3... |
| 46 | **mtl_npc_csl_tshirt_00m_01_02bw** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_tshirt_00m_01_02bw.mat` | `npc_csl_tshirt_00m_01_02bw_c` | `npc_csl_tshirt_00m_01_01_n` | - | Chưa dùng |
| 47 | **mtl_npc_csl_tshirt_00m_01_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_csl_tshirt_00m_01_03.mat` | `npc_csl_tshirt_00m_01_03_c` | `npc_csl_tshirt_00m_01_01_n` | - | **Có (5 vật thể):** npc_csl_tshirt_00m_lod4, npc_csl_tshirt_00m_lod3... |
| 48 | **mtl_npc_csl_tshirt_00m_01_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_csl_tshirt_00m_01_03.mat` | `npc_csl_tshirt_00m_01_03_c` | `npc_csl_tshirt_00m_01_01_n` | - | Chưa dùng |
| 49 | **mtl_npc_hair_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_01.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 50 | **mtl_npc_hair_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_01.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 51 | **mtl_npc_hair_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_02.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 52 | **mtl_npc_hair_02** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_02.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 53 | **mtl_npc_hair_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_03.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 54 | **mtl_npc_hair_03** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_03.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 55 | **mtl_npc_hair_04** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_04.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | **Có (5 vật thể):** npc_haircut_a_01_lod4, npc_haircut_a_01_lod3... |
| 56 | **mtl_npc_hair_04** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_04.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 57 | **mtl_npc_hair_05** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_05.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | **Có (5 vật thể):** npc_haircut_c_01_lod4, npc_haircut_c_01_lod3... |
| 58 | **mtl_npc_hair_05** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_05.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | **Có (5 vật thể):** npc_haircut_c_01_lod4, npc_haircut_c_01_lod3... |
| 59 | **mtl_npc_hair_06** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_06.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | **Có (20 vật thể):** npc_hairfacial_b_01_lod4, npc_hairfacial_b_01_lod3... |
| 60 | **mtl_npc_hair_06** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_06.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | **Có (5 vật thể):** npc_haircut_a_01_lod4, npc_haircut_a_01_lod3... |
| 61 | **mtl_npc_hair_07** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_07.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 62 | **mtl_npc_hair_07** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_07.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 63 | **mtl_npc_hair_08** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_08.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | **Có (5 vật thể):** npc_haircut_c_03_lod4, npc_haircut_c_03_lod3... |
| 64 | **mtl_npc_hair_08** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_08.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | **Có (5 vật thể):** npc_haircut_c_03_lod4, npc_haircut_c_03_lod3... |
| 65 | **mtl_npc_hair_09** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_09.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 66 | **mtl_npc_hair_09** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_09.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 67 | **mtl_npc_hair_10** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_10.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | **Có (5 vật thể):** npc_haircut_c_02_lod4, npc_haircut_c_02_lod3... |
| 68 | **mtl_npc_hair_10** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_10.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 69 | **mtl_npc_hair_11** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_11.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 70 | **mtl_npc_hair_11** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_11.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 71 | **mtl_npc_hair_12** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hair_12.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 72 | **mtl_npc_hair_12** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hair_12.mat` | `npc_hair_01_c` | `npc_hair_01_n` | - | Chưa dùng |
| 73 | **mtl_npc_hmn_body_01f_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_body_01f_01_01.mat` | `npc_hmn_body_01f_01_01_c` | `npc_hmn_body_01f_01_01_n` | ✅ Có | **Có (15 vật thể):** npc_hmn_01f_head2_lod4, npc_hmn_01f_head2_lod3... |
| 74 | **mtl_npc_hmn_body_01f_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_body_01f_01_01.mat` | `npc_hmn_body_01f_01_01_c` | `npc_hmn_body_01f_01_01_n` | ✅ Có | **Có (15 vật thể):** npc_hmn_01f_head2_lod4, npc_hmn_01f_head2_lod3... |
| 75 | **mtl_npc_hmn_body_01m_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_body_01m_01_01.mat` | `npc_hmn_body_01m_01_01_c` | `npc_hmn_body_01m_01_01_n` | ✅ Có | **Có (15 vật thể):** npc_hmn_01m_head2_lod4, npc_hmn_01m_head2_lod3... |
| 76 | **mtl_npc_hmn_body_01m_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_body_01m_01_01.mat` | `npc_hmn_body_01m_01_01_c` | `npc_hmn_body_01m_01_01_n` | ✅ Có | **Có (10 vật thể):** npc_hmn_01m_head2_lod4, npc_hmn_01m_head2_lod3... |
| 77 | **mtl_npc_hmn_face_01f_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_face_01f_01_01.mat` | `npc_hmn_face_01f_01_01_c` | `npc_hmn_face_01f_01_01_n` | ✅ Có | **Có (5 vật thể):** npc_hmn_01f_head2_lod4, npc_hmn_01f_head2_lod3... |
| 78 | **mtl_npc_hmn_face_01f_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_face_01f_01_01.mat` | `npc_hmn_face_01f_01_01_c` | `npc_hmn_face_01f_01_01_n` | ✅ Có | **Có (5 vật thể):** npc_hmn_01f_head2_lod4, npc_hmn_01f_head2_lod3... |
| 79 | **mtl_npc_hmn_face_01f_02_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_face_01f_02_01.mat` | `npc_hmn_face_01f_02_01_c` | `npc_hmn_face_01f_02_01_n` | ✅ Có | **Có (5 vật thể):** npc_hmn_01f_head2_lod4, npc_hmn_01f_head2_lod3... |
| 80 | **mtl_npc_hmn_face_01f_02_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_face_01f_02_01.mat` | `npc_hmn_face_01f_02_01_c` | `npc_hmn_face_01f_02_01_n` | ✅ Có | **Có (5 vật thể):** npc_hmn_01f_head2_lod4, npc_hmn_01f_head2_lod3... |
| 81 | **mtl_npc_hmn_face_01f_03_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_face_01f_03_01.mat` | `npc_hmn_face_01f_03_01_c` | `npc_hmn_face_01f_03_01_n` | ✅ Có | **Có (5 vật thể):** npc_hmn_01f_head2_lod4, npc_hmn_01f_head2_lod3... |
| 82 | **mtl_npc_hmn_face_01f_03_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_face_01f_03_01.mat` | `npc_hmn_face_01f_03_01_c` | `npc_hmn_face_01f_03_01_n` | ✅ Có | Chưa dùng |
| 83 | **mtl_npc_hmn_face_01f_04_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_face_01f_04_01.mat` | `npc_hmn_face_01f_04_01_c` | `npc_hmn_face_01f_04_01_n` | ✅ Có | Chưa dùng |
| 84 | **mtl_npc_hmn_face_01f_04_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_face_01f_04_01.mat` | `npc_hmn_face_01f_04_01_c` | `npc_hmn_face_01f_04_01_n` | ✅ Có | Chưa dùng |
| 85 | **mtl_npc_hmn_face_01f_05_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_face_01f_05_01.mat` | `npc_hmn_face_01f_05_01_c` | `npc_hmn_face_01f_05_01_n` | ✅ Có | **Có (5 vật thể):** npc_hmn_01f_head2_lod4, npc_hmn_01f_head2_lod3... |
| 86 | **mtl_npc_hmn_face_01f_05_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_face_01f_05_01.mat` | `npc_hmn_face_01f_05_01_c` | `npc_hmn_face_01f_05_01_n` | ✅ Có | Chưa dùng |
| 87 | **mtl_npc_hmn_face_01m_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_face_01m_01_01.mat` | `npc_hmn_face_01m_01_01_c` | `npc_hmn_face_01m_01_01_n` | ✅ Có | **Có (5 vật thể):** npc_hmn_01m_head2_lod4, npc_hmn_01m_head2_lod3... |
| 88 | **mtl_npc_hmn_face_01m_01_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_face_01m_01_01.mat` | `npc_hmn_face_01m_01_01_c` | `npc_hmn_face_01m_01_01_n` | ✅ Có | **Có (5 vật thể):** npc_hmn_01m_head2_lod4, npc_hmn_01m_head2_lod3... |
| 89 | **mtl_npc_hmn_face_01m_02_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_face_01m_02_01.mat` | `npc_hmn_face_01m_02_01_c` | `npc_hmn_face_01m_02_01_n` | ✅ Có | Chưa dùng |
| 90 | **mtl_npc_hmn_face_01m_02_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_face_01m_02_01.mat` | `npc_hmn_face_01m_02_01_c` | `npc_hmn_face_01m_02_01_n` | ✅ Có | Chưa dùng |
| 91 | **mtl_npc_hmn_face_01m_03_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_face_01m_03_01.mat` | `npc_hmn_face_01m_03_01_c` | `npc_hmn_face_01m_03_01_n` | ✅ Có | Chưa dùng |
| 92 | **mtl_npc_hmn_face_01m_03_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_face_01m_03_01.mat` | `npc_hmn_face_01m_03_01_c` | `npc_hmn_face_01m_03_01_n` | ✅ Có | Chưa dùng |
| 93 | **mtl_npc_hmn_face_01m_04_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_face_01m_04_01.mat` | `npc_hmn_face_01m_04_01_c` | `npc_hmn_face_01m_04_01_n` | ✅ Có | Chưa dùng |
| 94 | **mtl_npc_hmn_face_01m_04_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_face_01m_04_01.mat` | `npc_hmn_face_01m_04_01_c` | `npc_hmn_face_01m_04_01_n` | ✅ Có | Chưa dùng |
| 95 | **mtl_npc_hmn_face_01m_05_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/Materials/mtl_npc_hmn_face_01m_05_01.mat` | `npc_hmn_face_01m_05_01_c` | `npc_hmn_face_01m_05_01_n` | ✅ Có | **Có (5 vật thể):** npc_hmn_01m_head2_lod4, npc_hmn_01m_head2_lod3... |
| 96 | **mtl_npc_hmn_face_01m_05_01** | `Universal Render Pipeline/Lit` | `Assets/npc_casual_set_00/MaterialsUPR/mtl_npc_hmn_face_01m_05_01.mat` | `npc_hmn_face_01m_05_01_c` | `npc_hmn_face_01m_05_01_n` | ✅ Có | Chưa dùng |

### 📂 Phân nhóm: Mặt Sàn Bảo Tàng (Floor) (1 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **images** | `Museum/Glossy Wood Mirror Floor` | `Assets/Floor/Materials/images.mat` | `images` | `images_Normal` | - | Chưa dùng |

### 📂 Phân nhóm: Mô Hình Dân Tộc & Trưng Bày (66 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/B_rau_nam_00001.glb` | - | - | - | **Có (1 vật thể):** B_rau_nam_00001 |
| 2 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/B_rau_nu_00001.glb` | - | - | - | **Có (1 vật thể):** B_rau_nu_00001 |
| 3 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Ba_na_nam_00001.glb` | - | - | - | **Có (1 vật thể):** Ba_na_nam_00001 |
| 4 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Ba_na_nu_00001.glb` | - | - | - | **Có (1 vật thể):** Ba_na_nu_00001 |
| 5 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Chu_ru_nam_00001.glb` | - | - | - | **Có (1 vật thể):** Chu_ru_nam_00001 |
| 6 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Chu_ru_nu_00001.glb` | - | - | - | **Có (1 vật thể):** Chu_ru_nu_00001 |
| 7 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Co_ho_nam_00001.glb` | - | - | - | **Có (1 vật thể):** Co_ho_nam_00001 |
| 8 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Co_ho_nu_00001.glb` | - | - | - | **Có (1 vật thể):** Co_ho_nu_00001 |
| 9 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/E_de_nam_00001.glb` | - | - | - | **Có (1 vật thể):** E_de_nam_00001 |
| 10 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/E_de_nu_00001.glb` | - | - | - | **Có (1 vật thể):** E_de_nu_00001 |
| 11 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Gia_rai_nam_00001.glb` | - | - | - | **Có (1 vật thể):** Gia_rai_nam_00001 |
| 12 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Gia_rai_nu_00001.glb` | - | - | - | **Có (1 vật thể):** Gia_rai_nu_00001 |
| 13 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Gie_trieng_nam_00001.glb` | - | - | - | **Có (1 vật thể):** Gie_trieng_nam_00001 |
| 14 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Gie_trieng_nu_00001.glb` | - | - | - | **Có (1 vật thể):** Gie_trieng_nu_00001 |
| 15 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/M_nong_nam_00001.glb` | - | - | - | **Có (1 vật thể):** M_nong_nam_00001 |
| 16 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/M_nong_nu_00001.glb` | - | - | - | **Có (1 vật thể):** M_nong_nu_00001 |
| 17 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Ma_nam_00001.glb` | - | - | - | **Có (1 vật thể):** Ma_nam_00001 |
| 18 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Ma_nu_00001.glb` | - | - | - | **Có (1 vật thể):** Ma_nu_00001 |
| 19 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Ro_mam_nam_00001.glb` | - | - | - | **Có (1 vật thể):** Ro_mam_nam_00001 |
| 20 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Ro_mam_nu_00001.glb` | - | - | - | **Có (1 vật thể):** Ro_mam_nu_00001 |
| 21 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/X_tieng_nam_00001.glb` | - | - | - | **Có (1 vật thể):** X_tieng_nam_00001 |
| 22 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/X_tieng_nu_00001.glb` | - | - | - | **Có (1 vật thể):** X_tieng_nu_00001 |
| 23 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Xo_dang_nam_00001.glb` | - | - | - | **Có (1 vật thể):** Xo_dang_nam_00001 |
| 24 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/3d/Xo_dang_nu_00001.glb` | - | - | - | **Có (1 vật thể):** Xo_dang_nu_00001 |
| 25 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/1789713034894_6587468119155234841_g4083560111675440089_0aa065c110b72af69e5e6960ca38c480.jpg_00001.glb` | - | - | - | Chưa dùng |
| 26 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/1789720362593_6587468119155234841_g4083560111675440089_325124447e5affe6fd2ed038686ddea0.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1789720362593_6587468119155234841_g4083560111675440089_325124447e5affe6fd2ed038686ddea0.jpg_00001 |
| 27 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/1789720422505_6587468119155234841_g4083560111675440089_be951d2fdcd77e8ca852df28fd2f7500.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1789720422505_6587468119155234841_g4083560111675440089_be951d2fdcd77e8ca852df28fd2f7500.jpg_00001 |
| 28 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/1789720522046_6587468119155234841_g4083560111675440089_6243280bd69c9bdd8ae6ea636a7c6b7d.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1789720522046_6587468119155234841_g4083560111675440089_6243280bd69c9bdd8ae6ea636a7c6b7d.jpg_00001 (1) |
| 29 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/1789720751897_6587468119155234841_g4083560111675440089_65cf169fd5219d8b319df624232b5139.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1789720751897_6587468119155234841_g4083560111675440089_65cf169fd5219d8b319df624232b5139.jpg_00001 |
| 30 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/1789720751897_6587468119155234841_g4083560111675440089_65cf169fd5219d8b319df624232b5139.jpg_00002.glb` | - | - | - | Chưa dùng |
| 31 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/cua.glb` | - | - | - | **Có (1 vật thể):** cua |
| 32 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/image_9e2619-give-me-a-single-door.webp_00002.glb` | - | - | - | Chưa dùng |
| 33 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/output3/1790049462076_6587468119155234841_g4083560111675440089_86c08848d28c4ce590cab48ab2897974.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1790049462076_6587468119155234841_g4083560111675440089_86c08848d28c4ce590cab48ab2897974.jpg_00001 (1) |
| 34 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/output3/1790049484541_6587468119155234841_g4083560111675440089_e18dda212f0a08cbf649aa559d119288.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1790049484541_6587468119155234841_g4083560111675440089_e18dda212f0a08cbf649aa559d119288.jpg_00001 (1) |
| 35 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/output3/1790049533226_6587468119155234841_g4083560111675440089_afc3560457f6c63fe00aac2534ad2370.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1790049533226_6587468119155234841_g4083560111675440089_afc3560457f6c63fe00aac2534ad2370.jpg_00001 (1) |
| 36 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/output3/1790049621633_6587468119155234841_g4083560111675440089_f5ee226c817138c13cdd2c188f6fe90b.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1790049621633_6587468119155234841_g4083560111675440089_f5ee226c817138c13cdd2c188f6fe90b.jpg_00001 (1) |
| 37 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/output3/1790049655346_6587468119155234841_g4083560111675440089_6e66e0e4a4089f47aaf6aa6740b0101d.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1790049655346_6587468119155234841_g4083560111675440089_6e66e0e4a4089f47aaf6aa6740b0101d.jpg_00001 (1) |
| 38 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/output3/1790049717534_6587468119155234841_g4083560111675440089_3808d7c98d1af0bd8bfe3284c7879ac6.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1790049717534_6587468119155234841_g4083560111675440089_3808d7c98d1af0bd8bfe3284c7879ac6.jpg_00001 (1) |
| 39 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/output3/1790049770668_6587468119155234841_g4083560111675440089_28fc2f786a82dd8e50993d533c44b7cf.jpg_00001.glb` | - | - | - | **Có (2 vật thể):** 1790049770668_6587468119155234841_g4083560111675440089_28fc2f786a82dd8e50993d533c44b7cf.jpg_00001 (1), 1790049770668_6587468119155234841_g4083560111675440089_28fc2f786a82dd8e50993d533c44b7cf.jpg_00001 (2) |
| 40 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/output3/1790049782217_6587468119155234841_g4083560111675440089_4f74732d7db677dea97d6f40e84ef70d.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1790049782217_6587468119155234841_g4083560111675440089_4f74732d7db677dea97d6f40e84ef70d.jpg_00001 (1) |
| 41 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/output3/1790049992490_6587468119155234841_g4083560111675440089_d06fa59dff94ba1f43c5f52c4f09036b.jpg_00001.glb` | - | - | - | **Có (1 vật thể):** 1790049992490_6587468119155234841_g4083560111675440089_d06fa59dff94ba1f43c5f52c4f09036b.jpg_00001 (1) |
| 42 | **** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/output3/1790050711102_6587468119155234841_g4083560111675440089_11136e2ac551f234447c5c16c1f91cd0.jpg_00001.glb` | - | - | - | Chưa dùng |
| 43 | **316313476cfe2ddee7782e7855b64367** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/fire_escape__exit_doors.glb` | - | - | - | **Có (8 vật thể):** Object_16, Object_15... |
| 44 | **360_F_904748852_arIWUgQkRvMzxRENUGH0LeUB9MaPU68K** | `Universal Render Pipeline/Lit` | `Assets/3d/nội thất/Materials/360_F_904748852_arIWUgQkRvMzxRENUGH0LeUB9MaPU68K.mat` | `360_F_904748852_arIWUgQkRvMzxRENUGH0LeUB9MaPU68K` | - | - | **Có (27 vật thể):** Cube (26), Cube (75)... |
| 45 | **4dee0a89-1287-4d1e-8eef-b399bd986a92** | `Universal Render Pipeline/Lit` | `Assets/3d/3d/Materials/4dee0a89-1287-4d1e-8eef-b399bd986a92.mat` | `4dee0a89-1287-4d1e-8eef-b399bd986a92` | - | - | **Có (1 vật thể):** Cube (16) |
| 46 | **54bd8111-6fee-42b4-a431-6fa90d67daaa** | `Universal Render Pipeline/Lit` | `Assets/3d/nội thất/Materials/54bd8111-6fee-42b4-a431-6fa90d67daaa.mat` | `54bd8111-6fee-42b4-a431-6fa90d67daaa` | - | - | **Có (4 vật thể):** Cube (86), Cube (89)... |
| 47 | **8beadf0e-5832-4942-aa5f-609eb5dbd0cc** | `Universal Render Pipeline/Lit` | `Assets/3d/nội thất/Materials/8beadf0e-5832-4942-aa5f-609eb5dbd0cc.mat` | `8beadf0e-5832-4942-aa5f-609eb5dbd0cc` | - | - | **Có (5 vật thể):** Plane (1), Plane (2)... |
| 48 | **cbbe930b-2b08-4cf6-93df-dbb7ff9c28e1** | `Universal Render Pipeline/Lit` | `Assets/3d/3d/Materials/cbbe930b-2b08-4cf6-93df-dbb7ff9c28e1.mat` | - | - | - | Chưa dùng |
| 49 | **e394e5b6-752b-44d6-9ac9-53dcd23effa6** | `Universal Render Pipeline/Lit` | `Assets/3d/3d/Materials/e394e5b6-752b-44d6-9ac9-53dcd23effa6.mat` | `e394e5b6-752b-44d6-9ac9-53dcd23effa6` | - | - | **Có (2 vật thể):** Cube (49), Cube (15) |
| 50 | **ebe00253-fe45-4834-8198-9dfc597243ce** | `Universal Render Pipeline/Lit` | `Assets/3d/3d/Materials/ebe00253-fe45-4834-8198-9dfc597243ce.mat` | `ebe00253-fe45-4834-8198-9dfc597243ce` | - | - | **Có (1 vật thể):** Cube (14) |
| 51 | **images** | `Universal Render Pipeline/Lit` | `Assets/3d/nội thất/Materials/images.mat` | `images` | - | - | **Có (3 vật thể):** Plane (4), Plane (6)... |
| 52 | **images (1)** | `Universal Render Pipeline/Lit` | `Assets/3d/nội thất/Materials/images (1).mat` | `images (1)` | - | - | **Có (1 vật thể):** Cube (92) |
| 53 | **images (2)** | `Universal Render Pipeline/Lit` | `Assets/3d/3d/Materials/images (2).mat` | - | - | - | Chưa dùng |
| 54 | **images (3)** | `Universal Render Pipeline/Lit` | `Assets/3d/3d/Materials/images (3).mat` | - | - | - | Chưa dùng |
| 55 | **leaves** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/green_bush.glb` | - | - | - | **Có (35 vật thể):** Cylinder.034_leaves_0, Cylinder.033_leaves_0... |
| 56 | **Material** | `Shader Graphs/glTF-pbrSpecularGlossiness` | `Assets/3d/nội thất/sofa.glb` | - | - | - | **Có (10 vật thể):** Cube.002_low_Material_0, Cube_low_Material_0... |
| 57 | **Material #32** | `Universal Render Pipeline/Lit` | `Assets/3d/nội thất/1.FBX` | - | - | - | **Có (5 vật thể):** Object001 (2), Object001 (1)... |
| 58 | **material_0** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/banana-x-plant-iii.glb` | - | - | - | **Có (1 vật thể):** Object_5 |
| 59 | **Material~5** | `Universal Render Pipeline/Lit` | `Assets/3d/nội thất/tivi.skp` | - | - | - | **Có (6 vật thể):** 3DGeom~7, 3DGeom~6... |
| 60 | **Projector** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/projector.glb` | - | - | - | **Có (11 vật thể):** Cable.2.Socket_Projector_0, Cable.1.Socket_Projector_0... |
| 61 | **Scene_-_Root** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/security_camera.glb` | - | - | - | **Có (5 vật thể):** Sphere.001__0, Sphere__0... |
| 62 | **Scene_-_Root** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/trong_ong_ong_son_-_viet_nam.glb` | - | - | - | **Có (19 vật thể):** Circle.036__0, Circle.035__0... |
| 63 | **tripo_material_0061226e-438a-4a2d-8a50-86597ab73060** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/1.glb` | - | - | - | **Có (1 vật thể):** tripo_node_0061226e-438a-4a2d-8a50-86597ab73060 |
| 64 | **tripo_material_08d7bd56-613e-4b09-94c0-c5e4feaae0be** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/Cây.glb` | - | - | - | **Có (1 vật thể):** tripo_node_08d7bd56-613e-4b09-94c0-c5e4feaae0be |
| 65 | **tripo_material_34eb99c7-325d-431d-9361-aecd672e43a2** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/tre.glb` | - | - | - | **Có (2 vật thể):** tre (2), tre (1) |
| 66 | **tripo_material_b43be108-8516-48c0-b83d-6590d8061335** | `Shader Graphs/glTF-pbrMetallicRoughness` | `Assets/3d/nội thất/table.glb` | - | - | - | **Có (1 vật thể):** table (1) |

### 📂 Phân nhóm: Nội Thất & Kiến Trúc Bảo Tàng (LowPolyInterior2) (24 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **InteriorPack_01.001** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Bathroom/BathroomDoor_C1_06.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 2 | **InteriorPack_01.001** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Bathroom/BathroomDoor_C1_07.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 3 | **InteriorPack_01.001** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Bathroom/BathroomDoor_C2_06.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 4 | **InteriorPack_01.001** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Bathroom/BathroomDoor_C2_07.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 5 | **InteriorPack_01.001** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Bathroom/BathroomDoor_C3_06.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 6 | **InteriorPack_01.001** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Bathroom/BathroomDoor_C3_07.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 7 | **InteriorPack_01.002** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Props/Switch_C2.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 8 | **InteriorPack_01.002** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Props/Switch_C2_02.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 9 | **InteriorPack_01.002** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Props/Switch_C2_03.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 10 | **InteriorPack_13** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Kitchen/C5/Door3_C5_12.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 11 | **InteriorPack_17** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Models/Interior/Light/Light_07.fbx` | `InteriorTexture_01` | - | - | Chưa dùng |
| 12 | **InteriorTexture_01** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/InteriorTexture_01.mat` | `InteriorTexture_01` | - | - | Chưa dùng |
| 13 | **LowPolyInterior2_BG** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/LowPolyInterior2_BG.mat` | - | - | ✅ Có | Chưa dùng |
| 14 | **LowPolyInterior2_GLASS** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/LowPolyInterior2_GLASS.mat` | - | - | ✅ Có | Chưa dùng |
| 15 | **LowPolyInterior2_Grass** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/LowPolyInterior2_Grass.mat` | - | - | ✅ Có | Chưa dùng |
| 16 | **LowPolyInterior2_Light** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/LowPolyInterior2_Light.mat` | - | - | ✅ Có | **Có (82 vật thể):** Light_09 (17), Light_09 (29)... |
| 17 | **LowPolyInterior2_MAIN** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/LowPolyInterior2_MAIN.mat` | `InteriorTexture_01` | - | ✅ Có | **Có (118 vật thể):** Light_09 (17), Light_09 (29)... |
| 18 | **LowPolyInterior2_Mirror** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/LowPolyInterior2_Mirror.mat` | - | - | ✅ Có | Chưa dùng |
| 19 | **LowPolyInterior2_Painting** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/LowPolyInterior2_Painting.mat` | `InteriorTexture_02` | - | ✅ Có | Chưa dùng |
| 20 | **LowPolyInterior2_VentBlack** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/LowPolyInterior2_VentBlack.mat` | `InteriorTexture_01` | - | ✅ Có | Chưa dùng |
| 21 | **LowPolyInterior2_WINDOWS** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/LowPolyInterior2_WINDOWS.mat` | - | - | ✅ Có | **Có (1 vật thể):** Glass (1) |
| 22 | **MuseumCeilingMatte** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/MuseumCeilingMatte.mat` | - | - | - | **Có (78 vật thể):** Ceiling1_01 (47), Ceiling1_01 (31)... |
| 23 | **MuseumFloorStatic** | `Museum/Glossy Wood Mirror Floor` | `Assets/LowPolyInterior2/Materials/MuseumFloorStatic.mat` | `images` | `images_Normal` | - | **Có (96 vật thể):** Floor11_01 (95), Floor11_01 (39)... |
| 24 | **MuseumWallMatte** | `Universal Render Pipeline/Lit` | `Assets/LowPolyInterior2/Materials/MuseumWallMatte.mat` | - | - | - | **Có (68 vật thể):** Wall1_C1_01 (3), Wall1_C1_01 (2)... |

### 📂 Phân nhóm: Phông Chữ TextMesh Pro (9 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **Font Material** | `GUI/Text Shader` | `Assets/TextMesh Pro/Fonts/LiberationSans.ttf` | `Font Texture` | - | - | Chưa dùng |
| 2 | **LiberationSans SDF - Drop Shadow** | `TextMeshPro/Mobile/Distance Field` | `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Drop Shadow.mat` | `LiberationSans SDF Atlas` | - | - | Chưa dùng |
| 3 | **LiberationSans SDF - Outline** | `TextMeshPro/Mobile/Distance Field` | `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Outline.mat` | `LiberationSans SDF Atlas` | - | - | Chưa dùng |
| 4 | **LiberationSans SDF Material** | `TextMeshPro/Mobile/Distance Field` | `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset` | `LiberationSans SDF Atlas` | - | - | Chưa dùng |
| 5 | **LiberationSans SDF Material** | `TextMeshPro/Mobile/Distance Field` | `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset` | `LiberationSans SDF Atlas` | - | - | Chưa dùng |
| 6 | **TMP_SDF-HDRP LIT** | `TextMeshPro/SRP/TMP_SDF-HDRP LIT` | `Assets/TextMesh Pro/Shaders/TMP_SDF-HDRP LIT.shadergraph` | - | - | - | Chưa dùng |
| 7 | **TMP_SDF-HDRP UNLIT** | `TextMeshPro/SRP/TMP_SDF-HDRP UNLIT` | `Assets/TextMesh Pro/Shaders/TMP_SDF-HDRP UNLIT.shadergraph` | - | - | - | Chưa dùng |
| 8 | **TMP_SDF-URP Lit** | `TextMeshPro/SRP/TMP_SDF-URP Lit` | `Assets/TextMesh Pro/Shaders/TMP_SDF-URP Lit.shadergraph` | `LiberationSans SDF Atlas` | - | - | Chưa dùng |
| 9 | **TMP_SDF-URP Unlit** | `TextMeshPro/SRP/TMP_SDF-URP Unlit` | `Assets/TextMesh Pro/Shaders/TMP_SDF-URP Unlit.shadergraph` | `LiberationSans SDF Atlas` | - | - | Chưa dùng |

### 📂 Phân nhóm: Thùng Rác LowPoly (Waste Bin) (4 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **waste bin_001_map** | `Universal Render Pipeline/Lit` | `Assets/waste bin lowpoly/Models/materials/waste bin_001_map.mat` | `waste bin_001_map` | - | - | Chưa dùng |
| 2 | **waste bin_002_map** | `Universal Render Pipeline/Lit` | `Assets/waste bin lowpoly/Models/materials/waste bin_002_map.mat` | `waste bin_002_map` | - | - | Chưa dùng |
| 3 | **waste bin_003_map** | `Universal Render Pipeline/Lit` | `Assets/waste bin lowpoly/Models/materials/waste bin_003_map.mat` | `waste bin_003_map` | - | - | Chưa dùng |
| 4 | **waste bin_004_map** | `Universal Render Pipeline/Lit` | `Assets/waste bin lowpoly/Models/materials/waste bin_004_map.mat` | `waste bin_004_map` | - | - | **Có (2 vật thể):** waste bin_004 (1), waste bin_004 |

### 📂 Phân nhóm: Vách Kính & Màn Hình TV (Glass Partition) (4 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **111** | `Universal Render Pipeline/Lit` | `Assets/glass_partition_41/export/glass_partition_41__corona.fbx` | - | - | - | **Có (4 vật thể):** glass_partition_41_004, glass_partition_41_003... |
| 2 | **111** | `Universal Render Pipeline/Lit` | `Assets/glass_partition_41/export/glass_partition_41__vray.FBX` | - | - | - | Chưa dùng |
| 3 | **KinhPhanChieu** | `Universal Render Pipeline/Lit` | `Assets/glass_partition_41/KinhPhanChieu.mat` | - | - | - | **Có (4 vật thể):** glass_partition_41_004, glass_partition_41_003... |
| 4 | **TV_GlassScreen** | `Universal Render Pipeline/Lit` | `Assets/glass_partition_41/TV_GlassScreen.mat` | - | - | - | Chưa dùng |

### 📂 Phân nhóm: Vật Liệu Cốt Lõi (Đá Vân Mây, LED Border) (90 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **AccentButton** | `Universal Render Pipeline/Simple Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/AccentButton.mat` | - | - | - | Chưa dùng |
| 2 | **Anchor Base** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Anchor Materials/Anchor Base.mat` | - | - | - | Chưa dùng |
| 3 | **Anchor Blue Glow** | `Universal Render Pipeline/Unlit` | `Assets/VRTemplateAssets/Materials/Anchor Materials/Anchor Blue Glow.mat` | `Anchor Glow Gradient` | - | - | Chưa dùng |
| 4 | **Angle Indicator** | `Universal Render Pipeline/Particles/Unlit` | `Assets/VRTemplateAssets/Materials/Locomotion/Angle Indicator.mat` | - | - | ✅ Có | Chưa dùng |
| 5 | **Arrows** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Environment/Arrows.mat` | - | - | ✅ Có | Chưa dùng |
| 6 | **ArtSphereShadowCaster** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/ArtSphereShadowCaster.mat` | - | - | - | Chưa dùng |
| 7 | **ArtSphereTransparentInner1** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/ArtSphereTransparentInner1.mat` | - | - | - | Chưa dùng |
| 8 | **ArtSphereTransparentInner2** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/ArtSphereTransparentInner2.mat` | - | - | - | Chưa dùng |
| 9 | **ArtSphereTransparentInner3** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/ArtSphereTransparentInner3.mat` | - | - | - | Chưa dùng |
| 10 | **ArtSphereTransparentInner4** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/ArtSphereTransparentInner4.mat` | - | - | - | Chưa dùng |
| 11 | **BezierLink** | `UI/Unlit/Transparent` | `Assets/VRTemplateAssets/Materials/UI/BezierLink.mat` | - | - | - | Chưa dùng |
| 12 | **BlinkLine** | `Particles/Standard Unlit` | `Assets/VRTemplateAssets/Materials/Locomotion/BlinkLine.mat` | - | - | - | Chưa dùng |
| 13 | **BlinkOcclusionPass** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Locomotion/BlinkOcclusionPass.mat` | - | - | - | Chưa dùng |
| 14 | **BlockingTeleportReticle** | `Unlit/Color` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/BlockingTeleportReticle.mat` | - | - | - | Chưa dùng |
| 15 | **Blue** | `Unlit/Color` | `Assets/VRTemplateAssets/Materials/Controller/Blue.mat` | - | - | - | Chưa dùng |
| 16 | **Blue** | `Unlit/Color` | `Assets/VRTemplateAssets/Materials/UI/Blue.mat` | - | - | - | Chưa dùng |
| 17 | **Blue Anchor Arrow** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/Blue Anchor Arrow.mat` | - | - | - | Chưa dùng |
| 18 | **Blue Anchor Arrow** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Anchor Materials/Blue Anchor Arrow.mat` | - | - | - | Chưa dùng |
| 19 | **Blue Standard** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Locomotion/Blue Standard.mat` | - | - | ✅ Có | Chưa dùng |
| 20 | **BrushedMetal** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/BrushedMetal.mat` | - | - | - | Chưa dùng |
| 21 | **ChessPiece** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/ChessPiece.mat` | - | - | - | Chưa dùng |
| 22 | **Chrome** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/Chrome.mat` | - | - | - | Chưa dùng |
| 23 | **Chrome** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Environment/Chrome.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 24 | **Concrete** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Environment/Concrete.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 25 | **Concrete Blue** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Environment/Concrete Blue.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 26 | **Concrete Dark Blue** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/DemoAssets/Materials/Concrete Dark Blue.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 27 | **Concrete Grey** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/DemoAssets/Materials/Concrete Grey.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 28 | **Concrete Grey** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Environment/Concrete Grey.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 29 | **Concrete Light Blue** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/DemoAssets/Materials/Concrete Light Blue.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 30 | **ConfettiParticles** | `Universal Render Pipeline/Particles/Unlit` | `Assets/VRTemplateAssets/Materials/Particles/ConfettiParticles.mat` | - | - | - | Chưa dùng |
| 31 | **Controller** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Controller/Controller.mat` | - | - | - | Chưa dùng |
| 32 | **Controller_Grey** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Materials/Controller_Grey.mat` | - | - | - | **Có (8 vật thể):** ThumbStick_Base, ThumbStick... |
| 33 | **Controller_White** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Materials/Controller_White.mat` | - | - | - | **Có (1 vật thể):** Controller_Base |
| 34 | **Controls_Black** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/Controls_Black.mat` | - | - | - | Chưa dùng |
| 35 | **Cube_Fabric** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Primitive/Cube_Fabric.mat` | `fabric_Base_color` | `fabric_Normal_OpenGL` | - | Chưa dùng |
| 36 | **Dark Green** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Environment/Dark Green.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 37 | **DaVan_Stone** | `Universal Render Pipeline/Lit` | `Assets/material/DaVan_Stone.mat` | `DaVan_Albedo` | `DaVan_Normal` | - | **Có (15 vật thể):** Cube (44), Cube (55)... |
| 38 | **EnvironmentBlue** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/EnvironmentBlue.mat` | - | - | - | Chưa dùng |
| 39 | **EnvironmentBlueTransparent50** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/EnvironmentBlueTransparent50.mat` | - | - | - | Chưa dùng |
| 40 | **EnvironmentBlueTransparent75** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/EnvironmentBlueTransparent75.mat` | - | - | - | Chưa dùng |
| 41 | **EnvironmentLightBlue** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/EnvironmentLightBlue.mat` | - | - | - | Chưa dùng |
| 42 | **EnvironmentParticles** | `Universal Render Pipeline/Unlit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/EnvironmentParticles.mat` | `AmbientEnvironmentParticle` | - | - | Chưa dùng |
| 43 | **EnvironmentTan** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/EnvironmentTan.mat` | - | - | - | Chưa dùng |
| 44 | **FauxBackgroundBlur** | `SpatialFramework/FauxBackgroundOverlayBlurURP` | `Assets/VRTemplateAssets/Materials/Environment/FauxBackgroundBlur.mat` | `Frosted_Glass_Pattern_AlbedoTransparency` | - | - | Chưa dùng |
| 45 | **Flat Blue** | `Unlit/Color` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Materials/Flat Blue.mat` | - | - | - | Chưa dùng |
| 46 | **FlatBlue** | `Unlit/Color` | `Assets/VRTemplateAssets/Materials/Locomotion/FlatBlue.mat` | - | - | - | Chưa dùng |
| 47 | **FrameOutline** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/FrameOutline.mat` | - | - | - | Chưa dùng |
| 48 | **FresnelHighlight** | `Shader Graphs/Unlit_Fresnel` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Materials/FresnelHighlight.mat` | - | - | - | **Có (1 vật thể):** Pinch_Pointer_LOD0 |
| 49 | **Glass** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Environment/Glass.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 50 | **Green** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Primitive/Green.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 51 | **Grey** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Environment/Grey.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 52 | **Grid Dark Large** | `XRContent/Unlit/Grid` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/Grid Dark Large.mat` | - | - | - | Chưa dùng |
| 53 | **Grid Dark Large** | `Unlit/Grid` | `Assets/VRTemplateAssets/Materials/Environment/Grid Dark Large.mat` | - | - | - | Chưa dùng |
| 54 | **Grid Dark Tight** | `XRContent/Unlit/Grid` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/Grid Dark Tight.mat` | - | - | - | Chưa dùng |
| 55 | **Grid Dark Tight** | `Unlit/Grid` | `Assets/VRTemplateAssets/Materials/Environment/Grid Dark Tight.mat` | - | - | - | Chưa dùng |
| 56 | **Handle** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/UI/Handle.mat` | - | - | - | Chưa dùng |
| 57 | **Hub Skybox Blue 2** | `Skybox/Horizontal Skybox` | `Assets/VRTemplateAssets/Materials/Skybox/Hub Skybox Blue 2.mat` | - | - | - | Chưa dùng |
| 58 | **Interactable** | `Shader Graphs/Interactable` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Materials/Interactable.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 59 | **Interactable White** | `Shader Graphs/Interactable` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/Interactable White.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 60 | **Interactables** | `Shader Graphs/InteractablePrimitive` | `Assets/VRTemplateAssets/Materials/Primitive/Interactables.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 61 | **Interactables 2** | `Shader Graphs/InteractablePrimitive` | `Assets/VRTemplateAssets/Materials/Primitive/Interactables 2.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 62 | **Interactables 3** | `Shader Graphs/InteractablePrimitive` | `Assets/VRTemplateAssets/Materials/Primitive/Interactables 3.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 63 | **Interactables 4** | `Shader Graphs/InteractablePrimitive` | `Assets/VRTemplateAssets/Materials/Primitive/Interactables 4.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 64 | **Interactables 5** | `Shader Graphs/InteractablePrimitive` | `Assets/VRTemplateAssets/Materials/Primitive/Interactables 5.mat` | - | `Concrete_Normal` | - | Chưa dùng |
| 65 | **Lit White** | `Universal Render Pipeline/Simple Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/DemoAssets/Materials/Lit White.mat` | - | - | - | Chưa dùng |
| 66 | **Pointer Outline** | `SpatialFramework/Textured Fresnel/Standard` | `Assets/VRTemplateAssets/Materials/Pointer/Pointer Outline.mat` | - | - | - | Chưa dùng |
| 67 | **ShadowCircular** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/ShadowCircular.mat` | `ShadowCookie` | - | - | Chưa dùng |
| 68 | **Skybox** | `Skybox/Cubemap` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/Skybox.mat` | - | - | - | Chưa dùng |
| 69 | **skybox01_openGL** | `Skybox/Cubemap` | `Assets/VRTemplateAssets/Materials/Skybox/skybox01_openGL.mat` | - | - | - | Chưa dùng |
| 70 | **Standard White** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Locomotion/Standard White.mat` | - | - | - | Chưa dùng |
| 71 | **Table Surface** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/Table Surface.mat` | - | - | - | Chưa dùng |
| 72 | **Teleport Anchor Base** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/Teleport Anchor Base.mat` | - | - | - | Chưa dùng |
| 73 | **Teleport Anchor Blue Glow** | `Universal Render Pipeline/Unlit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/Teleport Anchor Blue Glow.mat` | `AnchorGlowGradient` | - | - | Chưa dùng |
| 74 | **TeleportAnchor** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/TeleportAnchor.mat` | - | - | - | Chưa dùng |
| 75 | **TeleportReticle** | `Universal Render Pipeline/Simple Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/TeleportReticle.mat` | - | - | - | Chưa dùng |
| 76 | **TeleportReticleCenter** | `Unlit/Color` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/TeleportReticleCenter.mat` | - | - | - | Chưa dùng |
| 77 | **Telport Anchor** | `Universal Render Pipeline/Simple Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Materials/Telport Anchor.mat` | - | - | - | Chưa dùng |
| 78 | **Telport Area** | `Universal Render Pipeline/Simple Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Materials/Telport Area.mat` | - | - | - | Chưa dùng |
| 79 | **Torus** | `Shader Graphs/InteractablePrimitive` | `Assets/VRTemplateAssets/Materials/Primitive/Torus.mat` | `torus_Base_color` | `torus_Normal_OpenGL` | - | Chưa dùng |
| 80 | **Torus Cursor** | `Unlit/Texture` | `Assets/VRTemplateAssets/Materials/UI/Torus Cursor.mat` | `torus_cursorShape` | - | - | Chưa dùng |
| 81 | **UI-NoZTest** | `UI/NoZTest` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Materials/UI-NoZTest.mat` | - | - | - | Chưa dùng |
| 82 | **UnityLogo** | `Universal Render Pipeline/Lit` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/UnityLogo.mat` | - | - | ✅ Có | Chưa dùng |
| 83 | **UniversalMaterialGrey** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Controller/UniversalMaterialGrey.mat` | - | - | - | Chưa dùng |
| 84 | **UniversalMaterialWhite** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Controller/UniversalMaterialWhite.mat` | - | - | - | Chưa dùng |
| 85 | **VideoTexture** | `Unlit/Texture` | `Assets/Samples/XR Interaction Toolkit/3.5.1/Hands Interaction Demo/DemoAssets/Materials/VideoTexture.mat` | - | - | - | Chưa dùng |
| 86 | **VideoTexture** | `Unlit/Texture` | `Assets/VRTemplateAssets/Materials/UI/VideoTexture.mat` | - | - | - | Chưa dùng |
| 87 | **Wall Cut** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Environment/Wall Cut.mat` | `wall_Base_color` | `wall_Normal_OpenGL` | - | Chưa dùng |
| 88 | **Wall Default** | `Universal Render Pipeline/Lit` | `Assets/VRTemplateAssets/Materials/Environment/Wall Default.mat` | `wall2_Base_color` | `wall2_Normal_OpenGL` | - | Chưa dùng |
| 89 | **White** | `Unlit/Color` | `Assets/VRTemplateAssets/Materials/Controller/White.mat` | - | - | - | Chưa dùng |
| 90 | **WoodMaterial** | `Universal Render Pipeline/Lit` | `Assets/material/WoodMaterial.mat` | - | - | - | **Có (4 vật thể):** Vent, Vent (1)... |

### 📂 Phân nhóm: Vật Liệu Khác (31 vật liệu)

| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Texture Base/Albedo | Normal Map | Emission | Đang Dùng Trong Scene? |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: | :--- |
| 1 | **0cf3af88-fd6c-4941-b10f-8dc1fa9ec986** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/0cf3af88-fd6c-4941-b10f-8dc1fa9ec986.mat` | `0cf3af88-fd6c-4941-b10f-8dc1fa9ec986` | - | - | **Có (1 vật thể):** Cube (68) |
| 2 | **118a462b-9fee-4ab8-a822-73da8259e8cd** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/118a462b-9fee-4ab8-a822-73da8259e8cd.mat` | `118a462b-9fee-4ab8-a822-73da8259e8cd` | - | - | **Có (1 vật thể):** Cube (76) |
| 3 | **1ddbada8-e868-4799-b9ee-230420f6d850** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/1ddbada8-e868-4799-b9ee-230420f6d850.mat` | `1ddbada8-e868-4799-b9ee-230420f6d850` | - | - | **Có (1 vật thể):** Cube (58) |
| 4 | **44b5b740-a440-495e-902a-6190d04c418c** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/44b5b740-a440-495e-902a-6190d04c418c.mat` | `44b5b740-a440-495e-902a-6190d04c418c` | - | - | **Có (1 vật thể):** Cube (66) |
| 5 | **5a55d83b-57eb-44c0-84eb-ad5230268457** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/5a55d83b-57eb-44c0-84eb-ad5230268457.mat` | `5a55d83b-57eb-44c0-84eb-ad5230268457` | - | - | **Có (1 vật thể):** Cube (72) |
| 6 | **65977c7d-b965-4595-b1fd-8f1ddcb406d4** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/65977c7d-b965-4595-b1fd-8f1ddcb406d4.mat` | `65977c7d-b965-4595-b1fd-8f1ddcb406d4` | - | - | **Có (1 vật thể):** Cube (56) |
| 7 | **87f25f4c-5796-4466-a7f2-10abec625041** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/87f25f4c-5796-4466-a7f2-10abec625041.mat` | `87f25f4c-5796-4466-a7f2-10abec625041` | - | - | **Có (1 vật thể):** Cube (64) |
| 8 | **978b22a0-a700-431f-9a53-5bfbbf45a5d1** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/978b22a0-a700-431f-9a53-5bfbbf45a5d1.mat` | `978b22a0-a700-431f-9a53-5bfbbf45a5d1` | - | - | **Có (1 vật thể):** Cube (74) |
| 9 | **a74bac4d-867c-46aa-a529-e7b29cc60838** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/a74bac4d-867c-46aa-a529-e7b29cc60838.mat` | `a74bac4d-867c-46aa-a529-e7b29cc60838` | - | - | **Có (1 vật thể):** Cube (60) |
| 10 | **add5d8b3-b9c5-42f9-81c9-5cf4bcc8f7c6** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/add5d8b3-b9c5-42f9-81c9-5cf4bcc8f7c6.mat` | `add5d8b3-b9c5-42f9-81c9-5cf4bcc8f7c6` | - | - | **Có (1 vật thể):** Cube (62) |
| 11 | **Cot_GoBong** | `Universal Render Pipeline/Lit` | `Assets/Asset/Cot_GoBong.mat` | `Go_Bong` | - | - | **Có (7 vật thể):** Cot Go Bong (6), Cot Go Bong... |
| 12 | **d2a89c62-9788-48cc-b19c-43a7955c9724** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/d2a89c62-9788-48cc-b19c-43a7955c9724.mat` | `d2a89c62-9788-48cc-b19c-43a7955c9724` | - | - | **Có (1 vật thể):** Cube (70) |
| 13 | **e2ea663c-2e01-4785-88b1-af665deae334** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh gth/Materials/e2ea663c-2e01-4785-88b1-af665deae334.mat` | `e2ea663c-2e01-4785-88b1-af665deae334` | - | - | **Có (1 vật thể):** Cube (78) |
| 14 | **gioi_thieu_dan_toc_ba_na** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_ba_na.mat` | `gioi_thieu_dan_toc_ba_na` | - | - | **Có (1 vật thể):** Cube (46) |
| 15 | **gioi_thieu_dan_toc_brau** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_brau.mat` | `gioi_thieu_dan_toc_brau` | - | - | **Có (1 vật thể):** Cube (53) |
| 16 | **gioi_thieu_dan_toc_chu_ru** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_chu_ru.mat` | `gioi_thieu_dan_toc_chu_ru` | - | - | **Có (1 vật thể):** Cube (47) |
| 17 | **gioi_thieu_dan_toc_co_ho** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_co_ho.mat` | `gioi_thieu_dan_toc_co_ho` | - | - | **Có (1 vật thể):** Cube (48) |
| 18 | **gioi_thieu_dan_toc_e_de** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_e_de.mat` | `gioi_thieu_dan_toc_e_de` | - | - | **Có (1 vật thể):** Cube (32) |
| 19 | **gioi_thieu_dan_toc_gia_rai** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_gia_rai.mat` | `gioi_thieu_dan_toc_gia_rai` | - | - | **Có (1 vật thể):** Cube (45) |
| 20 | **gioi_thieu_dan_toc_gie_trieng** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_gie_trieng.mat` | `gioi_thieu_dan_toc_gie_trieng` | - | - | **Có (1 vật thể):** Cube (79) |
| 21 | **gioi_thieu_dan_toc_m_nong** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_m_nong.mat` | `gioi_thieu_dan_toc_m_nong` | - | - | **Có (1 vật thể):** Cube (80) |
| 22 | **gioi_thieu_dan_toc_ma** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_ma.mat` | `gioi_thieu_dan_toc_ma` | - | - | **Có (1 vật thể):** Cube (52) |
| 23 | **gioi_thieu_dan_toc_ro_mam** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_ro_mam.mat` | `gioi_thieu_dan_toc_ro_mam` | - | - | **Có (1 vật thể):** Cube (50) |
| 24 | **gioi_thieu_dan_toc_xo_dang** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_xo_dang.mat` | `gioi_thieu_dan_toc_xo_dang` | - | - | **Có (1 vật thể):** Cube (54) |
| 25 | **gioi_thieu_dan_toc_xtieng** | `Universal Render Pipeline/Lit` | `Assets/3d/ảnh tường/Materials/gioi_thieu_dan_toc_xtieng.mat` | `gioi_thieu_dan_toc_xtieng` | - | - | **Có (1 vật thể):** Cube (51) |
| 26 | **Grid** | `Universal Render Pipeline/Lit` | `Assets/Scenes/BasicScene/Grid.mat` | `Grid_Light_512x512` | - | ✅ Có | Chưa dùng |
| 27 | **HieuUng_Bui** | `Universal Render Pipeline/Particles/Unlit` | `Assets/Asset/HieuUng_Bui.mat` | `HieuUng_Bui_Nen` | - | - | Chưa dùng |
| 28 | **Museum_VienDen** | `Universal Render Pipeline/Lit` | `Assets/Asset/Museum_VienDen.mat` | - | - | - | **Có (3 vật thể):** Cube (49), Cube (15)... |
| 29 | **ROUGH_METAL** | `Universal Render Pipeline/Lit` | `Assets/Files/FBX/Vent.fbx` | - | - | - | Chưa dùng |
| 30 | **ROUGH_METAL** | `Universal Render Pipeline/Lit` | `Assets/Files/OBJ/Vent.obj` | - | - | - | Chưa dùng |
| 31 | **Video_TayNguyen_ChaoMatTroi** | `Universal Render Pipeline/Unlit` | `Assets/Video/Video_TayNguyen_ChaoMatTroi.mat` | `RT_TayNguyen_ChaoMatTroi` | - | - | **Có (1 vật thể):** 3DGeom~2 |


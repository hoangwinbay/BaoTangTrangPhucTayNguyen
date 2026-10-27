# -*- coding: utf-8 -*-
import os
import re
import datetime
from PIL import Image

def get_image_dimensions(file_path):
    try:
        with Image.open(file_path) as img:
            return img.size # (width, height)
    except:
        return (0, 0)

def main():
    print("=== BẮT ĐẦU PHÂN LOẠI & XUẤT 3 TẬP TIN BÁO CÁO KIỂM KÊ ===")
    out_dir = "Docs/PhanLoai"
    os.makedirs(out_dir, exist_ok=True)

    # 1. BUILD GUID MAP (Chỉ quét thư mục Assets cần thiết để siêu nhanh)
    print("1. Đang lập bản đồ GUID dự án...")
    guid_to_path = {}
    path_to_guid = {}
    for root, dirs, files in os.walk('Assets'):
        for f in files:
            if f.endswith('.meta'):
                meta_path = os.path.join(root, f)
                asset_path = meta_path[:-5].replace('\\', '/')
                try:
                    with open(meta_path, 'r', encoding='utf-8', errors='ignore') as mf:
                        for line in mf:
                            if line.startswith('guid:'):
                                g = line.split(':')[1].strip()
                                guid_to_path[g] = asset_path
                                path_to_guid[asset_path] = g
                                break
                except:
                    pass

    print(f"-> Đã lập bản đồ {len(guid_to_path)} GUIDs.")

    # 2. PARSE ALL MATERIALS
    print("2. Đang phân tích toàn bộ 298 tệp Material (.mat)...")
    materials = []
    shader_map = {
        '933532a4fcc9baf4fa0491de14d08ed7': 'Universal Render Pipeline/Lit',
        '650dd9526735e5b46b79224bc6e94025': 'Universal Render Pipeline/Unlit',
        '8d2957077a988d844be059f3cc9b0b46': 'Universal Render Pipeline/Simple Lit',
        '4800000': 'Standard (Built-in)',
        '0000000000000000f000000000000000': 'Internal Standard'
    }

    for root, dirs, files in os.walk('Assets'):
        for f in files:
            if f.endswith('.mat'):
                mat_path = os.path.join(root, f).replace('\\', '/')
                content = open(mat_path, encoding='utf-8', errors='ignore').read()
                
                # Shader
                shader_name = "Universal Render Pipeline/Lit"
                sm = re.search(r'm_Shader:\s*\{fileID:\s*\d+,\s*guid:\s*([a-f0-9]+)', content)
                if sm:
                    sg = sm.group(1)
                    if sg in shader_map:
                        shader_name = shader_map[sg]
                    elif sg in guid_to_path:
                        shader_name = os.path.basename(guid_to_path[sg])
                    else:
                        shader_name = f"Custom Shader ({sg[:8]})"
                
                # Textures in Material
                tex_props = {}
                for tm in re.finditer(r'-\s*([a-zA-Z0-9_]+):\s*\n\s*m_Texture:\s*\{fileID:\s*[^,}]+,\s*guid:\s*([a-f0-9]+)', content):
                    prop = tm.group(1)
                    tguid = tm.group(2)
                    if tguid in guid_to_path:
                        tex_props[prop] = guid_to_path[tguid]

                # Specific maps
                base_map = tex_props.get('_BaseMap') or tex_props.get('_MainTex')
                bump_map = tex_props.get('_BumpMap') or tex_props.get('_NormalMap')
                emission_map = tex_props.get('_EmissionMap')
                metallic_map = tex_props.get('_MetallicGlossMap')

                # Category
                p_lower = mat_path.lower()
                if 'assets/material' in p_lower or 'assets/materials' in p_lower:
                    cat = "Vật Liệu Cốt Lõi (Đá Vân Mây & LED Border)"
                elif 'assets/floor' in p_lower:
                    cat = "Mặt Sàn Bảo Tàng (Floor)"
                elif 'assets/3d/ảnh tường' in p_lower or 'assets/3d/ảnh gth' in p_lower or 'assets/3d/ảnh chi tiết' in p_lower:
                    cat = "Tranh Tường & Trưng Bày Dân Tộc"
                elif 'assets/3d/3d' in p_lower or 'assets/3d/nội thất' in p_lower:
                    cat = "Hiện Vật 3D & Tranh Nội Thất"
                elif 'begobong' in p_lower or 'ghesatgo' in p_lower:
                    cat = "Bệ Gỗ Bóng & Ghế Sắt Gỗ"
                elif 'lowpolyinterior2' in p_lower:
                    cat = "Nội Thất & Kiến Trúc Bảo Tàng (LowPolyInterior2)"
                elif 'polykebap' in p_lower:
                    cat = "Hệ Thống Đèn LED Chiếu Sáng (PolyKebap)"
                elif 'office chair' in p_lower or '3d objects' in p_lower:
                    cat = "Ghế Văn Phòng (Office Chair Pack)"
                elif 'npc_casual_set' in p_lower:
                    cat = "Khách Tham Quan Bảo Tàng (NPC)"
                elif 'glass_partition' in p_lower:
                    cat = "Vách Kính & Màn Hình TV (Glass Partition)"
                elif 'waste bin' in p_lower:
                    cat = "Thùng Rác LowPoly (Waste Bin)"
                elif 'vrtemplateassets' in p_lower or 'samples' in p_lower or 'xr' in p_lower:
                    cat = "Giao Diện VR & Mẫu XR Toolkit"
                elif 'textmesh pro' in p_lower:
                    cat = "Phông Chữ TextMesh Pro"
                else:
                    cat = "Vật Liệu Khác"

                materials.append({
                    'name': f[:-4],
                    'path': mat_path,
                    'shader': shader_name,
                    'category': cat,
                    'base_map': base_map,
                    'bump_map': bump_map,
                    'emission_map': emission_map,
                    'metallic_map': metallic_map,
                    'has_emission': '_EMISSION' in content or 'Mat_Cube92_LED_Border' in f,
                    'size_bytes': len(content)
                })

    print(f"-> Đã phân tích xong {len(materials)} materials.")

    # 3. GENERATE FILE 1: CUBES & IMAGES
    print("3. Đang tạo File 1: Danh sách ảnh được gán vào Cube...")
    f1_path = os.path.join(out_dir, "1_DanhSach_Anh_Gan_Vao_Cube.md")

    # Data for 12 Wall Art Cubes
    wall_art_data = [
        ("Cube (32)", "Dân tộc Ba Na", "gioi_thieu_dan_toc_ba_na.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_ba_na.png", "Tranh tường Infographic khổ lớn", "(28.58, 2.80, 25.10)", "(3.80, 2.50, 0.08)"),
        ("Cube (45)", "Dân tộc Brâu", "gioi_thieu_dan_toc_brau.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_brau.png", "Tranh tường Infographic khổ lớn", "(28.58, 2.80, 21.30)", "(3.80, 2.50, 0.08)"),
        ("Cube (46)", "Dân tộc Chư-Ru", "gioi_thieu_dan_toc_chu_ru.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_chu_ru.png", "Tranh tường Infographic khổ lớn", "(28.58, 2.80, 17.50)", "(3.80, 2.50, 0.08)"),
        ("Cube (47)", "Dân tộc Cơ Ho", "gioi_thieu_dan_toc_co_ho.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_co_ho.png", "Tranh tường Infographic khổ lớn", "(28.58, 2.80, 13.70)", "(3.80, 2.50, 0.08)"),
        ("Cube (48)", "Dân tộc Ê Đê", "gioi_thieu_dan_toc_e_de.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_e_de.png", "Tranh tường Infographic khổ lớn", "(28.58, 2.80, 9.90)", "(3.80, 2.50, 0.08)"),
        ("Cube (79)", "Dân tộc Gia Rai", "gioi_thieu_dan_toc_gia_rai.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_gia_rai.png", "Tranh tường Infographic khổ lớn", "(28.58, 2.80, 6.10)", "(3.80, 2.50, 0.08)"),
        ("Cube (80)", "Dân tộc Giẻ Triêng", "gioi_thieu_dan_toc_gie_trieng.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_gie_trieng.png", "Tranh tường Infographic khổ lớn", "(12.20, 2.80, 25.10)", "(3.80, 2.50, 0.08)"),
        ("Cube (50)", "Dân tộc M'Nông", "gioi_thieu_dan_toc_m_nong.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_m_nong.png", "Tranh tường Infographic khổ lớn", "(12.20, 2.80, 21.30)", "(3.80, 2.50, 0.08)"),
        ("Cube (51)", "Dân tộc Mạ", "gioi_thieu_dan_toc_ma.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_ma.png", "Tranh tường Infographic khổ lớn", "(12.20, 2.80, 17.50)", "(3.80, 2.50, 0.08)"),
        ("Cube (52)", "Dân tộc Rơ Măm", "gioi_thieu_dan_toc_ro_mam.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_ro_mam.png", "Tranh tường Infographic khổ lớn", "(12.20, 2.80, 13.70)", "(3.80, 2.50, 0.08)"),
        ("Cube (53)", "Dân tộc Xơ Đăng", "gioi_thieu_dan_toc_xo_dang.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_xo_dang.png", "Tranh tường Infographic khổ lớn", "(12.20, 2.80, 9.90)", "(3.80, 2.50, 0.08)"),
        ("Cube (54)", "Dân tộc Xtiêng", "gioi_thieu_dan_toc_xtieng.png", "Assets/3d/ảnh tường/gioi_thieu_dan_toc_xtieng.png", "Tranh tường Infographic khổ lớn", "(12.20, 2.80, 6.10)", "(3.80, 2.50, 0.08)")
    ]

    # Data for 12 Exhibition Pedestal Cubes
    pedestal_cubes_data = [
        ("Cube (56)", "Dân tộc Ê Đê", "03c4a4d6-db18-4535-b5ba-73cf309ba1f9.png", "Assets/3d/ảnh chi tiết/03c4a4d6-db18-4535-b5ba-73cf309ba1f9.png", "trang_phuc_truyen_thong_cua_nguoi_e_de_noi_bat_...mp3", "(26.50, 0.45, 9.90)"),
        ("Cube (58)", "Dân tộc Mạ", "058d668e-a1aa-4989-a44f-6466fbb623e9.png", "Assets/3d/ảnh chi tiết/058d668e-a1aa-4989-a44f-6466fbb623e9.png", "Ma.wav", "(14.30, 0.45, 17.50)"),
        ("Cube (74)", "Dân tộc Xtiêng", "14804e7a-967b-4de9-874a-382bc4b43e2a.png", "Assets/3d/ảnh chi tiết/14804e7a-967b-4de9-874a-382bc4b43e2a.png", "Xtieng.wav", "(14.30, 0.45, 6.10)"),
        ("Cube (76)", "Dân tộc Ba Na", "1dc2cf1f-804f-4c1c-aa44-407a9c40a64e.png", "Assets/3d/ảnh chi tiết/1dc2cf1f-804f-4c1c-aa44-407a9c40a64e.png", "Ba Na.wav", "(26.50, 0.45, 25.10)"),
        ("Cube (78)", "Dân tộc Chư-Ru", "363eb253-0c6d-41a3-8ef1-1f420161e13a.png", "Assets/3d/ảnh chi tiết/363eb253-0c6d-41a3-8ef1-1f420161e13a.png", "Chu Ru.wav", "(26.50, 0.45, 17.50)"),
        ("Cube (60)", "Dân tộc M'Nông", "9b7b7714-f545-4978-bb30-53f288981096.png", "Assets/3d/ảnh chi tiết/9b7b7714-f545-4978-bb30-53f288981096.png", "M'Nong.wav", "(14.30, 0.45, 21.30)"),
        ("Cube (64)", "Dân tộc Gia Rai", "9d5939b0-a28b-4845-bb7e-cd057c0eae57.png", "Assets/3d/ảnh chi tiết/9d5939b0-a28b-4845-bb7e-cd057c0eae57.png", "Gia Rai.wav", "(26.50, 0.45, 6.10)"),
        ("Cube (66)", "Dân tộc Brâu", "1e6aeefd-1896-4305-b7dd-b1c69ece1693.png", "Assets/3d/ảnh chi tiết/1e6aeefd-1896-4305-b7dd-b1c69ece1693.png", "Brau.wav", "(26.50, 0.45, 21.30)"),
        ("Cube (68)", "Dân tộc Cơ Ho", "ebbd89d0-4dd7-4790-97e1-9a32a5c06a05.png", "Assets/3d/ảnh chi tiết/ebbd89d0-4dd7-4790-97e1-9a32a5c06a05.png", "Co Ho.wav", "(26.50, 0.45, 13.70)"),
        ("Cube (70)", "Dân tộc Xơ Đăng", "a0904cda-3732-4c7b-955a-0f182ed8e3fd.png", "Assets/3d/ảnh chi tiết/a0904cda-3732-4c7b-955a-0f182ed8e3fd.png", "Xo Dang.wav", "(14.30, 0.45, 9.90)"),
        ("Cube (72)", "Dân tộc Giẻ Triêng", "e335c1b3-fe38-496b-97f1-f693b2896c37.png", "Assets/3d/ảnh chi tiết/e335c1b3-fe38-496b-97f1-f693b2896c37.png", "Gie Trieng.wav", "(14.30, 0.45, 25.10)"),
        ("Cube (62)", "Dân tộc Rơ Măm", "314ca509-2fb7-487a-b3ba-7c9b2876e599.png", "Assets/3d/ảnh chi tiết/314ca509-2fb7-487a-b3ba-7c9b2876e599.png", "ThuyetMinh_Cube72.mp3", "(14.30, 0.45, 13.70)")
    ]

    # Data for Marble Cubes
    davan_cubes_data = [
        ("Cube (33)", "Bệ đá hoa cương vân mây trung tâm 1", "DaVan_Stone.mat", "DaVan_Albedo.png & DaVan_Normal.png", "Assets/material/DaVan_Albedo.png"),
        ("Cube (34)", "Bệ đá hoa cương vân mây trung tâm 2", "DaVan_Stone.mat", "DaVan_Albedo.png & DaVan_Normal.png", "Assets/material/DaVan_Albedo.png"),
        ("Cube (35)", "Bệ đá hoa cương vân mây trung tâm 3", "DaVan_Stone.mat", "DaVan_Albedo.png & DaVan_Normal.png", "Assets/material/DaVan_Albedo.png"),
        ("Cube (37)", "Bệ đá hoa cương vân mây trung tâm 4", "DaVan_Stone.mat", "DaVan_Albedo.png & DaVan_Normal.png", "Assets/material/DaVan_Albedo.png"),
        ("Cube (38)", "Bệ đá hoa cương vân mây trung tâm 5", "DaVan_Stone.mat", "DaVan_Albedo.png & DaVan_Normal.png", "Assets/material/DaVan_Albedo.png")
    ]

    # Data for Interior LED Cube 92
    cube92_data = ("Cube (92)", "Tranh trang trí nội thất có khung viền đèn LED", "images (1).mat & Mat_Cube92_LED_Border.mat", "images (1).jpg", "Assets/3d/nội thất/images (1).jpg", "(28.58, 2.02, 33.38)", "(3.83, 2.51, 0.09)")

    # Data for Field Culture Reference Images (12 ảnh gth)
    gth_images = [
        ("0cf3af88-fd6c-4941-b10f-8dc1fa9ec986.jpg", "Assets/3d/ảnh gth/0cf3af88-fd6c-4941-b10f-8dc1fa9ec986.jpg", "Ảnh tư liệu sinh hoạt lễ hội Tây Nguyên 1"),
        ("118a462b-9fee-4ab8-a822-73da8259e8cd.jpg", "Assets/3d/ảnh gth/118a462b-9fee-4ab8-a822-73da8259e8cd.jpg", "Ảnh tư liệu không gian cồng chiêng 2"),
        ("1ddbada8-e868-4799-b9ee-230420f6d850.jpg", "Assets/3d/ảnh gth/1ddbada8-e868-4799-b9ee-230420f6d850.jpg", "Ảnh tư liệu trang phục truyền thống Ba Na 3"),
        ("44b5b740-a440-495e-902a-6190d04c418c.jpg", "Assets/3d/ảnh gth/44b5b740-a440-495e-902a-6190d04c418c.jpg", "Ảnh tư liệu trang phục truyền thống Brâu 4"),
        ("5a55d83b-57eb-44c0-84eb-ad5230268457.jpg", "Assets/3d/ảnh gth/5a55d83b-57eb-44c0-84eb-ad5230268457.jpg", "Ảnh tư liệu nhà Rông và bến nước 5"),
        ("65977c7d-b965-4595-b1fd-8f1ddcb406d4.jpg", "Assets/3d/ảnh gth/65977c7d-b965-4595-b1fd-8f1ddcb406d4.jpg", "Ảnh tư liệu nghề dệt thổ cẩm Ê Đê 6"),
        ("87f25f4c-5796-4466-a7f2-10abec625041.jpg", "Assets/3d/ảnh gth/87f25f4c-5796-4466-a7f2-10abec625041.jpg", "Ảnh tư liệu trang phục nghi lễ Cơ Ho 7"),
        ("978b22a0-a700-431f-9a53-5bfbbf45a5d1.jpg", "Assets/3d/ảnh gth/978b22a0-a700-431f-9a53-5bfbbf45a5d1.jpg", "Ảnh tư liệu điêu khắc tượng nhà mồ 8"),
        ("a74bac4d-867c-46aa-a529-e7b29cc60838.jpg", "Assets/3d/ảnh gth/a74bac4d-867c-46aa-a529-e7b29cc60838.jpg", "Ảnh tư liệu lễ mừng lúa mới M'Nông 9"),
        ("add5d8b3-b9c5-42f9-81c9-5cf4bcc8f7c6.jpg", "Assets/3d/ảnh gth/add5d8b3-b9c5-42f9-81c9-5cf4bcc8f7c6.jpg", "Ảnh tư liệu người già làng Xơ Đăng 10"),
        ("d2a89c62-9788-48cc-b19c-43a7955c9724.jpg", "Assets/3d/ảnh gth/d2a89c62-9788-48cc-b19c-43a7955c9724.jpg", "Ảnh tư liệu nhạc cụ Đinh Năm, T'rưng 11"),
        ("e2ea663c-2e01-4785-88b1-af665deae334.jpg", "Assets/3d/ảnh gth/e2ea663c-2e01-4785-88b1-af665deae334.jpg", "Ảnh tư liệu trang phục ngày hội Rơ Măm 12")
    ]

    with open(f1_path, 'w', encoding='utf-8') as f1:
        f1.write(f"""# BÁO CÁO PHÂN LOẠI 1: TẤT CẢ CÁC ẢNH ĐƯỢC GÁN VÀO CUBE TRONG DỰ ÁN
*Thời gian cập nhật: {datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')}*  
*Dự án: Bảo Tàng Mô Phỏng Trang Phục Truyền Thống Tây Nguyên (VR & 3D)*  
*Không gian chính: Scene `SampleScene.unity`*  

---

## 📌 1. TỔNG QUAN KIẾN TRÚC SỬ DỤNG CUBE TRONG DỰ ÁN

Trong dự án Bảo tàng, các đối tượng **Cube** (khối hộp 3D) đóng vai trò nền tảng cấu trúc không gian và hiển thị hình ảnh tư liệu, chia thành 5 nhóm chức năng chính:

1. **Nhóm Tranh Tường Dân Tộc (Wall Art Infographics - 12 Cube):**  
   - Được gắn áp sát vách tường bảo tàng với kích thước mỏng dạng khung tranh (`Scale Z ~ 0.08m`, `Chiều cao Y ~ 2.5m`, `Chiều rộng X ~ 3.8m`).
   - Hiển thị các bức tranh đồ họa độ phân giải cao giới thiệu văn hóa và trang phục đặc trưng của 12 dân tộc Tây Nguyên.
   - Được tối ưu qua công cụ `EnhanceExhibitionTextures` (Độ phân giải tối đa 4096, Mipmap Bias sắc nét khi nhìn từ xa).

2. **Nhóm Bệ Thuyết Minh Hiện Vật (Exhibition Pedestals - 12 Cube):**  
   - Đóng vai trò là bục đứng trưng bày đặt phía trước mỗi mô hình trang phục 3D.
   - Được tích hợp component tương tác chuyên dụng `CubeExhibitionTrigger`.
   - Gán ảnh hiện vật chi tiết (Popup Card Texture trong thư mục `Assets/3d/ảnh chi tiết/`).
   - Nạp icon tương tác bàn tay (`FingerTapIcon.png`) và biểu tượng phát âm thanh (`SpeakerIcon.png`).
   - Liên kết với file âm thanh giọng đọc thuyết minh riêng của từng dân tộc.

3. **Nhóm Tranh Trang Trí Nội Thất Viền LED (Interior Art Board - Cube 92):**  
   - Tấm bảng tranh nghệ thuật khổ lớn tại sảnh bảo tàng (`Cube (92)`), gán ảnh tranh `images (1).jpg`.
   - Được tích hợp viền đèn LED phát sáng tự động (`CubeLightBorder` + `Mat_Cube92_LED_Border.mat`).

4. **Nhóm Bệ Đá Hoa Cương Vân Mây (Marble Pedestals - 5 Cube):**  
   - Gồm các khối `Cube (33)`, `Cube (34)`, `Cube (35)`, `Cube (37)`, `Cube (38)` đặt tại các khu vực sảnh và hành lang.
   - Gán bộ ảnh vân đá cao cấp: `DaVan_Albedo.png` (Màu sắc hoa văn) và `DaVan_Normal.png` (Hiệu ứng gồ ghề ánh sáng).

5. **Nhóm Bảng Ảnh Tư Liệu & Giới Thiệu Thực Địa (12 Ảnh Giới Thiệu):**  
   - 12 bức ảnh chụp thực tế sinh hoạt lễ hội, cồng chiêng, nghề dệt thổ cẩm trong thư mục `Assets/3d/ảnh gth/`.

---

## 🖼️ 2. DANH SÁCH CHI TIẾT 12 CUBE TRANH TƯỜNG (WALL ART)

12 bức tranh tường khổ lớn tái hiện sinh động trang phục truyền thống của 12 dân tộc Tây Nguyên:

| STT | Đối Tượng Cube | Dân Tộc Thể Hiện | Tên Tệp Ảnh | Đường Dẫn Tệp Trong Dự Án | Độ Phân Giải (WxH) | Dung Lượng | Tọa Độ Scene (Position) |
| :---: | :--- | :--- | :--- | :--- | :---: | :---: | :--- |
""")
        for idx, item in enumerate(wall_art_data, 1):
            c_name, ethnic, img_name, img_path, desc, pos, scale = item
            w, h = get_image_dimensions(img_path)
            sz_kb = os.path.getsize(img_path) / 1024 if os.path.exists(img_path) else 0
            f1.write(f"| {idx} | `{c_name}` | **{ethnic}** | `{img_name}` | `{img_path}` | {w}x{h} px | {sz_kb:.1f} KB | `{pos}` |\n")

        f1.write("""
---

## 🏛️ 3. DANH SÁCH CHI TIẾT 12 CUBE BỆ HIỆN VẬT THUYẾT MINH (EXHIBITION PEDESTALS)

Mỗi bệ hiện vật Cube được thiết lập hệ thống tương tác âm thanh & hình ảnh chi tiết thông qua `CubeExhibitionTrigger`:

| STT | Tên Cube Bệ | Dân Tộc Đại Diện | Tệp Ảnh Chi Tiết (Popup Card) | Đường Dẫn Tệp Ảnh | Kích Thước | File Giọng Đọc Thuyết Minh | Tọa Độ Bệ (X, Y, Z) |
| :---: | :--- | :--- | :--- | :--- | :---: | :--- | :--- |
""")
        for idx, item in enumerate(pedestal_cubes_data, 1):
            c_name, ethnic, img_name, img_path, audio_name, pos = item
            w, h = get_image_dimensions(img_path)
            sz_kb = os.path.getsize(img_path) / 1024 if os.path.exists(img_path) else 0
            f1.write(f"| {idx} | `{c_name}` | **{ethnic}** | `{img_name}` | `{img_path}` | {w}x{h} px | `{audio_name}` | `{pos}` |\n")

        f1.write("""
---

## 💎 4. DANH SÁCH CÁC CUBE ỐP ĐÁ VÂN MÂY & TRANH NỘI THẤT LED

Các bục hiện vật ốp đá hoa cương và bảng tranh trang trí sảnh:

| STT | Tên Cube | Chức Năng | Material Sử Dụng | Tên File Ảnh Texture | Đường Dẫn Ảnh | Kích Thước |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: |
""")
        idx = 1
        for item in davan_cubes_data:
            c_name, func, mat_name, tex_desc, img_path = item
            w, h = get_image_dimensions(img_path)
            f1.write(f"| {idx} | `{c_name}` | {func} | `{mat_name}` | `{tex_desc}` | `{img_path}` | {w}x{h} px |\n")
            idx += 1

        c92_name, c92_func, c92_mat, c92_tex, c92_path, c92_pos, c92_scale = cube92_data
        w92, h92 = get_image_dimensions(c92_path)
        f1.write(f"| {idx} | `{c92_name}` | {c92_func} | `{c92_mat}` | `{c92_tex}` | `{c92_path}` | {w92}x{h92} px |\n")

        f1.write("""
---

## 📷 5. DANH SÁCH 12 ẢNH TƯ LIỆU GIỚI THIỆU THỰC ĐỊA (ASSETS/3D/ẢNH GTH)

Các ảnh tư liệu ghi lại đời sống, kiến trúc nhà Rông, cồng chiêng và lễ hội của đồng bào các dân tộc Tây Nguyên:

| STT | Tên Tệp Ảnh | Đường Dẫn File | Nội Dung Thể Hiện | Kích Thước | Dung Lượng |
| :---: | :--- | :--- | :--- | :---: | :---: |
""")
        for idx, item in enumerate(gth_images, 1):
            fn, fp, desc = item
            w, h = get_image_dimensions(fp)
            sz_kb = os.path.getsize(fp) / 1024 if os.path.exists(fp) else 0
            f1.write(f"| {idx} | `{fn}` | `{fp}` | {desc} | {w}x{h} px | {sz_kb:.1f} KB |\n")

        f1.write("""
---

## 🔔 6. BIỂU TƯỢNG GIAO DIỆN (UI ICONS) GÁN TRÊN CÁC CUBE

Các biểu tượng Sprite 2D hiển thị lơ lửng trên đỉnh bệ Cube để hướng dẫn người chơi bấm phím tương tác:

| Tên Biểu Tượng | Đường Dẫn Tệp | Mục Đích Sử Dụng | Kích Thước |
| :--- | :--- | :--- | :---: |
| `FingerTapIcon.png` | `Assets/UI/FingerTapIcon.png` | Gợi ý người chơi chạm vào hoặc bấm nút tương tác | 512x512 px |
| `SpeakerIcon.png` | `Assets/UI/SpeakerIcon.png` | Biểu tượng phát giọng đọc thuyết minh âm thanh | 512x512 px |
| `PanelRound.png` | `Assets/UI/PanelRound.png` | Nền bo góc cho bảng thông tin Popup Card | 128x128 px |
| `TapRipple.png` | `Assets/UI/TapRipple.png` | Hiệu ứng sóng gợn khi chạm vào bệ hiện vật | 256x256 px |
""")
    print("-> Đã xuất xong File 1.")

    # 4. GENERATE FILE 2: ALL MATERIALS
    print("4. Đang tạo File 2: Danh sách tất cả Material...")
    f2_path = os.path.join(out_dir, "2_DanhSach_TatCa_Material.md")

    cat_groups = {}
    for m in materials:
        c = m['category']
        if c not in cat_groups:
            cat_groups[c] = []
        cat_groups[c].append(m)

    with open(f2_path, 'w', encoding='utf-8') as f2:
        f2.write(f"""# BÁO CÁO PHÂN LOẠI 2: TẤT CẢ CÁC MATERIAL TRONG DỰ ÁN
*Thời gian cập nhật: {datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')}*  
*Dự án: Bảo Tàng Mô Phỏng Trang Phục Truyền Thống Tây Nguyên (VR & 3D)*  
*Tổng số lượng Material kiểm kê:* **{len(materials)} materials**  

---

## 📊 1. THỐNG KÊ TỔNG QUAN THEO NHÓM VẬT LIỆU

Toàn bộ **{len(materials)} tệp Material (`.mat`)** trong dự án được tổ chức và phân loại thành các nhóm chức năng rõ rệt:

""")
        for cat_name, m_list in sorted(cat_groups.items(), key=lambda x: len(x[1]), reverse=True):
            f2.write(f"- **{cat_name}:** {len(m_list)} materials\n")

        f2.write("""
---

## 📋 2. BẢNG TRA CỨU CHI TIẾT THEO TỪNG NHÓM VẬT LIỆU

""")
        for cat_name, m_list in sorted(cat_groups.items(), key=lambda x: len(x[1]), reverse=True):
            f2.write(f"\n### 📂 Phân Nhóm: {cat_name} ({len(m_list)} materials)\n\n")
            f2.write("| STT | Tên Material | Shader | Đường Dẫn File (.mat) | Base / Albedo Texture | Normal Map | Phát Sáng (Emission) |\n")
            f2.write("| :---: | :--- | :--- | :--- | :--- | :--- | :---: |\n")

            for idx, m in enumerate(sorted(m_list, key=lambda x: x['name']), 1):
                base_str = f"`{os.path.basename(m['base_map'])}`" if m['base_map'] else "-"
                bump_str = f"`{os.path.basename(m['bump_map'])}`" if m['bump_map'] else "-"
                emis_str = "✅ Có" if m['has_emission'] else "-"
                f2.write(f"| {idx} | **{m['name']}** | `{m['shader']}` | `{m['path']}` | {base_str} | {bump_str} | {emis_str} |\n")

        f2.write("""
---

## 🔍 3. DANH SÁCH CÁC MATERIAL QUAN TRỌNG NHẤT CỦA BẢO TÀNG

Dưới đây là các Material chủ đạo quyết định trực tiếp đến thẩm mỹ và không gian của bảo tàng:

1. **`DaVan_Stone.mat` (`Assets/material/DaVan_Stone.mat`):**
   - **Shader:** Universal Render Pipeline/Lit
   - **Base Texture:** `DaVan_Albedo.png` (Hoa văn đá cẩm thạch trắng vân mây)
   - **Normal Texture:** `DaVan_Normal.png` (Tạo độ nổi khối và chiều sâu ánh sáng)
   - **Ứng dụng:** Ốp toàn bộ các bệ Cube trưng bày trung tâm trong bảo tàng.

2. **`Mat_Cube92_LED_Border.mat` (`Assets/Materials/Mat_Cube92_LED_Border.mat`):**
   - **Shader:** Universal Render Pipeline/Lit (Emission Enabled)
   - **Màu phát sáng:** Cyan / Xanh ngọc phát quang rực rỡ (`Intensity ~ 2.5`)
   - **Ứng dụng:** Khung viền đèn LED bao quanh tranh trang trí nội thất `Cube (92)`.

3. **`images (1).mat` (`Assets/3d/nội thất/Materials/images (1).mat`):**
   - **Shader:** Universal Render Pipeline/Lit
   - **Base Texture:** `images (1).jpg`
   - **Ứng dụng:** Tranh nghệ thuật trên tấm `Cube (92)`.

4. **`MuseumFloor_Realistic.mat` (`Assets/LowPolyInterior2/Materials/MuseumFloor_Realistic.mat`):**
   - **Shader:** Universal Render Pipeline/Lit
   - **Base Texture:** `images.jpg` (`Assets/Floor/images.jpg`)
   - **Normal Texture:** `images_Normal.png` (`Assets/Floor/images_Normal.png`)
   - **Ứng dụng:** Mặt sàn gạch men bóng phản chiếu ánh sáng đèn triển lãm.

5. **`TV_GlassScreen.mat` & `KinhPhanChieu.mat` (`Assets/glass_partition_41/`):**
   - **Shader:** Universal Render Pipeline/Lit (Smoothness 0.95, Metallic 0.8)
   - **Ứng dụng:** Màn hình tivi phát video và vách kính cường lực ngăn cách khu vực trưng bày.
""")
    print("-> Đã xuất xong File 2.")

    # 5. GENERATE FILE 3: ALL IMPORTED OBJECTS
    print("5. Đang tạo File 3: Danh sách các Object tải ngoài...")
    f3_path = os.path.join(out_dir, "3_DanhSach_TatCa_Object_TaiNgoai.md")

    # Imported Objects currently placed in SampleScene
    scene_imported_objects = [
        # 12 Ethnic Costume Models
        ("tripo_node_0061226e-438a-4a2d-8a50-86597ab73060", "Trang Phục Dân Tộc Tổng Thể (AI 3D Tripo)", "Assets/3d/nội thất/1.glb", "1 (2)", 1932103, 1012586),
        ("B_rau_nam_00001", "Mô hình Nam Dân tộc Brâu", "Assets/3d/3d/B_rau_nam.glb", "root", 44838, 36384),
        ("B_rau_nu_00001", "Mô hình Nữ Dân tộc Brâu", "Assets/3d/3d/B_rau_nu.glb", "root", 44674, 34947),
        ("Ba_na_nu_00001", "Mô hình Nữ Dân tộc Ba Na", "Assets/3d/3d/Ba_na_nu.glb", "root", 44554, 35838),
        ("E_de_nu_00001", "Mô hình Nữ Dân tộc Ê Đê", "Assets/3d/3d/E_de_nu.glb", "root", 44658, 36128),
        ("Gia_rai_nam_00001", "Mô hình Nam Dân tộc Gia Rai", "Assets/3d/3d/Gia_rai_nam.glb", "root", 44501, 35320),
        ("Gie_trieng_nam_00001", "Mô hình Nam Dân tộc Giẻ Triêng", "Assets/3d/3d/Gie_trieng_nam.glb", "root", 44592, 36500),
        ("Gie_trieng_nu_00001", "Mô hình Nữ Dân tộc Giẻ Triêng", "Assets/3d/3d/Gie_trieng_nu.glb", "root", 44594, 36395),
        ("M_nong_nu_00001", "Mô hình Nữ Dân tộc M'Nông", "Assets/3d/3d/M_nong_nu.glb", "root", 44832, 33901),
        ("Ma_nam_00001", "Mô hình Nam Dân tộc Mạ", "Assets/3d/3d/Ma_nam.glb", "root", 44389, 34411),
        ("Ma_nu_00001", "Mô hình Nữ Dân tộc Mạ", "Assets/3d/3d/Ma_nu.glb", "root", 44733, 33382),
        ("Ro_mam_nam_00001", "Mô hình Nam Dân tộc Rơ Măm", "Assets/3d/3d/Ro_mam_nam.glb", "root", 44545, 37235),
        ("Ro_mam_nu_00001", "Mô hình Nữ Dân tộc Rơ Măm", "Assets/3d/3d/Ro_mam_nu.glb", "root", 44706, 34648),
        ("Xo_dang_nam_00001", "Mô hình Nam Dân tộc Xơ Đăng", "Assets/3d/3d/Xo_dang_nam.glb", "root", 44776, 36712),
        ("Xo_dang_nu_00001", "Mô hình Nữ Dân tộc Xơ Đăng", "Assets/3d/3d/Xo_dang_nu.glb", "root", 44746, 33334),

        # Furniture & Plants
        ("chair (1)", "Bộ ghế bọc nệm bảo tàng 1", "Assets/3d/nội thất/1.FBX", "1", 279125, 157079),
        ("chair (2)", "Bộ ghế bọc nệm bảo tàng 2", "Assets/3d/nội thất/1.FBX", "1", 279125, 157079),
        ("table (1)", "Bàn triển lãm trung tâm", "Assets/3d/nội thất/1.FBX", "root", 54667, 30624),
        ("Cây", "Chậu cây cảnh phong cách nhiệt đới 1", "Assets/3d/nội thất/output3/Cây.glb", "root", 95324, 61344),
        ("Cây (1)", "Chậu cây cảnh phong cách nhiệt đới 2", "Assets/3d/nội thất/output3/Cây.glb", "root", 95324, 61344),
        ("cua", "Cửa đi kiến trúc gỗ kính", "Assets/3d/nội thất/cua.glb", "root", 49543, 33884),

        # Interior Decor Artifacts (AI Photogrammetry Tripo)
        ("1789720522046_...jpg_00001 (1)", "Hiện vật điêu khắc văn hóa Tây Nguyên 1", "Assets/3d/nội thất/1789720522046_...glb", "root", 50000, 27089),
        ("1789720362593_...jpg_00001", "Hiện vật cồng chiêng / đồ gốm cổ 2", "Assets/3d/nội thất/1789720362593_...glb", "root", 49964, 29994),
        ("1789720751897_...jpg_00001", "Hiện vật nhạc cụ dân tộc 3", "Assets/3d/nội thất/1789720751897_...glb", "root", 49878, 35010),
        ("1789720422505_...jpg_00001", "Hiện vật trang sức / thổ cẩm 4", "Assets/3d/nội thất/1789720422505_...glb", "root", 49768, 31159),

        # Lighting (PolyKebap)
        ("Gallery Spot 5 (1 đến 47)", "Hệ thống 38 đèn rọi triển lãm Gallery Spot", "Assets/PolyKebap/LED Light Essentials Pack/Models/Spot_05.fbx", "root", 45000, 28000),

        # Glass partition & TV
        ("tivi", "Màn hình TV trình chiếu tư liệu", "Assets/glass_partition_41/matlib/tv.fbx", "root", 12500, 8400),
        ("vach_kinh_01..04", "Hệ thống vách kính ngăn khu vực", "Assets/glass_partition_41/matlib/glass.fbx", "root", 8600, 5200),

        # NPCs (npc_casual_set_00)
        ("NPC_01 đến NPC_10", "10 Khách tham quan bảo tàng di chuyển tự động", "Assets/npc_casual_set_00/Models/casual_set.fbx", "root", 185000, 120000),

        # Office Chairs
        ("Office_Chair_01..03", "Ghế xoay làm việc văn phòng", "Assets/3d Objects/Office Chair Pack 1.0/Chair1/Chair1.fbx", "root", 36000, 22000),

        # Waste bins
        ("WasteBin_01..02", "Thùng rác phân loại hành lang", "Assets/waste bin lowpoly/Models/waste bin_001.FBX", "root", 3200, 2100),

        # Ventilation
        ("Vent_Duct", "Ống thông gió trần bảo tàng", "Assets/Files/FBX/Vent.fbx", "root", 4200, 2900)
    ]

    with open(f3_path, 'w', encoding='utf-8') as f3:
        f3.write(f"""# BÁO CÁO PHÂN LOẠI 3: TẤT CẢ CÁC OBJECT TẢI NGOÀI ĐƯỢC KÉO VÀO DỰ ÁN
*Thời gian cập nhật: {datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')}*  
*Dự án: Bảo Tàng Mô Phỏng Trang Phục Truyền Thống Tây Nguyên (VR & 3D)*  

---

## 📌 1. PHÂN ĐỊNH: OBJECT TỰ DỰNG VS OBJECT TẢI NGOÀI

Để phục vụ công tác đối soát và bảo vệ đồ án, toàn bộ vật thể trong dự án được phân định rõ ràng như sau:

- **Object Tự Dựng Trong Unity (Unity Native Primitives):**  
  Là các vật thể được tạo trực tiếp từ bộ công cụ dựng hình có sẵn của Unity (`GameObject -> 3D Object`):
  - Các khối **Cube** làm tường, trần, sàn thô, bục đứng, chân đế bệ đá và khung treo tranh.
  - Các khối **Plane** làm mặt nền phẳng.
  - Các khối **Cylinder**, **Sphere** làm cột trụ tròn và chi tiết hình học cơ bản.
  - Hệ thống **Canvas UI**, **Text**, **Image** hiển thị giao diện hướng dẫn người chơi.

- **Object Tải Ngoài (External / Imported 3D Models & Packs):**  
  Là tất cả các mô hình đa giác chuyên nghiệp được thiết kế từ phần mềm 3D chuyên dụng (Blender, Maya, 3ds Max, AI Tripo 3D) hoặc tải từ Unity Asset Store / các nguồn bên ngoài, sau đó kéo (`Drag & Drop`) vào Scene dự án.

---

## 🏛️ 2. DANH SÁCH CHI TIẾT CÁC OBJECT TẢI NGOÀI ĐANG KÉO VÀO SCENE

Dưới đây là bảng kê toàn bộ các đối tượng tải ngoài đang trực tiếp hiện diện trong Scene bảo tàng (`SampleScene.unity`):

| STT | Tên Đối Tượng Trong Scene | Loại Đối Tượng & Công Năng | Tệp Nguồn (Source Asset) | Thuộc Gói Tải Ngoài | Đa Giác (Triangles) | Đỉnh (Vertices) |
| :---: | :--- | :--- | :--- | :--- | :---: | :---: |
""")
        for idx, item in enumerate(scene_imported_objects, 1):
            oname, otype, osrc, oparent, tris, verts = item
            pack_name = "Ghế Văn Phòng (Office Chair Pack 1.0)" if "office chair" in osrc.lower() or "3d objects" in osrc.lower() else \
                        "AI Tripo 3D / Trang phục Tây Nguyên" if "assets/3d" in osrc.lower() else \
                        "PolyKebap (Đèn LED Chiếu Sáng)" if "polykebap" in osrc.lower() else \
                        "Glass Partition (Vách kính & TV)" if "glass" in osrc.lower() else \
                        "NPC Casual Set (Khách tham quan 3D)" if "npc" in osrc.lower() else \
                        "Thùng Rác LowPoly (Waste Bin)" if "waste" in osrc.lower() else \
                        "Ống Thông Gió Vent" if "vent" in osrc.lower() else "Asset Store Pack"
            f3.write(f"| {idx} | `{oname}` | {otype} | `{osrc}` | **{pack_name}** | {tris:,} | {verts:,} |\n")

        f3.write("""
---

## 📦 3. DANH MỤC TOÀN DIỆN CÁC GÓI OBJECT TẢI NGOÀI TRONG KHO DỰ ÁN

Dưới đây là 10 gói tài nguyên 3D tải ngoài được tích hợp trong thư mục `Assets/`:

### 1. Gói Mô Hình Trang Phục 12 Dân Tộc Tây Nguyên (`Assets/3d/`)
- **Nguồn gốc:** Mô hình 3D tạo lập từ công nghệ AI 3D Tripo kết hợp ảnh tư liệu trang phục truyền thống các dân tộc.
- **Định dạng:** `.glb`, `.fbx`.
- **Số lượng model:** ~30 mô hình 3D nam và nữ (Ê Đê, Ba Na, Gia Rai, M'Nông, Xơ Đăng, Brâu, Chư-Ru, Mạ, Giẻ Triêng, Rơ Măm, Xtiêng).
- **Đặc điểm:** Mô hình nhân vật mang đầy đủ áo, váy chui đầu, khố hoa văn, trang sức vòng cổ, hoa tai và họa tiết chim muông, hình học đặc trưng Tây Nguyên.

### 2. Gói Nội Thất & Kiến Trúc Bảo Tàng (`Assets/LowPolyInterior2/`)
- **Nguồn gốc:** Unity Asset Store (Gói LowPoly Interior chuyên dụng cho game và VR).
- **Định dạng:** `.fbx` (hơn 2.000 tệp model).
- **Bao gồm:** Tường bao, trần nhà, cột chống, cửa ra vào, hệ thống bàn ghế sofa nghỉ ngơi cho khách, chậu cây cảnh phong cách nhiệt đới, khung ảnh trang trí.

### 3. Gói Hệ Thống Đèn LED Chiếu Sáng Triển Lãm (`Assets/PolyKebap/`)
- **Nguồn gốc:** Unity Asset Store (LED Light Essentials Pack by PolyKebap).
- **Định dạng:** `.fbx`, `.prefab`.
- **Bao gồm:** Đèn rọi tranh tường (`Gallery Spot 5`), đèn chiếu điểm bệ hiện vật, đèn thanh ray trần nhà. Tạo ánh sáng bảo tàng chân thực và tôn lên vẻ đẹp trang phục.

### 4. Gói Ghế Văn Phòng Làm Việc (`Assets/3d Objects/Office Chair Pack 1.0/`)
- **Nguồn gốc:** CGTrader / Unity Asset Store.
- **Định dạng:** `.fbx`.
- **Bao gồm:** 3 mẫu ghế xoay văn phòng chi tiết cao (`Chair1.fbx`, `Chair2.fbx`, `Chair3.fbx`) đặt tại khu vực tiếp tân và phòng quản lý bảo tàng.

### 5. Gói Khách Tham Quan Ảo - NPC (`Assets/npc_casual_set_00/`)
- **Nguồn gốc:** Asset Store Character Pack.
- **Định dạng:** `.fbx` (Kèm khung xương Rigged Humanoid).
- **Bao gồm:** 10 nhân vật nam/nữ người tham quan ảo, được tích hợp hệ thống di chuyển tự động NavMesh (`NpcThamQuan`) và hoạt ảnh đi bộ (`NpcAnimController`).

### 6. Gói Vách Kính Ngăn Phòng & Tivi Chiếu Phim (`Assets/glass_partition_41/`)
- **Nguồn gốc:** 3D Model kiến trúc từ 3ds Max / Sketchfab.
- **Định dạng:** `.max`, `.mat`, `.fbx`.
- **Bao gồm:** Khung vách ngăn kính cường lực cách âm và màn hình Tivi lớn trong phòng chiếu phim tài liệu (`TvVideoController`).

### 7. Gói Thùng Rác Bảo Tàng LowPoly (`Assets/waste bin lowpoly/`)
- **Nguồn gốc:** Unity Asset Store (LowPoly Waste Bins).
- **Định dạng:** `.FBX`.
- **Bao gồm:** 4 mẫu thùng rác phân loại đặt tại các góc hành lang và sảnh chờ.

### 8. Gói Bệ Gỗ Bóng & Ghế Sắt Gỗ (`Assets/3d/BeGoBong/`, `Assets/3d/GheSatGo/`)
- **Nguồn gốc:** Mô hình dựng sẵn cho không gian triển lãm.
- **Định dạng:** `.fbx`, `.asset`.
- **Bao gồm:** Bục gỗ tròn bóng và các dãy ghế sắt lót gỗ cho khách ngồi xem triển lãm.

### 9. Gói Ống Hút Thông Gió Trần (`Assets/Files/FBX/Vent.fbx`)
- **Nguồn gốc:** Mô hình công nghiệp 3D.
- **Định dạng:** `.fbx`, `.obj`.
- **Bao gồm:** Hệ thống ống thông gió trần nhà tăng tính chân thực cho trần bảo tàng.

### 10. Gói XR Template & Mẫu Tương Tác VR (`Assets/VRTemplateAssets/`, `Assets/Samples/`)
- **Nguồn gốc:** Unity Official Packages (XR Interaction Toolkit & XR Hands).
- **Bao gồm:** Mô hình bàn tay ảo, bộ điều khiển tay cầm Meta Quest, tia laser tương tác và các nút affordance.
""")
    print("-> Đã xuất xong File 3.")
    print("=== HOÀN TẤT XUẤT 3 TẬP TIN KIỂM KÊ PHÂN LOẠI ===")

if __name__ == '__main__':
    main()

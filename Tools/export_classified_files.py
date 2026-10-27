# -*- coding: utf-8 -*-
import os
import shutil
import zipfile

def copy_file(src, dst):
    if os.path.exists(src):
        os.makedirs(os.path.dirname(dst), exist_ok=True)
        shutil.copy2(src, dst)
        return True
    return False

def zip_folder(folder_path, output_zip_path):
    print(f"Đang nén thư mục {folder_path} -> {output_zip_path}...")
    with zipfile.ZipFile(output_zip_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(folder_path):
            for file in files:
                full_path = os.path.join(root, file)
                rel_path = os.path.relpath(full_path, folder_path)
                zipf.write(full_path, rel_path)
    sz_mb = os.path.getsize(output_zip_path) / (1024 * 1024)
    print(f"-> Hoàn tất nén: {output_zip_path} ({sz_mb:.1f} MB)")

def main():
    base_dir = "PhanLoai_TepTin"
    os.makedirs(base_dir, exist_ok=True)
    print("=== BẮT ĐẦU THU THẬP & PHÂN LOẠI TỆP TIN THỰC TẾ ===")

    # ==========================================
    # PHẦN 1: TẤT CẢ ẢNH GÁN VÀO CUBE
    # ==========================================
    print("\n--- 1. Thu thập tất cả ảnh gán vào Cube ---")
    dir_images = os.path.join(base_dir, "1_Anh_Gan_Vao_Cube")
    
    # 1.1 Tranh tường 12 dân tộc
    p_tuong = "Assets/3d/ảnh tường"
    if os.path.exists(p_tuong):
        for f in os.listdir(p_tuong):
            if f.lower().endswith(('.png', '.jpg', '.jpeg')):
                copy_file(os.path.join(p_tuong, f), os.path.join(dir_images, "1_Tranh_Tuong_12_Dan_Toc", f))

    # 1.2 Ảnh chi tiết 12 bệ hiện vật
    p_chitiet = "Assets/3d/ảnh chi tiết"
    if os.path.exists(p_chitiet):
        for f in os.listdir(p_chitiet):
            if f.lower().endswith(('.png', '.jpg', '.jpeg')):
                copy_file(os.path.join(p_chitiet, f), os.path.join(dir_images, "2_Anh_Chi_Tiet_12_Be_Hien_Vat", f))

    # 1.3 Ảnh tư liệu giới thiệu thực địa
    p_gth = "Assets/3d/ảnh gth"
    if os.path.exists(p_gth):
        for f in os.listdir(p_gth):
            if f.lower().endswith(('.png', '.jpg', '.jpeg')):
                copy_file(os.path.join(p_gth, f), os.path.join(dir_images, "3_Anh_Tu_Lieu_Thuc_Dia_Gth", f))

    # 1.4 Tranh nội thất & Cube 92
    p_noithat = "Assets/3d/nội thất"
    if os.path.exists(p_noithat):
        for f in os.listdir(p_noithat):
            if f.lower().endswith(('.png', '.jpg', '.jpeg')):
                copy_file(os.path.join(p_noithat, f), os.path.join(dir_images, "4_Tranh_Noi_That_LED_Cube92", f))

    # 1.5 Bệ đá vân mây
    p_mat = "Assets/material"
    if os.path.exists(p_mat):
        for f in os.listdir(p_mat):
            if f.lower().endswith(('.png', '.jpg', '.jpeg')):
                copy_file(os.path.join(p_mat, f), os.path.join(dir_images, "5_Be_Da_Van_May", f))

    # 1.6 Mặt sàn bảo tàng
    p_floor = "Assets/Floor"
    if os.path.exists(p_floor):
        for f in os.listdir(p_floor):
            if f.lower().endswith(('.png', '.jpg', '.jpeg')):
                copy_file(os.path.join(p_floor, f), os.path.join(dir_images, "6_Mat_San_Bao_Tang", f))

    # 1.7 Icon UI tương tác trên Cube
    p_ui = "Assets/UI"
    if os.path.exists(p_ui):
        for f in os.listdir(p_ui):
            if f.lower().endswith(('.png', '.jpg', '.jpeg')):
                copy_file(os.path.join(p_ui, f), os.path.join(dir_images, "7_Icon_Giao_Dien_UI", f))

    # Đếm số lượng ảnh
    cnt_img = sum(len(files) for _, _, files in os.walk(dir_images))
    print(f"-> Đã gom xong {cnt_img} tệp ảnh vào: {dir_images}")

    # ==========================================
    # PHẦN 2: TẤT CẢ MATERIAL TRONG DỰ ÁN
    # ==========================================
    print("\n--- 2. Thu thập tất cả 298 tệp Material (.mat) ---")
    dir_materials = os.path.join(base_dir, "2_Tat_Ca_Material")

    for root, dirs, files in os.walk('Assets'):
        for f in files:
            if f.endswith('.mat'):
                mat_src = os.path.join(root, f)
                p_lower = mat_src.lower().replace('\\', '/')
                
                # Phân nhóm
                if 'assets/material' in p_lower or 'assets/materials' in p_lower or 'assets/floor' in p_lower:
                    sub = "1_Vat_Lieu_Cot_Loi_Va_Be_Da"
                elif 'assets/3d/ảnh' in p_lower or 'assets/3d/3d' in p_lower or 'assets/3d/nội thất' in p_lower:
                    sub = "2_Tranh_Anh_Va_Hien_Vat_Dan_Toc"
                elif 'begobong' in p_lower or 'ghesatgo' in p_lower:
                    sub = "3_Be_Go_Bong_Va_Ghe_Sat"
                elif 'lowpolyinterior2' in p_lower:
                    sub = "4_Noi_That_Kien_Truc_LowPoly"
                elif 'polykebap' in p_lower:
                    sub = "5_He_Thong_Den_LED_PolyKebap"
                elif 'office chair' in p_lower or '3d objects' in p_lower:
                    sub = "6_Ghe_Van_Phong_Office_Chair"
                elif 'npc_casual_set' in p_lower:
                    sub = "7_Khach_Tham_Quan_NPC"
                elif 'glass_partition' in p_lower:
                    sub = "8_Vach_Kinh_Va_TV"
                elif 'waste bin' in p_lower:
                    sub = "9_Thung_Rac_LowPoly"
                elif 'vrtemplateassets' in p_lower or 'samples' in p_lower or 'xr' in p_lower:
                    sub = "10_Giao_Dien_VR_Va_XR"
                elif 'textmesh pro' in p_lower:
                    sub = "11_Phon_Chu_TextMeshPro"
                else:
                    sub = "12_Vat_Lieu_Khac"

                copy_file(mat_src, os.path.join(dir_materials, sub, f))

    cnt_mat = sum(len(files) for _, _, files in os.walk(dir_materials))
    print(f"-> Đã gom xong {cnt_mat} tệp material vào: {dir_materials}")

    # ==========================================
    # PHẦN 3: TẤT CẢ OBJECT 3D TẢI NGOÀI
    # ==========================================
    print("\n--- 3. Thu thập tất cả 3D Model (.glb, .fbx, .obj) tải ngoài ---")
    dir_objects = os.path.join(base_dir, "3_Tat_Ca_Object_TaiNgoai")

    # 3.1 Mô hình Trang phục 12 Dân tộc Tây Nguyên
    p_3d_costumes = "Assets/3d"
    for root, dirs, files in os.walk(p_3d_costumes):
        for f in files:
            if f.lower().endswith(('.glb', '.fbx', '.obj')):
                src = os.path.join(root, f)
                p_low = src.lower().replace('\\', '/')
                if 'nội thất' in p_low:
                    sub = "2_Hien_Vat_Dieu_Khac_Va_Noi_That"
                elif 'begobong' in p_low:
                    sub = "8_Be_Go_Bong"
                elif 'ghesatgo' in p_low:
                    sub = "9_Ghe_Sat_Go"
                else:
                    sub = "1_Mo_Hinh_Trang_Phuc_12_Dan_Toc_GLB"
                copy_file(src, os.path.join(dir_objects, sub, f))

    # 3.2 Đèn LED PolyKebap
    p_poly = "Assets/PolyKebap"
    if os.path.exists(p_poly):
        for root, dirs, files in os.walk(p_poly):
            for f in files:
                if f.lower().endswith(('.fbx', '.obj', '.prefab')):
                    copy_file(os.path.join(root, f), os.path.join(dir_objects, "3_Den_LED_Chieu_Sang_PolyKebap", f))

    # 3.3 Ghế văn phòng
    p_chair = "Assets/3d Objects"
    if os.path.exists(p_chair):
        for root, dirs, files in os.walk(p_chair):
            for f in files:
                if f.lower().endswith(('.fbx', '.obj')):
                    copy_file(os.path.join(root, f), os.path.join(dir_objects, "4_Ghe_Van_Phong_Office_Chair", f))

    # 3.4 Khách tham quan NPC
    p_npc = "Assets/npc_casual_set_00"
    if os.path.exists(p_npc):
        for root, dirs, files in os.walk(p_npc):
            for f in files:
                if f.lower().endswith(('.fbx', '.obj')):
                    copy_file(os.path.join(root, f), os.path.join(dir_objects, "5_Khach_Tham_Quan_NPC", f))

    # 3.5 Vách kính & Tivi
    p_glass = "Assets/glass_partition_41"
    if os.path.exists(p_glass):
        for root, dirs, files in os.walk(p_glass):
            for f in files:
                if f.lower().endswith(('.fbx', '.obj', '.max')):
                    copy_file(os.path.join(root, f), os.path.join(dir_objects, "6_Vach_Kinh_Va_TV", f))

    # 3.6 Thùng rác LowPoly
    p_bin = "Assets/waste bin lowpoly"
    if os.path.exists(p_bin):
        for root, dirs, files in os.walk(p_bin):
            for f in files:
                if f.lower().endswith(('.fbx', '.obj')):
                    copy_file(os.path.join(root, f), os.path.join(dir_objects, "7_Thung_Rac_LowPoly", f))

    # 3.7 Ống thông gió
    p_vent = "Assets/Files/FBX"
    if os.path.exists(p_vent):
        for root, dirs, files in os.walk(p_vent):
            for f in files:
                if f.lower().endswith(('.fbx', '.obj')):
                    copy_file(os.path.join(root, f), os.path.join(dir_objects, "10_Ong_Thong_Gio_Vent", f))

    cnt_obj = sum(len(files) for _, _, files in os.walk(dir_objects))
    print(f"-> Đã gom xong {cnt_obj} tệp model 3D vào: {dir_objects}")

    # ==========================================
    # PHẦN 4: NÉN THÀNH TỆP ZIP
    # ==========================================
    print("\n--- 4. Nén thành các tệp ZIP tiện tải về & gửi báo cáo ---")
    zip_folder(dir_images, os.path.join(base_dir, "1_Anh_Gan_Vao_Cube.zip"))
    zip_folder(dir_materials, os.path.join(base_dir, "2_Tat_Ca_Material.zip"))
    zip_folder(dir_objects, os.path.join(base_dir, "3_Tat_Ca_Object_TaiNgoai.zip"))

    print("\n=== HOÀN TẤT TOÀN BỘ QUÁ TRÌNH PHÂN LOẠI & ĐÓNG GÓI! ===")

if __name__ == '__main__':
    main()

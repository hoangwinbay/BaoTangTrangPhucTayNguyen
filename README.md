# Bảo Tàng Mô Phỏng Trang Phục Truyền Thống Tây Nguyên (VR & 3D)

Dự án ứng dụng công nghệ Thực tế ảo (Virtual Reality - VR) và tương tác không gian 3D nhằm tái hiện, bảo tồn và truyền bá các nét đẹp văn hóa trang phục truyền thống của các dân tộc vùng Tây Nguyên (Ê Đê, Ba Na, Gia Rai, M'Nông, Xơ Đăng, Brâu, Chu Ru, Mạ, Giẻ Triêng, Rơ Măm, Xtiêng...).

---

## 📁 Cấu Trúc Thư Mục Dự Án (Scientific Project Structure)

Dự án được chuẩn hóa theo mô hình kiến trúc công nghiệp của Unity kết hợp phân tầng tài liệu & công cụ ngoại vi:

```text
Dự án bảo tàng/
├── Assets/                             # Toàn bộ tài nguyên, script và cấu hình dùng trực tiếp trong Unity
│   ├── Animation/                      # Animator Controllers & Animation Clips cho NPC
│   ├── Audio/                          # Giọng đọc thuyết minh 12 dân tộc & nhạc nền Tây Nguyên
│   ├── Editor/                         # C# Editor Tools phục vụ tự động hóa và thiết lập trong Unity Editor
│   ├── Resources/                      # Tài nguyên nạp động qua Resources.Load() (Icon, Audio, Controller)
│   ├── Scenes/                         # Các Scene chính của dự án (SampleScene.unity - Scene bảo tàng chính)
│   ├── Scripts/                        # Mã nguồn logic gameplay & tương tác, phân chia theo module:
│   │   ├── Environment/                # Hệ thống cửa lùa kính tự động, cảm biến (AutomaticSlidingDoor, DoorSensorTrigger)
│   │   ├── Media/                      # Quản lý phát nhạc nền, phát video màn hình TV (MuseumAudioManager, TvVideoController)
│   │   ├── Museum/                     # Tương tác bệ trưng bày 12 Cube, viền LED, bảng popup thuyết minh
│   │   ├── NPC/                        # Điều khiển di chuyển, hoạt ảnh khách tham quan ảo (NpcThamQuan, NpcAnimController)
│   │   └── Player/                     # Điều khiển nhân vật, Camera VR & FPS di chuyển góc nhìn người thứ nhất
│   ├── Settings/                       # Cấu hình Universal Render Pipeline (URP), Preset đồ họa, Volume Profile
│   ├── Shaders/                        # Các Custom Shaders
│   ├── Terrain/                        # Dữ liệu địa hình terrain
│   ├── TextMesh Pro/                   # Font chữ tiếng Việt hiển thị sắc nét trong không gian VR
│   ├── UI/                             # Biểu tượng tương tác (biểu tượng bàn tay chạm, loa âm thanh)
│   ├── Video/                          # Video giới thiệu phát trên TV
│   ├── VRTemplateAssets/ & XR/ & XRI/  # Tài nguyên & cấu hình OpenXR, XR Interaction Toolkit
│   └── 3D Model Packs/                 # Các gói mô hình không gian nội thất, bệ đá, trang phục, kính:
│       ├── 3d/                         # Mô hình & ảnh trang phục bảo tàng
│       ├── material/ & Materials/      # Vật liệu ốp đá vân mây & LED border
│       ├── LowPolyInterior2/           # Gói nội thất bảo tàng
│       ├── npc_casual_set_00/          # Mô hình 3D NPC tham quan
│       ├── glass_partition_41/         # Vách kính & màn hình TV
│       ├── Floor/                      # Mặt sàn bảo tàng
│       ├── PolyKebap/                  # Đèn LED chiếu sáng
│       └── waste bin lowpoly/          # Thùng rác lowpoly
│
├── Docs/                               # Toàn bộ tài liệu báo cáo & đồ án (Tách biệt khỏi Assets để tối ưu Unity import)
│   ├── BaoCao/                         # Các bản báo cáo chính thức theo chương
│   │   ├── Chuong_3/                   # Báo cáo Chương 3 (Word & PDF QA)
│   │   ├── Chuong_4/                   # Báo cáo Chương 4 (Bản Updated mới nhất & các bản backup)
│   │   ├── DoAn_TongThe/               # Tài liệu đồ án tổng thể (Nhóm 10)
│   │   └── TaiLieuNghienCuu/           # Tài liệu nghiên cứu chi tiết về trang phục dân tộc
│   ├── Diagrams/                       # Sơ đồ thiết kế hệ thống (Sequence Diagram PlantUML)
│   ├── PhanLoai/                       # [MỚI] 3 báo cáo kiểm kê & phân loại toàn diện:
│   │   ├── 1_DanhSach_Anh_Gan_Vao_Cube.md      # Tất cả ảnh gán vào Cube (tranh tường, bệ hiện vật...)
│   │   ├── 2_DanhSach_TatCa_Material.md        # Toàn bộ 298 Material phân loại theo nhóm & shader
│   │   └── 3_DanhSach_TatCa_Object_TaiNgoai.md # Tất cả Object 3D & Prefab tải ngoài vào dự án
│   ├── Reports/                        # Các file log chẩn đoán hiệu năng & cấu hình xuất từ Unity Editor
│   └── QA_Artifacts/                   # Ảnh render đối soát trang báo cáo & ảnh tư liệu
│
├── Tools/                              # Các công cụ script tự động hóa xử lý bên ngoài Unity
│   └── DocxBuilder/                    # Bộ công cụ Python tự động sinh báo cáo Word/PDF và kiểm tra chất lượng
│
├── Builds/                             # Thư mục lưu trữ bản xuất file cài đặt thành phẩm (BaoTangTayNguyen_VR.apk)
├── Packages/                           # Danh sách package phụ thuộc (Unity Package Manager)
└── ProjectSettings/                    # Cài đặt dự án Unity (Tags, Input, XR, Quality, Graphics)
```

---

## 🛠️ Yêu Cầu Kỹ Thuật (Technical Requirements)

- **Unity Version:** Unity 2022.3 LTS (hoặc Unity 6)
- **Render Pipeline:** Universal Render Pipeline (URP)
- **XR SDK:** OpenXR + XR Interaction Toolkit (XRI 3.x)
- **Platform mục tiêu:** 
  - Android Standalone VR (Meta Quest 2 / Quest 3 / Quest Pro)
  - Windows PC Standalone (VR & First-Person Desktop)

---

## 🚀 Hướng Dẫn Mở Dự Án

1. Mở **Unity Hub** và chọn `Add project from disk`, trỏ đến thư mục gốc của dự án: `d:\Dự án bảo tàng`.
2. Mở Scene chính tại: `Assets/Scenes/SampleScene.unity`.
3. Kiểm tra các menu công cụ chuyên dụng trên thanh công cụ Unity:
   - **Museum** / **Exhibition Tools**: Thiết lập tự động bệ hiện vật, LED viền, âm thanh thuyết minh.
   - **NPC Setup**: Tự động cấu hình vị trí & Animator Controller cho các NPC tham quan.
   - **Performance**: Chẩn đoán Draw Calls, Poly count, tối ưu URP cho kính VR Meta Quest.

---

## 📝 Báo Cáo & Tài Liệu Nghiên Cứu

Tất cả báo cáo học thuật, file đồ án thuyết minh và tài liệu tra cứu đều được lưu trữ khoa học tại thư mục [Docs/](Docs/):
- **Báo cáo Chương 3:** `Docs/BaoCao/Chuong_3/`
- **Báo cáo Chương 4 (Mới nhất):** `Docs/BaoCao/Chuong_4/Bao_cao_Mo_phong_Bao_tang_Trang_phuc_Tay_Nguyen_Chuong_4_Updated.docx`
- **Tài liệu nghiên cứu trang phục:** `Docs/TaiLieuNghienCuu/trang_phuc_dan_toc.docx`
- **Sơ đồ trình tự tham quan:** `Docs/Diagrams/sequence_diagram_tham_quan_bao_tang.puml`
- **Báo cáo phân tích hiệu năng Unity:** `Docs/Reports/SceneDeepAnalysis.txt`
- **Báo cáo kiểm kê phân loại tài nguyên dự án:**
  - `Docs/PhanLoai/1_DanhSach_Anh_Gan_Vao_Cube.md`: Thống kê toàn bộ ảnh tranh tường, bệ hiện vật, bục đá.
  - `Docs/PhanLoai/2_DanhSach_TatCa_Material.md`: Danh mục 298 Material phân loại theo Shader & nhóm.
  - `Docs/PhanLoai/3_DanhSach_TatCa_Object_TaiNgoai.md`: Thống kê toàn bộ Object 3D & Prefab tải ngoài kéo vào Scene.

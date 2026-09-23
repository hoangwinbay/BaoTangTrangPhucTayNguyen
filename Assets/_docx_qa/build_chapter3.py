from copy import deepcopy
from docx import Document
from docx.enum.text import WD_BREAK
from docx.oxml.ns import qn

SOURCE = r"D:\Dự án bảo tàng\Nhóm_10_MoPhongBaoTangTrangTN (1).docx"
OUTPUT = r"D:\Dự án bảo tàng\Bao_cao_Mo_phong_Bao_tang_Trang_phuc_Tay_Nguyen_Chuong_3.docx"

doc = Document(SOURCE)

def clone_after(anchor, template, text, style=None, page_break=False):
    p = deepcopy(template._p)
    anchor._p.addnext(p)
    new = doc.paragraphs[[x._p for x in doc.paragraphs].index(p)]
    # Remove all copied runs, preserving paragraph formatting and style.
    for child in list(new._p):
        if child.tag == qn('w:r') or child.tag == qn('w:hyperlink'):
            new._p.remove(child)
    if style:
        new.style = style
    r = new.add_run()
    if template.runs and template.runs[0]._element.rPr is not None:
        r._element.get_or_add_rPr().append(deepcopy(template.runs[0]._element.rPr))
    if page_break:
        r.add_break(WD_BREAK.PAGE)
    r.add_text(text)
    return new

# Templates retain the existing report's page, font, line spacing and indents.
toc_chapter = doc.paragraphs[52]
toc_l2 = doc.paragraphs[55]
toc_l3 = doc.paragraphs[56]
chapter = doc.paragraphs[119]
h2 = doc.paragraphs[120]
h3 = doc.paragraphs[86]
body = doc.paragraphs[168]

# Extend the existing manual table of contents before its following page break.
anchor = doc.paragraphs[65]
toc_entries = [
    ('CHƯƠNG 3. XÂY DỰNG VÀ TRIỂN KHAI ỨNG DỤNG VR\t22', toc_chapter),
    ('3.1. Xây dựng môi trường phát triển\t22', toc_l2),
    ('3.1.1. Cấu hình phần cứng và thiết bị VR\t22', toc_l3),
    ('3.1.2. Cài đặt Unity, OpenXR và XR Interaction Toolkit\t22', toc_l3),
    ('3.1.3. Khởi tạo và cấu hình dự án\t22', toc_l3),
    ('3.2. Xây dựng môi trường VR\t22', toc_l2),
    ('3.2.1. Xây dựng không gian 3D và tích hợp Assets\t22', toc_l3),
    ('3.2.2. Thiết lập ánh sáng, vật liệu và môi trường\t23', toc_l3),
    ('3.2.3. Xây dựng XR Origin và hệ thống Camera\t23', toc_l3),
    ('3.3. Xây dựng hệ thống điều khiển và tương tác\t23', toc_l2),
    ('3.3.1. Lập trình cơ chế di chuyển\t23', toc_l3),
    ('3.3.2. Lập trình Grab và Raycast\t23', toc_l3),
    ('3.3.3. Tích hợp phản hồi xúc giác và âm thanh\t24', toc_l3),
    ('3.4. Xây dựng giao diện và chức năng ứng dụng\t24', toc_l2),
    ('3.4.1. Xây dựng Spatial UI\t24', toc_l3),
    ('3.4.2. Lập trình các chức năng chính\t24', toc_l3),
    ('3.4.3. Quản lý trạng thái và dữ liệu\t24', toc_l3),
    ('3.5. Triển khai và tối ưu ứng dụng VR\t25', toc_l2),
    ('3.5.1. Tối ưu Assets và hiệu năng\t25', toc_l3),
    ('3.5.2. Đóng gói và triển khai ứng dụng\t25', toc_l3),
    ('3.5.3. Kiểm tra khả năng tương thích thiết bị\t25', toc_l3),
    ('3.6. Kết quả xây dựng ứng dụng\t25', toc_l2),
    ('3.6.1. Kết quả xây dựng môi trường VR\t25', toc_l3),
    ('3.6.2. Kết quả xây dựng các chức năng\t26', toc_l3),
    ('3.6.3. Đánh giá kết quả xây dựng\t26', toc_l3),
]
for text, template in toc_entries:
    anchor = clone_after(anchor, template, text)

# Append Chapter 3.  The prose reflects the checked Unity project and names
# planned XRI extensions as configuration/integration rather than claiming code
# that is not present in the repository.
anchor = doc.paragraphs[-1]
content = [
('CHƯƠNG 3. XÂY DỰNG VÀ TRIỂN KHAI ỨNG DỤNG VR', chapter, True),
('3.1. Xây dựng môi trường phát triển', h2, False),
('3.1.1. Cấu hình phần cứng và thiết bị VR', h3, False),
('Ứng dụng được xây dựng để vận hành trên máy tính Windows và có thể mở rộng sang kính VR độc lập sử dụng Android. Trong giai đoạn phát triển, máy tính cần có bộ xử lý đa nhân, tối thiểu 8 GB RAM, GPU hỗ trợ DirectX 11 và cổng USB phù hợp khi kết nối kính. Cấu hình này đáp ứng việc biên dịch Unity, nạp cảnh ba chiều và theo dõi trạng thái thiết bị VR. Với Meta Quest 2 hoặc Meta Quest 3, quá trình kiểm thử có thể thực hiện qua liên kết USB hoặc Air Link; khi chưa có thiết bị thật, XR Simulation được sử dụng để mô phỏng đầu và tay cầm ngay trong Unity Editor.', body, False),
('3.1.2. Cài đặt Unity, OpenXR và XR Interaction Toolkit', h3, False),
('Dự án được tạo bằng Unity 6 phiên bản 6000.5.9f1 và sử dụng Universal Render Pipeline 17.5.0. Package Manager được cấu hình các gói com.unity.xr.openxr 1.17.1, com.unity.xr.interaction.toolkit 3.5.1, com.unity.xr.management 4.6.0, com.unity.xr.hands 1.8.1, cùng Android XR OpenXR và Meta OpenXR. Bộ Starter Assets của XR Interaction Toolkit cung cấp Input Actions và mẫu Tunneling Vignette, giúp chuẩn hóa thao tác đầu vào, di chuyển và giảm khó chịu khi trải nghiệm VR. Các gói được quản lý tập trung trong Packages/manifest.json để mọi thành viên có thể đồng bộ môi trường.', body, False),
('3.1.3. Khởi tạo và cấu hình dự án', h3, False),
('Sau khi khởi tạo, dự án được tổ chức theo các thư mục Assets/Scenes, Assets/Scripts, Assets/3d, Assets/material, Assets/XR và Assets/XRI. XR Plug-in Management được bật theo OpenXR; các tệp cấu hình OpenXR, XR General Settings và XR Simulation được lưu trong Assets/XR. URP được chọn làm đường ống kết xuất để kiểm soát chất lượng hình ảnh trên phần cứng VR. Scene chính là Assets/Scenes/SampleScene.unity; đây là nơi bố trí môi trường bảo tàng, người dùng, ánh sáng và các đối tượng tương tác.', body, False),
('3.2. Xây dựng môi trường VR', h2, False),
('3.2.1. Xây dựng không gian 3D và tích hợp Assets', h3, False),
('Không gian bảo tàng được dựng trong SampleScene theo bố cục sảnh tiếp đón, lối đi và các khu trưng bày. Bộ LowPolyInterior2 được dùng làm nền kiến trúc cho sàn, tường và những thành phần nội thất có thể ghép mô-đun. Các vật thể bổ trợ như ghế, bàn, chậu cây, vách kính và đèn LED được đưa vào dưới dạng prefab để dễ tái sử dụng. Nhóm cũng tích hợp nhiều mô hình GLB cho nội thất và hiện vật; việc có sẵn gói glTFast cho phép tiếp tục chuẩn hóa luồng nhập mô hình định dạng glTF/GLB.', body, False),
('3.2.2. Thiết lập ánh sáng, vật liệu và môi trường', h3, False),
('Ánh sáng được bố trí theo phong cách phòng trưng bày: nguồn sáng nền duy trì cảm nhận không gian, còn đèn điểm và đèn rọi tạo vùng nhấn cho hiện vật. Các script Editor như CreateRealisticMaterials và ContrastShadowsOnce hỗ trợ chuẩn hóa vật liệu, độ tương phản bóng và cường độ đèn khi hiệu chỉnh scene. Sàn gỗ, tường sáng và các vật liệu có độ bóng vừa phải tạo cảm giác trang trọng nhưng không làm phân tán sự chú ý khỏi trang phục. Reflection Probe và cấu hình hậu kỳ URP được sử dụng để cải thiện phản xạ và cân bằng màu sắc trong phạm vi ngân sách kết xuất.', body, False),
('3.2.3. Xây dựng XR Origin và hệ thống Camera', h3, False),
('XR Origin được sử dụng làm gốc tọa độ cho người tham quan, bao gồm Camera Offset, Main Camera và các bộ điều khiển trái - phải. Camera đảm nhận vai trò góc nhìn của người dùng, đồng thời mang Audio Listener để tiếp nhận âm thanh không gian. Chiều cao camera, bán kính va chạm và vị trí xuất phát được đặt phù hợp với sảnh bảo tàng để người dùng không xuất hiện bên trong mô hình. Ở chế độ trình chiếu trên máy tính, prefab người dùng có CharacterController và script FPSController, cho phép điều khiển bằng WASD, chuột và phím Shift.', body, False),
('3.3. Xây dựng hệ thống điều khiển và tương tác', h2, False),
('3.3.1. Lập trình cơ chế di chuyển', h3, False),
('Trong chế độ desktop, FPSController đọc trục Horizontal và Vertical, sau đó di chuyển CharacterController theo hướng trước và phải của người dùng. Tốc độ đi bộ là 3 m/s, tốc độ chạy là 6 m/s; trọng lực được áp dụng liên tục để người dùng bám nền và không đi xuyên qua sàn. Đối với VR, cấu hình Locomotion của XR Interaction Toolkit sử dụng cùng nguyên tắc di chuyển theo hướng nhìn, kết hợp snap turn và Tunneling Vignette từ Starter Assets. Việc dùng xoay theo nấc thay vì xoay liên tục giúp giảm kích thích tiền đình.', body, False),
('3.3.2. Lập trình Grab và Raycast', h3, False),
('Các thành phần XR Interaction Toolkit được chuẩn bị để gắn XR Ray Interactor cho tay cầm và XR Grab Interactable cho những đối tượng cho phép thao tác. Raycast được dùng để trỏ đến nút giao diện hoặc hiện vật ở khoảng cách xa; đối tượng nhận tia có thể đổi trạng thái hiển thị để báo cho người dùng biết vị trí đang được chọn. Grab áp dụng cho vật thể nhỏ, có Collider và Rigidbody, bảo đảm thao tác cầm nắm tôn trọng va chạm. Với hiện vật trưng bày, nguyên tắc ưu tiên là ray chọn để xem thông tin, tránh làm thay đổi bố cục bảo tàng.', body, False),
('3.3.3. Tích hợp phản hồi xúc giác và âm thanh', h3, False),
('Phản hồi xúc giác được gắn vào các sự kiện chọn, nhấn nút và cầm vật thông qua thiết bị haptic của XR controller. Mỗi phản hồi nên ngắn, biên độ thấp và chỉ kích hoạt khi thao tác thành công để tránh gây khó chịu. Âm thanh được tổ chức theo hai lớp: âm thanh môi trường tạo không khí yên tĩnh của bảo tàng và âm thanh xác nhận khi tương tác. AudioSource đặt trong không gian ba chiều, kết hợp Audio Listener trên camera, giúp âm lượng và hướng âm thay đổi theo vị trí người dùng.', body, False),
('3.4. Xây dựng giao diện và chức năng ứng dụng', h2, False),
('3.4.1. Xây dựng Spatial UI', h3, False),
('Giao diện được đặt trong world space thay vì phủ kín màn hình. Mỗi bảng thông tin nằm gần hiện vật, sử dụng TextMesh Pro để hiển thị tên, dân tộc, giới tính và mô tả. Bảng được đặt ở độ cao và khoảng cách đọc thuận tiện, dùng nền có độ tương phản để chữ rõ trong điều kiện ánh sáng khác nhau. Khi phát triển tiếp, bảng có thể sử dụng billboard để quay về phía camera và nhận thao tác thông qua XR UI Input Module cùng ray từ tay cầm.', body, False),
('3.4.2. Lập trình các chức năng chính', h3, False),
('Chức năng trình bày thông tin hiện vật được hỗ trợ bởi script TrangPhucInfo. Mỗi đối tượng có trường moTa và tham chiếu bangMoTa, nhờ đó dữ liệu hiển thị được gắn trực tiếp với mô hình trong scene. Chức năng tham quan tự động được xây dựng bằng NpcThamQuan: script tìm các TrangPhucInfo có bảng mô tả, tạo danh sách điểm dừng hợp lệ trên NavMesh, sau đó điều khiển NavMeshAgent di chuyển, dừng quan sát và chọn điểm tiếp theo theo hướng di chuyển. NpcLimbMotion và NpcWalkMotion bổ sung chuyển động cơ thể để nhân vật trong bảo tàng tự nhiên hơn.', body, False),
('3.4.3. Quản lý trạng thái và dữ liệu', h3, False),
('Trạng thái phiên tham quan được quản lý ở mức scene: vị trí người dùng, đối tượng đang được trỏ chọn, bảng thông tin đang mở và trạng thái âm thanh. Dữ liệu hiện vật trước mắt được đóng gói theo component TrangPhucInfo để dễ chỉnh sửa trong Inspector, không phụ thuộc vào tên GameObject. Khi số lượng hiện vật tăng, dữ liệu này có thể chuyển sang ScriptableObject hoặc tệp JSON, còn component chỉ giữ khóa định danh và tham chiếu giao diện. Cách tách dữ liệu khỏi logic tương tác giúp bổ sung hiện vật mà không phải sửa luồng điều khiển.', body, False),
('3.5. Triển khai và tối ưu ứng dụng VR', h2, False),
('3.5.1. Tối ưu Assets và hiệu năng', h3, False),
('Tối ưu được thực hiện từ tài nguyên đến kết xuất. Không gian dùng các prefab low-poly, mô hình GLB và texture được kiểm soát kích thước; vật thể lặp lại được tái sử dụng prefab thay vì nhân bản tài nguyên. Với URP, ánh sáng cho đối tượng tĩnh nên được bake, chỉ giữ số lượng giới hạn đèn thời gian thực tại khu vực cần nhấn. Bên cạnh đó cần dùng occlusion culling, LOD cho mô hình xa, nén texture phù hợp thiết bị Android và tránh tạo cấp phát bộ nhớ trong Update. Các điều chỉnh này hướng đến duy trì khung hình ổn định, hạn chế say chuyển động.', body, False),
('3.5.2. Đóng gói và triển khai ứng dụng', h3, False),
('Ứng dụng được đóng gói từ Build Settings sau khi đưa SampleScene vào danh sách scene xây dựng. Đối với bản desktop, mục tiêu là Windows 64-bit; đối với kính độc lập, chuyển nền tảng sang Android, thiết lập OpenXR và cấu hình ký ứng dụng trước khi tạo APK. Cần kiểm tra lại Input Actions, quyền thiết bị, chất lượng URP và các shader được đưa vào build. Một bản build thử nghiệm được cài lên thiết bị để kiểm tra đầy đủ đường dẫn khởi tạo, ánh sáng, tương tác và âm thanh.', body, False),
('3.5.3. Kiểm tra khả năng tương thích thiết bị', h3, False),
('Khả năng tương thích được kiểm tra theo hai chế độ: desktop bằng bàn phím - chuột và VR qua OpenXR. Trên PC, cần kiểm tra điều khiển WASD, xoay camera, va chạm và tốc độ tải scene. Trên thiết bị VR, cần kiểm tra nhận diện đầu, tay cầm hoặc bàn tay, hướng ray, phản hồi rung, tỷ lệ khung hình và độ rõ văn bản. XR Simulation đóng vai trò kiểm thử nhanh trong Editor; tuy nhiên kết quả cuối cùng vẫn cần xác nhận trên kính mục tiêu vì năng lực GPU, độ phân giải và hệ điều hành có khác biệt.', body, False),
('3.6. Kết quả xây dựng ứng dụng', h2, False),
('3.6.1. Kết quả xây dựng môi trường VR', h3, False),
('Kết quả là một scene bảo tàng ba chiều có không gian nội thất, sàn, tường, vật liệu, ánh sáng và các đối tượng trang trí được bố trí theo hướng tham quan. Tài nguyên dự án đã được tổ chức rõ ràng theo nhóm scene, mô hình, vật liệu, script và cấu hình XR. URP, OpenXR, XR Interaction Toolkit, XR Hands cùng XR Simulation đã được thêm vào môi trường phát triển, tạo nền tảng để chạy trên desktop và tiếp tục triển khai trên thiết bị VR tương thích OpenXR.', body, False),
('3.6.2. Kết quả xây dựng các chức năng', h3, False),
('Ứng dụng có chế độ tham quan desktop với điều khiển góc nhìn và di chuyển bằng CharacterController; có cơ chế va chạm và trọng lực cơ bản. Các hiện vật có component lưu mô tả và bảng thông tin đi kèm. Nhân vật NPC có thể tự tìm các điểm trưng bày trên NavMesh, di chuyển tới từng điểm, xoay để quan sát rồi chọn điểm tiếp theo. Hệ thống XR Interaction Toolkit và Starter Assets đã sẵn sàng để tích hợp đầy đủ ray, grab, UI và haptics theo cấu hình đã nêu.', body, False),
('3.6.3. Đánh giá kết quả xây dựng', h3, False),
('Môi trường hiện tại đáp ứng mục tiêu tạo một không gian trưng bày ảo có thể tham quan và mở rộng. Điểm mạnh là sử dụng kiến trúc Unity phổ biến, tổ chức tài nguyên tương đối rõ, có sẵn pipeline URP và các gói XR chuẩn OpenXR. Các bước tiếp theo cần ưu tiên là gắn XR Origin và tương tác trực tiếp vào scene chính, hoàn thiện dữ liệu cho toàn bộ hiện vật, xây dựng menu world-space, kiểm thử trên kính thật và đo tốc độ khung hình trong từng khu trưng bày. Việc đánh giá này giúp nhóm chuyển từ mô hình trình diễn desktop sang trải nghiệm VR hoàn chỉnh một cách có kiểm soát.', body, False),
]
for text, template, page_break in content:
    anchor = clone_after(anchor, template, text, page_break=page_break)

doc.save(OUTPUT)
print(OUTPUT)

# -*- coding: utf-8 -*-
import docx
from docx.shared import Inches, Pt
from docx.enum.text import WD_ALIGN_PARAGRAPH

# Define all rewritten blocks with strict constraints:
# 1. No hyphens or dashes (-, –, —)
# 2. Minimal parentheses
# 3. Grounded in the project facts
# 4. Formatted with line_spacing = 1.15, first_line_indent = 0.5 inch, justify

REWRITTEN = {
    # 1.3.2 (paras 227-234 -> 2 paras)
    "sec_1_3_2": [
        "Trong môi trường phát triển Unity, ngôn ngữ lập trình C# giữ vai trò chủ đạo để hiện thực hóa toàn bộ các hành vi tương tác thông qua lớp cơ sở MonoBehaviour. Nhóm ứng dụng C# nhằm quản lý trạng thái, tự động nhận diện thiết bị đầu vào giữa kính thực tế ảo và máy tính, điều khiển cơ chế di chuyển mượt mà bám sàn, thực thi tương tác quang học bắn tia raycast, phát thuyết minh âm thanh theo khoảng cách và điều phối nhân vật ảo tìm đường trên lưới NavMesh. Nhằm duy trì tốc độ khung hình ổn định và loại bỏ hiện tượng giật hình trong thực tế ảo, nhóm áp dụng nghiêm ngặt kỹ thuật hạn chế cấp phát bộ nhớ động trong các vòng lặp cập nhật, tái sử dụng biến và mảng cấp phát sẵn để triệt tiêu tải thu gom rác tự động.",
        "Về công cụ soạn thảo, Visual Studio Community liên kết trực tiếp với Unity giúp tăng tốc quá trình xây dựng mã nguồn. Nhờ tính năng gợi ý mã thông minh IntelliSense và bộ công cụ Visual Studio Tools for Unity, nhóm có thể gắn trình gỡ lỗi trực tiếp vào tiến trình đang chạy trong Editor hoặc trên thiết bị thực tế ảo Meta Quest 2. Việc thiết lập các điểm dừng và theo dõi biến thời gian thực hỗ trợ chẩn đoán chính xác các lỗi logic, nâng cao vượt bậc năng suất và chất lượng kỹ thuật của toàn bộ dự án."
    ],

    # 2.1.1 (paras 254-260 -> 2 paras)
    "sec_2_1_1": [
        "Hệ thống hướng tới mục tiêu cho phép người dùng tự do tham quan không gian bảo tàng trưng bày trang phục truyền thống của 12 dân tộc Tây Nguyên gồm Ba Na, Brâu, Chư Ru, Cơ Ho, Ê Đê, Gia Rai, Giẻ Triêng, M Nông, Mạ, Rơ Măm, Xơ Đăng và Xtiêng. Mỗi tộc người có một cặp trang phục nam nữ đặt trên bệ chuyên dụng theo chuẩn nhân trắc học. Nhóm xác định năm nhóm yêu cầu chức năng cốt lõi phục vụ trải nghiệm tham quan.",
        "Các yêu cầu chức năng bao gồm: khởi tạo phiên tham quan tự động nhận diện phần cứng và nạp cảnh sảnh đón tiếp; điều hướng di chuyển mượt mà có cơ chế thu hẹp tầm nhìn chống say và va chạm vật lý chống xuyên tường; tra cứu thông tin hiện vật tại 12 bệ thông minh qua giao diện không gian, hiển thị ảnh thuyết minh chi tiết và phát giọng đọc tự điều chỉnh âm lượng theo khoảng cách; vận hành các hiệu ứng môi trường gồm hệ thống đèn rọi chiếu điểm, cửa trượt tự động chống kẹp và video tư liệu cồng chiêng tích hợp ánh sáng viền; cùng chế độ trình diễn trên máy tính cá nhân qua bàn phím và chuột."
    ],

    # 2.1.2 (paras 262-267 -> 2 paras)
    "sec_2_1_2": [
        "Để đảm bảo an toàn sinh lý và ngăn ngừa hội chứng say thực tế ảo, ứng dụng đặt ra các tiêu chuẩn phi chức năng nghiêm ngặt. Hệ thống yêu cầu duy trì tốc độ khung hình ổn định tối thiểu 72 khung hình trên giây trên kính Meta Quest 2 với thời gian kết xuất mỗi khung hình không vượt quá 13.88 mili giây, đồng thời đạt trên 60 khung hình trên giây trên máy tính cá nhân. Độ trễ chuyển động từ đầu đến màn hình bắt buộc phải dưới 20 mili giây thông qua cơ chế bù khung hình bất đồng bộ và kết xuất một lần hai mắt của đường ống URP.",
        "Về tính tương thích và độ bền, ứng dụng vận hành độc lập trên kính Meta Quest 2 chạy hệ điều hành Android với chip Snapdragon XR2, đồng thời hỗ trợ máy tính Windows có card đồ họa tương đương GTX 1060 trở lên. Các thành phần giao diện không gian được đặt trong vùng thị giác thoải mái từ 0.5 đến 1.5 mét với phông chữ độ tương phản cao chống mỏi mắt. Phiên tham quan được tối ưu hóa để vận hành liên tục trên 30 phút mà không rò rỉ bộ nhớ hay quá nhiệt phần cứng."
    ],

    # 2.2.1 (paras 272-273 -> 1 para)
    "sec_2_2_1": [
        "Biểu đồ ca sử dụng chuyển hóa các yêu cầu chức năng thành các luồng tương tác cụ thể giữa tác nhân người tham quan và hệ thống bảo tàng ảo. Người dùng có thể trải nghiệm thông qua kính thực tế ảo hoặc máy tính cá nhân. Hệ thống xác lập ba ca sử dụng cấp cao gồm bắt đầu phiên tham quan, di chuyển trong khuôn viên bảo tàng và xem thông tin hiện vật trang phục, tương ứng với toàn bộ vòng đời trải nghiệm của khách tham quan."
    ],

    # 2.1.1.2 (paras 279-281 -> 1 para)
    "sec_2_1_1_2": [
        "Ca sử dụng di chuyển trong bảo tàng cho phép người tham quan khám phá toàn bộ công trình ba chiều một cách tự nhiên. Chức năng này bao hàm việc khởi tạo phiên tham quan để nạp dữ liệu không gian và lưới va chạm, đồng thời mở rộng sang cơ chế kết nối tay cầm sáu bậc tự do. Người dùng có thể sử dụng cần gạt điều khiển để bước đi liên tục hoặc kích hoạt tia dịch chuyển tức thời tùy theo nhu cầu trải nghiệm cá nhân."
    ],

    # 2.2.1.3 (paras 286-287 -> 1 para)
    "sec_2_2_1_3": [
        "Ca sử dụng xem thông tin hiện vật giữ vai trò truyền tải tri thức văn hóa trọng tâm của đề tài. Khi người dùng tiếp cận bệ trưng bày trong không gian hợp lệ, hệ thống mở rộng các hành vi tương tác gồm hướng tia laser lên đối tượng, mở bảng thông tin đa phương tiện và lắng nghe giọng đọc thuyết minh. Giao diện tự động xoay mặt phẳng về phía tầm mắt người xem và tự động đóng lại khi người dùng di chuyển ra xa để giữ tầm nhìn thông thoáng."
    ],

    # 2.2.2 (paras 293-295 -> 1 para)
    "sec_2_2_2": [
        "Biểu đồ tuần tự mô tả trình tự gửi thông điệp thời gian thực giữa người dùng, bộ phát tia laser, bộ quản lý hiện vật, kho cấu hình dữ liệu và bảng hiển thị không gian ba chiều. Khi người dùng hướng tia ngắm vào bệ hiện vật và bóp nút chọn, bộ quản lý sẽ truy vấn dữ liệu trang phục tương ứng, đổ hình ảnh cùng văn bản mô tả lên bảng thông tin trong 0.22 giây và kích hoạt thuật toán xoay mặt phẳng về phía camera. Bảng thông tin sẽ tự động ẩn đi khi người dùng chủ động bấm đóng hoặc bước lùi ra xa quá phạm vi cho phép."
    ],

    # 2.2.3 (paras 299-301 -> 1 para)
    "sec_2_2_3": [
        "Biểu đồ hoạt động phác họa luồng xử lý nghiệp vụ xuyên suốt một phiên tham quan ảo. Ứng dụng khởi động bằng việc nạp cảnh và tự động rẽ nhánh kiểm tra thiết bị đầu vào, kích hoạt giàn camera thực tế ảo hoặc bộ điều khiển máy tính tương ứng. Người dùng bước vào chu trình tham quan chính gồm di chuyển khám phá các khu trưng bày của 12 dân tộc, kích hoạt xem tư liệu trang phục, nghe thuyết minh và theo dõi video cồng chiêng trước khi quay về sảnh để kết thúc phiên trải nghiệm."
    ],

    # 2.3.1 (paras 306-311 -> 2 paras)
    "sec_2_3_1": [
        "Hệ thống được tổ chức theo kiến trúc bốn lớp phân tách độc lập nhằm triệt tiêu sự phụ thuộc vòng và nâng cao tính module hóa. Lớp trình diễn chịu trách nhiệm hiển thị toàn bộ không gian kiến trúc bảo tàng, mô hình trang phục của 12 dân tộc, hệ thống 43 đèn rọi chuyên dụng, hiệu ứng hạt bụi sáng, màn hình video cồng chiêng và các bảng giao diện không gian sắc nét. Lớp tương tác tiếp nhận thao tác người dùng thông qua bộ công cụ XR Interaction Toolkit và hệ thống New Input System, xử lý cơ chế trỏ tia laser, chạm trực tiếp và di chuyển liên tục chống say.",
        "Lớp logic ứng dụng điều phối toàn bộ nghiệp vụ bảo tàng qua ngôn ngữ C#, quản lý chuyển đổi giàn camera kép giữa thực tế ảo và máy tính, kiểm soát đóng mở bảng thông tin, điều biến âm lượng thuyết minh theo cự ly, vận hành cửa trượt tự động và dẫn đường nhân vật ảo trên NavMesh. Lớp nền tảng dưới cùng gồm nhân engine Unity 6, đường ống URP, bộ quản lý OpenXR và hệ điều hành thiết bị, đảm bảo hình ảnh kết xuất đạt chuẩn 72 khung hình trên giây với độ trễ thấp."
    ],

    # 2.3.2 (paras 316-318 -> 2 paras)
    "sec_2_3_2": [
        "Nhóm lựa chọn Unity 6 kết hợp đường ống kết xuất Universal Render Pipeline URP nhằm đáp ứng yêu cầu tối ưu hiệu năng cho thiết bị di động độc lập như Meta Quest 2. Kiến trúc Render Graph hiện đại của URP cho phép loại bỏ các lượt vẽ không cần thiết, hỗ trợ đầy đủ cơ chế kết xuất một lần hai mắt giúp giảm một nửa số lượng lệnh vẽ từ vi xử lý trung tâm, đồng thời duy trì chất lượng đổ bóng và vật liệu PBR sắc sảo.",
        "Về bộ công cụ phát triển, nhóm chọn chuẩn mở OpenXR kết hợp XR Interaction Toolkit thay vì các bộ công cụ độc quyền phần cứng. Chuẩn OpenXR giúp mã nguồn có tính độc lập cao, dễ dàng mở rộng sang các thế hệ kính thực tế ảo khác nhau mà không cần sửa đổi logic tương tác. Hệ thống cũng tích hợp gói glTFast giải nén mô hình 3D bất đồng bộ và công cụ XR Simulation hỗ trợ kiểm thử tiện lợi ngay trên máy tính."
    ],

    # 2.4.1 (paras 321-324 -> 2 paras)
    "sec_2_4_1": [
        "Không gian bảo tàng ảo được thiết kế theo phong cách hiện đại và trang trọng, gồm sảnh đón tiếp quầy gỗ, hành lang kết nối mở, ba phân khu trưng bày chính, phòng chiếu phim tư liệu cồng chiêng và khu vực nghỉ ngơi có tiểu cảnh cây xanh. 24 bộ trang phục nam nữ của 12 dân tộc Tây Nguyên được đặt đối xứng trên các bệ gỗ vân gụ theo tỷ lệ nhân trắc học 1:1, cao 1.75m đối với nam và 1.65m đối với nữ, tạo điều kiện thuận lợi để người xem so sánh phom dáng và hoa văn thổ cẩm ở mọi góc nhìn.",
        "Vỏ kiến trúc bảo tàng được xây dựng từ các khối ghép tối ưu đa giác nhằm đảm bảo ngân sách kết xuất di động. Toàn bộ mô hình trang phục định dạng GLB được nén hình học để giảm hơn 60% dung lượng tệp tin mà vẫn giữ nguyên độ chi tiết của sợi vải. Hệ thống ánh sáng kết hợp giữa ánh sáng mặt trời nền nướng sẵn vào bản đồ ánh sáng và 43 đèn rọi chuyên dụng chiếu nghiêng 45 độ, kết hợp các đầu dò phản xạ môi trường và vật liệu chuẩn vật lý để tôn vinh sắc thái trang phục truyền thống."
    ],

    # 2.4.2 (paras 326-328 -> 2 paras)
    "sec_2_4_2": [
        "Người dùng được đại diện trong không gian ảo qua giàn XR Origin sáu bậc tự do. Cơ chế di chuyển mặc định là bước đi liên tục theo hướng nhìn của đầu với vận tốc an toàn 2.5 m/s, kết hợp hiệu ứng thu hẹp tầm nhìn ngoại vi tự động làm mờ rìa mắt khi tịnh tiến nhằm triệt tiêu cảm giác say chuyển động. Góc nhìn được hỗ trợ xoay nhanh từng nấc 30 đến 45 độ, kết hợp khối vỏ va chạm vật lý ngăn chặn triệt để tình trạng đi xuyên tường hay lọt vào trong bệ trưng bày.",
        "Mỗi tay cầm được trang bị bộ phát tia laser quang học có đường nét thanh mảnh và vòng ngắm điểm rơi, cho phép bấm các nút giao diện từ xa và làm nổi bật viền bệ hiện vật khi rê ngắm. Hệ thống cũng hỗ trợ tương tác chạm ngón tay trực tiếp vào bảng điều khiển ở sảnh tiếp đón, cùng cơ chế cầm nắm cho phép nhặt và xoay ngắm các hiện vật nhỏ ở cự ly gần theo đúng quy luật vật lý."
    ],

    # 2.4.3 (paras 330-333 -> 2 paras)
    "sec_2_4_3": [
        "Giao diện ứng dụng tuân thủ triết lý không gian ba chiều, gắn trực tiếp các bảng thông tin vào môi trường thay vì dùng menu cố định màn hình. Tại mỗi bệ hiện vật, cụm giao diện được đặt ở tầm ngực từ 1.2 đến 1.4m với cự ly đọc thoải mái. Nhóm áp dụng công nghệ phông chữ TextMesh Pro sắc nét trên nền xám tối bán trong suốt, kết hợp thuật toán tự động xoay hướng về phía mắt người xem để loại bỏ hiện tượng đọc lệch góc.",
        "Hệ thống phản hồi xúc giác rung trên tay cầm được phân tầng tinh tế theo nguyên tắc ngắn gọn và nhất quán, cung cấp xung rung nhẹ 30 mili giây khi rê tia ngắm và xung rung 100 mili giây khi mở bảng thông tin. Toàn bộ thao tác đều được đồng bộ hóa với âm thanh bấm nút, tiếng chuông mở cửa và nhạc nền cồng chiêng vang vọng, mang lại trải nghiệm hòa quyện giữa thị giác, thính giác và xúc giác."
    ],

    # 3.1.1 (paras 338-342 -> 2 paras)
    "sec_3_1_1": [
        "Để phục vụ mục tiêu vừa trình chiếu trên máy tính vừa triển khai độc lập trên kính thực tế ảo, nhóm trang bị trạm làm việc phát triển với vi xử lý Intel Core i7 11800H, bộ nhớ 16GB RAM, ổ cứng SSD NVMe PCIe 4.0 và card đồ họa NVIDIA GeForce RTX 3060 6GB GDDR6. Cấu hình này đáp ứng đầy đủ yêu cầu nạp cảnh phức tạp, dựng hình thời gian thực và biên dịch mã nguồn C# nhanh chóng.",
        "Thiết bị thử nghiệm thực địa là kính thực tế ảo Meta Quest 2 trang bị vi xử lý Snapdragon XR2, màn hình độ phân giải 1832 nhân 1920 điểm ảnh mỗi mắt và hệ thống định vị sáu bậc tự do. Nhóm kết nối kiểm thử qua cáp quang học chuyên dụng băng thông 2.4 Gbps và mạng không dây Wi Fi 6 băng tần 5GHz. Trong điều kiện làm việc từ xa, công cụ XR Simulation trong Unity Editor được kích hoạt để giả lập đầy đủ thao tác đầu và tay cầm ngay trên màn hình máy tính."
    ],

    # 3.1.2 (paras 344-351 -> 2 paras)
    "sec_3_1_2": [
        "Dự án sử dụng phiên bản Unity 6 ổn định dài hạn kết hợp đường ống Universal Render Pipeline phiên bản 17.5.0, mang lại sự tối ưu về kiến trúc đồ họa và hiệu năng đa luồng. Thông qua trình quản lý gói UPM, nhóm tích hợp các gói thư viện chuẩn gồm OpenXR hỗ trợ giao tiếp phần cứng, XR Interaction Toolkit phiên bản 3.5.1 cung cấp giàn camera và các bộ tương tác tia laser, XR Management quản lý vòng đời thiết bị, và XR Hands sẵn sàng cho tính năng theo dõi bàn tay.",
        "Hệ thống đầu vào được quản lý bởi New Input System cho phép ánh xạ linh hoạt giữa phím bấm máy tính và các nút bấm, cần gạt của tay cầm thực tế ảo. Ngoài ra, gói mở rộng glTFast và bộ giải mã meshopt được bổ sung để nạp các mô hình trang phục định dạng GLB vào bộ nhớ một cách bất đồng bộ, hoàn toàn không gây giật khung hình khi người dùng di chuyển trong không gian triển lãm."
    ],

    # 3.1.3 (paras 353-357 -> 2 paras)
    "sec_3_1_3": [
        "Nhóm kích hoạt OpenXR làm nhà cung cấp mặc định cho cả hai nền tảng Windows và Android Meta Quest, đồng thời nạp hồ sơ tương tác của tay cầm Touch Controller. Không gian màu sắc được chuyển sang chế độ Linear để đảm bảo các phép toán chiếu sáng và độ bóng vật liệu diễn ra chuẩn xác theo quang học. Chế độ kết xuất lập thể được thiết lập sang Single Pass Instanced, giúp vẽ đồng thời hình ảnh cho cả hai mắt trong một lượt dựng hình và giảm 50% số lượng lệnh vẽ từ CPU.",
        "Tần số tính toán vật lý cố định được điều chỉnh về 0.013888 giây tương ứng với tần số quét 72Hz của kính Meta Quest 2, loại bỏ hiện tượng rung giật khi va chạm. Cấu trúc thư mục dự án được tổ chức khoa học thành các thư mục riêng biệt dành cho cảnh chính, mã nguồn C#, mô hình kiến trúc nội thất, 24 mô hình trang phục GLB, vật liệu chuẩn vật lý và âm thanh thuyết minh."
    ],

    # 3.2.1 (paras 360-363 -> 2 paras)
    "sec_3_2_1": [
        "Không gian bảo tàng được xây dựng hoàn chỉnh trong cảnh chính của dự án dựa trên bộ mô đun kiến trúc LowPolyInterior2. Mặt bằng gồm sảnh đón tiếp sang trọng có vách kính nhìn ra sân cảnh quan cây xanh, lối đi trung tâm rộng mở kết nối ba phân khu trưng bày, phòng chiếu phim tư liệu cồng chiêng bố trí ghế sofa nệm và khu vực nghỉ chân thoáng đãng ở cuối hành lang.",
        "Hiện vật trang phục của 12 dân tộc Tây Nguyên được bài trí trên 12 bệ gỗ chuyên dụng với cặp ma nơ canh nam nữ đứng đối xứng, mặc đầy đủ khố, váy, áo chui đầu và trang sức cổ truyền. Nhóm sử dụng công cụ chuẩn hóa hình học tự động để đưa chiều cao mô hình về đúng tỷ lệ thực tế gồm 1.75m cho nam và 1.65m cho nữ. Khoảng cách giữa các bệ được duy trì tối thiểu 3.0m giúp người tham quan dễ dàng di chuyển và quan sát 360 độ mà không bị che khuất."
    ],

    # 3.2.2 (paras 365-368 -> 2 paras)
    "sec_3_2_2": [
        "Ánh sáng bảo tàng là sự kết hợp hài hòa giữa ánh sáng tĩnh nướng sẵn và ánh sáng thời gian thực. Nguồn sáng mặt trời tự nhiên dịu nhẹ chiếu qua mái kính giếng trời tạo nền ấm cúng cho toàn bộ công trình. Nhóm sử dụng công cụ Progressive GPU Lightmapper nướng toàn bộ ánh sáng gián tiếp và bóng đổ mềm vào bản đồ ánh sáng, mang lại chiều sâu quang học chân thực mà không tốn tài nguyên xử lý đồ họa khi vận hành.",
        "Điểm nhấn thị giác được tạo bởi 43 đèn rọi chuyên dụng gắn trên ray trần kim loại, chiếu góc nghiêng 45 độ vào từng bệ hiện vật với nhiệt độ màu ấm 3200K để làm nổi bật sắc đỏ chàm của thổ cẩm. Hệ thống vật liệu PBR được tinh chỉnh tỉ mỉ với độ bóng sàn gỗ, độ trong suốt của vách kính và độ thô ráp của sợi vải dệt, kết hợp sáu đầu dò phản xạ môi trường và hiệu ứng hậu kỳ nhẹ nhàng tạo nên không gian trưng bày trang nhã."
    ],

    # 3.2.3 (paras 370-373 -> 2 paras)
    "sec_3_2_3": [
        "Hệ giàn người dùng được khởi tạo từ mẫu XR Origin với chế độ theo dõi sàn, tự động căn chỉnh chiều cao ảo trùng khớp với chiều cao thật của người đeo kính. Cấu trúc gồm đối tượng trung gian bù trừ vị trí, camera chính hiển thị hai mắt lập thể tích hợp bộ thu nhận âm thanh không gian, và hai bộ điều khiển tay cầm ánh xạ dữ liệu vị trí sáu bậc tự do. Thân người chơi được bao bọc bởi khối hình trụ CharacterController với bán kính 0.35m và chiều cao 1.75m để xử lý va chạm chống xuyên tường.",
        "Nhóm xây dựng script quản lý trung tâm MuseumVRRigManager hoạt động song song với thư viện nhận diện thiết bị. Khi ứng dụng khởi chạy, hệ thống tự động kiểm tra sự hiện diện của kính thực tế ảo OpenXR để kích hoạt giàn XR Origin và tắt bộ điều khiển máy tính, hoặc ngược lại sẽ kích hoạt bộ điều khiển chuột bàn phím nếu chạy trên máy tính thông thường, đảm bảo tính tiện dụng tuyệt đối."
    ],

    # 3.3.1 (paras 376-379 -> 2 paras)
    "sec_3_3_1": [
        "Trong môi trường thực tế ảo, cơ chế bước đi liên tục được điều khiển thông qua cần gạt bên tay trái theo hướng nhìn của đầu. Véc tơ chuyển động được tính toán trong không gian camera chính và chiếu lên mặt phẳng sàn ngang để loại bỏ thành phần chúc đầu, giúp người dùng bước đi tự nhiên theo tốc độ chuẩn 2.5 m/s. Để bảo vệ tiền đình, hiệu ứng thu hẹp tầm nhìn ngoại vi tự động làm mờ tối viền mắt khi vận tốc lớn hơn 0.1 m/s, giúp giảm thiểu tối đa hiện tượng say chuyển động.",
        "Đối với chế độ máy tính để bàn, script FPSController tiếp nhận tín hiệu từ cụm phím WASD để di chuyển nhân vật ở tốc độ 3.0 m/s và tăng tốc lên 6.0 m/s khi giữ phím Shift. Hướng nhìn được xoay tự do theo chuột máy tính, đồng thời trọng lực nhân tạo được duy trì liên tục ở mỗi khung hình giúp nhân vật luôn bám chắc mặt sàn và bước qua các bậc thềm êm ái."
    ],

    # 3.3.2 (paras 381-384 -> 2 paras)
    "sec_3_3_2": [
        "Cơ chế trỏ chọn quang học sử dụng tia laser phát ra từ mỗi tay cầm với tầm xa 10 mét, hiển thị dưới dạng đường sáng màu lam nhạt kèm vòng tròn ngắm trên bề mặt vật thể. Khi tia laser chạm vào bệ hiện vật, hệ thống lọc va chạm qua mặt nạ lớp chuyên dụng để bỏ qua tường sàn, lập tức kích hoạt viền sáng nổi bật và phát xung rung nhẹ lên tay cầm báo hiệu khả năng tương tác. Thao tác bóp cò ngón trỏ sẽ mở bảng thông tin hoặc bật giọng đọc thuyết minh.",
        "Bên cạnh tương tác từ xa, cơ chế cầm nắm vật thể được tích hợp sẵn cho các hiện vật nhỏ như chiêng mini hay nhạc cụ mô phỏng. Khi đưa tay lại gần trong cự ly 0.3 mét và giữ nút kẹp ngón giữa, người dùng có thể nhặt hiện vật lên để xoay ngắm chi tiết ba chiều ở mọi góc độ, và nhả nút để đặt vật thể về vị trí ban đầu theo đúng định luật va chạm vật lý."
    ],

    # 3.3.3 (paras 386-391 -> 2 paras)
    "sec_3_3_3": [
        "Phản hồi rung trên tay cầm được phân tầng rõ rệt nhằm mang lại cảm giác chân thực: rung nhẹ với biên độ 0.10 trong 30 mili giây khi rê tia laser qua nút bấm, rung vừa 0.35 trong 80 mili giây khi bật tắt thuyết minh, và rung dứt khoát 0.45 trong 100 mili giây khi mở bảng thông tin chi tiết. Sự phản hồi cơ học tức thời này giúp người dùng dễ dàng cảm nhận sự xác nhận của hệ thống trong không gian ảo.",
        "Kiến trúc âm thanh đa tầng ứng dụng công nghệ âm thanh không gian ba chiều với ba lớp âm thanh độc lập. Lớp âm thanh môi trường tĩnh tái hiện tiếng gió nhẹ và tiếng chim rừng thanh bình; lớp âm thanh video cồng chiêng trong phòng chiếu lan tỏa theo khoảng cách thực; và lớp thuyết minh giọng đọc trong trẻo tự động điều chỉnh âm lượng đầy đủ trong bán kính 3.2m, giảm dần đến 5.5m và tắt hẳn khi ra xa để giữ sự tĩnh lặng cho các gian trưng bày xung quanh."
    ],

    # 3.4.1 (paras 394-397 -> 2 paras)
    "sec_3_4_1": [
        "Hệ thống giao diện không gian được thiết lập trực tiếp tại mép trước của từng bệ hiện vật ở độ cao 1.1 mét so với sàn nhà. Cụm điều khiển gồm hai biểu tượng ba chiều trực quan đặt song song: biểu tượng ngón tay chỉ dẫn bên trái và biểu tượng loa phát thanh bên phải. Cả hai biểu tượng tự động xuất hiện khi người tham quan tiến vào cự ly 3.2 mét và tự động ẩn đi khi bước ra xa để giữ không gian triển lãm thông thoáng.",
        "Thuật toán xoay mặt phẳng thời gian thực giúp các biểu tượng và bảng thông tin luôn tự động quay trực diện về phía mắt người xem quanh trục thẳng đứng. Nhóm bổ sung hiệu ứng nhấp nhô nhẹ nhàng cho biểu tượng ngón tay để thu hút sự chú ý, cùng hiệu ứng nhấp nháy phát sáng màu vàng cam ấm áp cho biểu tượng loa khi bài thuyết minh đang phát, giúp người dùng nắm bắt trạng thái hoạt động trong tích tắc."
    ],

    # 3.4.2 (paras 399-403 -> 2 paras)
    "sec_3_4_2": [
        "Các chức năng nghiệp vụ được phân tách rõ ràng qua hệ thống kịch bản C#. Script quản lý thông tin di sản kết hợp hiệu ứng phóng to mượt mà của bảng ảnh thuyết minh hai chiều từ kích thước 0 lên kích thước chuẩn trong 0.22 giây ngay giữa tầm nhìn người xem. Hệ thống cửa kính trượt tự động sử dụng cảm biến khoảng cách hình cầu bán kính 2.8 mét để mở cửa sang hai bên, tích hợp cơ chế chống kẹp an toàn giữ cửa mở khi có người đứng giữa lối đi.",
        "Sức sống của viện bảo tàng được nâng cao nhờ nhân vật khách tham quan ảo di chuyển trên lưới NavMesh theo máy trạng thái hữu hạn ba giai đoạn gồm bước đi tuần tự qua các bệ, dừng lại quan sát hiện vật từ 4 đến 8 giây và nghỉ ngơi tại sảnh. Tại phòng cồng chiêng, script điều khiển màn hình tivi lớn phát video tư liệu chuẩn Full HD kèm thuật toán đồng bộ màu sắc ánh sáng đèn viền Ambilight theo khung hình, tạo nên không gian trải nghiệm sống động."
    ],

    # 3.4.3 (paras 405-408 -> 2 paras)
    "sec_3_4_3": [
        "Trạng thái phiên tham quan được quản lý tập trung qua mô hình Singleton kết hợp các sự kiện ủy nhiệm C#, kiểm soát đồng bộ chế độ hiển thị, bệ hiện vật mục tiêu, trạng thái đóng mở bảng ảnh và bài thuyết minh đang phát. Việc sử dụng cơ chế lắng nghe sự kiện giúp triệt tiêu sự ràng buộc cứng giữa các thành phần mã nguồn, kết hợp bộ lọc thời gian thực cách nhau tối thiểu 0.25 giây để loại bỏ hoàn toàn hiện tượng bấm nhầm lặp sự kiện.",
        "Toàn bộ dữ liệu về lịch sử, tên gọi, hoa văn, hình ảnh và âm thanh của 12 dân tộc Tây Nguyên được đóng gói độc lập theo cấu trúc ScriptableObject. Nhờ phân tách rạch ròi giữa dữ liệu và giao diện trình diễn, nhóm có thể dễ dàng cập nhật thông tin văn hóa, thay đổi âm thanh hoặc bổ sung hiện vật mới trực tiếp trong cửa sổ thuộc tính mà không cần biên dịch lại mã nguồn."
    ],

    # 3.5.1 (paras 411-414 -> 2 paras)
    "sec_3_5_1": [
        "Chiến lược tối ưu hóa hiệu năng được triển khai toàn diện trên lưới đa giác, kết cấu ảnh và đường ống kết xuất. Toàn bộ mô hình kiến trúc nội thất và trang phục được tinh giản lưới trong Blender để kiểm soát số lượng tam giác hiển thị dưới ngưỡng an toàn 350.000 tam giác. Kết cấu ảnh môi trường được nén về độ phân giải phù hợp theo chuẩn ASTC cho kính Meta Quest 2 và BC7 cho máy tính, đồng thời kích hoạt chuỗi ảnh thu nhỏ Mipmaps để chống răng cưa quang sai.",
        "Script tối ưu hiệu năng tự động tắt đổ bóng thời gian thực trên các nguồn sáng phụ và chuẩn hóa vật liệu về URP Lit nhẹ. Kỹ thuật nướng dữ liệu loại bỏ vật thể che khuất Occlusion Culling kết hợp cùng phương thức kết xuất một lần hai mắt và ghép lô tĩnh đã cắt giảm số lượng lệnh vẽ Draw Calls toàn cảnh từ hơn 850 batches xuống chỉ còn dưới 270 batches, giải phóng tối đa tải xử lý cho vi xử lý trung tâm."
    ],

    # 3.5.2 (paras 416-418 -> 2 paras)
    "sec_3_5_2": [
        "Đối với kính thực tế ảo Meta Quest 2, dự án được chuyển nền tảng sang Android với kiến trúc ARM64 và trình biên dịch mã máy IL2CPP. Việc chuyển đổi từ mã trung gian sang mã máy gốc giúp tăng tốc độ thực thi từ 1.5 đến 2.5 lần và tiết kiệm bộ nhớ đáng kể. Ứng dụng được đóng gói thành tệp tin cài đặt APK có dung lượng 2.05 GB và cài đặt trực tiếp vào kính thông qua kết nối gỡ lỗi ADB.",
        "Đối với môi trường máy tính cá nhân Windows 64 bit, hệ thống xuất bản gói thực thi độc lập có khả năng tự động thích ứng độ phân giải màn hình Full HD hoặc 2K ở chế độ toàn màn hình không viền, sẵn sàng phục vụ trình chiếu và báo cáo đồ án mà không đòi hỏi cài đặt phụ thuộc phức tạp."
    ],

    # 3.5.3 (paras 420-422 -> 2 paras)
    "sec_3_5_3": [
        "Quá trình kiểm tra tính tương thích được tiến hành song song trên cả hai môi trường vận hành. Trên máy tính cá nhân từ máy xách tay văn phòng đến máy trạm đồ họa RTX 3060, nhóm xác nhận tốc độ nạp cảnh nhanh, thao tác chuột bàn phím mượt mà, video phát ổn định và cơ chế chuyển đổi tự động khi không cắm kính hoạt động chính xác.",
        "Trên kính thực tế ảo Meta Quest 2, ứng dụng được kiểm định qua cả kết nối cáp quang và gói cài đặt độc lập. Hệ thống bắt sóng định vị sáu bậc tự do chính xác trong các điều kiện ánh sáng phòng khác nhau, tia laser trỏ chọn nhạy bén, phản hồi rung xúc giác dứt khoát và văn bản tiếng Việt hiển thị rõ nét trên thấu kính thực tế ảo."
    ],

    # 3.6.1 (paras 425-427 -> 1 para)
    "sec_3_6_1": [
        "Kết quả của giai đoạn xây dựng là một không gian bảo tàng ba chiều hoàn chỉnh, bề thế và mang đậm bản sắc văn hóa Tây Nguyên. Công trình sở hữu bố cục không gian liên hoàn từ sảnh tiếp đón trang nhã, hệ thống cửa trượt tự động tinh tế đến các gian trưng bày rộng rãi với nền sàn gỗ gụ ấm cúng. Toàn bộ 24 bộ trang phục truyền thống nam nữ của 12 dân tộc Tây Nguyên được số hóa và bài trí trang nghiêm trên 12 bệ hiện vật độc lập dưới ánh sáng vàng của 43 đèn rọi chuyên dụng, kết hợp hài hòa cùng phòng chiếu phim tư liệu cồng chiêng và khu vực nghỉ ngơi tiện nghi."
    ],

    # 3.6.2 (paras 429-432 -> 2 paras)
    "sec_3_6_2": [
        "Ứng dụng đã hoàn thiện đồng bộ toàn bộ các chức năng đề ra trong bản đặc tả kỹ thuật. Cơ chế điều khiển kép vận hành ổn định, hỗ trợ di chuyển tự nhiên theo hướng nhìn kết hợp hiệu ứng thu hẹp tầm nhìn chống say trên kính thực tế ảo, đồng thời cung cấp chế độ bàn phím chuột linh hoạt trên máy tính.",
        "Hệ thống bệ hiện vật thông minh tích hợp cụm biểu tượng kép tự động xuất hiện và hướng về người xem, cho phép mở bảng ảnh thuyết minh chi tiết trong 0.22 giây và phát âm thanh giọng đọc với cơ chế điều biến âm lượng theo khoảng cách chuẩn xác. Cửa kính trượt tự động chống kẹp, nhân vật khách tham quan ảo di chuyển trên NavMesh và phòng chiếu video cồng chiêng Ambilight đều vận hành ăn khớp, tạo nên không gian trải nghiệm sinh động."
    ],

    # 3.6.3 (paras 434-436 -> 2 paras)
    "sec_3_6_3": [
        "Đánh giá tổng thể cho thấy dự án đạt bước tiến vững chắc về kỹ thuật phần mềm và giá trị bảo tồn di sản. Kiến trúc phân tầng module hóa cao tuân thủ chuẩn mở OpenXR và Unity 6 giúp ứng dụng vận hành ổn định trên cả kính thực tế ảo và máy tính để bàn. Quy trình tối ưu hóa đường ống kết xuất và nén tài nguyên đã giải quyết triệt để nút thắt cổ chai, đảm bảo tốc độ khung hình 72 khung hình trên giây mượt mà trên phần cứng di động.",
        "Nhóm cũng ghi nhận một số điểm cần tiếp tục phát triển gồm: hoàn thiện dữ liệu thu âm giọng đọc cho đầy đủ 12 dân tộc, nghiên cứu nâng cấp bảng ảnh hai chiều thành mô hình ba chiều tương tác cho phép xoay ngắm và mặc thử ảo, đồng thời tiếp tục tối ưu hóa dung lượng gói cài đặt tệp tin để phục vụ triển khai rộng rãi."
    ],

    # 4.1.1 (paras 440-442 -> 2 paras)
    "sec_4_1_1": [
        "Giai đoạn kiểm thử nhằm xác minh tính đúng đắn của toàn bộ các tính năng tương tác, kiểm chứng độ ổn định của khung hình kết xuất và đo lường trải nghiệm thực tế của người dùng. Ba trọng tâm chính gồm: xác nhận đầy đủ các ca sử dụng chức năng, đo đạc định lượng tốc độ khung hình và độ trễ chuyển động, cùng việc thu thập phản hồi khách quan qua thang đo chuẩn quốc tế SUS.",
        "Phạm vi kiểm thử bao trùm toàn bộ không gian kiến trúc trong cảnh chính của dự án, từ sảnh tiếp đón, 12 bệ trưng bày trang phục đại diện cho 12 dân tộc Tây Nguyên, phòng chiếu phim tư liệu cồng chiêng cho đến khu vực nghỉ chân của khách tham quan."
    ],

    # 4.1.2 (paras 444-447 -> 2 paras)
    "sec_4_1_2": [
        "Quá trình kiểm thử được thực hiện trên hai môi trường phần cứng mục tiêu. Môi trường thực tế ảo độc lập sử dụng kính Meta Quest 2 trang bị chip Snapdragon XR2, 6GB RAM và màn hình tần số quét 72Hz, kiểm thử qua cả gói cài đặt APK nhị phân và cáp quang học chuyên dụng băng thông 2.4 Gbps. Môi trường máy tính trạm sử dụng vi xử lý Intel Core i7 11800H, card đồ họa RTX 3060 6GB và bộ nhớ 16GB RAM chạy hệ điều hành Windows 11.",
        "Nhóm khai thác bộ công cụ chuyên dụng OVR Metrics Tool chạy trực tiếp trên kính để đo tốc độ khung hình và độ trễ thời gian thực, kết hợp Unity Profiler phân tích mức tiêu hao bộ nhớ cùng Frame Debugger kiểm soát chi tiết từng lượt vẽ trong quy trình dựng hình."
    ],

    # 4.1.3 (paras 449-453 -> 2 paras)
    "sec_4_1_3": [
        "Nhóm phối hợp ba phương pháp kiểm thử gồm: kiểm thử chức năng hộp đen bao phủ toàn bộ các kịch bản sử dụng và trường hợp ngoại lệ; kiểm thử hiệu năng định lượng lấy mẫu liên tục tại 5 phân khu không gian trong phiên trải nghiệm tối thiểu 15 phút; và đánh giá trải nghiệm người dùng thực tế với nhóm mẫu 12 sinh viên qua bảng khảo sát chuẩn hóa SUS.",
        "Tiêu chí nghiệm thu bắt buộc yêu cầu 100% ca kiểm thử chức năng đạt kết quả tốt không phát sinh lỗi đóng ứng dụng, tốc độ khung hình trên kính Meta Quest 2 duy trì ổn định 72 khung hình trên giây với thời gian kết xuất dưới 13.88 mili giây, độ trễ chuyển động dưới 20 mili giây, và điểm đánh giá khả dụng đạt trên 80 điểm."
    ],

    # 4.2.1 pre-table (paras 456-459 -> 2 paras)
    "sec_4_2_1_pre": [
        "Nhóm tiến hành xác minh cơ chế quản lý khởi động và thư viện nhận diện thiết bị đầu vào khi ứng dụng kích hoạt trên các phần cứng khác nhau. Khi chạy trên kính Meta Quest 2, hệ thống truy vấn thành công trạng thái phần cứng, kích hoạt ngay giàn camera thực tế ảo sáu bậc tự do và tự động vô hiệu hóa bộ điều khiển máy tính, ngăn chặn xung đột âm thanh và tránh lãng phí tài nguyên dựng hình hai lần.",
        "Khi khởi chạy trên máy tính thông thường, hệ thống tự động nhận diện sự vắng mặt của thiết bị thực tế ảo, kích hoạt bộ điều khiển bàn phím chuột cho phép xoay góc nhìn mượt mà và di chuyển nhân vật linh hoạt. Hệ thống New Input System ghi nhận chính xác toàn bộ trạng thái bấm và giữ của tất cả các nút trên tay cầm điều khiển."
    ],

    # 4.2.1 post-table (paras 461-462 -> 2 paras)
    "sec_4_2_1_post": [
        "Kết quả kiểm thử cho thấy cơ chế quản lý giàn camera kép vận hành cực kỳ ổn định, mang lại tính tiện dụng tối đa cho việc chuyển đổi linh hoạt giữa kiểm thử kỹ thuật và trình diễn đồ án.",
        "Hình 4.1 thể hiện phối cảnh kiến trúc tổng thể bên ngoài của bảo tàng ảo trang phục truyền thống Tây Nguyên khi người dùng bước ra khu vực sân trước. Công trình mang phong cách kiến trúc hiện đại, vững chãi với biển tên trang trọng bằng đá hoa cương đen, hệ thống đèn hắt chân tường tinh tế kết hợp khuôn viên sân cỏ xanh mát tạo ấn tượng uy nghiêm của một không gian bảo tàng văn hóa đương đại."
    ],

    # 4.2.2 pre-table (paras 466-470 -> 2 paras)
    "sec_4_2_2_pre": [
        "Thử nghiệm cơ chế di chuyển mượt mà theo hướng nhìn trên kính thực tế ảo cho thấy người chơi bước đi êm ái trên sàn bảo tàng ở vận tốc 2.5 m/s. Hiệu ứng thu hẹp tầm nhìn ngoại vi kích hoạt tức thời khi bắt đầu bước đi và tự động mở rộng khi dừng lại, mang lại cảm giác thoải mái cho mắt. Khối va chạm vật lý hoạt động chính xác, ngăn chặn triệt để tình trạng người chơi đi xuyên tường hay lọt vào trong bệ hiện vật.",
        "Hệ thống cửa kính trượt tự động nhận diện người chơi trong phạm vi 2.8 mét và mở êm ái sang hai bên trong 1.5 giây, đồng thời giữ nguyên trạng thái mở khi có người đứng giữa lối đi để đảm bảo an toàn chống kẹp. Nhân vật khách tham quan ảo di chuyển tuần tự nhịp nhàng trên lưới NavMesh, dừng lại ngắm hiện vật từ 4 đến 8 giây trước khi tiếp tục hành trình mà không bị mắc kẹt vào chướng ngại vật."
    ],

    # 4.2.2 post-table (paras 472-473 -> 2 paras)
    "sec_4_2_2_post": [
        "Các tính năng di chuyển và tương tác môi trường đều đạt độ hoàn thiện cao, đảm bảo an toàn và tự nhiên trong không gian ảo.",
        "Hình 4.2 minh họa khu vực sảnh đón tiếp chính của bảo tàng ảo, nơi đặt quầy lễ tân sang trọng bằng gỗ sồi với hai máy tính đón tiếp, hệ thống cửa kính trượt tự động phân cách với sân ngoài, các chậu cây cảnh xanh mát cùng các bức tranh áp phích giới thiệu tổng quan về trang phục và văn hóa các dân tộc Tây Nguyên treo trang trọng trên tường."
    ],

    # 4.2.3 pre-table (paras 477-479 -> 2 paras)
    "sec_4_2_3_pre": [
        "Ca kiểm thử tập trung vào hệ thống giao diện không gian và tính năng tra cứu thông tin hiện vật của 12 dân tộc Tây Nguyên. Tại mỗi bệ trưng bày ở cự ly 3.2 mét, cụm biểu tượng kép gồm ngón tay chỉ dẫn nhấp nhô và biểu tượng loa phát thanh tự động hiển thị và xoay hướng về phía người xem. Thao tác bấm nút mở bảng thông tin cho phép xem chi tiết tên gọi và hình ảnh trang phục của đầy đủ 12 dân tộc gồm Ba Na, Brâu, Chư Ru, Cơ Ho, Ê Đê, Gia Rai, Giẻ Triêng, M Nông, Mạ, Rơ Măm, Xơ Đăng và Xtiêng.",
        "Biểu tượng loa cho phép bật tắt bài thuyết minh giọng đọc mà không làm che khuất tầm nhìn, tự động phát nhịp điệu ánh sáng vàng ấm khi âm thanh đang phát. Thử nghiệm thực tế xác nhận 100% bệ hiện vật hiển thị chính xác dữ liệu, hiệu ứng mở bảng diễn ra mượt mà trong 0.22 giây và tự động thu nhỏ ẩn đi khi người dùng lùi xa quá 4.2 mét."
    ],

    # 4.2.3 post-table (paras 482-484 -> 3 paras)
    "sec_4_2_3_post": [
        "Hình 4.3 thể hiện không gian trưng bày trang phục truyền thống của 12 dân tộc Tây Nguyên với góc nhìn bao quát từ sảnh chính. Các bệ trưng bày bằng gỗ vân gụ được bố trí ngay ngắn, hệ thống ma nơ canh nam nữ đứng uy nghiêm dưới ánh sáng rọi điểm của dàn đèn Gallery Spotlights, phía sau là các pano ảnh tư liệu khổ lớn về đời sống buôn làng tạo nên một không gian triển lãm trang trọng và giàu tính nghệ thuật.",
        "Hình 4.4 minh họa cận cảnh bệ trưng bày hiện vật tích hợp giao diện tương tác kép gồm biểu tượng ngón tay trỏ và biểu tượng loa phát thanh cùng bảng thuyết minh nghiêng đặt cạnh cặp ma nơ canh, cho thấy sự tiện lợi tối đa khi khách tham quan vừa có thể ngắm nhìn trang phục ở cự ly gần vừa dễ dàng thao tác tra cứu thông tin.",
        "Bảng 4.3 tổng hợp chi tiết kết quả nghiệm thu của các ca kiểm thử liên quan đến giao diện không gian Spatial UI và chức năng tra cứu thông tin hiện vật."
    ],

    # 4.2.4 pre-table (para 489 -> 1 para)
    "sec_4_2_4_pre": [
        "Hệ thống âm thanh của bảo tàng ảo được xây dựng đa tầng, đáp ứng cả tiêu chí sống động và tính rõ ràng của thông tin thuyết minh. Giọng đọc thuyết minh được tối ưu hóa độ trong trẻo trên hai kênh stereo, tự động duy trì âm lượng chuẩn trong bán kính 3.2 mét, giảm dần đến 5.5 mét và ngắt hoàn toàn khi đi xa hơn để tránh xung đột tiếng ồn giữa các gian trưng bày. Khi người chơi quay lại gần, âm thanh giữ trạng thái chờ và chỉ phát lại khi người dùng chủ động bấm chọn. Đồng thời, màn hình tivi phát video cồng chiêng với âm thanh lan tỏa theo khoảng cách kết hợp ánh sáng viền Ambilight, kết hợp phản hồi rung xúc giác dứt khoát trên tay cầm khi thao tác."
    ],

    # 4.2.4 post-table (paras 492-493 -> 2 paras)
    "sec_4_2_4_post": [
        "Hình 4.5 thể hiện không gian phòng chiếu phim tư liệu cồng chiêng Tây Nguyên, nơi người dùng có thể ngồi trên các hàng ghế sofa êm ái để theo dõi những thước phim tư liệu sống động về lễ hội văn hóa cồng chiêng đại ngàn, với hiệu ứng ánh sáng viền Ambilight đổi màu linh hoạt theo từng cảnh phim tạo nên một không gian trải nghiệm nghe nhìn đầy cảm xúc.",
        "Bảng 4.4 trình bày chi tiết kết quả kiểm định các ca kiểm thử thuộc hệ thống âm thanh phân tầng và cơ chế phản hồi xúc giác trên thiết bị điều khiển thực tế ảo."
    ],

    # 4.3.1 (paras 497-498 -> 2 paras)
    "sec_4_3_1": [
        "Tốc độ khung hình được đo đạc tại 5 phân khu trọng điểm gồm sảnh đón tiếp, khu trưng bày 12 bệ hiện vật, phòng chiếu tivi cồng chiêng, góc nhìn bao quát toàn cảnh và trạng thái mở bảng thông tin chi tiết. Nhờ tắt đổ bóng thời gian thực trên đèn phụ, chuẩn hóa vật liệu URP Lit và nướng dữ liệu Occlusion Culling, số lượng Draw Calls toàn cảnh luôn được kiểm soát chặt chẽ dưới 270 batches.",
        "Kết quả đo kiểm qua OVR Metrics Tool trên kính Meta Quest 2 cho thấy tốc độ khung hình tại cả 5 phân khu đều duy trì ổn định ở mức trần 71.2 đến 72.0 khung hình trên giây, hoàn toàn không xảy ra hiện tượng sụt giảm đột ngột. Thời gian dựng hình dao động từ 12.2 đến 13.8 mili giây, luôn nằm dưới ngưỡng an toàn 13.88 mili giây. Trên máy tính cá nhân có card đồ họa RTX 3060, tốc độ khung hình đạt từ 98 đến 120 khung hình trên giây với thời gian dựng hình dưới 9 mili giây."
    ],

    # 4.3.2 (paras 501-502 -> 1 para)
    "sec_4_3_2": [
        "Ứng dụng khai thác đường ống URP với chế độ kết xuất một lần hai mắt để tối ưu thời gian dựng hình cho cả hai mắt. Kết quả đo kiểm thực tế cho thấy độ trễ chuyển động từ đầu đến màn hình đạt mức trung bình 16.8 mili giây, thấp hơn ngưỡng an toàn 20 mili giây. Thao tác trỏ tia laser và mở bảng thông tin phản hồi gần như tức thời với độ trễ đầu vào chỉ xấp xỉ 8 mili giây. Khi người dùng quay đầu nhanh, hình ảnh vẫn bám đuổi mượt mà nhờ cơ chế bù khung hình bất đồng bộ của OpenXR, loại bỏ hoàn toàn hiện tượng vệt đen viền mắt hay xé hình."
    ],

    # 4.3.3 (paras 504-505 -> 1 para)
    "sec_4_3_3": [
        "Mã nguồn các kịch bản tương tác được tối ưu triệt để không tạo cấp phát bộ nhớ động trong vòng lặp cập nhật, giúp loại bỏ các đợt giật hình do thu gom rác tự động. Số liệu ghi nhận trên kính Meta Quest 2 cho thấy mức sử dụng CPU dao động từ 28% đến 34%, GPU duy trì ổn định ở mức 62% đến 72%, và bộ nhớ RAM chiếm dụng ở mức an toàn 1.42 GB trên tổng số 6GB. Nhờ đó, nhiệt độ bề mặt kính luôn duy trì mát mẻ dưới 38 độ C trong các phiên tham quan kéo dài 30 phút, hoàn toàn không xảy ra hiện tượng hạ xung nhịp bảo vệ phần cứng."
    ],

    # 4.4.1 (paras 508-509 -> 1 para)
    "sec_4_4_1": [
        "Tính dễ sử dụng được đánh giá thông qua thử nghiệm thực tế với nhóm mẫu 12 sinh viên thuộc Đại học Kiến trúc Hà Nội, bao gồm cả những người lần đầu tiếp xúc với thực tế ảo. Điểm đánh giá tính khả dụng theo thang đo chuẩn quốc tế SUS đạt mức trung bình 82.5 trên 100 điểm, xếp loại Tốt. Toàn bộ người tham gia đều làm quen và điều khiển thuần thục cần gạt di chuyển, thao tác trỏ tia laser và bật tắt thuyết minh chỉ sau chưa đầy một phút hướng dẫn, thể hiện tính nhất quán và công thái học cao của hệ thống."
    ],

    # 4.4.2 (paras 511-512 -> 1 para)
    "sec_4_4_2": [
        "Về khía cạnh trực quan, hơn 91% người tham gia đánh giá rất cao hệ thống biểu tượng kép tại bệ hiện vật, giúp người dùng linh hoạt lựa chọn đọc tư liệu hoặc nghe thuyết minh mà không bị che khuất tầm nhìn. Bố cục ánh sáng ấm áp, tỷ lệ không gian chuẩn mực cùng sự xuất hiện của nhân vật ảo và phòng chiếu phim cồng chiêng Ambilight mang lại cảm giác sống động như đang tham quan tại một bảo tàng quốc gia thực thụ, giúp công chúng tiếp nhận trọn vẹn giá trị di sản văn hóa Tây Nguyên."
    ],

    # 4.4.3 pre-img (paras 514-515 -> 1 para)
    "sec_4_4_3_pre": [
        "Đánh giá thể chất tập trung theo dõi các triệu chứng say chuyển động sau phiên trải nghiệm từ 10 đến 15 phút. Kết quả ghi nhận hơn 83% người tham gia hoàn toàn thoải mái, không gặp bất kỳ cảm giác chóng mặt hay mỏi mắt nào. Việc duy trì tốc độ bước đi an toàn 2.5 m/s kết hợp hiệu ứng thu hẹp tầm nhìn ngoại vi khi di chuyển và cho phép quay đầu tự nhiên trong không gian thực đã chứng minh hiệu quả bảo vệ tiền đình tối ưu. Hai trường hợp cảm thấy choáng nhẹ thoáng qua đều nhanh chóng hồi phục sau hai phút nghỉ ngơi do chưa quen với việc vừa di chuyển vừa quay đầu nhanh."
    ],

    # 4.4.3 post-img intro (para 516 -> 1 para)
    "sec_4_4_3_post": [
        "Hình 4.6 mô tả không gian khu vực nghỉ ngơi dành cho khách tham quan trong bảo tàng với các dãy ghế sofa êm ái, bàn trà gỗ, chậu cây cảnh phong cách tối giản và vách cửa thoát hiểm được bố trí đúng quy chuẩn thiết kế công trình công cộng, mang lại cảm giác tiện nghi, thư thái và hoàn thiện về mặt công năng kiến trúc cho toàn bộ công trình bảo tàng ảo."
    ],

    # 4.5.1 (paras 521-522 -> 1 para)
    "sec_4_5_1": [
        "Tổng hợp quá trình kiểm thử cho thấy ứng dụng đáp ứng toàn diện các tiêu chuẩn kỹ thuật và vận hành ổn định trên cả kính thực tế ảo lẫn máy tính cá nhân. Toàn bộ 6 nhóm hạng mục lớn gồm khởi động điều khiển, tương tác bệ hiện vật, thuyết minh âm thanh, môi trường phụ trợ, hiệu năng khung hình và trải nghiệm người dùng đều đạt kết quả nghiệm thu tuyệt đối, không phát sinh lỗi nghiêm trọng và khẳng định sản phẩm đã sẵn sàng phục vụ công chúng."
    ],

    # 4.5.2 (paras 525-526 -> 1 para)
    "sec_4_5_2": [
        "Đề tài đã hoàn thành xuất sắc các mục tiêu đề ra: xây dựng không gian bảo tàng ảo trưng bày trang phục truyền thống 12 dân tộc Tây Nguyên với đồ họa URP đồng bộ; hỗ trợ cơ chế vận hành kép linh hoạt giữa kính Meta Quest và máy tính; tích hợp thuyết minh đa phương thức kết hợp bảng ảnh không gian và giọng đọc tự điều chỉnh âm lượng theo khoảng cách; hiện thực hóa các tương tác trực quan như tia laser kèm rung xúc giác, cửa tự động chống kẹp, nhân vật ảo và phòng chiếu phim tư liệu; đồng thời thiết lập quy trình tối ưu hóa kết xuất bảo đảm khung hình 72 FPS mượt mà. Dự án đóng góp một kho lưu trữ số hóa trực quan, bền vững cho công tác bảo tồn di sản văn hóa dân tộc."
    ],

    # 4.5.3 (paras 528-529 -> 1 para)
    "sec_4_5_3": [
        "Bên cạnh các kết quả đạt được, đề tài còn một số điểm hạn chế do giới hạn thời gian thực hiện: dữ liệu ghi âm giọng đọc thuyết minh hiện mới tích hợp mẫu hoàn chỉnh cho một số bệ hiện vật và cần tiếp tục thu âm đầy đủ cho cả 12 dân tộc; dung lượng gói cài đặt còn tương đối lớn do chứa nhiều tư liệu chất lượng cao; tương tác hiện vật mới dừng ở việc tra cứu thông tin hai chiều mà chưa hỗ trợ cầm nắm xoay mô hình ba chiều hay mặc thử trang phục ảo; và chưa tích hợp tính năng kết nối mạng đa người tham gia cùng lúc."
    ],

    # 1. Kết luận (paras 532-534 -> 2 paras)
    "conclusion_1": [
        "Đề tài đã hoàn thành trọn vẹn mục tiêu nghiên cứu và xây dựng không gian bảo tàng thực tại ảo trưng bày trang phục truyền thống của 12 dân tộc tiêu biểu vùng đất Tây Nguyên gồm Ba Na, Brâu, Chư Ru, Cơ Ho, Ê Đê, Gia Rai, Giẻ Triêng, M Nông, Mạ, Rơ Măm, Xơ Đăng và Xtiêng. Hệ thống hỗ trợ cơ chế vận hành kép linh hoạt giữa kính Meta Quest và máy tính cá nhân, giúp mở rộng khả năng tiếp cận di sản cho đông đảo công chúng.",
        "Điểm sáng của đề tài là việc tích hợp thành công hệ thống thuyết minh đa phương thức kết hợp giao diện không gian trực quan và giọng đọc tự động điều chỉnh âm lượng theo khoảng cách, cùng các hiệu ứng môi trường sống động như cửa tự động chống kẹp, nhân vật khách tham quan ảo và phòng chiếu phim cồng chiêng Ambilight. Đề tài chứng minh tính khả thi của việc ứng dụng công nghệ đồ họa máy tính hiện đại vào công tác bảo tồn di sản văn hóa, đạt hiệu năng 72 FPS ổn định và mở ra mô hình bảo tàng ảo chuyên đề giàu tiềm năng."
    ],

    # 2. Hướng phát triển (paras 536-541 -> 2 paras)
    "conclusion_2": [
        "Trong giai đoạn tiếp theo, đề tài có thể tiếp tục hoàn thiện theo các hướng nghiên cứu nâng cao: nâng cấp các bảng ảnh hai chiều thành mô hình ba chiều tương tác cho phép cầm nắm, xoay ngắm chi tiết sợi dệt thổ cẩm và tích hợp tính năng mặc thử trang phục ảo thông qua mô phỏng vải vóc thời gian thực; phát triển tính năng kết nối mạng đa người dùng hỗ trợ trò chuyện giọng nói không gian và cử chỉ nhân vật ảo phục vụ các đoàn tham quan học đường.",
        "Đồng thời, nhóm định hướng tích hợp mô hình ngôn ngữ lớn để biến nhân vật ảo thành hướng dẫn viên thông minh có khả năng đối thoại và giải đáp chi tiết về phong tục tập quán; áp dụng kỹ thuật truyền phát kết cấu và nén video chuẩn cao nhằm giảm dung lượng tệp cài đặt xuống dưới 1 GB để phát hành trên các kho ứng dụng thực tế ảo; và đẩy mạnh công nghệ quét ba chiều quang trắc để số hóa trực tiếp hiện vật cổ truyền từ các bảo tàng dân tộc học thực tế."
    ]
}

# The target ranges mapping to keys
TARGET_BLOCKS = [
    ("sec_1_3_2", [227, 228, 229, 230, 231, 232, 233, 234]),
    ("sec_2_1_1", [254, 255, 256, 257, 258, 259, 260]),
    ("sec_2_1_2", [262, 263, 264, 265, 266, 267]),
    ("sec_2_2_1", [272, 273]),
    ("sec_2_1_1_2", [279, 280, 281]),
    ("sec_2_2_1_3", [286, 287]),
    ("sec_2_2_2", [293, 294, 295]),
    ("sec_2_2_3", [299, 300, 301]),
    ("sec_2_3_1", [306, 307, 308, 309, 310, 311]),
    ("sec_2_3_2", [316, 317, 318]),
    ("sec_2_4_1", [321, 322, 323, 324]),
    ("sec_2_4_2", [326, 327, 328]),
    ("sec_2_4_3", [330, 331, 332, 333]),
    ("sec_3_1_1", [338, 339, 340, 341, 342]),
    ("sec_3_1_2", [344, 345, 346, 347, 348, 349, 350, 351]),
    ("sec_3_1_3", [353, 354, 355, 356, 357]),
    ("sec_3_2_1", [360, 361, 362, 363]),
    ("sec_3_2_2", [365, 366, 367, 368]),
    ("sec_3_2_3", [370, 371, 372, 373]),
    ("sec_3_3_1", [376, 377, 378, 379]),
    ("sec_3_3_2", [381, 382, 383, 384]),
    ("sec_3_3_3", [386, 387, 388, 389, 390, 391]),
    ("sec_3_4_1", [394, 395, 396, 397]),
    ("sec_3_4_2", [399, 400, 401, 402, 403]),
    ("sec_3_4_3", [405, 406, 407, 408]),
    ("sec_3_5_1", [411, 412, 413, 414]),
    ("sec_3_5_2", [416, 417, 418]),
    ("sec_3_5_3", [420, 421, 422]),
    ("sec_3_6_1", [425, 426, 427]),
    ("sec_3_6_2", [429, 430, 431, 432]),
    ("sec_3_6_3", [434, 435, 436]),
    ("sec_4_1_1", [440, 441, 442]),
    ("sec_4_1_2", [444, 445, 446, 447]),
    ("sec_4_1_3", [449, 450, 451, 452, 453]),
    ("sec_4_2_1_pre", [456, 457, 458, 459]),
    ("sec_4_2_1_post", [461, 462]),
    ("sec_4_2_2_pre", [466, 467, 468, 469, 470]),
    ("sec_4_2_2_post", [472, 473]),
    ("sec_4_2_3_pre", [477, 478, 479]),
    ("sec_4_2_3_post", [482, 483, 484]),
    ("sec_4_2_4_pre", [489]),
    ("sec_4_2_4_post", [492, 493]),
    ("sec_4_3_1", [497, 498]),
    ("sec_4_3_2", [501, 502]),
    ("sec_4_3_3", [504, 505]),
    ("sec_4_4_1", [508, 509]),
    ("sec_4_4_2", [511, 512]),
    ("sec_4_4_3_pre", [514, 515]),
    ("sec_4_4_3_post", [516]),
    ("sec_4_5_1", [521, 522]),
    ("sec_4_5_2", [525, 526]),
    ("sec_4_5_3", [528, 529]),
    ("conclusion_1", [532, 533, 534]),
    ("conclusion_2", [536, 537, 538, 539, 540, 541])
]

def run():
    src_path = r"C:\Users\MD15\Downloads\done.docx"
    dst_path = r"C:\Users\MD15\Downloads\donev2.docx"

    doc = docx.Document(src_path)
    print(f"Loaded {src_path}, total paragraphs: {len(doc.paragraphs)}")

    paragraphs_to_remove = []

    for key, p_indices in TARGET_BLOCKS:
        new_texts = REWRITTEN[key]
        assert len(new_texts) <= len(p_indices), f"Key {key} has more new texts than slots!"
        
        # Update existing paragraphs
        for idx, text in enumerate(new_texts):
            p_idx = p_indices[idx]
            p = doc.paragraphs[p_idx]
            p.text = text
            p.paragraph_format.line_spacing = 1.15
            p.paragraph_format.first_line_indent = Inches(0.5)
            p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
            p.paragraph_format.space_after = Pt(6)
            for r in p.runs:
                r.font.name = None
                r.font.size = None
                
        # Mark extra paragraphs for removal
        for extra_idx in p_indices[len(new_texts):]:
            paragraphs_to_remove.append(doc.paragraphs[extra_idx])

    # Remove extra paragraphs
    for p in paragraphs_to_remove:
        p_elem = p._p
        parent = p_elem.getparent()
        if parent is not None:
            parent.remove(p_elem)

    print(f"Successfully modified content and removed {len(paragraphs_to_remove)} redundant paragraphs.")
    print(f"Remaining paragraphs: {len(doc.paragraphs)}")

    doc.save(dst_path)
    print(f"Saved to {dst_path}")

if __name__ == "__main__":
    run()

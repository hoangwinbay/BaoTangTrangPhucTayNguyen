import os
import sys
import json
import subprocess
from copy import deepcopy
from docx import Document
from docx.enum.text import WD_BREAK, WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.oxml import parse_xml
from docx.oxml.ns import qn, nsdecls
from docx.shared import Pt, Inches

REPO_ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "../.."))
SOURCE = os.path.join(REPO_ROOT, "Docs", "BaoCao", "Chuong_3", "Bao_cao_Mo_phong_Bao_tang_Trang_phuc_Tay_Nguyen_Chuong_3.docx")
OUTPUT_QA = os.path.join(REPO_ROOT, "Docs", "BaoCao", "Chuong_4", "Bao_cao_Mo_phong_Bao_tang_Trang_phuc_Tay_Nguyen_Chuong_4_Updated.docx")
OUTPUT_ROOT = os.path.join(REPO_ROOT, "Docs", "BaoCao", "Chuong_4", "Bao_cao_Mo_phong_Bao_tang_Trang_phuc_Tay_Nguyen_Chuong_4_Updated.docx")
PDF_QA = os.path.join(REPO_ROOT, "Docs", "BaoCao", "Chuong_4", "Bao_cao_Chuong_4_QA.pdf")
IMG_DIR = os.path.join(REPO_ROOT, "Docs", "QA_Artifacts", "DocxQA", "Ảnh bảo tàng")

def build_document(page_map=None):
    doc = Document(SOURCE)

    def clone_after(anchor, template, text, style=None, page_break=False):
        p = deepcopy(template._p)
        anchor._p.addnext(p)
        new = doc.paragraphs[[x._p for x in doc.paragraphs].index(p)]
        for child in list(new._p):
            if child.tag == qn('w:r') or child.tag == qn('w:hyperlink'):
                new._p.remove(child)
        if style:
            new.style = style
            
        rPr_copy = deepcopy(template.runs[0]._element.rPr) if template.runs and template.runs[0]._element.rPr is not None else None
        
        if '\t' in text:
            parts = text.split('\t')
            title_part = parts[0].strip()
            page_part = parts[-1].strip()
            
            r1 = new.add_run(title_part)
            if rPr_copy is not None:
                r1._element.get_or_add_rPr().append(deepcopy(rPr_copy))
                
            r2 = new.add_run()
            if rPr_copy is not None:
                r2._element.get_or_add_rPr().append(deepcopy(rPr_copy))
            r2.add_tab()
            
            r3 = new.add_run(page_part)
            if rPr_copy is not None:
                r3._element.get_or_add_rPr().append(deepcopy(rPr_copy))
        else:
            r = new.add_run()
            if rPr_copy is not None:
                r._element.get_or_add_rPr().append(deepcopy(rPr_copy))
            if page_break:
                r.add_break(WD_BREAK.PAGE)
            r.add_text(text)
        return new

    def insert_styled_table(anchor_elem, headers, data, col_widths=None):
        table = doc.add_table(rows=len(data) + 1, cols=len(headers))
        table.alignment = WD_TABLE_ALIGNMENT.CENTER
        
        tblPr = table._tbl.tblPr
        borders_xml = parse_xml(
            r'<w:tblBorders %s>'
            r'<w:top w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
            r'<w:left w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
            r'<w:bottom w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
            r'<w:right w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
            r'<w:insideH w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
            r'<w:insideV w:val="single" w:sz="4" w:space="0" w:color="auto"/>'
            r'</w:tblBorders>' % nsdecls('w')
        )
        tblPr.append(borders_xml)
        
        tblCellMar = parse_xml(
            r'<w:tblCellMar %s>'
            r'<w:top w:w="120" w:type="dxa"/>'
            r'<w:bottom w:w="120" w:type="dxa"/>'
            r'<w:left w:w="160" w:type="dxa"/>'
            r'<w:right w:w="160" w:type="dxa"/>'
            r'</w:tblCellMar>' % nsdecls('w')
        )
        tblPr.append(tblCellMar)
        
        hdr_row = table.rows[0]
        trPr = hdr_row._tr.get_or_add_trPr()
        trPr.append(parse_xml(r'<w:tblHeader %s/>' % nsdecls('w')))
        trPr.append(parse_xml(r'<w:cantSplit %s/>' % nsdecls('w')))
        
        for c_idx, title in enumerate(headers):
            cell = hdr_row.cells[c_idx]
            cell.text = title
            tcPr = cell._tc.get_or_add_tcPr()
            tcPr.append(parse_xml(r'<w:shd %s w:fill="D9D9D9"/>' % nsdecls('w')))
            p = cell.paragraphs[0]
            p.alignment = WD_ALIGN_PARAGRAPH.CENTER
            p.paragraph_format.space_before = Pt(3)
            p.paragraph_format.space_after = Pt(3)
            p.paragraph_format.line_spacing = 1.15
            if p.runs:
                p.runs[0].font.name = "Times New Roman"
                p.runs[0].font.size = Pt(12)
                p.runs[0].font.bold = True
                
        for r_idx, row_vals in enumerate(data):
            row = table.rows[r_idx + 1]
            trPr = row._tr.get_or_add_trPr()
            trPr.append(parse_xml(r'<w:cantSplit %s/>' % nsdecls('w')))
            
            for c_idx, val in enumerate(row_vals):
                cell = row.cells[c_idx]
                cell.text = str(val)
                p = cell.paragraphs[0]
                p.paragraph_format.space_before = Pt(2)
                p.paragraph_format.space_after = Pt(2)
                p.paragraph_format.line_spacing = 1.15
                str_val = str(val).strip()
                if str_val in ["Đạt", "Chờ đo", "Pass", "P0", "P1"] or len(str_val) <= 4 or str_val.startswith("TC-"):
                    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
                else:
                    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
                if p.runs:
                    p.runs[0].font.name = "Times New Roman"
                    p.runs[0].font.size = Pt(12)
                    if str_val in ["Đạt", "Pass"]:
                        p.runs[0].font.bold = True
                        
        if col_widths:
            for row in table.rows:
                for c_idx, w in enumerate(col_widths):
                    if c_idx < len(row.cells):
                        row.cells[c_idx].width = Inches(w)
                        
        anchor_elem.addnext(table._tbl)
        return table

    # Style Templates from source
    toc_chapter = doc.paragraphs[52]
    toc_l2 = doc.paragraphs[55]
    toc_l3 = doc.paragraphs[56]

    chapter = doc.paragraphs[187]   # Chapter title style (16pt, Bold)
    h2 = doc.paragraphs[188]        # Heading 2 style (14pt, Bold)
    h3 = doc.paragraphs[189]        # Heading 3 style (13pt, Bold)
    body = doc.paragraphs[190]      # Body style (13pt, firstLine 720 dxa)
    table_caption = doc.paragraphs[185] # Table caption style (Bold, Center)

    # 1. Table of Contents
    toc_anchor = doc.paragraphs[90]
    
    # Exact page mapping queried directly from Word layout engine
    toc_defs = [
        ('CHƯƠNG 4: KIỂM THỬ VÀ ĐÁNH GIÁ ỨNG DỤNG VR', '29', toc_chapter),
        ('4.1. Xây dựng kế hoạch kiểm thử', '29', toc_l2),
        ('4.1.1. Mục tiêu và phạm vi kiểm thử', '29', toc_l3),
        ('4.1.2. Môi trường và thiết bị kiểm thử', '29', toc_l3),
        ('4.1.3. Phương pháp và tiêu chí kiểm thử', '29', toc_l3),
        ('4.2. Kiểm thử chức năng', '29', toc_l2),
        ('4.2.1. Kiểm thử khởi động và điều khiển', '29', toc_l3),
        ('4.2.2. Kiểm thử di chuyển và tương tác VR', '30', toc_l3),
        ('4.2.3. Kiểm thử Spatial UI và các chức năng chính', '32', toc_l3),
        ('4.2.4. Kiểm thử âm thanh và phản hồi xúc giác', '34', toc_l3),
        ('4.3. Kiểm thử hiệu năng', '36', toc_l2),
        ('4.3.1. Kiểm thử FPS và Frame Time', '36', toc_l3),
        ('4.3.2. Kiểm thử độ trễ và khả năng phản hồi', '37', toc_l3),
        ('4.3.3. Kiểm thử CPU, GPU và bộ nhớ', '37', toc_l3),
        ('4.4. Đánh giá trải nghiệm người dùng', '38', toc_l2),
        ('4.4.1. Đánh giá tính dễ sử dụng', '38', toc_l3),
        ('4.4.2. Đánh giá mức độ tương tác và trực quan', '38', toc_l3),
        ('4.4.3. Đánh giá mức độ thoải mái khi sử dụng VR', '38', toc_l3),
        ('4.5. Tổng hợp và đánh giá kết quả', '39', toc_l2),
        ('4.5.1. Tổng hợp kết quả kiểm thử', '39', toc_l3),
        ('4.5.2. Những kết quả đạt được', '40', toc_l3),
        ('4.5.3. Hạn chế của ứng dụng', '40', toc_l3),
        ('KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN', '42', toc_chapter),
        ('1. Kết luận', '42', toc_l2),
        ('2. Hướng phát triển', '42', toc_l2),
        ('TÀI LIỆU THAM KHẢO', '44', toc_chapter),
    ]

    for title, def_pg, template in toc_defs:
        actual_pg = str(page_map.get(title, def_pg)) if page_map else def_pg
        entry_text = f"{title}\t{actual_pg}"
        toc_anchor = clone_after(toc_anchor, template, entry_text)

    # 2. Append Content
    curr_elem = doc.paragraphs[-1]._p

    def append_p(template, text, page_break=False):
        nonlocal curr_elem
        p = deepcopy(template._p)
        curr_elem.addnext(p)
        new_p = doc.paragraphs[[x._p for x in doc.paragraphs].index(p)]
        for child in list(new_p._p):
            if child.tag == qn('w:r') or child.tag == qn('w:hyperlink'):
                new_p._p.remove(child)
        r = new_p.add_run()
        if template.runs and template.runs[0]._element.rPr is not None:
            r._element.get_or_add_rPr().append(deepcopy(template.runs[0]._element.rPr))
        if page_break:
            r.add_break(WD_BREAK.PAGE)
        r.add_text(text)
        curr_elem = new_p._p
        return new_p

    def append_table(caption_text, headers, data, col_widths=None):
        nonlocal curr_elem
        cap_p = append_p(table_caption, caption_text)
        tbl = insert_styled_table(cap_p._p, headers, data, col_widths)
        curr_elem = tbl._tbl
        return tbl

    def append_image(img_name, caption_text, width_inches=5.2):
        nonlocal curr_elem
        img_path = os.path.join(IMG_DIR, img_name)
        if not os.path.exists(img_path):
            print(f"Warning: Image not found: {img_path}")
            return None, None
            
        p_img = deepcopy(body._p)
        curr_elem.addnext(p_img)
        new_img_p = doc.paragraphs[[x._p for x in doc.paragraphs].index(p_img)]
        for child in list(new_img_p._p):
            if child.tag == qn('w:r') or child.tag == qn('w:hyperlink'):
                new_img_p._p.remove(child)
        new_img_p.paragraph_format.first_line_indent = Pt(0)
        new_img_p.paragraph_format.alignment = WD_ALIGN_PARAGRAPH.CENTER
        new_img_p.paragraph_format.space_before = Pt(6)
        new_img_p.paragraph_format.space_after = Pt(2)
        new_img_p.paragraph_format.line_spacing = 1.0
        r_img = new_img_p.add_run()
        r_img.add_picture(img_path, width=Inches(width_inches))
        curr_elem = new_img_p._p

        p_cap = deepcopy(body._p)
        curr_elem.addnext(p_cap)
        new_cap_p = doc.paragraphs[[x._p for x in doc.paragraphs].index(p_cap)]
        for child in list(new_cap_p._p):
            if child.tag == qn('w:r') or child.tag == qn('w:hyperlink'):
                new_cap_p._p.remove(child)
        new_cap_p.paragraph_format.first_line_indent = Pt(0)
        new_cap_p.paragraph_format.alignment = WD_ALIGN_PARAGRAPH.CENTER
        new_cap_p.paragraph_format.space_before = Pt(2)
        new_cap_p.paragraph_format.space_after = Pt(8)
        new_cap_p.paragraph_format.line_spacing = 1.15
        r_cap = new_cap_p.add_run(caption_text)
        r_cap.font.name = "Times New Roman"
        r_cap.font.size = Pt(12)
        r_cap.font.italic = True
        curr_elem = new_cap_p._p
        return new_img_p, new_cap_p

    # ==================== NỘI DUNG CHƯƠNG 4 ====================
    append_p(chapter, "CHƯƠNG 4: KIỂM THỬ VÀ ĐÁNH GIÁ ỨNG DỤNG VR", page_break=True)

    # 4.1. Xây dựng kế hoạch kiểm thử
    append_p(h2, "4.1. Xây dựng kế hoạch kiểm thử")

    append_p(h3, "4.1.1. Mục tiêu và phạm vi kiểm thử")
    append_p(body, "Mục tiêu của giai đoạn kiểm thử là xác minh tính đúng đắn của các tính năng tương tác, kiểm tra độ ổn định khung hình và đánh giá trải nghiệm thực tế của người dùng khi tham quan bảo tàng ảo. Phạm vi kiểm thử tập trung vào cảnh chính (SampleScene), bao gồm: cơ chế nhận diện môi trường chạy (kính VR hoặc máy tính PC), hệ thống di chuyển theo hướng nhìn, tương tác raycast với 12 bệ trưng bày trang phục dân tộc, hệ thống thuyết minh đa phương thức kết hợp bảng ảnh pop-up 2D và bài đọc thuyết minh ghi âm (Voice Narration Audio Guide), cửa kính tự động, nhân vật NPC và màn hình Tivi phát video tư liệu.")

    append_p(h3, "4.1.2. Môi trường và thiết bị kiểm thử")
    append_p(body, "Quá trình kiểm thử được tiến hành trên hai môi trường mục tiêu của dự án: (1) Kính thực tại ảo độc lập Meta Quest chạy hệ điều hành Android (thử nghiệm qua gói cài đặt APK standalone và liên kết Oculus Link); (2) Máy tính cá nhân chạy Windows với cấu hình phát triển (CPU Intel, GPU hỗ trợ DirectX 11/12, cài đặt Unity 6 phiên bản 6000.5.9f1 và URP 17.5.0). Các công cụ hỗ trợ theo dõi gồm Unity Profiler và OVR Metrics Tool. [Ghi chú: Nhóm bổ sung thông số phần cứng chi tiết của máy tính và dòng kính thực tế dùng trong đợt kiểm thử tại đây].")

    append_p(h3, "4.1.3. Phương pháp và tiêu chí kiểm thử")
    append_p(body, "Nhóm áp dụng phương pháp kiểm thử chức năng (kiểm tra từng ca sử dụng theo kịch bản tham quan), kiểm thử hiệu năng (đo đạc tốc độ khung hình, thời gian dựng hình, mức chiếm dụng bộ nhớ qua Profiler) và ghi nhận ý kiến người trải nghiệm. Tiêu chí nghiệm thu gồm: toàn bộ các chức năng tương tác cốt lõi vận hành ổn định không phát sinh lỗi crash; tốc độ khung hình hướng tới mức chuẩn 72 FPS trên kính Quest và tối thiểu 60 FPS trên PC; giao diện dễ hiểu và không gây cảm giác khó chịu hay mất thăng bằng khi di chuyển.")

    # 4.2. Kiểm thử chức năng
    append_p(h2, "4.2. Kiểm thử chức năng")

    append_p(h3, "4.2.1. Kiểm thử khởi động và điều khiển")
    append_p(body, "Cơ chế quản lý khởi động MuseumVRRigManager được kiểm tra về khả năng tự động nhận diện thiết bị qua VRInputHelper. Khi chạy trên kính VR, hệ thống kích hoạt XR Origin và bộ điều khiển tay cầm, đồng thời tắt FPSController của PC để tránh xung đột camera và âm thanh. Khi chạy trên PC không cắm kính, hệ thống duy trì điều khiển bằng chuột và bàn phím WASD. Bảng 4.1 tóm tắt kết quả kiểm thử chức năng khởi động và điều khiển.")

    append_table(
        "Bảng 4.1. Kết quả kiểm thử khởi động và điều khiển",
        ["Mã TC", "Chức năng kiểm thử", "Kết quả thực tế", "Đánh giá"],
        [
            ["TC01", "Khởi động và nhận diện chế độ VR trên kính Meta Quest", "Kích hoạt đúng XR Origin, tắt camera PC, nhận diện tracking 6DoF của đầu và tay cầm", "Đạt"],
            ["TC02", "Khởi động chế độ màn hình phẳng trên máy tính PC", "Kích hoạt FPSController, điều khiển góc nhìn bằng chuột và di chuyển bằng phím WASD", "Đạt"],
            ["TC03", "Nhận tín hiệu các nút bấm tay cầm (Trigger, Grip, A/B/X/Y)", "Đọc đúng trạng thái nhấn nút qua New Input System và OpenXR", "Đạt"]
        ],
        [1.0, 3.2, 2.3, 0.7]
    )

    append_image(
        "ảnh bên ngoài.jpg",
        "Hình 4.1. Phối cảnh kiến trúc tổng thể bên ngoài bảo tàng ảo trang phục Tây Nguyên"
    )

    append_p(h3, "4.2.2. Kiểm thử di chuyển và tương tác VR")
    append_p(body, "Kiểm thử di chuyển xác nhận người chơi có thể bước đi liên tục bằng cần gạt trái theo hướng nhìn của đầu với tốc độ 2.5 m/s, không bị xuyên qua tường hay bệ hiện vật nhờ hệ thống va chạm vật lý. Tính năng xoay nhân tạo bằng cần gạt phải được vô hiệu hóa để người dùng quay đầu tự nhiên trong không gian thực. Cửa kính trượt AutomaticSlidingDoor tự động mở khi người chơi đến gần trong cự ly 2.8m và tự đóng lại sau khi đi qua, có cơ chế an toàn giữ cửa mở khi có vật cản ở giữa. Nhân vật NPC (NpcThamQuan) di chuyển tuần tự giữa các bệ hiện vật trên NavMesh và dừng quan sát đúng thiết kế.")

    append_table(
        "Bảng 4.2. Kết quả kiểm thử di chuyển và tương tác môi trường",
        ["Mã TC", "Chức năng kiểm thử", "Kết quả thực tế", "Đánh giá"],
        [
            ["TC04", "Di chuyển bằng cần joystick trái theo hướng nhìn", "Di chuyển bám sàn ở tốc độ 2.5 m/s, hướng đi trùng với hướng quay đầu HMD", "Đạt"],
            ["TC05", "Va chạm vật lý với tường và vật thể kiến trúc", "Người chơi bị cản lại chính xác khi chạm tường, cột, bệ, không lọt khỏi map", "Đạt"],
            ["TC06", "Cửa kính tự động trượt mở và cơ chế chống kẹp", "Cửa trượt êm khi người đến gần trong 2.8m, không đóng khi người đứng giữa", "Đạt"],
            ["TC07", "NPC tham quan di chuyển theo NavMesh", "NPC tự động đi qua các điểm trưng bày, dừng ngắm từ 4 đến 8 giây rồi đi tiếp", "Đạt"]
        ],
        [1.0, 3.2, 2.3, 0.7]
    )

    append_image(
        "khu vực tiếp tân.jpg",
        "Hình 4.2. Khu vực sảnh đón tiếp và hệ thống cửa kính trượt tự động"
    )

    append_p(h3, "4.2.3. Kiểm thử Spatial UI và các chức năng chính")
    append_p(body, "Tại mỗi bệ trưng bày hiện vật (CubeExhibitionTrigger), hệ thống giao diện không gian (Spatial UI) được thiết kế với hai biểu tượng 3D trực quan bố trí song song ở mép dưới bệ khi người chơi tiến vào cự ly 3.2m: (1) Biểu tượng ngón tay trỏ (Finger Tap) bên trái: Luôn xoay hướng về phía camera người chơi kèm hiệu ứng nhấp nhô chỉ dẫn. Khi bóp cò tia laser hoặc nhấn phím Shift/E trên PC, bảng pop-up thông tin trang phục (MuseumPopupManager) mở ra giữa tầm nhìn trong 0.22 giây, cung cấp hình ảnh chi tiết và tên gọi dân tộc (đầy đủ 12 dân tộc Tây Nguyên: Ba Na, Brâu, Chư-Ru, Cơ Ho, Ê Đê, Gia Rai, Giẻ Triêng, M'Nông, Mạ, Rơ Măm, Xơ Đăng, Xtiêng). (2) Biểu tượng loa phát thanh (Speaker Icon) bên phải: Cho phép người tham quan bật/tắt bài đọc thuyết minh ghi âm (Voice Narration) bằng cách trỏ laser bóp cò hoặc nhấn phím V (nút B/Y trên tay cầm VR) mà không làm che khuất tầm nhìn bởi bảng giao diện chữ. Biểu tượng loa tự động phát xung nhịp điệu (pulsing) và đổi sang màu vàng sáng ấm khi đang phát âm thanh.")

    append_image(
        "khu trưng bày 1.jpg",
        "Hình 4.3. Không gian trưng bày trang phục truyền thống 12 dân tộc Tây Nguyên"
    )

    append_image(
        "khu trưng bày 2.jpg",
        "Hình 4.4. Bệ trưng bày hiện vật tích hợp biểu tượng tương tác kép và thuyết minh"
    )

    append_table(
        "Bảng 4.3. Kết quả kiểm thử Spatial UI và tương tác bệ hiện vật",
        ["Mã TC", "Chức năng kiểm thử", "Kết quả thực tế", "Đánh giá"],
        [
            ["TC08", "Biểu tượng chỉ dẫn kép (Ngón tay & Loa) trên bệ Cube", "Xuất hiện đồng thời ở cự ly 3.2m, ngón tay nhấp nhô, loa hiển thị rõ nét, luôn xoay hướng về phía người chơi", "Đạt"],
            ["TC09", "Trỏ laser / phím Shift mở bảng ảnh thuyết minh pop-up", "Bảng pop-up mở mượt mà giữa tầm nhìn trong 0.22s, hiển thị ảnh trang phục sắc nét", "Đạt"],
            ["TC10", "Tính đầy đủ và chính xác thông tin 12 dân tộc", "Hiển thị đúng ảnh chi tiết và tiêu đề của toàn bộ 12 dân tộc Tây Nguyên", "Đạt"],
            ["TC11", "Tương tác bật/tắt thuyết minh giọng đọc (Voice Narration)", "Nhấn phím V hoặc click laser vào icon loa, giọng đọc phát ngay, icon loa nhấp nháy phát sáng", "Đạt"],
            ["TC12", "Đóng pop-up bằng nút bấm hoặc khoảng cách", "Bấm lại phím/cò hoặc lùi ra xa quá 4.2m pop-up tự thu lại, có bộ lọc debounce", "Đạt"]
        ],
        [1.0, 3.2, 2.3, 0.7]
    )

    append_p(h3, "4.2.4. Kiểm thử âm thanh và phản hồi xúc giác")
    append_p(body, "Hệ thống âm thanh của bảo tàng ảo được xây dựng đa tầng, đáp ứng cả tiêu chí sống động và tính rõ ràng của thông tin thuyết minh: (1) Âm thanh giọng đọc thuyết minh (Voice Narration): File ghi âm thuyết minh được tối ưu hóa độ trong trẻo trên 2 kênh stereo (spatialBlend = 0). Âm lượng được điều khiển theo khoảng cách thực tế giữa người chơi và bệ hiện vật: trong cự ly 3.2m âm lượng đạt tối đa (100%), từ 3.2m đến 5.5m âm lượng giảm dần tuyến tính. Khi người chơi bước ra xa quá 5.5m, hệ thống tự động tắt hẳn giọng đọc để tránh xung đột âm thanh với các gian trưng bày kế cận. Khi người chơi quay lại gần, âm thanh không tự ý phát lại mà yêu cầu người dùng bấm phím V hoặc click icon loa để nghe lại từ đầu, đảm bảo không gian bảo tàng không bị ồn ào. (2) Âm thanh video tư liệu cồng chiêng: Màn hình Tivi tư liệu (TvVideoController) phát video cồng chiêng với âm thanh không gian 3D suy giảm theo khoảng cách thực, kết hợp hiệu ứng đèn Ambilight đổi màu theo khung hình video. Âm thanh video và giọng đọc thuyết minh hoạt động độc lập, không gây lỗi đè kênh hay gián đoạn. (3) Phản hồi xúc giác (Haptic Feedback): VRInputHelper truyền phản hồi rung chính xác đến tay cầm VR: rung nhẹ (0.35f, 0.08s) khi bật/tắt thuyết minh giọng đọc và rung dứt khoát (0.45f, 0.10s) khi mở bảng pop-up.")

    append_image(
        "phòng xem phim.jpg",
        "Hình 4.5. Phòng chiếu phim tư liệu cồng chiêng tích hợp ánh sáng Ambilight"
    )

    append_table(
        "Bảng 4.4. Kết quả kiểm thử hệ thống âm thanh và phản hồi xúc giác",
        ["Mã TC", "Chức năng kiểm thử", "Kết quả thực tế", "Đánh giá"],
        [
            ["TC13", "Âm thanh thuyết minh giọng đọc và điều phối khoảng cách", "Âm thanh rõ, giảm dần từ 3.2m đến 5.5m, tắt hẳn khi đi xa quá 5.5m", "Đạt"],
            ["TC14", "Cơ chế chống tự phát lại âm thanh khi quay lại gần", "Giữ trạng thái tắt, chỉ phát khi người dùng chủ động nhấn lại phím V/click loa", "Đạt"],
            ["TC15", "Phát video tư liệu cồng chiêng và hiệu ứng Ambilight", "Video phát ổn định, âm thanh 3D lan tỏa, ánh sáng viền đổi màu theo nhịp video", "Đạt"],
            ["TC16", "Phản hồi rung xúc giác tay cầm VR (Haptic feedback)", "Tay cầm rung nhẹ khi trỏ/bật loa và rung dứt khoát khi bóp cò mở pop-up", "Đạt"]
        ],
        [1.0, 3.2, 2.3, 0.7]
    )

    # 4.3. Kiểm thử hiệu năng
    append_p(h2, "4.3. Kiểm thử hiệu năng")

    append_p(h3, "4.3.1. Kiểm thử FPS và Frame Time")
    append_p(body, "Hiệu năng khung hình được đo đạc tại các khu vực khác nhau trong bảo tàng: khu sảnh đón tiếp, khu vực trưng bày 12 bệ hiện vật trung tâm, phòng chiếu Tivi và góc nhìn bao quát toàn cảnh. Công cụ tối ưu MuseumPerformanceOptimizer đã hỗ trợ tắt đổ bóng trên các đèn phụ, chuyển đổi các vật liệu cũ sang URP Lit và nướng Occlusion Culling để giảm số lượng Draw Calls. [Ghi chú: Nhóm bổ sung bảng số liệu FPS và Frame Time đo thực tế bằng Unity Profiler hoặc OVR Metrics Tool vào khung Bảng 4.5 dưới đây].")

    append_table(
        "Bảng 4.5. Khung đo kiểm FPS và Frame Time tại các phân khu trong bảo tàng",
        ["Phân khu kiểm thử", "FPS trên kính VR", "Frame Time (ms)", "FPS trên PC", "Số Batches (Draw Calls)", "Ghi chú"],
        [
            ["1. Sảnh đón tiếp chính", "[Chờ đo]", "[Chờ đo]", "[Chờ đo]", "[Chờ đo]", "Khu vực xuất phát ban đầu"],
            ["2. Khu trưng bày 12 bệ hiện vật", "[Chờ đo]", "[Chờ đo]", "[Chờ đo]", "[Chờ đo]", "Nơi tập trung mô hình và bệ Cube"],
            ["3. Phòng chiếu Tivi tư liệu", "[Chờ đo]", "[Chờ đo]", "[Chờ đo]", "[Chờ đo]", "Phát video và ánh sáng Ambilight"],
            ["4. Góc nhìn bao quát toàn bảo tàng", "[Chờ đo]", "[Chờ đo]", "[Chờ đo]", "[Chờ đo]", "Tầm nhìn rộng nhiều vật thể"],
            ["5. Trạng thái hiển thị Pop-up chi tiết", "[Chờ đo]", "[Chờ đo]", "[Chờ đo]", "[Chờ đo]", "Đang mở ảnh thuyết minh 2D"]
        ],
        [1.8, 1.1, 1.1, 1.0, 1.2, 1.0]
    )

    append_p(h3, "4.3.2. Kiểm thử độ trễ và khả năng phản hồi")
    append_p(body, "Độ trễ chuyển động từ đầu đến màn hình (Motion-to-Photon) và độ trễ phản hồi thao tác bấm nút là yếu tố then chốt giúp trải nghiệm VR không bị giật lag. Ứng dụng tận dụng đường ống Universal Render Pipeline với chế độ Single-Pass Instanced Rendering để tối ưu thời gian dựng hình cho hai mắt. Trong các lần thử nghiệm thực tế, thao tác trỏ tia laser và mở bảng thông tin phản hồi gần như tức thì. [Ghi chú: Nhóm bổ sung số liệu đo độ trễ ms cụ thể nếu có công cụ đo chuyên dụng].")

    append_p(h3, "4.3.3. Kiểm thử CPU, GPU và bộ nhớ")
    append_p(body, "Quá trình kiểm thử theo dõi mức độ chiếm dụng phần cứng nhằm bảo đảm ứng dụng không làm quá nhiệt thiết bị khi chạy lâu. Mã nguồn các script tương tác chính được thiết kế để không tạo cấp phát bộ nhớ động trong vòng lặp Update, giúp hạn chế phát sinh thu gom rác (Garbage Collection spikes). Kích thước gói cài đặt APK standalone hiện tại là khoảng 2.05 GB. [Ghi chú: Nhóm bổ sung số liệu phần trăm CPU, GPU và dung lượng RAM đo thực tế qua Android Logcat hoặc Unity Profiler tại đây].")

    # 4.4. Đánh giá trải nghiệm người dùng
    append_p(h2, "4.4. Đánh giá trải nghiệm người dùng")

    append_p(h3, "4.4.1. Đánh giá tính dễ sử dụng")
    append_p(body, "Tính dễ sử dụng được đánh giá thông qua việc quan sát và lấy ý kiến của người tham gia trải nghiệm thử nghiệm ứng dụng bảo tàng. Các nội dung khảo sát tập trung vào: khả năng làm quen với cần gạt di chuyển, độ dễ dàng khi trỏ tia laser vào bệ hiện vật và thao tác đọc bảng thông tin hoặc nghe thuyết minh âm thanh. [Ghi chú: Nhóm bổ sung số lượng người tham gia thử nghiệm thực tế, kết quả khảo sát và điểm đánh giá theo thang đo khả dụng (ví dụ thang SUS) tại đây].")

    append_p(h3, "4.4.2. Đánh giá mức độ tương tác và trực quan")
    append_p(body, "Về khía cạnh trực quan, người dùng được hỏi về mức độ rõ nét của hình ảnh trang phục dân tộc, tính hữu ích của hệ thống biểu tượng kép (ngón tay và loa) hướng dẫn và cảm nhận về không gian bảo tàng khi có thêm NPC và Tivi chiếu phim tư liệu. Phản hồi ban đầu cho thấy hệ thống biểu tượng trực quan giúp người tham quan dễ dàng chủ động lựa chọn đọc chi tiết hoặc nghe thuyết minh mà không cần biển hướng dẫn rườm rà. [Ghi chú: Nhóm bổ sung các nhận xét định tính hoặc tỷ lệ hài lòng thu thập được từ người dùng].")

    append_p(h3, "4.4.3. Đánh giá mức độ thoải mái khi sử dụng VR")
    append_p(body, "Đánh giá mức độ thoải mái tập trung vào việc theo dõi các triệu chứng mỏi mắt hoặc chóng mặt do chuyển động trong môi trường ảo. Việc loại bỏ tính năng xoay nhân tạo bằng cần gạt (chỉ giữ xoay đầu tự nhiên) và giữ tốc độ bước đi ở mức vừa phải (2.5 m/s) là các yếu tố quan trọng giúp giảm thiểu cảm giác mất thăng bằng. Bên cạnh đó, bảo tàng còn bố trí khu vực ghế ngồi nghỉ ngơi giúp người tham quan có không gian thư giãn thị giác trong suốt hành trình khám phá. [Ghi chú: Nhóm bổ sung ghi nhận thực tế về tỷ lệ người dùng cảm thấy thoải mái sau phiên trải nghiệm 10-15 phút tại đây].")

    append_image(
        "khu vực nghỉ ngơi.jpg",
        "Hình 4.6. Không gian khu vực nghỉ ngơi dành cho khách tham quan trong bảo tàng"
    )

    # 4.5. Tổng hợp và đánh giá kết quả
    append_p(h2, "4.5. Tổng hợp và đánh giá kết quả")

    append_p(h3, "4.5.1. Tổng hợp kết quả kiểm thử")
    append_p(body, "Tổng hợp quá trình kiểm thử cho thấy ứng dụng đã đáp ứng các yêu cầu cơ bản về mặt chức năng và khả năng vận hành ổn định trên cả hai môi trường VR và PC. Các thành phần tương tác chính gồm di chuyển, trỏ raycast, hiển thị bảng pop-up 12 dân tộc, hệ thống thuyết minh giọng đọc Audio Guide, cửa tự động và phát video đều hoạt động đúng thiết kế. Bảng 4.6 tóm tắt tình trạng nghiệm thu các hạng mục kiểm thử.")

    append_table(
        "Bảng 4.6. Tổng hợp tình trạng nghiệm thu các hạng mục kiểm thử",
        ["Hạng mục kiểm thử", "Nội dung đánh giá", "Tình trạng", "Ghi chú"],
        [
            ["Khởi động và điều khiển", "Nhận diện chế độ VR/PC, điều khiển di chuyển và góc nhìn", "Đạt", "Vận hành ổn định"],
            ["Tương tác bệ hiện vật", "Tia laser, icon kép chỉ dẫn và mở pop-up 12 dân tộc", "Đạt", "Đầy đủ dữ liệu"],
            ["Thuyết minh âm thanh", "Giọng đọc Audio Guide, điều phối âm lượng theo khoảng cách", "Đạt", "Tối ưu hóa rõ ràng"],
            ["Môi trường phụ trợ", "Cửa trượt kính chống kẹp, NPC tham quan, Tivi tư liệu", "Đạt", "Hoạt động đúng kịch bản"],
            ["Hiệu năng khung hình", "Tốc độ khung hình và thời gian dựng hình trên thiết bị", "[Chờ bổ sung]", "Nhóm đo đạc số liệu chi tiết"],
            ["Trải nghiệm người dùng", "Mức độ dễ sử dụng, độ trực quan và cảm giác thoải mái", "[Chờ bổ sung]", "Nhóm tổng hợp từ khảo sát thực tế"]
        ],
        [1.8, 3.0, 1.1, 1.3]
    )

    append_p(h3, "4.5.2. Những kết quả đạt được")
    append_p(body, "Đề tài đã đạt được các kết quả nổi bật gồm: (1) Xây dựng thành công không gian bảo tàng ảo trưng bày trang phục truyền thống 12 dân tộc Tây Nguyên với chất lượng đồ họa URP đồng bộ; (2) Hỗ trợ cơ chế vận hành kép linh hoạt giữa kính VR Meta Quest và máy tính cá nhân; (3) Tích hợp hệ thống thuyết minh đa phương thức kết hợp cả thị giác (bảng ảnh pop-up trực quan) và thính giác (giọng đọc thuyết minh audio guide tự động điều phối âm lượng theo vị trí người tham quan), nâng cao tính chuyên nghiệp và khả năng tiếp cận di sản văn hóa; (4) Hiện thực hóa các tương tác trực quan như tia laser kèm phản hồi rung haptic, cửa tự động chống kẹp, NPC và phòng chiếu video tư liệu sống động; (5) Chuẩn bị sẵn quy trình tối ưu hóa kết xuất giúp giảm thiểu Draw Calls phục vụ triển khai trên thiết bị di động.")

    append_p(h3, "4.5.3. Hạn chế của ứng dụng")
    append_p(body, "Bên cạnh các kết quả đạt được, ứng dụng hiện còn một số hạn chế: (1) File ghi âm giọng đọc thuyết minh hiện đã tích hợp mẫu hoàn chỉnh cho bệ hiện vật (Cube 72 - Giẻ Triêng / Rơ Măm) và cấu trúc code sẵn sàng, cần tiếp tục thu âm đầy đủ dữ liệu audio cho toàn bộ 12 dân tộc; (2) Dung lượng gói cài đặt APK standalone còn tương đối lớn (khoảng 2.05 GB) do chứa nhiều tài nguyên hình ảnh chất lượng cao và video tư liệu chưa qua nén chuyên sâu; (3) Tương tác với hiện vật hiện dừng lại ở việc xem thông tin thuyết minh 2D, chưa hỗ trợ cầm nắm xoay mô hình 3D hay mặc thử ảo trên nhân vật; (4) Chưa hỗ trợ tính năng kết nối mạng đa người tham quan cùng lúc (Multiplayer); (5) Cử động của NPC tham quan còn ở mức mô phỏng chuyển động cơ bản, chưa áp dụng hệ thống animation xương hay Inverse Kinematics phức tạp.")

    # ==================== KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN ====================
    append_p(chapter, "KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN", page_break=True)

    append_p(h2, "1. Kết luận")
    append_p(body, "Đề tài \"Mô phỏng Bảo tàng Trưng bày Trang phục Truyền thống Tây Nguyên\" đã hoàn thành trọn vẹn mục tiêu nghiên cứu và xây dựng một không gian bảo tàng thực tại ảo hoàn chỉnh, giàu tính văn hóa và tính tương tác. Ứng dụng đã số hóa và trưng bày trang phục truyền thống của 12 dân tộc tiêu biểu vùng đất Tây Nguyên (Ba Na, Brâu, Chư-Ru, Cơ Ho, Ê Đê, Gia Rai, Giẻ Triêng, M'Nông, Mạ, Rơ Măm, Xơ Đăng, Xtiêng). Hệ thống hỗ trợ cơ chế vận hành kép linh hoạt giữa kính VR (Meta Quest) và máy tính thông thường (Desktop PC), đảm bảo khả năng tiếp cận rộng rãi cho nhiều đối tượng người dùng.")
    append_p(body, "Điểm nổi bật của đề tài là việc tích hợp thành công hệ thống thuyết minh đa phương thức: kết hợp giữa bảng ảnh pop-up trực quan (Spatial UI) và bài đọc thuyết minh ghi âm (Voice Narration Audio Guide) với cơ chế tự động điều phối âm lượng theo khoảng cách. Cùng với hệ thống cửa trượt kính tự động chống kẹp, nhân vật NPC tham quan và phòng chiếu phim tư liệu cồng chiêng với hiệu ứng ánh sáng Ambilight, ứng dụng mang lại trải nghiệm tham quan sống động, tự nhiên và trọn vẹn như bước vào một bảo tàng thực thụ.")

    append_p(h2, "2. Hướng phát triển")
    append_p(body, "Trong các giai đoạn phát triển tiếp theo, đề tài có thể tiếp tục được hoàn thiện và mở rộng theo các định hướng sau:")
    append_p(body, "(1) Hoàn thiện dữ liệu thuyết minh âm thanh: Tiếp tục thu âm và tích hợp trọn bộ 12 file giọng đọc thuyết minh chuyên nghiệp cho toàn bộ các bệ hiện vật dân tộc còn lại trong bảo tàng.")
    append_p(body, "(2) Nâng cấp mô hình 3D và tính năng thử trang phục ảo: Chuyển đổi các pano hình ảnh 2D sang mô hình 3D tương tác cao, cho phép người dùng dùng tay cầm VR cầm nắm, xoay 360 độ, phóng to quan sát từng đường kim mũi chỉ, hoa văn thổ cẩm và tích hợp tính năng mặc thử trang phục ảo (Virtual Try-On).")
    append_p(body, "(3) Phát triển tính năng mạng đa người dùng (Multiplayer VR): Cho phép nhiều khách tham quan hoặc hướng dẫn viên du lịch cùng kết nối, giao tiếp giọng nói và cùng trải nghiệm trong không gian bảo tàng ảo theo thời gian thực.")
    append_p(body, "(4) Tích hợp trí tuệ nhân tạo cho nhân vật NPC (AI Guide NPC): Ứng dụng mô hình ngôn ngữ lớn (LLM) và giọng nói tổng hợp để biến NPC thành hướng dẫn viên ảo thông minh, có thể trò chuyện và giải đáp trực tiếp các câu hỏi lịch sử, văn hóa của khách tham quan.")
    append_p(body, "(5) Tối ưu hóa dung lượng đóng gói: Áp dụng Texture Streaming và nén video chuyên sâu nhằm giảm dung lượng file cài đặt APK xuống dưới 1 GB, chuẩn bị cho việc phát hành trực tiếp trên kho ứng dụng Meta Quest Store / SideQuest.")

    # ==================== TÀI LIỆU THAM KHẢO ====================
    append_p(chapter, "TÀI LIỆU THAM KHẢO", page_break=True)

    references = [
        "[1] Unity Technologies (2024), Unity User Manual (6000.0 LTS) - XR Interaction Toolkit & Universal Render Pipeline Documentation, https://docs.unity3d.com.",
        "[2] Khronos Group (2023), The OpenXR Specification - Cross-platform portable virtual reality and augmented reality standard, Khronos Standards.",
        "[3] Meta Platforms Inc. (2024), Meta Quest Developer Documentation - Designing for Virtual Reality & Performance Guidelines, Meta Horizon Developers.",
        "[4] Viện Văn hóa Dân gian (2012), Trang phục truyền thống các dân tộc thiểu số miền núi phía Bắc và Tây Nguyên, Nhà xuất bản Văn hóa Dân tộc, Hà Nội.",
        "[5] Bảo tàng Dân tộc học Việt Nam (2018), Sắc màu các dân tộc Việt Nam qua sưu tập trang phục truyền thống, Nhà xuất bản Mỹ thuật, Hà Nội.",
        "[6] Ngô Đức Thịnh (2009), Không gian văn hóa Cồng chiêng Tây Nguyên - Kiệt tác di sản văn hóa phi vật thể của nhân loại, Nhà xuất bản Trẻ, TP. Hồ Chí Minh.",
        "[7] ISO/IEC 9241-940:2017, Ergonomics of human-system interaction - Evaluation of tactile and haptic interactions in virtual environments, International Organization for Standardization."
    ]

    for ref in references:
        append_p(body, ref)

    # Save
    os.makedirs(os.path.dirname(OUTPUT_QA), exist_ok=True)
    doc.save(OUTPUT_QA)
    doc.save(OUTPUT_ROOT)
    print(f"Saved docx successfully.")

if __name__ == "__main__":
    build_document()

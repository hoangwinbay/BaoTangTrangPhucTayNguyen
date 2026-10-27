import os
import shutil

def remove_cs_comments(source: str) -> str:
    i = 0
    n = len(source)
    output = []
    
    while i < n:
        # Raw string literal
        if source[i:i+3] == '"""':
            q_count = 0
            while i + q_count < n and source[i + q_count] == '"':
                q_count += 1
            output.append(source[i:i+q_count])
            i += q_count
            end_seq = '"' * q_count
            end_idx = source.find(end_seq, i)
            if end_idx != -1:
                output.append(source[i:end_idx + q_count])
                i = end_idx + q_count
            else:
                output.append(source[i:])
                i = n
        # $@" or @$"
        elif (source[i:i+3] == '$@"' or source[i:i+3] == '@$"'):
            output.append(source[i:i+3])
            i += 3
            while i < n:
                if source[i] == '"':
                    output.append('"')
                    i += 1
                    if i < n and source[i] == '"':
                        output.append('"')
                        i += 1
                    else:
                        break
                else:
                    output.append(source[i])
                    i += 1
        # @"
        elif source[i:i+2] == '@"':
            output.append('@"')
            i += 2
            while i < n:
                if source[i] == '"':
                    output.append('"')
                    i += 1
                    if i < n and source[i] == '"':
                        output.append('"')
                        i += 1
                    else:
                        break
                else:
                    output.append(source[i])
                    i += 1
        # " or $"
        elif source[i] == '"' or (source[i] == '$' and i + 1 < n and source[i+1] == '"'):
            if source[i] == '$':
                output.append('$"')
                i += 2
            else:
                output.append('"')
                i += 1
            while i < n:
                ch = source[i]
                output.append(ch)
                i += 1
                if ch == '\\':
                    if i < n:
                        output.append(source[i])
                        i += 1
                elif ch == '"':
                    break
        # '
        elif source[i] == "'":
            output.append("'")
            i += 1
            while i < n:
                ch = source[i]
                output.append(ch)
                i += 1
                if ch == '\\':
                    if i < n:
                        output.append(source[i])
                        i += 1
                elif ch == "'":
                    break
        # //
        elif source[i:i+2] == '//':
            i += 2
            while i < n and source[i] not in ('\r', '\n'):
                i += 1
        # /*
        elif source[i:i+2] == '/*':
            i += 2
            while i + 1 < n and not (source[i] == '*' and source[i+1] == '/'):
                if source[i] == '\n':
                    output.append('\n')
                i += 1
            i += 2
        else:
            output.append(source[i])
            i += 1

    cleaned = "".join(output)
    
    clean_lines = cleaned.splitlines()
    orig_lines = source.splitlines()
    
    final_lines = []
    consecutive_blank = 0
    
    for idx, line in enumerate(clean_lines):
        r = line.rstrip()
        if not r.strip():
            # If line had content in original, it was purely comments -> drop it
            if idx < len(orig_lines) and orig_lines[idx].strip():
                continue
            else:
                consecutive_blank += 1
                if consecutive_blank <= 1:
                    final_lines.append("")
        else:
            consecutive_blank = 0
            final_lines.append(r)
            
    while final_lines and not final_lines[0].strip():
        final_lines.pop(0)
    while final_lines and not final_lines[-1].strip():
        final_lines.pop()
        
    return "\n".join(final_lines) + "\n"

def process_all():
    target_dirs = ["Assets/Editor", "Assets/Scripts"]
    backup_root = "ScriptBackups_BeforeCommentRemoval"
    os.makedirs(backup_root, exist_ok=True)
    
    total_files = 0
    total_orig_lines = 0
    total_new_lines = 0
    
    report = []
    
    for d in target_dirs:
        for root, _, files in os.walk(d):
            for f in files:
                if f.endswith(".cs"):
                    file_path = os.path.join(root, f)
                    
                    # 1. Backup
                    rel_path = os.path.relpath(file_path, ".")
                    backup_path = os.path.join(backup_root, rel_path)
                    os.makedirs(os.path.dirname(backup_path), exist_ok=True)
                    shutil.copy2(file_path, backup_path)
                    
                    # 2. Read with raw bytes to detect BOM
                    with open(file_path, "rb") as bf:
                        raw_bytes = bf.read()
                    has_bom = raw_bytes.startswith(b"\xef\xbb\xbf")
                    
                    # Decode
                    text = raw_bytes.decode("utf-8-sig")
                    orig_line_count = len(text.splitlines())
                    
                    # 3. Strip comments
                    stripped_text = remove_cs_comments(text)
                    new_line_count = len(stripped_text.splitlines())
                    
                    # 4. Write back
                    out_encoding = "utf-8-sig" if has_bom else "utf-8"
                    with open(file_path, "w", encoding=out_encoding, newline="\n") as out_f:
                        out_f.write(stripped_text)
                        
                    total_files += 1
                    total_orig_lines += orig_line_count
                    total_new_lines += new_line_count
                    diff = orig_line_count - new_line_count
                    report.append((rel_path, orig_line_count, new_line_count, diff))
                    
    print(f"Hoàn thành xử lý {total_files} file scripts!")
    print(f"Tổng số dòng ban đầu: {total_orig_lines:,}")
    print(f"Tổng số dòng sau khi loại bỏ comment: {total_new_lines:,}")
    print(f"Tổng số dòng comment đã loại bỏ: {(total_orig_lines - total_new_lines):,}")
    print("-" * 60)
    for path, o, n, d in report:
        print(f"{path}: {o} -> {n} dòng (đã bỏ {d} dòng comment)")

if __name__ == "__main__":
    process_all()

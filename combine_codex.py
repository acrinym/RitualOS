import os

CODEX_DIR = "docs/DreamDictionary/The_Dictionary_of_Dreams_10000_Dreams_Interpreted/codex"
OUTPUT_FILE = os.path.join(CODEX_DIR, "codex_combined.md")

files = sorted(f for f in os.listdir(CODEX_DIR) if f.endswith('.md') and f != 'codex_combined.md')

with open(OUTPUT_FILE, 'w') as out:
    for filename in files:
        path = os.path.join(CODEX_DIR, filename)
        with open(path, 'r') as f:
            lines = f.readlines()
        if not lines:
            continue
        header = lines[0].strip()
        if header.startswith('### SYMBOL:'):
            symbol = header.replace('### SYMBOL:', '').strip()
            out.write(f"## {symbol}\n")
            idx = 1
            if idx < len(lines) and lines[idx].strip() == '':
                idx += 1
            out.write('\n')
            out.writelines(lines[idx:])
            if not lines[-1].endswith('\n'):
                out.write('\n')
        else:
            out.writelines(lines)
            if not lines[-1].endswith('\n'):
                out.write('\n')

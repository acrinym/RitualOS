import os
from sumy.parsers.plaintext import PlaintextParser
from sumy.nlp.tokenizers import Tokenizer
from sumy.summarizers.lsa import LsaSummarizer

SRC_DIR = "docs/DreamDictionary/The_Dictionary_of_Dreams_10000_Dreams_Interpreted"
DST_DIR = os.path.join(SRC_DIR, "codex")
os.makedirs(DST_DIR, exist_ok=True)

summarizer = LsaSummarizer()

def summarize(text, sentences=3):
    if not text.strip():
        return "(No content found)"
    parser = PlaintextParser.from_string(text, Tokenizer("english"))
    summary = summarizer(parser.document, sentences)
    return " ".join(str(sentence) for sentence in summary)

for filename in sorted(os.listdir(SRC_DIR)):
    if not filename.endswith(".md"):
        continue
    src_path = os.path.join(SRC_DIR, filename)
    with open(src_path, 'r') as f:
        lines = f.readlines()
    # remove page header and blank lines
    body_lines = [ln for ln in lines if not ln.startswith("# Page")]
    body_text = "".join(body_lines).strip()
    summary = summarize(body_text)

    page_num = filename.replace("page", "").replace(".md", "").lstrip("0") or "0"
    out = []
    out.append(f"### SYMBOL: Page {page_num}")
    out.append("")
    out.append("Original:")
    out.append("> " + body_text.replace('\n', '\n> '))
    out.append("\n---\n")
    out.append("**Codex, reinterpret this for RitualOS.**\nWrite it as if it were being read aloud in a sacred dream chamber.\nLink it to potential chakras, elements, and field dynamics.\nUse poetic but grounded language.\nHighlight both shadow and light meanings.\n")
    out.append("---\n")
    out.append("🔁 Codex Interpretation:")
    out.append(summary)
    out.append("\nChakras: Root 🟥, Sacral 🟧")
    out.append("\nElemental Echo: Fire 🔥, Earth 🌍")
    out.append("\nField Directive: Pause before action. Let truth shed old skin.\n")

    dst_path = os.path.join(DST_DIR, filename)
    with open(dst_path, 'w') as f:
        f.write("\n".join(out))

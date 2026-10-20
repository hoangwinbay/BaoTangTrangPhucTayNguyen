import os
import sys

# Ensure this directory is in python path
current_dir = os.path.dirname(os.path.abspath(__file__))
if current_dir not in sys.path:
    sys.path.insert(0, current_dir)

from build_full_report import build_document

if __name__ == "__main__":
    build_document()

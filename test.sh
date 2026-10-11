#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
BIN="$ROOT/bin"
TEST_EXE="$BIN/CompareLogicTests.exe"

mkdir -p "$BIN"

if ! command -v mcs >/dev/null 2>&1; then
    echo "FEHLER: 'mcs' (Mono C# Compiler) nicht gefunden." >&2
    echo "Installieren mit: sudo apt install mono-devel   (bzw. das Paket deiner Distribution)" >&2
    exit 1
fi

mcs -out:"$TEST_EXE" \
  "$ROOT/tests/CompareLogicTests.cs" \
  "$ROOT/src/CompareLogic.cs"
mono "$TEST_EXE"
python3 "$ROOT/tests/validate_mod.py"

printf 'All Compare tests passed.\n'

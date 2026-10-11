#!/usr/bin/env bash
# Compare Build Script (7 Days to Die v3.x, Linux / Mono)
#
# Verwendung:
#   bash build.sh                 # Spielverzeichnis automatisch suchen
#   bash build.sh /pfad/zum/spiel # Spielverzeichnis explizit angeben
#   GAME_DIR=/pfad/zum/spiel bash build.sh
#
# Voraussetzung: Mono (`mcs`). Unter Debian/Ubuntu: sudo apt install mono-devel
#
# Die Spiel-Assemblies werden NUR zum Kompilieren referenziert und nie kopiert.
# Harmony kommt ausschließlich aus der Spielinstallation (<Spiel>/Mods/0_TFP_Harmony/).

set -u

MOD_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SRC_DIR="$MOD_ROOT/src"
BIN_DIR="$MOD_ROOT/bin"
DIST="$MOD_ROOT/dist/Compare"
OUT_DLL="$BIN_DIR/Compare.dll"

# ---------------------------------------------------------------------------
# 1) Spielverzeichnis bestimmen
# ---------------------------------------------------------------------------
find_game_dir() {
    local candidates=(
        "${GAME_DIR:-}"
        "$HOME/.steam/steam/steamapps/common/7 Days To Die"
        "$HOME/.local/share/Steam/steamapps/common/7 Days To Die"
        "$HOME/.steam/steam/steamapps/common/7 Days to Die"
        "/mnt/c/Program Files (x86)/Steam/steamapps/common/7 Days To Die"
    )
    local c
    for c in "${candidates[@]}"; do
        [ -n "$c" ] || continue
        if [ -d "$c/7DaysToDie_Data/Managed" ]; then
            printf '%s\n' "$c"
            return 0
        fi
    done
    return 1
}

GAME_DIR="${1:-$(find_game_dir || true)}"

if [ -z "$GAME_DIR" ] || [ ! -d "$GAME_DIR/7DaysToDie_Data/Managed" ]; then
    echo "FEHLER: Spielverzeichnis nicht gefunden." >&2
    echo "Bitte explizit angeben:  bash build.sh \"/pfad/zu/7 Days To Die\"" >&2
    echo "oder:                    GAME_DIR=\"/pfad/zu/7 Days To Die\" bash build.sh" >&2
    exit 1
fi

MANAGED="$GAME_DIR/7DaysToDie_Data/Managed"
HARMONY="${HARMONY:-$GAME_DIR/Mods/0_TFP_Harmony/0Harmony.dll}"

echo "=== Compare Build ==="
echo "Spielverzeichnis: $GAME_DIR"
echo "Ausgabe:          $OUT_DLL"
echo

# ---------------------------------------------------------------------------
# 2) Voraussetzungen prüfen (klare Meldung statt Compiler-Stacktrace)
# ---------------------------------------------------------------------------
if ! command -v mcs >/dev/null 2>&1; then
    echo "FEHLER: 'mcs' (Mono C# Compiler) nicht gefunden." >&2
    echo "Installieren mit: sudo apt install mono-devel   (bzw. das Paket deiner Distribution)" >&2
    exit 1
fi

if ! ls "$SRC_DIR"/*.cs >/dev/null 2>&1; then
    echo "FEHLER: Keine Quelldateien in $SRC_DIR gefunden." >&2
    exit 1
fi

MISSING=0
for req in Assembly-CSharp.dll LogLibrary.dll UnityEngine.dll UnityEngine.CoreModule.dll; do
    if [ ! -f "$MANAGED/$req" ]; then
        echo "FEHLER: Referenz fehlt: $MANAGED/$req" >&2
        echo "        Stimmt das Spielverzeichnis? Erwartet wird <Spiel>/7DaysToDie_Data/Managed." >&2
        MISSING=1
    fi
done
[ "$MISSING" -eq 0 ] || exit 1

# Harmony liegt NICHT im Mod-Ordner, sondern in der Spielinstallation.
if [ ! -f "$HARMONY" ]; then
    FOUND="$(find "$GAME_DIR" -name '0Harmony.dll' 2>/dev/null | head -1)"
    if [ -n "$FOUND" ]; then
        HARMONY="$FOUND"
    else
        echo "FEHLER: 0Harmony.dll nicht gefunden." >&2
        echo "Erwartet: $GAME_DIR/Mods/0_TFP_Harmony/0Harmony.dll" >&2
        echo "Sie gehört zur Spielinstallation (Harmony 2.13) und darf NICHT in den Mod-Ordner kopiert werden." >&2
        echo "Alternativ per HARMONY=/pfad/0Harmony.dll übergeben." >&2
        exit 1
    fi
fi

# netstandard.dll (kommt mit Mono)
NETSTANDARD="${NETSTANDARD:-$(find /usr/lib/mono -name "netstandard.dll" -path "*/4.8-api/*" 2>/dev/null | head -1)}"
if [ -z "$NETSTANDARD" ]; then
    NETSTANDARD="$(find /usr/lib/mono -name "netstandard.dll" 2>/dev/null | head -1)"
fi
if [ -z "$NETSTANDARD" ]; then
    echo "FEHLER: netstandard.dll nicht gefunden (Mono-Installation prüfen)." >&2
    exit 1
fi

# ---------------------------------------------------------------------------
# 3) Kompilieren
# ---------------------------------------------------------------------------
mkdir -p "$BIN_DIR" "$DIST/Config/XUi_InGame"

mcs -target:library \
    -out:"$OUT_DLL" \
    -lib:"$MANAGED" \
    -r:Assembly-CSharp.dll \
    -r:LogLibrary.dll \
    -r:UnityEngine.dll \
    -r:UnityEngine.CoreModule.dll \
    -r:"$HARMONY" \
    -r:"$NETSTANDARD" \
    "$SRC_DIR"/*.cs || { echo "BUILD FAILED" >&2; exit 1; }

# ---------------------------------------------------------------------------
# 4) Runtime-Baum bauen (fertig zum Kopieren in den Mods-Ordner)
# ---------------------------------------------------------------------------
cp "$OUT_DLL" "$DIST/Compare.dll"
cp "$MOD_ROOT/ModInfo.xml" "$DIST/ModInfo.xml"
cp "$MOD_ROOT/Config/XUi_InGame/windows.xml" "$DIST/Config/XUi_InGame/windows.xml"

# Harmony NIE mitpacken: 7 Days to Die liefert es selbst mit
# (<Mods>/0_TFP_Harmony/0Harmony.dll, Harmony 2.13). ModManager lädt JEDE *.dll in
# einem Mod-Ordner; eine zweite Kopie mit gleichem Simple Name erzeugt auf manchen
# Setups Startfehler (Logfenster mit Errors). Nur als Compile-Referenz verwenden.
rm -f "$DIST/0Harmony.dll"
if find "$DIST" -type f \( -iname '*harmony*.dll' -o -iname 'monomod*' -o -iname 'mono.cecil*' \) | grep -q .; then
    echo "FEHLER: Harmony/MonoMod-DLL im Runtime-Baum ($DIST) — niemals mitliefern." >&2
    exit 1
fi

echo
echo "BUILD SUCCESSFUL"
ls -lh "$OUT_DLL"
echo
printf 'Packaged %s (ohne Harmony)\n' "$DIST"
echo
echo "Installation: den Ordner $DIST nach"
echo "  .../7 Days To Die/Mods/Compare/"
echo "kopieren."

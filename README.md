# Compare – Crafting Stat Preview

**In-game crafting comparison tab for 7 Days to Die v3.x (PC / Linux).**

Adds a fourth tab to the crafting window: it compares the weapon or tool you are holding with the
fixed stat values of the selected recipe — before you spend the materials.

An independent development by **Eltern vs Zombies**, released under the **MIT License**.

---

## Features

| Feature | Description |
|---------|-------------|
| ⚖️ **Two-column comparison** | *Held* vs *Craft* side by side in the crafting window |
| 🧰 **Melee & ranged** | Weapons and tools of the display groups `groupMelee` and `groupRanged` |
| 🔧 **Mod-aware** | The held item is evaluated including its combine stats and installed weapon mods |
| 🎯 **Fair preview** | The craft column uses the unmodified item of the selected crafting quality |
| 🔫 **Ammo-aware** | Uses the currently selected ammo type (runtime magazine name); warns when held and crafted weapon use different ammo and marks ammo-dependent values with `*` |
| 📊 **Percentage on the craft page** | Shows the actual improvement/drop versus the held item |
| 🔢 **Consistent rounding** | Percentages are computed from the rounded vanilla values, so `13` vs `13` is always `0 %` |
| 🎨 **Inverted colour logic** | For "lower is better" stats (e.g. stamina cost) the sign stays mathematical while the colour is inverted — less stamina = negative *and* green |
| 🧩 **Data-driven** | No hard-coded vanilla or overhaul weapons; custom weapons reuse registered vanilla display types and ammo is read from `MagazineItemNames` |
| 🔄 **Live refresh** | The comparison updates immediately when you change the craftable quality level |

---

## Usage

1. Hold the weapon or tool you want to compare.
2. Open the crafting window or the matching workstation.
3. Select the recipe and the quality level you are aiming for.
4. Open the tab with the swap-arrow icon and the **Compare** tooltip.

---

## Installation

1. Download `Compare-Crafting-Stat-Preview-<version>.zip` from the
   [Releases page](https://github.com/hewal-dev/Compare/releases) (or build from source, below).
2. Extract the `Compare` folder into your mods directory:
   ```
   .../7 Days To Die/Mods/Compare/
   ```
   After extracting you should see `Compare.dll`, `ModInfo.xml` and `Config/` in that folder.
   **Do not add `0Harmony.dll` here:** the game ships Harmony itself in
   `<Mods>/0_TFP_Harmony/` (Harmony 2.13) and an extra copy inside a mod folder can produce
   errors at startup. Only an older release ZIP (≤ 1.0.4) contained that file — delete it if you
   extracted one of those.
3. **Disable EAC** (Easy Anti-Cheat) for the game — required for any Harmony mod.
4. Start the game. `[Compare] Initializing Compare x.y.z` appears in the client log.

For a clean install, only copy build outputs — never the source files, build scripts or project
configuration into the game's mods folder.

Multiplayer: client and server must run the **same mod version**.

---

## Building from Source

The mod is plain C# compiled against the game's own assemblies. Nothing is bundled from the game —
you compile against your own local installation.

### Requirements

- **7 Days to Die v3.x** installed locally (used only as a compile-time reference)
- **Mono** (`mcs`) on Linux
- `0Harmony.dll` from your own installation, at `<Game>/Mods/0_TFP_Harmony/0Harmony.dll`
  (Harmony 2.13, MIT-licensed). The build script picks it up automatically — never copy a Harmony
  DLL into a mod folder.

### Linux (Mono)

```bash
# auto-detect the game directory
bash build.sh

# or point it explicitly
bash build.sh "/path/to/7 Days To Die"
GAME_DIR="/path/to/7 Days To Die" bash build.sh
```

The script checks its prerequisites (compiler, game assemblies, Harmony, `netstandard.dll`) and
fails with a clear message if something is missing. It writes `bin/Compare.dll` and prepares the
ready-to-copy runtime tree in `dist/Compare/` (DLL + `ModInfo.xml` + `Config/`).

### Windows

No MSBuild project is shipped yet — the compile line from `build.sh` maps directly to the .NET
compiler:

```bat
csc -target:library -out:Compare.dll ^
    -lib:"C:\Program Files (x86)\Steam\steamapps\common\7 Days To Die\7DaysToDie_Data\Managed" ^
    -r:Assembly-CSharp.dll -r:LogLibrary.dll -r:UnityEngine.dll -r:UnityEngine.CoreModule.dll ^
    -r:"C:\Program Files (x86)\Steam\steamapps\common\7 Days To Die\Mods\0_TFP_Harmony\0Harmony.dll" ^
    src\*.cs
```

Then copy `Compare.dll`, `ModInfo.xml` and `Config/` into
`.../7 Days To Die/Mods/Compare/`.

### Tests

```bash
bash test.sh     # logic tests (Mono) + package validation (python3)
```

---

## Compatibility

Statically checked against:

- Vanilla V 3.1.0 (b14)
- V3 Preppocalypse 3.1.0
- Preppocalypse 96-slot backpack
- ProxiStack 1.1.x
- OcbStopFuelWaste

Preppocalypse does not replace the `craftingInfoPanel` that Compare extends. In-game verification
lives in [PROGRESS.md](PROGRESS.md) (developer log, German); the README is authoritative for users.

---

## Project Layout

```
Compare/
├── src/CompareMod.cs           Mod entry point + Harmony bootstrap
├── src/CraftingComparePatch.cs Harmony patches on the crafting window
├── src/CompareLogic.cs         Pure stat/percentage logic (unit-tested)
├── Config/XUi_InGame/          XUi modlet (windows.xml, Compare tab)
├── tests/CompareLogicTests.cs  Logic tests (Mono)
├── tests/validate_mod.py       Package validation
├── build.sh                    Linux/Mono build script
├── test.sh                     Runs logic tests + package validation
└── tools/build_nexus_media.py  Promo-image generator (optional, needs Pillow)
```

---

## License & Credits

**MIT License** — see [LICENSE](LICENSE). Copyright (c) 2026 Eltern vs Zombies.

You may modify this mod and include it in other projects, including overhauls, as long as
**Eltern vs Zombies** is credited as the original author.

- **Author:** Eltern vs Zombies — [YouTube](https://www.youtube.com/@ElternvsZombies)
- **Third-party:** the mod is compiled against the [Harmony](https://github.com/pardeike/Harmony)
  library by Andreas Pardeike (MIT). **No Harmony binary is included in this repository or in the
  release ZIP** — 7 Days to Die ships its own copy in `<Mods>/0_TFP_Harmony/`. 7 Days to Die game
  assemblies are **not** included and are referenced only at compile time.

**Not affiliated with The Fun Pimps.** 7 Days to Die is a trademark of The Fun Pimps Entertainment LLC.

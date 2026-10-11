# Compare – Fortschritt

## Status

Version **1.0.5** ist implementiert, kompiliert, statisch geprüft und nach `/home/hewal/7 Days/0 einzelne mods/Compare/` synchronisiert — funktional unverändert gegenüber 1.0.4, nur Versionsstring und harmony-freies Paket. **Von Hewal im Spiel bestätigt: Compare funktioniert wieder und die Zutatenliste wird bei Rezepten wieder angezeigt** (Bestätigung gilt für den 1.0.4-Stand; die 1.0.5-Laufzeit ist identisch, DLL-Hash `cef4426d…`, aktiv nach dem nächsten Spielstart mit Logzeile `[Compare] Initializing Compare 1.0.5`).

## Quellcode-Veröffentlichung – 11.10.2026

Öffentliches Quellcode-Repository angelegt: **https://github.com/hewal-dev/Compare**
(public, MIT, Branch `main`) — analog zu ProxiStack.

Was für ein fremdes Setup ergänzt/korrigiert wurde:

- **`LICENSE`** (MIT, Copyright 2026 Eltern vs Zombies) — vorher gab es nur die Angabe im Text.
- **`.gitignore`** — schließt Build-Artefakte (`*.dll`, `bin/`, `obj/`, `*.exe`), `dist/`,
  `releases/`, `archives/`, `*.zip`, `media/` und `__pycache__/` aus. Die lokale
  Harmony-Kopie im Projektordner (`Compare/0Harmony.dll`, Build-Referenz) wird damit **nicht**
  mitveröffentlicht.
- **`build.sh` portabel** — Spielverzeichnis per Argument/`GAME_DIR`/Auto-Erkennung,
  Voraussetzungen (mcs, Spiel-Assemblies, Harmony, `netstandard.dll`) werden mit klarer Meldung
  geprüft, absoluter Autorenpfad entfernt. Die Harmony-Referenz kommt jetzt aus der
  **Spielinstallation** (`<Spiel>/Mods/0_TFP_Harmony/0Harmony.dll`), nicht aus dem Projektordner.
  Der Runtime-Baum `dist/Compare/` wird mitgebaut (3 Dateien) inkl. Harmony-Wächter.
- **`test.sh` portabel** — Testbinär nach `bin/` statt `/tmp`, `python3` statt `python`,
  `mcs`-Prüfung.
- **`README.md` neu für Fremde** (englisch, gleiche Struktur wie ProxiStack): Funktionen,
  Bedienung, Installation, Build unter Linux/Windows, Testaufruf, Layout, Lizenz/Credits,
  „nicht mit The Fun Pimps verbunden".
- **`tools/build_nexus_media.py`** — fest eingetragene Autorenpfade entfernt: Bildquellen jetzt
  per Argument, `NEXUS_SOURCES` oder `media/sources/`, Schriften per `NEXUS_FONT_*`
  überschreibbar, klare Fehlermeldungen statt Traceback (benötigt weiterhin Pillow).

**Nachweis (fremdes Layout, ohne lokale Harmony-Kopie):** Arbeitsbaum in eine Scratch-Kopie
ohne `0Harmony.dll`/`bin`/`dist`/`releases`/`media` gestellt, dort `bash build.sh` und
`bash test.sh` ausgeführt → `BUILD SUCCESSFUL`, alle Tests grün, erzeugte `Compare.dll`
**byte-identisch** mit dem Release-Build
(`cef4426d9ab11cf17333bc69233db107abe31d8d5a137b6791dfd7c938dd834b`). Der Live-Ordner wurde dabei
nicht angefasst (Hash vor/nach unverändert).

**Repo gegengeprüft:** public, Lizenz MIT, Topics `7daystodie`, `7-days-to-die`, `mod`, `harmony`,
`csharp`; Remote-Tree **16 Dateien ohne Binaries/Archive**; Release **`v1.0.5`** (Latest) mit dem ZIP
als Asset — **8.783 Bytes**, Status `uploaded`, identisch mit der lokalen Datei →
https://github.com/hewal-dev/Compare/releases/tag/v1.0.5 · Initial-Commit `96babd3`.
Die 1.0.5-ZIP ist zusätzlich bei Nexus hochgeladen (Hewal, 11.10.2026).

## Release-ZIP 1.0.5 – 11.10.2026 (Harmony-frei)

Öffentliches Release-Archiv erstellt: `releases/Compare-Crafting-Stat-Preview-1.0.5.zip`

- **3 Laufzeitdateien** (statt 4 — die Harmony-DLL ist raus): `Compare/ModInfo.xml`,
  `Compare/Compare.dll`, `Compare/Config/XUi_InGame/windows.xml`.
- Größe 21.438 Bytes (ungepackt), `validate_release_zip.py` → **PASS** (ein Wurzelordner,
  Version `1.0.5` == Startmeldung `[Compare] Initializing Compare 1.0.5`, keine Arbeitsdateien,
  kein Harmony/MonoMod im Baum).
- SHA-256: `0e262d53dc575019c691f75348261650bf897d83e756d14f20b828508c645426`.
- Version in `ModInfo.xml` **und** `src/CompareMod.cs` auf 1.0.5 erhöht; Neuaufbau über
  `bash build.sh` (Compile-Referenz ist die Spiel-Harmony `0_TFP_Harmony/0Harmony.dll`).
  DLL-Hash in `bin/`, `dist/` und Livetest: `cef4426d9ab11cf17333bc69233db107abe31d8d5a137b6791dfd7c938dd834b`.
- Livetest synchronisiert (nur `Compare.dll` + `ModInfo.xml`, Dry-Run vorher geprüft):
  `/home/hewal/7 Days/0 einzelne mods/Compare/` enthält jetzt genau `Compare.dll`,
  `ModInfo.xml`, `Config/` — also den Auslieferungszustand.
- Prüfungen: `bash test.sh` → „All Compare tests passed"; `tests/validate_mod.py` →
  „Compare package validation passed"; zusätzlich `validate_release_zip.py` → PASS.
- `NEXUS_DESCRIPTION_DE.md` Kopfzeile auf „Modversion: 1.0.5" korrigiert.
- **Offen:** Upload auf Nexus (Beschreibung dort ebenfalls auf „keine Harmony im Paket" +
  1.0.5 aktualisieren — Textdateien im Repo sind vorbereitet).

## 11.10.2026 – Paketierung ohne Harmony (dauerhaft)

`build.sh` hat die Harmony-DLL in den Runtime-Baum (`dist/Compare/0Harmony.dll`) kopiert —
genau wie bei ProxiStack 1.1.6. Das Spiel liefert Harmony selbst mit
(`<Mods>/0_TFP_Harmony/0Harmony.dll`, Harmony **2.13.0.0**); `ModManager.loadModsFromFolder()`
lädt **jede** `*.dll` in einem Mod-Ordner, und eine zweite Assembly mit gleichem Simple Name
erzeugt auf manchen Setups Startfehler. Deshalb:

- `cp "$HARMONY" "$DIST/0Harmony.dll"` aus `build.sh` entfernt; stattdessen `rm -f` plus eine
  Prüfung, die den Build abbricht, falls doch eine Harmony-/MonoMod-DLL im `dist`-Baum liegt.
- `dist/Compare/0Harmony.dll` gelöscht → Runtime-Baum jetzt **3 Dateien** (ModInfo.xml,
  Compare.dll, Config/XUi_InGame/windows.xml); Paket 21,4 KiB statt 124,5 KiB.
- `NEXUS_DESCRIPTION_DE.md` / `NEXUS_DESCRIPTION_EN.md` / `README.md` korrigiert („Harmony ist
  im Paket enthalten" war falsch bzw. ist ab der nächsten Version falsch).
- Verifikation: `bash build.sh` ohne Warnungen, `Compare.dll`-Hash unverändert
  `8644d3ce…` in `bin/`, `dist/` und Livetest; `bash test.sh` → „All Compare tests passed";
  `tests/validate_mod.py` → „Compare package validation passed"; Testpaketierung mit
  `package_release.py` → 3 Laufzeitdateien, kein Harmony.
- **Unberührt:** Der Live-Ordner behält vorerst seine `0Harmony.dll`; sie ist versionsgleich
  mit der Spielkopie (2.13) und wird von Mono ohnehin auf `0_TFP_Harmony` gebunden
  (Log: `[Compare] Harmony patches registered`). Compare läuft also unverändert weiter.
- **Offen:** Die veröffentlichten ZIPs 1.0.3/1.0.4 enthalten die Harmony-DLL noch. Für ein
  sauberes Paket bei Gelegenheit eine Version **1.0.5** aus dem jetzigen `dist/` bauen und
  hochladen (auf Wunsch übernehme ich das).

1.0.0–1.0.3 liefen gegen V3.1/V3.2 und brachen mit 3.3: die Item-Member `Quality`, `Seed` und `SelectedAmmoTypeIndex` sind von **Feldern auf Properties** umgestellt worden. Der dadurch fehlgeschlagene Binding-Aufbau ließ zusätzlich das Compare-Panel sichtbar über die Vanilla-Zutatenliste fallen (Details unten).

## Release-ZIP 1.0.4 – 07.10.2026

Öffentliches Release-Archiv neu erstellt: `releases/Compare-Crafting-Stat-Preview-1.0.4.zip`
(124.544 Bytes, 4 Laufzeitdateien).

- Inhalt: `Compare/ModInfo.xml`, `Compare/Compare.dll`, `Compare/0Harmony.dll`,
  `Compare/Config/XUi_InGame/windows.xml` — byte-identisch mit `dist/Compare/` und mit
  der im Spiel bestätigten Live-Version.
- Paketierung über `package_release.py` des Nexus-Publishing-Skills; Validierung
  (`validate_release_zip.py`) **PASS**: genau ein Wurzelordner `Compare/`,
  `ModInfo.xml` mit Version `1.0.4`, keine Quell-/Arbeitsdateien, CRC fehlerfrei.
- SHA-256: `ed75b0bdaa12b55cb2528c026c6700bf12101aa1b5916c69386250bfa7d73dcc`.
- Vorheriger Stand war `Compare-Crafting-Stat-Preview-1.0.3.zip` (noch aus der
  V3.1/V3.2-Zeit, entsprach nicht der laufenden 1.0.4).

## 1.0.4 – Portierung V3.2 → V3.3.0 (b18)

Ursache per Log, `monodis` gegen die installierte Assembly und IL-Vergleich belegt:

```
WRN [Compare] Binding error: System.MissingFieldException:
    Field not found: uint16 .ItemValue.Quality
    at Compare.CraftingComparePatch.GetCachedViewModel(...)
```

Betroffen waren **drei** Member, nicht nur der im Log genannte:

| v3.2 und früher | v3.3.0 (b18) | Fundstelle |
|---|---|---|
| `ldfld ItemValue::Quality` | `callvirt ItemValue::get_Quality()` | `oldValue.Quality` |
| `ldfld ItemValue::Seed` | `callvirt ItemValue::get_Seed()` | `itemValue.Seed` |
| `ldfld`/`stfld ItemValue::SelectedAmmoTypeIndex` | `get_`/`set_SelectedAmmoTypeIndex` | `previewValue.SelectedAmmoTypeIndex` |

Belege: `monodis --fields` führt `ItemValue::quality` (kleingeschrieben, privat) statt `Quality`; `monodis --property` liefert `Quality`, `Seed`, `SelectedAmmoTypeIndex`; die Setter `set_Quality`, `set_Seed`, `set_SelectedAmmoTypeIndex` sind vorhanden. Reines Rekompilieren gegen die v3.3-Assembly genügt – der Quelltext bleibt unverändert, `mcs` bindet die Properties.

### Zwei Folgefehler der Portierung

- **Zutatenliste unsichtbar.** Schlägt der Modellaufbau fehl, wird das Binding nicht aufgelöst. XUi fällt dann auf den **literalen** Ausdruck zurück und wertet den nicht-leeren String `"{compareshow}"` als wahr – das Compare-Panel (Höhe 303) legt sich über die Vanilla-Zutatenliste (`ingredients`, Höhe 264) und verdeckt sie. Deshalb steht im Screenshot `Wert`/`Munition` dort, wo `ITEM/NAME/COUNT` stehen müsste. Abgesichert: die Sichtbarkeitsbindings `compareshow`, `compareavailable`, `compareammorow` melden im Fehlerfall `"false"` als aufgelöst. Vanilla-Vorbild für „leer = unsichtbar": `XUiC_DeathBar.GetBindingValueInternal` liefert für `visible` einen leeren Wert.
- **Falscher Style-Key.** `color="[orange]"` ist in V3.3 kein definierter Style-Key (`XUi_Common/styles.xml`, 107 Einträge) und erzeugte 7 × `ERR [XUi] Style key '[orange]' not found!`. Ersetzt durch `[gold]`. Alle von der Mod referenzierten Keys sind jetzt gegen die Vanilla-Definitionen geprüft.

### Werkzeugänderungen

- `build.sh`: `HARMONY` zeigte auf das nicht mehr existierende `/home/hewal/7 Days/v3 Preppocalypse/`; jetzt auf die installierte `Mods/0_TFP_Harmony/0Harmony.dll`.
- Versionsnummer in `ModInfo.xml` **und** der Start-Log-Zeile auf 1.0.4 gehoben.

### Verifikation

- `bash build.sh` gegen `Assembly-CSharp.dll` V 3.3.0 (b18), SHA-256 `6f3e9192…` – grün.
- `bash test.sh`: Logiktests und Paketvalidierung grün.
- IL-Gegenprobe: kein `ldfld ItemValue::Quality|Seed|SelectedAmmoTypeIndex` mehr, 15 Property-Aufrufe.
- Sync nach `/home/hewal/7 Days/0 einzelne mods/Compare/` mit `--dry-run`-Vorprüfung; alle vier Dateien per SHA-256 gegengeprüft.
- Alle `[style]`-Referenzen der Mod existieren in den Vanilla-Style-Definitionen.

### Offen

- **Livetest in 3.3 erforderlich.** Die Diagnose ist belegt, aber die Wirkung im Herstellungsfenster ist noch nicht bestätigt.

## Abgeschlossen

- [x] feste Herstellungswerte ohne Magnitude-Roll verifiziert
- [x] Compare-Reiter in `craftingInfoPanel` ergänzt
- [x] aktuell gehaltenen Gegenstand als Vergleichsquelle verwendet
- [x] Vanilla-Statistikberechnung über `UIDisplayInfoManager` und `XUiM_ItemStack` wiederverwendet
- [x] Combine-Stats und installierte Mods des vorhandenen Gegenstands berücksichtigt
- [x] gleiche Munition per `SelectedAmmoTypeIndex` und Laufzeit-Munitionsname zugeordnet
- [x] Warnung bei inkompatibler Munition eingebaut
- [x] Pure-Logic-Tests und Paketvalidierung erstellt
- [x] gegen V3.1-Assemblies kompiliert
- [x] Preppocalypse-XUi und sechs Custom-Waffen statisch geprüft
- [x] Runtime-Paket mit genau vier Dateien erzeugt
- [x] Mono-IL-Verifikation (`pedump --verify all`) bestanden
- [x] nach `/home/hewal/7 Days/v3 Preppocalypse/Compare/` synchronisiert
- [x] Quell-/Zielhashes aller vier Runtime-Dateien stimmen überein
- [x] unabhängigen Review-Befund behoben: Vanilla-`ref value` wird nur noch nach erfolgreichem Compare-Binding überschrieben
- [x] Regressionstest für unveränderte Vanilla-Bindings ergänzt
- [x] ungültigen `SelectedAmmoTypeIndex` als nicht direkt vergleichbar abgesichert
- [x] Cache bei Plus/Minus der herstellbaren Quality sofort invalidiert
- [x] Magnitude-Prozentdifferenz durch tatsächliche Gesamtwert-Änderung ersetzt
- [x] Prozentbasis ist der gehaltene Gegenstand: `(Herstellung - Gehalten) / |Gehalten|`
- [x] niedrigere bevorzugte Werte wie Ausdauerverbrauch korrekt als Verbesserung bewertet
- [x] Version 1.0.1 nach `/home/hewal/7 Days/v3 Preppocalypse/Compare/` synchronisiert und per SHA-256 verifiziert
- [x] Regressionstest: intern `13,97` und `13` werden sichtbar zu `13`/`13` und ergeben `0 %`
- [x] Integer-/Decimal1-/Decimal2-Rundung analog zum Vanilla-Formatter vor Prozentrechnung
- [x] Version 1.0.2 synchronisiert; Quell- und Ziel-DLL haben SHA-256 `16a6df3c54a5afa248217a87b87024ddaee6c22835abe5d5b859b08d1e902c41`
- [x] Ausdauerkosten: negative Änderung bleibt negativ und wird grün bewertet
- [x] Ausdauerkosten: positive Änderung bleibt positiv und wird rot bewertet
- [x] Version 1.0.3 synchronisiert; Quell- und Ziel-DLL haben SHA-256 `8639c253db836837e0af7af3af2d4ba0c86b50f941ef6536b65fd3295917b285`
- [x] Drei Nutzer-Screenshots für Werkzeug- und Waffenvergleiche als Nexus-Medien aufbereitet
- [x] Nexus-Thumbnail `1920×1080`, Header `1300×372` und drei Galeriebilder `1920×1080` erzeugt
- [x] Alle Nexus-Medien als optimierte JPGs deutlich unter dem 8-MiB-Limit verifiziert
- [x] Reproduzierbares Erzeugungsskript unter `tools/build_nexus_media.py` angelegt
- [x] Einheitlichen EVZ-Aufbau für künftige Nexus-Modbeschreibungen definiert
- [x] Deutschen Compare-Beschreibungsentwurf einschließlich kopierfertiger BBCode-Fassung erstellt
- [x] Nexus-Titel auf `Compare - Crafting Stat Preview` festgelegt (nur von Nexus erlaubte Zeichen)
- [x] Testlisten aus der öffentlichen Beschreibung entfernt
- [x] Nexus-Reiter `Bugs` als Meldeweg festgelegt
- [x] Weiterverwendung und Änderung mit Nennung von Eltern vs Zombies erlaubt
- [x] Englische Nexus-Endfassung als reines, kopierfertiges BBCode erstellt und strukturell validiert
- [x] Zweizeilige englische Kurzbeschreibung am Dateianfang visuell abgetrennt; 309 Zeichen und damit innerhalb des 350-Zeichen-Feldlimits
- [x] Sauberes Runtime-Release als `Compare-Crafting-Stat-Preview-1.0.3.zip` erstellt
- [x] Release-ZIP enthält ausschließlich vier benötigte Dateien im installierbaren Ordner `Compare/`
- [x] ZIP-Pfade, exakte Inhaltsmenge und CRC erfolgreich geprüft

## Offene Punkte

- [x] Spiel startet mit Compare ohne grundlegenden Harmony-/XUi-Fehler
- [x] Compare-Schaltfläche und Vergleich funktionieren grundsätzlich im Herstellungsfenster
- [x] 1.0.4 gegen V3.3.0 (b18) neu gebaut: `ItemValue.Quality/Seed/SelectedAmmoTypeIndex` sind Properties statt Felder
- [x] Zutatenliste wird bei fehlgeschlagenem Binding nicht mehr verdeckt (Sichtbarkeitsbindings melden `"false"`)
- [x] ungültigen Style-Key `[orange]` durch `[gold]` ersetzt
- [x] **Livetest 3.3 bestanden (Hewal): Compare funktioniert wieder, Zutatenliste wieder sichtbar**
- [ ] Log nach dem Start auf `Binding error` und `Style key` prüfen – beide müssen verschwunden sein
- [ ] Compare-Schaltfläche erscheint an Werkbank und Ammo Press
- [ ] Q6-Steinaxt mit Combine-Stats gegen Q1-Feuerwehraxt vergleichen
- [ ] Swiss Vetterli mit `.44 Silver` auf beiden Seiten testen
- [ ] .22-Pistole mit Ball, HP und AP testen
- [ ] unterschiedliche Munition zeigt Warnung und `*` ohne falsche Grün/Rot-Wertung
- [ ] Preppocalypse-96-Slot-Rucksack verursacht keine Layoutkollision
- [ ] Controller-Navigation und 1920×1080-Layout prüfen
- [ ] Q6/Q5/Q4-Vorschau nacheinander wählen und sofort wechselnde Werte/Prozente bestätigen
- [ ] Prozentwert entspricht der Verbesserung/Verschlechterung des herstellbaren Items gegenüber dem gehaltenen Item
- [ ] Nutzerpunkt 3 ergänzen – in der Änderungsnachricht war kein Text enthalten
- [ ] Nexus-Medienentwurf durch Hewal abnehmen lassen
- [ ] Entscheiden, ob die englischen Bildüberschriften beibehalten oder deutsch/zweisprachig angelegt werden
- [ ] Deutschen Nexus-Beschreibungsentwurf durch Hewal abnehmen lassen



## Nexus-Medien

Ausgabeordner: `media/nexus/`

- `compare-thumbnail-1920x1080.jpg` – Produktbild/Thumbnail
- `compare-header-1300x372.jpg` – Modseiten-Header mit ruhiger linker Overlay-Zone
- `compare-gallery-01-pickaxe-1920x1080.jpg` – Werkzeugvergleich Stahlspitzhacke
- `compare-gallery-02-ak47-1920x1080.jpg` – Waffenvergleich AK-47
- `compare-gallery-03-fireaxe-1920x1080.jpg` – Grün/Rot-Prozentvergleich Feuerwehraxt
- `compare-nexus-media-preview.jpg` – Kontaktbogen zur gemeinsamen Vorschau

Beschreibungsentwurf: `NEXUS_DESCRIPTION_DE.md`

Kopierfertige englische Nexus-Fassung: `NEXUS_DESCRIPTION_EN.md`

Release-ZIP: `releases/Compare-Crafting-Stat-Preview-1.0.4.zip`

## Testumgebung dieser Session

`/home/hewal/7 Days/v3 Preppocalypse/Compare/`

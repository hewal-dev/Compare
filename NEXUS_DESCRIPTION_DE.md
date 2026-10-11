# Compare – deutscher Nexus-Beschreibungsentwurf

Stand: 2026-08-17  
Modversion: 1.0.5  
Zielversion: 7 Days to Die V3.1.0 (b14)

## Modtitel

**Compare - Crafting Stat Preview**

Der englische Zusatz passt zum bereits erstellten Produktbild und erklärt internationalen Nutzern sofort, worum es geht.

## Kurzbeschreibung für die Nexus-Mod-Karte

Compare erweitert das Herstellungsfenster um einen eigenen Vergleichsreiter. Dort siehst du vor dem Craften, wie sich die Werte der gewählten Waffe oder des Werkzeugs gegenüber dem aktuell gehaltenen Gegenstand verändern.

## Ausführliche Beschreibung – Inhaltsfassung

### Was macht Compare?

In 7 Days to Die können ältere Waffen und Werkzeuge durch gute Werte, Combine-Stats und installierte Mods stärker sein als ein Gegenstand aus einer höheren Tierstufe. Im normalen Herstellungsfenster ist vor dem Craften jedoch nur schwer zu erkennen, ob sich der neue Gegenstand wirklich lohnt.

Compare ergänzt das Herstellungsfenster um einen vierten Reiter. Er stellt den aktuell gehaltenen Gegenstand den festen Werten des ausgewählten Rezepts und der gewählten Qualitätsstufe gegenüber. Verbesserungen und Verschlechterungen werden direkt als Prozentwert angezeigt.

Die Mod verändert keine Rezepte und keine Gegenstandswerte. Sie fügt ausschließlich eine Vorschau zum Herstellungsfenster hinzu.

### Funktionen

- zweispaltiger Vergleich zwischen dem aktuell gehaltenen und dem herstellbaren Gegenstand;
- unterstützt Nahkampfwaffen, Fernkampfwaffen und Werkzeuge mit den passenden Statistikgruppen;
- berücksichtigt beim vorhandenen Gegenstand dessen tatsächliche Werte einschließlich Combine-Stats und installierter Mods;
- zeigt für die Herstellung einen sauberen, unmodifizierten Gegenstand der ausgewählten Qualitätsstufe;
- aktualisiert die Anzeige sofort, wenn du die herstellbare Qualitätsstufe änderst;
- zeigt die tatsächliche prozentuale Veränderung gegenüber dem gehaltenen Gegenstand;
- Grün bedeutet besser, Rot bedeutet schlechter;
- berücksichtigt Werte, bei denen weniger besser ist, zum Beispiel Ausdauerkosten;
- verwendet bei kompatiblen Fernkampfwaffen die aktuell ausgewählte Munitionsart;
- warnt bei unterschiedlichen Munitionsarten und bewertet davon betroffene Werte nicht irreführend;
- arbeitet datengetrieben und ist nicht auf eine feste Liste von Vanilla-Waffen beschränkt.

### Was wird miteinander verglichen?

Die linke Spalte zeigt den Gegenstand, den dein Charakter gerade in der Hand hält. Seine Combine-Stats und installierten Mods fließen in die angezeigten Werte ein.

Die rechte Spalte zeigt den Gegenstand aus dem ausgewählten Herstellungsrezept in der aktuell eingestellten Qualitätsstufe. Diese Vorschau ist unmodifiziert und besitzt keine nachträglich installierten Mods oder übernommenen Combine-Werte.

Der Prozentwert beschreibt die Veränderung des herstellbaren Gegenstands gegenüber dem gehaltenen Gegenstand. Bei normalen Werten ist eine Erhöhung grün und eine Verringerung rot. Bei Verbrauchswerten wie Ausdauerkosten ist die Bewertung umgekehrt: weniger Verbrauch ist besser und wird deshalb grün dargestellt, das mathematische Minuszeichen bleibt aber erhalten.

### Bedienung

1. Nimm die Waffe oder das Werkzeug in die Hand, das du als Vergleich verwenden möchtest.
2. Öffne das Herstellungsfenster oder eine passende Workstation.
3. Wähle das gewünschte Rezept.
4. Stelle die Qualitätsstufe ein, die du herstellen möchtest.
5. Öffne den Reiter mit dem Tauschpfeil-Symbol und dem Tooltip "Compare".

Die Anzeige wird beim Wechsel der Qualitätsstufe automatisch neu berechnet.

### Installation

1. Lade das Archiv herunter und entpacke es.
2. Kopiere den vollständigen Ordner `Compare` in den `Mods`-Ordner deiner 7-Days-to-Die-Installation.
3. Deaktiviere Easy Anti-Cheat. Compare verwendet Harmony und wird mit aktivem EAC nicht geladen.
4. Starte das Spiel vollständig neu.

Der Pfad sollte anschließend ungefähr so aussehen:

`7 Days To Die/Mods/Compare/ModInfo.xml`

### Anforderungen

- 7 Days to Die V3.1.0 (b14);
- Easy Anti-Cheat muss deaktiviert sein;
- keine weitere Gameplay-Mod erforderlich;
- Harmony wird von **7 Days to Die selbst** mitgeliefert (`Mods/0_TFP_Harmony/0Harmony.dll`); im Paket ist **keine** Harmony-DLL enthalten. Aus Versionen ≤ 1.0.4 bitte `Mods/Compare/0Harmony.dll` löschen.

Für Multiplayer sollte auf Server und beteiligten Clients dieselbe Version der Mod verwendet werden. Jeder Spieler, der die Vergleichsanzeige nutzen möchte, benötigt die Mod lokal.

### Kompatibilität

Compare verändert das `craftingInfoPanel` und die dazugehörige lokale UI-Logik. Die Mod verändert weder Rezepte noch die tatsächliche Herstellung oder die Werte gespeicherter Gegenstände.

Mod-Waffen können unterstützt werden, wenn sie gültige 7-Days-to-Die-Statistik- und Displaygruppen verwenden. Eigene UI-Mods können mit Compare kollidieren, wenn sie denselben Bereich des Herstellungsfensters ersetzen statt ihn zu erweitern.

### Bekannte Einschränkungen

- Der Vergleich ist für unterstützte Waffen und Werkzeuge gedacht. Rüstung, Fahrzeuge, Verbrauchsgegenstände und andere Itemgruppen werden derzeit nicht angezeigt.
- Es werden maximal sieben Statistikzeilen dargestellt.
- Unterschiedliche Munitionsarten können keinen fairen direkten Vergleich ergeben. In diesem Fall warnt die Mod und kennzeichnet betroffene Werte.
- Die Anzeige verwendet den aktuell gehaltenen Gegenstand. Ein Gegenstand, der nur im Rucksack liegt, wird nicht automatisch als Vergleichsquelle ausgewählt.
- Sehr stark veränderte Herstellungsfenster anderer UI-Mods können eine gesonderte Kompatibilitätsanpassung benötigen.

### Deinstallation

Beende das Spiel und lösche den Ordner `Compare` aus deinem `Mods`-Verzeichnis. Die Mod verändert keine Gegenstände, Rezepte oder Spielstände. Eine besondere Bereinigung des Spielstands ist daher nicht erforderlich.

### Fehlerberichte

Fehler sollen über den Reiter **Bugs** auf der Nexus-Modseite gemeldet werden. Bitte gib dort folgende Informationen an:

- deine 7-Days-to-Die-Version einschließlich Buildnummer;
- die verwendete Compare-Version;
- Einzelspieler oder Multiplayer;
- die verwendete Waffe beziehungsweise das Werkzeug und das ausgewählte Rezept;
- andere installierte UI- oder Crafting-Mods;
- einen Screenshot der Anzeige;
- den relevanten Ausschnitt aus dem Client-Log.

### Credits

Entwicklung: **Eltern vs Zombies**  
YouTube: https://www.youtube.com/@ElternVSZombies

Compare verwendet Harmony, um die zusätzliche Anzeige in das Herstellungsfenster einzubinden.

### Berechtigungen

Die Mod darf verändert und als Bestandteil anderer Projekte, zum Beispiel Overhauls, verwendet werden. Dabei muss **Eltern vs Zombies** als ursprünglicher Autor der Mod genannt werden.

## Kopierfertige Nexus-BBCode-Fassung

```text
[heading][color=#e8892a]Was macht Compare?[/color][/heading]

In 7 Days to Die können ältere Waffen und Werkzeuge durch gute Werte, Combine-Stats und installierte Mods stärker sein als ein Gegenstand aus einer höheren Tierstufe. Im normalen Herstellungsfenster ist vor dem Craften jedoch nur schwer zu erkennen, ob sich der neue Gegenstand wirklich lohnt.

Compare ergänzt das Herstellungsfenster um einen vierten Reiter. Er stellt den aktuell gehaltenen Gegenstand den festen Werten des ausgewählten Rezepts und der gewählten Qualitätsstufe gegenüber. Verbesserungen und Verschlechterungen werden direkt als Prozentwert angezeigt.

[b]Compare verändert keine Rezepte und keine Gegenstandswerte.[/b] Die Mod fügt ausschließlich eine Vorschau zum Herstellungsfenster hinzu.

[line]
[heading][color=#e8892a]Funktionen[/color][/heading]

[list]
[*]Zweispaltiger Vergleich zwischen dem aktuell gehaltenen und dem herstellbaren Gegenstand
[*]Unterstützt Nahkampfwaffen, Fernkampfwaffen und Werkzeuge mit passenden Statistikgruppen
[*]Berücksichtigt beim vorhandenen Gegenstand Combine-Stats und installierte Mods
[*]Zeigt einen sauberen, unmodifizierten Herstellungsgegenstand der ausgewählten Qualitätsstufe
[*]Aktualisiert die Anzeige sofort beim Wechsel der herstellbaren Qualitätsstufe
[*]Zeigt die tatsächliche prozentuale Veränderung gegenüber dem gehaltenen Gegenstand
[*]Grün bedeutet besser, Rot bedeutet schlechter
[*]Bewertet auch Werte korrekt, bei denen weniger besser ist, zum Beispiel Ausdauerkosten
[*]Verwendet bei kompatiblen Fernkampfwaffen die aktuell ausgewählte Munitionsart
[*]Warnt bei unterschiedlichen Munitionsarten
[*]Arbeitet datengetrieben und ist nicht auf eine feste Vanilla-Waffenliste beschränkt
[/list]

[line]
[heading][color=#e8892a]Was wird verglichen?[/color][/heading]

Die linke Spalte zeigt den Gegenstand, den dein Charakter gerade in der Hand hält. Seine Combine-Stats und installierten Mods fließen in die angezeigten Werte ein.

Die rechte Spalte zeigt den Gegenstand aus dem ausgewählten Herstellungsrezept in der aktuell eingestellten Qualitätsstufe. Diese Vorschau ist unmodifiziert und besitzt keine nachträglich installierten Mods oder übernommenen Combine-Werte.

Der Prozentwert beschreibt die Veränderung des herstellbaren Gegenstands gegenüber dem gehaltenen Gegenstand. Bei normalen Werten ist eine Erhöhung grün und eine Verringerung rot. Bei Verbrauchswerten wie Ausdauerkosten ist die Bewertung umgekehrt: weniger Verbrauch ist besser und wird deshalb grün dargestellt, das mathematische Minuszeichen bleibt aber erhalten.

[line]
[heading][color=#e8892a]Bedienung[/color][/heading]

[list=1]
[*]Nimm die Waffe oder das Werkzeug in die Hand, das du als Vergleich verwenden möchtest.
[*]Öffne das Herstellungsfenster oder eine passende Workstation.
[*]Wähle das gewünschte Rezept.
[*]Stelle die Qualitätsstufe ein, die du herstellen möchtest.
[*]Öffne den Reiter mit dem Tauschpfeil-Symbol und dem Tooltip [b]Compare[/b].
[/list]

Die Anzeige wird beim Wechsel der Qualitätsstufe automatisch neu berechnet.

[line]
[heading][color=#e8892a]Installation[/color][/heading]

[list=1]
[*]Lade das Archiv herunter und entpacke es.
[*]Kopiere den vollständigen Ordner [b]Compare[/b] in den [b]Mods[/b]-Ordner deiner 7-Days-to-Die-Installation.
[*]Deaktiviere Easy Anti-Cheat. Compare verwendet Harmony und wird mit aktivem EAC nicht geladen.
[*]Starte das Spiel vollständig neu.
[/list]

Der Pfad sollte anschließend ungefähr so aussehen:

[code]7 Days To Die/Mods/Compare/ModInfo.xml[/code]

[line]
[heading][color=#e8892a]Anforderungen[/color][/heading]

[list]
[*]7 Days to Die V3.1.0 (b14)
[*]Easy Anti-Cheat muss deaktiviert sein
[*]Keine weitere Gameplay-Mod erforderlich
[*]Harmony wird von 7 Days to Die selbst mitgeliefert (Mods/0_TFP_Harmony); im Paket ist keine Harmony-DLL enthalten. Aus Versionen ≤ 1.0.4 bitte Mods/Compare/0Harmony.dll löschen.
[/list]

Für Multiplayer sollte auf Server und beteiligten Clients dieselbe Version der Mod verwendet werden. Jeder Spieler, der die Vergleichsanzeige nutzen möchte, benötigt die Mod lokal.

[line]
[heading][color=#e8892a]Kompatibilität[/color][/heading]

Compare verändert das Herstellungsfenster und die dazugehörige lokale UI-Logik. Die Mod verändert weder Rezepte noch die tatsächliche Herstellung oder die Werte gespeicherter Gegenstände.

Mod-Waffen können unterstützt werden, wenn sie gültige 7-Days-to-Die-Statistik- und Displaygruppen verwenden. Eigene UI-Mods können mit Compare kollidieren, wenn sie denselben Bereich des Herstellungsfensters ersetzen statt ihn zu erweitern.

[line]
[heading][color=#e8892a]Bekannte Einschränkungen[/color][/heading]

[list]
[*]Der Vergleich ist für unterstützte Waffen und Werkzeuge gedacht. Rüstung, Fahrzeuge, Verbrauchsgegenstände und andere Itemgruppen werden derzeit nicht angezeigt.
[*]Es werden maximal sieben Statistikzeilen dargestellt.
[*]Unterschiedliche Munitionsarten können keinen fairen direkten Vergleich ergeben. In diesem Fall warnt die Mod und kennzeichnet betroffene Werte.
[*]Die Anzeige verwendet den aktuell gehaltenen Gegenstand. Ein Gegenstand, der nur im Rucksack liegt, wird nicht automatisch als Vergleichsquelle ausgewählt.
[*]Sehr stark veränderte Herstellungsfenster anderer UI-Mods können eine gesonderte Kompatibilitätsanpassung benötigen.
[/list]

[line]
[heading][color=#e8892a]Deinstallation[/color][/heading]

Beende das Spiel und lösche den Ordner [b]Compare[/b] aus deinem [b]Mods[/b]-Verzeichnis. Die Mod verändert keine Gegenstände, Rezepte oder Spielstände. Eine besondere Bereinigung des Spielstands ist daher nicht erforderlich.

[line]
[heading][color=#e8892a]Fehlerberichte[/color][/heading]

Fehler sollen über den Reiter [b]Bugs[/b] auf der Nexus-Modseite gemeldet werden. Bitte gib dort folgende Informationen an:

[list]
[*]7-Days-to-Die-Version einschließlich Buildnummer
[*]Compare-Version
[*]Einzelspieler oder Multiplayer
[*]Verwendete Waffe beziehungsweise Werkzeug und ausgewähltes Rezept
[*]Andere installierte UI- oder Crafting-Mods
[*]Screenshot der Anzeige
[*]Relevanter Ausschnitt aus dem Client-Log
[/list]

[line]
[heading][color=#e8892a]Credits[/color][/heading]

Entwicklung: [b]Eltern vs Zombies[/b]

[url=https://www.youtube.com/@ElternVSZombies]Eltern vs Zombies auf YouTube[/url]

Compare verwendet Harmony, um die zusätzliche Anzeige in das Herstellungsfenster einzubinden.

[line]
[heading][color=#e8892a]Berechtigungen[/color][/heading]

Die Mod darf verändert und als Bestandteil anderer Projekte, zum Beispiel Overhauls, verwendet werden. Dabei muss [b]Eltern vs Zombies[/b] als ursprünglicher Autor der Mod genannt werden.
```

## Vor der englischen Endfassung zu entscheiden

- welche Spielversionen ausdrücklich als unterstützt genannt werden;
- ob zusätzlich ein Link zu den Modquellen veröffentlicht werden soll.

Compare - Crafting Stat Preview

[heading][color=#e8892a]Short description[/color][/heading]

[quote]Compare adds a crafting preview that compares the selected weapon or tool with the item currently in your hands before you spend any resources. It shows real percentage differences for the chosen crafting quality while accounting for the held item's Combine bonuses, installed mods, and compatible ammunition.[/quote]

[line]
[heading][color=#e8892a]What does Compare do?[/color][/heading]

In 7 Days to Die, an older weapon or tool with strong stats, Combine bonuses, and installed mods can outperform an item from a higher tier. The standard crafting window does not make it easy to tell whether crafting the new item will actually be an upgrade.

Compare adds a fourth tab to the crafting window. It places the item you are currently holding next to the fixed stats of the selected recipe and crafting quality. Improvements and downgrades are shown as percentage changes.

[b]Compare does not change recipes or item stats.[/b] It only adds a stat preview to the crafting window.

[line]
[heading][color=#e8892a]Features[/color][/heading]

[list]
[*]Side-by-side comparison of the currently held item and the item you are about to craft
[*]Supports compatible melee weapons, ranged weapons, and tools
[*]Includes Combine bonuses and installed mods when calculating the held item's stats
[*]Shows a clean, unmodified crafting result at the selected quality
[*]Updates immediately when you change the crafting quality
[*]Shows the actual percentage change compared with the held item
[*]Green means better and red means worse
[*]Correctly evaluates stats where lower is better, such as stamina cost
[*]Uses the currently selected ammunition type for compatible ranged weapons
[*]Warns you when different ammunition types prevent a fair comparison
[*]Uses the game's display data instead of relying on a hardcoded list of vanilla weapons
[/list]

[line]
[heading][color=#e8892a]What is being compared?[/color][/heading]

The left column shows the item your character is currently holding. Its Combine bonuses and installed mods are included in the displayed stats.

The right column shows the item from the selected crafting recipe at the chosen quality. This preview is unmodified and does not include installed mods or inherited Combine bonuses.

The percentage shows how the craftable item differs from the held item. For normal stats, an increase is green and a decrease is red. For consumption stats such as stamina cost, the rating is reversed because using less stamina is better. The mathematical sign is preserved, so a lower stamina cost is displayed as a negative percentage in green.

[line]
[heading][color=#e8892a]How to use[/color][/heading]

[list=1]
[*]Hold the weapon or tool you want to use as the comparison item.
[*]Open the crafting window or a compatible workstation.
[*]Select the recipe you want to craft.
[*]Choose the quality you intend to craft.
[*]Open the tab with the swap-arrow icon and the [b]Compare[/b] tooltip.
[/list]

The comparison updates automatically whenever you change the crafting quality.

[line]
[heading][color=#e8892a]Installation[/color][/heading]

[list=1]
[*]Download and extract the archive.
[*]Copy the complete [b]Compare[/b] folder into the [b]Mods[/b] folder of your 7 Days to Die installation.
[*]Disable Easy Anti-Cheat. Compare uses Harmony and will not load while EAC is enabled.
[*]Fully restart the game.
[/list]

The resulting path should look similar to this:

[code]7 Days To Die/Mods/Compare/ModInfo.xml[/code]

[line]
[heading][color=#e8892a]Requirements[/color][/heading]

[list]
[*]7 Days to Die V3.1.0 (b14)
[*]Easy Anti-Cheat must be disabled
[*]No additional gameplay mod is required
[*]Harmony is provided by 7 Days to Die itself (Mods/0_TFP_Harmony); the package contains **no** Harmony DLL. If you installed version ≤ 1.0.4, please delete Mods/Compare/0Harmony.dll.
[/list]

For multiplayer, use the same version of Compare on the server and participating clients. Every player who wants to use the comparison display needs the mod installed locally.

[line]
[heading][color=#e8892a]Compatibility[/color][/heading]

Compare modifies the crafting window and its local UI logic. It does not alter recipes, the crafting process, or the stats of saved items.

Modded weapons can be supported when they use valid 7 Days to Die stat and display groups. UI mods may conflict with Compare if they replace the same part of the crafting window instead of extending it.

[line]
[heading][color=#e8892a]Known limitations[/color][/heading]

[list]
[*]The comparison is intended for supported weapons and tools. Armor, vehicles, consumables, and other item groups are not currently displayed.
[*]A maximum of seven stat rows can be displayed.
[*]Different ammunition types may not allow a fair direct comparison. Compare warns you and marks affected stats in this situation.
[*]The display uses the currently held item. An item that is only stored in your backpack is not selected automatically as the comparison source.
[*]UI mods that heavily replace the crafting window may require a dedicated compatibility patch.
[/list]

[line]
[heading][color=#e8892a]Uninstallation[/color][/heading]

Close the game and delete the [b]Compare[/b] folder from your [b]Mods[/b] directory. Compare does not modify items, recipes, or save files, so no save-game cleanup is required.

[line]
[heading][color=#e8892a]Bug reports[/color][/heading]

Please report problems through the [b]Bugs[/b] tab on this mod page and include the following information:

[list]
[*]Your 7 Days to Die version and build number
[*]Your Compare version
[*]Single-player or multiplayer
[*]The weapon or tool you were holding and the selected recipe
[*]Any other installed UI or crafting mods
[*]A screenshot of the comparison display
[*]The relevant section of the client log
[/list]

[line]
[heading][color=#e8892a]Credits[/color][/heading]

Developed by [b]Eltern vs Zombies[/b]

[url=https://www.youtube.com/@ElternVSZombies]Eltern vs Zombies on YouTube[/url]

Compare uses Harmony to add the comparison display to the crafting window.

[line]
[heading][color=#e8892a]Permissions[/color][/heading]

You may modify this mod and include it in other projects, such as overhauls, as long as [b]Eltern vs Zombies[/b] is credited as the original author of the mod.

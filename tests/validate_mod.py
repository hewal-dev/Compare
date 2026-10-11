#!/usr/bin/env python3
from pathlib import Path
import sys
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
errors = []

required = [
    ROOT / "ModInfo.xml",
    ROOT / "Config" / "XUi_InGame" / "windows.xml",
    ROOT / "src" / "CompareMod.cs",
    ROOT / "src" / "CraftingComparePatch.cs",
]
for path in required:
    if not path.exists():
        errors.append(f"missing {path.relative_to(ROOT)}")

if not errors:
    mod_info = ET.parse(ROOT / "ModInfo.xml").getroot()
    values = {node.tag: node.get("value") for node in mod_info}
    if values.get("Name") != "Compare":
        errors.append("ModInfo Name must be Compare")
    if values.get("DisplayName") != "Compare":
        errors.append("ModInfo DisplayName must be Compare")
    if values.get("SkipWithAntiCheat") != "true":
        errors.append("SkipWithAntiCheat must be true")
    if values.get("ServerSideOnly") == "true":
        errors.append("UI mod must not be ServerSideOnly")

    windows_root = ET.parse(ROOT / "Config" / "XUi_InGame" / "windows.xml").getroot()
    xpath_values = [node.get("xpath", "") for node in windows_root]
    if not any("craftingInfoPanel" in value for value in xpath_values):
        errors.append("XUi patch must target craftingInfoPanel")
    if any("windowCraftingQueue" in value or "windowBackpack" in value for value in xpath_values):
        errors.append("XUi patch must not touch crafting queue or backpack")

    source = "\n".join(path.read_text() for path in (ROOT / "src").glob("*.cs"))
    for required_symbol in (
        "XUiC_CraftingInfoWindow",
        "GetBindingValueInternal",
        "SelectedAmmoTypeIndex",
        "UIDisplayInfoManager",
        "XUiM_ItemStack",
    ):
        if required_symbol not in source:
            errors.append(f"source missing runtime integration: {required_symbol}")

    forbidden_hardcoded_items = (
        "gunRifleT2LeverActionRifleSwissVetterli",
        "gun22HandgunT0Pistol",
        "ammo44MagnumBulletSilver",
    )
    for item_name in forbidden_hardcoded_items:
        if item_name in source:
            errors.append(f"source hardcodes overhaul item {item_name}")

    if "TryGetCompareBinding(state, model, bindingName, out value)" in source:
        errors.append("binding postfix must not pass Vanilla ref value directly as an out parameter")
    if "value = compareValue;" not in source:
        errors.append("binding postfix must assign ref value only after a successful Compare binding match")
    if '"AddQualityButton_OnPress"' not in source or '"SubtractQualityButton_OnPress"' not in source:
        errors.append("quality button handlers must invalidate the Compare view model")

if errors:
    for error in errors:
        print("FAIL", error)
    sys.exit(1)

print("Compare package validation passed.")

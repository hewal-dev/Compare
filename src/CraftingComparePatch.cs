using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Compare
{
    internal sealed class CompareWindowState
    {
        public bool Attached;
        public bool CompareMode;
        public XUiController CompareButton;
        public CompareViewModel CachedModel;
    }

    internal sealed class CompareViewModel
    {
        public bool Available;
        public string OldName = "—";
        public string NewName = "—";
        public string OldAmmo = string.Empty;
        public string NewAmmo = string.Empty;
        public string AmmoWarning = string.Empty;
        public readonly string[] Titles = new string[7];
        public readonly string[] OldValues = new string[7];
        public readonly string[] NewValues = new string[7];

        public CompareViewModel()
        {
            for (int i = 0; i < 7; i++)
            {
                Titles[i] = string.Empty;
                OldValues[i] = string.Empty;
                NewValues[i] = string.Empty;
            }
        }

        public bool HasAmmoRow
        {
            get { return !string.IsNullOrEmpty(OldAmmo) || !string.IsNullOrEmpty(NewAmmo); }
        }
    }

    public static class CraftingComparePatch
    {
        private static readonly ConditionalWeakTable<XUiC_CraftingInfoWindow, CompareWindowState> States =
            new ConditionalWeakTable<XUiC_CraftingInfoWindow, CompareWindowState>();

        private static bool errorLogged;

        private static CompareWindowState GetState(XUiC_CraftingInfoWindow window)
        {
            return States.GetOrCreateValue(window);
        }

        private static void SetCompareMode(XUiC_CraftingInfoWindow window, bool enabled)
        {
            CompareWindowState state = GetState(window);
            state.CompareMode = enabled;
            InvalidateViewModel(window);

            if (state.CompareButton != null)
            {
                state.CompareButton.Selected(enabled);
            }

            if (enabled)
            {
                if (window.ingredientsButton != null)
                {
                    window.ingredientsButton.Selected(false);
                }
                if (window.descriptionButton != null)
                {
                    window.descriptionButton.Selected(false);
                }
                if (window.unlockedByButton != null)
                {
                    window.unlockedByButton.Selected(false);
                }
            }

        }

        private static void InvalidateViewModel(XUiC_CraftingInfoWindow window)
        {
            GetState(window).CachedModel = null;
            window.IsDirty = true;
        }

        [HarmonyPatch(typeof(XUiC_CraftingInfoWindow), "Init")]
        private static class Patch_Init
        {
            [HarmonyPostfix]
            private static void Postfix(XUiC_CraftingInfoWindow __instance)
            {
                CompareWindowState state = GetState(__instance);
                if (state.Attached)
                {
                    return;
                }

                state.CompareButton = __instance.GetChildById("compareButton");
                if (state.CompareButton == null)
                {
                    Log.Warning("[Compare] compareButton was not found in craftingInfoPanel");
                    return;
                }

                state.CompareButton.OnPress += delegate(XUiController sender, int mouseButton)
                {
                    SetCompareMode(__instance, true);
                };
                state.Attached = true;
                Log.Out("[Compare] Compare button attached to craftingInfoPanel");
            }
        }

        [HarmonyPatch(typeof(XUiC_CraftingInfoWindow), "IngredientsButton_OnPress")]
        private static class Patch_IngredientsButton
        {
            [HarmonyPostfix]
            private static void Postfix(XUiC_CraftingInfoWindow __instance)
            {
                SetCompareMode(__instance, false);
            }
        }

        [HarmonyPatch(typeof(XUiC_CraftingInfoWindow), "DescriptionButton_OnPress")]
        private static class Patch_DescriptionButton
        {
            [HarmonyPostfix]
            private static void Postfix(XUiC_CraftingInfoWindow __instance)
            {
                SetCompareMode(__instance, false);
            }
        }

        [HarmonyPatch(typeof(XUiC_CraftingInfoWindow), "UnlockedByButton_OnPress")]
        private static class Patch_UnlockedByButton
        {
            [HarmonyPostfix]
            private static void Postfix(XUiC_CraftingInfoWindow __instance)
            {
                SetCompareMode(__instance, false);
            }
        }

        [HarmonyPatch(typeof(XUiC_CraftingInfoWindow), "AddQualityButton_OnPress")]
        private static class Patch_AddQualityButton
        {
            [HarmonyPostfix]
            private static void Postfix(XUiC_CraftingInfoWindow __instance)
            {
                InvalidateViewModel(__instance);
            }
        }

        [HarmonyPatch(typeof(XUiC_CraftingInfoWindow), "SubtractQualityButton_OnPress")]
        private static class Patch_SubtractQualityButton
        {
            [HarmonyPostfix]
            private static void Postfix(XUiC_CraftingInfoWindow __instance)
            {
                InvalidateViewModel(__instance);
            }
        }

        [HarmonyPatch(typeof(XUiC_CraftingInfoWindow), "OnClose")]
        private static class Patch_OnClose
        {
            [HarmonyPostfix]
            private static void Postfix(XUiC_CraftingInfoWindow __instance)
            {
                SetCompareMode(__instance, false);
            }
        }

        [HarmonyPatch(typeof(XUiC_CraftingInfoWindow), "GetBindingValueInternal")]
        private static class Patch_GetBindingValueInternal
        {
            [HarmonyPostfix]
            private static void Postfix(
                XUiC_CraftingInfoWindow __instance,
                ref string value,
                string bindingName,
                ref bool __result)
            {
                try
                {
                    CompareWindowState state = GetState(__instance);
                    bool compareBinding = bindingName.StartsWith("compare", StringComparison.Ordinal);
                    bool vanillaPanelBinding = bindingName == "showingredients"
                        || bindingName == "showdescription"
                        || bindingName == "showunlockedby";
                    if (!compareBinding && !(state.CompareMode && vanillaPanelBinding))
                    {
                        return;
                    }

                    CompareViewModel model = GetCachedViewModel(__instance, state);
                    if (!model.Available && state.CompareMode)
                    {
                        SetCompareMode(__instance, false);
                    }

                    if (state.CompareMode &&
                        (bindingName == "showingredients" || bindingName == "showdescription" || bindingName == "showunlockedby"))
                    {
                        value = "false";
                        __result = true;
                        return;
                    }

                    string compareValue;
                    if (TryGetCompareBinding(state, model, bindingName, out compareValue))
                    {
                        value = compareValue;
                        __result = true;
                    }
                }
                catch (Exception exception)
                {
                    if (!errorLogged)
                    {
                        errorLogged = true;
                        Log.Warning("[Compare] Binding error: " + exception);
                    }

                    // Sicherheitsnetz gegen Ueberlagerung: Wird das Binding nicht aufgeloest,
                    // faellt XUi auf den literalen Ausdruck zurueck. "{compareshow}" ist als
                    // nicht-leerer String wahr -> das Compare-Panel legt sich sichtbar ueber die
                    // Vanilla-Zutatenliste. Compare-Sichtbarkeitsbindings deshalb in jedem Fall
                    // als aufgeloest melden. Vanilla macht es genauso: XUiC_DeathBar liefert fuer
                    // "visible" einen leeren Wert, wenn das Element unsichtbar sein soll.
                    if (bindingName == "compareshow"
                        || bindingName == "compareavailable"
                        || bindingName == "compareammorow")
                    {
                        value = "false";
                        __result = true;
                    }
                    else if (bindingName.StartsWith("compare", StringComparison.Ordinal))
                    {
                        value = string.Empty;
                        __result = true;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(XUiC_CraftingInfoWindow), "Update")]
        private static class Patch_Update
        {
            [HarmonyPrefix]
            private static void Prefix(XUiC_CraftingInfoWindow __instance)
            {
                if (__instance.IsDirty)
                {
                    GetState(__instance).CachedModel = null;
                }
            }
        }

        private static CompareViewModel GetCachedViewModel(
            XUiC_CraftingInfoWindow window,
            CompareWindowState state)
        {
            if (state.CachedModel == null)
            {
                state.CachedModel = BuildViewModel(window);
            }
            return state.CachedModel;
        }

        private static bool TryGetCompareBinding(
            CompareWindowState state,
            CompareViewModel model,
            string bindingName,
            out string value)
        {
            value = string.Empty;

            switch (bindingName)
            {
                case "compareavailable":
                    value = model.Available ? "true" : "false";
                    return true;
                case "compareshow":
                    value = state.CompareMode && model.Available ? "true" : "false";
                    return true;
                case "compareoldname":
                    value = model.OldName;
                    return true;
                case "comparenewname":
                    value = model.NewName;
                    return true;
                case "compareammorow":
                    value = model.HasAmmoRow ? "true" : "false";
                    return true;
                case "compareoldammo":
                    value = model.OldAmmo;
                    return true;
                case "comparenewammo":
                    value = model.NewAmmo;
                    return true;
                case "compareammowarning":
                    value = model.AmmoWarning;
                    return true;
            }

            int index;
            if (TryGetIndexedBinding(bindingName, "comparestattitle", out index))
            {
                value = model.Titles[index];
                return true;
            }
            if (TryGetIndexedBinding(bindingName, "compareoldstat", out index))
            {
                value = model.OldValues[index];
                return true;
            }
            if (TryGetIndexedBinding(bindingName, "comparenewstat", out index))
            {
                value = model.NewValues[index];
                return true;
            }

            return false;
        }

        private static bool TryGetIndexedBinding(string bindingName, string prefix, out int index)
        {
            index = -1;
            if (!bindingName.StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }

            int uiIndex;
            if (!int.TryParse(bindingName.Substring(prefix.Length), out uiIndex))
            {
                return false;
            }

            index = uiIndex - 1;
            return index >= 0 && index < 7;
        }

        private static CompareViewModel BuildViewModel(XUiC_CraftingInfoWindow window)
        {
            CompareViewModel model = new CompareViewModel();
            if (window == null || window.recipe == null || window.xui == null || window.xui.playerUI == null)
            {
                return model;
            }

            EntityPlayerLocal player = window.xui.playerUI.entityPlayer;
            if (player == null)
            {
                return model;
            }

            int quality = window.SelectedCraftingTier;
            if (quality < 1)
            {
                quality = 1;
            }
            if (quality > 6)
            {
                quality = 6;
            }

            ItemValue previewValue = new ItemValue(window.recipe.itemValueType, quality, quality, false, null, 1f);
            ItemClass previewClass = previewValue.ItemClass;
            if (previewClass == null || string.IsNullOrEmpty(previewClass.DisplayType) || !UIDisplayInfoManager.HasInstance)
            {
                return model;
            }

            ItemDisplayEntry previewDisplay = UIDisplayInfoManager.Current.GetDisplayStatsForTag(previewClass.DisplayType);
            if (previewDisplay == null || !CompareLogic.IsSupportedDisplayGroup(previewDisplay.DisplayGroup))
            {
                return model;
            }

            model.Available = true;
            model.NewName = ShortName(previewClass.GetLocalizedItemName(), quality);

            ItemValue oldValue = player.inventory == null ? null : player.inventory.holdingItemItemValue;
            ItemDisplayEntry oldDisplay = GetDisplayEntry(oldValue);
            bool hasCompatibleOld = oldValue != null
                && !oldValue.IsEmpty()
                && oldDisplay != null
                && CompareLogic.AreDisplayGroupsCompatible(previewDisplay.DisplayGroup, oldDisplay.DisplayGroup);

            if (hasCompatibleOld)
            {
                model.OldName = ShortName(oldValue.ItemClass.GetLocalizedItemName(), oldValue.Quality);
            }

            bool ammoMismatch = false;
            ConfigureAmmunition(oldValue, previewValue, hasCompatibleOld, model, out ammoMismatch);

            int rowCount = previewDisplay.DisplayStats == null ? 0 : previewDisplay.DisplayStats.Count;
            if (rowCount > 7)
            {
                rowCount = 7;
            }

            for (int i = 0; i < rowCount; i++)
            {
                DisplayInfoEntry entry = previewDisplay.DisplayStats[i];
                model.Titles[i] = GetStatTitle(entry);
                model.NewValues[i] = FormatNewValue(previewValue, oldValue, player, entry, hasCompatibleOld, ammoMismatch);
                model.OldValues[i] = hasCompatibleOld ? FormatOldValue(oldValue, player, entry, ammoMismatch) : "—";
            }

            return model;
        }

        private static ItemDisplayEntry GetDisplayEntry(ItemValue value)
        {
            if (value == null || value.IsEmpty() || value.ItemClass == null ||
                string.IsNullOrEmpty(value.ItemClass.DisplayType) || !UIDisplayInfoManager.HasInstance)
            {
                return null;
            }

            return UIDisplayInfoManager.Current.GetDisplayStatsForTag(value.ItemClass.DisplayType);
        }

        private static void ConfigureAmmunition(
            ItemValue oldValue,
            ItemValue previewValue,
            bool hasCompatibleOld,
            CompareViewModel model,
            out bool ammoMismatch)
        {
            ammoMismatch = false;
            ItemActionAttack previewAction = GetPrimaryAttack(previewValue);
            string[] previewAmmoNames = previewAction == null ? null : previewAction.MagazineItemNames;
            string previewAmmoName = CompareLogic.SafeAmmoName(previewAmmoNames, previewValue.SelectedAmmoTypeIndex);

            if (!hasCompatibleOld)
            {
                model.NewAmmo = GetLocalizedAmmoName(previewAmmoName);
                return;
            }

            ItemActionAttack oldAction = GetPrimaryAttack(oldValue);
            string[] oldAmmoNames = oldAction == null ? null : oldAction.MagazineItemNames;
            bool oldUsesAmmo = oldAmmoNames != null && oldAmmoNames.Length > 0;
            bool oldAmmoIndexValid = CompareLogic.IsValidAmmoIndex(oldAmmoNames, oldValue.SelectedAmmoTypeIndex);
            string oldAmmoName = CompareLogic.SafeAmmoName(oldAmmoNames, oldValue.SelectedAmmoTypeIndex);
            model.OldAmmo = GetLocalizedAmmoName(oldAmmoName);

            if (oldUsesAmmo && !oldAmmoIndexValid)
            {
                ammoMismatch = true;
                model.AmmoWarning = "Munitionsauswahl ungültig – markierte Werte nicht direkt vergleichbar";
                model.NewAmmo = GetLocalizedAmmoName(previewAmmoName);
                return;
            }

            if (string.IsNullOrEmpty(oldAmmoName) || previewAmmoNames == null || previewAmmoNames.Length == 0)
            {
                model.NewAmmo = GetLocalizedAmmoName(previewAmmoName);
                return;
            }

            int matchingIndex = CompareLogic.FindMatchingAmmoIndex(oldAmmoName, previewAmmoNames);
            if (matchingIndex >= 0)
            {
                previewValue.SelectedAmmoTypeIndex = (byte)matchingIndex;
                previewAmmoName = previewAmmoNames[matchingIndex];
            }
            else
            {
                ammoMismatch = true;
                model.AmmoWarning = "Unterschiedliche Munition – markierte Werte nicht direkt vergleichbar";
            }

            model.NewAmmo = GetLocalizedAmmoName(previewAmmoName);
        }

        private static ItemActionAttack GetPrimaryAttack(ItemValue value)
        {
            if (value == null || value.ItemClass == null || value.ItemClass.Actions == null ||
                value.ItemClass.Actions.Length == 0)
            {
                return null;
            }

            return value.ItemClass.Actions[0] as ItemActionAttack;
        }

        private static string GetLocalizedAmmoName(string ammoName)
        {
            if (string.IsNullOrEmpty(ammoName))
            {
                return string.Empty;
            }

            ItemClass ammoClass = ItemClass.GetItemClass(ammoName, false);
            return ammoClass == null ? ammoName : ammoClass.GetLocalizedItemName();
        }

        private static string GetStatTitle(DisplayInfoEntry entry)
        {
            if (entry == null)
            {
                return string.Empty;
            }

            return !string.IsNullOrEmpty(entry.TitleOverride)
                ? entry.TitleOverride
                : UIDisplayInfoManager.Current.GetLocalizedName(entry.StatType);
        }

        private static string FormatOldValue(
            ItemValue oldValue,
            EntityPlayerLocal player,
            DisplayInfoEntry entry,
            bool ammoMismatch)
        {
            string text = StripMagnitudePercent(
                XUiM_ItemStack.GetStatItemValueTextWithModColoring(oldValue, player, entry));
            if (ammoMismatch && CompareLogic.IsAmmoDependentStat(entry.StatType.ToString()))
            {
                return text + " *";
            }
            return text;
        }

        private static string FormatNewValue(
            ItemValue previewValue,
            ItemValue oldValue,
            EntityPlayerLocal player,
            DisplayInfoEntry entry,
            bool hasCompatibleOld,
            bool ammoMismatch)
        {
            bool ammoDependentMismatch = ammoMismatch && CompareLogic.IsAmmoDependentStat(entry.StatType.ToString());
            string text = StripMagnitudePercent(
                XUiM_ItemStack.GetStatItemValueTextWithModColoring(previewValue, player, entry));
            if (!hasCompatibleOld || ammoDependentMismatch)
            {
                return ammoDependentMismatch ? text + " *" : text;
            }

            float oldEffectiveValue = GetEffectiveStatValue(oldValue, player, entry, true);
            float newEffectiveValue = GetEffectiveStatValue(previewValue, player, entry, true);
            oldEffectiveValue = NormalizeEffectiveValueForDisplay(oldEffectiveValue, entry);
            newEffectiveValue = NormalizeEffectiveValueForDisplay(newEffectiveValue, entry);
            bool lowerIsBetter = entry.NegativePreferred || ItemValue.IsStatLowerBetter(entry.StatType);
            float changePercent;
            if (entry.DisplayType != DisplayInfoEntry.DisplayTypes.Bool &&
                CompareLogic.TryCalculateChangePercent(
                    oldEffectiveValue,
                    newEffectiveValue,
                    out changePercent))
            {
                text += CompareLogic.FormatChangePercent(changePercent, lowerIsBetter);
            }
            return text;
        }

        private static float GetEffectiveStatValue(
            ItemValue itemValue,
            EntityPlayerLocal player,
            DisplayInfoEntry entry,
            bool useMods)
        {
            FastTags<TagGroup.Global> tags = entry.TagsSet
                ? entry.Tags
                : XUiM_ItemStack.primaryFastTags | XUiM_ItemStack.physicalDamageFastTags;

            float result;
            if (string.IsNullOrEmpty(entry.CustomName))
            {
                MinEventParams.CachedEventParam.ItemValue = itemValue;
                MinEventParams.CachedEventParam.Seed = itemValue.Seed;
                result = EffectManager.GetValue(
                    entry.StatType,
                    itemValue,
                    0f,
                    player,
                    null,
                    tags,
                    false,
                    false,
                    false,
                    false,
                    true,
                    1,
                    useMods,
                    false);
            }
            else
            {
                result = XUiM_ItemStack.GetCustomValue(entry, itemValue, useMods);
            }

            XUiM_ItemStack.degradationMaxMod(
                entry.StatType,
                itemValue,
                player,
                tags,
                useMods,
                ref result);
            return result;
        }

        private static float NormalizeEffectiveValueForDisplay(float value, DisplayInfoEntry entry)
        {
            CompareDisplayPrecision precision;
            switch (entry.DisplayType)
            {
                case DisplayInfoEntry.DisplayTypes.Integer:
                    precision = CompareDisplayPrecision.Integer;
                    break;
                case DisplayInfoEntry.DisplayTypes.Decimal1:
                    precision = CompareDisplayPrecision.Decimal1;
                    break;
                case DisplayInfoEntry.DisplayTypes.Decimal2:
                    precision = CompareDisplayPrecision.Decimal2;
                    break;
                case DisplayInfoEntry.DisplayTypes.Percent:
                    precision = CompareDisplayPrecision.Percent;
                    break;
                default:
                    precision = CompareDisplayPrecision.Raw;
                    break;
            }

            float normalized = CompareLogic.NormalizeDisplayedValue(value, precision);
            if (entry.ShowInverted)
            {
                normalized -= precision == CompareDisplayPrecision.Percent ? 100f : 1f;
            }
            return normalized;
        }

        private static string StripMagnitudePercent(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int suffixIndex = text.IndexOf(" [00f0f0](", StringComparison.OrdinalIgnoreCase);
            return suffixIndex < 0 ? text : text.Substring(0, suffixIndex);
        }

        private static string ShortName(string localizedName, int quality)
        {
            string name = string.IsNullOrEmpty(localizedName) ? "—" : localizedName;
            if (name.Length > 16)
            {
                name = name.Substring(0, 15) + "…";
            }
            return name + " Q" + quality;
        }
    }
}

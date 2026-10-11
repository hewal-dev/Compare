using System;
using System.Globalization;

namespace Compare
{
    public enum CompareDisplayPrecision
    {
        Raw,
        Integer,
        Decimal1,
        Decimal2,
        Percent
    }

    public static class CompareLogic
    {
        public static int FindMatchingAmmoIndex(string ammoName, string[] candidateAmmoNames)
        {
            if (string.IsNullOrEmpty(ammoName) || candidateAmmoNames == null)
            {
                return -1;
            }

            for (int i = 0; i < candidateAmmoNames.Length; i++)
            {
                if (string.Equals(ammoName, candidateAmmoNames[i], StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
        }

        public static string SafeAmmoName(string[] ammoNames, int selectedIndex)
        {
            if (!IsValidAmmoIndex(ammoNames, selectedIndex))
            {
                return string.Empty;
            }

            return ammoNames[selectedIndex] ?? string.Empty;
        }

        public static bool IsValidAmmoIndex(string[] ammoNames, int selectedIndex)
        {
            return ammoNames != null && selectedIndex >= 0 && selectedIndex < ammoNames.Length;
        }

        public static bool IsAmmoDependentStat(string statName)
        {
            switch (statName)
            {
                case "EntityDamage":
                case "BlockDamage":
                case "RoundRayCount":
                case "DamageFalloffRange":
                case "ProjectileVelocity":
                    return true;
                default:
                    return false;
            }
        }

        public static bool AreDisplayGroupsCompatible(string firstGroup, string secondGroup)
        {
            return !string.IsNullOrEmpty(firstGroup)
                && !string.IsNullOrEmpty(secondGroup)
                && string.Equals(firstGroup, secondGroup, StringComparison.Ordinal);
        }

        public static bool IsSupportedDisplayGroup(string displayGroup)
        {
            return string.Equals(displayGroup, "groupMelee", StringComparison.Ordinal)
                || string.Equals(displayGroup, "groupRanged", StringComparison.Ordinal);
        }

        public static bool TryCalculateChangePercent(
            float equippedValue,
            float craftableValue,
            out float percent)
        {
            percent = 0f;
            if (Math.Abs(equippedValue) < 0.0001f)
            {
                return false;
            }

            percent = (craftableValue - equippedValue) / Math.Abs(equippedValue) * 100f;
            return true;
        }

        public static string FormatChangePercent(float percent, bool lowerIsBetter)
        {
            if (Math.Abs(percent) < 0.05f)
            {
                return " [FFFFFF](0%)[-]";
            }

            string formatted = Math.Abs(percent).ToString("0.#", CultureInfo.InvariantCulture);
            bool improvement = lowerIsBetter ? percent < 0f : percent > 0f;
            string color = improvement ? "00FF00" : "FF0000";
            string sign = percent > 0f ? "+" : "-";
            return " [" + color + "](" + sign + formatted + "%)[-]";
        }

        public static float NormalizeDisplayedValue(float value, CompareDisplayPrecision precision)
        {
            switch (precision)
            {
                case CompareDisplayPrecision.Integer:
                    return (float)Math.Floor(value);
                case CompareDisplayPrecision.Decimal1:
                    return (float)Math.Floor(value * 10f) / 10f;
                case CompareDisplayPrecision.Decimal2:
                    return (float)Math.Floor(value * 100f) / 100f;
                case CompareDisplayPrecision.Percent:
                    return (float)Math.Floor(value * 100f);
                default:
                    return value;
            }
        }
    }
}

using System;
using Compare;

public static class CompareLogicTests
{
    private static int failures;

    public static int Main()
    {
        TestFindMatchingAmmoIndexUsesExactRuntimeName();
        TestFindMatchingAmmoIndexRejectsMissingAmmo();
        TestSafeAmmoNameHandlesInvalidIndex();
        TestAmmoDependentStatsAreRecognized();
        TestDisplayGroupsMustMatchForDirectComparison();
        TestOnlyWeaponAndToolDisplayGroupsAreSupported();
        TestAmmoIndexValidation();
        TestImprovementPercentageUsesEquippedItemAsBaseline();
        TestLowerValuesCanBeBetter();
        TestPercentageUsesDisplayedRoundedValues();

        if (failures == 0)
        {
            Console.WriteLine("All CompareLogic tests passed.");
            return 0;
        }

        Console.Error.WriteLine(failures + " CompareLogic test(s) failed.");
        return 1;
    }

    private static void TestFindMatchingAmmoIndexUsesExactRuntimeName()
    {
        string[] previewAmmo = { "ammo44MagnumBulletBall", "ammo44MagnumBulletSilver" };
        AssertEqual(1, CompareLogic.FindMatchingAmmoIndex("ammo44MagnumBulletSilver", previewAmmo),
            "matching custom ammunition by runtime item name");
    }

    private static void TestFindMatchingAmmoIndexRejectsMissingAmmo()
    {
        string[] previewAmmo = { "ammo762mmBulletBall", "ammo762mmBulletAP" };
        AssertEqual(-1, CompareLogic.FindMatchingAmmoIndex("ammo44MagnumBulletSilver", previewAmmo),
            "rejecting ammunition unsupported by the preview weapon");
    }

    private static void TestSafeAmmoNameHandlesInvalidIndex()
    {
        string[] ammo = { "ammo22ShortBulletBall" };
        AssertEqual(string.Empty, CompareLogic.SafeAmmoName(ammo, 5),
            "invalid selected ammunition index");
    }

    private static void TestAmmoDependentStatsAreRecognized()
    {
        AssertEqual(true, CompareLogic.IsAmmoDependentStat("EntityDamage"),
            "EntityDamage ammunition dependency");
        AssertEqual(true, CompareLogic.IsAmmoDependentStat("RoundRayCount"),
            "RoundRayCount ammunition dependency");
        AssertEqual(false, CompareLogic.IsAmmoDependentStat("MagazineSize"),
            "MagazineSize ammunition independence");
    }

    private static void TestDisplayGroupsMustMatchForDirectComparison()
    {
        AssertEqual(true, CompareLogic.AreDisplayGroupsCompatible("groupRanged", "groupRanged"),
            "matching ranged display groups");
        AssertEqual(false, CompareLogic.AreDisplayGroupsCompatible("groupMelee", "groupRanged"),
            "rejecting different display groups");
        AssertEqual(false, CompareLogic.AreDisplayGroupsCompatible(string.Empty, "groupRanged"),
            "rejecting missing display group metadata");
    }

    private static void TestOnlyWeaponAndToolDisplayGroupsAreSupported()
    {
        AssertEqual(true, CompareLogic.IsSupportedDisplayGroup("groupMelee"),
            "melee and tool display group");
        AssertEqual(true, CompareLogic.IsSupportedDisplayGroup("groupRanged"),
            "ranged weapon display group");
        AssertEqual(false, CompareLogic.IsSupportedDisplayGroup("groupConsumables"),
            "consumables are outside Compare scope");
    }

    private static void TestAmmoIndexValidation()
    {
        string[] ammo = { "ammo22ShortBulletBall" };
        AssertEqual(true, CompareLogic.IsValidAmmoIndex(ammo, 0),
            "valid selected ammunition index");
        AssertEqual(false, CompareLogic.IsValidAmmoIndex(ammo, 2),
            "invalid selected ammunition index");
        AssertEqual(false, CompareLogic.IsValidAmmoIndex(null, 0),
            "missing ammunition list");
    }

    private static void TestImprovementPercentageUsesEquippedItemAsBaseline()
    {
        float percent;
        AssertEqual(true, CompareLogic.TryCalculateChangePercent(100f, 120f, out percent),
            "calculating positive change percentage");
        AssertNearly(20f, percent, "craftable item is twenty percent better");
        AssertEqual(" [00FF00](+20%)[-]", CompareLogic.FormatChangePercent(percent, false),
            "green positive crafting improvement");

        AssertEqual(true, CompareLogic.TryCalculateChangePercent(100f, 80f, out percent),
            "calculating negative change percentage");
        AssertNearly(-20f, percent, "craftable item is twenty percent worse");
        AssertEqual(" [FF0000](-20%)[-]", CompareLogic.FormatChangePercent(percent, false),
            "red negative crafting improvement");
    }

    private static void TestLowerValuesCanBeBetter()
    {
        float percent;
        AssertEqual(true, CompareLogic.TryCalculateChangePercent(100f, 80f, out percent),
            "calculating lower-is-better percentage");
        AssertNearly(-20f, percent, "twenty percent less stamina cost remains numerically negative");
        AssertEqual(" [00FF00](-20%)[-]", CompareLogic.FormatChangePercent(percent, true),
            "negative lower-is-better change is green");

        AssertEqual(true, CompareLogic.TryCalculateChangePercent(100f, 130f, out percent),
            "calculating higher stamina cost");
        AssertNearly(30f, percent, "thirty percent more stamina cost remains numerically positive");
        AssertEqual(" [FF0000](+30%)[-]", CompareLogic.FormatChangePercent(percent, true),
            "positive lower-is-better change is red");

        AssertEqual(false, CompareLogic.TryCalculateChangePercent(0f, 10f, out percent),
            "zero baseline cannot produce a meaningful percentage");
    }

    private static void TestPercentageUsesDisplayedRoundedValues()
    {
        float equipped = CompareLogic.NormalizeDisplayedValue(13.97f, CompareDisplayPrecision.Integer);
        float craftable = CompareLogic.NormalizeDisplayedValue(13f, CompareDisplayPrecision.Integer);
        float percent;
        AssertNearly(13f, equipped, "equipped magazine display value");
        AssertNearly(13f, craftable, "craftable magazine display value");
        AssertEqual(true, CompareLogic.TryCalculateChangePercent(equipped, craftable, out percent),
            "percentage from displayed magazine values");
        AssertNearly(0f, percent, "equal displayed magazine sizes are zero percent apart");

        AssertNearly(150.5f,
            CompareLogic.NormalizeDisplayedValue(150.59f, CompareDisplayPrecision.Decimal1),
            "one-decimal Vanilla floor formatting");
        AssertNearly(12.34f,
            CompareLogic.NormalizeDisplayedValue(12.349f, CompareDisplayPrecision.Decimal2),
            "two-decimal Vanilla floor formatting");
    }

    private static void AssertEqual<T>(T expected, T actual, string scenario)
    {
        if (!object.Equals(expected, actual))
        {
            failures++;
            Console.Error.WriteLine("FAIL " + scenario + ": expected=" + expected + " actual=" + actual);
        }
    }

    private static void AssertNearly(float expected, float actual, string scenario)
    {
        if (Math.Abs(expected - actual) > 0.001f)
        {
            failures++;
            Console.Error.WriteLine("FAIL " + scenario + ": expected=" + expected + " actual=" + actual);
        }
    }
}

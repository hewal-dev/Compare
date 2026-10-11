using HarmonyLib;
using System.Reflection;

namespace Compare
{
    public class CompareMod : IModApi
    {
        public const string HarmonyId = "com.elternvszombies.compare";

        public void InitMod(Mod modInstance)
        {
            Log.Out("[Compare] Initializing Compare 1.0.5");
            Harmony harmony = new Harmony(HarmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Out("[Compare] Harmony patches registered");
        }
    }
}

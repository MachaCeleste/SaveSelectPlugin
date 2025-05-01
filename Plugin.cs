using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace SaveSelectPlugin;

[BepInPlugin("com.machaceleste.saveselectplugin", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        DataUtils.LoadBundle("asset.bundle");
        var harmony = new Harmony("com.machaceleste.saveselectplugin");
        harmony.PatchAll();
    }
}
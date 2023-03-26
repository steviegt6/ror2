using BepInEx;

namespace Void.TestMod; 

[BepInPlugin(GUID, NAME, VERSION)]
public class TestPlugin : BaseUnityPlugin {
    public const string GUID = "Tomat.Void.TestMod";
    public const string NAME = "TestMod";
    public const string VERSION = "1.0.0";

    private void Awake() {
        Logger.LogInfo("Hello, world!");
    }
}

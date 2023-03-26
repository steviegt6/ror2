using BepInEx;
using Tomlyn;

namespace Void.TestMod;

[BepInPlugin(GUID, NAME, VERSION)]
public class TestPlugin : BaseUnityPlugin {
    public const string GUID = "Tomat.Void.TestMod";
    public const string NAME = "TestMod";
    public const string VERSION = "1.0.0";

    private void Awake() {
        Logger.LogInfo("Hello, world!");

        const string toml = @"
test = ""test""
[table]
table_test = ""table_test""
";
        var model = Toml.ToModel(toml);
        Logger.LogError(Toml.FromModel(model));
    }
}

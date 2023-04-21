/* Copyright (C) 2023 Tomat & Contributors
 * 
 * Licensed under the GNU Lesser General Public License, version 2.1; you may
 * not use this file except in compliance with the License.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

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

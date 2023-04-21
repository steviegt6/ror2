﻿/* Copyright (C) 2023 Tomat & Contributors
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

using Void.Build.NuGet.Shared;

namespace Void.Build.NuGet.AssemblyPublicizer.Tasks;

public sealed class AssemblyPublicizerTask : VoidTask {
    protected override bool Execute(VoidContext ctx) {
        return true;
    }
}

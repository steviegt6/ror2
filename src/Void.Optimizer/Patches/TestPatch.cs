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

using Mono.Cecil;
using Void.Optimizer.Core;
using Void.Optimizer.Core.Visitors;

namespace Void.Optimizer.Patches;

/* Applies only to the assembly Rewired_Core:
 * [AssemblyWhitelist(AllowanceType.Whitelist, "Rewired_Core")]
 *
 * Applies only to the assemblies Rewired_Core and Rewired_CSharp:
 * [AssemblyWhitelist(
 *     AllowanceType.Whitelist,
 *     "Rewired_Core",
 *     "Rewired_CSharp",
 * )]
 *
 * Applies to all assemblies except the assembly Rewired_Core:
 * [AssemblyWhitelist(AllowanceType.Blacklist, "Rewired_Core")]
 *
 * Applies to all assemblies except the assemblies Rewired_Core and
 * Rewired_CSharp:
 * [AssemblyWhitelist(
 *     AllowanceType.Blacklist,
 *    "Rewired_Core",
 *    "Rewired_CSharp",
 * )]
 *
 * Applies to all assemblies:
 * [AssemblyWhitelist(AllowanceType.None)]
 *
 * Applies only to the type Rewired.InputManager_Base:
 * [TypeWhitelist(AllowanceType.Whitelist, "Rewired.InputManager_Base")]
 *
 * Applies only to the types Rewired.InputManager_Base and Rewired.InputManager:
 * [TypeWhitelist(
 *     AllowanceType.Whitelist,
 *     "Rewired.InputManager_Base",
 *     "Rewired.InputManager",
 * )]
 *
 * Applies to all types except the type Rewired.InputManager_Base:
 * [TypeWhitelist(AllowanceType.Blacklist, "Rewired.InputManager_Base")]
 *
 * Applies to all types except the types Rewired.InputManager_Base and
 * Rewired.InputManager:
 * [TypeWhitelist(
 *     AllowanceType.Blacklist,
 *     "Rewired.InputManager_Base",
 *     "Rewired.InputManager",
 * )]
 *
 * Applies to all types:
 * [TypeWhitelist(AllowanceType.None)]
 */
public sealed class TestPatch : ITypeVisitor,
                                IMemberVisitor,
                                IAllowanceProvider {
    public void Visit(ref TypeDefinition typeDefinition) {
        throw new System.NotImplementedException();
    }

    public void VisitNested(ref TypeDefinition nestedTypeDefinition) {
        throw new System.NotImplementedException();
    }

    public void Visit(ref MethodDefinition methodDefinition) {
        throw new System.NotImplementedException();
    }

    public void Visit(ref FieldDefinition fieldDefinition) {
        throw new System.NotImplementedException();
    }

    public void Visit(ref PropertyDefinition propertyDefinition) {
        throw new System.NotImplementedException();
    }

    public void Visit(ref EventDefinition eventDefinition) {
        throw new System.NotImplementedException();
    }

    public bool IsAssemblyAllowed(
        AssemblyDefinition assemblyDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist
    ) {
        return false;
    }

    public bool IsTypeAllowed(
        TypeDefinition typeDefinition,
        Whitelist assemblyWhitelist,
        Whitelist typeWhitelist
    ) {
        return false;
    }
}

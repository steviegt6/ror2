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

using System.IO;
using System.Security.Cryptography;

namespace Void.Build.NuGet.Shared.Util;

/// <summary>
///     A class that provides methods for hashing files.
/// </summary>
public static class FileHasher {
    private static readonly SHA256 sha256 = SHA256.Create();

    /// <summary>
    ///     Gets the SHA256 hash of the file at the given path.
    /// </summary>
    /// <param name="path">The path to the file to hash.</param>
    /// <returns>The SHA256 hash of the file at the given path.</returns>
    public static string GetSha256Hash(string path) {
        using var stream = File.OpenRead(path);
        return GetSha256Hash(stream);
    }

    /// <summary>
    ///     Gets the SHA256 hash of the given bytes.
    /// </summary>
    /// <param name="bytes">The bytes to hash.</param>
    /// <returns>The SHA256 hash of the given bytes.</returns>
    public static string GetSha256Hash(byte[] bytes) {
        var hash = sha256.ComputeHash(bytes);
        return ToHexString(hash);
    }

    /// <summary>
    ///     Gets the SHA256 hash of the given stream.
    /// </summary>
    /// <param name="stream">The stream to hash.</param>
    /// <returns>The SHA256 hash of the given stream.</returns>
    public static string GetSha256Hash(Stream stream) {
        var hash = sha256.ComputeHash(stream);
        return ToHexString(hash);
    }

    private static string ToHexString(byte[] bytes) {
        var hex = new char[bytes.Length * 2];

        for (var i = 0; i < bytes.Length; i++) {
            var b = bytes[i];
            hex[i * 2] = ToHexChar(b >> 4);
            hex[i * 2 + 1] = ToHexChar(b & 0xF);
        }

        return new string(hex);
    }

    private static char ToHexChar(int i) {
        return (char)(i < 10 ? i + '0' : i - 10 + 'a');
    }
}

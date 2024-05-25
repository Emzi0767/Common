// This file is part of Emzi0767.Common project.
//
// Copyright © 2020-2024 Emzi0767
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Globalization;

namespace Emzi0767.Serialization;

/// <summary>
/// Defines the format for string-serialized <see cref="TimeSpan"/> objects.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class TimeSpanFormatAttribute : SerializationAttribute
{
    /// <summary>
    /// Gets the ISO 8601 format string of @"ddThh\:mm\:ss\.fff".
    /// </summary>
    public const string FormatISO8601 = @"ddThh\:mm\:ss\.fff";

    /// <summary>
    /// Gets the constant format.
    /// </summary>
    public const string FormatConstant = "c";

    /// <summary>
    /// Gets the general long format.
    /// </summary>
    public const string FormatLong = "G";

    /// <summary>
    /// Gets the general short format.
    /// </summary>
    public const string FormatShort = "g";

    /// <summary>
    /// Gets the custom datetime format string to use.
    /// </summary>
    public string Format { get; }

    /// <summary>
    /// Gets the predefined datetime format kind.
    /// </summary>
    public TimeSpanFormatKind Kind { get; }

    /// <summary>
    /// Specifies a predefined format to use.
    /// </summary>
    /// <param name="kind">Predefined format kind to use.</param>
    public TimeSpanFormatAttribute(TimeSpanFormatKind kind)
    {
        if (kind < 0 || kind > TimeSpanFormatKind.InvariantLocaleShort)
            throw new ArgumentOutOfRangeException(nameof(kind), "Specified format kind is not legal or supported.");

        this.Kind = kind;
        this.Format = null;
    }

    /// <summary>
    /// <para>Specifies a custom format to use.</para>
    /// <para>See https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings for more details.</para>
    /// </summary>
    /// <param name="format">Custom format string to use.</param>
    public TimeSpanFormatAttribute(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
            throw new ArgumentNullException(nameof(format), "Specified format cannot be null or empty.");

        this.Kind = TimeSpanFormatKind.Custom;
        this.Format = format;
    }
}

/// <summary>
/// <para>Defines which built-in format to use for <see cref="TimeSpan"/> serialization.</para>
/// <para>See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings and https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings for more details.</para>
/// </summary>
public enum TimeSpanFormatKind : int
{
    /// <summary>
    /// Specifies ISO 8601-like time format, which is equivalent to .NET format string of @"ddThh\:mm\:ss\.fff".
    /// </summary>
    ISO8601 = 0,

    /// <summary>
    /// Specifies a format defined by <see cref="CultureInfo.InvariantCulture"/>, with a format string of "c".
    /// </summary>
    InvariantConstant = 1,

    /// <summary>
    /// Specifies a format defined by <see cref="CultureInfo.CurrentCulture"/>, with a format string of "G". This format is not recommended for portability reasons.
    /// </summary>
    CurrentLocaleLong = 2,

    /// <summary>
    /// Specifies a format defined by <see cref="CultureInfo.CurrentCulture"/>, with a format string of "g". This format is not recommended for portability reasons.
    /// </summary>
    CurrentLocaleShort = 3,

    /// <summary>
    /// Specifies a format defined by <see cref="CultureInfo.InvariantCulture"/>, with a format string of "G". This format is not recommended for portability reasons.
    /// </summary>
    InvariantLocaleLong = 4,

    /// <summary>
    /// Specifies a format defined by <see cref="CultureInfo.InvariantCulture"/>, with a format string of "g". This format is not recommended for portability reasons.
    /// </summary>
    InvariantLocaleShort = 5,

    /// <summary>
    /// Specifies a custom format. This value is not usable directly.
    /// </summary>
    Custom = int.MaxValue
}

﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Flags used to specify a set of one or more 3D axes.
    /// </summary>
    [System.Flags]
    public enum AxisFlags
    {
        /// <summary>
        /// No flags have been set.
        /// </summary>
        None = 0,

        /// <summary>
        /// The horizontal axis.
        /// </summary>
        XAxis = 1 << 0,

        /// <summary>
        /// The vertical axis.
        /// </summary>
        YAxis = 1 << 1,

        /// <summary>
        /// The depth axis.
        /// </summary>
        ZAxis = 1 << 2
    }

    /// <summary>
    /// Extension methods specific to the <see cref="AxisFlags"/> enumeration.
    /// </summary>
    public static class AxisFlagsExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="AxisFlags"/> value.</param>
        /// <param name="b"><see cref="AxisFlags"/> mask.</param>
        /// <returns><see langword="true"/> if all of the bits in the specified mask are set in the
        /// current value, otherwise <see langword="false"/>.</returns>
        public static bool IsMaskSet(this AxisFlags a, AxisFlags b)
        {
            return ((a & b) == b);
        }
    }
}

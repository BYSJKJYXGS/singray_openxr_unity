﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Enum specifying whether an object should be rotated
    /// around its origin, or around the center of the calculated bounds.
    /// </summary>
    public enum RotateAnchorType
    {
        /// <summary>
        /// Rotate around the object's origin.
        /// </summary>
        ObjectOrigin = 0,

        /// <summary>
        /// Rotate around the center of the calculated bounds.
        /// </summary>
        BoundsCenter
    }

    /// <summary>
    /// Enum specifying whether an object should be scaled
    /// around the opposite corner, or around the center of the calculated bounds.
    /// </summary>
    public enum ScaleAnchorType
    {
        /// <summary>
        /// Scale around the opposite bounds corner.
        /// </summary>
        OppositeCorner = 0,

        /// <summary>
        /// Scale around the bounds center point.
        /// </summary>
        BoundsCenter
    }

    /// <summary>
    /// An enumeration describing a type of handle on a <see cref="BoundsControl"/>. grabbed can be a rotation (edge-mounted)
    /// handle, a scaling (corner-mounted) handle, or a translation (face-mounted)
    /// handle.
    /// </summary>
    [Flags]
    public enum HandleType
    {
        /// <summary>
        /// No handles on the <see cref="BoundsControl"/>.
        /// </summary>
        None = 0,

        /// <summary>
        /// A handle that is mounted to the edge of a <see cref="BoundsControl"/>, and can rotate the object.
        /// </summary>
        Rotation = 1 << 0,
        
        /// <summary>
        /// A handle that is mounted to the corner of a <see cref="BoundsControl"/>, and can scale the object.
        /// </summary>
        Scale = 1 << 1,
        
        /// <summary>
        /// A handle that is mounted to the face of a <see cref="BoundsControl"/>, and can move the object.
        /// </summary>
        /// <remarks>
        /// Handles of this type are currently not supported.
        /// </remarks>
        Translation = 1 << 2,
    }

    /// <summary>
    /// Scale mode that is used for scaling behavior of bounds control.
    /// </summary>
    public enum HandleScaleMode
    {
        /// <summary>
        /// Control will be scaled uniformly.
        /// </summary>
        Uniform,

        /// <summary>
        /// Scales non uniformly according to movement in 3d space.
        /// </summary>
        NonUniform
    }

    /// <summary>
    /// Scale mode that is used for scaling behavior of bounds control.
    /// </summary>
    public enum FlattenMode
    {
        /// <summary>
        /// Regardless of how thin the bounds are, the BoundsControl will not flatten.
        /// </summary>
        Never,

        /// <summary>
        /// If the bounds is sufficiently thin, the BoundsControl will automatically flatten along
        /// the thinnest axis.
        /// </summary>
        Auto,

        /// <summary>
        /// Regardless of how thin or thick the bounds are,
        /// the BoundsControl will always flatten along the thinnest axis.
        /// </summary>
        Always,
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX.Experimental
{
    /// <summary>
    /// All theme elements of the MRTK <c>ObjectBar</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A MRTK <c>ObjectBar</c> component can be added to the an object via the MRTK
    /// menu items within Unity's add component menu.
    /// </para>
    /// <para>
    /// Note that by default the <see cref="UXBindingProfileTemplate"/> only maps
    /// to <c>UX.Common.xxx</c> instead of UX.ObjectBar.xxx for any
    /// element that is common across all UX elements.
    /// </para>
    /// <para>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven't fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </para>
    /// </remarks>
    [Serializable]
    public class ObjectBarTheme
    {

        [Tooltip("Backplate material for a object bar.")]
        [SerializeField]
        private Material backplateMaterial;

        /// <summary>
        /// Backplate material for a object bar.
        /// </summary>
        public Material BackplateMaterial => backplateMaterial;

    }
}

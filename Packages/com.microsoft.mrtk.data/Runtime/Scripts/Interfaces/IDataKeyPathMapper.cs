﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Interface for a key-path mapper that is used to translate between data source namespaces
    /// and view namespaces. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Any key path mapper can be dependency injected into any data source.
    /// This allows reusable view prefabs to be populated from any variety of data sources
    /// containing similar information.
    /// </para>
    /// <para>
    /// If no Key Path Mapper is assigned to a data source, then the view and data source key paths
    /// are assumed to be the same.
    /// </para>
    /// <para>
    /// TODO: Consider adding a namespace argument so that a single key mapper can map multiple
    ///       different views to the same data source without a chance of naming conflicts.
    ///       In this case, the combination of namespace and local (view) key path would be used
    ///       to look up the correct mapping.
    /// </para>
    /// </remarks>
    public interface IDataKeyPathMapper
    {
        /// <summary>
        /// Given a a view keyPath return the data keyPath.
        /// </summary>
        ///
        /// <remarks>
        /// To allow for standard templated views to map to different data sources without
        /// the need to modify for different data sources that use different naming conventions, this
        /// service provides a mapping from view keyPath to a data source keyPath.
        ///
        /// If a viewKeyPath is provided that's not mapped, it is returned unchanged.
        /// </remarks>
        ///
        /// <param name="viewKeyPath">The keyPath used in a data view.</param>
        ///
        /// <returns>A string that can be used to map to the data source.</returns>
        string GetDataKeyPathFromViewKeyPath(string viewKeyPath);

        /// <summary>
        /// Given a a data keyPath return the view keyPath.
        /// </summary>
        ///
        /// <remarks>
        /// Given a data keyPath, return the associated view keyPath.
        ///
        /// If a dataKeyPath is provided that's not mapped, it is returned unchanged.
        /// </remarks>
        ///
        /// <param name="dataKeyPath">The keyPath used in a data view.</param>
        ///
        /// <returns>A string that can be used to map to a view data consumer.</returns>
        string GetViewKeyPathFromDataKeyPath(string dataKeyPath);
    }
}
#pragma warning restore CS1591
﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Interface for all data consumers that manage collections.
    /// </summary>
    public interface IDataConsumerCollection : IDataConsumer
    {
        /// <summary>
        /// Request specific items from an item placer for immediate use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The individual items will be provided one at a time to <see cref="IDataCollectionItemPlacer.PlaceItem"/> method of the calling <paramref name="itemPlacer"/>. 
        /// This allows data fetching and data presenting to occur in a pseudo parallel fashion.
        /// </para>
        /// <para>
        /// This is used by an item placer to request only the subset of items in the collection that are currently relevant, usually those that are currently visible.
        /// </para>
        /// </remarks>
        /// <param name="itemPlacer">The Item Placer making this request.</param>
        /// <param name="rangeStart">The zero-based start index of the range to retrieve.</param>
        /// <param name="rangeCount">The number of items to retrieve. If end of collection is reached, fewer items may be provided.</param>
        /// <param name="requestRef">A request reference object that will be passed to the PlaceItem method.</param>
        void RequestCollectionItems(IDataCollectionItemPlacer itemPlacer, int rangeStart, int rangeCount, object requestRef);

        /// <summary>
        /// Request specific items from an item placer for future use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The individual items will be cached for potential future use.
        /// </para>
        /// <para>
        /// This is used by an Item Placer to predictively request a subset of items in the collection that may soon become visible.
        /// </para>
        /// </remarks>
        /// <param name="itemPlacer">The Item Placer making this request.</param>
        /// <param name="indexRangeStart">The zero-based start index of the range to retrieve.</param>
        /// <param name="indexRangeCount">The number of items to retrieve. If end of collection is reached, fewer items may be provided.</param>
        void PrefetchCollectionItems(IDataCollectionItemPlacer itemPlacer, int indexRangeStart, int indexRangeCount);

        /// <summary>
        /// Get the total current number of items in the collection
        /// </summary>
        /// <returns>The number of items in the collection.</returns>
        int GetCollectionItemCount();

        /// <summary>
        /// Return a game object that is no longer needed for visual presentation.
        /// </summary>
        /// <param name="itemIndex">The index in the collection of the item to return.</param>
        /// <param name="itemGO">The actual game object to return, usually a prefab.</param>
        void ReturnGameObjectForReuse(int itemIndex, GameObject itemGO);
    }
}
#pragma warning restore CS1591
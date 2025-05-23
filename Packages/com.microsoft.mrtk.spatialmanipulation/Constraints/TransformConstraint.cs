// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Base class for all constraints that be applied to <see cref="ObjectManipulator"/>
    /// or <see cref="BoundsControl"/> components.
    /// </summary>
    /// <remarks>
    /// The constraint system might be reworked in the future. In such a case, these existing components will be deprecated.
    /// </remarks>
    public abstract class TransformConstraint : MonoBehaviour
    {
        #region Properties

        [SerializeField]
        [EnumFlags]
        [Tooltip("What type of manipulation this constraint applies to. Defaults to One Handed and Two Handed.")]
        private ManipulationHandFlags handType = ManipulationHandFlags.OneHanded | ManipulationHandFlags.TwoHanded;

        /// <summary>
        /// Whether this constraint applies to one hand manipulation, two hand manipulation or both
        /// </summary>
        public ManipulationHandFlags HandType
        {
            get => handType;
            set => handType = value;
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("What type of manipulation this constraint applies to. Defaults to Near and Far.")]
        private ManipulationProximityFlags proximityType = ManipulationProximityFlags.Near | ManipulationProximityFlags.Far;

        /// <summary>
        /// Whether this constraint applies to near manipulation, far manipulation or both
        /// </summary>
        public ManipulationProximityFlags ProximityType
        {
            get => proximityType;
            set => proximityType = value;
        }

        [SerializeField]
        [Tooltip("Execution order priority of this constraint. Lower numbers will be executed before higher numbers.")]
        private int executionOrder = 0;

        /// <summary>
        /// Execution order priority of this constraint. Lower numbers will be executed before higher numbers.
        /// </summary>
        public int ExecutionPriority
        {
            get => executionOrder;
            set
            {
                executionOrder = value;

                // Notify all ConstraintManagers to re-sort these priorities.
                foreach (var mgr in gameObject.GetComponents<ConstraintManager>())
                {
                    mgr.RefreshPriorities();
                }
            }
        }

        /// <summary>
        /// The world pose of the object when the manipulation began.
        /// </summary>
        protected MixedRealityTransform WorldPoseOnManipulationStart;

        /// <summary>
        /// The initial world pose before any manipulations.
        /// </summary>
        protected MixedRealityTransform InitialWorldPose;

        /// <summary>
        /// Get flags that describe the type of transformations this constraint applies to.
        /// </summary> 
        public abstract TransformFlags ConstraintType { get; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Called once before any constraints are computed.
        /// </summary>
        public virtual void Setup(MixedRealityTransform worldPose)
        {
            InitialWorldPose = worldPose;
        }

        /// <summary>
        /// Called when manipulation starts on the attached object.
        /// </summary>
        public virtual void OnManipulationStarted(MixedRealityTransform worldPose)
        {
            WorldPoseOnManipulationStart = worldPose;
        }

        /// <summary>
        /// Abstract method for applying constraints to transforms during manipulation
        /// </summary>
        public abstract void ApplyConstraint(ref MixedRealityTransform transform);

        #endregion Public Methods

        #region MonoBehaviour

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        protected void OnEnable()
        {
            var managers = gameObject.GetComponents<ConstraintManager>();
            foreach (var manager in managers)
            {
                manager.AutoRegisterConstraint(this);
            }
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been disabled.
        /// </summary>
        protected void OnDisable()
        {
            var managers = gameObject.GetComponents<ConstraintManager>();
            foreach (var manager in managers)
            {
                manager.AutoUnregisterConstraint(this);
            }
        }

        #endregion
    }
}

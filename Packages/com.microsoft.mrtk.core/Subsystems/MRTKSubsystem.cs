// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// A SubsystemWithProvider that supports Mixed Reality Toolkit lifecycle methods.
    /// </summary>
    public abstract class MRTKSubsystem<TSubsystem, TSubsystemDescriptor, TProvider> :
        SubsystemWithProvider<TSubsystem, TSubsystemDescriptor, TProvider>, IMRTKManagedSubsystem
        where TSubsystem : SubsystemWithProvider<TSubsystem, TSubsystemDescriptor, TProvider>, IMRTKManagedSubsystem, new()
        where TSubsystemDescriptor : SubsystemDescriptorWithProvider, IMRTKSubsystemDescriptor
        where TProvider : MRTKSubsystemProvider<TSubsystem>
    {
        private static readonly ProfilerMarker UpdatePerfMarker =
            new ProfilerMarker("[MRTK] MRTKSubsystem.Update");

        void IMRTKManagedSubsystem.Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (!running) { return; }
                OnUpdate();
            }
        }

        private static readonly ProfilerMarker LateUpdatePerfMarker =
            new ProfilerMarker("[MRTK] MRTKSubsystem.LateUpdate");

        void IMRTKManagedSubsystem.LateUpdate()
        {
            using (LateUpdatePerfMarker.Auto())
            {
                if (!running) { return; }
                OnLateUpdate();
            }
        }

        private static readonly ProfilerMarker FixedUpdatePerfMarker =
            new ProfilerMarker("[MRTK] MRTKSubsystem.FixedUpdate");

        void IMRTKManagedSubsystem.FixedUpdate()
        {
            using (FixedUpdatePerfMarker.Auto())
            {
                if (!running) { return; }
                OnFixedUpdate();
            }
        }

        /// <summary>
        /// Called in response to the lifecycle manager
        /// update event function being called, if this subsystem is running.
        /// </summary>
        protected virtual void OnUpdate()
        {
            provider.Update();
        }

        /// <summary>
        /// Called in response to the lifecycle manager
        /// late update event function being called, if this subsystem is running.
        /// </summary>
        protected virtual void OnLateUpdate()
        {
            provider.LateUpdate();
        }

        /// <summary>
        /// Called in response to the lifecycle manager
        /// fix update event function being called, if this subsystem is running.
        /// </summary>
        protected virtual void OnFixedUpdate()
        {
            provider.FixedUpdate();
        }
    }
}

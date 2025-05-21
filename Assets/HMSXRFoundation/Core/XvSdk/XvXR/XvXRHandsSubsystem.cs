// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using UnityEngine.XR;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

namespace HMS.openxr.Input
{
    [Preserve]
    [MRTKSubsystem(
        Name = "com.xv.openxr.xrhands",
        DisplayName = "Subsystem for Xv XRSDK Hands API",
        Author = "Xv",
        ProviderType = typeof(XvXRProvider),
        SubsystemTypeOverride = typeof(XvXRHandsSubsystem),
        ConfigType = typeof(BaseSubsystemConfig))]
    public class XvXRHandsSubsystem : HandsSubsystem
    {
        private const string TAG = "XvXRHandsSubsystem";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            XvUtils.log(TAG, "Register");
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<XvXRHandsSubsystem, HandsSubsystemCinfo>();

            // Populate remaining cinfo field.
            cinfo.IsPhysicalData = true;

            if (!Register(cinfo))
            {
                XvUtils.log(TAG, $"Failed to register the {cinfo.Name} subsystem.");
            }
        }



        private class XvXRHandContainer : HandDataContainer
        {
            public XvXRHandContainer(XRNode handNode) : base(handNode)
            {
                XvUtils.log(TAG, "XvXRHandContainer");
            }

            private static readonly ProfilerMarker TryGetEntireHandPerfMarker =
                new ProfilerMarker("[XV] XvXRHandsSubsystem.TryGetEntireHand");

            /// <inheritdoc/>
            public override bool TryGetEntireHand(out IReadOnlyList<HandJointPose> result)
            {
                using (TryGetEntireHandPerfMarker.Auto())
                {
                    bool ret = false;
                    if (XvXRController.instance != null)
                    { 
                        ret = XvXRController.instance.getHandData(HandNode, HandJoints);
                    }
                    result = HandJoints;
                    return ret;
                }
            }

            private static readonly ProfilerMarker TryGetJointPerfMarker =
                new ProfilerMarker("[XV] XvXRHandsSubsystem.TryGetJoint");

            /// <inheritdoc/>
            public override bool TryGetJoint(TrackedHandJoint joint, out HandJointPose pose)
            {
                using (TryGetJointPerfMarker.Auto())
                {
                    bool ret = false;
                    if (XvXRController.instance != null)
                    {
                        ret = XvXRController.instance.getHandData(HandNode, HandJoints);
                    }
                    pose = HandJoints[(int)(joint)];
                    return ret;
                }
            }

        }

        [Preserve]
        private class XvXRProvider : Provider
        {
            private Dictionary<XRNode, XvXRHandContainer> hands = null;

            /// <inheritdoc/>
            public override void Start()
            {
                XvUtils.log(TAG, "XvXRProvider.Start");
                base.Start();

                hands ??= new Dictionary<XRNode, XvXRHandContainer>
                {
                    { XRNode.LeftHand, new XvXRHandContainer(XRNode.LeftHand) },
                    { XRNode.RightHand, new XvXRHandContainer(XRNode.RightHand) }
                };

                InputSystem.onBeforeUpdate += ResetHands;
            }

            public override void Stop()
            {
                XvUtils.log(TAG, "XvXRProvider.Stop");
                ResetHands();
                InputSystem.onBeforeUpdate -= ResetHands;
                base.Stop();
            }

            private void ResetHands()
            {
                hands[XRNode.LeftHand].Reset();
                hands[XRNode.RightHand].Reset();
            }

            #region IHandsSubsystem implementation

            /// <inheritdoc/>
            public override bool TryGetEntireHand(XRNode handNode, out IReadOnlyList<HandJointPose> jointPoses)
            {
                Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, "Non-hand XRNode used in TryGetEntireHand query");

                bool ret = hands[handNode].TryGetEntireHand(out jointPoses);
                if (ret)
                { 
                    XvUtils.log(TAG, $"TryGetEntireHand :{jointPoses[0].Position.x}, {jointPoses[0].Position.y}, {jointPoses[0].Position.z}");
                }

                return ret;
            }

            /// <inheritdoc/>
            public override bool TryGetJoint(TrackedHandJoint joint, XRNode handNode, out HandJointPose jointPose)
            {
                Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, "Non-hand XRNode used in TryGetJoint query");

                bool ret = hands[handNode].TryGetJoint(joint, out jointPose);
                if (ret)
                { 
                    XvUtils.log(TAG, $"TryGetJoint :{jointPose.Position.x}, {jointPose.Position.y}, {jointPose.Position.z}");
                }
                return ret;
            }

            #endregion IHandsSubsystem implementation
        }
    }
}

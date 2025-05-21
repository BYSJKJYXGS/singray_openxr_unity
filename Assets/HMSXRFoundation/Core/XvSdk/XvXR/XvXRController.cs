using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using Unity.Profiling;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using UnityEngine.XR;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using HMS.Spaces;
using static HMS.Spaces.XvFeature;

namespace HMS.openxr.Input
{
    public enum HandJointID
    {
        Invalid = -1,
        ThumbMetacarpal = 0,
        ThumbProximal,
        ThumbDistal,
        ThumbTip,
        IndexProximal,//4
        IndexMiddle,
        IndexDistal,
        IndexTip,
        MiddleProximal,//8
        MiddleMiddle,
        MiddleDistal,
        MiddleTip,
        RingProximal,
        RingMiddle,
        RingDistal,
        RingTip,
        PinkyMetacarpal,
        PinkyProximal,
        PinkyMiddle,
        PinkyDistal,
        PinkyTip,
        MiddleMetacarpal=21,
        Wrist = 22,
        
        Palm = 24,

        Max = Palm + 1
    }

    public class XvXRController : MonoBehaviour
    {
        private const string TAG = "XvXRController";

        public static XvXRController instance;

        private Vector3 DEFAULT_POINT = new Vector3(0,0,100);

        private const int PRE_COUNT = 25;

        private HandJointPose[] mLeftHand { get; set; } = new HandJointPose[PRE_COUNT];
        private bool mLeftTracked = false;

        private HandJointPose[] mRightHand { get; set; } = new HandJointPose[PRE_COUNT];
        private bool mRightTracked = false;

        void Start()
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
            GameObject simulator = GameObject.Find("MRTKInputSimulator");
            if (simulator != null)
            { 
            #if UNITY_EDITOR
                simulator.SetActive(true);
            #elif UNITY_ANDROID
                simulator.SetActive(false);
            #endif
            }

            List<HandsSubsystem> handsSubsystems = new List<HandsSubsystem>();
            SubsystemManager.GetSubsystems(handsSubsystems);
            XvUtils.log(TAG, $"XvHandSubsystem count: {handsSubsystems.Count}.");
            foreach(HandsSubsystem handSub in handsSubsystems)
            {
                XvUtils.log(TAG, $"XvHandSubsystem {handSub.GetType().Name}.");
            }
        }


        int count = 0;

        void Update()
        {
            count++;
            if (count % 2 == 0)
            {
            #if UNITY_EDITOR

            #elif UNITY_ANDROID
                Vector3 camT = Camera.main.transform.localPosition;
                Vector3 camR = Camera.main.transform.localRotation.ToEulerAngles();
                // XvUtils.log(TAG, $"camera pos:({camT.x}, {camT.y}, {camT.z}), rotation:({camR.x}, {camR.y}, {camR.z})");
                XvXRSkeleton skeleton = new XvXRSkeleton();      
                bool ret1 = XvFeature.XvFeatureGetHandTrackingData(ref skeleton);
                for (int i = 0; i < skeleton.size; i++)
                {
                    Vector3 pos = DEFAULT_POINT;
                    bool isTracked = false;
                    XrPose joint = skeleton.joints_ex[i];
                    if (Double.NaN == joint.pos.x || Double.NaN == joint.pos.y || Double.NaN == joint.pos.z)
                    {
                        pos = DEFAULT_POINT;
                    }
                    else if (joint.pos.x == 0 && joint.pos.y == 0 && joint.pos.z == 0)
                    {
                        pos = DEFAULT_POINT;
                    }
                    else
                    {
                        isTracked = true;
                        pos = new Vector3(skeleton.joints_ex[i].pos.x, -skeleton.joints_ex[i].pos.y, skeleton.joints_ex[i].pos.z);
                    }

                    Quaternion rot = new Quaternion(-joint.rotatePoint.x, joint.rotatePoint.y, -joint.rotatePoint.z, joint.rotatePoint.w);
                    
                    if (i < PRE_COUNT)
                    {
                        mLeftHand[i%PRE_COUNT].Position = pos;
                        mLeftHand[i%PRE_COUNT].Rotation = rot;
                        mLeftTracked = isTracked;
                    }
                    else if (i < 2 * PRE_COUNT)
                    { 
                        mRightHand[i%PRE_COUNT].Position = pos;
                        mRightHand[i%PRE_COUNT].Rotation = rot;
                        mRightTracked = isTracked;
                    }
                }

                if (mLeftTracked || mRightTracked)
                { 
                    Vector3 lPos = mLeftHand[(int)HandJointID.Palm].Position;
                    Vector3 lRot = mLeftHand[(int)HandJointID.Palm].Rotation.ToEulerAngles();
                    Vector3 rPos = mRightHand[(int)HandJointID.Palm].Position;
                    Vector3 rRot = mRightHand[(int)HandJointID.Palm].Rotation.ToEulerAngles();
                    XvUtils.log(TAG, $"GetHandTrackingData head pos:({camT.x}, {camT.y}, {camT.z}) rot:({camR.x}, {camR.y}, {camR.z})");
                    XvUtils.log(TAG, $"GetHandTrackingData left pos:({lPos.x}, {lPos.y}, {lPos.z}) rot:({lRot.x}, {lRot.y}, {lRot.z})");
                    XvUtils.log(TAG, $"GetHandTrackingData right pos:({rPos.x}, {rPos.y}, {rPos.z}) rot:({rRot.x}, {rRot.y}, {rRot.z})");
                }
            #endif
            }
        }

        static internal HandJointID getMRTKJoint(TrackedHandJoint joint)
        {
            switch (joint)
            {
                case TrackedHandJoint.Palm: return HandJointID.Palm;//AttachmentPointFlags.Palm;
                case TrackedHandJoint.Wrist: return HandJointID.Wrist;//AttachmentPointFlags.Wrist;
                   
                case TrackedHandJoint.ThumbProximal: return HandJointID.ThumbProximal;//AttachmentPointFlags.ThumbProximalJoint;
                case TrackedHandJoint.ThumbDistal: return HandJointID.ThumbDistal;//AttachmentPointFlags.ThumbDistalJoint;
                case TrackedHandJoint.ThumbTip: return HandJointID.ThumbTip;//AttachmentPointFlags.ThumbTip;
                case TrackedHandJoint.ThumbMetacarpal: return HandJointID.ThumbMetacarpal;

		        case TrackedHandJoint.IndexMetacarpal: return HandJointID.IndexProximal;
                case TrackedHandJoint.IndexProximal: return HandJointID.IndexProximal;//AttachmentPointFlags.IndexKnuckle;
                case TrackedHandJoint.IndexIntermediate: return HandJointID.IndexMiddle;//.IndexMiddleJoint;
                case TrackedHandJoint.IndexDistal: return HandJointID.IndexDistal;//AttachmentPointFlags.IndexDistalJoint;
                case TrackedHandJoint.IndexTip: return HandJointID.IndexTip;//AttachmentPointFlags.IndexTip;
                
                case TrackedHandJoint.MiddleMetacarpal: return HandJointID.MiddleMetacarpal;
                case TrackedHandJoint.MiddleProximal: return HandJointID.MiddleProximal;//AttachmentPointFlags.MiddleKnuckle;
                case TrackedHandJoint.MiddleIntermediate: return HandJointID.MiddleMiddle;//AttachmentPointFlags.MiddleMiddleJoint;
                case TrackedHandJoint.MiddleDistal: return HandJointID.MiddleDistal;//AttachmentPointFlags.MiddleDistalJoint;
                case TrackedHandJoint.MiddleTip: return HandJointID.MiddleTip;//AttachmentPointFlags.MiddleTip;

		        case TrackedHandJoint.RingMetacarpal: return HandJointID.RingProximal;//AttachmentPointFlags.RingKnuckle;
                case TrackedHandJoint.RingProximal: return HandJointID.RingProximal;//AttachmentPointFlags.RingKnuckle;
                case TrackedHandJoint.RingIntermediate: return HandJointID.RingMiddle;//AttachmentPointFlags.RingMiddleJoint;
                case TrackedHandJoint.RingDistal: return HandJointID.RingDistal;//AttachmentPointFlags.RingDistalJoint;
                case TrackedHandJoint.RingTip: return HandJointID.RingTip;//AttachmentPointFlags.RingTip;

                case TrackedHandJoint.LittleProximal: return HandJointID.PinkyProximal;//AttachmentPointFlags.PinkyKnuckle;
                case TrackedHandJoint.LittleIntermediate: return HandJointID.PinkyMiddle;//AttachmentPointFlags.PinkyMiddleJoint;
                case TrackedHandJoint.LittleDistal: return HandJointID.PinkyDistal;//AttachmentPointFlags.PinkyDistalJoint;
                case TrackedHandJoint.LittleTip: return HandJointID.PinkyTip;//AttachmentPointFlags.PinkyTip;
                case TrackedHandJoint.LittleMetacarpal: return HandJointID.PinkyMetacarpal;

                // Metacarpals are not included in AttachmentPointFlags
                default: return HandJointID.Wrist;//AttachmentPointFlags.Wrist;
            }
        }

        public bool getHandData(XRNode handNode, HandJointPose[] result) 
        {
            HandJointPose[] list = handNode == XRNode.LeftHand ? mLeftHand : mRightHand;
            for (TrackedHandJoint j = TrackedHandJoint.Palm; j < TrackedHandJoint.TotalJoints; j++)
            {
                HandJointID joint = getMRTKJoint(j);
                result[(int)j] = list[(int)joint];
            }

            return handNode == XRNode.LeftHand ? mLeftTracked : mRightTracked;
        }
    }
}

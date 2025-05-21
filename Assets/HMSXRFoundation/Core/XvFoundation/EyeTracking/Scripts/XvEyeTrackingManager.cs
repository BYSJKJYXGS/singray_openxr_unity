using AOT;
using System;
using System.IO;
using UnityEngine;
using HMS.Spaces;

namespace HMSXR.Foundation
{
    using static HMS.Spaces.XvFeature;
    using static XvEyeTracking;

    /// <summary>
    /// This class is responsible for eye tracking activation/deactivation and eye data acquisition
    /// </summary>

    public sealed class XvEyeTrackingManager : MonoBehaviour
    {
        private XvEyeTrackingManager() { }
        static string config_path = "/data/misc/xr/";
        static string TAG = "wuxh:";

        int ret_startGaze = 0;
        int ret_calirationApply = 0;

        int ret_setExposure = 0;
        int ret_setBright = 0;

        // Eye tracking head 6DOF matrix
        Matrix4x4 MatrixHead = Matrix4x4.identity;
        // Pose matrix of binocular center relative to IMU
        private Matrix4x4 MatrixMiddleOfEyes = Matrix4x4.identity;

        private xrEtEyeDataEx eyeData;

        private Matrix4x4 middleOfEyeToHeadMatrix; // Transformation from binocular center to world coordinate system

        public Matrix4x4 MiddleOfEyeToHeadMatrix
        {
            get { return middleOfEyeToHeadMatrix; }
        }

        /// <summary>
        /// Whether eye tracking is active
        /// </summary>
        private static bool tracking;
        public bool Tracking
        {
            get
            {
                return tracking;
            }
        }

        public xrEtEyeDataEx EyeData
        {
            get
            {
                return eyeData;
            }
        }

        /// <summary>
        /// Binocular origin point
        /// </summary>
        public Vector3 GazeOrigin
        {
            get
            {
                return GetGazePoint(eyeData.recomGaze.gazeOrigin);
            }
        }

        /// <summary>
        /// Binocular gaze direction
        /// </summary>
        public Vector3 GazeDirection
        {
            get
            {
                return GetDirection(eyeData.recomGaze.gazeOrigin, eyeData.recomGaze.gazeDirection);
            }
        }

        /// <summary>
        /// Left eye gaze origin point
        /// </summary>
        public Vector3 LeftGazeOrigin
        {
            get
            {
                return GetGazePoint(eyeData.leftGaze.gazeOrigin);
            }
        }

        /// <summary>
        /// Left eye gaze direction
        /// </summary>
        public Vector3 LeftGazeDirection
        {
            get
            {
                return GetDirection(eyeData.leftGaze.gazeOrigin, eyeData.leftGaze.gazeDirection);
            }
        }

        // Right eye origin point
        public Vector3 RightGazeOrigin
        {
            get
            {
                return GetGazePoint(eyeData.rightGaze.gazeOrigin);
            }
        }

        /// <summary>
        /// Right eye gaze direction
        /// </summary>
        public Vector3 RightGazeDirection
        {
            get
            {
                return GetDirection(eyeData.rightGaze.gazeOrigin, eyeData.rightGaze.gazeDirection);
            }
        }

        /// <summary>
        /// Start eye tracking
        /// </summary>
        public void StartGaze()
        {
#if UNITY_EDITOR
            return;
#endif
            if (!tracking)
            {
                bool setFile = XvFeature.XvFeatureGazeSetConfigPath(config_path);
                MyDebugTool.Log($"XvFeatureGazeSetConfigPath:{setFile}   config_path={config_path}");

                bool startGaze = XvFeature.XvFeatureStartGaze(ref ret_startGaze);

                DebugLog($"{TAG}XvFeatureStartGaze:{startGaze}   ret={ret_startGaze}");

                string path = "/data/misc/xr/XVETcaliData_.dat";

                bool cApply = true;
                if (File.Exists(path))
                {
                    cApply = XvFeature.XvFeatureGazeCalirationApply(path, ref ret_calirationApply);
                    DebugLog($"{TAG}XvFeatureGazeCalirationApply:{cApply}   ret={ret_calirationApply}");
                }
                else
                {
                    DebugLog($"{TAG}File Exists:{false}   ");
                }

                bool sExposure = XvFeature.XvFeatureSetExposure(10, 6, 10, 6, ref ret_setExposure);
                DebugLog($"{TAG}XvFeatureSetExposure:{sExposure}   ret={ret_setExposure}");

                bool sBright = XvFeature.XvFeatureSetBright(2, 8, 27, ref ret_setBright);
                DebugLog($"{TAG}XvFeatureSetBright:{sBright}   ret={ret_setBright}");

                tracking = setFile && startGaze && sExposure && sBright && cApply;
                DebugLog($"{TAG}tracking:{tracking}  ");
            }
        }

        /// <summary>
        /// Stop eye tracking
        /// </summary>
        public void StopGaze()
        {
#if UNITY_EDITOR
            return;
#endif

            if (Tracking)
            {
                tracking = false;
                XvFeature.XvFeatureUnsetGazeCallback(ref ret_calirationApply);
                XvFeature.XvFeatureSetBright(2, 8, 0, ref ret_setBright);
                XvFeature.XvFeatureSetExposure(10, 6, 10, 6, ref ret_setExposure);
                XvFeature.XvFeatureStopGaze(ref ret_startGaze);
            }
        }

        private void Update()
        {
            if (!Tracking)
            {
                return;
            }
#if PLATFORM_ANDROID && !UNITY_EDITOR
        eyeData = new xrEtEyeDataEx();

        if (XvFeature.XvFeatureGetGazeCallback(ref eyeData))
        {
            //Debug.Log($"{TAG}XvFeatureGetGazeCallback:true");
            UpdateMatrix();
        }
        else
        {
            DebugLog($"{TAG}XvFeatureGetGazeCallback:false");
        }

#endif
        }

        private void UpdateMatrix()
        {
            MatrixHead.SetTRS(Camera.main.transform.position, Camera.main.transform.rotation, Vector3.one);

            xrStereoPdmCalibration fed = new xrStereoPdmCalibration();
            bool dCalibration = XvFeature.XvFeatureReadStereoDisplayCalibration(ref fed);
            DebugLog($"{TAG}dCalibration:" + dCalibration);

            Vector3 LeftdisplayPos = new Vector3((float)fed.calibrations[0].extrinsic.translation[0],
                -(float)fed.calibrations[0].extrinsic.translation[1],
                (float)fed.calibrations[0].extrinsic.translation[2]);
            Vector3 RightdisplayPos = new Vector3((float)fed.calibrations[1].extrinsic.translation[0],
                -(float)fed.calibrations[1].extrinsic.translation[1],
                (float)fed.calibrations[1].extrinsic.translation[2]);

            Vector3 normal = (RightdisplayPos - LeftdisplayPos).normalized;
            float distance = Vector3.Distance(LeftdisplayPos, RightdisplayPos);
            Vector3 middleOfEyes_pos = normal * (distance * 0.5f) + LeftdisplayPos;

            Quaternion middleQua = RotationMatrixToQuaternion(fed.calibrations[0].extrinsic.rotation);
            MatrixMiddleOfEyes.SetTRS(middleOfEyes_pos, new Quaternion(-middleQua.x, middleQua.y, -middleQua.z, middleQua.w), Vector3.one);
            DebugLog($"{TAG}MatrixMiddleOfEyes:" + MatrixMiddleOfEyes);

            middleOfEyeToHeadMatrix = MatrixHead * MatrixMiddleOfEyes;
        }

        private Vector3 GetGazePoint(Vector3d oGazeOrigin)
        {
            Vector3 gazeOrigin = new Vector3(oGazeOrigin.x, -oGazeOrigin.y, oGazeOrigin.z) / 1000;

            Matrix4x4 Matrix_gazeOrigin = Matrix4x4.identity;
            Matrix4x4 Matrix_XVgazeOrigin = Matrix4x4.identity;
            Matrix_gazeOrigin.SetTRS(gazeOrigin, Quaternion.identity, Vector3.one);
            Matrix_XVgazeOrigin = middleOfEyeToHeadMatrix * Matrix_gazeOrigin;

            // Get position
            gazeOrigin = Matrix_XVgazeOrigin.GetColumn(3);
            return gazeOrigin;
        }

        private Vector3 GetDirection(Vector3d oGazeOrigin, Vector3d oGazeDirection)
        {
            Vector3 gazeOrigin = new Vector3(oGazeOrigin.x, -oGazeOrigin.y, oGazeOrigin.z) / 1000;
            Vector3 gazeDirection = gazeOrigin + new Vector3(oGazeDirection.x, -oGazeDirection.y, oGazeDirection.z) * 15;
            Matrix4x4 Matrix_target = Matrix4x4.identity;
            Matrix4x4 Matrix_XVtarget = Matrix4x4.identity;

            Matrix_target.SetTRS(gazeDirection, Quaternion.identity, Vector3.one);
            Matrix_XVtarget = middleOfEyeToHeadMatrix * Matrix_target;

            // Get new direction vector
            gazeDirection = (Vector3)Matrix_XVtarget.GetColumn(3) - GetGazePoint(oGazeOrigin);
            return gazeDirection;
        }

        private void OnDestroy()
        {
            StopGaze();
        }

        private void OnApplicationQuit()
        {
            StopGaze();
        }

        // Convert rotation matrix to quaternion
        private Quaternion RotationMatrixToQuaternion(double[] rm)
        {
            double w, x, y, z;
            w = Math.Sqrt(1 + rm[0] + rm[4] + rm[8]) / 2;
            x = (rm[7] - rm[5]) / (4 * w);
            y = (rm[2] - rm[6]) / (4 * w);
            z = (rm[3] - rm[1]) / (4 * w);

            Quaternion qua = new Quaternion((float)x, (float)y, (float)z, (float)w);
            return qua;
        }

        private void DebugLog(object msg)
        {
            MyDebugTool.Log(msg);
        }
    }
}
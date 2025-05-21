using System;
using System.Runtime.InteropServices;
using UnityEngine;
using HMS.Spaces;

namespace HMSXR.Foundation
{
    public class XvARCamera : XvCameraBase
    {
        public XvARCamera(int width, int height, int fps, FrameArrived frameArrived)
            : base(width, height, fps, frameArrived)
        {
        }

        private Texture2D tex = null;
        private Color32[] pixel32;
        private GCHandle pixelHandle;
        private IntPtr pixelPtr;

        private double rgbTimestamp = 0;
        private int lastWidth = 0;
        private int lastHeight = 0;
        private int countTime = 0;
        private int count = 0;

        public override void StartCapture()
        {
            isOpen = XvFeature.XvFeatureStartRgbStreaming();
        }

        public override void StopCapture()
        {
            if (IsOpen)
            {
                XvFeature.XvFeatureStopRgbStreaming();
            }
            isOpen = false;
        }

        public override void Update()
        {
#if UNITY_EDITOR
            return;
#endif
            if (isOpen)
            {
                if (!readRgbCalibrationFlag)
                {
                    ReadRgbCalibration();
                }

                int width = 0;
                int height = 0;
                bool ifGet1 = XvFeature.XvFeatureGetRgbImageSize(ref width, ref height);

                if (width > 0 && height > 0)
                {
                    if (lastWidth != width || lastHeight != height)
                    {
                        try
                        {
                            double r = 1.0;
                            int w = (int)(width * r);
                            int h = (int)(height * r);
                            TextureFormat format = TextureFormat.RGBA32;
                            tex = new Texture2D(w, h, format, false);
                            tex.Apply();

                            try { pixelHandle.Free(); } catch { }
                            pixel32 = tex.GetPixels32();
                            pixelHandle = GCHandle.Alloc(pixel32, GCHandleType.Pinned);
                            pixelPtr = pixelHandle.AddrOfPinnedObject();

                            cameraData.texWidth = w;
                            cameraData.texHeight = h;
                        }
                        catch (Exception e)
                        {
                            return;
                        }

                        lastWidth = width;
                        lastHeight = height;
                    }

                    try
                    {
                        if (XvFeature.XvFeatureGetRgbImageData(pixelPtr, tex.width, tex.height, ref rgbTimestamp))
                        {
                            tex.SetPixels32(pixel32);
                            tex.Apply();
                            cameraData.tex = tex;
                            cameraData.parameter.timeStamp = rgbTimestamp;
                            frameArrived?.Invoke(cameraData);
                        }
                    }
                    catch (Exception e)
                    {
                        MyDebugTool.Log(e.Message);
                        return;
                    }
                }
            }
            else
            {
                Debug.Log("wuxh:Camera not opened");
            }
        }

        private bool readRgbCalibrationFlag = false;
        private double[] _R;
        private double[] _T;
        private double[] _EulerAngles;
        private double[] _poseData = new double[7];

        void ReadRgbCalibration()
        {
            XvFeature.XrCalibrationXV rgb_Calibration = default(XvFeature.XrCalibrationXV);
            if (XvFeature.XvFeatureGetCalibrationData(0, ref rgb_Calibration))
            {
                _T = new double[3] {
                    rgb_Calibration.ex_trans[0],
                    -rgb_Calibration.ex_trans[1],
                    rgb_Calibration.ex_trans[2]
                };

                _R = new double[9] {
                    rgb_Calibration.ex_rotation[0],
                    -rgb_Calibration.ex_rotation[1],
                    rgb_Calibration.ex_rotation[2],
                    -rgb_Calibration.ex_rotation[3],
                    rgb_Calibration.ex_rotation[4],
                    -rgb_Calibration.ex_rotation[5],
                    rgb_Calibration.ex_rotation[6],
                    -rgb_Calibration.ex_rotation[7],
                    rgb_Calibration.ex_rotation[8]
                };

                RotationMatrixToEulerAngles(ref _EulerAngles, _R);

                cameraData.parameter.position = new Vector3(
                    (float)_T[0],
                    (float)_T[1],
                    (float)_T[2]
                );

                Vector3 localEuler = new Vector3(
                    (float)_EulerAngles[0],
                    (float)_EulerAngles[1],
                    (float)_EulerAngles[2]
                );

                cameraData.parameter.rotation = Quaternion.Euler(localEuler);

                cameraData.parameter.focal = 3.519f; // RGB camera focal length (mm)
                cameraData.parameter.fx = (float)rgb_Calibration.K[0];
                cameraData.parameter.fy = (float)rgb_Calibration.K[1];
                cameraData.parameter.cx = (float)rgb_Calibration.K[2];
                cameraData.parameter.cy = (float)rgb_Calibration.K[3];
                cameraData.parameter.width = (float)rgb_Calibration.K[9];
                cameraData.parameter.height = (float)rgb_Calibration.K[10];

                readRgbCalibrationFlag = true;
            }
            else
            {
                Debug.Log("wuxh:Failed to read RGB calibration data");
            }
        }

        internal void RotationMatrixToEulerAngles(ref double[] eulerAngle, double[] rm)
        {
            double sy = Math.Sqrt(rm[0] * rm[0] + rm[3] * rm[3]);
            bool singular = sy < 1e-6;
            double x, y, z;

            if (!singular)
            {
                x = Math.Atan2(rm[7], rm[8]);
                y = Math.Atan2(-rm[6], sy);
                z = Math.Atan2(rm[3], rm[0]);
            }
            else
            {
                x = Math.Atan2(-rm[5], rm[4]);
                y = Math.Atan2(-rm[6], sy);
                z = 0;
            }

            x = x * 180.0f / Math.PI;
            y = y * 180.0f / Math.PI;
            z = z * 180.0f / Math.PI;
            eulerAngle = new double[3] { x, y, z };
        }
    }
}
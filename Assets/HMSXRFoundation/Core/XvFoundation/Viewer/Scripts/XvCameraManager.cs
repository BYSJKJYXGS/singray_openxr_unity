using UnityEngine;
using UnityEngine.Events;

namespace HMSXR.Foundation
{
    /// <summary>
    /// This class provides access to camera image data and camera control functions for various camera types, including:
    /// - Time-of-Flight (ToF) camera
    /// - AR glasses camera
    /// - Computing unit camera
    /// - Left and right fisheye cameras
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class XvCameraManager : MonoBehaviour
    {
        private XvCameraManager() { }

        /// <summary>
        /// Camera resolution setting
        /// </summary>
        [SerializeField]
        private RgbResolution rgbResolution;

        /// <summary>
        /// Requested frame rate
        /// </summary>
        [SerializeField]
        private int requestedFPS = 30;

        /// <summary>
        /// Image dimensions
        /// </summary>
        private int requestedWidth = 1920;
        private int requestedHeight = 1080;

        public int Width
        {
            get { return requestedWidth; }
        }

        public int Height
        {
            get { return requestedHeight; }
        }

        public int Fps
        {
            get { return requestedFPS; }
        }

        /// <summary>
        /// Camera data event callbacks
        /// </summary>
        public static UnityEvent<cameraData> onARCameraStreamFrameArrived = new UnityEvent<cameraData>();
        public static UnityEvent<cameraData> onLeftStereoStreamFrameArrived = new UnityEvent<cameraData>();
        public static UnityEvent<cameraData> onRightStereoStreamFrameArrived = new UnityEvent<cameraData>();
        public static UnityEvent<cameraData> onTofDepthCameraStreamFrameArrived = new UnityEvent<cameraData>();
        public static UnityEvent<cameraData> onTofIRCameraStreamFrameArrived = new UnityEvent<cameraData>();
        public static UnityEvent<cameraData> onWebCameraStreamFrameArrived = new UnityEvent<cameraData>();

        private void SetCameraParameter(CameraSetting cameraSetting)
        {
            requestedWidth = cameraSetting.width;
            requestedHeight = cameraSetting.height;
            requestedFPS = cameraSetting.requestedFPS;
        }

        /// <summary>
        /// Start camera capture
        /// </summary>
        /// <param name="cameraType">Type of camera to start</param>
        public void StartCapture(XvCameraStreamType cameraType)
        {
            switch (rgbResolution)
            {
                case RgbResolution.RGB_1920x1080:
                    requestedWidth = 1920;
                    requestedHeight = 1080;
                    break;
                case RgbResolution.RGB_1280x720:
                    requestedWidth = 1280;
                    requestedHeight = 720;
                    break;
                case RgbResolution.RGB_640x480:
                    requestedWidth = 640;
                    requestedHeight = 480; // Fixed typo here (was 1048080)
                    break;
                case RgbResolution.RGB_320x240:
                    requestedWidth = 320;
                    requestedHeight = 240;
                    break;
                case RgbResolution.RGB_2560x1920:
                    requestedWidth = 2560;
                    requestedHeight = 1920;
                    break;
                default:
                    break;
            }

            switch (cameraType)
            {
                case XvCameraStreamType.WebCameraStream:
                    XvWebCameraManager.GetXvWebCameraManager().StartCapture(requestedWidth, requestedHeight, requestedFPS);
                    break;
                case XvCameraStreamType.ARCameraStream:
                    XvARCameraManager.GetXvARCameraManager().StartCapture(requestedWidth, requestedHeight, requestedFPS);
                    break;
                case XvCameraStreamType.TofDepthCameraStream:
                    XvTofManager.GetXvTofManager().StartCapture(requestedWidth, requestedHeight, requestedFPS, 0);
                    break;
                case XvCameraStreamType.LeftStereoCameraStream:
                    XvStereoCameraManager.GetXvStereoCameraManager(true).StartCapture(requestedWidth, requestedHeight, requestedFPS, true);
                    break;
                case XvCameraStreamType.RightStereoCameraStream:
                    XvStereoCameraManager.GetXvStereoCameraManager(false).StartCapture(requestedWidth, requestedHeight, requestedFPS, false);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Stop camera capture
        /// </summary>
        /// <param name="cameraType">Type of camera to stop</param>
        public void StopCapture(XvCameraStreamType cameraType)
        {
            switch (cameraType)
            {
                case XvCameraStreamType.WebCameraStream:
                    XvWebCameraManager.GetXvWebCameraManager().StopCapture();
                    break;
                case XvCameraStreamType.ARCameraStream:
                    XvARCameraManager.GetXvARCameraManager().StopCapture();
                    break;
                case XvCameraStreamType.TofDepthCameraStream:
                    XvTofManager.GetXvTofManager().StopCapture(0);
                    break;
                case XvCameraStreamType.LeftStereoCameraStream:
                    XvStereoCameraManager.GetXvStereoCameraManager(true).StopCapture();
                    break;
                case XvCameraStreamType.RightStereoCameraStream:
                    XvStereoCameraManager.GetXvStereoCameraManager(false).StopCapture();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Check if camera is active
        /// </summary>
        /// <param name="cameraType">Camera type to check</param>
        /// <returns>True if camera is active</returns>
        public bool IsOn(XvCameraStreamType cameraType)
        {
            switch (cameraType)
            {
                case XvCameraStreamType.WebCameraStream:
                    return XvWebCameraManager.GetXvWebCameraManager().IsOn;
                case XvCameraStreamType.ARCameraStream:
                    return XvARCameraManager.GetXvARCameraManager().IsOn;
                case XvCameraStreamType.TofDepthCameraStream:
                    return XvTofManager.GetXvTofManager().IsOn(0);
                case XvCameraStreamType.LeftStereoCameraStream:
                    return XvStereoCameraManager.GetXvStereoCameraManager(true).IsOn;
                case XvCameraStreamType.RightStereoCameraStream:
                    return XvStereoCameraManager.GetXvStereoCameraManager(false).IsOn;
                default:
                    break;
            }

            return false;
        }

        private void Update()
        {
            if (XvStereoCameraManager.GetXvStereoCameraManager(true).IsOn)
            {
                XvStereoCameraManager.GetXvStereoCameraManager(true).Update();
            }

            if (XvStereoCameraManager.GetXvStereoCameraManager(false).IsOn)
            {
                XvStereoCameraManager.GetXvStereoCameraManager(false).Update();
            }

            XvTofManager.GetXvTofManager().Update();

            if (XvARCameraManager.GetXvARCameraManager().IsOn)
            {
                XvARCameraManager.GetXvARCameraManager().Update();
            }

            if (XvWebCameraManager.GetXvWebCameraManager().IsOn)
            {
                XvWebCameraManager.GetXvWebCameraManager().Update();
            }
        }

        private void OnDestroy()
        {
            StopCapture(XvCameraStreamType.WebCameraStream);
            StopCapture(XvCameraStreamType.ARCameraStream);
            StopCapture(XvCameraStreamType.TofDepthCameraStream);
            StopCapture(XvCameraStreamType.LeftStereoCameraStream);
            StopCapture(XvCameraStreamType.RightStereoCameraStream);
        }

        private void OnApplicationQuit()
        {
            StopCapture(XvCameraStreamType.WebCameraStream);
            StopCapture(XvCameraStreamType.ARCameraStream);
            StopCapture(XvCameraStreamType.TofDepthCameraStream);
            StopCapture(XvCameraStreamType.LeftStereoCameraStream);
            StopCapture(XvCameraStreamType.RightStereoCameraStream);
        }
    }

    public enum XvCameraStreamType
    {
        WebCameraStream,      // Computing unit rear camera
        ARCameraStream,       // MR glasses RGB camera
        TofDepthCameraStream, // ToF depth camera
        // TofIRCameraStream,    // ToF IR camera
        LeftStereoCameraStream,  // Left fisheye camera
        RightStereoCameraStream  // Right fisheye camera
    }

    public class cameraData
    {
        public int texWidth;
        public int texHeight;
        public Texture tex;

        // Camera pose
        public CameraParameter parameter;
    }

    public struct CameraParameter
    {
        // AR camera pose
        public Vector3 position;
        public Quaternion rotation;

        // Timestamp
        public double timeStamp;

        // Camera intrinsics
        public float focal;
        public float fx;
        public float fy;
        public float cx;
        public float cy;

        // Texture dimensions
        public float width;
        public float height;
    }

    // RGB_1920x1080 = 0, ///< RGB 1080p
    // RGB_1280x720  = 1, ///< RGB 720p
    // RGB_640x480   = 2, ///< RGB 480p
    // RGB_320x240   = 3, ///< RGB QVGA
    // RGB_2560x1920 = 4, ///< RGB 5m
    public enum RgbResolution
    {
        RGB_1920x1080 = 0,
        RGB_1280x720 = 1,
        RGB_640x480 = 2,
        RGB_320x240 = 3,
        RGB_2560x1920 = 4
    }

    public struct CameraSetting
    {
        public int width;
        public int height;
        public int requestedFPS;
    }
}
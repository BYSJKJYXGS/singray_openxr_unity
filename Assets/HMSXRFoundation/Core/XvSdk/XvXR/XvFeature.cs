using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEditor;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;
using UnityEngine;
using static HMS.Spaces.XvFeature;
using System.Xml.Xsl;
using UnityEngine.InputSystem.XR;
#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace HMS.Spaces
{
    /// <summary>
    /// This OpenXRFeature implements XR_EXT_conformance_automation.
    /// See https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_conformance_automation
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Xv Unity API",
       BuildTargetGroups = new []{BuildTargetGroup.Standalone, BuildTargetGroup.WSA, BuildTargetGroup.Android},
        Company = "Unity",
        Desc = "The XV UNITY  API",
        DocumentationLink = Constants.k_DocumentationURL,
        OpenxrExtensionStrings = "XR_XV_api",
        Version = "0.0.1",
        FeatureId = featureId)]
#endif
    public partial class XvFeature : OpenXRFeature
    {
        /// <summary>
        /// Message to display upon interception.
        /// </summary>
        public string message = "Hello from C#!";

        /// <summary>
        /// Message received from native as a result of the intercepted xrCreateSession call.
        /// </summary>
        public string receivedMessage { get; private set; }
        public const string featureId = "com.unity.openxr.feature.XvFeature";
        //手势
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct Point
        {
            public float x;
            public float y;
            public float z;
        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct RotatePoint
        {
            public float x;
            public float y;
            public float z;
            public float w;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct XrPose
        {
            public RotatePoint rotatePoint;
            public Point pos;

        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct XvXRSkeleton
        {

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 42, ArraySubType = UnmanagedType.Struct)]
            //public Point[] joints;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52, ArraySubType = UnmanagedType.Struct)]
            public XrPose[] joints_ex;
            public int size;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct xrConfigInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string value;

        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct ObjectData
        {

            public Vector3 point;
            public int blobIndex;
            public int typeID;
            public string type;
            public double x;
            public double y;
            public double width;
            public double height;
            public double confidence;
            public int pointsSize;
            public IntPtr keypoints;

        };
        [StructLayout(LayoutKind.Sequential)]
        public struct Vector3d
        {
            public float x;
            public float y;
            public float z;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector3dInt
        {
            public uint x;
            public uint y;
            public uint z;
        };
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]


        public struct xplan_package
        {
            public uint points_nb;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string pid;
            public Vector3d normal; //X Y Z
            public uint verticesSize;
        
            public IntPtr vertices;
            public uint trianglesSize;
          
            public IntPtr triangles;
            public double distance;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct XslamSurface
        {
            public uint mapId;
            public uint version;
            public uint id;

            public uint verticesSize;
          
            public IntPtr vertices;
          
            public IntPtr vertexNormals;

            public uint trianglesSize;
          
            public IntPtr triangles;
           
            public uint textureWidth;
            public uint textureHeight;

        };

        [StructLayout(LayoutKind.Sequential)]
        public struct xrTagDataXV
        {
            public uint tagID;
            public Vector3d position;
            public Vector3d orientation;
            public RotatePoint quaternion;
            public uint edgeTimestamp;
            public double hostTimestamp;
            public float confidence;
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct xrTagArrayXV
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.Struct)]
            public xrTagDataXV[] detect;
        }
        ;
      




        private static ulong xrInstance = 0ul;
        private static ulong xrSession = 0ul;

        /// <inheritdoc/>
        protected override bool OnInstanceCreate(ulong instance)
        {

            if (!OpenXRRuntime.IsExtensionEnabled("XR_XV_api"))
            {
                Debug.LogError("XR_XV_api is not enabled. Disabling XvFeature");
                return false;
            }

            xrInstance = instance;
            xrSession = 0ul;

         
            initialize(xrGetInstanceProcAddr, xrInstance);
            Debug.LogError("XR_XV_api OnInstanceCreate==2");
            return true;
        }

      

        /// <inheritdoc/>
        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            base.OnInstanceDestroy(xrInstance);
            XvFeature.xrInstance = 0ul;
            Debug.LogError("XR_XV_api OnInstanceDestroy:" + xrInstance);
        }

        /// <inheritdoc/>
        protected override void OnSessionCreate(ulong xrSessionId)
        {
            XvFeature.xrSession = xrSessionId;
            base.OnSessionCreate(xrSession);
            Debug.LogError("XR_XV_api OnSessionCreate:" + xrSessionId);
        }

        /// <inheritdoc/>
        protected override void OnSessionDestroy(ulong xrSessionId)
        {
            base.OnSessionDestroy(xrSessionId);
            XvFeature.xrSession = 0ul;
            Debug.LogError("XR_XV_api OnSessionDestroy:" + xrSessionId);
        }



        /// <summary>
        /// Drive the xrSetInputDeviceActiveEXT function of the XR_EXT_conformance_automation.
        /// See https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XR_EXT_conformance_automation
        /// </summary>
        /// <param name="interactionProfile">An OpenXRPath that specifies the OpenXR Interaction Profile of the value to be changed (e.g. /interaction_profiles/khr/simple_controller).</param>
        /// <param name="topLevelPath">An OpenXRPath that specifies the OpenXR User Path of the value to be changed (e.g. /user/hand/left).</param>
        /// <param name="isActive">A boolean that specifies the desired state of the target.</param>
        /// <returns>Returns true if the state is set successfully, or false if there was an error.</returns>
        public static bool XvFeatureSetActive(string interactionProfile, string topLevelPath, bool isActive)
        {
            return xrSetInputDeviceActiveEXT(
                xrSession,
                GetCurrentInteractionProfile(interactionProfile),
                StringToPath(topLevelPath),
                isActive);
        }

        public static bool XvFeatureGetHandTrackingData(ref XvXRSkeleton skeleton)
        {
            return xrGetHandTrackingData(
                xrSession,
              ref skeleton);
        }

        public static bool XvFeatureRgbSetBrightness(int type, int brigtness)
        {
            return xrDeviceSetBrightness(
                xrSession,
                type,
              brigtness);
        }

        public static bool XvFeatureUpdateVariable(ref xrConfigInfo info) {
            return xrUpdateVariable( xrSession,ref info);
        }


        public static bool XvFeatureGetConfigVariable(  ref xrConfigInfo info) {
            return xrGetConfigVariable(xrSession, ref info);
        }


        public static bool XvFeatureCameraDistance(double distance)
        {
            return xrCameraDistance(
                xrSession,
              distance);
        }
        /*
        public static bool XvFeatureStartRgbStreaming(int solution,bool isStart)
        {
            return xrStartStreaming(
                xrSession,
              solution, isStart);
        }
        */
        public static bool XvFeatureGetRgbStream(System.IntPtr data, int width, int height, int type, ref double timestamp)
        {
            return xrGetRgbStream(
                xrSession,
              data, width, height, type, ref timestamp);
        }

        public static bool XvFeatureGetCalibrationData(int type, ref XrCalibrationXV data) {
            return xrGetCalibrationData( xrSession, type, ref data);
        }

        public static bool XvFeatureStartObjectTracking(string modelPath, string descriptorPath)
        {
            return xrStartObjectTracking(
                xrSession,
              modelPath, descriptorPath);
        }

        public static bool XvFeatureGetObjectTrackingData(ref int objNum, ref ObjectData objectDatas)
        {
            return xrGetObjectTrackingData(
                xrSession,
             ref objNum, ref objectDatas);
        }
        public static bool XvFeatureStartDetectPlane(int sourceType)
        {
            return xrStartDetectPlane(
                xrSession,
              sourceType);
        }
        public static bool XvFeatureGetPlaneData(out  IntPtr objectDatas, ref int objNum)
        {
            return xrGetPlaneData(
                xrSession,
             out objectDatas, ref objNum);
        }
        public static bool XvFeatureStartSlamMap()
        {
            return xrStartSlamMap(
                xrSession);
        }

        public static bool XvFeatureLoadMapAndSwitchToCslam(string mapPath)
        {
            return xrLoadMapAndSwitchToCslam(
                xrSession, mapPath);
        }

        public static bool XvFeatureSaveMapAndSwitchToCslam(string mapPath)
        {
            return xrSaveMapAndSwitchToCslam(
                xrSession, mapPath);
        }

        public static bool XvFeatureGetCslamCallbackData(ref XrCSlamCallbackDataXV xrCSlamCallbackDataXV) {
            return xrGetCslamCallbackData(
                 xrSession,ref xrCSlamCallbackDataXV);
        }
        public static bool XvFeatureStartGetSurface()
        {
            return xrStartGetSurface(
                xrSession);
        }
        public static bool XvFeatureGetSurfaceData(out IntPtr surfaceDatas, ref int surfaceCount)
        {
            return xrGetSurfaceData(
                xrSession,
             out surfaceDatas, ref surfaceCount);
        }


        #region wayland
        /// <summary>
        /// 打开 rgb camera
        /// </summary>
        /// <returns></returns>
        public static bool XvFeatureStartRgbStreaming()
        {
            return xrStartStreaming(
                xrSession,
                0, 0, true);
        }
        /// <summary>
        /// 关闭rgb camera
        /// </summary>
        /// <returns></returns>
        public static bool XvFeatureStopRgbStreaming()
        {
            return xrStartStreaming(
                xrSession,
                0, 0, false);
        }
        /// <summary>
        /// open tof
        /// </summary>
        /// <returns></returns>
        public static bool XvFeatureStartTofStreaming()
        {
            return xrStartStreaming(
                xrSession,
                1, 0, true);
        }
        /// <summary>
        /// close tof
        /// </summary>
        /// <returns></returns>
        public static bool XvFeatureStopTofStreaming()
        {
            return xrStartStreaming(
                xrSession,
                1, 0, false);
        }
        /// <summary>
        /// 打开鱼眼
        /// </summary>
        /// <returns></returns>
        public static bool XvFeatureStartStereoStreaming()
        {
            return xrStartStreaming(
                xrSession,
                2, 0, true);
        }
        /// <summary>
        /// close fisheye
        /// </summary>
        /// <returns></returns>
        public static bool XvFeatureStopStereoStreaming()
        {
            return xrStartStreaming(
                xrSession,
                2, 0, false);
        }
        /// <summary>
        /// get rgb size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool XvFeatureGetRgbImageSize(ref int width, ref int height)
        {
            return xrGetDeviceCameraImageSize(
                xrSession,
                0, ref width, ref height);
        }
        /// <summary>
        /// get tof image size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool XvFeatureGetTofImageSize(ref int width, ref int height)
        {
            return xrGetDeviceCameraImageSize(
                xrSession,
                1, ref width, ref height);
        }
        /// <summary>
        /// get Stereo size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool XvFeatureGetStereoImageSize(ref int width, ref int height)
        {
            return xrGetDeviceCameraImageSize(
                xrSession,
                2, ref width, ref height);
        }
        /// <summary>
        /// get rgb image
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static bool XvFeatureGetRgbImageData(IntPtr data, int width, int height, ref double timestamp)
        {
            return xrGetDeviceCameraImageData(
                xrSession,
               data, 0, width, height, ref timestamp);
        }
        /// <summary>
        /// get tof image
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static bool XvFeatureGetTofImageData(IntPtr data, int width, int height, ref double timestamp)
        {
            return xrGetDeviceCameraImageData(
                xrSession,
               data, 1, width, height, ref timestamp);
        }
        /// <summary>
        /// get fisheye data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static bool XvFeatureGetStereomageLeftData(IntPtr data, int width, int height, ref double timestamp)
        {
            return xrGetDeviceCameraImageData(
                xrSession,
               data, 2, width, height, ref timestamp);
        }
        public static bool XvFeatureGetStereomageRightData(IntPtr data, int width, int height, ref double timestamp)
        {
            return xrGetDeviceCameraImageData(
                xrSession,
               data, 3, width, height, ref timestamp);
        }


        #endregion


        // Dll imports

        private const string ExtLib = "XvFeature";

        /// <summary>
        /// Set up function pointers for xrSetInputDevice... functions.
        /// </summary>
        /// <param name="xrGetInstanceProcAddr">This is an IntPtr to the current OpenXR process address.</param>
        /// <param name="xrInstance">This is a ulong handle for the current OpenXR xrInstance.</param>
        [DllImport(ExtLib, EntryPoint = "script_initialize")]
        private static extern void initialize(IntPtr xrGetInstanceProcAddr, ulong xrInstance);

        [DllImport(ExtLib, EntryPoint = "script_xrSetInputDeviceActiveEXT")]
        private static extern bool xrSetInputDeviceActiveEXT(ulong xrSession, ulong interactionProfile, ulong topLevelPath, bool isActive);

        [DllImport(ExtLib, EntryPoint = "script_xrGetHandTrackingData")]
        private static extern bool xrGetHandTrackingData(ulong xrSession, ref XvXRSkeleton skeleton);

        [DllImport(ExtLib, EntryPoint = "script_xrCameraDistance")]
        private static extern bool xrCameraDistance(ulong xrSession, double distance);



        [DllImport(ExtLib, EntryPoint = "script_xrGetUpdateVariable")]
        private static extern bool xrUpdateVariable(ulong xrSession, ref xrConfigInfo info);

        [DllImport(ExtLib, EntryPoint = "script_xrGetConfigVariable")]
        private static extern bool xrGetConfigVariable(ulong xrSession, ref xrConfigInfo info);





        /**
        * type = 0 ：rgb ； type = 1 ： display 
        * @param[in] brightness brightness , [0,255]
        */
        [DllImport(ExtLib, EntryPoint = "script_xrDeviceSetBrightness")]
        private static extern bool xrDeviceSetBrightness(ulong xrSession, int type, int brightness);

        /*start streaming type 0 rgb 1 tof 2 stereo  soluton is rgb resoluton   isStart true is able  false is enable*/
        [DllImport(ExtLib, EntryPoint = "script_xrStartRgbStreaming")]
        private static extern bool xrStartStreaming(ulong xrSession, int type, int solution, bool isStart);

        [DllImport(ExtLib, EntryPoint = "script_xrGetRgbStream")]
        private static extern bool xrGetRgbStream(ulong xrSession, System.IntPtr data, int width, int height, int type, ref double timestamp);

        [StructLayout(LayoutKind.Sequential)]
       public struct XrCalibrationXV {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public double[] ex_rotation;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public double[] ex_trans;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public double[] K;
       
        
        }

        // Get RGB  GetCalibrationData

        [DllImport(ExtLib, EntryPoint = "script_xrGetCalibrationData")]
        private static extern bool xrGetCalibrationData(ulong xrSession, int  type, ref XrCalibrationXV data);

        [DllImport(ExtLib, EntryPoint = "script_xrStartObjectTracking")]
        private static extern bool xrStartObjectTracking(ulong xrSession, string modelPath, string descriptorPath);
        [DllImport(ExtLib, EntryPoint = "script_xrGetObjectTrackingData")]
        private static extern bool xrGetObjectTrackingData(ulong xrSession, ref int objNum, ref ObjectData objectDatas);


        [DllImport(ExtLib, EntryPoint = "script_xrStartDetectPlane")]
        private static extern bool xrStartDetectPlane(ulong xrSession, int sourceType);
        [DllImport(ExtLib, EntryPoint = "script_xrGetPlaneData")]
        private static extern bool xrGetPlaneData(ulong xrSession,out   IntPtr objectDatas, ref int objNum);



        public struct XrCSlamCallbackDataXV
        {
            public uint map_quality;
            public float percent;
            public uint status_of_saved_map;
        }
     

        [DllImport(ExtLib, EntryPoint = "script_xrStartSlamMap")]
        private static extern bool xrStartSlamMap(ulong xrSession);

        [DllImport(ExtLib, EntryPoint = "script_xrLoadMapAndSwitchToCslam")]
        private static extern bool xrLoadMapAndSwitchToCslam(ulong xrSession, string mapPath);
        [DllImport(ExtLib, EntryPoint = "script_xrSaveMapAndSwitchToCslam")]
        private static extern bool xrSaveMapAndSwitchToCslam(ulong xrSession, string mapPath);

        [DllImport(ExtLib, EntryPoint = "script_xrGetCslamCallbackData")]
        private static extern bool xrGetCslamCallbackData(ulong XrSession ,ref XrCSlamCallbackDataXV  data);



        [DllImport(ExtLib, EntryPoint = "script_xrStartGetSurface")]
        private static extern bool xrStartGetSurface(ulong xrSession);

        [DllImport(ExtLib, EntryPoint = "script_xrGetSurfaceData")]
        private static extern bool xrGetSurfaceData(ulong xrSession, out IntPtr surfaceDatas, ref int surfaceCount);

        [DllImport(ExtLib, EntryPoint = "script_xrGetDeviceCameraImageSize")]
        private static extern bool xrGetDeviceCameraImageSize(ulong xrSession, int type, ref int width, ref int height);

        [DllImport(ExtLib, EntryPoint = "script_xrGetDeviceCameraImageData")]
        private static extern bool xrGetDeviceCameraImageData(ulong xrSession, System.IntPtr data, int type, int width, int height, ref double timestamp);
        public struct Vector4F
        {
            public float x;
            public float y;
            public float z;
            public float w;
        };

        public struct Vector3F
        {
            public float x;
            public float y;
            public float z;
        };

        // ** AprilTag **
        [StructLayout(LayoutKind.Sequential)]
        public struct DetectData
        {
            public int tagID;
            public Vector3F position;
            public Vector3F orientation;
            public Vector4F quaternion;
            public long edgeTimestamp;
            public double hostTimestamp;
            public float confidence;
        };

        public struct TagData
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public DetectData[] detect;
        };

        [DllImport(ExtLib, EntryPoint = "script_xrDetectTags")]
        private static extern bool xrDetectTags(ulong xrSession, string tagFamily, double size, ref xrTagArrayXV tagsArray, ref int tagSize, int arraySize, int type);
        [DllImport(ExtLib, EntryPoint = "script_xrStopDetectTags")]
        private static extern bool xrStopDetectTags(ulong xrSession, string tagFamily, int type);




        #region eyetracking



        [StructLayout(LayoutKind.Sequential)]

        public struct xrVector2f
        {
            public float x;
            public float y;
        };
        [StructLayout(LayoutKind.Sequential)]

        public struct XrGazePupilINFOXV
        {
            public uint pupilBitMask;
            public xrVector2f pupilCenter;
            public float pupilDistance;
            public float pupilDiameter;
            public float pupilDiameterMM;
            public float pupilMinorAxis;
            public float pupilMinorAxisMM;
        };
        [StructLayout(LayoutKind.Sequential)]

        public struct XrGazeEyeExDataXV
        {
            public uint eyeDataExBitMask;
            public int blink;
            public float openness;
            public float eyelidUp;
            public float eyelidDown;
        };


        [StructLayout(LayoutKind.Sequential)]

        public struct XrGazePointXV
        {
            public uint gazeBitMask;
            public Vector3d gazePoint;
            public Vector3d rawPoint;
            public Vector3d smoothPoint;
            public Vector3d gazeOrigin;
            public Vector3d gazeDirection;
            public float re;
            public uint exDataBitMask;

        };

        [StructLayout(LayoutKind.Sequential)]

        public struct xrEtEyeDataEx
        {
            public ulong timestamp;
            public int recommend;
            public XrGazePointXV recomGaze;
            public XrGazePointXV leftGaze;
            public XrGazePointXV rightGaze;

            public XrGazePupilINFOXV leftPupil;
            public XrGazePupilINFOXV rightPupil;

            public XrGazeEyeExDataXV leftExData;
            public XrGazeEyeExDataXV rightExData;
            public float ipd;
            public int leftEyeMove;
            public int rightEyeMove;
        };



        [StructLayout(LayoutKind.Sequential)]

        public struct xrCtransform
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.R8)]
            public double[] rotation;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R8)]
            public double[] translation;
        };
        [StructLayout(LayoutKind.Sequential)]

        public struct xrPdm
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = UnmanagedType.R8)]
            public double[] rotation;
        }

        public struct xrPdmCalibration
        {
            public xrCtransform extrinsic;
            public xrPdm intrinsic;
        };
        public struct xrStereoPdmCalibration
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.Struct)]
            public xrPdmCalibration[] calibrations;
        };




        //设置眼控校准文件路径
        [DllImport(ExtLib, EntryPoint = "script_xrGazeSetConfigPath")]
        private static extern bool xrGazeSetConfigPath(ulong xrSession, string path);
        [DllImport(ExtLib, EntryPoint = "script_xrStartGaze")]

        //开启眼控追踪
        private static extern bool xrStartGaze(ulong xrSession, ref int ret);

        [DllImport(ExtLib, EntryPoint = "script_xrStopGaze")]

        //停止眼控追踪
        private static extern bool xrStopGaze(ulong xrSession, ref int ret);

        //设置亮度

        [DllImport(ExtLib, EntryPoint = "script_xrSetBright")]
        private static extern bool xrSetBright(ulong xrSession, int eye, int led, int brightness, ref int ret);


        //设置曝光
        [DllImport(ExtLib, EntryPoint = "script_xrSetExposure")]

        private static extern bool xrSetExposure(ulong xrSession, int leftGain,float leftTimeMs, int rightGain, float rightTimeMs, ref int ret);

        //获取眼动数据

        [DllImport(ExtLib, EntryPoint = "script_xrGetGazeCallback")]
        private static extern bool xrGetGazeCallback(ulong xrSession, ref xrEtEyeDataEx data);

        //取消眼动数据回调
        [DllImport(ExtLib, EntryPoint = "script_xrUnsetGazeCallback")]
        private static extern bool xrUnsetGazeCallback(ulong xrSession, ref int ret);

        //设置校准文件
        [DllImport(ExtLib, EntryPoint = "script_xrGazeCalirationApply")]
        private static extern bool xrGazeCalirationApply(ulong xrSession, string file, ref int ret);



        //获取双目信息
        [DllImport(ExtLib, EntryPoint = "script_xrReadStereoDisplayCalibration")]
        private static extern bool xrReadStereoDisplayCalibration(ulong xrSession, ref xrStereoPdmCalibration calibration);
        public static bool XvFeatureReadStereoDisplayCalibration(ref xrStereoPdmCalibration calibration)
        {
            return xrReadStereoDisplayCalibration(
                xrSession,
                ref calibration);
        }


        public static bool XvFeatureGazeSetConfigPath(string path)
        {
            return xrGazeSetConfigPath(
                xrSession, path);
        }

       

        public static bool XvFeatureStartGaze(ref int ret)
        {
            return xrStartGaze(
                xrSession,
                ref ret);
        }


        public static bool XvFeatureStopGaze(ref int ret)
        {
            return xrStopGaze(
                xrSession,
                ref ret);
        }

        public static bool XvFeatureSetBright(int eye, int led, int brightness, ref int ret)
        {
            return xrSetBright(
                xrSession,
                eye,
                led,
                brightness,
                ref ret);
        }
        public static bool XvFeatureSetExposure(int leftGain, float leftTimeMs, int rightGain, float rightTimeMs, ref int ret)
        {
            return xrSetExposure(
                xrSession, leftGain, leftTimeMs, rightGain, rightTimeMs,ref ret);
        }

        public static bool XvFeatureGetGazeCallback(ref xrEtEyeDataEx data)
        {
            return xrGetGazeCallback(
                xrSession,
                ref data);
        }

        public static bool XvFeatureUnsetGazeCallback(ref int ret)
        {
            return xrUnsetGazeCallback(
                xrSession, ref ret);
        }


        public static bool XvFeatureGazeCalirationApply(string file, ref int ret)
        {
            return xrGazeCalirationApply(
                xrSession,
                file,
                ref ret);
        }




        #endregion

        public struct XrControllerXV { 
            public uint type;
            public Vector3d position;
            public RotatePoint quaternion;
            public float confidence;
            public uint keyTrigger;
            public uint keySide;
            public uint rocker_x;
            public uint rocker_y;
            public uint keyA;
            public uint keyB;
        }

        [DllImport(ExtLib, EntryPoint = "script_xrGetControllerData")]
        private static extern bool xrGetControllerData(ulong xrSession,int type,ref XrControllerXV xrControllerXV);

        public static bool XvFeatureGetControllerData(ulong xrSession, int type,ref XrControllerXV xrControllerXV)
        {
            Debug.Log("xrDetectTags");

            try
            {
                return xrGetControllerData(
                    xrSession,
                    type , ref xrControllerXV);
            }
            catch (Exception ex)
            {
                Debug.Log("xrDetectTagsException" + ex.Message);
                return false;
            }
        }

        public static bool XvFeatureCheckApriltag(string tagFamily, double size, ref xrTagArrayXV tagsArray, ref int tagSize, int arraySize, int type)
        {
            Debug.Log("xrDetectTags");

            try {

                return xrDetectTags(
                    xrSession,
                    tagFamily, size, ref tagsArray, ref tagSize, arraySize, type);
            }
            catch (Exception ex) {
                Debug.Log("xrDetectTagsException" + ex.Message);
                return false;
            }
        }

        public static bool XvFeatureStopDetectTags(string tagFamily, int type) {
            return xrStopDetectTags(xrSession,  tagFamily,  type);
        }
    }
}

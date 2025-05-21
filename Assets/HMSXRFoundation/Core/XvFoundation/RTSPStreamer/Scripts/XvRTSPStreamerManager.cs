using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HMSXR.Foundation
{
    /// <summary>
    /// This class is responsible for streaming mixed reality first-person view (FPV) via RTSP, which can be viewed using RTSP streaming software.
    /// RTSP URL: rtsp://{ip}:8554/stream/            Example: rtsp://192.168.32.98:8554/stream/  
    /// </summary>
    public sealed class XvRTSPStreamerManager : MonoBehaviour
    {
        private XvRTSPStreamerManager() { }


        [SerializeField]
        private XvMRVideoCaptureManager xvMRVideoCaptureManager;
        public XvMRVideoCaptureManager XvMRVideoCaptureManager
        {
            get
            {
                if (xvMRVideoCaptureManager == null)
                {
                    xvMRVideoCaptureManager = FindObjectOfType<XvMRVideoCaptureManager>();
                }

                if (xvMRVideoCaptureManager == null)
                {
                    GameObject newObj = Instantiate(Resources.Load<GameObject>("XvMRVideoCaptureManager"));
                    xvMRVideoCaptureManager = newObj.GetComponent<XvMRVideoCaptureManager>();
                    newObj.name = "XvMRVideoCaptureManager";
                }

                return xvMRVideoCaptureManager;
            }
        }

        public bool autoPush;
        private bool isStreeaming;
        public bool IsStreeaming
        {
            get { return isStreeaming; }
        }
        private static int renderInit = 0x2;
        private static int renderDraw = 0x4;



        protected const string dllName = "WifiDisplayPlugin";
        [DllImport(dllName)]
        private static extern void SetCameraTextureFromUnity(System.IntPtr texture, int width, int height);

        [DllImport(dllName)]
        private static extern IntPtr GetRenderEventFunc();



        private AndroidJavaObject interfaceObject;


        private RenderTexture wifiCameraRenderTexture = null;

        private AndroidJavaObject InterfaceObject
        {
            get
            {
                if (interfaceObject == null)
                {
                    AndroidJavaClass activityClass = AndroidHelper.GetClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject activityObject = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                    if (activityObject != null)
                    {
                        AndroidJavaClass interfaceClass = AndroidHelper.GetClass("com.xv.wifidisply.UnityInterface");

                        interfaceObject = interfaceClass.CallStatic<AndroidJavaObject>("getInstance", new object[] { activityObject });
                    }
                }
                return interfaceObject;
            }
        }


        IEnumerator Start()
        {

            if (Application.platform == RuntimePlatform.Android)
            {
                MyDebugTool.LogError("nativeInit start.....");
                AndroidHelper.CallObjectMethod(InterfaceObject, "nativeInit", new object[] { });
                MyDebugTool.LogError("nativeInit end.....");
                CreateTextureAndPassToPlugin();
                yield return StartCoroutine("CallPluginAtEndOfFrames");
            }
        }

        private void OnEnable()
        {
            if (autoPush)
            {
                CancelInvoke();
                Invoke("StartRtspStreaming", 3);
            }
        }

        private void OnDisable()
        {
            CancelInvoke();
            StopRtspStreaming();
        }
        private void CreateTextureAndPassToPlugin()
        {
            wifiCameraRenderTexture = XvMRVideoCaptureManager.CameraRenderTexture;
            SetCameraTextureFromUnity(wifiCameraRenderTexture.GetNativeTexturePtr(), wifiCameraRenderTexture.width, wifiCameraRenderTexture.height);
            GL.IssuePluginEvent(GetRenderEventFunc(), renderInit);
        }

        private IEnumerator CallPluginAtEndOfFrames()
        {
            while (true)
            {
                // Wait until all frame rendering is done
                yield return new WaitForEndOfFrame();
                GL.IssuePluginEvent(GetRenderEventFunc(), renderDraw);
                // yield return new WaitForEndOfFrame ();

            }
        }



        /// <summary>
        /// Start RTSP streaming
        /// </summary>
        public void StartRtspStreaming()
        {
            if (isStreeaming)
            {
                MyDebugTool.LogWarning("Currently streaming...");
                return;
            }
            isStreeaming = true;
            XvMRVideoCaptureManager.StartCapture();
            OnPcDisplayClick();
        }

        /// <summary>
        /// Stop RTSP streaming
        /// </summary>
        public void StopRtspStreaming()
        {
            if (isStreeaming)
            {
                isStreeaming = false;
                OnTvStopClick();
            }
        }

        private void OnPcDisplayClick()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                MyDebugTool.LogError("OnPcDisplayClick");
                AndroidHelper.CallObjectMethod(InterfaceObject, "setUseDLNA", new object[] { false });//true: Use PC mode (direct via VLC); false: Use TV mode (requires hotspot connection)
                MyDebugTool.LogError("setUseDLNA");

                AndroidHelper.CallObjectMethod(InterfaceObject, "tvDisplayClicked", new object[] { });
                MyDebugTool.LogError("tvDisplayClicked");


            }
        }



        private void OnTvStopClick()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallObjectMethod(InterfaceObject, "tvStopClicked", new object[] { });

            }
        }



        private void OnTvDisplayClick()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidHelper.CallObjectMethod(InterfaceObject, "setUseDLNA", new object[] { true });//false: Mobile hotspot mode (requires manual hotspot setup)
                AndroidHelper.CallObjectMethod(InterfaceObject, "tvDisplayClicked", new object[] { });

            }
        }
    }
}
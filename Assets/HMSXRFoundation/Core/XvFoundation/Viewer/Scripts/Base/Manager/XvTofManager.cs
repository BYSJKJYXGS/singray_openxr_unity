

using HMS.Spaces;
using static HMSXR.Foundation.XvCameraBase;

namespace HMSXR.Foundation
{
    /// <summary>
    /// ToF image
    /// </summary>
    public class XvTofDepth
    {


        private XvCameraBase frameBase;
       
        public bool IsOn { get; private set; }
        public void StartCapture(int requestedWidth, int requestedHeight, int requestedFPS)
        {
#if UNITY_EDITOR
            return;
#endif
            if (IsOn)
            {
                return;
            }

           // StopCapture();

            if (frameBase==null) { 
            
            frameBase = new XvTofCamera(requestedWidth, requestedHeight, requestedFPS, FrameArrived);
            }
            IsOn = true;

            if (XvTofManager.GetXvTofManager().modelSet == false)
            {
                XvTofManager.GetXvTofManager().SetTofStreamMode(0);
            }
            XvTofManager.GetXvTofManager().StartTofStream();
            frameBase.StartCapture();
        }

        public void StopCapture()
        {
#if UNITY_EDITOR
            return;
#endif
            if (frameBase != null && frameBase.IsOpen)
            {
                frameBase.StopCapture();
                XvTofManager.GetXvTofManager().StopTofStream();
            }

            frameBase = null;
            // GC.Collect();

            IsOn = false;
        }

        private void FrameArrived(cameraData cameraData)
        {
            XvCameraManager.onTofDepthCameraStreamFrameArrived?.Invoke(cameraData);
        }

        public void Update() {
            frameBase?.Update();
        }

    }
    /// <summary>
    /// IR Image
    /// </summary>
    public class XvTofIR {


        private XvCameraBase frameBase;

        public bool IsOn
        {
            get;
            private set;
        }
        public void StartCapture(int requestedWidth, int requestedHeight, int requestedFPS)
        {
            MyDebugTool.Log("StartIRCapture == 1:");
#if UNITY_EDITOR
            return;
#endif
            if (IsOn)
            {
                return;
            }
            MyDebugTool.Log("StartIRCapture");
           // StopCapture();

            if (frameBase==null) { 
            
            frameBase = new XvTofIRCamera(requestedWidth, requestedHeight, requestedFPS, FrameArrived);
            }
            IsOn = true;

            if (XvTofManager.GetXvTofManager().modelSet == false)
            {
                XvTofManager.GetXvTofManager().SetTofStreamMode(0);
            }

            XvTofManager.GetXvTofManager().StartTofIRStream();
            frameBase.StartCapture();
        }

        public void StopCapture()
        {
#if UNITY_EDITOR
            return;
#endif
            if (frameBase != null && frameBase.IsOpen)
            {
                frameBase.StopCapture();
                XvTofManager.GetXvTofManager(). StopTofStream();
            }

            frameBase = null;
            // GC.Collect();

            IsOn = false;
        }

        private void FrameArrived(cameraData cameraData)
        {
            XvCameraManager.onTofIRCameraStreamFrameArrived?.Invoke(cameraData);
        }


        public void Update()
        {
            frameBase?.Update();
        }
    }

  
    #region tof
    public sealed class XvTofManager
    {

        private static XvTofManager xvTofManager;

        public static XvTofManager GetXvTofManager()
        {
            if (xvTofManager == null)
            {
                xvTofManager = new XvTofManager();

            }
            return xvTofManager;
        }

        private XvTofManager() { }
        public bool modelSet = false;

        private XvTofDepth xvTofDepth = null;
        private XvTofIR xvTofIR = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tofImageType">0: deapth  1:IR </param>
        public void StartCapture(int requestedWidth, int requestedHeight, int requestedFPS, int tofImageType)
        {
            MyDebugTool.Log("StartCapture:"+ tofImageType);
            if (tofImageType==0) {
                if (xvTofDepth == null)
                {
                    xvTofDepth = new XvTofDepth();
                }
                xvTofDepth.StartCapture(requestedWidth, requestedHeight, requestedFPS);
            }
            else

            if (tofImageType == 1)
            {
                if (xvTofIR == null)
                {
                    xvTofIR = new XvTofIR();
                }
                MyDebugTool.Log("tofImageType == 1:" + tofImageType);

                xvTofIR.StartCapture(requestedWidth, requestedHeight, requestedFPS);
            }


        }

        public void StopCapture(int tofImageType) {
            if (tofImageType == 0)
            {
                if (xvTofDepth != null)
                {
                    xvTofDepth.StopCapture();

                }
            }
            else

                if (tofImageType == 1)
            {
                if (xvTofIR != null)
                {
                    xvTofIR.StopCapture();

                }
            }
        }

        public bool IsOn(int tofImageType) {
            if (tofImageType == 0)
            {
                if (xvTofDepth != null)
                {
                  return  xvTofDepth.IsOn;

                }
            }
            else

                   if (tofImageType == 1)
            {
                if (xvTofIR != null)
                {
                   return xvTofIR.IsOn;

                }
            }

            return false;
        }

        public void StartTofStream()
        {
            XvFeature.XvFeatureStartTofStreaming();
        }

        public void StartTofIRStream()
        {
            XvFeature.XvFeatureStartTofStreaming();
        }

      public void SetTofStreamMode(int mode) {
            XvFeature.XvFeatureStopTofStreaming();
           // XvFeature.XvFeatureStopTofStreaming(mode);

            modelSet = true;
        }

        public void StopTofStream()
        {
            XvFeature.XvFeatureStopTofStreaming();
        }





        public void Update()
        {
            
#if UNITY_EDITOR
            return;
#endif
            if (xvTofDepth!=null)
            {
                xvTofDepth.Update();

            }
            if (xvTofIR!=null)
            {

                xvTofIR.Update();
            }

        }
    }

    #endregion
}
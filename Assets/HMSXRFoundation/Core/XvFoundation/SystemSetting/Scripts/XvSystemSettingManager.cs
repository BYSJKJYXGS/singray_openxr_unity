using UnityEngine;
using HMS.Spaces;
using static HMS.Spaces.XvFeature;

namespace HMSXRFoundation
{
    /// <summary>
    /// This class provides methods for setting and retrieving system-related parameters
    /// </summary>
    public sealed class XvSystemSettingManager : MonoBehaviour
    {
        private XvSystemSettingManager() { }

        /// <summary>
        /// Get the current brightness level
        /// </summary>
        /// <returns></returns>
        public int GetBrightnessLevel()
        {
            xrConfigInfo xrConfigInfo = new xrConfigInfo();
            xrConfigInfo.name = "gGlassLight";

            if (XvFeature.XvFeatureGetConfigVariable(ref xrConfigInfo))
            {
                return (int)float.Parse(xrConfigInfo.value);
            }

            return 100;
        }

        /// <summary>
        /// Set the glasses brightness level
        /// </summary>
        /// <param name="level">1~9</param>
        public void SetBrightnessLevel(int level)
        {
#if UNITY_EDITOR
            return;
#endif
            XvFeature.XvFeatureRgbSetBrightness(1, level);
        }

        /// <summary>
        /// Set the current interpupillary distance (IPD)
        /// </summary>
        /// <param name="ipd">55mm~75mm</param>
        public void SetIPD(float ipd)
        {
#if UNITY_EDITOR
            return;
#endif

            xrConfigInfo xrConfigInfo = new xrConfigInfo();
            xrConfigInfo.name = "gGlassIpd";
            xrConfigInfo.value = ((int)ipd).ToString();

            MyDebugTool.Log("wuxh:" + xrConfigInfo.name + ":" + xrConfigInfo.value);
            XvFeature.XvFeatureUpdateVariable(ref xrConfigInfo);
        }

        /// <summary>
        /// Get the current IPD
        /// </summary>
        /// <returns> Return value is in mm (millimeters)</returns>
        public float GetIPD()
        {
#if UNITY_EDITOR
            return 0;
#endif
            xrConfigInfo xrConfigInfo = new xrConfigInfo();
            xrConfigInfo.name = "gGlassIpd";

            if (XvFeature.XvFeatureGetConfigVariable(ref xrConfigInfo))
            {
                return float.Parse(xrConfigInfo.value);
            }

            return 63;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HMSXRFoundation
{
    /// <summary>
    /// Key = 2, State = 0: Glasses removed state
    /// Key = 2, State = 1: Glasses worn state
    /// Key = 6, State = 0: Light sensor
    /// Key = 14, 1, 13, 3, State = 254: Pressed down, 255: Released
    /// Key = 17, 18, State = 101: Rotate +, 99: Rotate -
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XvEvent
    {
        public double hostTimestamp;
        public long edgeTimestampUs;
        public int type;
        public int state;
    };

    public class XvSystemSetting
    {
        /// <summary>
        /// Set glasses brightness level
        /// </summary>
        /// <param name="level">0~9 brightness levels</param>
        [DllImport("xslam-unity-wrapper")]
        public static extern void xslam_display_set_brightnesslevel(int level); // level represents brightness level
    }
}
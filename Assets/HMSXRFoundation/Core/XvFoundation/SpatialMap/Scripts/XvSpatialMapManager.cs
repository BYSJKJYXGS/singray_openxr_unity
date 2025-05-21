using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using HMS.Spaces;
using static HMS.Spaces.XvFeature;

namespace HMSXR.Foundation
{
    /// <summary>
    /// This class implements CSlam functionality, allowing spatial map scanning to generate map files for multi-person collaboration and other features via map files.
    /// </summary>
    public sealed class XvSpatialMapManager : MonoBehaviour
    {
        private XvSpatialMapManager() { }
        private bool loadMap;


        private bool startSlam;

        /// <summary>
        /// First parameter: Map save status
        /// Second parameter: Map quality
        /// </summary>
        public static UnityEvent<int, int> onMapSaveCompleteEvent = new UnityEvent<int, int>();

        /// <summary>
        /// First parameter: Map quality
        /// </summary>
        public static UnityEvent<int> onMapLoadCompleteEvent = new UnityEvent<int>();

        public static UnityEvent<float> onMapMatchingEvent = new UnityEvent<float>();



        /// <summary>
        /// Start map scanning
        /// </summary>
        public void StartSlamMap()
        {
#if UNITY_EDITOR
            return;
#endif

            if (startSlam)
            {
                StopSlamMap();
            }

            if (!startSlam)
            {
                startSlam = XvFeature.XvFeatureStartSlamMap();
            }
        }

        /// <summary>
        /// Save the scanned map
        /// </summary>
        /// <returns>Map file path</returns>
        public string SaveSlamMap()
        {
            if (startSlam)
            {
                string cslamName = GetNowStamp() + "_map.bin";
                string mapPath = Application.persistentDataPath + "/" + cslamName;
                XvFeature.XvFeatureSaveMapAndSwitchToCslam(mapPath);
                return mapPath;
            }
            else
            {
                MyDebugTool.Log("SLAM not started");
            }

            return null;
        }

        /// <summary>
        /// Load an existing map
        /// </summary>
        /// <param name="mapPath">Map file path</param>
        public void LoadSlamMap(string mapPath)
        {
            if (!startSlam)
            {
                StartSlamMap();
            }

            if (startSlam)
            {
                loadMap = true;
                XvFeature.XvFeatureLoadMapAndSwitchToCslam(mapPath);
            }
        }

        /// <summary>
        /// Stop map scanning functionality
        /// </summary>
        public void StopSlamMap()
        {
            if (startSlam)
            {
                //todo
            }
            startSlam = false;
        }

        void Update()
        {
            if (loadMap)
            {
                XrCSlamCallbackDataXV xrCSlamCallbackDataXV = default(XrCSlamCallbackDataXV);

                if (XvFeatureGetCslamCallbackData(ref xrCSlamCallbackDataXV))
                {
                    load_map_quality = (int)xrCSlamCallbackDataXV.map_quality;
                    onMapLoadCompleteEvent?.Invoke(load_map_quality);
                }
            }
        }

        private void OnDestroy()
        {
            StopSlamMap();
        }

        private void OnApplicationQuit()
        {
            StopSlamMap();
        }

        /// <summary>
        /// Not recommended for frequent calls; call at regular intervals
        /// </summary>
        /// <returns>Spatial coordinates of feature points</returns>
        public List<Vector3> GetFeaturePoint()
        {
            if (startSlam)
            {
                return null;
            }

            return null;
        }


        private static float similarity;
        private static int load_map_quality;



        /// <summary>
        /// Convert DateTime to timestamp
        /// </summary>
        /// <param name="time">Target time</param>
        /// <returns>Timestamp (long)</returns>
        public long ConvertDateTimeTotTmeStamp(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new System.DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            long t = (time.Ticks - startTime.Ticks) / 10000;  // Divide by 10000 to adjust to 13 digits
            return t;
        }

        /// <summary>
        /// Get the current timestamp
        /// </summary>
        /// <returns>Current timestamp</returns>
        public long GetNowStamp()
        {
            return ConvertDateTimeTotTmeStamp(DateTime.Now);
        }
    }
}
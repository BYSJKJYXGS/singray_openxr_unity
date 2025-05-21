using System;
using UnityEngine;
using static HMS.Spaces.XvFeature;
using HMS.Spaces;

namespace HMSXR.Foundation
{
    /// <summary>
    /// Recognition result
    /// </summary>
    public class TagDetection
    {
        public int id; // Tag ID
        public Vector3 translation; // Position
        public Vector3 rotation; // Rotation in Euler angles
        public Vector4 quaternion; // Rotation in quaternion
        public float confidence; // Recognition confidence, range 0-1
        public char[] qrcode; // QR code information
    }

    public class XvAprilTag
    {
        private static string tagFamily;

        /// <summary>
        /// Start fisheye camera detection mode for AprilTag
        /// </summary>
        /// <param name="tagFamily">
        /// For AprilTag detection, pass "36h11"
        /// For QRCode detection, pass "qr-code"
        /// </param>
        /// <param name="size">Physical size of the tag in meters</param>
        /// <returns></returns>
        public static TagDetection[] StartFishEyeDetector(string tagFamily, double size)
        {
            XvAprilTag.tagFamily = tagFamily;
            TagDetection[] result = null;
            int len = 1;
            XvFeature.xrTagArrayXV tags = default(XvFeature.xrTagArrayXV);

            try
            {
                bool act = XvFeature.XvFeatureCheckApriltag(tagFamily, size, ref tags, ref len, 64, 1);
                result = new TagDetection[len];

                if (act)
                {
                    Debug.Log("AprilTagDemo##StartDetect len:" + len + "  " + tags.detect.Length);
                    for (int i = 0; i < len; i++)
                    {
                        xrTagDataXV tag = tags.detect[i];

                        TagDetection detection = new TagDetection();
                        detection.id = (int)tag.tagID;
                        detection.translation = new Vector3(tag.position.x, tag.position.y, tag.position.z);
                        detection.rotation = new Vector3(tag.orientation.x, tag.orientation.y, tag.orientation.z);
                        detection.quaternion = new Vector4(tag.quaternion.x, tag.quaternion.y, tag.quaternion.z, tag.quaternion.w);
                        detection.confidence = tag.confidence;
                        result[i] = detection;

                        Debug.Log("AprilTag##StartDetector detection translation:(" + detection.translation.x + "," + detection.translation.y + "," + detection.translation.z + ")");
                        Debug.Log("AprilTag##StartDetector detection rotation:(" + detection.rotation.x + "," + detection.rotation.y + "," + detection.rotation.z + ")");
                        Debug.Log("AprilTag##StartDetector detection quaternion:(" + detection.quaternion.x + "," + detection.quaternion.y + "," + detection.quaternion.z + "," + detection.quaternion.w + ")");
                    }
                }
                else
                {
                    // No tags detected
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("AprilTag##" + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Stop fisheye camera detection mode for AprilTag
        /// </summary>
        public static void StopFishEyeDetector()
        {
            XvFeature.XvFeatureStopDetectTags(tagFamily, 1);
        }

        /// <summary>
        /// Start RGB camera detection mode for AprilTag
        /// </summary>
        /// <param name="tagFamily">
        /// For AprilTag detection, pass "36h11"
        /// For QRCode detection, pass "qr-code"
        /// </param>
        /// <param name="size">Physical size of the tag in meters</param>
        /// <returns></returns>
        public static TagDetection[] StartRgbDetector(string tagFamily, double size)
        {
            XvAprilTag.tagFamily = tagFamily;
            TagDetection[] result = null;
            int len = 1;
            XvFeature.xrTagArrayXV tags = default(XvFeature.xrTagArrayXV);

            try
            {
                bool act = XvFeature.XvFeatureCheckApriltag(tagFamily, size, ref tags, ref len, 64, 0);
                result = new TagDetection[len];

                if (act)
                {
                    Debug.Log("AprilTagDemo##StartDetect len:" + len + "  " + tags.detect.Length);
                    for (int i = 0; i < len; i++)
                    {
                        xrTagDataXV tag = tags.detect[i];

                        TagDetection detection = new TagDetection();
                        detection.id = (int)tag.tagID;
                        detection.translation = new Vector3(tag.position.x, tag.position.y, tag.position.z);
                        detection.rotation = new Vector3(tag.orientation.x, tag.orientation.y, tag.orientation.z);
                        detection.quaternion = new Vector4(tag.quaternion.x, tag.quaternion.y, tag.quaternion.z, tag.quaternion.w);
                        detection.confidence = tag.confidence;
                        result[i] = detection;

                        Debug.Log("AprilTag##StartDetector detection translation:(" + detection.translation.x + "," + detection.translation.y + "," + detection.translation.z + ")");
                        Debug.Log("AprilTag##StartDetector detection rotation:(" + detection.rotation.x + "," + detection.rotation.y + "," + detection.rotation.z + ")");
                        Debug.Log("AprilTag##StartDetector detection quaternion:(" + detection.quaternion.x + "," + detection.quaternion.y + "," + detection.quaternion.z + "," + detection.quaternion.w + ")");
                    }
                }
                else
                {
                    // No tags detected
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("AprilTag##" + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Stop RGB camera detection mode for AprilTag
        /// </summary>
        public static void StopRgbDetector()
        {
            XvFeature.XvFeatureStopDetectTags(tagFamily, 0);
        }
    }
}
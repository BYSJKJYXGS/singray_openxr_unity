using System.Collections;
using UnityEngine;


namespace HMSXR.Foundation
{
    /// <summary>
    /// Depends on the XvMediaRecorderManager class.
    /// This class mainly implements local video recording and screenshot functions via menu controls.
    /// </summary>
    public class XvMediaRecorder : MonoBehaviour
    {
        [SerializeField]
        private XvMediaRecorderManager xvMediaRecorderManager;

        public XvMediaRecorderManager XvMediaRecorderManager
        {
            get
            {
                if (xvMediaRecorderManager == null)
                {
                    xvMediaRecorderManager = FindFirstObjectByType<XvMediaRecorderManager>();
                    if (xvMediaRecorderManager == null)
                    {
                        GameObject newObj = new GameObject("XvMediaRecorderManager");
                        xvMediaRecorderManager = newObj.AddComponent<XvMediaRecorderManager>();
                    }
                }
                return xvMediaRecorderManager;
            }
        }


        [SerializeField]
        private XvCameraManager xvCameraManager;

        public XvCameraManager XvCameraManager
        {
            get
            {
                if (xvCameraManager == null)
                {
                    xvCameraManager = FindFirstObjectByType<XvCameraManager>();
                    if (xvCameraManager == null)
                    {
                        GameObject newObj = new GameObject("XvCameraManager");
                        xvCameraManager = newObj.AddComponent<XvCameraManager>();
                    }
                }
                return xvCameraManager;
            }
        }

        [SerializeField]
        private XvRTSPStreamerManager xvRTSPStreamerManager;

        public XvRTSPStreamerManager XvRTSPStreamerManager
        {
            get
            {
                if (xvRTSPStreamerManager == null)
                {
                    xvRTSPStreamerManager = FindFirstObjectByType<XvRTSPStreamerManager>();
                    if (xvRTSPStreamerManager == null)
                    {
                        GameObject newObj = new GameObject("XvRTSPStreamerManager");
                        xvRTSPStreamerManager = newObj.AddComponent<XvRTSPStreamerManager>();
                    }
                }
                return xvRTSPStreamerManager;
            }
        }

        //[SerializeField]
        private XvMedioRecordTips tips;

        private TextMesh RtspTips;
        private TextMesh VideoRecordTips;
        private TextMesh ScreenshotTips;
        private TextMesh cameraTips;


        private TextMesh RtspStreamingText;
        private TextMesh VideoRecordingText;
        private TextMesh cameraText;

        private void Awake()
        {
            RtspTips = transform.Find("HandMenu/MenuContent/ButtonCollection/RtspStreaming/IconAndText/RtspTips").GetComponent<TextMesh>();
            VideoRecordTips = transform.Find("HandMenu/MenuContent/ButtonCollection/VideoRecording/IconAndText/VideoRecordTips").GetComponent<TextMesh>();
            ScreenshotTips = transform.Find("HandMenu/MenuContent/ButtonCollection/SaveScreenshot/IconAndText/ScreenshotTips").GetComponent<TextMesh>();
            cameraTips = transform.Find("HandMenu/MenuContent/ButtonCollection/Camera/IconAndText/CameraTips").GetComponent<TextMesh>();
            RtspStreamingText = transform.Find("HandMenu/MenuContent/ButtonCollection/RtspStreaming/IconAndText/RtspStreamingText").GetComponent<TextMesh>();
            VideoRecordingText = transform.Find("HandMenu/MenuContent/ButtonCollection/VideoRecording/IconAndText/VideoRecordingText").GetComponent<TextMesh>();
            cameraText = transform.Find("HandMenu/MenuContent/ButtonCollection/Camera/IconAndText/TextMeshPro").GetComponent<TextMesh>();

            tips = transform.Find("HandMenu/MenuContent/XvMedioRecordTips").GetComponent<XvMedioRecordTips>();
        }


        private void OnEnable()
        {
            /// MR video capture must be enabled before recording
            XvMediaRecorderManager.StartCapture();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                SaveScreenshot();
            }

            if (Time.frameCount % 5 == 0)
            {
                if (XvCameraManager.IsOn(XvCameraStreamType.ARCameraStream))
                {
                    cameraText.text = "Turn Off Camera";
                }
                else
                {
                    cameraText.text = "Turn On Camera";
                }

                if (XvRTSPStreamerManager.IsStreeaming)
                {
                    RtspStreamingText.text = "Stop Streaming";
                }
                else
                {
                    RtspStreamingText.text = "Start Streaming";
                }

                if (xvMediaRecorderManager.IsVideoRecording())
                {
                    VideoRecordingText.text = "Stop Recording";
                }
                else
                {
                    VideoRecordingText.text = "Start Recording";
                }

                if (XvMediaRecorderManager.IsTakingScreenshots() || isScreenshot)
                {
                    MyDebugTool.LogError("Currently taking a screenshot...");
                    return;
                }

                ScreenshotTips.gameObject.SetActive(XvMediaRecorderManager.IsTakingScreenshots() || isScreenshot);
                RtspTips.gameObject.SetActive(XvRTSPStreamerManager.IsStreeaming);
                cameraTips.gameObject.SetActive(XvCameraManager.IsOn(XvCameraStreamType.ARCameraStream));
                VideoRecordTips.gameObject.SetActive(xvMediaRecorderManager.IsVideoRecording());
            }
        }


        private void OnDisable()
        {
            XvMediaRecorderManager.StopCapture(false);
        }

        public void ARCameraStreaming()
        {
            if (XvCameraManager.IsOn(XvCameraStreamType.ARCameraStream))
            {
                cameraTips.gameObject.SetActive(false);
                XvCameraManager.StopCapture(XvCameraStreamType.ARCameraStream);
                cameraText.text = "Turn On Camera";
            }
            else
            {
                cameraTips.gameObject.SetActive(true);
                cameraText.text = "Turn Off Camera";
                XvCameraManager.StartCapture(XvCameraStreamType.ARCameraStream);
            }
        }

        public void RtspStreaming()
        {
            if (XvRTSPStreamerManager.IsStreeaming)
            {
                RtspTips.gameObject.SetActive(false);
                XvRTSPStreamerManager.StopRtspStreaming();
                RtspStreamingText.text = "Start Streaming";
            }
            else
            {
                RtspTips.gameObject.SetActive(true);
                XvRTSPStreamerManager.StartRtspStreaming();
                RtspStreamingText.text = "Stop Streaming";
            }
        }


        /// <summary>
        /// Start recording local video
        /// </summary>
        public void VideoRecording()
        {
            if (xvMediaRecorderManager.IsVideoRecording())
            {
                VideoRecordTips.gameObject.SetActive(false);
                xvMediaRecorderManager.StopRecording((filePath) =>
                {
                    tips.ShowTips("Save Path: " + filePath, 2);
                });
                VideoRecordingText.text = "Start Recording";
            }
            else
            {
                VideoRecordTips.gameObject.SetActive(true);
                XvMediaRecorderManager.StartRecording();
                VideoRecordingText.text = "Stop Recording";
            }
        }

        private bool isScreenshot;

        /// <summary>
        /// Save screenshot
        /// </summary>
        public void SaveScreenshot()
        {
            if (XvMediaRecorderManager.IsTakingScreenshots() || isScreenshot)
            {
                MyDebugTool.LogError("Currently taking a screenshot...");
                return;
            }
            isScreenshot = true;
            ScreenshotTips.gameObject.SetActive(true);
            StartCoroutine(RelayScreenshot());
        }


        private IEnumerator RelayScreenshot()
        {
            yield return new WaitForSeconds(1);

            for (int i = 3; i >= 0; i--)
            {
                tips.ShowTips(i.ToString());
                yield return new WaitForSeconds(1f);
            }
            tips.HideTips();
            XvMediaRecorderManager.SaveScreenshot((filePath) => {
                tips.ShowTips("Save Path: " + filePath, 1);
                ScreenshotTips.gameObject.SetActive(false);
                isScreenshot = false;
            });
        }
    }
}
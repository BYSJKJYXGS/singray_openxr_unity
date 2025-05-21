using UnityEngine;
using UnityEngine.UI;

namespace HMSXR.Foundation
{
    /// <summary>
    /// This class handles mixed reality video capture and depends on the XvCameraManager class.
    /// Ensure the camera is turned on before enabling mixed reality capture.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class XvMRVideoCaptureManager : MonoBehaviour
    {
        private XvMRVideoCaptureManager() { }
        [SerializeField]
        private XvCameraManager cameraManager;

        public XvCameraManager CameraManager
        {
            get
            {
                if (cameraManager == null)
                {
                    cameraManager = FindObjectOfType<XvCameraManager>();
                }

                if (cameraManager == null)
                {
                    cameraManager = new GameObject("XvCameraManager").AddComponent<XvCameraManager>();
                }
                return cameraManager;
            }
        }
        [SerializeField]
        private RawImage rgbBackground;

        [SerializeField]
        private Camera bgCamera;

        public Camera BgCamera
        {
            get
            {
                if (bgCamera == null)
                {
                    bgCamera = transform.Find("BgCamera").GetComponent<Camera>();
                }
                return bgCamera;
            }
        }

        [SerializeField]
        private bool autoCapture = false;

        /// <summary>
        /// Mixed reality texture
        /// </summary>
        private RenderTexture cameraRenderTexture = null;

        public RenderTexture CameraRenderTexture
        {
            get
            {
                if (cameraRenderTexture == null)
                {
                    cameraRenderTexture = new RenderTexture(CameraManager.Width, CameraManager.Height, 24, RenderTextureFormat.RGB565);
                }
                return cameraRenderTexture;
            }
        }

        private bool isOn = false;
        public bool IsOn
        {
            get { return isOn; }
        }

        // Start is called before the first frame update
        void Awake()
        {
            BgCamera.targetTexture = CameraRenderTexture;
        }

        private void Start()
        {
            if (autoCapture)
            {
                StartCapture();
            }
        }

        /// <summary>
        /// Enable mixed reality video stream capture
        /// </summary>
        public void StartCapture()
        {
            // Ensure AR camera is active before capturing
            if (!CameraManager.IsOn(XvCameraStreamType.ARCameraStream))
            {
                CameraManager.StartCapture(XvCameraStreamType.ARCameraStream);
            }

            if (!isOn)
            {
                isOn = true;
                gameObject.SetActive(true);
                rgbBackground.gameObject.SetActive(true);
                BgCamera.gameObject.SetActive(true);
                XvCameraManager.onARCameraStreamFrameArrived.AddListener(onFrameArrived);
            }
        }

        /// <summary>
        /// Stop mixed reality video stream capture. Since the camera is shared, check if stopping will affect other functions.
        /// </summary>
        /// <param name="closeCamera">True: Close the camera; False: Keep the camera open</param>
        public void StopCapture(bool closeCamera = false)
        {
            if (isOn)
            {
                isOn = false;
                if (closeCamera && CameraManager.IsOn(XvCameraStreamType.ARCameraStream))
                {
                    CameraManager.StopCapture(XvCameraStreamType.ARCameraStream);
                }
                XvCameraManager.onARCameraStreamFrameArrived.RemoveListener(onFrameArrived);
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Receive camera images and apply them to the background texture.
        /// Set camera parameters based on captured data.
        /// </summary>
        /// <param name="cameraData">Camera frame data</param>
        private void onFrameArrived(cameraData cameraData)
        {
            if (rgbBackground != null)
            {
                rgbBackground.texture = cameraData.tex;
            }

            BgCamera.usePhysicalProperties = true;
            BgCamera.focalLength = cameraData.parameter.focal;
            BgCamera.sensorSize = new Vector2(
                cameraData.parameter.focal * cameraData.parameter.width / cameraData.parameter.fx,
                cameraData.parameter.focal * cameraData.parameter.height / cameraData.parameter.fy
            );

            BgCamera.lensShift = new Vector2(
                -(cameraData.parameter.cx - cameraData.parameter.width * 0.5f) / cameraData.parameter.width,
                (cameraData.parameter.cy - cameraData.parameter.height * 0.5f) / cameraData.parameter.height
            );

            BgCamera.gateFit = Camera.GateFitMode.Vertical;
            transform.localPosition = cameraData.parameter.position;
            transform.localRotation = cameraData.parameter.rotation;
        }
    }
}
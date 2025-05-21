using UnityEngine;
using UnityEngine.UI;

namespace HMSXR.Foundation
{ 

public class ScreenCanvas : MonoBehaviour
{
        [SerializeField]
        private XvMRVideoCaptureManager captureManager;

        public RawImage videoTexture;

        private void Awake()
        {
            if (captureManager == null)
            {
                captureManager = FindObjectOfType<XvMRVideoCaptureManager>();

                if (captureManager == null)
                {
                    GameObject newObj = Instantiate(Resources.Load<GameObject>("XvMRVideoCaptureManager"));

                    newObj.name = "XvMRVideoCaptureManager";
                    captureManager = newObj.GetComponent<XvMRVideoCaptureManager>();
                }
            }

        }
        void Start()
        {
            Show();
        }

        public void Show() {
            captureManager.StartCapture();
            videoTexture.gameObject.SetActive(true);
            videoTexture.texture = captureManager.CameraRenderTexture;
        }

        public void Hide() {
            captureManager.StopCapture(true);
            videoTexture.gameObject.SetActive(false);
        }
    }

}
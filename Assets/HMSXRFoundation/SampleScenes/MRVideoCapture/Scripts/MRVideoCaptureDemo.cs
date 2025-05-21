using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HMSXR.Foundation.SampleScenes
{
    public class MRVideoCaptureDemo : MonoBehaviour
    {
        [SerializeField]
        private XvMRVideoCaptureManager captureManager;

        public RawImage rawImage;
        void Start()
        {
            if (captureManager==null) {
                captureManager = FindObjectOfType<XvMRVideoCaptureManager>();

                if (captureManager==null) {
                    GameObject newObj = Instantiate(Resources.Load<GameObject>("XvMRVideoCaptureManager"));

                    newObj.name = "XvMRVideoCaptureManager";
                    captureManager = newObj.GetComponent<XvMRVideoCaptureManager>();
                }
            }
        }

        public void StartMRCaptureCamera() {
           
            rawImage.texture = captureManager.CameraRenderTexture;
            captureManager.StartCapture();
        }

        public void StopMRCaptureCamera()
        {
            rawImage.texture = null;
            captureManager.StopCapture();
        }
    }
}

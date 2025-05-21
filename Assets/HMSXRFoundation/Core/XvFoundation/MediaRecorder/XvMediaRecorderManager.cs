using NatSuite.Examples;
using UnityEngine;
using UnityEngine.Events;

namespace HMSXR.Foundation
{
    /// <summary>
    /// ������Ҫ���𱾵���Ƶ��¼�ƺͽ�ͼ
    /// </summary>
    [RequireComponent(typeof(ReplayCam))]
    [RequireComponent(typeof(JPG))]

    public sealed class XvMediaRecorderManager : MonoBehaviour
    {
        private XvMediaRecorderManager() { }
        private ReplayCam replayCam;
        private JPG jpg;

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

        [SerializeField]
        private int width = 1920;
        public int Width
        {
            get { return width; }
        }
        [SerializeField]
        private int height = 1080;
        public int Height
        {
            get { return height; }
        }

        private void Awake()
        {
            replayCam = GetComponent<ReplayCam>();
            jpg = GetComponent<JPG>();
            replayCam.videoWidth = width;
            replayCam.videoHeight = height;
            jpg.imageWidth = width;
            jpg.imageHeight = height;
            jpg.cam = XvMRVideoCaptureManager.BgCamera;
            replayCam.cam = XvMRVideoCaptureManager.BgCamera;

        }


        /// <summary>
        /// ������׽��ʵ�����Ƶ��
        /// </summary>
        public void StartCapture()
        {
            XvMRVideoCaptureManager.StartCapture();
        }
        /// <summary>
        /// ֹͣ��׽��ʵ�����Ƶ��
        /// ע�⣺����ǹ��õģ��رյ�ʱ����Ҫ��������ģ���Ƿ���Ҫ�õ�������������ģ��ʹ����������closeCamera==false
        /// </summary>
        /// <param name="closeCamera">true:�ر����   false:���ر����</param>
        public void StopCapture(bool closeCamera = false)
        {
            XvMRVideoCaptureManager.StopCapture(closeCamera);
        }

        /// <summary>
        /// ��ʼ¼���ٿ�ʼ¼��ǰ����Ҫȷ��StartCapture�����Ѿ����ã������ʵ��Ƶ���Ѿ�������
        /// </summary>

        public void StartRecording()
        {
            StartCapture();
            replayCam.StartRecording();
        }

        /// <summary>
        /// ֹͣ¼��
        /// </summary>
        /// <param name="callback"></param>
        public void StopRecording(UnityAction<string> callback)
        {

            replayCam.StopRecording(callback);
        }

        public bool IsVideoRecording()
        {
            return replayCam.IsRecording;
        }

        /// <summary>
        /// �����ͼ����ͼǰ��Ҫȷ��StartCapture�����Ѿ����ã������ʵ��Ƶ���Ѿ�������
        /// </summary>
        /// <param name="callback"></param>
        public void SaveScreenshot(UnityAction<string> callback)
        {
            StartCapture();

            jpg.SaveScreenshot(callback);
        }

        public bool IsTakingScreenshots()
        {
            return jpg.IsRecording;


        }
    }
}

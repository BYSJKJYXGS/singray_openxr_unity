using HMSXR.Foundation;
using UnityEditor;
using UnityEngine;
namespace HMSXRFoundation.SampleScenes
{
    public class HMSXRFoundation : MonoBehaviour
    {
        [MenuItem("GameObject/XvXR/XvFoundation/XvCameraManager", false, 0)]
        static void CreateXvCameraManager()
        {

            XvCameraManager xvCameraManager = FindObjectOfType<XvCameraManager>();

            if (xvCameraManager == null)
            {
                xvCameraManager = new GameObject("XvCameraManager").AddComponent<XvCameraManager>();

                Undo.RegisterCreatedObjectUndo(xvCameraManager.gameObject, xvCameraManager.gameObject.name);

            }
            else
            {
                Debug.LogWarning("XvCameraManager already exists in the scene");
            }
            Selection.activeObject = xvCameraManager.gameObject;


        }




        [MenuItem("GameObject/XvXR/XvFoundation/XvMRVideoCaptureManager", false, 1)]
        static void CreateXvMRVideoCaptureManager()
        {
            CreateXvCameraManager();

            XvMRVideoCaptureManager xvMRVideoCaptureManager = FindObjectOfType<XvMRVideoCaptureManager>();

            if (xvMRVideoCaptureManager == null)
            {

                GameObject newObj = Instantiate(Resources.Load<GameObject>("XvMRVideoCaptureManager"));

                newObj.name = "XvMRVideoCaptureManager";
                xvMRVideoCaptureManager = newObj.GetComponent<XvMRVideoCaptureManager>();

                Undo.RegisterCreatedObjectUndo(newObj.gameObject, newObj.gameObject.name);

            }
            else
            {
                Debug.LogWarning("XvMRVideoCaptureManager already exists in the scene");
            }
            Selection.activeObject = xvMRVideoCaptureManager.gameObject;

        }


        [MenuItem("GameObject/XvXR/XvFoundation/XvMediaRecorderManager", false, 2)]

        static void CreateXvMediaRecorderManager()
        {
            CreateXvMRVideoCaptureManager();
            XvMediaRecorderManager xvMediaRecorderManager = FindObjectOfType<XvMediaRecorderManager>();

            if (xvMediaRecorderManager == null)
            {
                xvMediaRecorderManager = new GameObject("XvMediaRecorderManager").AddComponent<XvMediaRecorderManager>();

                Undo.RegisterCreatedObjectUndo(xvMediaRecorderManager.gameObject, xvMediaRecorderManager.gameObject.name);

            }
            else
            {
                Debug.LogWarning("XvMediaRecorderManager already exists in the scene");
            }

            Selection.activeObject = xvMediaRecorderManager.gameObject;
        }

        [MenuItem("GameObject/XvXR/XvFoundation/XvMediaRecorder", false, 3)]

        static void CreateXvMediaRecorder()
        {
            CreateXvMediaRecorderManager();
            XvMediaRecorder xvMediaRecorder = FindObjectOfType<XvMediaRecorder>();

            if (xvMediaRecorder == null)
            {

                GameObject newObj = Instantiate(Resources.Load<GameObject>("XvMediaRecorder"));

                newObj.name = "XvMediaRecorder";
                xvMediaRecorder = newObj.GetComponent<XvMediaRecorder>();

                Undo.RegisterCreatedObjectUndo(newObj.gameObject, newObj.gameObject.name);

            }
            else
            {
                Debug.LogWarning("XvMediaRecorder already exists in the scene");
            }

            Selection.activeObject = xvMediaRecorder.gameObject;

        }

        [MenuItem("GameObject/XvXR/XvFoundation/XvTagRecognizerManager", false, 4)]

        static void CreateXvTagRecognizerManager()
        {

            XvTagRecognizerManager xvAprilTagManager = FindObjectOfType<XvTagRecognizerManager>();

            if (xvAprilTagManager == null)
            {

                GameObject newObj = Instantiate(Resources.Load<GameObject>("XvTagRecognizerManager"));

                newObj.name = "XvTagRecognizerManager";
                xvAprilTagManager = newObj.GetComponent<XvTagRecognizerManager>();

                Undo.RegisterCreatedObjectUndo(newObj.gameObject, newObj.gameObject.name);

            }
            else
            {
                Debug.LogWarning("XvTagRecognizerManager already exists in the scene");
            }

            Selection.activeObject = xvAprilTagManager.gameObject;
        }


        //[MenuItem("GameObject/XvXR/XvFoundation/XvRgbdManager", false, 5)]

        //static void CreateXvRgbdManager()
        //{

        //    XvRgbdManager xvRgbdManager = FindObjectOfType<XvRgbdManager>();

        //    if (xvRgbdManager == null)
        //    {
        //        xvRgbdManager = new GameObject("XvRgbdManager").AddComponent<XvRgbdManager>();

        //        Undo.RegisterCreatedObjectUndo(xvRgbdManager.gameObject, xvRgbdManager.gameObject.name);

        //    }
        //    else
        //    {
        //        Debug.LogWarning("XvRgbdManager already exists in the scene");
        //    }

        //    Selection.activeObject = xvRgbdManager.gameObject;

        //}

        [MenuItem("GameObject/XvXR/XvFoundation/XvRTSPStreamerManager", false,6)]

        static void CreateXvRTSPStreamerManager()
        {

            CreateXvMRVideoCaptureManager();
            XvRTSPStreamerManager xvRTSPStreamerManager = FindObjectOfType<XvRTSPStreamerManager>();

            if (xvRTSPStreamerManager == null)
            {
                xvRTSPStreamerManager = new GameObject("XvRTSPStreamerManager").AddComponent<XvRTSPStreamerManager>();

                Undo.RegisterCreatedObjectUndo(xvRTSPStreamerManager.gameObject, xvRTSPStreamerManager.gameObject.name);

            }
            else
            {
                Debug.LogWarning("XvRTSPStreamerManager already exists in the scene");
            }

            Selection.activeObject = xvRTSPStreamerManager.gameObject;

        }


        [MenuItem("GameObject/XvXR/XvFoundation/XvPlaneManager", false, 7)]

        static void CreateXvPlaneManager()
        {
            XvPlaneManager xvPlaneManager = FindObjectOfType<XvPlaneManager>();

            if (xvPlaneManager == null)
            {

                GameObject newObj = Instantiate(Resources.Load<GameObject>("XvPlaneManager"));

                newObj.name = "XvPlaneManager";
                xvPlaneManager = newObj.GetComponent<XvPlaneManager>();

                Undo.RegisterCreatedObjectUndo(newObj, newObj.name);

            }
            else
            {
                Debug.LogWarning("XvPlaneManager already exists in the scene");
            }

            Selection.activeObject = xvPlaneManager.gameObject;

        }


        [MenuItem("GameObject/XvXR/XvFoundation/XvSpatialMapManager", false, 8)]

        static void CreateXvSpatialMapManager()
        {
            XvSpatialMapManager xvSpatialMapManager = FindObjectOfType<XvSpatialMapManager>();

         

            if (xvSpatialMapManager == null)
            {
                xvSpatialMapManager = new GameObject("XvSpatialMapManager").AddComponent<XvSpatialMapManager>();

                Undo.RegisterCreatedObjectUndo(xvSpatialMapManager.gameObject, xvSpatialMapManager.gameObject.name);

            }
            else
            {
                Debug.LogWarning("XvRTSPStreamerManager already exists in the scene");
            }

            Selection.activeObject = xvSpatialMapManager.gameObject;

        }

        [MenuItem("GameObject/XvXR/XvFoundation/XvSpatialMeshManager", false, 9)]

        static void CreateXvSpatialMeshManager()
        {
            XvSpatialMeshManager xvSpatialMeshManager = FindObjectOfType<XvSpatialMeshManager>();

            if (xvSpatialMeshManager == null)
            {

                GameObject newObj = Instantiate(Resources.Load<GameObject>("XvSpatialMeshManager"));

                newObj.name = "XvSpatialMeshManager";
                xvSpatialMeshManager = newObj.GetComponent<XvSpatialMeshManager>();

                Undo.RegisterCreatedObjectUndo(newObj, newObj.name);

            }
            else
            {
                Debug.LogWarning("XvPlaneManager already exists in the scene");
            }

            Selection.activeObject = xvSpatialMeshManager.gameObject;

        }


      


        [MenuItem("GameObject/XvXR/XvFoundation/XvEyeTrackingManager", false, 10)]

        static void CreateXvEyeTrackingManager()
        {
            XvEyeTrackingManager xvEyeTrackingManager = FindObjectOfType<XvEyeTrackingManager>();



            if (xvEyeTrackingManager == null)
            {
                xvEyeTrackingManager = new GameObject("XvEyeTrackingManager").AddComponent<XvEyeTrackingManager>();

                Undo.RegisterCreatedObjectUndo(xvEyeTrackingManager.gameObject, xvEyeTrackingManager.gameObject.name);

            }
            else
            {
                Debug.LogWarning("XvEyeTrackingManager already exists in the scene");
            }

            Selection.activeObject = xvEyeTrackingManager.gameObject;

        }


    

        

       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using HMS.Spaces;
using static HMS.Spaces.XvFeature;

namespace HMSXR.Foundation
{
    /// <summary>
    /// This class is mainly responsible for the recognition of horizontal and vertical planes in the space.
    /// </summary>

    public sealed class XvPlaneManager : MonoBehaviour
    {
        private XvPlaneManager() { }
        public event Action<plane[]> planesChanged;

        private bool isDetecting;


        /// <summary>
        /// 
        /// </summary>
        public void StartPlaneDetction()
        {
            if (!isDetecting)
            {

#if !PLATFORM_ANDROID || UNITY_EDITOR
                return;
#endif
                isDetecting = XvFeature.XvFeatureStartDetectPlane(1);

            }

        }

        public void StopPlaneDetection()
        {
#if !PLATFORM_ANDROID || UNITY_EDITOR
            return;
#endif

            isDetecting = false;

        }

        private void Update()
        {
            if (isDetecting)
            {




                IntPtr intPtr;

                int objNum = 0;
                if (XvFeature.XvFeatureGetPlaneData(out intPtr, ref objNum))
                {
                    xplan_package[] xplan_Package = new xplan_package[objNum];



                    for (int i = 0; i < objNum; i++)
                    {

                        IntPtr ptr = intPtr + i * Marshal.SizeOf(typeof(xplan_package));

                        if (ptr == IntPtr.Zero)
                        {
                            return;
                        }
                        xplan_Package[i] = (xplan_package)Marshal.PtrToStructure(intPtr, typeof(xplan_package));


                        //text.text = "Number of detected planes:" + objNum.ToString() + "  distance£º" + xplan_Package[i].distance+ "  normal£º" + xplan_Package[i].normal.x+"  "+ xplan_Package[i].normal.y+"  "+ xplan_Package[i].normal.x;




                        Vector3d normal = xplan_Package[i].normal;

                        //Debug.Log("wuxh: normal" + i + "  " + normal.x + "    " + normal.y + "    " + normal.z);


                        Vector3d[] vertices = new Vector3d[xplan_Package[i].verticesSize];
                        IntPtr vIntPtr = xplan_Package[i].vertices;
                        for (int vi = 0; vi < xplan_Package[i].verticesSize; vi++)
                        {
                            IntPtr vIntPtrTmp = vIntPtr + vi * Marshal.SizeOf(typeof(Vector3d));
                            vertices[vi] = (Vector3d)Marshal.PtrToStructure(vIntPtrTmp, typeof(Vector3d));
                            /// Debug.Log(" vertices"+ vi + "  "+ vertices[vi].x +"    "+ vertices[vi].y +"    "+ vertices[vi].z);
                        }




                        plane[] planes = GetPlane(normal, vertices, xplan_Package[i].pid, (int)xplan_Package[i].verticesSize);
                        if (planes != null)
                        {
                            planesChanged?.Invoke(planes);
                        }
                        Marshal.DestroyStructure(ptr, typeof(xplan_package));

                    }
                    Marshal.FreeHGlobal(intPtr);
                }
                else
                {
                    MyDebugTool.LogError("No planes detected");
                }

            }
        }





        private plane[] GetPlane(Vector3d normal, Vector3d[] vertices, string id, int vSize)
        {
            plane plane = new plane();
            plane.normal = normal;
            plane.points = vertices.ToList();
            plane.id = id;
            plane.d = vSize;
            return null;
        }


        private void OnDestroy()
        {
            StopPlaneDetection();
        }

        private void OnApplicationQuit()
        {
            StopPlaneDetection();
        }
    }
    public struct Vector3D
    {
        public double X;
        public double Y;
        public double Z;
    };


    public class plane
    {
        public List<Vector3d> points;//Plane vertex coordinates
        public Vector3d normal;//Plane normal
        public double d;
        public string id;//Plane ID
    };
}
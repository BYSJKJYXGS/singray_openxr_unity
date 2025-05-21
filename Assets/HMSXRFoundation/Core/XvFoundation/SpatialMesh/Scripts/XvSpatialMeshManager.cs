using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using HMS.Spaces;
namespace HMSXR.Foundation
{
    /// <summary>
    /// This class is responsible for spatial mesh scanning.
    /// </summary>
    public sealed class XvSpatialMeshManager : MonoBehaviour
    {
        private XvSpatialMeshManager() { }


        private static XslamSurface[] objdata;
        public static int meshSurfaceId;
        private bool isCreatMesh = false;

        public static event Action<NowXslamSurface> meshChanged;

        /// <summary>
        /// Enable spatial mesh scanning functionality
        /// </summary>
        public void StartMeshDetection()
        {
#if UNITY_EDITOR
            return;
#endif
            isCreatMesh = XvFeature.XvFeatureStartGetSurface();
            if (isCreatMesh)
            {
                StartCoroutine(IE_GetSurfaceData());
            }
        }

        /// <summary>
        /// Disable spatial mesh scanning functionality
        /// </summary>
        public void StopMeshDetection()
        {
            if (isCreatMesh)
            {
                isCreatMesh = false;
            }
        }

        private IEnumerator IE_GetSurfaceData()
        {
            while (isCreatMesh)
            {
                IntPtr intPtr;
                int surfaceCount = 0;
                if (XvFeature.XvFeatureGetSurfaceData(out intPtr, ref surfaceCount))
                {
                    if (surfaceCount > 0)
                    {
                        yield return StartCoroutine(IEIntPtrToStructure(intPtr, surfaceCount));
                    }
                }
                yield return WaitForSeconds;
            }
        }

        private WaitForSeconds WaitForSeconds = new WaitForSeconds(0.01f);
        private WaitForSeconds WaitForSeconds1 = new WaitForSeconds(0.04f);

        private IEnumerator IEIntPtrToStructure(IntPtr surfaces, int size)
        {
            XslamSurface[] objdata = new XslamSurface[size];
            for (int i = 0; i < size; i++)
            {
                objdata[i] = new XslamSurface();
                IntPtr ptr = surfaces + i * Marshal.SizeOf(typeof(XslamSurface));
                if (ptr == IntPtr.Zero) yield break;

                objdata[i] = (XslamSurface)Marshal.PtrToStructure(ptr, typeof(XslamSurface));

                IntPtr intptr_kp = objdata[i].vertices;
                if (intptr_kp == IntPtr.Zero) yield break;

                IntPtr intptr_nor = objdata[i].vertexNormals;
                if (intptr_nor == IntPtr.Zero) yield break;

                IntPtr intptr_t = objdata[i].triangles;
                if (intptr_t == IntPtr.Zero) yield break;

                List<Vector3> viList = new List<Vector3>();
                List<Vector3> vi_nList = new List<Vector3>();
                List<Vector3uint> trianglesList = new List<Vector3uint>();

                // All points of each detected object
                for (int n = 0; n < objdata[i].verticesSize; n++)
                {
                    IntPtr ptr_vi = intptr_kp + n * Marshal.SizeOf(typeof(Vector3));
                    if (ptr_vi == IntPtr.Zero) yield break;
                    Vector3 vi = (Vector3)Marshal.PtrToStructure(ptr_vi, typeof(Vector3));
                    viList.Add(vi);

                    IntPtr ptr_vi_n = intptr_nor + n * Marshal.SizeOf(typeof(Vector3));
                    if (ptr_vi_n == IntPtr.Zero) yield break;
                    Vector3 vi_n = (Vector3)Marshal.PtrToStructure(ptr_vi_n, typeof(Vector3));
                    vi_nList.Add(vi_n);

                    Marshal.DestroyStructure(ptr_vi, typeof(Vector3));
                    Marshal.DestroyStructure(ptr_vi_n, typeof(Vector3));
                }

                for (int n = 0; n < objdata[i].trianglesSize; n++)
                {
                    IntPtr ptr_vi_t = intptr_t + n * Marshal.SizeOf(typeof(Vector3uint));
                    if (ptr_vi_t == IntPtr.Zero) yield break;
                    Vector3uint vi_t = (Vector3uint)Marshal.PtrToStructure(ptr_vi_t, typeof(Vector3uint));
                    trianglesList.Add(vi_t);

                    Marshal.DestroyStructure(ptr_vi_t, typeof(Vector3uint));
                }

                getCreatMeshData(viList, vi_nList, trianglesList, objdata[i].mapId.ToString());
                Marshal.DestroyStructure(ptr, typeof(XslamSurface));
                Marshal.FreeHGlobal(intptr_kp);
                Marshal.FreeHGlobal(intptr_nor);
                Marshal.FreeHGlobal(intptr_t);

                yield return WaitForSeconds1;
            }

            Marshal.FreeHGlobal(surfaces);
        }




        private static void getCreatMeshData(List<Vector3> vList0, List<Vector3> vList1, List<Vector3uint> tList1, string mapID)
        {
            if (mapID == "") return;

            NowXslamSurface surface = new NowXslamSurface();
            surface.vList0_t = vList0;
            surface.vList1_t = vList1;
            surface.vListt_t = tList1;
            surface.mapID = mapID;

            meshChanged?.Invoke(surface);
        }

        private void OnDestroy()
        {
            StopMeshDetection();
        }

        private void OnApplicationQuit()
        {
            StopMeshDetection();
        }
    }

    public class NowXslamSurface
    {
        public List<Vector3> vList0_t; // Vertex coordinates
        public List<Vector3> vList1_t; // Normals
        public List<Vector3uint> vListt_t; // Triangle indices
        public string mapID;
    }

    public struct Vector3uint
    {
        public uint x;
        public uint y;
        public uint z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XslamSurface
    {
        public uint mapId;
        public uint version;
        public uint id;

        public uint verticesSize;

        public IntPtr vertices;

        public IntPtr vertexNormals;

        public uint trianglesSize;

        public IntPtr triangles;

        public IntPtr textureCoordinates;
        public uint textureWidth;
        public uint textureHeight;
        //[MarshalAs(UnmanagedType.LPArray)] 
        //public byte[] textureRgba;
    };
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace HMSXR.Foundation
{
    /// <summary>
    /// Spatial mesh visualization processing class
    /// </summary>
    public sealed class XvSpatialMeshVisualizer : WorkQueue
    {
        private XvSpatialMeshVisualizer() { }

        /// <summary>
        /// All created meshes
        /// </summary>
        private Dictionary<string, GameObject> meshDic = new Dictionary<string, GameObject>();


        [SerializeField]
        private XvSpatialMeshManager xvSpatialMeshManager;

        [SerializeField]
        private Material meshMat;




        [SerializeField]
        private bool enableCollider = true;

        public bool EnableCollider
        {
            get { return enableCollider; }
        }

        [SerializeField]
        private bool enableRender = true;

        public bool EnableRender
        {
            get { return enableRender; }
        }

        [SerializeField]

        public bool autoDetection = true;
        public bool AutoDetection
        {
            get { return autoDetection; }
        }

        protected override void Awake()
        {
            if (xvSpatialMeshManager == null)
            {
                xvSpatialMeshManager = FindObjectOfType<XvSpatialMeshManager>();
            }
        }

        private void OnEnable()
        {
            XvSpatialMeshManager.meshChanged += meshChanged;

            if (autoDetection)
            {
                xvSpatialMeshManager.StartMeshDetection();
            }
        }


        private void OnDisable()
        {
            XvSpatialMeshManager.meshChanged -= meshChanged;
            if (autoDetection)
            {
                xvSpatialMeshManager.StopMeshDetection();
            }
        }


        private void meshChanged(NowXslamSurface nowXslamSurface)
        {
            InvokeOnAppThread(() =>
            {
                creatMesh(nowXslamSurface);
            });
        }

        private void creatMesh(NowXslamSurface data)
        {
            if (meshDic.TryGetValue(data.mapID, out GameObject meshObj))
            {
                Destroy(meshObj);
                meshDic.Remove(data.mapID);
            }

            GameObject meshMother = new GameObject();
            meshMother.name = data.mapID;
            meshMother.transform.parent = transform;

            GameObject c = new GameObject();
            c.transform.parent = meshMother.transform;

            c.AddComponent<MeshFilter>();
            c.AddComponent<MeshRenderer>();

            Vector3[] v3 = new Vector3[data.vList0_t.Count];
            for (int i = 0; i < data.vList0_t.Count; i++)
            {
                v3[i] = new Vector3(data.vList0_t[i].x, -data.vList0_t[i].y, data.vList0_t[i].z);
            }

            Vector3[] vn = new Vector3[data.vList1_t.Count];
            for (int i = 0; i < data.vList1_t.Count; i++)
            {
                vn[i] = new Vector3(data.vList1_t[i].x, -data.vList1_t[i].y, data.vList1_t[i].z);
            }

            int[] i3 = new int[data.vListt_t.Count * 3];
            for (int i = 0; i < data.vListt_t.Count; i++)
            {
                i3[i * 3] = (int)data.vListt_t[i].x;
                i3[i * 3 + 1] = (int)data.vListt_t[i].y;
                i3[i * 3 + 2] = (int)data.vListt_t[i].z;
            }

            Vector2[] uv = { new Vector2(0.5f, 0.5f), new Vector2(1, 0), new Vector2(0, 0) };
            Mesh mesh = c.GetComponent<MeshFilter>().mesh;
            mesh.vertices = v3;
            Array.Reverse(i3);
            mesh.triangles = i3;
            mesh.normals = vn;
            mesh.uv = uv;

            c.GetComponent<MeshRenderer>().material = meshMat;
            c.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            c.GetComponent<MeshRenderer>().receiveShadows = false;
            c.isStatic = true;
            c.name = "tPolygon";

            c.GetComponent<MeshRenderer>().enabled = enableRender;
            Collider collider = c.AddComponent<MeshCollider>();
            collider.enabled = enableCollider;
            meshDic.Add(data.mapID, c);
        }

        public void SetCollider(bool enable)
        {
            enableCollider = enable;
            foreach (var item in meshDic.Values)
            {
                item.GetComponent<Collider>().enabled = enableCollider;
            }
        }

        public void SetVisualizer(bool enable)
        {
            this.enableRender = enable;
            foreach (var item in meshDic.Values)
            {
                item.GetComponent<MeshRenderer>().enabled = enableRender;
            }
        }
    }
}
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using HMS.Spaces;

namespace HMSXR.Foundation
{
    public class XvTofCamera : XvCameraBase
    {
        public XvTofCamera(int width, int height, int fps, FrameArrived frameArrived) : base(width, height, fps, frameArrived)
        {

        }

        private Texture2D tex = null;


        private Color32[] pixel32;
        private GCHandle pixelHandle;
        private IntPtr pixelPtr;
        private double rgbTimestamp = 0;
        public override void StartCapture()
        {
            if (!isOpen) {

                isOpen = true;
            }
        }

        public override void StopCapture()
        {
            if (isOpen) {
                pixelHandle.Free();
                tex = null;
                
            }
            isOpen = false;

        }
        public override void Update()
        {
            if (isOpen )
            {
                int width = 0;
                int height = 0;

                bool ifGet1 = XvFeature.XvFeatureGetTofImageSize(ref width, ref height);

                if (width > 0 && height > 0)
                {

                    if (!tex)
                    {
                        MyDebugTool.Log("Create TOF texture " + width + "x" + height);
                        TextureFormat format = TextureFormat.RGBA32;
                        tex = new Texture2D(width, height, format, false);


                        pixel32 = tex.GetPixels32();
                        pixelHandle = GCHandle.Alloc(pixel32, GCHandleType.Pinned);
                        pixelPtr = pixelHandle.AddrOfPinnedObject();


                    }


                    if (XvFeature.XvFeatureGetTofImageData(pixelPtr, tex.width, tex.height, ref rgbTimestamp))
                    {
                        //Update the Texture2D with array updated in C++
                        tex.SetPixels32(pixel32);
                        tex.Apply();
                        cameraData.tex = tex;
                        cameraData.texWidth = tex.width;

                        cameraData.texHeight = tex.height;
                        

                        frameArrived?.Invoke(cameraData);
                    }
                    else
                    {
                        MyDebugTool.Log("Invalid TOF texture");
                    }



                }
            }
        }
      
       
    }
}

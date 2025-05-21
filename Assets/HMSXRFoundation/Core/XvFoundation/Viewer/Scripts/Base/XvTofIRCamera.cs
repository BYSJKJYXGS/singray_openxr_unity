using UnityEngine;
using System.Runtime.InteropServices;
using System;
using HMS.Spaces;

namespace HMSXR.Foundation
{

    public class XvTofIRCamera : XvCameraBase
{
    public XvTofIRCamera(int width, int height, int fps, FrameArrived frameArrived) : base(width, height, fps, frameArrived)
    {

    }

    private Texture2D tex = null;


    private Color32[] pixel32;
    private GCHandle pixelHandle;
    private IntPtr pixelPtr;
    private int lastWidth = 0;
    private int lastHeight = 0;
        private double rgbTimestamp = 0;
        public override void StartCapture()
    {
        if (!isOpen)
        {
               
                isOpen = true;
        }
    }

    public override void StopCapture()
    {
        if (isOpen)
        {
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
                MyDebugTool.Log("Create TOF IR Update " + width + "x" + height);

                if (width > 0 && height > 0)
                {

                    if (lastWidth != width || lastHeight != height)
                    {
                        try
                        {
                            double r = 1.0;
                            int w = (int)(width * r);
                            int h = (int)(height * r);
                            Debug.Log("Create RGB texture " + w + "x" + h);
                            TextureFormat format = TextureFormat.RGBA32;
                            tex = new Texture2D(w, h, format, false);
                            //tex.filterMode = FilterMode.Point;
                            tex.Apply();
                            try
                            {
                                pixelHandle.Free();
                            }
                            catch { }
                            pixel32 = tex.GetPixels32();
                            pixelHandle = GCHandle.Alloc(pixel32, GCHandleType.Pinned);
                            pixelPtr = pixelHandle.AddrOfPinnedObject();

                            
                        }
                        catch (Exception e)
                        {
                            MyDebugTool.Log(e.Message);
                            return;
                        }

                        lastWidth = width;
                        lastHeight = height;
                    }

                    Debug.Log("====================================");
                    Debug.Log("pixelPtr ============== " + pixelPtr);
                    Debug.Log("tex.width ============== " + tex.width);
                    Debug.Log("tex.height ============== " + tex.height);
                    try
                    {
                        if (XvFeature.XvFeatureGetTofImageData(pixelPtr, tex.width, tex.height, ref rgbTimestamp))
                        {
                            Debug.Log("vr_log:pixelPtr RGBRecord Update xslam_get_rgb_image_RGBA pixelPtr: " + pixelPtr);
                            Debug.Log("vr_log:tex.width RGBRecord Update xslam_get_rgb_image_RGBA width: " + tex.width);
                            Debug.Log("vr_log:tex.height RGBRecord Update xslam_get_rgb_image_RGBA height: " + tex.height);
                            Debug.Log("vr_log:timestamp RGBRecord Update xslam_get_rgb_image_RGBA rgbTimestamp: " + rgbTimestamp);

                            tex.SetPixels32(pixel32);
                            tex.Apply();

                            cameraData.tex = tex;
                            cameraData.texWidth = tex.width;

                            cameraData.texHeight = tex.height;


                            frameArrived?.Invoke(cameraData);
                        }
                        else
                        {
                            Debug.Log("Invalid texture");
                        }
                    }
                    catch (Exception e)
                    {
                        MyDebugTool.Log(e.Message);
                        return;
                    }
                    Debug.Log("====================================");
                }

             
        }
    }

   
}
}

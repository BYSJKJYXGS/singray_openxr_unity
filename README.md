# Singray OpenXR Unity SDK User Guide

This SDK provides complete OpenXR development capabilities for Singray AR Glasses within the Unity environment, pre-integrating MRTK, related dependencies, and sample scenes.

---

## 1. Environment Preparation

1. **Download Android Tool SDK**  
   Download and extract our specified Android Tool SDK from the following link:  
   [Android Tool SDK Download Link](https://drive.google.com/file/d/1yXS1ufckKG2HPbTcyq4Wz2ohaVvfGISS/view?usp=sharing)

2. **Obtain Unity Project**  
   Download our Unity project using `git clone` or other methods. The project already contains the MRTK, OpenXR dependencies, and sample scenes.

---

## 2. Sample Scene Reference

- Please refer to the sample scene:  
  `HMSXRFoundation/SamplesScenes/Viewer/Scenes/Viewer.unity`  
  This scene demonstrates how the SDK accesses RGB, ToF, and MR video streams.

---

## 3. Build and Deployment

1. **Unity Version Requirement**  
   It is **highly recommended** to use Unity version **2021.3.2x** for development.

2. **Configure Android SDK**  
   - Open Unity and go to  
     `Edit -> Preferences -> External Tools`
   - In the `Android SDK Tools Installed with Unity` section, replace the path with the directory of the Android Tool SDK downloaded and extracted from Google Drive.

3. **Build the Project**  
   - Navigate to  
     `HMSXR -> toolkit -> Build scenes-viewer`
   - Follow instructions to compile and build your application.

---

## 4. Service APK Installation & Setup

1. **Download and Install APK**  
   - Go to the [Release Page](https://github.com/BYSJKJYXGS/singray_openxr_unity/releases/latest) and download **Initial Release for Singray OpenXR Service APK Latest**. Install it on your Android device.

2. **Connect AR Glasses and Authorize**  
   - After installation, connect your AR glasses to your device and grant all required permissions, making sure “Display over other apps” is enabled.
   - Once the “HCM Device” status displays “Enabled,” the OpenXR runtime is ready.

---

## 5. Features

- Version **v1.0.0** currently supports:  
  - 6DoF (Six degrees of freedom)  
  - MRTK gesture recognition  
  - Sensor data display  

- The SDK will continue to be optimized and updated. Please stay tuned.

---

## 6. Technical Support

If you encounter any problems during use, please contact us by email:  
**li.zhang@edge-perception.com**

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

namespace HMSXRFoundation
{
    public class BuildSettingWindows : EditorWindow, IHasCustomMenu
    {
        [MenuItem("HMSXR/Toolkit/OpenXR Feature", false, 99)]
        static void ConfigurePlayerSettings() => ConfigureOpenXRBuildSettings();

        [MenuItem("GameObject/XR/OpenXR Feature", false, 99)]
        static void ConfigureGameObjectSettings() => ConfigureOpenXRBuildSettings();

        private static void ConfigureOpenXRBuildSettings()
        {
            // Configure Android API levels
            Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel30;

            // Set IL2CPP scripting backend for better performance
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            // Configure .NET Framework compatibility
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_Unity_4_8);

            // Disable Auto Graphics API selection
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);

            // Set OpenGLES3 as the graphics API
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });

            // Target ARM64 architecture
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            // Open XR Plug-in Management settings
            const string xrSettingsPath = "Project/XR Plug-in Management/OpenXR";
            SettingsService.OpenProjectSettings(xrSettingsPath);
        }

        // Unused event handlers removed for clarity
        // Consider implementing these only if needed

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Apply Settings"), false, ApplySettings);
        }

        private static void ApplySettings()
        {
            ConfigureOpenXRBuildSettings();
            Debug.Log("OpenXR build settings applied successfully!");
        }
    }
}
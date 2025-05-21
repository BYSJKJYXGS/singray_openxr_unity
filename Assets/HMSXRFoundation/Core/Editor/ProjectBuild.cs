using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build.Reporting;

namespace HMSXRFoundation
{
    public class ProjectBuild : Editor
    {
        // Get all enabled scene paths
        private static string[] GetEnabledScenes()
        {
            var scenePaths = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled && !string.IsNullOrEmpty(scene.path))
                {
                    scenePaths.Add(scene.path);
                }
            }
            return scenePaths.ToArray();
        }

        // Generic build method
        private static void BuildProject(string scenePath, string appName, string bundleId)
        {
            // Validate scene path
            if (!File.Exists(scenePath))
            {
                Debug.LogError($"Scene not found: {scenePath}");
                return;
            }

            // Configure build settings
            PlayerSettings.productName = appName;
            PlayerSettings.applicationIdentifier = bundleId;
            PlayerSettings.Android.useCustomKeystore = false; // Use default signing configuration

            // Generate timestamped APK path
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmm");
            string apkPath = Path.Combine(
                Application.dataPath.Replace("Assets", "Builds"),
                $"{appName}_{timestamp}.apk"
            );

            // Create output directory
            Directory.CreateDirectory(Path.GetDirectoryName(apkPath));

            // Execute build
            var result = BuildPipeline.BuildPlayer(
                new[] { scenePath },
                apkPath,
                BuildTarget.Android,
                BuildOptions.None
            );

            // Log build result
            if (result.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build succeeded: {apkPath}");
            }
            else
            {
                Debug.LogError($"Build failed: {result.summary.result}");
            }
        }

        // Viewer scene build shortcut
        [MenuItem("HMSXR/Toolkit/Build Scenes/Viewer", false, 0)]
        private static void BuildViewerScene() =>
            BuildProject(
                "Assets/HMSXRFoundation/SampleScenes/Viewer/Scenes/Viewer.unity",
                "Viewer",
                "com.HMS.Viewer"
            );

        // Tag Recognizer scene build shortcut
        [MenuItem("HMSXR/Toolkit/Build Scenes/TagRecognizer", false, 1)]
        private static void BuildTagRecognizerScene() =>
            BuildProject(
                "Assets/HMSXRFoundation/SampleScenes/TagRecognizer/Scenes/TagRecognizer.unity",
                "TagRecognizer",
                "com.HMS.TagRecognizer"
            );

        // Batch build all sample scenes
        [MenuItem("HMSXR/Toolkit/Build Scenes/All Samples", false, 100)]
        private static void BuildAllSamples()
        {
            var scenes = new Dictionary<string, (string, string)>
            {
                {"Viewer", ("Viewer.unity", "com.HMS.Viewer")},
                {"TagRecognizer", ("TagRecognizer.unity", "com.HMS.TagRecognizer")},
                {"PlaneDetection", ("PlaneDetection.unity", "com.HMS.PlaneDetection")},
                // Add other scenes here
            };

            foreach (var (appName, (sceneName, bundleId)) in scenes)
            {
                string scenePath = $"Assets/HMSXRFoundation/SampleScenes/{appName}/Scenes/{sceneName}";
                BuildProject(scenePath, appName, bundleId);
            }
        }

        // Get formatted timestamp
        private static string GetFormattedTimestamp() =>
            DateTime.Now.ToString("yyyyMMddHHmm");

        // Initialize build settings (e.g., set app icon)
        private static void InitializeBuildSettings(string iconName = "DefaultIcon")
        {
            string iconPath = $"Assets/Images/Icon/{iconName}.png";
            var iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

            if (iconTexture == null)
            {
                Debug.LogWarning($"Icon not found at: {iconPath}");
                return;
            }

            // Set Android icons (auto-adapt sizes)
            PlayerSettings.SetIconsForTargetGroup(
                BuildTargetGroup.Android,
                new[] { iconTexture } // Recommend providing icon array with different sizes
            );
            AssetDatabase.SaveAssets();
        }

        // Configure environment path for Android build tools
        private static void ConfigureEnvironmentPath()
        {
            string sdkRoot = EditorPrefs.GetString("AndroidSdkRoot", "");
            if (string.IsNullOrEmpty(sdkRoot)) return;

            string buildToolsPath = Path.Combine(sdkRoot, "build-tools");
            if (!Directory.Exists(buildToolsPath)) return;

            var levels = Directory.GetDirectories(buildToolsPath);
            Array.Sort(levels, Comparer<string>.Create((a, b) =>
                Version.Parse(Path.GetFileName(b))?.CompareTo(Version.Parse(Path.GetFileName(a))) ?? 0
            ));

            string pathVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process) ?? "";
            string newPaths = string.Join(Path.PathSeparator.ToString(), levels);
            Environment.SetEnvironmentVariable("PATH", $"{pathVariable}{Path.PathSeparator}{newPaths}",
                EnvironmentVariableTarget.Process);
        }
    }
}
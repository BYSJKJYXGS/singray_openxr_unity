﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Editor
{
    /// <summary>
    /// Custom editor for the Tap to Place component.
    /// </summary>
    [CustomEditor(typeof(TapToPlace))]
    [CanEditMultipleObjects]
    public class TapToPlaceEditor : UnityEditor.Editor
    {
        private TapToPlace instance;

        // Tap to Place properties
        private SerializedProperty placementAction;
        private SerializedProperty autoStart;
        private SerializedProperty defaultPlacementDistance;
        private SerializedProperty maxRaycastDistance;
        private SerializedProperty surfaceNormalOffset;
        private SerializedProperty useDefaultSurfaceNormalOffset;
        private SerializedProperty keepOrientationVertical;
        private SerializedProperty rotateAccordingToSurface;
        private SerializedProperty debugEnabled;
        private SerializedProperty onPlacingStarted;
        private SerializedProperty onPlacingStopped;

        // Advanced properties
        private SerializedProperty magneticSurfaces;
        private SerializedProperty updateLinkedTransformProperty;
        private SerializedProperty moveLerpTimeProperty;
        private SerializedProperty rotateLerpTimeProperty;
        private SerializedProperty scaleLerpTimeProperty;
        private SerializedProperty maintainScaleOnInitializationProperty;
        private SerializedProperty smoothingProperty;
        private SerializedProperty lifetimeProperty;

        private const string AdvancedPropertiesFoldoutKey = "TapToPlaceAdvancedProperties";

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            instance = (TapToPlace)target;

            // Main Tap to Place Properties
            placementAction = serializedObject.FindProperty("placementActionReference");
            autoStart = serializedObject.FindProperty("autoStart");
            defaultPlacementDistance = serializedObject.FindProperty("defaultPlacementDistance");
            maxRaycastDistance = serializedObject.FindProperty("maxRaycastDistance");
            useDefaultSurfaceNormalOffset = serializedObject.FindProperty("useDefaultSurfaceNormalOffset");
            surfaceNormalOffset = serializedObject.FindProperty("surfaceNormalOffset");
            keepOrientationVertical = serializedObject.FindProperty("keepOrientationVertical");
            rotateAccordingToSurface = serializedObject.FindProperty("rotateAccordingToSurface");
            debugEnabled = serializedObject.FindProperty("debugEnabled");
            onPlacingStopped = serializedObject.FindProperty("onPlacingStopped");
            onPlacingStarted = serializedObject.FindProperty("onPlacingStarted");

            // Advanced Properties
            updateLinkedTransformProperty = serializedObject.FindProperty("updateLinkedTransform");
            moveLerpTimeProperty = serializedObject.FindProperty("moveLerpTime");
            rotateLerpTimeProperty = serializedObject.FindProperty("rotateLerpTime");
            scaleLerpTimeProperty = serializedObject.FindProperty("scaleLerpTime");
            maintainScaleOnInitializationProperty = serializedObject.FindProperty("maintainScaleOnInitialization");
            smoothingProperty = serializedObject.FindProperty("smoothing");
            lifetimeProperty = serializedObject.FindProperty("lifetime");
            magneticSurfaces = serializedObject.FindProperty("magneticSurfaces");
        }

        /// <summary>
        /// Called by the Unity editor to render custom inspector UI for this component.
        /// </summary>
        public override void OnInspectorGUI()
        {
            RenderCustomInspector();
        }
        
        /// <summary>
        /// Render the custom inspector with the basic and advanced properties
        /// </summary>
        private void RenderCustomInspector()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(autoStart);
            EditorGUILayout.PropertyField(defaultPlacementDistance);
            EditorGUILayout.PropertyField(maxRaycastDistance);
            EditorGUILayout.PropertyField(useDefaultSurfaceNormalOffset);

            // Only show the SurfaceNormalOffset property if UseDefaultSurfaceNormalOffset is false because setting the SurfaceNormalOffset of 
            // a tap to place object is only relevant if the defaultSurfaceNormalOffset is not used
            if (!instance.UseDefaultSurfaceNormalOffset)
            {
                EditorGUILayout.PropertyField(surfaceNormalOffset);
            }

            EditorGUILayout.PropertyField(keepOrientationVertical);
            EditorGUILayout.PropertyField(rotateAccordingToSurface);
            EditorGUILayout.PropertyField(debugEnabled);
            EditorGUILayout.PropertyField(onPlacingStarted);
            EditorGUILayout.PropertyField(onPlacingStopped);

            // Render Advanced Properties Foldout
            RenderAdvancedProperties();

            serializedObject.ApplyModifiedProperties();
        }

        // Render the Advanced Properties under an indented foldout titled Advanced Properties
        private void RenderAdvancedProperties()
        {
            // Render Advanced Settings
            if (InspectorUIUtility.DrawSectionFoldoutWithKey("Advanced Properties", AdvancedPropertiesFoldoutKey,
                MRTKEditorStyles.TitleFoldoutStyle, false))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(updateLinkedTransformProperty);
                    EditorGUILayout.PropertyField(moveLerpTimeProperty);
                    EditorGUILayout.PropertyField(rotateLerpTimeProperty);
                    EditorGUILayout.PropertyField(scaleLerpTimeProperty);
                    EditorGUILayout.PropertyField(maintainScaleOnInitializationProperty);
                    EditorGUILayout.PropertyField(smoothingProperty);
                    EditorGUILayout.PropertyField(lifetimeProperty);
                    EditorGUILayout.PropertyField(magneticSurfaces, true);
                }
            }
        }
    }
}

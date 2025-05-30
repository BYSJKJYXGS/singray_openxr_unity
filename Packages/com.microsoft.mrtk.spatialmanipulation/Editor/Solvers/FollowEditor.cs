﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Editor
{
    /// <summary>
    /// A custom editor for the <see cref="Follow"/> class.
    /// </summary>
    [CustomEditor(typeof(Follow))]
    [CanEditMultipleObjects]
    public class FollowEditor : SolverEditor
    {
        // Orientation
        private SerializedProperty orientationType;
        private SerializedProperty faceTrackedObjectWhileClamped;
        private SerializedProperty faceUserDefinedTargetTransform;
        private SerializedProperty targetToFace;
        private SerializedProperty pivotAxis;
        private SerializedProperty reorientWhenOutsideParameters;
        private SerializedProperty orientToControllerDeadZoneDegrees;

        // Distance
        private SerializedProperty ignoreDistanceClamp;
        private SerializedProperty minDistance;
        private SerializedProperty maxDistance;
        private SerializedProperty defaultDistance;
        private SerializedProperty verticalMaxDistance;

        // Direction
        private SerializedProperty ignoreAngleClamp;
        private SerializedProperty ignoreReferencePitchAndRoll;
        private SerializedProperty pitchOffset;
        private SerializedProperty angularClampMode;
        private SerializedProperty tetherAngleSteps;
        private SerializedProperty maxViewHorizontalDegrees;
        private SerializedProperty maxViewVerticalDegrees;
        private SerializedProperty boundsScaler;

        private bool orientationFoldout = true;
        private bool distanceFoldout = true;
        private bool directionFoldout = true;

        private Follow solverInBetween;

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            orientationType = serializedObject.FindProperty("orientationType");
            faceTrackedObjectWhileClamped = serializedObject.FindProperty("faceTrackedObjectWhileClamped");
            faceUserDefinedTargetTransform = serializedObject.FindProperty("faceUserDefinedTargetTransform");
            targetToFace = serializedObject.FindProperty("targetToFace");
            pivotAxis = serializedObject.FindProperty("pivotAxis");
            reorientWhenOutsideParameters = serializedObject.FindProperty("reorientWhenOutsideParameters");
            orientToControllerDeadZoneDegrees = serializedObject.FindProperty("orientToControllerDeadZoneDegrees");

            ignoreDistanceClamp = serializedObject.FindProperty("ignoreDistanceClamp");
            minDistance = serializedObject.FindProperty("minDistance");
            maxDistance = serializedObject.FindProperty("maxDistance");
            defaultDistance = serializedObject.FindProperty("defaultDistance");
            verticalMaxDistance = serializedObject.FindProperty("verticalMaxDistance");

            ignoreAngleClamp = serializedObject.FindProperty("ignoreAngleClamp");
            ignoreReferencePitchAndRoll = serializedObject.FindProperty("ignoreReferencePitchAndRoll");
            pitchOffset = serializedObject.FindProperty("pitchOffset");
            angularClampMode = serializedObject.FindProperty("angularClampMode");
            tetherAngleSteps = serializedObject.FindProperty("tetherAngleSteps");
            maxViewHorizontalDegrees = serializedObject.FindProperty("maxViewHorizontalDegrees");
            maxViewVerticalDegrees = serializedObject.FindProperty("maxViewVerticalDegrees");
            boundsScaler = serializedObject.FindProperty("boundsScaler");

            solverInBetween = target as Follow;
        }

        /// <summary>
        /// Called by the Unity editor to render custom inspector UI for this component.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            GUIStyle style = EditorStyles.foldout;
            FontStyle previousStyle = style.fontStyle;
            style.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();
            orientationFoldout = EditorGUILayout.Foldout(orientationFoldout, "Orientation", true);

            if (orientationFoldout)
            {
                EditorGUILayout.PropertyField(faceUserDefinedTargetTransform);
                if (faceUserDefinedTargetTransform.boolValue)
                {
                    EditorGUILayout.PropertyField(targetToFace);
                    EditorGUILayout.PropertyField(pivotAxis);
                }
                else
                {
                    EditorGUILayout.PropertyField(orientationType);
                    EditorGUILayout.PropertyField(faceTrackedObjectWhileClamped);
                }

                EditorGUILayout.PropertyField(reorientWhenOutsideParameters);
                if (reorientWhenOutsideParameters.boolValue)
                {
                    EditorGUILayout.PropertyField(orientToControllerDeadZoneDegrees);
                }
            }

            EditorGUILayout.Space();
            distanceFoldout = EditorGUILayout.Foldout(distanceFoldout, "Distance", true);

            if (distanceFoldout)
            {
                EditorGUILayout.PropertyField(ignoreDistanceClamp);
                if (!ignoreDistanceClamp.boolValue)
                {
                    EditorGUILayout.PropertyField(minDistance);
                    EditorGUILayout.PropertyField(maxDistance);
                    EditorGUILayout.PropertyField(defaultDistance);
                    EditorGUILayout.PropertyField(verticalMaxDistance);
                }
                else
                {
                    EditorGUILayout.HelpBox("Disable \"Ignore Distance Clamp\" to show options", MessageType.Info);
                }
            }

            EditorGUILayout.Space();
            directionFoldout = EditorGUILayout.Foldout(directionFoldout, "Direction", true);

            if (directionFoldout)
            {
                EditorGUILayout.PropertyField(ignoreAngleClamp);
                if (!ignoreAngleClamp.boolValue)
                {
                    EditorGUILayout.PropertyField(ignoreReferencePitchAndRoll);
                    if (ignoreReferencePitchAndRoll.boolValue)
                    {
                        EditorGUILayout.PropertyField(pitchOffset);
                    }

                    EditorGUILayout.PropertyField(angularClampMode);

                    switch ((Follow.AngularClampType)angularClampMode.intValue)
                    {
                        case Follow.AngularClampType.AngleStepping:
                            {
                                EditorGUILayout.PropertyField(tetherAngleSteps);
                            }
                            break;
                        case Follow.AngularClampType.ViewDegrees:
                            {
                                EditorGUILayout.PropertyField(maxViewHorizontalDegrees);
                                EditorGUILayout.PropertyField(maxViewVerticalDegrees);
                            }
                            break;

                        case Follow.AngularClampType.RendererBounds:
                        case Follow.AngularClampType.ColliderBounds:
                            {
                                EditorGUILayout.PropertyField(boundsScaler);
                            }
                            break;
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Disable \"Ignore Angle Clamp\" to show options", MessageType.Info);
                }
            }

            // reset foldouts style
            style.fontStyle = previousStyle;

            serializedObject.ApplyModifiedProperties();
        }
    }
}

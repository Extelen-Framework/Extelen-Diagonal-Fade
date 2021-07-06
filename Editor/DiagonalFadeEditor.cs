using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DiagonalFade))]
public class DiagonalFadeEditor : UnityEditor.Editor
{
    //Set Params
    private SerializedProperty m_rect = null;
    private SerializedProperty m_childRect = null;

    private SerializedProperty m_animationCurve = null;
    private SerializedProperty m_animationTime = null;

    private SerializedProperty m_rectOffset = null;
    private SerializedProperty m_inverseHorizontal = null;
    private SerializedProperty m_inverseVertical = null;

    private SerializedProperty m_loadingPercentCall = null;

    //Methods
    private void OnEnable()
    {
        m_rect = serializedObject.FindProperty("m_rect");
        m_childRect = serializedObject.FindProperty("m_childRect");

        m_animationCurve = serializedObject.FindProperty("m_animationCurve");
        m_animationTime = serializedObject.FindProperty("m_animationTime");

        m_rectOffset = serializedObject.FindProperty("m_rectOffset");
        m_inverseHorizontal = serializedObject.FindProperty("m_inverseHorizontal");
        m_inverseVertical = serializedObject.FindProperty("m_inverseVertical");

        m_loadingPercentCall = serializedObject.FindProperty("m_loadingPercentCall");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_rect);
        EditorGUILayout.PropertyField(m_childRect);

        Object m_rectIsAssigned = m_rect.objectReferenceValue;
        Object m_childIsAssigned = m_childRect.objectReferenceValue;

        if (m_rectIsAssigned == m_childIsAssigned)
        {
            EditorGUILayout.HelpBox("You can't use the same rect for the child.", MessageType.Error);
        }

        else
        {
            if (m_rect.objectReferenceValue && m_childRect.objectReferenceValue)
            {

                EditorGUILayout.PropertyField(m_animationCurve);
                EditorGUILayout.PropertyField(m_animationTime);

                EditorGUILayout.PropertyField(m_rectOffset);
                EditorGUILayout.PropertyField(m_inverseHorizontal);
                EditorGUILayout.PropertyField(m_inverseVertical);

                EditorGUILayout.PropertyField(m_loadingPercentCall);
            }
            else
            {
                EditorGUILayout.HelpBox("Child rect is not assigned.", MessageType.Error);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
using System;
using UnityEditor;
using UnityEngine;

public class CustomUtilityEditor : EditorWindow
{
    private static CustomUtilityEditor window;

    private GUIStyle titleStyle;

    [MenuItem("Window/CustomUtilityEditor/CustomUtility - Custom Editor")]
    private static void Setup()
    {
        window = GetWindow<CustomUtilityEditor>();
        window.titleContent = new GUIContent("CustomUtilityEditor");
        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(1920, 1080);
    }

    private void Awake()
    {
        titleStyle = new GUIStyle(EditorStyles.label);
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.yellow;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Custom Utility", titleStyle);
        
        // 실행할 씬 선택 [0번 씬부터, 현재 씬부터]
        Editor_SelectPlayScene.OnEditorGUI(titleStyle);
        
        // 씬 자동 저장
        Editor_SceneAutoSave.OnEditorGUI(titleStyle);
        
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CustomUtility.Editor
{
    public static class EditorSelectPlayScene
    {
        private const string Prefs = "CUSTOMUTILITY_SELECTPLAYSCENE_";
        private static readonly string[] PlaySceneTable = { "0번 씬부터 재생", "현재 씬부터 재생" };

        private static int _selectedIndex = 0;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Load();
        }

        public static void OnEditorGUI(GUIStyle titleStyle)
        {
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField("Select Play Scene", titleStyle);

            EditorGUI.BeginChangeCheck();
            _selectedIndex = EditorGUILayout.Popup("재생 씬 선택", _selectedIndex, PlaySceneTable);

            if (!EditorGUI.EndChangeCheck()) return;

            Save();

            switch (_selectedIndex)
            {
                case 0:
                    StartFromFirstScene();
                    break;
                case 1:
                    StartFromCurrentScene();
                    break;
            }
        }

        private static void Load()
        {
            _selectedIndex = EditorPrefs.GetInt($"{Prefs}{nameof(_selectedIndex)}");
        }

        private static void Save()
        {
            EditorPrefs.SetInt($"{Prefs}{nameof(_selectedIndex)}", _selectedIndex);
        }

        private static void StartFromFirstScene()
        {
            string pathOfFirstScene = EditorBuildSettings.scenes[0].path;
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);

            EditorSceneManager.playModeStartScene = sceneAsset;
        }

        private static void StartFromCurrentScene()
        {
            EditorSceneManager.playModeStartScene = null;
        }
    }
}
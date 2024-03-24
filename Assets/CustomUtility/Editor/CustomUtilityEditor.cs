using UnityEditor;
using UnityEngine;

// 새로운 윈도우를 생성할 수 있도록 EditorWindow 상속
namespace CustomUtility.Editor
{
    public class CustomUtilityEditor : EditorWindow
    {
        // 윈도우 정보
        private static CustomUtilityEditor _window;

        // 제목 Text의 스타일
        private GUIStyle _titleStyle;

        // Window/CustomUtilityEditor 경로에 CustomUtility - Custom Editor 메뉴 생성 
        [MenuItem("Window/CustomUtilityEditor/CustomUtility - Custom Editor")]
        // 메뉴 실행 함수
        private static void Setup()
        {
            // 윈도우가 이미 열려 있으면 인스턴스를 얻어오고, 없으면 새로운 윈도우를 생성한 후 활성화한다
            _window = GetWindow<CustomUtilityEditor>();
            // 윈도우 제목, 최소 사이즈, 최대 사이즈 설정
            _window.titleContent = new GUIContent("CustomUtilityEditor");
            _window.minSize = new Vector2(300, 300);
            _window.maxSize = new Vector2(1920, 1080);
        }

        // 새창이 열리면 호출
        private void Awake()
        {
            // 제목 스타일 설정
            SetTitleStyle();
        }

        // GUI 이벤트가 발생할 때 호출
        private void OnGUI()
        {
            // 제목 스타일이 할당되지 않았을 경우 실행
            if (_titleStyle is null)
            {
                // 제목 스타일 설정
                SetTitleStyle();
            }
            // 제목 스타일 텍스트 표출
            EditorGUILayout.LabelField("Custom Utility", _titleStyle);

            // 실행할 씬 선택 [0번 씬부터, 현재 씬부터]
            EditorSelectPlayScene.OnEditorGUI(_titleStyle);

            // 씬 자동 저장
            EditorSceneAutoSave.OnEditorGUI(_titleStyle);
        }

        // 인스펙터를 업데이트할 때 호출
        private void OnInspectorUpdate()
        {
            // GUI 상태를 업데이트
            Repaint();
        }

        // 제목 Text의 스타일을 설정하는 함수
        private void SetTitleStyle()
        {
            // 현재 유니티 에디터의 label과 동일한 스타일 할당, 폰트 굵기와 색상 설정
            _titleStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.yellow
                }
            };
        }
    }
}
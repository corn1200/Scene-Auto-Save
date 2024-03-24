using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CustomUtility.Editor
{
    public static class EditorSceneAutoSave
    {
        // 레지스트리 Key
        private const string Prefs = "CUSTOMUTILITY_SCENEAUTOSAVE_";

        // 저장 주기 배열
        private static readonly string[] TimeTable = { "5분", "10분", "30분", "1시간", "Custom" };

        private static readonly double[] TimeToSeconds = { 300, 600, 1800, 3600 };

        // 시간 표기방식
        private const string TimeNotation = "[yyyy-MM-dd] HH:mm:ss";

        // 자동 저장 On/Off 여부
        private static bool _isActivated = false;

        // 저장 시간 로그 펼쳐짐 여부
        private static bool _isShowLogExpanded = false;

        // 저장 주기 순번 정보
        private static int _selcetedTimeTableIndex = 0;

        // 저장 주기, 다음 저장까지 남은 시간, 마지막 저장 시간
        private static double _saveCycle = 0;
        private static double _nextSaveRemainingTime = 0;
        private static DateTime _lastSaveTime = DateTime.Now;

        // 에디터가 로드되는 시점(프로젝트 실행, 컴파일, "Play" 버튼 클릭)에 호출
        [InitializeOnLoadMethod]
        // 설정 초기화 함수
        private static void Initialize()
        {
            // 저장된 정보 불러오기
            Load();

            // 마지막 저장 시간 초기화
            _lastSaveTime = DateTime.Now;

            // Update 이벤트에 등록된 모든 메소드 불러오기
            Delegate[] handlers = EditorApplication.update.GetInvocationList();

            // 자동 저장 함수 Update 이벤트 기등록 여부
            bool hasAlready = handlers.Any(handler => handler.Method.Name.Equals(nameof(UpdateAutoSave)));

            // 자동 저장 함수가 Update 이벤트에 등록되어 있지 않을 경우 실행
            if (hasAlready == false)
            {
                // Update 이벤트에 자동 저장 함수 등록
                EditorApplication.update += UpdateAutoSave;
            }
        }

        // GUI 생성 함수
        public static void OnEditorGUI(GUIStyle titleStyle)
        {
            // 공백, 제목 설정
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField("Scene Auto Save", titleStyle);

            // GUI 요소 변경 여부 확인 시작
            EditorGUI.BeginChangeCheck();

            // 자동저장 On/Off Toggle UI
            string textActive = _isActivated ? "ON" : "OFF";
            _isActivated = EditorGUILayout.Toggle($"자동 저장 {textActive}", _isActivated);

            // 토글 활성화 여부에 따라 GUI 활성화 시작 
            EditorGUI.BeginDisabledGroup(!_isActivated);

            // 저장 주기 Dropdown UI
            _selcetedTimeTableIndex = EditorGUILayout.Popup("저장 주기", _selcetedTimeTableIndex, TimeTable);
            // 저장 주기를 "Custom"으로 설정했을 때 (초 단위의 저장 주기 입력)
            if (_selcetedTimeTableIndex == TimeTable.Length - 1)
            {
                // 커스텀 저장 주기 들여쓰기
                EditorGUI.indentLevel++;
                _saveCycle = EditorGUILayout.DoubleField("시간(초) : ", _saveCycle);
                EditorGUI.indentLevel--;

                // 저장 주기는 최소 10초 이상
                if (_saveCycle < 10)
                {
                    _saveCycle = 10;
                }
            }
            else
            {
                // 선택한 저장 주기 설정
                _saveCycle = TimeToSeconds[_selcetedTimeTableIndex];
            }

            // 공백 생성
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);
            // 로그 정보 접기/펼치기 가능한 GUI 영역 생성
            _isShowLogExpanded = EditorGUILayout.Foldout(_isShowLogExpanded, "로그 정보", true);
            if (_isShowLogExpanded)
            {
                // 들여쓰기
                EditorGUI.indentLevel++;
                // 마지막 저장 시간과 다음 저장까지 남은 시간 표시
                EditorGUILayout.LabelField($"마지막 저장 시간 : {_lastSaveTime.ToString(TimeNotation)}");
                EditorGUILayout.LabelField($"다음 저장까지 남은 시간 : {_nextSaveRemainingTime:00.00}");
                EditorGUI.indentLevel--;
            }

            // 토글 활성화 여부에 따라 GUI 활성화 종료
            EditorGUI.EndDisabledGroup();

            // GUI 요소 변경되었을 경우 실행
            if (EditorGUI.EndChangeCheck())
            {
                // 에디터 정보 저장
                Save();
            }
        }

        // 저장된 에디터 정보 불러오는 함수
        private static void Load()
        {
            Debug.Log("Editor_SceneAutoSave::Load()");

            // 레지스트리에 저장된 값으로 변수 초기화
            _isActivated = EditorPrefs.GetBool($"{Prefs}{nameof(_isActivated)}", false);
            _isShowLogExpanded = EditorPrefs.GetBool($"{Prefs}{nameof(_isShowLogExpanded)}", false);
            _selcetedTimeTableIndex = EditorPrefs.GetInt($"{Prefs}{nameof(_selcetedTimeTableIndex)}", 0);
            _saveCycle = EditorPrefs.GetFloat($"{Prefs}{nameof(_saveCycle)}", 300f);
        }

        // 에디터 정보 저장하는 함수
        private static void Save()
        {
            Debug.Log("Editor_SceneAutoSave::Save()");

            // 에디터 정보를 레지스트리에 저장
            EditorPrefs.SetBool($"{Prefs}{nameof(_isActivated)}", _isActivated);
            EditorPrefs.SetBool($"{Prefs}{nameof(_isShowLogExpanded)}", _isShowLogExpanded);
            EditorPrefs.SetInt($"{Prefs}{nameof(_selcetedTimeTableIndex)}", _selcetedTimeTableIndex);
            EditorPrefs.SetFloat($"{Prefs}{nameof(_saveCycle)}", (float)_saveCycle);
        }

        // 자동 저장 함수
        private static void UpdateAutoSave()
        {
            // 현재 시간 초기화
            DateTime dateTime = DateTime.Now;

            // 자동 저장이 비활성화되어 있거나 플레이 모드일 경우 실행
            if (!_isActivated || EditorApplication.isPlaying)
            {
                // 다음 저장까지 남은 시간을 저장 주기로 초기화
                _nextSaveRemainingTime = _saveCycle;

                // 함수 종료
                return;
            }

            // 현재 시간과 마지막 저장 시간 간격 계산
            double diff = dateTime.Subtract(_lastSaveTime).TotalSeconds;

            // 다음 저장까지 남은 시간 초기화
            _nextSaveRemainingTime = _saveCycle - diff;

            // 다음 저장까지 남은 시간이 0보다 작을 경우 실행
            if (_nextSaveRemainingTime < 0)
            {
                // 다음 저장까지 남은 시간 0으로 초기화
                _nextSaveRemainingTime = 0;
            }

            // 현재 시간과 마지막 저장 시간의 간격이 저장 주기보다 클 경우 실행 
            if (diff > _saveCycle)
            {
                Debug.Log("Editor_SceneAutoSave::UpdateAutoSave() - Save Scene");

                // 현재 열려 있는 모든 씬을 저장
                EditorSceneManager.SaveOpenScenes();
                // 마지막 저장 시간을 현재 시간으로 초기화
                _lastSaveTime = dateTime;
            }
        }
    }
}
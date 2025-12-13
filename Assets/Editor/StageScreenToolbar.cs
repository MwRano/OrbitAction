using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// ステージのスクリーン設定用ツールバー
    /// </summary>
    public class StageScreenToolbar : EditorWindow
    {
        private const string HeightKey = "StageScreen.Height";
        private const string WidthKey = "StageScreen.Width";
        private const string IsAutoFitKey = "StageScreen.IsAutoFit";

        private void OnGUI()
        {
            // 設定の読み込み
            var height = EditorPrefs.GetFloat(HeightKey, 18f);
            var width = EditorPrefs.GetFloat(WidthKey, 20f);
            var isAutoFit = EditorPrefs.GetBool(IsAutoFitKey, true);

            EditorGUI.BeginChangeCheck();

            // ステージのスクリーンサイズ設定
            EditorGUILayout.LabelField("Stage Screen Size", EditorStyles.boldLabel);
            height = EditorGUILayout.FloatField("Height", height);
            width = EditorGUILayout.FloatField("Width", width);

            // ステージのコライダー自動調整設定
            EditorGUILayout.LabelField("Collider Auto Fitting", EditorStyles.boldLabel);
            isAutoFit = EditorGUILayout.Toggle("Auto Fit", isAutoFit);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetFloat(HeightKey, height);
                EditorPrefs.SetFloat(WidthKey, width);
                EditorPrefs.SetBool(IsAutoFitKey, isAutoFit);
            }
        }

        [MenuItem("Tools/Stage Screen Settings")]
        private static void Open()
        {
            GetWindow<StageScreenToolbar>("Stage Screen Settings");
        }
    }
}
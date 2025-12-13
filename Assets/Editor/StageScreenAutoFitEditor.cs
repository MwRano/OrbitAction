using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Editor
{
    /// <summary>
    /// ステージのスクリーンのコライダーの大きさを自動調整するエディタ拡張
    /// </summary>
    [InitializeOnLoad]
    public static class StageScreenAutoFitEditor
    {
        private const string HeightKey = "StageScreen.Height";
        private const string WidthKey = "StageScreen.Width";

        static StageScreenAutoFitEditor()
        {
            Tilemap.tilemapTileChanged += OnTileMapChanged;
        }

        private static void OnTileMapChanged(Tilemap tilemap, Tilemap.SyncTile[] changes)
        {
            var isAutoFit = EditorPrefs.GetBool("StageScreen.IsAutoFit", false);
            if (tilemap.gameObject.layer != LayerMask.NameToLayer("Ground") || !isAutoFit)
                return;　// Groundレイヤーのタイルマップのみ処理

            var box = tilemap.transform.root.gameObject.GetComponent<BoxCollider2D>();
            if (box == null) return;

            FitBoxCollider(tilemap, box);
        }

        // タイルマップに合わせてBoxCollider2Dを調整
        private static void FitBoxCollider(Tilemap tilemap, BoxCollider2D box)
        {
            tilemap.CompressBounds();

            // 設定の読み込み
            var height = EditorPrefs.GetFloat(HeightKey, 18f);
            var width = EditorPrefs.GetFloat(WidthKey, 20f);

            // タイルマップの境界に基づいてサイズと中心を計算
            var bounds = tilemap.localBounds;
            var sizeX = Mathf.Max(bounds.max.x - bounds.min.x, width);
            var sizeY = Mathf.Max(bounds.max.y - bounds.min.y, height);
            var centerX = bounds.min.x + sizeX * 0.5f;
            var centerY = bounds.min.y + sizeY * 0.5f;

            Undo.RecordObject(box, "Auto Fit Stage Collider");
            box.size = new Vector2(sizeX, sizeY);
            box.offset = new Vector2(centerX, centerY);

            EditorUtility.SetDirty(box);
        }
    }
}
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

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
            if (!isAutoFit)
                return;

            if (tilemap.gameObject.layer != LayerMask.NameToLayer("Ground"))
                return;　// Groundレイヤーのタイルマップのみ処理

            var box = tilemap.transform.root.gameObject.GetComponent<BoxCollider2D>();
            if (box == null)
                return;

            // Groundレイヤーのみ対象
            var tilemaps = box.GetComponentsInChildren<Tilemap>()
                .Where(c => c.gameObject.layer == LayerMask.NameToLayer("Ground"))
                .ToArray();

            FitBoxCollider(tilemaps, box);
        }

        // タイルマップに合わせてBoxCollider2Dを調整
        private static void FitBoxCollider(Tilemap[] tilemaps, BoxCollider2D box)
        {
            // 設定の読み込み
            var height = EditorPrefs.GetFloat(HeightKey, 18f);
            var width = EditorPrefs.GetFloat(WidthKey, 20f);

            // フィット後のスクリーン座標計算
            var origin = new Vector2(float.MinValue, float.MinValue);
            var topRight = new Vector2(float.MaxValue, float.MaxValue);

            // 対象のタイルマップ境界を取得
            foreach (var tm in tilemaps)
            {
                tm.CompressBounds();
                var bounds = tm.localBounds;

                origin.x = Mathf.Max(origin.x, bounds.max.x);
                origin.y = Mathf.Max(origin.y, bounds.max.y);
                topRight.x = Mathf.Min(topRight.x, bounds.min.x);
                topRight.y = Mathf.Min(topRight.y, bounds.min.y);
            }

            // サイズと中心位置計算
            var sizeX = Mathf.Max(origin.x - topRight.x, width);
            var sizeY = Mathf.Max(origin.y - topRight.y, height);
            var centerX = topRight.x + sizeX * 0.5f;
            var centerY = topRight.y + sizeY * 0.5f;

            // BoxCollider2Dの更新
            Undo.RecordObject(box, "Auto Fit Stage Collider");
            box.size = new Vector2(sizeX, sizeY);
            box.offset = new Vector2(centerX, centerY);

            EditorUtility.SetDirty(box);
        }
    }
}
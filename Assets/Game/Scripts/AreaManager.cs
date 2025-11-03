using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Areaの出入りでカメラを切り替えるクラス
    /// </summary>
    public class AreaManager : MonoBehaviour
    {
        private const int ActivePriority = 11; // アクティブ時の優先度
        private const int InactivePriority = 10; // 非アクティブ時の優先度

        [SerializeField] private CinemachineCamera virtualCamera; // このエリアが管理するvcam

        [SerializeField] private Transform respawnPoint; // リスポーンポイント

        // プレイヤーがこのエリアに入った時
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            virtualCamera.Priority = ActivePriority;
            virtualCamera.Target.TrackingTarget = other.transform; // ターゲットをプレイヤーに設定
            other.gameObject.GetComponent<PlayerController>().SetRespawnPosition(respawnPoint.position);
        }

        // プレイヤーがこのエリアから出た時
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            virtualCamera.Priority = InactivePriority;
        }
    }
}
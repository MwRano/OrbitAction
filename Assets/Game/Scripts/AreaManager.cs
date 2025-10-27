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
        [SerializeField]
        private CinemachineCamera virtualCamera; // このエリアが管理するvcam
        
        [SerializeField]
        private Transform respawnPoint; // リスポーンポイント

        private const int ActivePriority = 11; // アクティブ時の優先度
        private const int InactivePriority = 10; // 非アクティブ時の優先度

        // プレイヤーがこのエリアに入った時
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            virtualCamera.Priority = ActivePriority;
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
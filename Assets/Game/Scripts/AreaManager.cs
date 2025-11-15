using Player;
using Unity.Cinemachine;
using UnityEngine;
using R3;

namespace Game
{
    /// <summary>
    /// PlayerがAreaの出入りに応じた処理を行うクラス
    /// </summary>
    public class AreaManager : MonoBehaviour
    {
        private const int ActivePriority = 11; // アクティブ時の優先度
        private const int InactivePriority = 10; // 非アクティブ時の優先度

        [SerializeField] private CinemachineCamera virtualCamera; // このエリアが管理するvcam
        [SerializeField] private Transform respawnPoint; // リスポーンポイント
        [SerializeField] private CinemachineImpulseSource impulseSource;

        // プレイヤーがこのエリアに入った時
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            virtualCamera.Priority = ActivePriority;
            virtualCamera.Target.TrackingTarget = other.transform; // ターゲットをプレイヤーに設定
            PlayerController player = other.GetComponent<PlayerController>();
            player.SetRespawnPosition(respawnPoint.position);
        }

        // プレイヤーがこのエリアから出た時
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            virtualCamera.Priority = InactivePriority;
        }
    }
}
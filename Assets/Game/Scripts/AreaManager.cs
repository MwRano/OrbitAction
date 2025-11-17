using Player;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Game
{
    /// <summary>
    /// PlayerがAreaの出入りに応じた処理を行うクラス
    /// </summary>
    public class AreaManager : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera virtualCamera; // このエリアが管理するvcam
        [SerializeField] private Transform respawnPoint; // リスポーンポイント
        [SerializeField] private CinemachineImpulseSource impulseSource;
        
        private PlayerRespawner _playerRespawner; // プレイヤーリスポーナー
        private const int ActivePriority = 11; // アクティブ時の優先度
        private const int InactivePriority = 10; // 非アクティブ時の優先度
        
        [Inject]
        public void Construct(PlayerRespawner playerRespawner) => _playerRespawner = playerRespawner;

        // プレイヤーがこのエリアに入った時
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            virtualCamera.Priority = ActivePriority;
            virtualCamera.Target.TrackingTarget = other.transform; // ターゲットをプレイヤーに設定
            _playerRespawner.SetRespawnPosition(respawnPoint.position);
        }

        // プレイヤーがこのエリアから出た時
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            virtualCamera.Priority = InactivePriority;
        }
        
    }
}
using Unity.Cinemachine;
using UnityEngine;

namespace Orbit.Game
{
    public class AreaManager : MonoBehaviour
    {
        private const int ActivePriority = 11;
        private const int InactivePriority = 10;
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private CinemachineImpulseSource impulseSource;

        // プレイヤーがこのエリアに入った時
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            virtualCamera.Priority = ActivePriority;
            virtualCamera.Target.TrackingTarget = other.transform;
        }

        // プレイヤーがこのエリアから出た時
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            virtualCamera.Priority = InactivePriority;
        }
    }
}
using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Areaの出入りでカメラを切り替えるクラス
    /// </summary>
    public class AreaCameraManager : MonoBehaviour
    {
        [SerializeField]
        private CinemachineCamera virtualCamera; // このエリアが管理するvcam

        private const int ActivePriority = 11; // アクティブ時の優先度
        private const int InactivePriority = 10; // 非アクティブ時の優先度

        // プレイヤーがこのエリアに入った時
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                virtualCamera.Priority = ActivePriority;
            }
        }

        // プレイヤーがこのエリアから出た時
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                virtualCamera.Priority = InactivePriority;
            }
        }
    }
}
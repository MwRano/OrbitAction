using UnityEngine;
using R3;
using System.Linq;
using VContainer.Unity;

namespace Player
{
    public class PlayerEffector : IStartable
    {
        private PlayerEffectParams _playerEffects;

        public PlayerEffector(PlayerCore player,
            PlayerEffectParams playerEffects)
        {
            var effects = playerEffects.PlayerEffects;

            // 着地エフェクト
            var landEffect = effects.FirstOrDefault(e => e.EffectType == PlayerEffectType.Land);
            player.IsGrounded
                .Where(isGrounded => isGrounded && landEffect != null)
                .Subscribe(_ =>
                    PlayEffect(landEffect.EffectPrefab,
                        (Vector2)player.Rb.transform.position + landEffect.SpawnOffset))
                .AddTo(player);

            // 滑るエフェクト（方向転換）
            var slipEffect = effects.FirstOrDefault(e => e.EffectType == PlayerEffectType.Slip);
            Observable.EveryValueChanged(player, p => p.Sprite.flipX)
                .Where(_ => player.IsGrounded.CurrentValue && slipEffect != null)
                .Subscribe(_ =>
                    PlayEffect(slipEffect.EffectPrefab,
                        (Vector2)player.Rb.transform.position + slipEffect.SpawnOffset))
                .AddTo(player);
        }

        public void Start()
        {
            // Do nothing
        }

        private void PlayEffect(GameObject effectPrefab, Vector2 position)
        {
            var effect = Object.Instantiate(effectPrefab, position, Quaternion.identity);
            Object.Destroy(effect, 1.0f);
        }
    }
}
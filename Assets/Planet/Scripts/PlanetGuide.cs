#nullable enable
using Player;
using UnityEngine;
using VContainer;
using R3;

namespace Planet
{
    public class PlanetGuide : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer planetView = null!;
        private DeployPositionCalculator _deployPositionCalculator = null!;
        private PlanetController _planet = null!;
        private PlanetStateMachine _planetStateMachine = null!;

        private PlayerCore _player = null!;
        private PlayerAimer _playerAimer = null!;

        private void Start()
        {
            // deploy状態のときは非表示にする
            Observable.EveryValueChanged(_planetStateMachine, sm => sm.CurrentState)
                .Subscribe(_ =>
                    planetView.enabled = _planetStateMachine.CurrentState != _planetStateMachine.Deploy)
                .AddTo(this);

            // プレイヤーの位置と向きに応じてガイドの位置を更新する
            var lookingObs = Observable.EveryValueChanged(_playerAimer, p => p.AimDirection).Select(v => (object)v);
            var positionObs = Observable.EveryValueChanged(_player, p => p.Rb.transform.position)
                .Select(v => (object)v);
            Observable.Merge(lookingObs, positionObs)
                .Subscribe(_ =>
                    planetView.transform.position = _deployPositionCalculator.Calculate(
                        _player.Rb.transform.position,
                        _playerAimer.AimDirection,
                        _planet.PlanetTransform.position,
                        _planet.PlanetSpriteRenderer.bounds.extents.x))
                .AddTo(this);
        }

        [Inject]
        public void Construct(
            PlayerCore player,
            PlayerAimer playerAimer,
            PlanetController planet,
            DeployPositionCalculator deployPositionCalculator,
            PlanetStateMachine planetStateMachine)
        {
            _player = player;
            _playerAimer = playerAimer;
            _planet = planet;
            _deployPositionCalculator = deployPositionCalculator;
            _planetStateMachine = planetStateMachine;
        }
    }
}
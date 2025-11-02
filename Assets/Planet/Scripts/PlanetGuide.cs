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

        private PlayerController _player = null!;

        private void Start()
        {
            // deploy状態のときは非表示にする
            Observable.EveryValueChanged(_planetStateMachine, sm => sm.CurrentState)
                .Subscribe(_ =>
                    planetView.enabled = _planetStateMachine.CurrentState != _planetStateMachine.Deploy)
                .AddTo(this);

            // プレイヤーの位置と向きに応じてガイドの位置を更新する
            var lookingObs = Observable.EveryValueChanged(_player, p => p.LookingDirection).Select(v => (object)v);
            var positionObs = Observable.EveryValueChanged(_player, p => p.PlayerTransform.position)
                .Select(v => (object)v);
            Observable.Merge(lookingObs, positionObs)
                .Subscribe(_ =>
                    planetView.transform.position = _deployPositionCalculator.Calculate(
                        _player.PlayerTransform.position,
                        _player.LookingDirection,
                        _planet.PlanetTransform.position,
                        _planet.PlanetSpriteRenderer.bounds.extents.x))
                .AddTo(this);
        }

        [Inject]
        public void Construct(
            PlayerController player,
            PlanetController planet,
            DeployPositionCalculator deployPositionCalculator,
            PlanetStateMachine planetStateMachine)
        {
            _player = player;
            _planet = planet;
            _deployPositionCalculator = deployPositionCalculator;
            _planetStateMachine = planetStateMachine;
        }
    }
}
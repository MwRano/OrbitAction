using Orbit.Player;
using R3;
using UnityEngine;
using VContainer;

namespace Orbit.Planet
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlanetGuide : MonoBehaviour
    {
        private SpriteRenderer _guidePlanetView;
        private DeployPositionCalculator _deployPositionCalculator;
        private PlanetCore _planet;
        private PlanetStateMachine _planetStateMachine;
        private PlayerCore _player;
        private PlayerAimer _playerAimer;
        
        [Inject]
        public void Construct(
            PlayerCore player,
            PlayerAimer playerAimer,
            PlanetCore planet,
            DeployPositionCalculator deployPositionCalculator,
            PlanetStateMachine planetStateMachine)
        {
            _player = player;
            _playerAimer = playerAimer;
            _planet = planet;
            _deployPositionCalculator = deployPositionCalculator;
            _planetStateMachine = planetStateMachine;
            
            _guidePlanetView = GetComponent<SpriteRenderer>();
        }
        
        private void Awake()
        {
            // deploy状態のときは非表示にする
            Observable.EveryValueChanged(_planetStateMachine, sm => sm.CurrentState)
                .Subscribe(_ =>
                    _guidePlanetView.enabled = _planetStateMachine.CurrentState != _planetStateMachine.Deploy)
                .AddTo(this);

            // プレイヤーの位置と向きに応じてガイドの位置を更新する
            var lookingObs = 
                Observable.EveryValueChanged(_playerAimer, p => p.AimDirection)
                .Select(v => (object)v);
            var positionObs = 
                Observable
                .EveryValueChanged(_player, p => p.transform.position)
                .Select(v => (object)v);
            Observable.Merge(lookingObs, positionObs)
                .Subscribe(_ =>
                    _guidePlanetView.transform.position = _deployPositionCalculator.Calculate(
                        _player.transform.position,
                        _playerAimer.AimDirection,
                        _planet.transform.position,
                        _planet.PlanetView.bounds.extents.x))
                .AddTo(this);
        }
        
    }
}
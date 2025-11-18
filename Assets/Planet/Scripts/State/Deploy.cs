using System;
using LitMotion;
using LitMotion.Extensions;
using Orbit.Core.StateMachine;
using UnityEngine;
using VContainer;

namespace Orbit.Planet.State
{
    /// <summary>
    /// planetの設置後の状態
    /// </summary>
    public class Deploy : IState<PlanetStateMachine>
    {
        private readonly PlanetParams _planetParams;
        private MotionHandle _floatingMotion;
        private readonly PlanetCore _planet;
        private readonly PlanetInput _planetInput;

        [Inject]
        public Deploy(
            PlanetParams planetParams, 
            PlanetCore planet,
            PlanetInput planetInput)
        {
            _planetParams = planetParams;
            _planet = planet;
            _planetInput = planetInput;
        }

        public void Enter()
        {
            // 浮遊モーション
            _floatingMotion = LMotion
                .Create(_planet.transform.position.y, _planet.transform.position.y - 0.2f, 1f)
                .WithEase(Ease.InOutSine)
                .WithLoops(-1, LoopType.Yoyo)
                .BindToPositionY(_planet.transform)
                .AddTo(_planet);
            
            // 拡大モーション
            _planet.OrbitableAreaView.enabled = true;
            var baseRadius = _planet.OrbitableAreaView.sprite.bounds.extents.x;
            CreateScaleMotion(Vector2.zero, Vector2.one * (_planetParams.OrbitalRange / baseRadius));
        }
        
        public void Update(PlanetStateMachine stateMachine)
        {
            // 状態遷移の判定
            if (!_planetInput.Launch.CurrentValue)
                stateMachine.TransitionTo(stateMachine.Follow);
        }
        
        public void Exit()
        {
            _floatingMotion.Cancel();
            
            // 縮小モーション
            CreateScaleMotion(_planet.OrbitableAreaView.transform.localScale, 
                Vector2.zero,
                () => _planet.OrbitableAreaView.enabled = false);
        }
        
        private void CreateScaleMotion(Vector2 fromScale, Vector2 toScale, Action onComplete = null)
        {
            var motionBuilder = LMotion.Create(fromScale, toScale, 0.3f)
                .WithEase(Ease.OutBack);
            
            if (onComplete != null)
            {
                motionBuilder = motionBuilder.WithOnComplete(onComplete);
            }
            
            motionBuilder.BindToLocalScaleXY(_planet.OrbitableAreaView.transform)
                .AddTo(_planet.OrbitableAreaView);
        }
    }
}
using System;
using Orbit.Player;
using UnityEngine;
using VContainer;

namespace Orbit.Planet
{
    public class DeployPositionCalculator
    {
        private readonly PlanetParams _planetParams;
        private readonly PlayerParam _playerParam;

        [Inject]
        public DeployPositionCalculator(
            PlanetParams planetParams,
            PlayerParam playerParam)
        {
            _planetParams = planetParams;
            _playerParam = playerParam;
        }

        public Vector2 Calculate(Vector2 playerPos, Vector2 playerLookDir, Vector2 planetPos, float planetRadius)
        {
            var hit = Physics2D.Raycast(playerPos, playerLookDir, 100,　_playerParam.GroundLayer);
            var target = hit.point;

            // 補正量を計算
            var planetToHitDir = (hit.point - planetPos).normalized;
            var planetToHitDist = Vector2.Distance(hit.point, planetPos);

            var distToGround = Math.Abs(hit.normal.x) > Math.Abs(hit.normal.y)
                ? Mathf.Abs(hit.point.x - planetPos.x)
                : Mathf.Abs(hit.point.y - planetPos.y);
            if (distToGround == 0) return target;

            // 壁や地面にplanetがめり込まないように補正
            target = hit.point + -planetToHitDir * (planetRadius * (planetToHitDist / distToGround));

            // 発射可能距離を超えていたら or 壁に到達しないときに補正
            if ((target - playerPos).sqrMagnitude > _planetParams.LaunchDistanceSqr || !hit)
            {
                target = playerPos + playerLookDir * _planetParams.LaunchDistance;
            }

            return target;
        }
    }
}
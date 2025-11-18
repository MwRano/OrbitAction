using Orbit.Planet;
using Orbit.Player;

namespace Orbit.Game
{
    /// <summary>
    /// ゲームクリア時の演出を制御するクラス
    /// </summary>
    public class GameClearDirector
    {
        private PlanetController _planetController = null!;
        private PlanetParams _planetParams = null!;
        private PlayerCore _player = null!;

        public GameClearDirector(
            PlayerCore player,
            PlanetController planetController,
            PlanetParams planetParams)
        {
            _player = player;
            _planetController = planetController;
            _planetParams = planetParams;
        }

        // public void Initialize()
        // {
        //     // ゴール地点に到達したらクリア演出を開始
        //     _player.IsGoalReached
        //         .Where(isReached => isReached)
        //         .Subscribe(_ => StartClearDirection())
        //         .AddTo(_player);
        // }
        //
        // private void StartClearDirection()
        // {
        //     _player
        //         .StartClearMotionAsync(_planetController.transform, _planetParams.OrbitalRange)
        //         .Forget();
        // }
    }
}
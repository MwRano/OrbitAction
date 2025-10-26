#nullable enable
using Cysharp.Threading.Tasks;
using Planet;
using Player;
using VContainer;
using R3;
using VContainer.Unity;

namespace Game
{
    /// <summary>
    /// ゲームクリア時の演出を制御するクラス
    /// </summary>
    public class GameClearDirector : IStartable
    {
        private PlanetController _planetController = null!;
        private PlanetParams _planetParams = null!;
        private PlayerController _playerController = null!;

        public void Start()
        {
            // ゴール地点に到達したらクリア演出を開始
            Observable.EveryValueChanged(_playerController, p => p.IsGoalReached)
                .Where(isReached => isReached)
                .Subscribe(_ => StartClearDirection())
                .AddTo(_playerController);
        }

        [Inject]
        public void Construct(
            PlayerController playerController,
            PlanetController planetController,
            PlanetParams planetParams)
        {
            _playerController = playerController;
            _planetController = planetController;
            _planetParams = planetParams;
        }

        private void StartClearDirection()
        {
            _playerController
                .StartClearMotionAsync(_planetController.transform, _planetParams.OrbitalRange)
                .Forget();
        }
    }
}
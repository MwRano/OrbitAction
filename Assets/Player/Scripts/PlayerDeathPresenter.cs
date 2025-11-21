using Orbit.Game;
using R3;
using VContainer;
using VContainer.Unity;

namespace Orbit.Player
{
    public class PlayerDeathPresenter : IInitializable
    {
        [Inject]
        public PlayerDeathPresenter(
            PlayerCore player,
            PlayerRespawner playerRespawner, 
            ScreenFader screenFader)
        {
            playerRespawner.IsRespawning
                .Where(isRespawning => isRespawning)
                .Subscribe(_ => screenFader.FadeOut())
                .AddTo(player);

            playerRespawner.IsRespawning
                .Where(isRespawning => !isRespawning)
                .Skip(1)
                .Subscribe(_ => screenFader.FadeIn())
                .AddTo(player);
        }

        public void Initialize()
        {
        }
    }
}
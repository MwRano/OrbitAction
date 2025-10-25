#nullable enable
using Game;
using UnityEngine;
using VContainer;
using UnityEngine.InputSystem;
using LitMotion;
using UnityEngine.SceneManagement;

namespace Title
{
    public class TitleManager : MonoBehaviour
    {
        private static readonly int Progress = Shader.PropertyToID("_Progress");
        [SerializeField] private SpriteRenderer wipeRenderer = null!;
        
        [Inject]
        public void Construct(
            InputSystemActions inputSystemActions)
        {
            inputSystemActions.Planet.Attract.performed += OnStartButtonPressed;
            inputSystemActions.Planet.Enable();
        }
        
        private void OnStartButtonPressed(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            TransitToGameScene();
        }
        
        private void TransitToGameScene()
        {
            LMotion.Create(1.1f, 0f, 1f)
                .WithEase(Ease.Linear)
                .WithOnComplete(() => SceneManager.LoadScene(nameof(SceneName.Game)))
                .Bind(value => wipeRenderer.material.SetFloat(Progress, value));
        }
    }
}


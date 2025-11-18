#nullable enable
using LitMotion;
using Orbit.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using VContainer;

namespace Orbit.Title
{
    public class TitleManager : MonoBehaviour
    {
        private static readonly int Progress = Shader.PropertyToID("_Progress");
        [SerializeField] private SpriteRenderer wipeRenderer = null!;
        
        [Inject]
        public void Construct(
            InputSystemActions inputSystemActions)
        {
            inputSystemActions.Planet.Orbit.performed += OnStartButtonPressed;
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


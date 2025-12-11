using UnityEngine;

namespace Orbit.Game
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioData audioData)
        {
            _audioSource.volume = audioData.Volume;
            _audioSource.pitch = audioData.Pitch;
            _audioSource.PlayOneShot(audioData.Clip);
        }    
    }
}
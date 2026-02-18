using UnityEngine;

namespace Wordle.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private AudioSource soundtrack;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            // Only call DontDestroyOnLoad in play mode (not in edit mode tests)
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (soundtrack == null)
            {
                soundtrack = GetComponent<AudioSource>();
            }

            if (soundtrack == null)
            {
                Debug.LogWarning($"{name} ({GetType().Name}): AudioSource for soundtrack not found.");
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void PlaySoundtrack()
        {
            if (soundtrack != null && !soundtrack.isPlaying)
            {
                soundtrack.Play();
            }
        }

        public void StopSoundtrack()
        {
            if (soundtrack != null && soundtrack.isPlaying)
            {
                soundtrack.Stop();
            }
        }
    }
}
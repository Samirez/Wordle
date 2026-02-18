using UnityEngine;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Wordle.Core
#pragma warning restore IDE0130 // Namespace does not match folder structure
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
            DontDestroyOnLoad(gameObject);

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
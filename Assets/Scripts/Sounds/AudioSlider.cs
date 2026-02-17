using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixMode audioMixMode = AudioMixMode.LinearAudioSourceVolume;
    [SerializeField] private Slider slider;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string exposedParameterName = "MasterVolume";

    [SerializeField] private TextMeshProUGUI valueText;

    public enum AudioMixMode
    {
        LinearAudioSourceVolume,
        LinearMixerVolume,
        LogarithmicMixerVolume,
    }

    void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnChangeSlider);
            OnChangeSlider(slider.value);
        }
    }

    void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnChangeSlider);
        }
    }
    
    public void OnChangeSlider(float value)
    {
        if (valueText != null)
        {
            valueText.SetText($"{value * 100:F0} %");
        }

        switch (audioMixMode)
        {
            case AudioMixMode.LinearAudioSourceVolume:
                if (audioSource != null)
                {
                    audioSource.volume = value;
                }
                break;
            case AudioMixMode.LinearMixerVolume:
                if (mixer != null)
                {
                    if (string.IsNullOrWhiteSpace(exposedParameterName))
                    {
                        Debug.LogWarning($"{name} ({GetType().Name}): Exposed mixer parameter name is empty.");
                        break;
                    }

                    mixer.SetFloat(exposedParameterName, Mathf.Lerp(-80f, 0f, value));
                }
                break;
            case AudioMixMode.LogarithmicMixerVolume:
                if (mixer != null)
                {
                    if (string.IsNullOrWhiteSpace(exposedParameterName))
                    {
                        Debug.LogWarning($"{name} ({GetType().Name}): Exposed mixer parameter name is empty.");
                        break;
                    }

                    float safeValue = Mathf.Max(value, 0.0001f);
                    mixer.SetFloat(exposedParameterName, Mathf.Log10(safeValue) * 20);
                }
                break;
        }
    }
}

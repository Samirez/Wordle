using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private AudioMixMode audioMixMode = AudioMixMode.LinearAudioSourceVolume;
    [SerializeField] private Slider slider;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixer Mixer;

    [SerializeField] private TextMeshProUGUI ValueText;

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
        if (ValueText != null)
        {
            ValueText.SetText($"{value * 100:F0} %");
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
                if (Mixer != null)
                {
                    Mixer.SetFloat("Volume", (-80 + value * 100));
                }
                break;
            case AudioMixMode.LogarithmicMixerVolume:
                if (Mixer != null)
                {
                    float safeValue = Mathf.Max(value, 0.0001f);
                    Mixer.SetFloat("Volume", Mathf.Log10(safeValue) * 20);
                }
                break;
        }
    }
}

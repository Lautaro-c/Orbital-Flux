using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider generalSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MasterParameter = "MasterVolume";
    private const string MusicParameter = "MusicVolume";
    private const string SfxParameter = "SFXVolume";

    private const string MasterKey = "Audio.Master";
    private const string MusicKey = "Audio.Music";
    private const string SfxKey = "Audio.SFX";

    private bool pendingSave;

    private void Start()
    {
        if (audioMixer == null)
        {
            Debug.LogError(
                "Asigná GameAudioMixer en AudioSettingsMenu.",
                this
            );
            return;
        }

        Initialize(
            generalSlider, MasterKey,
            MasterParameter, SetGeneralVolume
        );

        Initialize(
            musicSlider, MusicKey,
            MusicParameter, SetMusicVolume
        );

        Initialize(
            sfxSlider, SfxKey,
            SfxParameter, SetSfxVolume
        );
    }

    private void Initialize(
        Slider slider,
        string key,
        string parameter,
        UnityAction<float> callback)
    {
        float value = Mathf.Clamp01(PlayerPrefs.GetFloat(key, 1f));

        // Se aplica aunque no haya un slider asignado.
        ApplyVolume(parameter, value);

        if (slider == null)
            return;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.SetValueWithoutNotify(value);
        slider.onValueChanged.AddListener(callback);
    }

    private void SetGeneralVolume(float value)
    {
        ChangeVolume(MasterParameter, MasterKey, value);
    }

    private void SetMusicVolume(float value)
    {
        ChangeVolume(MusicParameter, MusicKey, value);
    }

    private void SetSfxVolume(float value)
    {
        ChangeVolume(SfxParameter, SfxKey, value);
    }

    private void ChangeVolume(
        string parameter,
        string key,
        float value)
    {
        value = Mathf.Clamp01(value);

        ApplyVolume(parameter, value);
        PlayerPrefs.SetFloat(key, value);
        pendingSave = true;
    }

    private void ApplyVolume(string parameter, float value)
    {
        // El slider usa 0..1; el Mixer usa decibelios.
        // -80 dB deja el sonido prácticamente inaudible.
        float decibels = value <= 0.0001f
            ? -80f
            : Mathf.Log10(value) * 20f;

        if (!audioMixer.SetFloat(parameter, decibels))
        {
            Debug.LogError(
                $"No se pudo cambiar '{parameter}'. " +
                "Comprobá que esté expuesto y bien escrito en el Mixer.",
                this
            );
        }
    }

    private void SaveSettings()
    {
        if (!pendingSave)
            return;

        PlayerPrefs.Save();
        pendingSave = false;
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            SaveSettings();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    private void OnDisable()
    {
        SaveSettings();
    }

    private void OnDestroy()
    {
        if (generalSlider != null)
            generalSlider.onValueChanged.RemoveListener(SetGeneralVolume);

        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);

        SaveSettings();
    }
}
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider sfxSlider;
    public Button BackButton;

    void Start()
    {
        // 加载保存的设置
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 0.5f);

        // 监听值变化
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);

        // 应用初始值
        ApplyVolume();
        ApplySFX();

        BackButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });

    }

    void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("Volume", value);
        ApplyVolume();
    }

    void OnSFXChanged(float value)
    {
        PlayerPrefs.SetFloat("SFX", value);
        ApplySFX();
    }

    void ApplyVolume()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.bgmVolume = volumeSlider.value;
            AudioManager.Instance.bgmSource.volume = volumeSlider.value;
        }

    }

    void ApplySFX()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.sfxVolume = sfxSlider.value;
    }
}
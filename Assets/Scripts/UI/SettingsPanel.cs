using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider ectSlider;

    private void OnEnable()
    {
        // AudioManager에서 현재 볼륨 가져와 슬라이더 초기화
        bgmSlider.value = AudioManager.Instance.GetVolume(EAudioMixerType.BGM);
        sfxSlider.value = AudioManager.Instance.GetVolume(EAudioMixerType.Sfx);
        ectSlider.value = AudioManager.Instance.GetVolume(EAudioMixerType.Ect);
    }
}

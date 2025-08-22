using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider ectSlider;

    private void OnEnable()
    {
        // AudioManager���� ���� ���� ������ �����̴� �ʱ�ȭ
        bgmSlider.value = AudioManager.Instance.GetVolume(EAudioMixerType.BGM);
        sfxSlider.value = AudioManager.Instance.GetVolume(EAudioMixerType.Sfx);
        ectSlider.value = AudioManager.Instance.GetVolume(EAudioMixerType.Ect);
    }
}

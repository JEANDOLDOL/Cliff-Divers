using UnityEngine;
using UnityEngine.Audio;

public enum EAudioMixerType { Master, BGM, Sfx, Ect }
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioMixer audioMixer;

    private bool[] isMute = new bool[System.Enum.GetValues(typeof(EAudioMixerType)).Length];
    private float[] audioVolumes = new float[System.Enum.GetValues(typeof(EAudioMixerType)).Length];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않게 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAudioVolume(EAudioMixerType audioMixerType, float volume)
    {
        float dB = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat(audioMixerType.ToString(), dB);
        audioVolumes[(int)audioMixerType] = dB; 
    }
    public float GetVolume(EAudioMixerType type)
    {
        return Mathf.Pow(10, audioVolumes[(int)type] / 20f);
    }


    public void SetAudioMute(EAudioMixerType audioMixerType)
    {
        int type = (int)audioMixerType;
        if (!isMute[type]) // 뮤트 
        {
            isMute[type] = true;
            audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
            audioVolumes[type] = curVolume;
            SetAudioVolume(audioMixerType, 0.001f);
        }
        else
        {
            isMute[type] = false;
            SetAudioVolume(audioMixerType, audioVolumes[type]);
        }
    }
    public void Mute()
    {
        AudioManager.Instance.SetAudioMute(EAudioMixerType.BGM);
    }

    public void ChangeVolume(float volume)
    {
        Debug.Log($"슬라이더로 받은 볼륨 값: {volume}");

        AudioManager.Instance.SetAudioVolume(EAudioMixerType.BGM, volume);
    }

    public void ChangeSfxVolume(float volume)
    {
        Debug.Log($"슬라이더로 받은 볼륨 값: {volume}");

        AudioManager.Instance.SetAudioVolume(EAudioMixerType.Sfx, volume);
    }
    public void ChangeEctVolume(float volume)
    {
        Debug.Log($"슬라이더로 받은 볼륨 값: {volume}");

        AudioManager.Instance.SetAudioVolume(EAudioMixerType.Ect, volume);
    }
}
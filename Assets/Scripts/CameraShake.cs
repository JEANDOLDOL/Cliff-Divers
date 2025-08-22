using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    CinemachineCamera vc;
    CinemachineBasicMultiChannelPerlin noise;

    [SerializeField] float lowAmp = 0.5f;
    [SerializeField] float midAmp = 1f;
    [SerializeField] float highAmp = 3f;
    [SerializeField] float freq = 1.0f;

    private void Awake()
    {
        vc = GetComponent<CinemachineCamera>();
    }

    void Start()
    {
        noise = vc.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCameraLow()
    {
        noise.AmplitudeGain = lowAmp;
        noise.FrequencyGain = freq;
    }

    public void ShakeCameraMid()
    {
        noise.AmplitudeGain = midAmp;
        noise.FrequencyGain = freq;
    }

    public void ShakeCameraHigh()
    {
        noise.AmplitudeGain = highAmp;
        noise.FrequencyGain = freq;
    }

    public void StopShakeCamera()
    {
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }
}

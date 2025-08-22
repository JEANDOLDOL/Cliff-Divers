using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] float defaultFOV = 75f;
    [SerializeField] float minFOV = 20f;
    [SerializeField] float maxFOV = 120f;

    [Header("Properties")]
    public float DefaultFOV => defaultFOV;
    public float MinFOV => minFOV;
    public float MaxFOV => maxFOV;

    [SerializeField] float zoomDuration = 1f;

    CinemachineCamera cinemachineCamera;

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    public void SetCameraFOV(float targetFOV)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeFOVRoutine(targetFOV));
    }


    IEnumerator ChangeFOVRoutine(float targetFOV)
    {
        float startFOV = cinemachineCamera.Lens.FieldOfView;
        float clampedFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / zoomDuration);

            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(startFOV, clampedFOV, t);
            yield return null;
        }
        cinemachineCamera.Lens.FieldOfView = clampedFOV;
    }

    public float GetCurrentFOV()
    {
        return cinemachineCamera.Lens.FieldOfView;
    }
}

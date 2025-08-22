using UnityEngine;

public class CameraPitchLimiter : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float minPitch = -30f;
    [SerializeField] float maxPitch = 60f;

    void LateUpdate()
    {
        Vector3 currentEuler = cameraTransform.localEulerAngles;
        float pitch = currentEuler.x;

        // ����Ƽ�� ������ 0~360���� ������ ������ 180�� �Ѿ�� ����
        if (pitch > 180) pitch -= 360;

        // ����
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // ����
        cameraTransform.localEulerAngles = new Vector3(pitch, currentEuler.y, currentEuler.z);
    }
}

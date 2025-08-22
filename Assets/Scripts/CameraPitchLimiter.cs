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

        // 유니티의 각도는 0~360도로 나오기 때문에 180도 넘어가면 보정
        if (pitch > 180) pitch -= 360;

        // 제한
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 적용
        cameraTransform.localEulerAngles = new Vector3(pitch, currentEuler.y, currentEuler.z);
    }
}

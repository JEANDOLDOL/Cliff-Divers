using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    CameraController controller;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.Restart();
        }
    }

    public void Init(CameraController controller)
    {
        this.controller = controller;
    }

    public void ChangeFOV(float targetFov)
    {
        if (controller != null)
        {
            controller.SetCameraFOV(targetFov);
        }
    }

}

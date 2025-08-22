using UnityEngine;
using TMPro;

public class CheckDistance : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;     // 거리 측정할 대상 (플레이어)
    [SerializeField] private TextMeshProUGUI distanceText;  // TMP UI Text

    void Update()
    {
        CheckDistanceToGround();
    }

    private void CheckDistanceToGround()
    {
        if (!playerRigidbody || !distanceText)
        {
            return;
        }

        // 플레이어 위치에서 아래 방향으로 Raycast
        Ray ray = new Ray(playerRigidbody.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            int distance = Mathf.RoundToInt(hit.distance); // 소수점 제거, 반올림
            distanceText.text = $"Distance to ground: {distance}";
        }
        else
        {
            distanceText.text = "No ground detected below!";
        }
    }
}

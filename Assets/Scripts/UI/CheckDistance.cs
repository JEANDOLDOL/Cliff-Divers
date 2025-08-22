using UnityEngine;
using TMPro;

public class CheckDistance : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;     // �Ÿ� ������ ��� (�÷��̾�)
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

        // �÷��̾� ��ġ���� �Ʒ� �������� Raycast
        Ray ray = new Ray(playerRigidbody.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            int distance = Mathf.RoundToInt(hit.distance); // �Ҽ��� ����, �ݿø�
            distanceText.text = $"Distance to ground: {distance}";
        }
        else
        {
            distanceText.text = "No ground detected below!";
        }
    }
}

using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] float DestroyTime = 3f;
    [SerializeField] Vector3 RandomizeIntensity = new Vector3(0.5f,0,0);

    void Start()
    {
        Destroy(gameObject, DestroyTime);

        transform.localPosition += new Vector3(
            Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
            Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
            Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z));
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver)
        {
            Destroy(gameObject);
        }
    }
}

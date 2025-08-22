using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    [Header("Class ref")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private TerrainPoolManager terrainPoolManager;

    [Header("Chunk Settings")]
    [SerializeField] private int chunkAmount = 5;
    [SerializeField] private float chunkLength = 1000f;
    [SerializeField] private float chunkHeight = 500f;
    [SerializeField] private Transform chunkParent;

    [Header("Chunk Speed Settings")]
    public float chunkMoveSpeed = 70;
    [SerializeField] private float chunkMaxMoveSpeed = 250f;
    [SerializeField] private float chunkMinMoveSpeed = 50f;
    [SerializeField] private float speedIncreaseRate = 5f;

    private float baseSpeed;
    private float defaultSpeedIncreaseRate;
    private bool isBoosting = false;
    private float boostTimer = 0f;
    private bool isPullingUp = false;

    private List<GameObject> chunks = new List<GameObject>();

    void Start()
    {
        defaultSpeedIncreaseRate = speedIncreaseRate;
        baseSpeed = chunkMoveSpeed;
        SpawnStartingChunks();
    }

    void Update()
    {
        HandleSpeed();
        MoveChunks(chunkMoveSpeed);
    }

    private void HandleSpeed()
    {
        float targetSpeed = isPullingUp ? chunkMinMoveSpeed : chunkMaxMoveSpeed;

        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosting = false;
                ResetSpeedIncreaseRate();
            }
        }

        chunkMoveSpeed = Mathf.MoveTowards(chunkMoveSpeed, targetSpeed, speedIncreaseRate * Time.deltaTime);
        chunkMoveSpeed = Mathf.Clamp(chunkMoveSpeed, chunkMinMoveSpeed, Mathf.Infinity);
        UpdateCameraFOVBySpeed();
    }

    private void UpdateCameraFOVBySpeed()
    {
        float minFOV = cameraController.DefaultFOV;
        float maxFOV = cameraController.MaxFOV;

        if (chunkMoveSpeed <= baseSpeed)
        {
            cameraController.SetCameraFOV(minFOV - cameraController.GetCurrentFOV());
            return;
        }

        if (chunkMoveSpeed >= chunkMaxMoveSpeed)
        {
            cameraController.SetCameraFOV(maxFOV);
            return;
        }

        float t = (chunkMoveSpeed - baseSpeed) / (chunkMaxMoveSpeed - baseSpeed);
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);
        cameraController.SetCameraFOV(targetFOV);
    }

    public void SetPullUp(bool isActive) => isPullingUp = isActive;

    public void BoostChunkSpeed(float boostAmount, float duration)
    {
        chunkMoveSpeed += boostAmount;
        boostTimer = duration;
        isBoosting = true;
    }

    public void BoostSpeedIncreaseRate(float newRate) => speedIncreaseRate = newRate;

    public void ResetSpeedIncreaseRate() => speedIncreaseRate = defaultSpeedIncreaseRate;

    private void SpawnStartingChunks()
    {
        for (int i = 0; i < chunkAmount; i++)
        {
            SpawnChunk();
        }
    }

    private void SpawnChunk()
    {
        float spawnZ = chunks.Count == 0 ? transform.position.z : chunks[^1].transform.position.z + chunkLength;
        float spawnY = chunks.Count == 0 ? transform.position.y : chunks[^1].transform.position.y - chunkHeight;

        Vector3 spawnPos = new Vector3(transform.position.x, spawnY, spawnZ);

        GameObject chunk = terrainPoolManager.GetRandomTerrain();
        chunk.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
        chunk.transform.SetParent(chunkParent, false);

        chunks.Add(chunk);
    }

    private void MoveChunks(float speed)
    {
        Vector3 moveDir = new Vector3(0, chunkHeight, -chunkLength).normalized;

        for (int i = 0; i < chunks.Count; i++)
        {
            GameObject chunk = chunks[i];
            chunk.transform.Translate(moveDir * (speed * Time.deltaTime), Space.World);

            if (chunk.transform.position.z <= Camera.main.transform.position.z - chunkLength)
            {
                terrainPoolManager.ReturnTerrain(chunk);
                chunks.RemoveAt(i);
                SpawnChunk();
                break;
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class PlayerMovements : MonoBehaviour
{
    [Header("Class ref")]
    [SerializeField] CameraController cameraController;
    [SerializeField] CameraShake cameraShake;
    [SerializeField] ChunkManager chunkManager;
    [SerializeField] InputReader inputReader;  // InputReader 연결

    [Header("Nature factor")]
    [SerializeField] float gravityForce = 9.8f;
    [SerializeField] float maxRiseSpeed = 10f;
    [SerializeField] float maxFallSpeed = 10f;

    [Header("Control factor")]
    [SerializeField] float controlSpeed = 50f;
    [SerializeField] float controlRollFactor = 20f;
    [SerializeField] float controlPitchFactor = 10f;
    [SerializeField] float rotationSpeed = 10f;

    // pullForce 대신 최소, 최대 힘을 설정
    [SerializeField] float pullMinForce = 5f;   // 가장 낮은 상승력
    [SerializeField] float pullMaxForce = 20f;  // 최대 상승력
    [SerializeField] float pullUpTimeToMax = 2.5f; // 최소-> 최대까지 걸리는 시간

    [SerializeField] float pullDownRoll = 5f;
    [SerializeField] float pullDownBoostRate = 20f;

    [Header("Animation")]
    [SerializeField] Animator anim;

    [Header("Audio")]
    [SerializeField] private AudioSource boostAudioSource;
    [SerializeField] private AudioClip boostClip;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine boostFadeCoroutine;
    private bool isBoostSoundPlaying = false;

    private Rigidbody rigidBody;
    private float startXpos;

    // 스페이스바 누르고 있는 시간 추적
    private float pullUpTimer = 0f;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        startXpos = rigidBody.position.x;
    }

    void FixedUpdate()
    {
        ProcessTranslation();
        ProcessRotation();
        HandlePullUp();
        HandlePullDown();
        AdjustSpeed();
    }

    void Update()
    {
        if (inputReader == null) return;

        HandleDebugFOV();
    }

    private void HandleDebugFOV()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            cameraController.SetCameraFOV(10f);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            cameraController.SetCameraFOV(-10f);
        }
    }

    void ProcessTranslation()
    {
        Vector2 movement = inputReader.MoveInput;

        float xOffset = controlSpeed * Time.deltaTime * movement.x;
        float yOffset = (controlSpeed / 2f) * Time.deltaTime * movement.y;
        float rawXpos = rigidBody.position.x + xOffset;

        Vector3 newPos = rigidBody.position;
        newPos.x = rawXpos;
        newPos.y = newPos.y - (gravityForce * Time.deltaTime) + yOffset;
        rigidBody.position = newPos;
    }

    void ProcessRotation()
    {
        Vector2 movement = inputReader.MoveInput;

        float pitch = -controlPitchFactor * movement.y;
        float roll = -controlRollFactor * movement.x;

        Quaternion targetRotation = Quaternion.Euler(pitch, 0f, roll);
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void HandlePullUp()
    {
        if (inputReader.PullUpTriggered)
        {
            // 스페이스바 누르고 있는 시간 누적
            pullUpTimer += Time.deltaTime;

            // 0 ~ 1로 보정 (pullUpTimeToMax초 동안 점진적으로 증가)
            float t = Mathf.Clamp01(pullUpTimer / pullUpTimeToMax);

            // t^2로 곡선을 적용해, 처음엔 느리게, 후반부엔 가파르게 상승
            float currentPullForce = Mathf.Lerp(pullMinForce, pullMaxForce, t * t);

            Vector3 newPos = rigidBody.position;
            newPos.y += currentPullForce * Time.deltaTime;
            rigidBody.position = newPos;

            // 살짝 위로 기울이는 회전
            Quaternion rollRotation = Quaternion.Euler(-5f, 0f, 0f);
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                rollRotation,
                rotationSpeed * Time.deltaTime
            );

            chunkManager.SetPullUp(true);
        }
        else
        {
            // 스페이스바 떼면 다시 0초부터
            pullUpTimer = 0f;
            chunkManager.SetPullUp(false);
        }
    }

    private void HandlePullDown()
    {
        if (inputReader.PullDownTriggered)
        {
            anim.SetBool("DoBoost", true);
            anim.SetBool("ExitBoost", false);

            Vector3 newPos = rigidBody.position;
            newPos.y -= pullMinForce * Time.fixedDeltaTime;
            rigidBody.position = newPos;

            Quaternion rollRotation = Quaternion.Euler(pullDownRoll, 0f, 0f);
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                rollRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            chunkManager.BoostSpeedIncreaseRate(pullDownBoostRate);
            cameraShake.ShakeCameraLow();

            // 부스트 사운드 시작
            if (!isBoostSoundPlaying)
            {
                isBoostSoundPlaying = true;
                boostAudioSource.clip = boostClip;
                boostAudioSource.volume = 0f;
                boostAudioSource.Play();

                if (boostFadeCoroutine != null)
                    StopCoroutine(boostFadeCoroutine);
                boostFadeCoroutine = StartCoroutine(FadeAudio(0.2f, fadeDuration));
            }
        }
        else
        {
            anim.SetBool("DoBoost", false);
            anim.SetBool("ExitBoost", true);
            chunkManager.ResetSpeedIncreaseRate();
            cameraShake.StopShakeCamera();

            // 부스트 사운드 종료
            if (isBoostSoundPlaying)
            {
                if (boostFadeCoroutine != null)
                    StopCoroutine(boostFadeCoroutine);
                boostFadeCoroutine = StartCoroutine(FadeAudio(0f, fadeDuration));
            }
        }
    }


    private void AdjustSpeed()
    {
        Vector3 velocity = rigidBody.linearVelocity;

        if (velocity.y > maxRiseSpeed)
        {
            velocity.y = maxRiseSpeed;
        }
        else if (velocity.y < -maxFallSpeed)
        {
            velocity.y = -maxFallSpeed;
        }

        rigidBody.linearVelocity = velocity;
    }

    private IEnumerator FadeAudio(float targetVolume, float duration)
    {
        float startVolume = boostAudioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            boostAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        boostAudioSource.volume = targetVolume;

        if (targetVolume == 0f)
        {
            boostAudioSource.Stop();
            isBoostSoundPlaying = false;
        }
    }

}

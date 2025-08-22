using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using Unity.Cinemachine;

public class PlayerCollision : MonoBehaviour
{
    bool isControllable = true;
    bool isCollidable = true;


    [Header("Class ref")]
    [SerializeField] ChunkManager chunkManager;
    [SerializeField] CameraController cameraController;
    [SerializeField] Rigidbody rb;

    [Header("Character object")]
    [SerializeField] GameObject charObj;
    private MeshRenderer[] meshRenderers;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    [Header("Ragdoll object")]
    [SerializeField] GameObject ragdollObj;
    [SerializeField] GameObject ragdollPajama;
    [SerializeField] Rigidbody ragdollRb;

    [SerializeField] float frontLimnitDistance = 3f;
    [SerializeField] float moveSpeedLerpDuration = 2.0f; // 감속 시간
    [SerializeField] float moveSpeedStart;
    [SerializeField] float moveSpeedEnd = 10f; // 목표 속도
    [SerializeField] float moveSpeedLerpTime = 0f;



    [Header ("Camera")]
    [SerializeField] CinemachineCamera cinemachineCamera;

    [Tooltip ("Time for camera moving to vantage point")]
    [SerializeField] float vantageDuration = 2f;

    [Tooltip("Location of vantage point")]
    [SerializeField] Vector3 vantageOffset = new Vector3(0f, 30f, 0f);

    [Header("GameoverUI")]
    [SerializeField] GameObject GameoverUI;
    [SerializeField] Animator anim;

    [Header("SoundSource")]
    public AudioClip crashClip;
    public AudioSource crashSource;

    private void Start()
    {
        // charObj 안에 있는 모든 MeshRenderer와 SkinnedMeshRenderer 가져오기
        meshRenderers = charObj.GetComponentsInChildren<MeshRenderer>();
        skinnedMeshRenderers = charObj.GetComponentsInChildren<SkinnedMeshRenderer>();
        // 애니메이터 타임스케일 영향가지 않도록
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    void Update()
    {
        CheckFront();
    }

    private void CheckFront()
    {
        RaycastHit hit;

        // 레이 발사: transform.forward (글로벌 Z 방향), 0.5f 거리
        if (Physics.Raycast(transform.position, transform.forward, out hit, frontLimnitDistance))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red); // 디버그용 레이 그리기
            Debug.Log("앞에 오브젝트 감지: " + hit.collider.name + ", 거리: " + hit.distance);

            // 원하는 처리 실행
            OnFrontObjectDetected(hit);
        }
        else
        {
            // 감지 실패 시 레이 (녹색, 최대 거리까지)
            Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.green);
        }
    }

    private void OnFrontObjectDetected(RaycastHit hit)
    {
        // 여기에 감지되었을 때 동작 추가
        Debug.Log("CheckFront() 호출됨 - 감지된 오브젝트: " + hit.collider.name);

        // 예: 충돌 대상이 특정 태그일 때만 반응
        if (hit.collider.CompareTag("Terrain"))
        {
            Debug.Log("장애물 감지!");
            chunkManager.chunkMoveSpeed = 10f;
        }
    }

    // 지형 및 아이템 충돌시 발생할 이벤트
    private void OnCollisionEnter(Collision other)
    {
        string hitObject = other.gameObject.tag;

        if (!isControllable || !isCollidable) return;

        switch (hitObject)
        {
            case "Terrain":
                Debug.Log("Crashed!");
                StartCrashSequence();
                if (crashClip != null && crashSource != null)
                {
                    crashSource.PlayOneShot(crashClip);
                }
                break;
            default:
                break;
        }
    }

    void StartCrashSequence()
    {
        Time.timeScale = 0.1f;
        isControllable = false;

        // 튕겨나가는 효과
        ragdollRb.AddForce(new Vector3(0f, 2000f, 0f), ForceMode.Impulse);

        // 캐릭터 이동 제어 비활성화 + 중력 적용
        GetComponent<PlayerMovements>().enabled = false;
        rb.useGravity = true;

        // ChunkManager moveSpeed 서서히 감소
        moveSpeedStart = chunkManager.chunkMoveSpeed;
        moveSpeedLerpTime = 0f;
        StartCoroutine(LerpMoveSpeed());

        // 랙돌을 캐릭터 위치에 배치
        ragdollObj.transform.position = charObj.transform.position;
        ragdollObj.transform.rotation = charObj.transform.rotation;

        // 캐릭터 MeshRenderer 끄기
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.enabled = false;
        }
        foreach (SkinnedMeshRenderer skinnedRenderer in skinnedMeshRenderers)
        {
            skinnedRenderer.enabled = false;
        }

        // 랙돌 활성화
        ragdollObj.SetActive(true);

        // Cinemachine 카메라 추적 해제
        cinemachineCamera.Follow = null;
        cinemachineCamera.LookAt = null;

        // 카메라 위로 이동(부드럽게)
        Transform ragdollRoot = ragdollPajama.transform;
        StartCoroutine(CameraMoveToVantage(cinemachineCamera.transform, ragdollRoot));

        // 게임오버 UI표시
        GameoverUI.SetActive(true);

        // 게임 오버시퀀스
        GameManager.Instance.GameOver();
    }

    IEnumerator LerpMoveSpeed()
    {
        while (moveSpeedLerpTime < moveSpeedLerpDuration)
        {
            moveSpeedLerpTime += Time.deltaTime;
            float t = moveSpeedLerpTime / moveSpeedLerpDuration;

            chunkManager.chunkMoveSpeed = Mathf.Lerp(moveSpeedStart, moveSpeedEnd, t);
            yield return null;
        }
        chunkManager.chunkMoveSpeed = moveSpeedEnd;
    }

    /// 카메라를 현재 위치/회전에서 관전 위치/회전으로 서서히 이동하는 코루틴
    IEnumerator CameraMoveToVantage(Transform camTransform, Transform target)
    {
        // 시작점(현재 카메라 상태)
        Vector3 startPos = camTransform.position;
        Quaternion startRot = camTransform.rotation;

        // 목표점(타겟 + vantageOffset)
        Vector3 endPos = target.position + vantageOffset;

        // endRot: 위에서 캐릭터를 내려다보도록
        // target의 위치 - 카메라 위치 => 아래쪽을 바라보는 벡터
        Quaternion endRot = Quaternion.LookRotation(target.position - endPos);

        float elapsed = 0f;

        while (elapsed < vantageDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / vantageDuration);

            // 위치 보간
            camTransform.position = Vector3.Lerp(startPos, endPos, t);
            // 회전 보간 (Slerp)
            camTransform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        // 최종 값 확정
        camTransform.position = endPos;
        camTransform.rotation = endRot;
    }
}

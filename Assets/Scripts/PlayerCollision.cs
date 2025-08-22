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
    [SerializeField] float moveSpeedLerpDuration = 2.0f; // ���� �ð�
    [SerializeField] float moveSpeedStart;
    [SerializeField] float moveSpeedEnd = 10f; // ��ǥ �ӵ�
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
        // charObj �ȿ� �ִ� ��� MeshRenderer�� SkinnedMeshRenderer ��������
        meshRenderers = charObj.GetComponentsInChildren<MeshRenderer>();
        skinnedMeshRenderers = charObj.GetComponentsInChildren<SkinnedMeshRenderer>();
        // �ִϸ����� Ÿ�ӽ����� ���Ⱑ�� �ʵ���
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    void Update()
    {
        CheckFront();
    }

    private void CheckFront()
    {
        RaycastHit hit;

        // ���� �߻�: transform.forward (�۷ι� Z ����), 0.5f �Ÿ�
        if (Physics.Raycast(transform.position, transform.forward, out hit, frontLimnitDistance))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red); // ����׿� ���� �׸���
            Debug.Log("�տ� ������Ʈ ����: " + hit.collider.name + ", �Ÿ�: " + hit.distance);

            // ���ϴ� ó�� ����
            OnFrontObjectDetected(hit);
        }
        else
        {
            // ���� ���� �� ���� (���, �ִ� �Ÿ�����)
            Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.green);
        }
    }

    private void OnFrontObjectDetected(RaycastHit hit)
    {
        // ���⿡ �����Ǿ��� �� ���� �߰�
        Debug.Log("CheckFront() ȣ��� - ������ ������Ʈ: " + hit.collider.name);

        // ��: �浹 ����� Ư�� �±��� ���� ����
        if (hit.collider.CompareTag("Terrain"))
        {
            Debug.Log("��ֹ� ����!");
            chunkManager.chunkMoveSpeed = 10f;
        }
    }

    // ���� �� ������ �浹�� �߻��� �̺�Ʈ
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

        // ƨ�ܳ����� ȿ��
        ragdollRb.AddForce(new Vector3(0f, 2000f, 0f), ForceMode.Impulse);

        // ĳ���� �̵� ���� ��Ȱ��ȭ + �߷� ����
        GetComponent<PlayerMovements>().enabled = false;
        rb.useGravity = true;

        // ChunkManager moveSpeed ������ ����
        moveSpeedStart = chunkManager.chunkMoveSpeed;
        moveSpeedLerpTime = 0f;
        StartCoroutine(LerpMoveSpeed());

        // ������ ĳ���� ��ġ�� ��ġ
        ragdollObj.transform.position = charObj.transform.position;
        ragdollObj.transform.rotation = charObj.transform.rotation;

        // ĳ���� MeshRenderer ����
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.enabled = false;
        }
        foreach (SkinnedMeshRenderer skinnedRenderer in skinnedMeshRenderers)
        {
            skinnedRenderer.enabled = false;
        }

        // ���� Ȱ��ȭ
        ragdollObj.SetActive(true);

        // Cinemachine ī�޶� ���� ����
        cinemachineCamera.Follow = null;
        cinemachineCamera.LookAt = null;

        // ī�޶� ���� �̵�(�ε巴��)
        Transform ragdollRoot = ragdollPajama.transform;
        StartCoroutine(CameraMoveToVantage(cinemachineCamera.transform, ragdollRoot));

        // ���ӿ��� UIǥ��
        GameoverUI.SetActive(true);

        // ���� ����������
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

    /// ī�޶� ���� ��ġ/ȸ������ ���� ��ġ/ȸ������ ������ �̵��ϴ� �ڷ�ƾ
    IEnumerator CameraMoveToVantage(Transform camTransform, Transform target)
    {
        // ������(���� ī�޶� ����)
        Vector3 startPos = camTransform.position;
        Quaternion startRot = camTransform.rotation;

        // ��ǥ��(Ÿ�� + vantageOffset)
        Vector3 endPos = target.position + vantageOffset;

        // endRot: ������ ĳ���͸� �����ٺ�����
        // target�� ��ġ - ī�޶� ��ġ => �Ʒ����� �ٶ󺸴� ����
        Quaternion endRot = Quaternion.LookRotation(target.position - endPos);

        float elapsed = 0f;

        while (elapsed < vantageDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / vantageDuration);

            // ��ġ ����
            camTransform.position = Vector3.Lerp(startPos, endPos, t);
            // ȸ�� ���� (Slerp)
            camTransform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        // ���� �� Ȯ��
        camTransform.position = endPos;
        camTransform.rotation = endRot;
    }
}

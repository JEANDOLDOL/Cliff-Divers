using UnityEngine;
using UnityEngine.EventSystems;

public class EscBtn : MonoBehaviour
{
    [SerializeField] GameObject escScreen;
    [SerializeField] InputReader inputReader;
    [SerializeField] GameObject firstSelected;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // �޴� ���� ���¿��� Cancel(ESC) �Է� �� �޴� �ݱ�
        if (isPaused && inputReader.CancelTriggered)
        {
            TogglePause();
            inputReader.ResetTriggers(); // ���� �ʱ�ȭ
        }
    }

    void OnEnable()
    {
        // �޴��� Ȱ��ȭ�Ǹ� ù ��ư �ڵ� ����
        if (escScreen.activeSelf && firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        escScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            inputReader.EnableUIInput();
            SetPauseState(true);

            if (firstSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelected);
            }
        }
        else
        {
            inputReader.EnablePlayerInput();
            SetPauseState(false);
        }
    }

    public void OnClickResumeBtn()
    {
        TogglePause();
    }

    public void OnclickRestartGame()
    {
        GameManager.Instance.Restart();
    }

    private void SetPauseState(bool pause)
    {
        GameManager.Instance.SetPause(pause);
    }
}

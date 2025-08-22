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

        // 메뉴 켜진 상태에서 Cancel(ESC) 입력 시 메뉴 닫기
        if (isPaused && inputReader.CancelTriggered)
        {
            TogglePause();
            inputReader.ResetTriggers(); // 상태 초기화
        }
    }

    void OnEnable()
    {
        // 메뉴가 활성화되면 첫 버튼 자동 선택
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

using UnityEngine;

public class QuitMenuController : MonoBehaviour
{
    public GameObject mainButtonsGroup;
    public GameObject quitConfirmPanel;

    // "Quit" 버튼 눌렀을 때
    public void OnQuitPressed()
    {
        mainButtonsGroup.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    // "예" 버튼 눌렀을 때
    public void OnConfirmQuit()
    {
        Application.Quit();

        // 에디터에서 테스트 시 종료되지 않으므로 로그 출력
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // "아니오" 버튼 눌렀을 때
    public void OnCancelQuit()
    {
        quitConfirmPanel.SetActive(false);
        mainButtonsGroup.SetActive(true);
    }
}

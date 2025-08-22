using UnityEngine;

public class QuitMenuController : MonoBehaviour
{
    public GameObject mainButtonsGroup;
    public GameObject quitConfirmPanel;

    // "Quit" ��ư ������ ��
    public void OnQuitPressed()
    {
        mainButtonsGroup.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    // "��" ��ư ������ ��
    public void OnConfirmQuit()
    {
        Application.Quit();

        // �����Ϳ��� �׽�Ʈ �� ������� �����Ƿ� �α� ���
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // "�ƴϿ�" ��ư ������ ��
    public void OnCancelQuit()
    {
        quitConfirmPanel.SetActive(false);
        mainButtonsGroup.SetActive(true);
    }
}

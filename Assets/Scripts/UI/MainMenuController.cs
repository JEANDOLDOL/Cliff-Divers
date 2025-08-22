using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    public List<GameObject> gameObjectsGroup = new List<GameObject>();
    public List<GameObject> gameObjectsGroupMainMenu = new List<GameObject>();
    public GameObject settingsPanel;
    public List<GameObject> otherButtons = new List<GameObject>();

    public void StartGame()
    {
        foreach (GameObject go in gameObjectsGroup)
        {
            if (go != null)
                go.SetActive(true);
        }
        foreach (GameObject go in gameObjectsGroupMainMenu)
        {
            if (go != null)
                go.SetActive(false);
        }
    }
    public void OpenSettings()
    {
        // �ٸ� ��ư�� ��Ȱ��ȭ
        foreach (GameObject go in otherButtons)
        {
            if (go != null)
                go.SetActive(false);
        }

        // ���� �г� Ȱ��ȭ
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        foreach (GameObject go in otherButtons)
        {
            if (go != null)
                go.SetActive(true);
        }

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}

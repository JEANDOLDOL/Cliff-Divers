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
        // 다른 버튼들 비활성화
        foreach (GameObject go in otherButtons)
        {
            if (go != null)
                go.SetActive(false);
        }

        // 세팅 패널 활성화
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

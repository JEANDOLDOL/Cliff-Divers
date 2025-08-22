using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseSelectableButton : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ���콺�� �÷��� �� ���õǵ���
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}

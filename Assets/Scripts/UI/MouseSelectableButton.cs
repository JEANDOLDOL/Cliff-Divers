using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseSelectableButton : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스로 올렸을 때 선택되도록
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}

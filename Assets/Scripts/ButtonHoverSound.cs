using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverClip;   // ���콺 �ø� ��
    public AudioClip clickClip;   // Ŭ���� ��
    public AudioSource audioSource; // Inspector���� ���� ����

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickClip);
        }
    }
}

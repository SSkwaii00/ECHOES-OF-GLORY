using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text textComponent;
    private Color normalColor = new Color(0.9137f, 0.8118f, 0.1686f); // #E9CF2B
    private Color hoverColor = new Color(0f, 0f, 0f);                 // Màu đen
    private bool isHovering = false;
    private float transitionSpeed = 5f;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        textComponent.color = normalColor;
    }

    void Update()
    {
        if (isHovering)
        {
            textComponent.color = Color.Lerp(textComponent.color, hoverColor, Time.deltaTime * transitionSpeed);
        }
        else
        {
            textComponent.color = Color.Lerp(textComponent.color, normalColor, Time.deltaTime * transitionSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
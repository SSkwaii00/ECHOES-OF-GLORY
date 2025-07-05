using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text textComponent;
    private Color normalColor = new Color(0.3294f, 0.3255f, 0.3255f); // #545353
    private Color hoverColor = new Color(0.8392f, 0.6510f, 0.2745f);  // #D6A646
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
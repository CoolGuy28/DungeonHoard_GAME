using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    public ButtonEvent onPressEvent;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SelectButton()
    {
        image.color = activeColor;
    }

    public void DeselectButton()
    {
        image.color = inactiveColor;
    }

    public void PressButton()
    {
        onPressEvent.Invoke();
    }
}
[System.Serializable]
public class ButtonEvent : UnityEvent { }
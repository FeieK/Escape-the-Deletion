using UnityEngine;
using UnityEngine.InputSystem;

public class KonamiCode : MonoBehaviour
{
    public InputActionReference upArrow;
    public InputActionReference downArrow;
    public InputActionReference leftArrow;
    public InputActionReference rightArrow;
    public InputActionReference leftMouse;
    public InputActionReference rightMouse;

    public KonamiCodeInput[] order = { KonamiCodeInput.UP_ARROW, KonamiCodeInput.UP_ARROW, KonamiCodeInput.DOWN_ARROW, KonamiCodeInput.DOWN_ARROW, KonamiCodeInput.LEFT_ARROW, KonamiCodeInput.RIGHT_ARROW, KonamiCodeInput.LEFT_ARROW, KonamiCodeInput.RIGHT_ARROW, KonamiCodeInput.LEFT_MOUSE, KonamiCodeInput.RIGHT_MOUSE };

    private int index = 0;

    void OnEnable()
    {
        upArrow.action.started += ctx => ClickButton(ctx, KonamiCodeInput.UP_ARROW);
        downArrow.action.started += ctx => ClickButton(ctx, KonamiCodeInput.DOWN_ARROW);
        leftArrow.action.started += ctx => ClickButton(ctx, KonamiCodeInput.LEFT_ARROW);
        rightArrow.action.started += ctx => ClickButton(ctx, KonamiCodeInput.RIGHT_ARROW);

        leftMouse.action.started += ctx => ClickButton(ctx, KonamiCodeInput.LEFT_MOUSE);
        rightMouse.action.started += ctx => ClickButton(ctx, KonamiCodeInput.RIGHT_MOUSE);
    }

    void OnDisable()
    {
        upArrow.action.started -= ctx => ClickButton(ctx, KonamiCodeInput.UP_ARROW);
        downArrow.action.started -= ctx => ClickButton(ctx, KonamiCodeInput.DOWN_ARROW);
        leftArrow.action.started -= ctx => ClickButton(ctx, KonamiCodeInput.LEFT_ARROW);
        rightArrow.action.started -= ctx => ClickButton(ctx, KonamiCodeInput.RIGHT_ARROW);

        leftMouse.action.started -= ctx => ClickButton(ctx, KonamiCodeInput.LEFT_MOUSE);
        rightMouse.action.started -= ctx => ClickButton(ctx, KonamiCodeInput.RIGHT_MOUSE);
    }

    void ClickButton(InputAction.CallbackContext? obj, KonamiCodeInput input)
    {
        if (order[index] == input && index < order.Length)
        {
            if (index == order.Length - 1)
            {
                MenuNavigation.instance.DeleteWidget.SetActive(true);
            }
            else
            {
                index++;
            }
        }
        else
        {
            index = 0;
        }
    }
    public enum KonamiCodeInput
    {
        UP_ARROW,
        DOWN_ARROW,
        LEFT_ARROW,
        RIGHT_ARROW,

        LEFT_MOUSE,
        RIGHT_MOUSE
    }
}

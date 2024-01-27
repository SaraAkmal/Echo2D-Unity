using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    [SerializeField] private const float nSecond = 0.3f;
    private PointerEventData eventDataPos;
    private bool pointerEntered;
    private float timer;

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    private void Update()
    {
        //If pointer is pointing on the object, start the timer
        if (pointerEntered)
        {
            //Increment timer
            timer += Time.deltaTime;
            if (timer > nSecond)
            {
                ShowJoystick(eventDataPos);
                pointerEntered = false;
            }
        }
        else
        {
            //Reset timer when it's no longer pointing
            timer = 0;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        pointerEntered = true;
        eventDataPos = eventData;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        pointerEntered = false;
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }

    private void ShowJoystick(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }
}
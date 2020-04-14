using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class VJHandler : DynamicButton, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RawImage NoItemRing;
    public RawImage ItemRing;
    private RawImage outerRing;
    public RawImage InnerNub;
    public Input_via_Touch Controls;
    private bool autoJump,currentJump;

    private Vector3 InputDirection;

    void Start()
    {
        currentJump = false;
        autoJump = false;
        outerRing = NoItemRing;
        //outerRing = GetComponent<Image>();
        //joystick = transform.GetChild(0).GetComponent<Image>(); //this command is used because there is only one child in hierarchy
        InputDirection = Vector3.zero;
    }

    public void OnDrag(PointerEventData ped)
    {
        Vector2 position = Vector2.zero;

        //To get InputDirection
        RectTransformUtility.ScreenPointToLocalPointInRectangle
                (outerRing.rectTransform,
                ped.position,
                ped.pressEventCamera,
                out position);

        position.x = (position.x / outerRing.rectTransform.sizeDelta.x);
        position.y = (position.y / outerRing.rectTransform.sizeDelta.y);

        float x = position.x * 2;// (outerRing.rectTransform.pivot.x == 1f) ? position.x * 2 + 1 : position.x * 2 - 1;
        float y = position.y * 2;// (outerRing.rectTransform.pivot.y == 1f) ? position.y * 2 + 1 : position.y * 2 - 1;

        InputDirection = new Vector2(x, y);
        InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

        //to define the area in which joystick can move around
        InnerNub.rectTransform.anchoredPosition = new Vector2(InputDirection.x * (outerRing.rectTransform.sizeDelta.x / 3)
                                                               , InputDirection.y * (outerRing.rectTransform.sizeDelta.y) / 3);

        SetControls(InputDirection);
        
    }

    private IEnumerator AutoJump()
    {
        autoJump = true;
        Controls.PlayerJump(false);
        currentJump = true;
        while (autoJump)
        {
            if (currentJump)
            {
                currentJump = false;
                Controls.PlayerJump(true);
            }
            else
            {
                Controls.PlayerJump(false);
                currentJump = true;
            }
            yield return null;
        }
        Controls.PlayerJump(false);
    }

    public void OnPointerDown(PointerEventData ped)
    {

        OnDrag(ped);
    }

    public void OnPointerUp(PointerEventData ped)
    {

        InputDirection = Vector3.zero;
        InnerNub.rectTransform.anchoredPosition = Vector3.zero;
        SetControls(InputDirection);
    }

    public override void UpdateButton(bool active)
    {
        if (active)
        {
            if (outerRing != ItemRing)
            {
                ItemRing.gameObject.SetActive(true);
                NoItemRing.gameObject.SetActive(false);
                outerRing = ItemRing;
            }
        }
        else
        {
            if (outerRing != NoItemRing)
            {
                ItemRing.gameObject.SetActive(false);
                NoItemRing.gameObject.SetActive(true);
                outerRing = NoItemRing;
            }
        }
    }

    private void SetControls(Vector2 InputDirection)
    {
        if (InputDirection.x < -.1f)
        {
            Controls.PlayerLeft(true);
        }
        else if (InputDirection.x > .1f)
        {
            Controls.PlayerRight(InputDirection.x);
        }
        else
        {
            Controls.PlayerRight(InputDirection.x);
        }

        if (InputDirection.y > .4f)
        {
            if (!autoJump)
            {
                StartCoroutine(AutoJump());
            }
            //if (autoJump)
            //{
            //    autoJump = false;
            //    Controls.PlayerJump(true);
            //}
            //else
            //{
            //    Controls.PlayerJump(false);
            //    autoJump = true;
            //}
        }
        else
        {
            autoJump = false;
            if (InputDirection.y < -.7f)
            {
                Controls.PlayerPickup(true);
            }
            else
            {
                Controls.PlayerPickup(false);
            }
        }
    }

    public override bool IsActive()
    {
        return (outerRing == ItemRing);
    }

    public override void Complete(bool success)
    {
    }
}
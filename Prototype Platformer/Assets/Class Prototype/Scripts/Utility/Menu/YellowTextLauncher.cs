using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class YellowTextLauncher : MonoBehaviour
{
    [Header("Screen Bounds")]
    [SerializeField]
    private Transform UpperLeft;
    [SerializeField]
    private Transform LowerRight;

    [System.Serializable]
    public struct Message
    {
        public string message;
        public Color color;
    }

    [Header("Messages")]
    [SerializeField]
    private Message[] messages;

    private Vector3 Heading;
    private const float speed = 100;

    // Start is called before the first frame update
    void Start()
    {
        Launch();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //float delta = Mathf.Min(Time.deltaTime, .26f);
        float speed = Time.deltaTime * YellowTextLauncher.speed;

        Vector3 newHeading=  Heading * speed;

        this.transform.localPosition += newHeading;

        if (OutOfBounds())
            //Launch();
            Destroy(gameObject);
    }
    

    private void Launch()
    {
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        Message message = messages[Random.Range(0, messages.Length)];
        text.text = message.message.Replace("\\n", "\n");
        text.faceColor = message.color;
        

        Vector2 pos;
        StarPos(out pos, out Heading);
        this.transform.localPosition = pos;

        if (OutOfBounds())
        {
            //Debug.LogError("Created out of bounds text");
            Launch();
        }
    }

    private void StarPos(out Vector2 localPosition, out Vector3 heading)
    {
        float marginpt = .2f;
        if (Random.Range(0, 2) == 1)
        {
            //print("1");
            //localPosition
            float margin = marginpt * (LowerRight.localPosition.x - UpperLeft.localPosition.x);
            float low = UpperLeft.localPosition.x + margin;
            float high = LowerRight.localPosition.x - margin;
            float x = Random.Range(low, high);

            float py, hy;
            if (Random.Range(0, 2) == 1)
            {
                py = UpperLeft.localPosition.y;
                hy = LowerRight.localPosition.y;
            }
            else
            {
                py = LowerRight.localPosition.y;
                hy = UpperLeft.localPosition.y;
            }

            print("1 low:" + low + " high:" + high + " y:" + py + " x:" + x + " ul:" + UpperLeft.localPosition + " lr:" + LowerRight.localPosition);
            //x = Mathf.Clamp(x, low,high);

            localPosition = new Vector2(x, py);

            //heading
            x = Random.Range(low, high);
            //x = Mathf.Clamp(x, low, high);

            heading = (new Vector2(x, hy) - localPosition).normalized;
        }
        else
        {
            //localPosition
            float margin = marginpt * (UpperLeft.localPosition.y - LowerRight.localPosition.y);
            float low = UpperLeft.localPosition.y - margin;
            float high = LowerRight.localPosition.y + margin;
            float y = Random.Range(low, high);

            float px, hx;
            if (Random.Range(0, 2) == 1)
            {
                px = UpperLeft.localPosition.x;
                hx = LowerRight.localPosition.x;
            }
            else
            {
                px = LowerRight.localPosition.x;
                hx = UpperLeft.localPosition.x;
            }
            print("2 low:"+low+" high:"+high + " x:" + px + " y:" + y + " ul:"+UpperLeft.localPosition+" lr:"+LowerRight.localPosition);

            //y = Mathf.Clamp(y, low,high);
            localPosition = new Vector2(px, y);

            //heading
            //margin = marginpt * (UpperLeft.localPosition.y - LowerRight.localPosition.y);
            y = Random.Range(low,high);
            //y = Mathf.Clamp(y, low, high);

            heading = (new Vector2(hx, y) - localPosition).normalized;
        }
    }

    private bool OutOfBounds()
    {
        if (transform.localPosition.y > UpperLeft.localPosition.y * 2)
            return true;
        if (transform.localPosition.y < LowerRight.localPosition.y * 2)
            return true;

        if (transform.localPosition.x < UpperLeft.localPosition.x * 2)
            return true;
        if (transform.localPosition.x > LowerRight.localPosition.x * 2)
            return true;

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Input_via_GamePad : MonoBehaviour
{

    [System.Serializable]
    public class ControlEvent : UnityEvent<ControlStruct> { }

    GamepadInput _input;

    public ControlEvent controller1;
    public ControlEvent controller2;
    public ControlEvent controller3;
    public ControlEvent controller4;


    public int maxPlayers = 4;

    private List<ControlStruct> previousControls;

    private const int jump = 0, attack = 1, action = 2, left = 0, right = 0;
    private const int AXIS = 1, BUTTON = 0;
    private float axisThreshold = 0.2f;

    private bool printOnce = true;

    public GamepadInput input
    {
        get
        {
            if (!_input)
                _input = GetComponent<GamepadInput>();
            return _input;
        }
    }
    

    // Use this for initialization
    void Start()
    {
        previousControls = new List<ControlStruct>(maxPlayers);
        for (int i = 0; i < maxPlayers; ++i)
            previousControls.Add(new ControlStruct());
    }
    
    

    //Update is called once per frame
    void Update()
    {
        //if (!Input.GetKey(KeyCode.LeftShift))
        {
            
            if (input == null)
                print("input is null");


            //if (input.gamepads.Count == 0)
            //{
            //    print("no gamepads connected");
            //}

            int playerNum = 1;
            

            foreach (GamepadDevice gamepad in input.gamepads)
            {
                //create a structure for holding controls
                ControlStruct playerControls = new ControlStruct();


                int[] buttonValues = (int[])System.Enum.GetValues(typeof(GamepadButton));


                for (int i = 0; i < buttonValues.Length; i++)
                {
                    
                     //print("Not left shift " + (GamepadButton)buttonValues[i] + ": " + gamepad.GetButton((GamepadButton)buttonValues[i]) + "\n");
                     updateControl(i, playerControls, gamepad.GetButton((GamepadButton)buttonValues[i]));
                        
                    
                        
                }

                int[] axisValues = (int[])System.Enum.GetValues(typeof(GamepadAxis));
                for (int i = 0; i < axisValues.Length; i++)
                {
                    if (gamepad.GetAxis((GamepadAxis)axisValues[i]) != 0)
                    {
                        
                        //print("Not left shift " + (GamepadAxis)axisValues[i] + ": " + gamepad.GetAxis((GamepadAxis)axisValues[i]) + "\n");
                        updateControl(i, playerControls, gamepad.GetAxis((GamepadAxis)axisValues[i]));
                        
                    }
                        
                }


                //pass the controls on to the player through an event
                //keep track of gamepads by ID, rather than player number
                //so that if a controller becomes unplugged, it doesn't shift
                //all controllers down an index.
                if (gamepad.deviceId == 0) controller1.Invoke(playerControls);
                if (gamepad.deviceId == 1) controller2.Invoke(playerControls);
                if (gamepad.deviceId == 2) controller3.Invoke(playerControls);
                if (gamepad.deviceId == 3) controller4.Invoke(playerControls);
                playerNum++;
            }
            if (printOnce)
            {
                print("controllers found: " + (playerNum - 1));
                printOnce = false;
            }
            
        }

    }

    private bool isControlUpdated(int currentControl, ControlStruct previous, bool state)
    {
        switch (currentControl)
        {
            case jump:
                if (previous.jump == state)
                    return false;
                break;
            case attack:
                if (previous.attack == state)
                    return false;
                break;
            case action:
                if (previous.action == state)
                    return false;
                break;
            default:
                return false;
        }
        return true;
    }
    private void updateControl(int currentControl, ControlStruct current, bool state)
    {
        switch (currentControl)
        {
            case jump:
                current.jump = state;
                break;
            case attack:
                current.attack = state;
                break;
            case action:
                current.action = state;
                break;
        }
    }
    private bool isControlUpdated(int currentControl, ControlStruct previous, float state)
    {
        switch (currentControl)
        {
            case left:
                if (previous.moveLeft >= state - axisThreshold && previous.moveLeft <= state + axisThreshold)
                    return false;
                break;
        }
        return true;
    }
    private void updateControl(int currentControl, ControlStruct current, float state)
    {
        switch (currentControl)
        {
            case left:
                current.moveLeft = state;
                break;
        }
        return;
    }

    


}

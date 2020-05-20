
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Input_via_GamePad : MonoBehaviour
{
    private const int NotFound = -1;

    [System.Serializable]
    public class ControlEvent : UnityEvent<ControlStruct> { }

    GamepadInput _input;

    public ControlEvent controller1;
    public ControlEvent controller2;
    public ControlEvent controller3;
    public ControlEvent controller4;
    

    public int maxPlayers = 4;
    private int playerCount = 4;

    private static GamepadDevice [] devices;
    private static Dictionary<string,GamepadMonitor> previous;
    private int[] lastInput;
    
    private List<ControlStruct> previousControls;
    //Axis 4=lefttrig, 5=righttrig, 0=leftAnalogH

    //Button A=0, B=1, X=2, Y=3, Rbumper=12


    private const int start=5, jump = 0, attackButton = 2, attackAxis1=4, attackAxis2 = 5, action = 3, door1=1, door2=12, door3 = 11, left = 0, right = 0, menu=1;
    private const int AXIS = 1, BUTTON = 0;
    private const float axisThreshold = 0.2f, triggerThrishold = 0.5f;

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
        playerCount = UndestroyableData.GetPlayers();

        lastInput = new int[2];
        lastInput[0] = -1;
        lastInput[1] = -1;
        if (devices == null)
        {
            print("Creating/initializing device map");
            devices = new GamepadDevice[4];
            previous = new Dictionary<string, GamepadMonitor>();
        }

        previousControls = new List<ControlStruct>(maxPlayers);
        for (int i = 0; i < maxPlayers; ++i)
            previousControls.Add(new ControlStruct(ControlStruct.Controller));
    }
    
    private int DeviceIndex(GamepadDevice d)
    {
        if (IsDeviceAt(d.deviceId, d))
            return d.deviceId;
        for (int i = 0; i < 4; ++i)
            if (IsDeviceAt(i,d))
                return i;
        return NotFound;
    }

    private bool IsDeviceAt(int index, GamepadDevice d)
    {
        if (index >= devices.Length)
            return false;
        if (devices[index] == null)
            return false;
        if (devices[index] == d)
            return true;
        if (d.deviceId == devices[index].deviceId)
        {
            devices[index] = d;
            return true;
        }
        return false;
    }

    private void RemoveDeviceAt(int index)
    {
        devices[index] = null;
    }
    private void FireDevice(int index,ControlStruct playerControls)
    {
        //If there is no num pad users might opt to reverse the order in which
        // players receive game pads, however only do this if there are 4 players
        // in the game, otherwise in games with less players controllers won't go
        // to those players.

        switch (playerCount)
        {
            default:
                if (UndestroyableData.GetReversePlayerOrder())
                {
                    if (index == 3) controller1.Invoke(playerControls);
                    if (index == 2) controller2.Invoke(playerControls);
                    if (index == 1) controller3.Invoke(playerControls);
                    if (index == 0) controller4.Invoke(playerControls);
                }
                else
                {
                    if (index == 0) controller1.Invoke(playerControls);
                    if (index == 1) controller2.Invoke(playerControls);
                    if (index == 2) controller3.Invoke(playerControls);
                    if (index == 3) controller4.Invoke(playerControls);
                }
                break;
            case 1:
                //if (playerControls.attack)
                //    print("controller input attack PRE convert" + index);
                playerControls.ConvertToSource(ControlStruct.GetDevice(index+1));
                //if (playerControls.attack)
                //    print("controller input attack POST convert" + index);
                //if (playerControls.source == ControlStruct.Device1)
                //    Debug.LogWarning("input gamepad: " +" index: "+index+" "+ playerControls);
                
                if (index == 0) controller1.Invoke(playerControls);
                if (index == 1) controller1.Invoke(playerControls);
                if (index == 2) controller1.Invoke(playerControls);
                if (index == 3) controller1.Invoke(playerControls);
                break;
            case 2:
                playerControls.ConvertToSource(ControlStruct.GetDevice(index + 1));
                if (index == 0||index==2) controller1.Invoke(playerControls);
                if (index == 1||index==3) controller2.Invoke(playerControls);
                break;
        }

        //if (UndestroyableData.GetReversePlayerOrder()&&UndestroyableData.GetPlayers()==4)
        //{
        //    if (index == 3) controller1.Invoke(playerControls);
        //    if (index == 2) controller2.Invoke(playerControls);
        //    if (index == 1) controller3.Invoke(playerControls);
        //    if (index == 0) controller4.Invoke(playerControls);
        //}
        //else
        //{
        //    if (index == 0) controller1.Invoke(playerControls);
        //    if (index == 1) controller2.Invoke(playerControls);
        //    if (index == 2) controller3.Invoke(playerControls);
        //    if (index == 3) controller4.Invoke(playerControls);
        //}
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

            int playerNum = 0;

            List<ControlStruct> Cs = new List<ControlStruct>(4);
            List<GamepadDevice> Ds = new List<GamepadDevice>(4);

            foreach (GamepadDevice gamepad in input.gamepads)
            {
                //if (gamepad.systemName == null || gamepad.systemName.Length == 0)
                //    continue;
                playerNum++;
                //create a structure for holding controls
                ControlStruct playerControls = new ControlStruct(ControlStruct.Controller);
                //ControlStruct playerControls = new ControlStruct(ControlStruct.GetDevice(playerNum));


                int[] buttonValues = (int[])System.Enum.GetValues(typeof(GamepadButton));


                for (int i = 0; i < buttonValues.Length; i++)
                {

                    //print("Not left shift " + (GamepadButton)buttonValues[i] + ": " + gamepad.GetButton((GamepadButton)buttonValues[i]) + "\n");
                    //print("Not left shift " + i + ": " + gamepad.GetButton((GamepadButton)buttonValues[i]) + "\n");
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
                if (!Used(gamepad, playerControls, playerNum))
                    continue;
                Ds.Add(gamepad);
                Cs.Add(playerControls);

                //if (gamepad.deviceId == 0) controller1.Invoke(playerControls);
                //if (gamepad.deviceId == 1) controller2.Invoke(playerControls);
                //if (gamepad.deviceId == 2) controller3.Invoke(playerControls);
                //if (gamepad.deviceId == 3) controller4.Invoke(playerControls);

                //if (printOnce)
                    //print("gamePad " + gamepad.deviceId + " " + gamepad.displayName + " -sys: " + gamepad.systemName);
            }
            //if (printOnce)
            //{
            //    //print("controllers found: " + (playerNum));
            //    //foreach (GamepadDevice d in Ds)
            //    //    print("  " + d.deviceId + ", " + d.systemName + ", " + d.displayName);
            //    printOnce = false;
            //}

            bool[] changed = new bool[4];

            //Invoke known devices
            for(int i=0; i<Ds.Count; ++i)
            {
                int index = DeviceIndex(Ds[i]);
                if (index == NotFound)
                    continue;
                changed[index] = true;
                //if (Cs[i].attack)
                //    Debug.LogError(" known fire sys:" + Ds[i].systemName + " dis:" + Ds[i].displayName + " " + Cs[i]);
                //else
                //    Debug.Log(" known fire sys:" + Ds[i].systemName + " dis:" + Ds[i].displayName + " " + Cs[i]);
                FireDevice(index, Cs[i]);
            }

            //Assign and invoke uknown devices
            for (int i = 0; i < Ds.Count; ++i)
            {
                int index = DeviceIndex(Ds[i]);
                if (index != NotFound)
                    continue;

                int freeSpot=GetFirstUnchanged(changed, Ds[i]);
                devices[freeSpot] = Ds[i];
                //Debug.LogWarning(" UN known fire" + " " + Cs[i]);
                FireDevice(freeSpot, Cs[i]);
            }

            //remove old devices
            PurgeUnchangedDevices(changed);
            
        }

    }

    private bool Used(GamepadDevice device, ControlStruct controls, int id)
    {
        if (!previous.ContainsKey(device.Key))
        {
            //Debug.LogError("Saving new gamepad " + device);
            previous.Add(device.Key, new GamepadMonitor(controls));
            return false;
        }
        GamepadMonitor m = previous[device.Key];
        if (m.used)
            return id==m.id;
        if (m.AnyChange(controls))
        {
            //Debug.LogWarning("controller changed " + device);
            m.Change(controls, id);
            return true;
        }
        //Debug.Log("controller unchanged changed " + device);
        return false;
    }

    


    private int GetFirstUnchanged(bool [] changed, GamepadDevice device)
    {
        int perferredIndex = device.deviceId;
        if(perferredIndex<UndestroyableData.GetPlayers() && !changed[perferredIndex])
        {
            changed[perferredIndex] = true;
            return perferredIndex;
        }
        for (int i=0; i<changed.Length; ++i)
        {
            if (!changed[i])
            {
                changed[i] = true;
                return i;
            }
        }
        return -1;
    }
    private void PurgeUnchangedDevices(bool [] changed)
    {
        for (int i = 0; i < changed.Length; ++i)
        {
            if (!changed[i])
            {
                devices[i] = null;
            }
        }
    }

    
    private void updateControl(int currentControl, ControlStruct current, bool state)
    {
        if(state)
            switch (currentControl)
            {
                case jump:
                    current.jump = true;
                    current.A = true;
                    break;
                case attackButton:
                    current.attack = true;
                    break;
                case action:
                    current.action = true;
                    break;
                case door1:
                    current.B = true;
                    current.door = true;
                    break;
                case door2:
                case door3:
                    current.door = true;
                    break;
                case start:
                    current.inGameMenu = true;
                    break;
            }
    }

    private void updateControl(int currentControl, ControlStruct current, float state)
    {
        //print("current control " + currentControl + " state " + state);

        switch (currentControl)
        {
            case left:
                current.moveLeft = state;
                break;
            case menu:
                if (state > axisThreshold)
                    current.A = true;
                else if (state < -axisThreshold)
                    current.B = true;
                break;
            case attackAxis1:
            case attackAxis2:
                if (state > triggerThrishold)
                {
                    //Debug.LogWarning("setAttack "+state+" axis thresh "+axisThreshold);
                    current.attack = true;
                }
                break;
        }
        return;
    }

    
    class GamepadMonitor
    {
        ControlStruct controls;
        public bool used { get; protected set; }
        public int id { get; protected set; }

        public GamepadMonitor(ControlStruct controls)
        {
            this.controls = controls;
            used = false;
            id = 0;
        }
        public bool AnyChange(ControlStruct controls)
        {
            return this.controls.AnyChange(controls);
        }
        public void Change(ControlStruct controls, int id)
        {
            this.controls = controls;
            used = true;
            this.id = id;
        }
    }

}
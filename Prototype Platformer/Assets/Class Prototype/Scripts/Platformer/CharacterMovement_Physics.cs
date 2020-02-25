using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement_Physics : MonoBehaviour
{
    private static readonly string[] name = { "Bibbs", "Leslie", "Giggles", "Hobbs" };
    private const int North = 0, East = 1, South = 2, West = 3, Any = 4;

    public enum CharacterState
    {
        frozen,
        idle,
        moving,
    }

    [Header("Level Properties")]
    public LevelRandomizer Environment;

    [Header("Player Properties")]
    public int PlayerNumber = 0;
    public Animator anim;
    public GameObject mountingBone;

    [Header("Movment Properties")]
    public float maxSpeed = 10f;
    public float acceleration = 60f;
    public float stopDrag = .9f;
    public float pullDrag = .9f;
    public float doorNearnessThreshold = 5;
    public int doorPullSpeed = 1000;

    [Header("Jump Properties")]
    public float jumpForce = 15f;
    [Range(0f, 1f)]
    public float airControl = 0.85f;

    [Header("Attack Properties")]
    public Transform attackPoint;

    //public bool isActionPressed { get { return _controllerStatus.action; } protected set { } }

    //Private Memeber Variables
    private Rigidbody _rigidbody;

    private bool _canMove = true;
    private bool _canAttack = true;
    private bool _canJump = true;
    private bool _inJump = false;
    private bool _canOpen = true;

    private bool _isGrounded = false;

    private ControlStruct _controllerStatus;

    private Vector3 _storedVelocity = Vector3.zero;
    private CharacterState _storedState;
    private CapsuleCollider _collider;
    
    //private GameObject _currentItem;
    public Item _currentItem;
    private Item defaultWeapon;
    public TextManager info;
    //public Text currentItemHUD;
    private GameObject HUDmount;

    private CharacterState _currentState = CharacterState.idle;

    private bool actionPressed = false;
    private bool actionable = true;

    private int clearText = 0;

    private Vector3 pullDirection;

    private AudioManager audio;

    public AnimationStates AnimState;
    public HUDPointer NavPoint;

    private List<HUDPointer> navPoints;

    public GameObject inGameMenu;
    [HideInInspector]
    public float damageFactor=1;

    private bool gameMenuActive=false;

    private bool ignoreInput = false;

    private HashSet<GameObject> PositiveNormalForce=new HashSet<GameObject>();
    private float groundTimer = 0;
    private const float groundTime = .2f;

    private HashSet<DoorController> currentDoors = new HashSet<DoorController>();

    public int AddDoor(DoorController cont, out DoorController.DoorType newOrientation)
    {
        newOrientation = cont.DoorLocation;
        if (newOrientation == DoorController.DoorType.East ||
            newOrientation == DoorController.DoorType.West)
            if (cont.gameObject.transform.position.x - transform.position.x > 0)
                newOrientation = DoorController.DoorType.East;
            else
                newOrientation = DoorController.DoorType.West;

        else if (newOrientation == DoorController.DoorType.North ||
            newOrientation == DoorController.DoorType.South)
            if (cont.gameObject.transform.position.x - transform.position.x > 0)
                newOrientation = DoorController.DoorType.South;
            else
                newOrientation = DoorController.DoorType.North;

        currentDoors.Add(cont);
        return currentDoors.Count;
    }
    public void RemoveDoor(DoorController cont)
    {
        if (currentDoors.Contains(cont))
            currentDoors.Remove(cont);
    }

    void Start()
    {
        HUDmount = FindObjectOfType<HUDScoreSetter>().transform.Find("HUD_" + name[PlayerNumber] + "/HUD/ItemInfoMountPoint").gameObject;
        navPoints = new List<HUDPointer>();

        GameObject g = Instantiate(FindObjectOfType<CommonPunch>().gameObject);
        g.transform.position = new Vector3(0,0,100);
        _currentItem = g.GetComponent<Item>();
        GameObject g2 = Instantiate(g);
        g2.transform.position = new Vector3(0, 0, 100);
        defaultWeapon = g2.GetComponent<Item>();

        //NoFriction = Resources.Load<PhysicMaterial>("Assets/Class Prototype/Physics Materials/NoFriction.physicMaterial");
        _collider = transform.Find("Collision/Body Collider").gameObject.GetComponent<CapsuleCollider>();
        SetupPhysicsMaterial();
        

        _controllerStatus = new ControlStruct(ControlStruct.None);

        _rigidbody = this.GetComponent<Rigidbody>();

        if (attackPoint == null) attackPoint = this.transform;

        AnimState = new AnimationStates(anim);
        audio = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if (!_canMove||gameMenuActive) return;

        if (isGrounded())
        {
            //print("grounded");
            if (!_isGrounded)
            {
                //if (gameObject.name.Contains("0"))
                //    print("land");
                setLanding(true);
            }
            _isGrounded = true;
        }
        else
        {
            //if (_isGrounded)
            //{
            //    if (gameObject.name.Contains("0"))
            //        print("in air");
            //    setJump(true);
            //}
            //else
            //{
            //    if (gameObject.name.Contains("0"))
            //        print("is grounded already false");
            //}
            _isGrounded = false;
        }

        

        if (_canJump && _isGrounded)
        {
            //if (gameObject.name.Contains("3"))
            //    print("player try jump");
            Jump();
        }
        else
        {
            //if (gameObject.name.Contains("3"))
            //    print("player can't jump " + _isGrounded + " " + _canJump);
            // Force the player to release the jump button between jumps, catch for 2x jump power corner case
            //if (Input.GetAxis("Jump") == 0f) _canJump = true;
            if (!_controllerStatus.jump) _canJump = true;
        }

        if (_canAttack) Attack();
        if (clearText == 0)
        {
            //currentItemHUD.text = _currentItem.getName();
            updateItemHUD(_currentItem.getInUseHUD());
            clearText = -1;
        }
        else clearText--;
    }


    private void updateItemHUD(GameObject newHUD)
    {
        foreach (Transform child in HUDmount.transform)
            Destroy(child.gameObject);
        GameObject g = Instantiate(newHUD) as GameObject;
        g.transform.SetParent(HUDmount.transform, false);
        g.transform.localPosition = Vector3.zero;
    }

    private void LateUpdate()
    {
        _storedVelocity = _rigidbody.velocity;
    }

    private void FixedUpdate()
    {
        if (ignoreInput)
        {
            if (_controllerStatus.B) return;
            if (_controllerStatus.jump) return;
            if (_controllerStatus.inGameMenu) return;
            ignoreInput = false;
        }
        if (gameMenuActive)
        {
            if (_controllerStatus.B)
            {
                inGameMenu.SetActive(false);
                FindObjectOfType<UndestroyableData>().OpenStartMenu();
                SceneLoader loader=FindObjectOfType<SceneLoader>();

                ignoreInput = true;

                loader.sceneLoadDelay = 0;
                loader.sceneFadeDuration = 0;
                loader.RestartScene();
                
            }
            else if (_controllerStatus.jump || _controllerStatus.inGameMenu)
            {
                inGameMenu.SetActive(false);
                gameMenuActive = false;
                ignoreInput = true;
                _canJump = false;
            }
            return;
        }

        if (_controllerStatus.inGameMenu)
        {
            inGameMenu.SetActive(true);
            gameMenuActive = true;
            ignoreInput = true;
            return;
        }

        if (!_canMove)
        {
            _rigidbody.velocity = Vector3.zero;
            return;
        }

        //Vector3 force = Vector3.right * Input.GetAxis(horizontalAxis) * acceleration;

        Vector3 force = Vector3.right * _controllerStatus.moveLeft * acceleration;



        if (_inJump) force *= airControl;

        // Orient player in direction of force, pass in _rigidbody.velocity for facing direction that matches momentum
        Orient(force);

        //add acceleration force to player if moving slower than max speed, overly verbose to allow changes in direction at max speed
        if ((force.x >= 0f && _rigidbody.velocity.x < maxSpeed) || (force.x <= 0f && _rigidbody.velocity.x > -maxSpeed))
        {
            _rigidbody.AddForce(force, ForceMode.Acceleration);
            
        }
        //_rigidbody.velocity += force;

        //if (Mathf.Abs(_rigidbody.velocity.magnitude) > maxSpeed)
        //    _rigidbody.velocity = Vector3.Normalize(_rigidbody.velocity)*maxSpeed;

        if (Mathf.Abs(_controllerStatus.moveLeft)<.1)
        {
            //if(PlayerNumber==0)
            //  print(" force=0");
            //_collider.material = null;
            BecomeSticky();
            if(!_isGrounded||_rigidbody.velocity.y<0)
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x * stopDrag, _rigidbody.velocity.y, _rigidbody.velocity.z * stopDrag);
            else
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x * stopDrag, _rigidbody.velocity.y * stopDrag, _rigidbody.velocity.z * stopDrag);
        }
        else
        {
            BecomeSlippery();
            if (_isGrounded)
            {
                if (Mathf.Abs(_rigidbody.velocity.magnitude) < maxSpeed / 2)
                {
                    // if (PlayerNumber == 0)
                    //print(" force!=0");
                    //_collider.material = NoFriction;
                    _rigidbody.AddForce(force, ForceMode.Acceleration);
                }
                //if (Mathf.Abs(_rigidbody.velocity.magnitude) > maxSpeed)
                //    _rigidbody.velocity = Vector3.Normalize(_rigidbody.velocity) * maxSpeed;
            }

        }
        if (pullDirection != Vector3.zero)
        {
            //print("pull " + pullDirection);
            _rigidbody.AddForce(pullDirection, ForceMode.Acceleration);
            pullDirection = new Vector3(pullDirection.x * pullDrag, pullDirection.y * pullDrag, 0);
            if (Mathf.Abs(pullDirection.x) < 100 && Mathf.Abs(pullDirection.y) < 100) pullDirection = Vector3.zero;
        }

        setRunning(Mathf.Abs(_rigidbody.velocity.x) > .1 && _isGrounded);

        setAttacking(_controllerStatus.attack);



        if (!_controllerStatus.action)
        {
            actionable = true;
            actionPressed = false;
        }
        else if (actionable) actionPressed = true;

        setDoorOpen(_controllerStatus.door);


    }

    public void encounteredItem(Item item)
    {
        if (!_canMove)
            return;

        info.say(item.getName(), 2);
        //currentItemHUD.text="GRAB:  "+item.getName();
        updateItemHUD(item.getPickUpHUD());
        clearText = 2;
        if (actionPressed)
        {
            actionPressed = false;
            actionable = false;

            if (_currentItem != null)
                releaseItem();
            grabItem(item);
        }
    }

    



    private void Jump()
    {
        if (_controllerStatus.jump)
        {
            //print("player jump");
            //add vertical impulse force
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            setJump(true);

           


            _inJump = true;
            _canJump = false;

            _isGrounded = false;
            PositiveNormalForce.Clear();
            groundTimer = 0;
        }
    }

    private void Orient(Vector3 direction)
    {
        Vector3 orientation = Vector3.zero;

        orientation.x = direction.x;

        if (orientation != Vector3.zero) this.transform.forward = orientation;
        //print(orientation);
    }


    private void OnCollisionEnter(Collision col)
    {
        bool isGrounded = false;

        foreach (ContactPoint contact in col.contacts)
        {
            if (contact.point.y < this.transform.position.y - .75f) isGrounded = true;
        }


        if (isGrounded)
        {
            //print("grounded");
            if (!_isGrounded)
            {
                setLanding(true);
                groundTimer = groundTime+1;
            }
            //_isGrounded = true;
            //_rigidbody.velocity = new Vector3(_storedVelocity.x, _rigidbody.velocity.y, 0f);
        }

    }

    private bool isGrounded()
    {
        if(groundTimer>groundTime)
        {
            //if (gameObject.name.Contains("0"))
            //    print("isGrounded =True");
            return true;
        }
        if( PositiveNormalForce.Count > 0)
        {
            groundTimer += Time.deltaTime;
        }
        //if (gameObject.name.Contains("0"))
        //    print("isGrounded =False");
        return false;
    }

    private void OnCollisionStay(Collision col)
    {

        foreach (ContactPoint contact in col.contacts)
        {
            if (isPositiveNormalForce(contact))
            {
                PositiveNormalForce.Add(col.gameObject);
                return;
            }
        }
        PositiveNormalForce.Remove(col.gameObject);
        if (PositiveNormalForce.Count == 0)
            groundTimer = 0;

    }
    void OnCollisionExit(Collision col)
    {
        PositiveNormalForce.Remove(col.gameObject);
        if (PositiveNormalForce.Count == 0)
            groundTimer = 0;
    }
    private bool isPositiveNormalForce(ContactPoint cp)
    {
        return cp.normal.y > .01f;
    }

    private void Attack()
    {
        //if (primaryAttack != null && Input.GetAxis(primaryAttackAxis) > 0.5f)
        //if (primaryAttack != null && _controllerStatus.attack)
        //{
        //    primaryAttack.Fire(attackPoint);
        //}

        //if (secondaryAttack != null && Input.GetAxis(secondaryAttackAxis) > 0.5f)
        //if (secondaryAttack != null && _controllerStatus.action)
        //{
        //    secondaryAttack.Fire(attackPoint);
        //}
    }
    

    public void Freeze(bool value)
    {
        if (_currentItem != null)
            releaseItem();
        _canMove = !value;

        if (value)
        {
            foreach (DoorController d in currentDoors)
                d.closeOutQue(PlayerNumber);

            _storedVelocity = _rigidbody.velocity;
            _storedState = _currentState;
            _rigidbody.velocity = Vector3.zero;

        }
        else
        {
            _rigidbody.velocity = _storedVelocity;
            _currentState = _storedState;
        }
    }

    public void PointTo(Transform target)
    {
        HUDPointer navPoint = Instantiate(NavPoint);
        navPoint.Init(target, transform, Vector3.zero, null);
        navPoints.Add(navPoint);
    }
    public void ClearNavPoints()
    {
        foreach (HUDPointer n in navPoints)
            Destroy(n.gameObject);
        navPoints = new List<HUDPointer>();
    }

    public void ControllerListener(ControlStruct controls)
    {
        //if the new controls message comes from AI, don't let input come from another source
        //otherwise, if the input comes from the last source, overwrite the state
        // if the input comes from a different source, combine states (allows keyboard and controller simotaniously
        if(controls.fromSource(ControlStruct.AI))
            _controllerStatus = controls;
        else if (!_controllerStatus.fromSource(ControlStruct.AI) && _controllerStatus.fromSource(controls.source))
            _controllerStatus = controls;
        else
            _controllerStatus.combine(controls);
        //if (controls.jump)
        //    print("player " + PlayerNumber + " received the jump instruction");
        //if (controls.action)
        //    print("player " + PlayerNumber + " received the action instruction");
        //if (controls.attack)
        //    print("player " + PlayerNumber + " received the attack instruction");
        //if (controls.moveLeft != 0)
        //    print("player " + PlayerNumber + " received the move instruction " + controls.moveLeft);
    }


    














    private void setJump(bool YayOrNay)
    {
        //this is an example of how you would get
        //the individual character type if you want them to make
        //specialized sounds
        // audio.Play("Jump_"+name[PlayerNumber]); //this will output Jump_Bibbs for player 0, Jump Leslie for player 1, etc
        //also note, if you want to have multiple jump sounds and randomly select them, you can add
        //Random.Range[0,5] and you'll get a 0,1,2,3, or 4 (not 5).

        //here's an example of what that might look like, this will print when a player jumps
        //print("Jump_" + name[PlayerNumber] + "_" + Random.Range(1, 5));

        //technically you said you might want to just go male of female for now and individual characters
        //later, for that I would recommend just copying each file (two males and two females) and still
        //using the character names, that way it's up to the voice actors later to replace their individual
        //files if you specifically name them jump_male, then you have to go through and change the code
        //yourself... so more work that way.

        //That said, if you really want male or female tags, here's how you could get that string:
        //print("Jump_" + (name[PlayerNumber] == "Hobbs" || name[PlayerNumber] == "Leslie" ? "Female" : "Male") );

        if(YayOrNay)audio.Play("Jump" + name[PlayerNumber]);


        AnimState.updateAnimationState(AnimationStates.Tag.jump, YayOrNay);
    }

    private void setRunning(bool YayOrNay)
    {

        //if (gameObject.name.Contains("0"))
        //    print("setRunning ="+YayOrNay+"isGrounded "+_isGrounded);
        if (YayOrNay == true)
            AnimState.updateAnimationState(AnimationStates.Tag.run, true);
        else
            if (_isGrounded)
            AnimState.updateAnimationState(AnimationStates.Tag.run, false);
        //not sure if there is a foot step sound, but if so, it can be placed here.
    }

    private void setAttacking(bool YayOrNay)
    {
        if (YayOrNay == true)
        {
            AnimState.updateAnimationState(_currentItem.getAnimationFlag(), true);
            ///anim.SetBool(_currentItem.getAnimationFlag(), true);
            if (_currentItem == null) print("current item is null "+name[PlayerNumber]);

            Transform atkPt = new GameObject().transform;
            atkPt.position = new Vector3(
                mountingBone.transform.position.x,
                mountingBone.transform.position.y,
                transform.position.z);
            atkPt.rotation = transform.rotation;
            _currentItem.use(attackPoint, this.transform);

            if (_currentItem.effect.Contains("useonce"))
            {
                if (_currentItem.effect.Contains("shield"))
                {
                    damageFactor = 1f;
                }
                AnimState.updateAnimationState(_currentItem.getAnimationFlag(), false);
                GameObject oldItem = _currentItem.transform.parent.gameObject;
                releaseItem();
                print("        OLD ITEM"+oldItem.name);
                Destroy(oldItem);
                _currentItem = Instantiate(defaultWeapon);
                print("use once");
            }
            if (defaultWeapon == null) print("default item is null " + name[PlayerNumber]);


            //I know I put this method here for your convenience, but technically,
            //attacking sounds should go in the individual "use" methods in the weapon
            //scripts found at scripts/ItemPickup/PunchPrototype.cs (or ShivPrototype or GunPrototype)
            //that way each weapon type can have it's own sound.
        }
        else
        {
            AnimState.updateAnimationState(_currentItem.getAnimationFlag(), false);
            //anim.SetBool(_currentItem.getAnimationFlag(), false);
        }
    }

    private bool doorOpenandPull(DoorController d)
    {
        
            if (d.open())
        {
            if (d.DoorLocation == DoorController.DoorType.East ||
               d.DoorLocation == DoorController.DoorType.West)
            {
                pullDirection = new Vector3(d.transform.position.x - transform.position.x, 0, 0);
                pullDirection.Normalize();
                pullDirection *= doorPullSpeed;
                return true;
            }
            pullDirection = d.transform.position - transform.position;
            pullDirection.Normalize();
            pullDirection *= doorPullSpeed;
            return true;
        }
        return false;
    }

    private bool openDoorInDirection(DoorController.DoorType dir)
    {
        foreach (DoorController d in currentDoors)
            if (d.DoorLocation == dir)
                if (doorOpenandPull(d))
                    return true;
        return false;
    }

    private bool openADoor()
    {
        foreach (DoorController d in currentDoors)
        {
            if (doorOpenandPull(d))
                return true;
        }
        return false;
    }

    private void setDoorOpen(bool YayOrNay)
    {
        if (YayOrNay == true)
        {
            if (_canOpen)
            {
                _canOpen = false;
                //print("current doors " + currentDoors.Count);
                if (currentDoors.Count == 0)
                    return;

                if (currentDoors.Count == 1)
                {
                    openADoor();
                    return;
                }

                        int preference=South;

                if (!_isGrounded) {
                    //print("       NOT GROUNDED door");
                    //north or south, but above you
                    foreach (DoorController d in currentDoors)
                        if (d.DoorLocation == DoorController.DoorType.North ||
                            d.DoorLocation == DoorController.DoorType.South)
                            if (d.transform.position.x - transform.position.x < 0)
                                if(doorOpenandPull(d))
                                    return;
                    //just north
                    if (openDoorInDirection(DoorController.DoorType.North))
                        return;
                    

                }// preference = North;

                //open a door east or west if moving horizontally
                if (_controllerStatus.moveLeft < -.1 || _controllerStatus.moveLeft > .1)
                {
                    //print("       Left or right door");
                    foreach (DoorController d in currentDoors)
                        if (d.DoorLocation == DoorController.DoorType.East ||
                            d.DoorLocation == DoorController.DoorType.West)
                            if (doorOpenandPull(d))
                                return;
                }
                //open a door south, of no inputs are used and player is grounded
                if (openDoorInDirection(DoorController.DoorType.South))
                    return;
                //lastly, any door will do
                openADoor();
                return;

                if (_controllerStatus.moveLeft < -.1)
                    foreach (DoorController d in currentDoors)
                    {
                        if(d.DoorLocation==DoorController.DoorType.East||
                            d.DoorLocation==DoorController.DoorType.West)
                            if (d.transform.position.x - transform.position.x < 0)
                            {

                            }

                    }//preference = East;
                else if (_controllerStatus.moveLeft > .1) preference = West;
                if (Environment.openNearestDoors(transform.position, doorNearnessThreshold, ref pullDirection, preference))
                { 
                    pullDirection.Normalize();
                    pullDirection *= doorPullSpeed;
                }
            }

        }
        else _canOpen = true;
    }


    private void grabItem(Item item)
    {
        AnimState.updateAnimationState(AnimationStates.Tag.lift, true);

        //if there's an item pickup sound, it can go anywhere here

        item.itemExterior.transform.parent = mountingBone.transform;
        item.itemExterior.transform.localPosition = Vector3.zero;
        item.itemExterior.transform.localRotation = Quaternion.identity;


        Rigidbody rigidBody = item.itemExterior.GetComponent<Rigidbody>();
        rigidBody.velocity = Vector3.zero;
        rigidBody.useGravity = false;
        rigidBody.angularDrag = 0;
        rigidBody.isKinematic = true;

        item.GetComponent<CapsuleCollider>().enabled = false;


        _currentItem = item;
        item.itemExterior.SendMessageUpwards("feedback", true, SendMessageOptions.DontRequireReceiver);
        if (_currentItem.effect.Contains("shield"))
        {
            string[] tokens = _currentItem.effect.Split(',');
            if (!float.TryParse(tokens[1], out damageFactor))
                print("incorrectly parsed shield");
        }
    }

    private void releaseItem()
    {
        if(_currentItem.effect.Contains("shield"))
        {
            damageFactor = 1f;
        }
        //make sure you're not animating an attack with the item to be dropped
        //anim.SetBool(_currentItem.getAnimationFlag(), false);
        AnimState.updateAnimationState(_currentItem.getAnimationFlag(), false);

        //if there's a drop item sound, it can go anywhere here

        _currentItem.GetComponent<CapsuleCollider>().enabled = true;
        _currentItem.itemExterior.transform.parent = null;

        Rigidbody rigidBody = _currentItem.itemExterior.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;

        _currentItem.itemExterior.SendMessageUpwards("feedback", false, SendMessageOptions.DontRequireReceiver);

        if (_currentItem.getType() == Item.Punch)
            Destroy(_currentItem);
        _currentItem = null;
    }

    private void setLanding(bool YayOrNay)
    {
        if (YayOrNay==true)
            AnimState.updateAnimationState(AnimationStates.Tag.land, true);
    }

    private void SetupPhysicsMaterial()
    {
        _collider.material = new PhysicMaterial();
    }

    private void BecomeSlippery()
    {
        _collider.material.dynamicFriction = 0;
        _collider.material.staticFriction = 0;
        _collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
    }
    private void BecomeSticky()
    {
        _collider.material.dynamicFriction = 2;
        _collider.material.staticFriction = 2;
        _collider.material.frictionCombine = PhysicMaterialCombine.Multiply;
    }
}




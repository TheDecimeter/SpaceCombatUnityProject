using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement_Physics : MonoBehaviour
{
    private static readonly string[] name = { "Bibbs", "Leslie", "Giggles", "Hobbs" };
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

    //private GameObject _currentItem;
    public Item _currentItem;
    public TextManager info;

    private CharacterState _currentState = CharacterState.idle;

    private bool actionPressed = false;
    private bool actionable = true;

    private int clearText = 0;

    private Vector3 pullDirection;

    private AudioManager audio;

    public AnimationStates AnimState;
    



    void Start()
    {
        _controllerStatus = new ControlStruct();

        _rigidbody = this.GetComponent<Rigidbody>();

        if (attackPoint == null) attackPoint = this.transform;

        AnimState = new AnimationStates(anim);
        audio = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if (!_canMove) return;

        if (_canJump && _isGrounded)
        {
            Jump();
        }
        else
        {
            // Force the player to release the jump button between jumps, catch for 2x jump power corner case
            //if (Input.GetAxis("Jump") == 0f) _canJump = true;
            if (!_controllerStatus.jump) _canJump = true;
        }

        if (_canAttack) Attack();
        
    }

    private void LateUpdate()
    {
        _storedVelocity = _rigidbody.velocity;
    }

    private void FixedUpdate()
    {
        if (!_canMove)
        {
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
        if (force == Vector3.zero)
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x * stopDrag, _rigidbody.velocity.y, _rigidbody.velocity.z * stopDrag);

        if (pullDirection != Vector3.zero)
        {
            print("pull " + pullDirection);
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
        //if (Input.GetAxis(jumpAxis) > 0.5f)
        {
            //add vertical impulse force
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            setJump(true);

            // jump sound


            _inJump = true;
            _canJump = false;

            _isGrounded = false;
            // jump sound
            //FindObjectsOfType<audioManager>().Play("ok");
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
            setLanding(true);
            _isGrounded = true;
            _rigidbody.velocity = new Vector3(_storedVelocity.x, _rigidbody.velocity.y, 0f);
        }

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

    private void OnCollisionExit(Collision col)
    {

    }

    public void Freeze(bool value)
    {
        if (_currentItem != null)
            releaseItem();
        _canMove = !value;

        if (value)
        {
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

    public void ControllerListener(ControlStruct controls)
    {
        _controllerStatus = controls;
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
        print("Jump_" + name[PlayerNumber] + "_" + Random.Range(1, 5));

        //technically you said you might want to just go male of female for now and individual characters
        //later, for that I would recommend just copying each file (two males and two females) and still
        //using the character names, that way it's up to the voice actors later to replace their individual
        //files if you specifically name them jump_male, then you have to go through and change the code
        //yourself... so more work that way.

        //That said, if you really want male or female tags, here's how you could get that string:
        print("Jump_" + (name[PlayerNumber] == "Hobbs" || name[PlayerNumber] == "Leslie" ? "Female" : "Male") );

        audio.Play("Jump");
    }

    private void setRunning(bool YayOrNay)
    {
        //anim.SetBool("isRunning", YayOrNay);
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
            if (_currentItem == null) print("current item is null");
            _currentItem.use(this.gameObject.transform.parent, this.transform);
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
    private void setDoorOpen(bool YayOrNay)
    {
        if (YayOrNay == true)
        {
            if (_canOpen)
            {
                _canOpen = false;
                if (Environment.openNearestDoors(transform.position, doorNearnessThreshold, ref pullDirection))
                {

                    //if there's an door open sound, it can go anywhere here
                    //HOWEVER, if there's a sound for door closing too, it might be best to put them
                    //in the Level Generation/DoorBehavior.cs
                    //there is an open and close method at the bottom of the file
                    //note that in the open statement has that "if" condition, so put it inside of there
                    //like below the "_state=Opened"


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
        item.itemExterior.transform.position = mountingBone.transform.position;
        item.itemExterior.transform.forward = mountingBone.transform.forward;


        Rigidbody rigidBody = item.itemExterior.GetComponent<Rigidbody>();
        rigidBody.velocity = Vector3.zero;
        rigidBody.useGravity = false;
        rigidBody.angularDrag = 0;
        rigidBody.isKinematic = true;




        _currentItem = item;
        item.itemExterior.SendMessageUpwards("feedback", true, SendMessageOptions.DontRequireReceiver);
    }

    private void releaseItem()
    {
        //make sure you're not animating an attack with the item to be dropped
        //anim.SetBool(_currentItem.getAnimationFlag(), false);
        AnimState.updateAnimationState(_currentItem.getAnimationFlag(), false);

        //if there's a drop item sound, it can go anywhere here

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

    
}




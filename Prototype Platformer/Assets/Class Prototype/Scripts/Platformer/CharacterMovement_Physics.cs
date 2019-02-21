using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement_Physics : MonoBehaviour
{
    public enum CharacterState
    {
        frozen,
        idle,
        moving,
    }

    [Header("Player Properties")]
    public int PlayerNumber=0;
    public Animator anim;
    public GameObject mountingBone;

    [Header("Input Axes")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string jumpAxis = "Jump";
    public string primaryAttackAxis = "Fire1";
    public string secondaryAttackAxis = "Fire2";

    [Header("Movment Properties")]
    public float maxSpeed = 10f;
    public float acceleration = 60f;
    public float stopDrag = .9f;

    [Header("Jump Properties")]
    public float jumpForce = 15f;
    [Range(0f, 1f)]
    public float airControl = 0.85f;

    [Header("Attack Properties")]
    public Weapon primaryAttack;
    public Weapon secondaryAttack;
    public Transform attackPoint;

    //public bool isActionPressed { get { return _controllerStatus.action; } protected set { } }

    //Private Memeber Variables
    private Rigidbody _rigidbody;

    private bool _canMove = true;
    private bool _canAttack = true;
    private bool _canJump = true;
    private bool _inJump = false;

    private bool _isGrounded = false;

    private ControlStruct _controllerStatus;

    private Vector3 _storedVelocity = Vector3.zero;
    private CharacterState _storedState;

    //private GameObject _currentItem;
    public Item _currentItem;
    public TextManager info;

    private CharacterState _currentState = CharacterState.idle;

    private bool actionPressed=false;
    private bool actionable = true;

    private int clearText = 0;

    

    void Start()
    {
        _controllerStatus = new ControlStruct();

        _rigidbody = this.GetComponent<Rigidbody>();

        if (attackPoint == null) attackPoint = this.transform;
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

        if (Mathf.Abs(_rigidbody.velocity.x) > .1 && _isGrounded)
            anim.SetBool("isRunning", true);
        else
            anim.SetBool("isRunning", false);

        if (_controllerStatus.attack)
        {
            anim.SetBool("isAttacking", true);
            if (_currentItem == null) print("current item is null");
            _currentItem.use(this.gameObject.transform.parent, this.transform);
        }
        else
            anim.SetBool("isAttacking", false);


        if (!_controllerStatus.action)
        {
            actionable = true;
            actionPressed = false;
        }
        else if (actionable) actionPressed = true;
    }

    public void encounteredItem(Item item)
    {
        if (!_canMove)
            return;

        info.say ( item.getName(),2);
        if (actionPressed)
        {
            actionPressed = false;
            actionable = false;

            if (_currentItem != null)
                releaseItem();
            grabItem(item);
        }
    }


    private void grabItem(Item item)
    {
        item.itemExterior.transform.parent = mountingBone.transform;
        item.itemExterior.transform.position = mountingBone.transform.position;
        item.itemExterior.transform.forward = mountingBone.transform.forward;


        Rigidbody rigidBody = item.itemExterior.GetComponent<Rigidbody>();
        rigidBody.velocity = Vector3.zero;
        rigidBody.useGravity = false;
        rigidBody.angularDrag = 0;
        rigidBody.isKinematic = true;
        



        //BoxCollider colider = item.transform.GetChild(0).GetComponent<BoxCollider>();
        //colider.isTrigger = false;


        _currentItem = item;
        item.itemExterior.SendMessageUpwards("feedback", true, SendMessageOptions.DontRequireReceiver);
    }
    private void releaseItem()
    {
        _currentItem.itemExterior.transform.parent = null;

        Rigidbody rigidBody = _currentItem.itemExterior.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        //Vector3 pos = _currentItem.itemExterior.transform.position;
        //_currentItem.itemExterior.transform.position = new Vector3(pos.x, pos.y, 0);

        //BoxCollider colider = _currentItem.transform.GetChild(0).GetComponent<BoxCollider>();
        //colider.isTrigger = false;
        //BoxCollider colider = _currentItem.GetComponent<BoxCollider>();
        //colider.isTrigger = true;

        _currentItem.itemExterior.SendMessageUpwards("feedback", false, SendMessageOptions.DontRequireReceiver);

        if (_currentItem.getType() == Item.Punch)
            Object.Destroy(_currentItem);
        _currentItem = null;
    }

    private void Jump()
    {
        if (_controllerStatus.jump)
            //if (Input.GetAxis(jumpAxis) > 0.5f)
            {
            //add vertical impulse force
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            _inJump = true;
            _canJump = false;

            _isGrounded = false;
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

        foreach(ContactPoint contact in col.contacts)
        {
            if (contact.point.y < this.transform.position.y - .75f) isGrounded = true;
        }


        if (isGrounded)
        {
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
}




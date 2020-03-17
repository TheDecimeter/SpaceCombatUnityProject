using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerCameraFollow : MonoBehaviour {

    public Transform followTransform;
    public bool useFixedUpdate;
    public float followSpeed = 8f;

    [Space(12)]

    public bool lookAhead = false;
    public float lookAheadAmount = 2f;
    public float lookAheadSpeed = 2f;

    public TextManager info;
    public float infoOffset = 2f;

    [Space(12)]

    public float startDelay = 0.5f;


    private bool _canFollow;

    private Vector3 _zOffset;
    private Vector3 _target;

    private Vector3 _lookOffset;

    private PlayerHealth player;

    void Start () 
    {
        _zOffset.z = this.transform.position.z - followTransform.position.z;     

        if (startDelay != 0f) 
        {
            StartCoroutine(StartFollowDelay());
        }
        else 
        {
            _canFollow = true;
        }

        player = followTransform.gameObject.GetComponent<PlayerHealth>();

        followTransform.gameObject.GetComponent<CameraLocator>().CameraLocation = this.transform.GetChild(0);
  	}
	
    void Update()
    {
        //if the player dies zoom out and follow the player with the most health
        if (player != null && player.isDead)
        {
            if (player.GetComponent<Rigidbody>().useGravity)
            {
                PlayerHealth p=null;
                int h = 0;
                foreach(Transform child in FindObjectOfType<LevelRandomizer>().PlayerArray.transform)
                {
                    PlayerHealth pPlayer = child.GetComponent<PlayerHealth>();
                    int health = pPlayer.getHealth();
                    if (health > h)
                    {
                        h = health;
                        p = pPlayer;
                    }
                }
                if (p != null)
                {
                    info.transform.SetParent(null);
                    player = p;
                    followTransform = p.transform;
                    //_zOffset.z = this.transform.position.z - followTransform.position.z;
                }
                else
                    player = null;
                _zOffset.z *= 4;
                _zOffset.z = Mathf.Clamp(_zOffset.z, -32, 0);
            }
        }
        _target = followTransform.position;
        info.transform.position = new Vector3(_target.x, _target.y+infoOffset, info.transform.position.z);

        if (lookAhead)
        {
            _lookOffset = Vector3.Lerp(_lookOffset, (followTransform.forward * lookAheadAmount), Time.deltaTime * lookAheadSpeed);
            _target += _lookOffset;
        }

        _target += _zOffset;

        if (!useFixedUpdate && _canFollow)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, _target, Time.deltaTime * followSpeed);
        }
    }

	void FixedUpdate () 
    {
        if (useFixedUpdate && _canFollow)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, _target, Time.fixedDeltaTime * followSpeed);
        }
	}

    IEnumerator StartFollowDelay ()
    {
        yield return new WaitForSeconds(startDelay);

        _canFollow = true;
    }

}

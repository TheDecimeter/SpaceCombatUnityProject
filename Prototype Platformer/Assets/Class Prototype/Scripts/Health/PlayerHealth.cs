using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class PlayerHealth : MonoBehaviour {

    private static readonly string[] name = { "Bibbs", "Leslie", "Giggles", "Hobbs" };
    private AudioManager audio;
    private int PlayerNumber;

    public int startingHealth = 100;
    public int blurFrames = 500;

    public UnityEvent damageEvent;
    public UnityEvent deathEvent;

    private int _currentHealth, frameCounter;
    private bool _canTakeDamage = true;
    public GameObject colliders;

    public TextManager info;
    private AnimationStates AnimState;
    public PostProcessLayer blurComponent;

    public bool isDead{get; protected set;}

    public void Start ()
    {
        isDead = false;
        _currentHealth = startingHealth;
        audio = FindObjectOfType<AudioManager>();
        PlayerNumber = GetComponent<CharacterMovement_Physics>().PlayerNumber;
        AnimState = GetComponent<CharacterMovement_Physics>().AnimState;
    }

    void FixedUpdate()
    {
        if (frameCounter < blurFrames)
        {
            frameCounter++;
            if (frameCounter == blurFrames)
                endBlur();
        }
    }

    public void DealDamage(DamageMessage message)
    {
        //sounds of getting hurt can go here
        if (message.friend==gameObject||message.friend == colliders)
            return;
        if (!_canTakeDamage) return;


        if (AnimState == null)
            AnimState = GetComponent<CharacterMovement_Physics>().AnimState;
        AnimState.updateAnimationState(AnimationStates.Tag.damage, true);

        print("deal damage " + message.damage);

        if (message.effect == "blur")
        {
            startBlur();
        }

        _currentHealth -= message.damage;

        print("PLAYER HEALTH: " + _currentHealth);
        info.say("HP: " + _currentHealth, 15);

        damageEvent.Invoke();

        if (_currentHealth <= 0)
        {
            PlayerDeath();
            _currentHealth = 0;
        }
    }

    private void startBlur()
    {
        print("start blur");
        blurComponent.gameObject.GetComponent<CameraBob>().Bob=true;
        blurComponent.enabled = true;
        frameCounter = 0;
    }
    private void endBlur()
    {
        print("end blur");
        blurComponent.gameObject.GetComponent<CameraBob>().Bob = false;
        blurComponent.enabled = false;
    }

    public void PlayerDeath ()
    {
        if (AnimState == null)
            AnimState = GetComponent<CharacterMovement_Physics>().AnimState;
        AnimState.startDie();
        GetComponent<Rigidbody>().useGravity = false;

        transform.Find("Collision/Foot Collider").gameObject.GetComponent<SphereCollider>().enabled = false;
        transform.Find("Collision/Body Collider").gameObject.GetComponent<CapsuleCollider>().enabled = false;


        isDead = true;
        //death sounds can go here
        audio.Play("Death" + name[PlayerNumber]);

        info.say("ya DEAD!", -1);
        print("PLAYER DEAD");
        deathEvent.Invoke();
        _canTakeDamage = false;

     

    }
}

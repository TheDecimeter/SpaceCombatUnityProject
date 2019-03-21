using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

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
    public Text health;

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
        if (message.friend==gameObject||message.friend == colliders)
            return;
        if (!_canTakeDamage) return;


        audio.Play("Hurt" + name[PlayerNumber]);
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
        health.text =" "+ _currentHealth;

        damageEvent.Invoke();

        if (_currentHealth <= 0)
        {
            PlayerDeath();
            _currentHealth = 0;
        }
    }

    public int getHealth()
    {
        return _currentHealth;
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
        print("Death" + name[PlayerNumber]);

        switch (Random.Range(1, 10))
        {
            case 0:
                health.text = " : (";
                info.say("I should have chosen love", -1);
                break;
            case 1:
                health.text = " :'{";
                info.say("KAAHHHHNN", -1);
                break;
            case 3:
                health.text = " : P";
                info.say("REKT??", -1);
                break;
            case 4:
                health.text = " :'{";
                info.say("Mother was right about you", -1);
                break;
            default:
                health.text = "0";
                info.say("", -1);
                break;
        }
        deathEvent.Invoke();
        _canTakeDamage = false;

     

    }
}

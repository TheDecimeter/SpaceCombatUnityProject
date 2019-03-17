using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerHealth : MonoBehaviour {

    private static readonly string[] name = { "Bibbs", "Leslie", "Giggles", "Hobbs" };
    private AudioManager audio;
    private int PlayerNumber;

    public int startingHealth = 100;

    public UnityEvent damageEvent;
    public UnityEvent deathEvent;

    private int _currentHealth;
    private bool _canTakeDamage = true;
    public GameObject colliders;

    public TextManager info;
    private AnimationStates AnimState;

    public void Start ()
    {
        _currentHealth = startingHealth;
        audio = FindObjectOfType<AudioManager>();
        PlayerNumber = GetComponent<CharacterMovement_Physics>().PlayerNumber;
        AnimState = GetComponent<CharacterMovement_Physics>().AnimState;
    }

    public void DealDamage (DamageMessage message)
    {
        //sounds of getting hurt can go here
        if (message.friend == colliders)
            return;
        if (!_canTakeDamage) return;


        if (AnimState == null)
            AnimState = GetComponent<CharacterMovement_Physics>().AnimState;
        AnimState.updateAnimationState(AnimationStates.Tag.damage, true);


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

    public void PlayerDeath ()
    {

        //death sounds can go here
        print("Dead_" + name[PlayerNumber] + "_" + Random.Range(1, 5));

        info.say("ya DEAD!", -1);
        print("PLAYER DEAD");
        deathEvent.Invoke();
        _canTakeDamage = false;
    }
}

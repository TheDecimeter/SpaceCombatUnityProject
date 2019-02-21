using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerHealth : MonoBehaviour {

    public int startingHealth = 100;

    public UnityEvent damageEvent;
    public UnityEvent deathEvent;

    private int _currentHealth;
    private bool _canTakeDamage = true;
    public GameObject colliders;

    public TextManager info;

    public void Start ()
    {
        _currentHealth = startingHealth;
    }

    public void DealDamage (DamageMessage message)
    {
        if (message.friend == colliders)
            return;
        if (!_canTakeDamage) return;

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
        info.say("ya DEAD!", -1);
        print("PLAYER DEAD");
        deathEvent.Invoke();
        _canTakeDamage = false;
    }
}

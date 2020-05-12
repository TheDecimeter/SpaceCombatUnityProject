using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStates
{
    public const int Run = 0, Jump = 1, Land = 2, Idle = 3, Lift = 5;

    private bool run, jump, land, lift, attack, damage;

    private string lastAttack;


    private Animator anim;


    public AnimationStates(Animator anim)
    {
        this.anim = anim;
    }

    public void stopAnimating()
    {
        anim.enabled = false;
    }

    public void updateAnimationState(string state, bool OnOrOff)
    {
        switch (state)
        {
            case Tag.run:
                startRun(OnOrOff);
                break;
            case Tag.jump:
                startJump(OnOrOff);
                break;
            case Tag.lift:
                startLift(OnOrOff);
                break;
            case Tag.land:
                startLand(OnOrOff);
                break;
            case Tag.damage:
                startDamage(OnOrOff);
                break;
            default:
                startAttack(state, OnOrOff);
                break;
        }
    }

    private void startRun(bool value)
    {
        if (run==value)
            return;

        //Debug.Log("run "+value);
        run = value;
        anim.SetBool(Tag.run, value);

        if (value == true)
        {
            startJump(false);
            startLift(false);
            startLand(false);
            startDamage(false);
        }
    }


    private void startJump(bool value)
    {
        if (jump == value)
            return;

        //Debug.Log("jump " + value);
        jump = value;
        anim.SetBool(Tag.jump, value);

        if (value == true)
        {
            startRun(false);
            startLift(false);
            startLand(false);
            startDamage(false);
        }
        else
        {
            startLand(true);
        }
    }

    private void startLift(bool value)
    {
        if (lift == value)
            return;
        lift = value;
        anim.SetBool(Tag.lift, value);

        if (value == true)
        {
            startJump(false);
            startRun(false);
            startLand(false);
            startDamage(false);
        }
    }

    private void startLand(bool value)
    {
        if (land == value)
            return;

        //Debug.Log("land " + value);
        land = value;
        anim.SetBool(Tag.land, value);

        if (value == true)
        {
            startJump(false);
            startLift(false);
            startRun(false);
            startDamage(false);
        }
    }
    private void startDamage(bool value)
    {
        if (damage == value)
            return;
        damage = value;
        anim.SetBool(Tag.damage, value);
    }

    private void startAttack(string AttackType, bool value)
    {
        if (attack == value && AttackType==lastAttack)
            return;

        if (lastAttack != null && AttackType != lastAttack)
            startAttack(lastAttack, false);

        lastAttack = AttackType;

        attack = value;
        anim.SetBool(AttackType, value);

        if (value == true)
        {
            startDamage(false);
            startLift(false);
        }
    }


    public void startDie()
    {
        anim.SetBool(Tag.die, true);

        if(lastAttack!=null)
            startAttack(lastAttack, false);

        startDamage(false);
        startJump(false);
        startRun(false);
        startLand(false);
        startDamage(false);
        startLift(false);
    }

    

    public static class Tag
    {
        public const string run = "isRunning", lift = "isLifting", jump = "isJumping", land = "isFalling",
            attack = "isAttacking", damage = "isHit", shoot = "isShooting", idle="isIdle", die="isDying";
    }
}

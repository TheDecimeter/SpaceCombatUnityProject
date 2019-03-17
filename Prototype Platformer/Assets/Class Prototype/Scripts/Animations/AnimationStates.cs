using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStates
{
    public const int Run = 0, Jump = 1, Land = 2, Idle = 3, Lift = 5;

    private bool run, jump, land, lift, attack, damage;


    private Animator anim;


    public AnimationStates(Animator anim)
    {
        this.anim = anim;
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
        jump = value;
        anim.SetBool(Tag.jump, value);

        if (value == true)
        {
            startRun(false);
            startLift(false);
            startLand(false);
            startDamage(false);
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
        if (attack == value)
            return;
        attack = value;
        anim.SetBool(AttackType, value);

        if (value == true)
        {
            startDamage(false);
        }
    }


    private void TurnOnMovementFlag(int flag)
    {
        switch (flag)
        {
            case Run:
                //print("run animation");
                anim.SetBool("isRunning", true);
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", false);
                anim.SetBool("isLifting", false);
                return;
            case Jump:
                //print("jump animation");
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", true);
                anim.SetBool("isFalling", false);
                anim.SetBool("isLifting", false);
                return;
            case Land:
                //print("land animation");
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", true);
                anim.SetBool("isLifting", false);
                return;
            case Idle:
                //print("idle animation");
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", false);
                anim.SetBool("isLifting", false);
                return;
            case Lift:
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", false);
                anim.SetBool("isLifting", true);
                return;


        }
    }

    public static class Tag
    {
        public const string run = "isRunning", lift = "isLifting", jump = "isJumping", land = "isFalling",
            attack = "isAttacking", damage = "isHit", shoot = "isShooting", idle="isIdle";
    }
}

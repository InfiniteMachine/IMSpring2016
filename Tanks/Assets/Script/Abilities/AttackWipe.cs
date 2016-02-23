﻿using UnityEngine;
using System.Collections;

public class AttackWipe : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    private CamFollow cFollow;
    //Use for initiation
    void Start()
    {
        cFollow = Camera.main.GetComponent<CamFollow>();
    }

    void UpdateTimer()
    {
        if (fireTimer >= 0)
            fireTimer -= Time.deltaTime;
    }

    //Used for effects that happen over time
    void Update()
    {
        UpdateTimer(); // Should probably always be called.

        // Update effect code here

    }

    public bool IsAttack()
    {
        //true if special attack, false if special defense
        return true;
    }

    public void StartAction()
    {
        //Start Action Here, If the action doesnt happen over time, call FinishAction()
        Vector2 bound1 = new Vector2(cFollow.GetLeftBound(), cFollow.GetTopBound());
        Vector2 bound2 = new Vector2(cFollow.GetRightBound(), cFollow.GetLowerBound());
        Collider2D[] cols = Physics2D.OverlapAreaAll(bound1, bound2);
        foreach(Collider2D col in cols)
        {
            if(col.tag == "Bullet")
            {
                Destroy(col.gameObject);
            }
        }
        FinishAction();
    }

    //Don't touch
    private bool canFire = false;
    private float fireTimer = 0; //counter variable
    public bool CanFire()
    {
        return fireTimer <= 0;
    }

    //Call this to start delay timer
    private void FinishAction()
    {
        fireTimer = fireDelay; //Resets the timer so that the action can't be fired repeatedly
    }
}

﻿using UnityEngine;
using System.Collections;

public class ForcePush : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public float force = 5.0f;
    public float radius = 2;
    public float disableTime = 2f;

    private ParticleSystem pSystem;
    private ParticleSystem pSystem2;

    void Start()
    {
        if (Manager.instance.gameMode == Manager.GameModes.BLITZKRIEG)
            fireDelay *= 0.5f;
        pSystem = transform.FindChild("Leo Force Push").GetComponent<ParticleSystem>();
        pSystem2 = pSystem.transform.FindChild("SubEmitter").GetComponent<ParticleSystem>();
    }

    void UpdateTimer()
    {
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer < 0)
                fireTimer = 0;
        }
    }

    //Used for effects that happen over time
    void Update()
    {
        UpdateTimer(); // Should probably always be called.

        // Update effect code here

    }

    public void ForceDeactivate()
    {
        FinishAction();
    }
    public bool IsAttack()
    {
        //true if special attack, false if special defense
        return true;
    }

    public void StartAction()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in cols)
        {
            if (col.tag == "Player" && col.gameObject != gameObject)
            {
                col.gameObject.GetComponent<PlayerController>().ForcePush((Vector2)((col.transform.position - (transform.position + Vector3.down * 0.25f)).normalized) * Mathf.Lerp(force, 0, Vector2.Distance(transform.position, col.transform.position) / radius), disableTime);
            }
        }
        pSystem.Play();
        pSystem2.Play();
        FinishAction();
        //Start Action Here, If the action doesnt happen over time, call FinishAction()
    }

    //Don't touch
    private float fireTimer = 0; //counter variable
    public bool CanFire()
    {
        return fireTimer == 0;
    }

    //Call this to start delay timer
    private void FinishAction()
    {
        fireTimer = fireDelay; //Resets the timer so that the action can't be fired repeatedly
    }

    public void AllowFire()
    {
        fireTimer = -1;
        StartAction();
    }

    public void ResetCounters()
    {
        fireTimer = 0;
    }

    public float GetPercentage()
    {
        return (fireDelay - fireTimer) / fireDelay;
    }
}
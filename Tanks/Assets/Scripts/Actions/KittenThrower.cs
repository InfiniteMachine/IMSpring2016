﻿using UnityEngine;
using System.Collections;

public class KittenThrower : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public float angle = 45f;
    public float speed = 3f;
    public GameObject kitten;
    private PlayerController pCont;
    private int playerID;
    void Start()
    {
        pCont = GetComponent<PlayerController>();
        playerID = pCont.GetPlayerID();
        if (Manager.instance.gameMode == Manager.GameModes.BLITZKRIEG)
            fireDelay *= 0.5f;
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
        GameObject go = (GameObject)Instantiate(kitten, transform.position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, angle) * Vector2.right * speed;
        pCont.IgnoreCollision(go.GetComponent<Collider2D>());
        ExplodingKitten k = go.GetComponent<ExplodingKitten>();
        k.player = gameObject;
        k.SetPlayerID(playerID);

        go = (GameObject)Instantiate(kitten, transform.position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 180 - angle) * Vector2.right * speed;
        pCont.IgnoreCollision(go.GetComponent<Collider2D>());
        k = go.GetComponent<ExplodingKitten>();
        k.player = gameObject;
        k.SetPlayerID(playerID);
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
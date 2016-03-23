using UnityEngine;
using System.Collections;

public class BoomerangThrow : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 7; //seconds between uses
    public GameObject boomerangPrefab;
    private GameObject boomerang;
    private PlayerController pCont;
    void Start()
    {
        pCont = transform.GetComponent<PlayerController>();
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
        if (boomerang != null)
            Destroy(boomerang);
        FinishAction();
    }

    public bool IsAttack()
    {
        //true if special attack, false if special defense
        return true;
    }

    public void StartAction()
    {
        //Start Action Here, If the action doesnt happen over time, call FinishAction()
        boomerang = (GameObject)Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
        BoomerangSepter bs = boomerang.GetComponent<BoomerangSepter>();
        bs.Setup(transform, (int)Mathf.Sign(transform.localScale.x));
        bs.SetPlayerID(pCont.playerID);
        pCont.IgnoreCollision(boomerang.GetComponent<Collider2D>());
        FinishAction();
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
        fireTimer = fireDelay; //Resets the timer so that the action can't be fired repeatedl
    }

    public void AllowFire()
    {
        fireTimer = -1;
        StartAction();
    }
}
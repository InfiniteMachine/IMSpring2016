using UnityEngine;
using System.Collections;

public class BearBite : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    private BoxCollider2D bearBite;
    public Vector2 Offset;
    public Vector2 Size;
    public float bearBiteDelay = 1;
    private bool bearBiteActive = false;
    private float bearBiteTimer = 0;

    //Use for initiation
    void Start()
    {
        bearBite = gameObject.AddComponent<BoxCollider2D>();
        bearBite.isTrigger = true;
        bearBite.enabled = false;
        bearBite.offset = Offset;
        bearBite.size = Size;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && bearBite.IsTouching(col))
        {
            col.gameObject.GetComponent<PlayerController>().Attack();
        }
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
        if (bearBiteTimer >= 0)
        {
            bearBiteTimer -= Time.deltaTime;
        }
        else if (bearBiteActive)
        {
            bearBite.enabled = false;
            bearBiteActive = false;
            FinishAction();
        }
        // Update effect code here

    }
    
    public void ForceDeactivate()
    {
        bearBiteTimer = 0;
        bearBite.enabled = false;
        bearBiteActive = false;
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
        bearBite.enabled = true;
        bearBiteTimer = bearBiteDelay;
        bearBiteActive = true;
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
}
using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public float shieldDelay = 10;
    private CircleCollider2D shield;
    private bool shieldActive = false;
    private float shieldTimer = 0;
    
    //Use for initiation
    void Start()
    {
        shield = gameObject.AddComponent<CircleCollider2D>();
        shield.isTrigger = true;
        shield.radius = 1.0f;
        shield.enabled = false;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet"))
        {
            Destroy(col.gameObject);
        }
    }


    //Used for effects that happen over time
    void Update()
    {
        if (fireTimer >= 0)
            fireTimer -= Time.deltaTime;
        if (shieldTimer >= 0)
        {
            shieldTimer -= Time.deltaTime;
        }
        else if (shieldActive)
        {
            shield.enabled = false;
            shieldActive = false;
            FinishAction();
        }
    }

    public bool IsAttack()
    {
        //true if special attack, false if special defense
        return false;
    }

    public void StartAction()
    {
        //Start Action Here, If the action doesnt happen over time, call FinishAction()
        shield.enabled = true;
        shieldTimer = shieldDelay;
        shieldActive = true;
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
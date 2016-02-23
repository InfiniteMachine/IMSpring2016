using UnityEngine;
using System.Collections;

public class ForcePush : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public float force = 5.0f;
    public float radius = 2;

    //Use for initiation
    void Start()
    {

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
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in cols)
        {
            if (col.tag == "Bullet")
            {
                Destroy(col.gameObject);
            }
        }
        FinishAction();
        //Start Action Here, If the action doesnt happen over time, call FinishAction()
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
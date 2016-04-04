using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public float blinkDistance = 5;
    private BoxCollider2D col;
    private Rigidbody2D rBody;
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        if(Manager.instance.gameMode == Manager.GameModes.BLITZKRIEG)
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
        return false;
    }

    public void StartAction()
    {
        //Start Action Here, If the action doesnt happen over time, call FinishAction()
        float direction = 1;
        if (rBody.velocity.x > 0)
            direction = 1;
        else if (rBody.velocity.x < 0)
            direction = -1;
        else if (transform.lossyScale.x > 0)
            direction = 1;
        else
            direction = -1;
        float distance = blinkDistance * direction;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direction, blinkDistance + col.bounds.extents.x);
        if(hit)
        {
            if(direction > 0)
                distance = hit.point.x - col.bounds.extents.x - transform.position.x;
            else
                distance = hit.point.x + col.bounds.extents.x - transform.position.x;
        }
        Debug.Log(distance);
        transform.Translate(distance, 0, 0);
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
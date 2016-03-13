using UnityEngine;
using System.Collections;

public class SpeedBoost : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    private PlayerController PControl;
    private float BoostTimer = 0;
    public float BoostDelay = 2.0f;
    private bool boostActive = false;
    //Use for initiation
    void Start()
    {
        PControl = GetComponent<PlayerController>();
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
        if (BoostTimer >= 0)
        {
            BoostTimer -= Time.deltaTime;
        }
        else if (boostActive)
        {
            PControl.movementSpeed /= 2;
            FinishAction();
        }
    }

    public void ForceDeactivate()
    {
        BoostTimer = 0;
        if (boostActive)
            PControl.movementSpeed /= 2;
        boostActive = false;
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
        PControl.movementSpeed *= 2;
        BoostTimer = BoostDelay;
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

    public void AllowFire()
    {
        fireTimer = -1;
        StartAction();
    }
}
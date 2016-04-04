using UnityEngine;
using System.Collections;

public class FreezeWave : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public GameObject wave;
    private PlayerController pCont;
    //Use for initiation
    void Start()
    {
        pCont = GetComponent<PlayerController>();
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
        GameObject go = (GameObject)Instantiate(wave, transform.position, Quaternion.identity);
        go.GetComponent<Wave>().direction = -1;
        go.transform.localScale = Vector3.zero;
        pCont.IgnoreCollision(go.GetComponent<Collider2D>());
        go = (GameObject)Instantiate(wave, transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.zero;
        go.GetComponent<Wave>().direction = 1;
        pCont.IgnoreCollision(go.GetComponent<Collider2D>());
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
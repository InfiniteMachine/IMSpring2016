using UnityEngine;
using System.Collections;

public class DropShield : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public float dropShieldDelay = 10;
    public Vector2 dropShieldlocation;
    public GameObject dropShield;
    private GameObject storage;
    public float dropShieldTimer = 0;
    private bool dropShieldActive = false;

    //Use for initiation
    void Start()
    {
        storage = Instantiate(dropShield);
        storage.transform.SetParent(transform);
        storage.transform.localPosition = dropShieldlocation;
        storage.SetActive(false);
    }

    //Used for effects that happen over time
    void Update()
    {
        if (fireTimer >= 0)
            fireTimer -= Time.deltaTime;
        if (dropShieldTimer >= 0)
        {
            dropShieldTimer -= Time.deltaTime;
        }else if (dropShieldActive)
        {
            storage.SetActive(false);
            dropShieldActive = false;
            FinishAction();
        }
    }

    public bool IsAttack()
    {
        //true if special attack, false if special defense
        return true;
    }

    public void StartAction()
    {
        //Start Action Here, If the action doesnt happen over time, call FinishAction()
        storage.SetActive(true);
        dropShieldActive = true;
        dropShieldTimer = dropShieldDelay;
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
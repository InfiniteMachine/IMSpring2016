using UnityEngine;
using System.Collections;

public class PyramidShield : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public float PyramidDelay = 10;
    public Vector2 Pyramidlocation;
    public GameObject pyramidShield;
    private GameObject storage;
    public float PyramidShieldTimer = 0;
    private bool PyramidShieldActive = false;

    //Use for initiation
    void Start()
    {
        storage = Instantiate(pyramidShield);
        storage.transform.SetParent(transform);
        storage.transform.localPosition = Pyramidlocation;
        storage.SetActive(false);
    }

    //Used for effects that happen over time
    void Update()
    {
        if (fireTimer >= 0)
            fireTimer -= Time.deltaTime;
        if (PyramidShieldTimer >= 0)
        {
            PyramidShieldTimer -= Time.deltaTime;
        }
        else if (PyramidShieldActive)
        {
            storage.SetActive(false);
            PyramidShieldActive = false;
            FinishAction();
        }
    }
    public void ForceDeactivate()
    {
        PyramidShieldTimer = 0;
        storage.SetActive(false);
        PyramidShieldActive = false;
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
        storage.SetActive(true);
        PyramidShieldActive = true;
        PyramidShieldTimer = PyramidDelay;
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
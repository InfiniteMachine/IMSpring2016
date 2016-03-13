using UnityEngine;

public class Spears : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public float spearDelay = 3;
    public Vector2 spearlocation;
    public GameObject spear;
    private GameObject storage;
    public float spearTimer = 0;
    private bool spearActive = false;

    //Use for initiation
    void Start()
    {
        
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
        if (spearTimer >= 0)
        {
            spearTimer -= Time.deltaTime;
        }
        else if (spearActive)
        {
            Destroy(storage);
            spearActive = false;
            FinishAction();
        }
    }

    public void ForceDeactivate()
    {
        spearTimer = 0;
        if(spearActive)
            Destroy(storage);
        spearActive = false;
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
        storage = Instantiate(spear);
        storage.transform.SetParent(transform);
        storage.transform.localPosition = spearlocation;
        spearActive = true;
        spearTimer = spearDelay;
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
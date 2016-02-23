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
        storage = Instantiate(spear);
        storage.transform.SetParent(transform);
        storage.transform.localPosition = spearlocation;
        storage.SetActive(false);
    }

    void UpdateTimer()
    {
        if (fireTimer >= 0)
            fireTimer -= Time.deltaTime;

        if (spearTimer >= 0)
        {
            spearTimer -= Time.deltaTime;
        }
        else if (spearActive)
        {
            storage.SetActive(false);
            spearActive = false;
            FinishAction();
        }
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
        //Start Action Here, If the action doesnt happen over time, call FinishAction()
        storage.SetActive(true);
        spearActive = true;
        spearTimer = spearDelay;
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
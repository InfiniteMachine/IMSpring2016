using UnityEngine;
public class SolarFlare : MonoBehaviour, IAction {
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public GameObject particles;
    public float radius = 3.185f;

    private int playerID;
    void Start()
    {
        playerID = GetComponent<PlayerController>().GetPlayerID();
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
        Instantiate(particles, transform.position, Quaternion.identity);
        //3.185
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach(Collider2D col in cols)
        {
            if (col.gameObject != gameObject && col.tag == "Player")
            {
                col.gameObject.GetComponent<PlayerController>().Attack(playerID);
            }
                
        }
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

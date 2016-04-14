using UnityEngine;
using System.Collections;

public class FreezeWave : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses

    public float killDistance = 2f;
    public float freezeDistance = 4f;
    public float freezeDelay = 1f;
    private float freezeCounter = 0;
    private bool waitingForFreeze = false;
    public float height = 4f;
    public GameObject freezer;
    private int playerID;
    //Use for initiation
    void Start()
    {
        PlayerController pCont = GetComponent<PlayerController>();
        if (Manager.instance.gameMode == Manager.GameModes.BLITZKRIEG)
            fireDelay *= 0.5f;
        playerID = pCont.GetPlayerID();
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
        if (waitingForFreeze)
        {
            freezeCounter += Time.deltaTime;
            if(freezeCounter >= freezeDelay)
            {
                freezeCounter = 0;
                waitingForFreeze = false;
                //Freeze
                Collider2D[] hits = Physics2D.OverlapAreaAll((Vector2)transform.position - (Vector2.up * height) - (Vector2.right * freezeDistance),
                    (Vector2)transform.position + (Vector2.right * freezeDistance));
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].gameObject != gameObject && hits[i].tag == "Player")
                    {
                        GameObject go = (GameObject)Instantiate(freezer, hits[i].transform.position, Quaternion.identity);
                        go.GetComponent<Freezer>().player = hits[i].gameObject;
                    }
                }
                FinishAction();
            }
        }
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
        //KillEnemies too close
        Collider2D[] hits = Physics2D.OverlapAreaAll((Vector2)transform.position - (Vector2.up * height) - (Vector2.right * killDistance),
            (Vector2)transform.position + (Vector2.right * killDistance));
        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].gameObject != gameObject)
            {
                if(hits[i].tag == "Ground")
                {
                    DestructibleObj des = hits[i].GetComponent<DestructibleObj>();
                    if(des != null)
                        Destroy(des.gameObject);
                }
                else if(hits[i].tag == "Player")
                {
                    PlayerController pCont = hits[i].GetComponent<PlayerController>();
                    if (!pCont.IsShield())
                        pCont.Attack(playerID);
                }
            }
        }
        waitingForFreeze = true;
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
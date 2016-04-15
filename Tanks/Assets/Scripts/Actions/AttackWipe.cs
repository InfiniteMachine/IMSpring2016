using UnityEngine;
using System.Collections;

public class AttackWipe : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    private CamFollow cFollow;
    public GameObject explosionPrefab;
    private int playerID;
    public Sprite newSprite;
    //Use for initiation
    void Start()
    {
        cFollow = Camera.main.GetComponent<CamFollow>();
        if (Manager.instance.gameMode == Manager.GameModes.BLITZKRIEG)
            fireDelay *= 0.5f;
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
        //Start Action Here, If the action doesnt happen over time, call FinishAction()
        Vector2 bound1 = new Vector2(cFollow.GetLeftBound(), cFollow.GetTopBound());
        Vector2 bound2 = new Vector2(cFollow.GetRightBound(), cFollow.GetLowerBound());
        Collider2D[] cols = Physics2D.OverlapAreaAll(bound1, bound2);
        foreach (Collider2D col in cols)
        {
            if (col.tag == "Bullet")
            {
                TankBullet tBullet = col.GetComponent<TankBullet>();
                if (tBullet != null)
                {
                    Instantiate(explosionPrefab, col.transform.position, Quaternion.identity);
                    col.gameObject.SetActive(false);
                    col.gameObject.SetActive(true);
                    tBullet.SetPower(tBullet.power, Quaternion.FromToRotation(Vector2.right, col.transform.position - transform.position).eulerAngles.z, tBullet.intType);
                    col.GetComponent<SpriteRenderer>().sprite = newSprite;
                    tBullet.SetPlayerID(playerID);
                }
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

using UnityEngine;

public class Spears : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    private int playerID;
    public GameObject spearObject;
    public float speed = 5f;
    private PlayerController pCont;
    //Use for initiation
    void Start()
    {
        playerID = GetComponent<PlayerController>().GetPlayerID();
        if (Manager.instance.gameMode == Manager.GameModes.BLITZKRIEG)
            fireDelay *= 0.5f;
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
        GameObject spear = (GameObject)Instantiate(spearObject, (Vector2)transform.position + Vector2.right * 2f, Quaternion.Euler(0, 0, 0));
        spear.GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
        spear.GetComponent<IPlayerID>().SetPlayerID(playerID);
        pCont.IgnoreCollision(spear.GetComponent<Collider2D>());

        spear = (GameObject)Instantiate(spearObject, transform.position + Quaternion.Euler(0, 0, 45) * Vector2.right * 2f, Quaternion.Euler(0, 0, 45));
        spear.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 45) * Vector2.right * speed;
        spear.GetComponent<IPlayerID>().SetPlayerID(playerID);
        pCont.IgnoreCollision(spear.GetComponent<Collider2D>());

        spear = (GameObject)Instantiate(spearObject, transform.position + Quaternion.Euler(0, 0, 90) * Vector2.right * 2f, Quaternion.Euler(0, 0, 90));
        spear.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 90) * Vector2.right * speed;
        spear.GetComponent<IPlayerID>().SetPlayerID(playerID);
        pCont.IgnoreCollision(spear.GetComponent<Collider2D>());

        spear = (GameObject)Instantiate(spearObject, transform.position + Quaternion.Euler(0, 0, 135) * Vector2.right * 2f, Quaternion.Euler(0, 0, 135));
        spear.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 135) * Vector2.right * speed;
        spear.GetComponent<IPlayerID>().SetPlayerID(playerID);
        pCont.IgnoreCollision(spear.GetComponent<Collider2D>());

        spear = (GameObject)Instantiate(spearObject, transform.position + Quaternion.Euler(0, 0, 180) * Vector2.right * 2f, Quaternion.Euler(0, 0, 180));
        spear.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 180) * Vector2.right * speed;
        spear.GetComponent<IPlayerID>().SetPlayerID(playerID);
        pCont.IgnoreCollision(spear.GetComponent<Collider2D>());

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
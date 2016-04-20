using UnityEngine;
public class ArrowStorm : MonoBehaviour, IAction {
    //Can be modified
    public float fireDelay = 5; //seconds between uses
    public float shootDuration = 3;
    private float shootCounter = 0;
    private bool shooting = false;
    public float shootDelay = 0.25f;
    private TankGun tGun;

    void Start()
    {
        tGun = GetComponentInChildren<TankGun>();
        if (Manager.instance.gameMode == Manager.GameModes.BLITZKRIEG)
            fireDelay *= 0.5f;
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
        if (shooting)
        {
            shootCounter -= Time.deltaTime;
            if (shootCounter <= 0)
            {
                FinishAction();
                shooting = false;
                CancelInvoke("FireRepeating");
            }
        }
    }

    public void ForceDeactivate()
    {
        FinishAction();
        shooting = false;
        CancelInvoke("FireRepeating");
    }

    public bool IsAttack()
    {
        //true if special attack, false if special defense
        return true;
    }

    public void StartAction()
    {
        InvokeRepeating("FireRepeating", 0, shootDelay);
        shootCounter = shootDuration;
        shooting = true;
    }

    public void FireRepeating()
    {
        Vector3 euler = tGun.transform.eulerAngles;
        tGun.Fire();
        tGun.transform.eulerAngles = new Vector3(0, 0, euler.z - 10f);
        tGun.Fire(false);
        tGun.transform.eulerAngles = new Vector3(0, 0, euler.z + 10f);
        tGun.Fire(false);
        tGun.transform.eulerAngles = euler;
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
        if (shooting)
            return 0;
        return (fireDelay - fireTimer) / fireDelay;
    }
}

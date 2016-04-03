using UnityEngine;
using System.Collections;

public class BearBite : MonoBehaviour, IAction
{
    //Can be modified
    public float fireDelay = 5; //seconds between uses

    public GameObject bearBitePrefab;
    private Animator bearBiteAnimator;

    private BoxCollider2D bearBite;
    public Vector2 offset = new Vector2(1.5f, 0);
    public Vector2 size = new Vector2(1, 1.5f);

    public float bearBiteDelay = 1;
    private bool bearBiteActive = false;
    private float bearBiteTimer = 0;

    private int tForceEnd;
    private int tPlayAnim;

    //Use for initiation
    void Start()
    {
        bearBite = gameObject.AddComponent<BoxCollider2D>();
        bearBite.size = size;
        bearBite.offset = offset;
        bearBite.isTrigger = true;
        bearBite.enabled = false;

        GameObject go = (GameObject)Instantiate(bearBitePrefab);
        go.transform.SetParent(transform);
        go.transform.localPosition = offset;
        go.transform.rotation = Quaternion.identity;
        bearBiteAnimator = go.GetComponent<Animator>();

        tForceEnd = Animator.StringToHash("tForceEnd");
        tPlayAnim = Animator.StringToHash("tPlayAnim");
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && bearBite.IsTouching(col))
        {
            col.gameObject.GetComponent<PlayerController>().Attack();
        }
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
        if (bearBiteTimer >= 0)
        {
            bearBiteTimer -= Time.deltaTime;
        }
        else if (bearBiteActive)
        {
            bearBite.enabled = false;
            bearBiteActive = false;
            FinishAction();
        }
        // Update effect code here

    }
    
    public void ForceDeactivate()
    {
        bearBiteTimer = 0;
        bearBite.enabled = false;
        bearBiteActive = false;
        bearBiteAnimator.SetTrigger(tForceEnd);
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
        bearBite.enabled = true;
        bearBiteTimer = bearBiteDelay;
        bearBiteActive = true;
        bearBiteAnimator.SetTrigger(tPlayAnim);
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
}
using UnityEngine;
using System.Collections;

public class BatShield : MonoBehaviour, IAction
{
    public float fireDelay = 5;
    public float batShieldDelay = 10;
    private CircleCollider2D batShield;
    private bool batShieldActive = false;
    private float batShieldTimer = 0;

    void Start()
    {
        batShield = gameObject.AddComponent<CircleCollider2D>();
        batShield.isTrigger = true;
        batShield.radius = 1.0f;
        batShield.enabled = false;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet"))
        {
            Destroy(col.gameObject);
        }
    }

    void Update()
    {
        if (fireTimer >= 0)
            fireTimer -= Time.deltaTime;
        if (batShieldTimer >= 0)
        {
            batShieldTimer -= Time.deltaTime;
        }
        else if (batShieldActive)
        {
            batShield.enabled = false;
            batShieldActive = false;
            FinishAction();
        }

    }

    public bool IsAttack()
    {
        return false;
    }

    public void StartAction()
    {
        batShield.enabled = true;
        batShieldTimer = batShieldDelay;
        batShieldActive = true;
    }

    private bool canFire = false;
    private float fireTimer = 0;
    public bool CanFire()
    {
        return fireTimer <= 0;
    }

    private void FinishAction()
    {
        fireTimer = fireDelay;
    }
}

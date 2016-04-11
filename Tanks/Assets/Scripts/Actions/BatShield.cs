using UnityEngine;
using System.Collections;

public class BatShield : MonoBehaviour, IAction
{
    public float fireDelay = 5;
    public float batShieldDuration = 10;
    public GameObject shieldPrefab;
    private CircleCollider2D batShield;
    private bool batShieldActive = false;
    private float batShieldTimer = 0;
    private InputController iCont;

    private PlayerController pCont;
    private TankGun tGun;
    private Rigidbody2D rbody;

    void Start()
    {
        iCont = GetComponent<InputController>();
        rbody = GetComponent<Rigidbody2D>();
        pCont = GetComponent<PlayerController>();
        tGun = GetComponentInChildren<TankGun>();
        batShield = gameObject.AddComponent<CircleCollider2D>();
        batShield.isTrigger = true;
        batShield.radius = 1.0f;
        batShield.enabled = false;

        GameObject shield = Instantiate(shieldPrefab);
        shield.transform.SetParent(transform);
        shield.transform.localPosition = Vector3.zero + Vector3.forward * -0.1f;
        shieldPrefab = shield;
        shieldPrefab.SetActive(false);
        if (Manager.instance.gameMode == Manager.GameModes.BLITZKRIEG)
            fireDelay *= 0.5f;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Bullet" && batShieldActive)
        {
            Vector2 direction;
            TankBullet tBullet = col.GetComponent<TankBullet>();
            if (tBullet != null)
            {
                RaycastHit2D hit = Physics2D.Raycast(col.transform.position, tBullet.myRigidbody.velocity.normalized);
                if (hit)
                {
                    if (hit.collider.gameObject == gameObject)
                        direction = Vector2.Reflect(tBullet.myRigidbody.velocity, hit.normal);
                    else
                        direction = -tBullet.myRigidbody.velocity;
                }
                else
                    direction = -tBullet.myRigidbody.velocity;
                col.gameObject.SetActive(false);
                col.gameObject.SetActive(true);
                pCont.IgnoreCollision(col);
                Physics2D.IgnoreCollision(batShield, col);
                tBullet.SetPower(tBullet.power, Quaternion.FromToRotation(Vector2.right, direction).eulerAngles.z, tBullet.intType);
            }
        }
    }

    void Update()
    {
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer < 0)
                fireTimer = 0;
        }
        if (batShieldTimer >= 0)
        {
            batShieldTimer -= Time.deltaTime;
            shieldPrefab.transform.localScale = Vector3.one * Mathf.Lerp(1.5f, 2.5f, batShieldTimer / batShieldDuration);
            if (batShieldActive && iCont.GetButton(InputController.Buttons.FIRE))
            {
                ForceDeactivate();
                tGun.Fire();
            }
        }
        else if (batShieldActive)
        {
            ForceDeactivate();
        }
    }

    public void ForceDeactivate()
    {
        batShieldTimer = 0;
        iCont.ClearButton(InputController.Buttons.SPECIAL_DEFENSE);
        batShield.enabled = false;
        batShieldActive = false;
        tGun.enabled = true;
        shieldPrefab.SetActive(false);
        FinishAction();
    }

    public bool IsAttack()
    {
        return false;
    }

    public void StartAction()
    {
        batShield.enabled = true;
        shieldPrefab.transform.localScale = Vector3.one * 3f;
        batShieldTimer = batShieldDuration;
        batShieldActive = true;
        tGun.enabled = false;
        shieldPrefab.SetActive(true);
    }

    private float fireTimer = 0;
    public bool CanFire()
    {
        return fireTimer == 0;
    }

    private void FinishAction()
    {
        fireTimer = fireDelay;
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
        if (batShieldActive)
            return 0;
        return (fireDelay - fireTimer) / fireDelay;
    }
}

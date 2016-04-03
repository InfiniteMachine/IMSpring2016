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
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer < 0)
                fireTimer = 0;
        }
        if (batShieldTimer >= 0 && iCont.GetButton(InputController.Buttons.SPECIAL_DEFENSE))
        {
            batShieldTimer -= Time.deltaTime;
            shieldPrefab.transform.localScale = Vector3.one * Mathf.Lerp(1.5f, 2.5f, batShieldTimer / batShieldDuration);
        }
        else if (batShieldActive)
        {
            iCont.ClearButton(InputController.Buttons.SPECIAL_DEFENSE);
            batShield.enabled = false;
            batShieldActive = false;
            pCont.enabled = true;
            tGun.enabled = true;
            rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            shieldPrefab.SetActive(false);
            FinishAction();
        }
    }

    public void ForceDeactivate()
    {
        batShieldTimer = 0;
        iCont.ClearButton(InputController.Buttons.SPECIAL_DEFENSE);
        batShield.enabled = false;
        batShieldActive = false;
        pCont.enabled = true;
        tGun.enabled = true;
        rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
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
        pCont.enabled = false;
        tGun.enabled = false;
        rbody.constraints = RigidbodyConstraints2D.FreezeAll;
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
}

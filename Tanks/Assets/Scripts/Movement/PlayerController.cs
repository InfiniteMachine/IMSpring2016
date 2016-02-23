using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
    public string characterName = "PlaceHolder";

    public float movementSpeed = 4f;
    public float accelDuration = 1f;

    private Transform crown;
    private bool hasCrown = false;
    public Vector3 crownLocation = new Vector3(0, 0, -0.1f);

    public float respawnTime = 3f;
    public float respawnCounter = 0;
    
    public float jumpVel = 5f;
    public float ignoreRange = 15f;
    
    [HideInInspector]
    public int playerID;
    [HideInInspector]
    public int controllerNumber = -1;

    private Vector3 startLocation;

    //Components
    private Rigidbody2D rBody;
    //private StayGrounded sGround;
    private IsGrounded groundCheck;
    private InputController iCont;
    private IAction specialAttack;
    private IAction specialDefense;

    private List<Collider2D> colliders;
    private List<SpriteRenderer> renderers;

    //ExtraGravity
    [Header("Extra Gravity")]
    public float downAccelDuration = 1f;
    public float downMaxAccel = 0.5f;
    private float gravityCounter = 0;

    private float forceMoveCounter = 0;
    private float forceMoveSpeed = 0;
    public float forceMoveDuration = 0.5f;

    private bool canDash = false;
    public float dashVelocity = 4;

    public float spawnInvulnurability = 2;
    private float spawnCounter = 0;

    private float disabledCounter = 0;
    // Use this for initialization
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        //sGround = GetComponent<StayGrounded>();
        groundCheck = GetComponentInChildren<IsGrounded>();
        iCont = GetComponent<InputController>();
        IAction[] attachedActions = GetComponents<IAction>();
        for(int i = 0; i < attachedActions.Length; i++)
        {
            if (attachedActions[i].IsAttack())
                specialAttack = attachedActions[i];
            else
                specialDefense = attachedActions[i];
        }
        GameObject go = GameObject.FindGameObjectWithTag("Baton");
        if (go != null)
            crown = go.transform;
        startLocation = transform.position;
        renderers = new List<SpriteRenderer>();
        renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
        renderers.Add(GetComponent<SpriteRenderer>());
        colliders = new List<Collider2D>();
        colliders.AddRange(GetComponentsInChildren<Collider2D>());
        colliders.AddRange(GetComponents<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        if (!Manager.instance.activeMatch)
            return;
        if (respawnCounter > 0)
        {
            respawnCounter -= Time.deltaTime;
            if(respawnCounter <= 0)
            {
                Respawn();
            }
        }
        else
        {
            if(spawnCounter > 0)
            {
                spawnCounter -= Time.deltaTime;
            }
            Vector2 velocity = rBody.velocity;
            if (forceMoveCounter > 0)
            {
                velocity.x = forceMoveSpeed;
                forceMoveCounter -= Time.deltaTime;
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(transform.localScale.x) * Mathf.Sign(forceMoveSpeed);
                transform.localScale = scale;
            }
            else {
                if (iCont.GetAxis(InputController.Axis.MOVE) != 0)
                {
                    velocity.x = movementSpeed * iCont.GetAxis(InputController.Axis.MOVE);
                }
                else
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, 0, movementSpeed / accelDuration * Time.deltaTime);
                }
            }

            if (rBody.velocity.y < 0)
            {
                gravityCounter += Time.fixedDeltaTime;
                velocity.y = rBody.velocity.y - Mathf.Lerp(0, downMaxAccel, gravityCounter / downAccelDuration);
            }
            else
            {
                gravityCounter = 0;
                velocity.y = rBody.velocity.y;
            }

            if (iCont.GetButton(InputController.Buttons.SPECIAL_FIRE) && specialAttack.CanFire())
            {
                specialAttack.AllowFire();
            }
            if (iCont.GetButton(InputController.Buttons.SPECIAL_DEFENSE) && specialDefense.CanFire())
            {
                specialDefense.AllowFire();
            }
            if (groundCheck.CheckGrounded())
            {
                canDash = true;
                if (iCont.GetButton(InputController.Buttons.JUMP))
                {
                    //sGround.Ignore(0.1f);
                    velocity.y = jumpVel;
                    iCont.ClearButton(InputController.Buttons.JUMP);
                }
            }
            else if(canDash)
            {
                if (iCont.GetButton(InputController.Buttons.DASH_DOWN))
                {
                    canDash = false;
                    velocity.y = -dashVelocity;
                }
            }
            if (disabledCounter > 0)
            {
                disabledCounter -= Time.deltaTime;
                return;
            }
            rBody.velocity = velocity;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (respawnCounter > 0 || !enabled)
            return;
        if (other.gameObject.CompareTag("Baton"))
        {
            other.enabled = false;
            hasCrown = true;
            Manager.instance.GiveBaton(playerID);
        }
        else if (other.gameObject.CompareTag("KillBox"))
        {
            Die(true);
        }
        else if (other.gameObject.CompareTag("Bullet") && spawnCounter <= 0)
        {
            Die(false);
        }
        else if (other.gameObject.CompareTag("ScreenWrap"))
        {
            forceMoveCounter = forceMoveDuration;
            if (other.name.Contains("Left"))
            {
                forceMoveSpeed = movementSpeed;
            }
            else
            {
                forceMoveSpeed = -movementSpeed;
            }
        }
    }
    
    void LateUpdate()
    {
        if (hasCrown)
        {
            crown.position = transform.position + crownLocation;
        }
    }

    public void Attack()
    {
        Die(false);
    }

    private void Die(bool self)
    {
        respawnCounter = respawnTime;
        if (hasCrown)
        {
            if (!crown.GetComponent<SpriteRenderer>().isVisible)
                Manager.instance.ResetBaton(true);
            else
                Manager.instance.ResetBaton(self);
            hasCrown = false;
        }
        GetComponentInChildren<TankGun>().enabled = false;
        SetInteractable(false);
    }

    public void ForcePush(Vector2 force, float disable)
    {
        rBody.AddForce(force, ForceMode2D.Impulse);
        disabledCounter = disable;
    }

    private void Respawn()
    {
        if (Manager.instance != null)
        {
            transform.position = Manager.instance.GetRandomSpawn();
        }
        else {
            transform.position = startLocation;
        }
        rBody.velocity = Vector2.zero;
        GetComponentInChildren<TankGun>().enabled = true;
        SetInteractable(true);
        spawnCounter = spawnInvulnurability;
    }

    private void SetInteractable(bool interactable)
    {
        foreach (Collider2D col in colliders)
            col.enabled = interactable;
        foreach (SpriteRenderer srend in renderers)
            srend.enabled = interactable;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IPlayerID {
    //Game Stuff
    public string characterName = "PlaceHolder";
    private int playerID;
    public void SetPlayerID(int id) { playerID = id; }
    public int GetPlayerID() { return playerID; }
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
    //Respawn Variables
    [Header("Respawn")]
    public float respawnTime = 3f;
    private float respawnCounter = 0;
    public float spawnInvulnurability = 2;
    private float spawnCounter = 0;
    [Header("Movement")]
    public float movementSpeed = 4f;
    public float accelDuration = 1f;
    //Force Movement
    private float forceMoveCounter = 0;
    private float forceMoveSpeed = 0;
    public float wrapMoveDuration = 0.5f;
    private bool interactable = true;
    [Header("Extra Gravity")]
    public float downAccelDuration = 1f;
    public float downMaxAccel = 0.5f;
    private float gravityCounter = 0;    
    [Header("Jump")]
    public float jumpVel = 5f;
    public float doubleJumpVel = 5f;
    public float extraJumpDelay = 0.25f;
    private bool canJump = false;
    private bool canDoubleJump = false;
    [Header("Dash")]
    public float dashVelocity = 4;
    public float sideDashDuration = 0.5f;
    public float sideDashSpeed = 10f;
    private bool canDash = false;
    public float dashDelay = 1.5f;
    private float dashCounter = 0f;
    private bool canSideDash = true;
    //Ability Specific
    private float disabledCounter = 0;
    //Crown
    private Transform crown;
    private bool hasCrown = false;
    private Transform crownLocation;
    //Animation variables
    private Animator aController;
    private ParticleSystem jumpParticles;
    private ParticleSystem explosionParticles;
    private ParticleSystem dustParticles;
    // Use this for initialization
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        //sGround = GetComponent<StayGrounded>();
        groundCheck = GetComponentInChildren<IsGrounded>();
        iCont = GetComponent<InputController>();
        IAction[] attachedActions = GetComponents<IAction>();
        for (int i = 0; i < attachedActions.Length; i++)
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
        aController = GetComponent<Animator>();
        crownLocation = transform.FindChild("CrownLocation");
        GetComponentInChildren<TankGun>().playerID = playerID;
        movementSpeed *= Manager.instance.moveMultiplier;
        explosionParticles = transform.FindChild("player explosion").GetComponent<ParticleSystem>();
        jumpParticles = transform.FindChild("Jump poof").GetComponent<ParticleSystem>();
        dustParticles = transform.FindChild("dust poof").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Manager.instance.activeMatch)
            return;
        if (respawnCounter > 0)
        {
            respawnCounter -= Time.deltaTime;
            if (respawnCounter <= 0)
            {
                Respawn();
            }
        }
        else
        {
            if (spawnCounter > 0)
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
                    if (disabledCounter > 0)
                    {
                        if (Mathf.Sign(iCont.GetAxis(InputController.Axis.MOVE)) != Mathf.Sign(velocity.x))
                        {
                            velocity.x = Mathf.MoveTowards(velocity.x, movementSpeed * iCont.GetAxis(InputController.Axis.MOVE), movementSpeed * Time.deltaTime);
                        }
                    }
                    else
                    {
                        velocity.x = movementSpeed * iCont.GetAxis(InputController.Axis.MOVE);
                    }
                }
                else if(!groundCheck.CheckGrounded())
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, 0, movementSpeed / accelDuration * Time.deltaTime);
                }
                else
                {
                    velocity.x = 0;
                }
                if (dashCounter >= 0)
                    dashCounter -= Time.deltaTime;
                if (canSideDash && dashCounter <= 0 && iCont.GetButton(InputController.Buttons.DASH))
                {
                    iCont.ClearButton(InputController.Buttons.DASH);
                    SoundManager.instance.PlayOneShot("dash");
                    forceMoveCounter = sideDashDuration;
                    if(velocity.x == 0)
                        forceMoveSpeed = sideDashSpeed * Mathf.Sign(transform.localScale.x);
                    else
                        forceMoveSpeed = sideDashSpeed * Mathf.Sign(velocity.x);
                    dashCounter = dashDelay;
                    canSideDash = false;
                }
            }

            if (rBody.velocity.y < 0)
            {
                gravityCounter += Time.deltaTime;
                velocity.y = rBody.velocity.y - Mathf.Lerp(0, downMaxAccel, gravityCounter / downAccelDuration);
            }
            else
            {
                gravityCounter = 0;
                velocity.y = rBody.velocity.y;
            }
            if (specialAttack != null)
            {
                if (specialDefense.GetPercentage() != 0 && iCont.GetButton(InputController.Buttons.SPECIAL_FIRE) && specialAttack.CanFire())
                {
                    specialAttack.AllowFire();
                    SoundManager.instance.PlayOneShot("special_attack");
                }
            }
            if (specialDefense != null)
            {
                if (iCont.GetButton(InputController.Buttons.SPECIAL_DEFENSE) && specialDefense.CanFire())
                {
                    specialDefense.AllowFire();
                    SoundManager.instance.PlayOneShot("special_attack");
                }
            }
            if (groundCheck.CheckGrounded() && velocity.y <= 0)
            {
                canDash = true;
                canSideDash = true;
                canJump = true;
                canDoubleJump = true;
            }
            else
            {
                if (canDash)
                {
                    if (iCont.GetButton(InputController.Buttons.DASH_DOWN))
                    {
                        canDash = false;
                        velocity.y = -dashVelocity;
                    }
                }
                canJump = false;
            }

            if ((canJump || canDoubleJump) && iCont.GetButton(InputController.Buttons.JUMP))
            {
                if (canJump)
                {
                    canJump = false;
                    velocity.y = jumpVel;
                    jumpParticles.Play();
                }
                else
                {
                    canDoubleJump = false;
                    velocity.y = doubleJumpVel;
                    jumpParticles.Play();
                }
                iCont.ClearButton(InputController.Buttons.JUMP);
                SoundManager.instance.PlayOneShot("jump");
            }

            if (disabledCounter > 0)
            {
                disabledCounter -= Time.deltaTime;
                if (rBody.velocity.sqrMagnitude <= movementSpeed * movementSpeed)
                    disabledCounter = 0;
            }
            if (aController != null)
            {
                aController.SetBool("bMoving", (velocity.x != 0));
            }
            
            if (canJump && velocity.y < 0)
                velocity.y = 0;
            if (interactable)
                rBody.velocity = velocity;
            else
                rBody.velocity = Vector2.zero;
        }
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Baton"))
        {
            hasCrown = true;
            col.collider.enabled = false;
            Manager.instance.GiveBaton(playerID);
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
            SoundManager.instance.PlayOneShot("crown_pickup");
        }
        else if (other.gameObject.CompareTag("KillBox"))
        {
            Die(true, playerID);
            SoundManager.instance.PlayOneShot("sploosh");
        }
        else if (other.gameObject.CompareTag("Bullet") && spawnCounter <= 0 && specialDefense.GetPercentage() != 0)
        {
            Die(false, other.GetComponent<IPlayerID>().GetPlayerID());
        }
        else if (other.gameObject.CompareTag("ScreenWrap"))
        {
            forceMoveCounter = wrapMoveDuration;
            if (other.name.Contains("Left"))
            {
                forceMoveSpeed = -movementSpeed;
            }
            else
            {
                forceMoveSpeed = movementSpeed;
            }
            SoundManager.instance.PlayOneShot("warp");
        }else if(other.gameObject.tag == "Ground" && other.transform.position.y < transform.position.y && rBody.velocity.y < -0.25f)
        {
            dustParticles.Play();
            SoundManager.instance.PlayOneShot("land");
        }
    }
    
    private void ResetJump()
    {
        canJump = false;
    }

    void LateUpdate()
    {
        if (hasCrown)
        {
            crown.position = crownLocation.position;
        }
    }

    public void Attack(int player)
    {
        Die(false, player);
    }

    private void Die(bool self, int player)
    {
        Manager.instance.RecordDeath(player, playerID);
        Die(self);
    }

    private void Die(bool self)
    {
        explosionParticles.Play();
        if(!self)
            SoundManager.instance.PlayOneShot("player_death");
        respawnCounter = respawnTime;
        Invoke("ResetPosition", respawnTime / 2);
        if (hasCrown)
        {
            if (!crown.GetComponent<SpriteRenderer>().isVisible)
                Manager.instance.ResetBaton(true);
            else
                Manager.instance.ResetBaton(self);
            hasCrown = false;
        }
        GetComponentInChildren<TankGun>().enabled = false;
        
        rBody.velocity = Vector2.zero;
        SetInteractable(false);
        if (specialAttack != null)
        {
            specialAttack.ForceDeactivate();
            specialAttack.ResetCounters();
        }
        if (specialDefense != null)
        {
            specialDefense.ForceDeactivate();
            specialDefense.ResetCounters();
        }
        CancelInvoke("UnFreeze");
    }

    private void ResetPosition()
    {
        if (Manager.instance != null)
            transform.position = Manager.instance.GetRandomSpawn();
        else
            transform.position = startLocation;
    }

    public void ForcePush(Vector2 force, float disable)
    {
        rBody.AddForce(force, ForceMode2D.Impulse);
        disabledCounter = disable;
    }

    private void Respawn()
    {
        SetInteractable(true);
        spawnCounter = spawnInvulnurability;
        SoundManager.instance.PlayOneShot("player_spawn");
        
    }

    private void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
        if(!interactable)
        {
            iCont.ResetAxes();
            iCont.ResetButtons();
        }
        GetComponentInChildren<TankGun>().enabled = interactable;
        iCont.enabled = interactable;
        foreach (Collider2D col in colliders)
            col.enabled = interactable;
        foreach (SpriteRenderer srend in renderers)
            srend.enabled = interactable;
        if (interactable)
            rBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            rBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void IgnoreCollision(Collider2D ignoreCol, bool ignore = true)
    {
        foreach (Collider2D col in colliders)
            Physics2D.IgnoreCollision(col, ignoreCol, ignore);
    }

    public bool Intersects(Collider2D col)
    {
        foreach (Collider2D c in colliders)
        {
            if(c.bounds.Intersects(col.bounds))
                return true;
        }
        return false;
    }

    public void Freeze(float duration)
    {
        SetInteractable(false);
        StartCoroutine(UnFreeze(duration));
    }

    IEnumerator UnFreeze(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetInteractable(true);
    }

    public bool IsShield()
    {
        return specialDefense.GetPercentage() == 0;
    }
}
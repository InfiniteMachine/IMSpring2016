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
    private bool waitingForJump = false;
    [Header("Dash")]
    public float dashVelocity = 4;
    public float sideDashDuration = 0.5f;
    public float sideDashSpeed = 10f;
    private bool canDash = false;
    public float dashDelay = 1.5f;
    private float dashCounter = 0f;
    //Ability Specific
    private float disabledCounter = 0;
    //Crown
    private Transform crown;
    private bool hasCrown = false;
    private Transform crownLocation;
    //Animation variables
    private Animator aController;
    private bool playingMove = false;
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
                    velocity.x = movementSpeed * iCont.GetAxis(InputController.Axis.MOVE);
                }
                else if(groundCheck.CheckGrounded())
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, 0, movementSpeed / accelDuration * Time.deltaTime);
                }
                if (dashCounter > 0)
                    dashCounter -= Time.deltaTime;
                if (dashCounter <= 0 && iCont.GetButton(InputController.Buttons.DASH))
                {
                    iCont.ClearButton(InputController.Buttons.DASH);
                    forceMoveCounter = sideDashDuration;
                    if(velocity.x == 0)
                        forceMoveSpeed = sideDashSpeed * Mathf.Sign(transform.localScale.x);
                    else
                        forceMoveSpeed = sideDashSpeed * Mathf.Sign(velocity.x);
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
                    SoundManager.instance.PlayOneShot("AbilityActivation");
                }
            }
            if (specialDefense != null)
            {
                if (iCont.GetButton(InputController.Buttons.SPECIAL_DEFENSE) && specialDefense.CanFire())
                {
                    specialDefense.AllowFire();
                    SoundManager.instance.PlayOneShot("AbilityActivation");
                }
            }
            if (groundCheck.CheckGrounded() && velocity.y >= 0)
            {
                if (!canJump)
                {
                    if (canDash)
                        SoundManager.instance.PlayOneShot("Landing");
                    else
                        SoundManager.instance.PlayOneShot("GroundPound");
                }
                canDash = true;
                canJump = true;
                canDoubleJump = true;
                waitingForJump = false;
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
                }
                else
                {
                    canDoubleJump = false;
                    velocity.y = doubleJumpVel;
                }
                iCont.ClearButton(InputController.Buttons.JUMP);
                SoundManager.instance.PlayOneShot("Jump");
            }

            if (disabledCounter > 0)
            {
                disabledCounter -= Time.deltaTime;
                return;
            }
            if(aController != null)
            {
                aController.SetBool("bMoving", (velocity.x != 0));
            }

            if(velocity.x != 0)
            {
                if (!playingMove)
                {
                    playingMove = true;
                    SoundManager.instance.PlayBackground("TankMovement");
                }
            }
            else if (playingMove)
            {
                playingMove = false;
                SoundManager.instance.StopBackground("TankMovement");
            }
            if (interactable)
                rBody.velocity = velocity;
            else
                rBody.velocity = Vector2.zero;
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
        else if (other.gameObject.CompareTag("Bullet") && spawnCounter <= 0 && specialDefense.GetPercentage() != 0)
        {
            Die(false, other.GetComponent<IPlayerID>().GetPlayerID());
        }
        else if (other.gameObject.CompareTag("ScreenWrap"))
        {
            forceMoveCounter = wrapMoveDuration;
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
        Manager.instance.kills[player]++;
        Manager.instance.deaths[playerID]++;
        Die(self);
    }

    private void Die(bool self)
    {
        SoundManager.instance.PlayOneShot("Explosion");
        if (playingMove)
        {
            playingMove = false;
            SoundManager.instance.StopBackground("TankMovement");
        }
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
        else
        {
            transform.position = startLocation;
        }
        SetInteractable(true);
        spawnCounter = spawnInvulnurability;
        rBody.velocity = Vector2.zero;
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
    }

    public void IgnoreCollision(Collider2D ignoreCol)
    {
        foreach (Collider2D col in colliders)
            Physics2D.IgnoreCollision(col, ignoreCol);
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
}
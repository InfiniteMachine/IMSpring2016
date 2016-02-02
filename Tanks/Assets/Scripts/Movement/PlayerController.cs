using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float movementSpeed = 4f;
    public float accelDuration = 1f;
    private float counter = 0;

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
    private StayGrounded sGround;
    private IsGrounded groundCheck;
    private InputController iCont;
    private IAction specialAttack;
    private IAction specialDefense;

    //ExtraGravity
    [Header("Extra Gravity")]
    public float downAccelDuration = 1f;
    public float downMaxAccel = 0.5f;
    private float gravityCounter = 0;

    private float forceMoveCounter = 0;
    private float forceMoveSpeed = 0;
    public float forceMoveDuration = 0.5f;

    // Use this for initialization
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        sGround = GetComponent<StayGrounded>();
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
    }

    // Update is called once per frame
    void Update()
    {
        if (respawnCounter > 0)
        {
            respawnCounter -= Time.deltaTime;
            if(respawnCounter <= 0)
            {
                if (Manager.instance != null)
                    transform.position = Manager.instance.GetSpawn();
                else
                    transform.position = startLocation;
                rBody.velocity = Vector2.zero;
            }
        }
        else
        {
            Vector2 velocity = rBody.velocity;
            if (forceMoveCounter > 0)
            {
                velocity.x = forceMoveSpeed;
                forceMoveCounter -= Time.deltaTime;
            }
            else {
                if (iCont.GetAxis(InputController.Axis.MOVE) > 0)
                {
                    if (velocity.x < 0)
                        counter = 0; velocity.x = 0;
                    counter += Time.deltaTime;
                    velocity.x = Mathf.Lerp(0, movementSpeed * iCont.GetAxis(InputController.Axis.MOVE), counter / accelDuration);
                    Vector3 scale = transform.localScale;
                    scale.x = Mathf.Abs(transform.localScale.x);
                    transform.localScale = scale;
                }
                else if (iCont.GetAxis(InputController.Axis.MOVE) < 0)
                {
                    if (velocity.x > 0)
                        counter = 0; velocity.x = 0;
                    counter += Time.deltaTime;
                    velocity.x = Mathf.Lerp(0, movementSpeed * iCont.GetAxis(InputController.Axis.MOVE), counter / accelDuration);
                    Vector3 scale = transform.localScale;
                    scale.x = -Mathf.Abs(transform.localScale.x);
                    transform.localScale = scale;
                }
                else
                {
                    counter = 0;
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
                specialAttack.StartAction();
                iCont.ClearButton(InputController.Buttons.SPECIAL_FIRE);
            }
            if (iCont.GetButton(InputController.Buttons.SPECIAL_DEFENSE) && specialDefense.CanFire())
            {
                specialDefense.StartAction();
                iCont.ClearButton(InputController.Buttons.SPECIAL_DEFENSE);
            }
            if (groundCheck.CheckGrounded() && iCont.GetButton(InputController.Buttons.JUMP))
            {
                sGround.Ignore(0.1f);
                velocity.y = jumpVel;
                iCont.ClearButton(InputController.Buttons.JUMP);
            }
            rBody.velocity = velocity;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 4);
            if (hit)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);
                if (Mathf.Abs(angle) > ignoreRange)
                    angle = 0;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (respawnCounter > 0)
            return;
        if (other.gameObject.CompareTag("Crown"))
        {
            other.enabled = false;
            hasCrown = true;
            //HEY I HAVE A BATON??
        }
        else if (other.gameObject.CompareTag("KillBox"))
        {
            respawnCounter = respawnTime;
            Manager.instance.ResetBaton();
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

	public void Die()
	{
		// Drop baton
		Manager.instance.TakeBaton(playerID);
		// Need something here like put-crown-here

		transform.position = Manager.instance.GetSpawn();
	}

    void LateUpdate()
    {
        if (hasCrown)
        {
            crown.position = transform.position + crownLocation;
        }
    }
}

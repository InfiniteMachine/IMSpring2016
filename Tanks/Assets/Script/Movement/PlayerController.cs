using UnityEngine;

public class PlayerController : MonoBehaviour {
    [System.Serializable]
    public class KeyboardInput
    {
        public KeyCode left;
        public KeyCode right;
        public KeyCode jump;
        public KeyCode specialAttack;
        public KeyCode specialDefense;
    }
    public KeyboardInput keys;
    public float movementSpeed = 4f;
    public float accelDuration = 1f;
    private float counter = 0;
    private Transform crown;
    private bool hasCrown = false;
    public Vector3 crownLocation = new Vector3(0, 0, -0.1f);
    private Rigidbody2D rBody;
    private StayGrounded sGround;
    public IsGrounded groundCheck;
    public float jumpVel = 5f;
    public float ignoreRange = 15f;
    private IAction specialAttack;
    private IAction specialDefense;

    private InputController iCont;

    [HideInInspector]
    public int playerID;
    [HideInInspector]
    public int controllerNumber;

    // Use this for initialization
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        sGround = GetComponent<StayGrounded>();
        GameObject go = GameObject.FindGameObjectWithTag("Crown");
        if (go != null)
            crown = go.transform;
        IAction[] attachedActions = GetComponents<IAction>();
        for(int i = 0; i < attachedActions.Length; i++)
        {
            if (attachedActions[i].IsAttack())
                specialAttack = attachedActions[i];
            else
                specialDefense = attachedActions[i];
        }
        iCont = GetComponent<InputController>();
    }

    // Update is called once per frame
    void Update () {
        Vector2 velocity = rBody.velocity;
        if(iCont.GetAxis(InputController.Axis.MOVE) > 0)
        {
            if (velocity.x < 0)
                counter = 0; velocity.x = 0;
            counter += Time.deltaTime;
            velocity.x = Mathf.Lerp(0, movementSpeed * iCont.GetAxis(InputController.Axis.MOVE), counter / accelDuration);
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(transform.localScale.x);
            transform.localScale = scale;
        }
        else if(iCont.GetAxis(InputController.Axis.MOVE) < 0)
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
        if(groundCheck.CheckGrounded() && iCont.GetButton(InputController.Buttons.JUMP))
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Crown"))
        {
            other.enabled = false;
            hasCrown = true;
            //HEY I HAVE A BATON??
        }
        else if (other.gameObject.CompareTag("KillBox"))
        {
            Debug.Log(transform.position);
            transform.position = Manager.instance.GetSpawn();
            Debug.Log(transform.position);
        }
    }

    void LateUpdate()
    {
        if (hasCrown)
        {
            crown.position = transform.position + crownLocation;
        }
    }
}
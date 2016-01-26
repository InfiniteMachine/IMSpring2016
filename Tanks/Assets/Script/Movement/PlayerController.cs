using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    [System.Serializable]
    public class KeyboardInput
    {
        public KeyCode left;
        public KeyCode right;
        public KeyCode jump;
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

    // Use this for initialization
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        sGround = GetComponent<StayGrounded>();
        crown = GameObject.FindGameObjectWithTag("Crown").transform;
    }

    // Update is called once per frame
    void Update () {
        Vector2 velocity = rBody.velocity;
        int input = (Input.GetKey(keys.right) ? 1 : 0) - (Input.GetKey(keys.left) ? 1 : 0);
        if(input > 0)
        {
            if (velocity.x < 0)
                counter = 0; velocity.x = 0;
            counter += Time.deltaTime;
            velocity.x = Mathf.Lerp(0, movementSpeed, counter / accelDuration);
        }else if(input < 0)
        {
            if (velocity.x > 0)
                counter = 0; velocity.x = 0;
            counter += Time.deltaTime;
            velocity.x = Mathf.Lerp(0, -movementSpeed, counter / accelDuration);
        }
        else
        {
            counter = 0;
            velocity.x = Mathf.MoveTowards(velocity.x, 0, movementSpeed / accelDuration * Time.deltaTime);
        }

        if(groundCheck.CheckGrounded() && Input.GetKeyDown(keys.jump))
        {
            sGround.Ignore(0.1f);
            velocity.y = jumpVel;
        }

        rBody.velocity = velocity;

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, Vector2.down, 4);
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
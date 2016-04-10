using UnityEngine;
using System.Collections;

public class TankBullet : MonoBehaviour, IPlayerID {

	static public float g = 9.81f;

	public class BulletType {
		public TankBullet script;
		public BulletType(){}
		virtual public void Init() {}
		virtual public void Update(float deltaTime) {}
		virtual public void BlockCollision(GameObject block) {}
		virtual public void TankCollision(GameObject tank) {}
	}

	public class ANewBulletType : BulletType {
		public ANewBulletType() {}
		// This is how you add specific bullets.
		// Not related to "charge levels" because the tankgun script
		// is changed based on the prefab it is attached to.
	}

	public class BasicBullet : BulletType {

		public BasicBullet() {}

		override public void Init() {
			script.velocity = script.power * 1f + 8f;
			script.weight = 1f;
		}

		override public void Update(float deltaTime) {
			script.myRigidbody.velocity += new Vector2(0f, -g*script.weight*deltaTime);
            if(script.myRigidbody.velocity.y > 0)
                script.transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(script.myRigidbody.velocity.normalized, Vector2.right));
            else
                script.transform.rotation = Quaternion.Euler(0, 0, -Vector2.Angle(script.myRigidbody.velocity.normalized, Vector2.right));
        }

		override public void BlockCollision(GameObject block) {
			Destroy(script.gameObject);
		}

		override public void TankCollision(GameObject tank) {
			Destroy(script.gameObject);
		}
	}

	//BulletType myBulletType;
	BulletType myBulletType;

	void GetBulletType(int newType)
	{
		if(newType==-1) // Example
			myBulletType = new ANewBulletType();
		else if(newType>=0)
			myBulletType = new BasicBullet();
		myBulletType.script = this;
	}


	public int intType = 0;
	public Rigidbody2D myRigidbody;
	public float power;
	public float weight = 0f;
	public float velocity = 0f;
	public float strength = 0f;
    private int playerID;
	// Use this for initialization
	void Awake () {
		myRigidbody = GetComponent<Rigidbody2D>();
	}

	void Update() {
		velocity = myRigidbody.velocity.magnitude;
	}

	void FixedUpdate() {
		myBulletType.Update(Time.fixedDeltaTime);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag=="Ground")
		{
			myBulletType.BlockCollision(other.gameObject);
		}
        else if(other.gameObject.tag == "Player" && !other.gameObject.GetComponent<PlayerController>().IsShield())
        {
            myBulletType.BlockCollision(other.gameObject);
        }
	}

	public void ApplyVel()
	{
		myRigidbody.velocity = myRigidbody.velocity.normalized * velocity;
	}

	public void SetPower(float newPower, float newAngle, int type = 0)
	{
		power = newPower;
		//myBulletType = bulletTypes[type];
		GetBulletType(type);
		myBulletType.Init();
		transform.eulerAngles = new Vector3(0f,0f,newAngle);
		// Not using ApplyVel here because the velocity is 0 and .normalized wouldnt work.
		myRigidbody.velocity = transform.right * velocity;
	}

    public int GetPlayerID()
    {
        return playerID;
    }

    public void SetPlayerID(int id)
    {
        playerID = id;
    }
}
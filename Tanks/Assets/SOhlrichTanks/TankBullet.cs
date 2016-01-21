using UnityEngine;
using System.Collections;

public class TankBullet : MonoBehaviour {

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
			script.myRigidbody.velocity += new Vector3(0f, -TankBullet.g*script.weight*deltaTime, 0f);
		}

		override public void BlockCollision(GameObject block) {
			GameObject.Destroy(script.gameObject);
		}

		override public void TankCollision(GameObject tank) {
			GameObject.Destroy(script.gameObject);
		}
	}

	//BulletType myBulletType;
	BulletType myBulletType;

	void GetBulletType(int newType)
	{
		if(newType==-1) // Example
			myBulletType = (BulletType) new ANewBulletType();
		else if(newType>=0)
			myBulletType = (BulletType) new BasicBullet();
		myBulletType.script = this;
	}


	public int intType = 0;
	public Rigidbody myRigidbody;
	public float power;
	public float weight = 0f;
	public float velocity = 0f;
	public float strength = 0f;

	// Use this for initialization
	void Awake () {
		myRigidbody = GetComponent<Rigidbody>();
	}

	void Update() {
		velocity = myRigidbody.velocity.magnitude;
	}

	void FixedUpdate() {
		myBulletType.Update(Time.fixedDeltaTime);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag=="Block")
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
}
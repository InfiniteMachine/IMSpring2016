using UnityEngine;
using System.Collections;

public class TankGun : MonoBehaviour {

	public GameObject bullet;

	[System.Serializable]
	public class Charge {
		public float time;
		public float bonusPower;
		public enum Behaviour{Exponential, Flat, Step};
		public Behaviour behaviour;
		public float exponent = 1f;
		[HideInInspector]
		public float chargedValue = 0f;
		public Charge(){}
	}

	public float barrelLength = 4f;
	public float angularVelocity = 25f;
	public float maxAngleUp = 70f;
	public float maxAngleDown = 0f;
	public int myBulletType = 1;
	public float reloadTimeAfterFiring = 0.4f;
	public float basePower = 10f;
	public Charge[] chargeLevels;

	public float currentReloadTime = 0f;
	public float currentPower;
	public float currentTime = 0f;
	public int currentLevel = 0;
	public bool charging = false;

	public KeyCode chargeButton = KeyCode.Space;
	public KeyCode upButton = KeyCode.W;
	public KeyCode downButton = KeyCode.S;

    bool faceRight = true;

    // Use this for initialization
    void Awake()
    {
        InitChargeStats();
        ResetPower();
    }

	void InitChargeStats()
	{
		float powerLevel = 0f;
		for(int x=0;x<chargeLevels.Length;x++)
		{
			chargeLevels[x].chargedValue = powerLevel;
			powerLevel += chargeLevels[x].bonusPower;
		}
	}
	
	// Update is called once per frame
	void Update () {

		RotateBarrel();

		if(charging)
		{
			currentReloadTime = 0f; // Just to make sure this never mistakenly reloads while charging.
			bool mustFire = AddCharge(Time.deltaTime);
			if(mustFire || !Input.GetKey(chargeButton))
			{
				Fire();
			}
		}
		else
		{
			currentReloadTime += Time.deltaTime;
			if(Input.GetKey(chargeButton) && currentReloadTime>reloadTimeAfterFiring)
			{
				if(chargeLevels.Length>0)
					charging = true;
				else Fire();
			}
		}

	}

	bool AddCharge(float deltaTime)
	{
		currentTime += deltaTime;
		if(currentTime>chargeLevels[currentLevel].time)
		{ // Completed this charge level
			currentPower = chargeLevels[currentLevel].bonusPower + chargeLevels[currentLevel].chargedValue + basePower;

			if(chargeLevels.Length>currentLevel+1)
			{ // Theres more levels to charge
				float extraTime = currentTime - chargeLevels[currentLevel].time;
				currentLevel += 1;
				return AddCharge(extraTime);
			}
			else
			{ // No more levels to charge, must fire
				return true;
			}
		}
		else
		{
			currentPower = chargeLevels[currentLevel].chargedValue + basePower;
			if(chargeLevels[currentLevel].behaviour==Charge.Behaviour.Step)
			{
				// Add nothing because the step function would go to 0
			}
			else if(chargeLevels[currentLevel].behaviour==Charge.Behaviour.Exponential)
			{
				currentPower += chargeLevels[currentLevel].bonusPower * Mathf.Pow((currentTime/chargeLevels[currentLevel].time), chargeLevels[currentLevel].exponent);
			}
			else if(chargeLevels[currentLevel].behaviour==Charge.Behaviour.Flat)
			{
				currentPower += chargeLevels[currentLevel].bonusPower * (currentTime/chargeLevels[currentLevel].time);
			}

			return false;
		}
	}

	void RotateBarrel() {
		if(Input.GetKey(upButton))
			transform.eulerAngles += new Vector3(0f, 0f, Time.deltaTime * angularVelocity);
		if(Input.GetKey(downButton))
			transform.eulerAngles += new Vector3(0f, 0f, -Time.deltaTime * angularVelocity);

		CheckBarrelBounds();
	}

	void CheckBarrelBounds() {
		if(transform.eulerAngles.z>maxAngleUp)
			transform.eulerAngles += new Vector3(0f,0f, -(transform.eulerAngles.z-maxAngleUp));
		if(transform.eulerAngles.z<maxAngleDown)
			transform.eulerAngles += new Vector3(0f,0f, -(transform.eulerAngles.z+maxAngleDown));
	}

	void Fire()	{
        if (transform.lossyScale.x > 0f)
            faceRight = true;
        else faceRight = false;
        Vector3 newPos = transform.position;
        if (faceRight)
            newPos += (transform.right * barrelLength);
        else
        {
            newPos.x += (transform.right.x * -barrelLength);
            newPos.y += (transform.right.y * barrelLength);
            newPos.z += (transform.right.z * barrelLength);
        }
        GameObject newBullet = (GameObject)Instantiate(bullet, newPos, Quaternion.identity);
		Destroy(newBullet, 15f);
        float angle = transform.eulerAngles.z;
        if (!faceRight)
        {
            angle = (90f - angle) + 90f;
        }
		newBullet.GetComponent<TankBullet>().SetPower(currentPower, angle, myBulletType);
		currentReloadTime = 0f;
		ResetPower();
	}

	void ResetPower()
	{
		currentPower = basePower;
		currentLevel = 0;
		currentTime = 0f;
		charging = false;
	}
}
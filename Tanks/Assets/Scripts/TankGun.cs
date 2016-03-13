using UnityEngine;
using System.Collections;

public class TankGun : MonoBehaviour {

	public GameObject bullet;
    public GameObject particles;
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

    private InputController iCont;
    private Transform firePosition;
    private Collider2D parentCollider;
    public int playerID;
    // Use this for initialization
    void Awake()
    {
        InitChargeStats();
        ResetPower();
        iCont = transform.parent.GetComponent<InputController>();
        firePosition = transform.FindChild("FirePosition");
        parentCollider = transform.parent.GetComponent<Collider2D>();
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
        if (!Manager.instance.activeMatch)
            return;
		RotateBarrel();

		if(charging)
		{
			currentReloadTime = 0f; // Just to make sure this never mistakenly reloads while charging.
			bool mustFire = AddCharge(Time.deltaTime);
			if(mustFire || iCont.GetButton(InputController.Buttons.FIRE))
			{
				Fire();
			}
		}
		else
		{
			currentReloadTime += Time.deltaTime;
			if(iCont.GetButton(InputController.Buttons.FIRE) && currentReloadTime>reloadTimeAfterFiring)
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

    void RotateBarrel()
    {
        Vector3 scale = transform.parent.localScale;
        if(iCont.GetAxis(InputController.Axis.AIM_Y) != 0 || iCont.GetAxis(InputController.Axis.AIM_X) != 0){
            if (iCont.GetAxis(InputController.Axis.AIM_ANGLE) > 90)
            {
                transform.eulerAngles = new Vector3(0, 0, Mathf.Clamp(180 - iCont.GetAxis(InputController.Axis.AIM_ANGLE), maxAngleDown, maxAngleUp));
                scale.x = -Mathf.Abs(scale.x);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, Mathf.Clamp(iCont.GetAxis(InputController.Axis.AIM_ANGLE), maxAngleDown, maxAngleUp));
                scale.x = Mathf.Abs(scale.x);
            }
            transform.parent.localScale = scale;
        }
    }

	void Fire()	{
        SoundManager.instance.PlayOneShot("Shoot");
        GameObject go = (GameObject)Instantiate(particles, firePosition.position, Quaternion.identity);
        go.transform.SetParent(firePosition);
        GameObject newBullet = (GameObject)Instantiate(bullet, firePosition.position, Quaternion.identity);
        Physics2D.IgnoreCollision(newBullet.GetComponent<Collider2D>(), parentCollider);
		Destroy(newBullet, 15f);
        newBullet.GetComponent<TankBullet>().SetPower(currentPower, Vector2.Angle(Vector2.right, (firePosition.position - transform.position)), myBulletType);
        newBullet.GetComponent<TankBullet>().playerID = playerID;
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
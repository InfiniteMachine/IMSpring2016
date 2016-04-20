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

    private InputController iCont;
    private Transform firePosition;
    public int playerID;

    private PlayerController pCont;
    // Use this for initialization
    void Awake()
    {
        InitChargeStats();
        ResetPower();
        iCont = transform.parent.GetComponent<InputController>();
        firePosition = transform.FindChild("FirePosition");
        pCont = transform.parent.GetComponent<PlayerController>();
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
        }
        else
        {
            if (iCont.GetAxis(InputController.Axis.MOVE) != 0)
                scale.x = Mathf.Sign(iCont.GetAxis(InputController.Axis.MOVE));
        }
        transform.parent.localScale = scale;
    }

    public void Fire(bool sfx = true)	{
        if(sfx)
            SoundManager.instance.PlayOneShot("basic_attack");
        //go.transform.SetParent(firePosition);
        GameObject newBullet = (GameObject)Instantiate(bullet, firePosition.position, Quaternion.identity);
        pCont.IgnoreCollision(newBullet.GetComponent<Collider2D>());
        newBullet.GetComponent<TankBullet>().SetPower(currentPower, Vector2.Angle(Vector2.right, (pCont.transform.localScale.x < 0 ? Quaternion.Euler(0, 0, 180) : Quaternion.identity) * firePosition.right), myBulletType);
        newBullet.GetComponent<TankBullet>().SetPlayerID(playerID);
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
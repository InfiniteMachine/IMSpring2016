using UnityEngine;
using System.Collections;

public class RandomCharacterSpawner : MonoBehaviour {
    public float[] yLocations = { -0.5f, -0.35f, -0.2f, -0.05f };
    public float xLeft = -1.1f;
    public float xRight = 1.1f;
    public float maxOffset = 1f;

    public Vector2 spawnRange = new Vector2(1f, 5f);
    public float speed = 0.1f;
    private GameObject leftGO, rightGO;
    private Animator leftAn, rightAn;
    private bool update = true;
	// Use this for initialization
	void Start () {
        leftGO = transform.FindChild("Left").gameObject;
        leftAn = leftGO.GetComponent<Animator>();
        
        rightGO = transform.FindChild("Right").gameObject;
        rightAn = rightGO.GetComponent<Animator>();

        NewAnimation();
    }
	
	// Update is called once per frame
	void Update () {
        if (update)
        {
            leftGO.transform.Translate(Vector2.right * speed * Time.deltaTime);
            rightGO.transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (rightGO.transform.position.x < xLeft && leftGO.transform.position.x > xRight)
            {
                update = false;
                Invoke("ResetUpdate", Random.Range(spawnRange.x, spawnRange.y));
            }
        }
    }

    public void NewAnimation()
    {
        int leftLoc = Random.Range(0, yLocations.Length);
        int rightLoc = leftLoc;
        while (rightLoc == leftLoc)
        {
            rightLoc = Random.Range(0, yLocations.Length);
        }

        int leftCharacter = Random.Range(0, 8);
        int rightCharacter = leftCharacter;
        while (rightCharacter == leftCharacter)
        {
            rightCharacter = Random.Range(0, 8);
        }

        if(rightLoc + 1 == leftLoc)
            leftGO.transform.position = new Vector3(xLeft - Random.Range(0, maxOffset), yLocations[leftLoc] + 0.05f, 0);
        else
            leftGO.transform.position = new Vector3(xLeft - Random.Range(0, maxOffset), yLocations[leftLoc], 0);

        if (leftLoc + 1 == rightLoc)
            rightGO.transform.position = new Vector3(xRight + Random.Range(0, maxOffset), yLocations[rightLoc] + 0.05f, 0);
        else
            rightGO.transform.position = new Vector3(xRight + Random.Range(0, maxOffset), yLocations[rightLoc], 0);

        leftAn.SetInteger("NewAnim", leftCharacter);
        leftAn.SetTrigger("StateChange");
        rightAn.SetInteger("NewAnim", rightCharacter);
        rightAn.SetTrigger("StateChange");
    }

    private void ResetUpdate()
    {
        NewAnimation();
        update = true;
    }
}

using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    private Controller c;

	// Use this for initialization
	void Start () {
        c = ControllerPool.GetInstance().GetController(1);
	}
	
	// Update is called once per frame
	void Update () {
        string s = "";
        for (int i = 0; i < 10; i++)
        {
            s += "B" + i + ": " + c.GetButton(i) + "; ";
        }

        for (int i = 0; i < 10; i++)
        {
            s += "A" + i + ": " + c.GetAxis(i) + "; ";
        }
        Debug.Log(s);
    }
}

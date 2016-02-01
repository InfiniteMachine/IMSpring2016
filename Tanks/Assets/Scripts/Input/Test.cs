using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        Controller c = ControllerPool.GetInstance().GetController(1);
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

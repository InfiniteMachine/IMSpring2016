using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour {
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.yellow;
    private enum Buttons { Play = 0, Quit};
    private int numButtons = 2;
    private Buttons selected = Buttons.Play;
    private Image[] buttons;
	// Use this for initialization
	void Start () {
        buttons = new Image[numButtons];
        for(int i = 0; i < numButtons; i++)
        {
            GameObject go = transform.FindChild(((Buttons)i).ToString()).GetChild(0).gameObject;
            if(go != null)
            {
                Image t = go.GetComponent<Image>();
                if(t != null)
                {
                    buttons[i] = t;
                }
                else
                {
                    Debug.LogError("There is an image component missing from " + go.name);
                }
            }
            else
            {
                Debug.LogError("There is an image missing from the menu.");
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        Controller[] controllers = ControllerPool.GetInstance().GetControllers();
        bool broken = false;
        for (int i = 0; i < controllers.Length; i++)
        {
            broken= true;
            if (controllers[i].GetAxisAsButton(0, true) || controllers[i].GetAxisAsButton(5, false))
            {
                //Down
                selected++;
                if (selected > Buttons.Quit)
                    selected = Buttons.Play;
                SoundManager.instance.PlayOneShot("Swap");
                break;
            }
            else if (controllers[i].GetAxisAsButton(0, false) || controllers[i].GetAxisAsButton(5, true))
            {
                //Up
                selected--;
                if (selected < Buttons.Play)
                    selected = Buttons.Quit;
                SoundManager.instance.PlayOneShot("Swap");
                break;
            }
            else if (controllers[i].GetButtonDown(0))
            {
                //Submit
                PerformAction();
                SoundManager.instance.PlayOneShot("Select");
                break;
            }
            broken = false;
        }
        if (!broken)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                //Down
                selected++;
                if (selected > Buttons.Quit)
                    selected = Buttons.Play;
                SoundManager.instance.PlayOneShot("Swap");
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //Up
                selected--;
                if (selected < Buttons.Play)
                    selected = Buttons.Quit;
                SoundManager.instance.PlayOneShot("Swap");
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                PerformAction();
                SoundManager.instance.PlayOneShot("Select");
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selected = Buttons.Quit;
            PerformAction();
        }
        UpdateColors();
	}

    private void PerformAction()
    {
        switch (selected)
        {
            case Buttons.Play:
                SceneManager.LoadScene("Player_Select_Screen");
                break;
            case Buttons.Quit:
                Application.Quit();
                Debug.Log("Would close if not in the editor");
                break;
        }
    }

    private void UpdateColors()
    {
        for(int i = 0; i < numButtons; i++)
        {
            buttons[i].color = ((int)selected == i) ? selectedColor : defaultColor;
        }
    }
}
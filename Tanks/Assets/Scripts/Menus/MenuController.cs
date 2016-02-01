using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour {
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.yellow;
    private enum Buttons { Play = 0, Options, Controls, Exit};
    private int numButtons = 4;
    private Buttons selected = Buttons.Play;
    private Text[] buttons;
	// Use this for initialization
	void Start () {
        buttons = new Text[numButtons];
        for(int i = 0; i < numButtons; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            if(go != null)
            {
                Text t = go.GetComponent<Text>();
                if(t != null)
                {
                    buttons[i] = t;
                }
                else
                {
                    Debug.LogError("There is a text component missing from " + go.name);
                }
            }
            else
            {
                Debug.LogError("There is a button missing from the menu.");
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
            if (controllers[i].GetAxisAsButton(1, true) || controllers[i].GetAxisAsButton(6, false))
            {
                //Down
                selected++;
                if (selected > Buttons.Exit)
                    selected = Buttons.Play;
                break;
            }
            else if (controllers[i].GetAxisAsButton(1, false) || controllers[i].GetAxisAsButton(6, true))
            {
                //Up
                selected--;
                if (selected < Buttons.Play)
                    selected = Buttons.Exit;
                break;
            }
            else if (controllers[i].GetButtonDown(0))
            {
                //Submit
                PerformAction();
                break;
            }
            broken = false;
        }
        if (!broken)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                //Down
                selected++;
                if (selected > Buttons.Exit)
                    selected = Buttons.Play;
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                //Up
                selected--;
                if (selected < Buttons.Play)
                    selected = Buttons.Exit;
            }
            else if (Input.GetKeyDown(KeyCode.Return))
                PerformAction();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selected = Buttons.Exit;
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
            case Buttons.Options:
                SceneManager.LoadScene("Options");
                break;
            case Buttons.Controls:
                SceneManager.LoadScene("Controls");
                break;
            case Buttons.Exit:
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
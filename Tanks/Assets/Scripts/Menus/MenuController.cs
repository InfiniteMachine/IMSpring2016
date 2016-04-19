using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour {
    private enum Buttons { Play = 0, Controls, Quit};
    private int numButtons = 3;
    private Buttons selected = Buttons.Play;
    private Image[] buttons;

    private enum State { Menu = 0, Controls1, Controls2 };
    private State s = State.Menu;
    private CanvasGroup controls1, controls2;
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
        controls1 = transform.parent.FindChild("Controls1").GetComponent<CanvasGroup>();
        controls1.alpha = 0;
        controls1.interactable = false;
        controls1.blocksRaycasts = false;
        controls2 = transform.parent.FindChild("Controls2").GetComponent<CanvasGroup>();
        controls2.alpha = 0;
        controls2.interactable = false;
        controls2.blocksRaycasts = false;

        SoundManager.instance.PlayBackground("menuMusic");
    }
	
	// Update is called once per frame
	void Update () {
        Controller[] controllers = ControllerPool.GetInstance().GetControllers();
        if (s == State.Menu)
        {
            bool broken = false;
            for (int i = 0; i < controllers.Length; i++)
            {
                broken = true;
                if (controllers[i].GetAxisAsButton(1, true) || controllers[i].GetAxisAsButton(6, false))
                {
                    //Down
                    selected++;
                    if (selected > Buttons.Quit)
                        selected = Buttons.Play;
                    SoundManager.instance.PlayOneShot("menu_move");
                    break;
                }
                else if (controllers[i].GetAxisAsButton(1, false) || controllers[i].GetAxisAsButton(6, true))
                {
                    //Up
                    selected--;
                    if (selected < Buttons.Play)
                        selected = Buttons.Quit;
                    SoundManager.instance.PlayOneShot("menu_move");
                    break;
                }
                else if (controllers[i].GetButtonDown(0))
                {
                    //Submit
                    PerformAction();
                    SoundManager.instance.PlayOneShot("menu_select");
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
                    if (selected > Buttons.Quit)
                        selected = Buttons.Play;
                    SoundManager.instance.PlayOneShot("menu_move");
                }
                else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    //Up
                    selected--;
                    if (selected < Buttons.Play)
                        selected = Buttons.Quit;
                    SoundManager.instance.PlayOneShot("menu_move");
                }
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    PerformAction();
                    SoundManager.instance.PlayOneShot("menu_select");
                }
            }
            UpdateColors();
        }
        else
        {
            for(int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i].GetButtonDown(0))
                {
                    //Submit
                    if (s == State.Controls2)
                    {
                        s = State.Menu;
                        controls2.alpha = 0;
                        controls2.interactable = false;
                        controls2.blocksRaycasts = false;
                    }
                    else
                    {
                        s = State.Controls2;
                        controls2.alpha = 1;
                        controls2.interactable = true;
                        controls2.blocksRaycasts = true;
                        controls1.alpha = 0;
                        controls1.interactable = false;
                        controls1.blocksRaycasts = false;
                    }
                    SoundManager.instance.PlayOneShot("menu_select");
                }
                else if (controllers[i].GetButtonDown(1))
                {
                    //Cancel
                    if (s == State.Controls1)
                    {
                        s = State.Menu;
                        controls1.alpha = 0;
                        controls1.interactable = false;
                        controls1.blocksRaycasts = false;
                    }
                    else
                    {
                        s = State.Controls1;
                        controls1.alpha = 1;
                        controls1.interactable = true;
                        controls1.blocksRaycasts = true;
                        controls2.alpha = 0;
                        controls2.interactable = false;
                        controls2.blocksRaycasts = false;
                    }
                    SoundManager.instance.PlayOneShot("menu_back");
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selected = Buttons.Quit;
            PerformAction();
        }
    }

    private void PerformAction()
    {
        switch (selected)
        {
            case Buttons.Play:
                SceneManager.LoadScene("Player_Select_Screen");
                break;
            case Buttons.Controls:
                s = State.Controls1;
                controls1.alpha = 1;
                controls1.interactable = true;
                controls1.blocksRaycasts = true;
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
            buttons[i].enabled = ((int)selected == i);
        }
    }
}
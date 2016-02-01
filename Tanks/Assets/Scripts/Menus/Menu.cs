using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void PerformAction(string action)
    {
        //controls
        if (action=="Controls"){
        SceneManager.LoadScene("Controls");
        }


        //play
        if (action == "Play")
        {
            SceneManager.LoadScene("Player_Select_Screen");
        }


        //options
        if (action == "Options")
        {
            SceneManager.LoadScene("Options");
        }


        //instructions
        if (action == "Instructions")
        {
            SceneManager.LoadScene("Instructions");
        }

        if (action == "Return")
        {
            SceneManager.LoadScene("Menu");
        }
    }
}

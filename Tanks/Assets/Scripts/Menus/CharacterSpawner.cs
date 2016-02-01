using UnityEngine;
using System.Collections;

public class CharacterSpawner : MonoBehaviour
{
    private GameObject Tank01Prefab;
    private GameObject Tank02Prefab;
    private GameObject Tank03Prefab;
    private GameObject Tank04Prefab;

    //This is where the script placed in the level inputs in this number for the player who was selected and saved by playerPrefs
    private int savedPlayer = 0;

    //this is called first before the Start function, so make sure it loads everything needed first.
    void Awake()
    {
        savedPlayer = PlayerPrefs.GetInt("selectedPlayer");
        Tank01Prefab = GameObject.Find("Player1");
        Tank02Prefab = GameObject.Find("Player2");
        Tank03Prefab = GameObject.Find("Player3");
        Tank04Prefab = GameObject.Find("Player4");
        if (savedPlayer == 0)
        {
            Tank01Prefab.SetActive(true);
            Tank02Prefab.SetActive(false);
            Tank03Prefab.SetActive(false);
            Tank04Prefab.SetActive(false);
        }
        else if (savedPlayer == 1)
        {
            Tank01Prefab.SetActive(true);
            Tank02Prefab.SetActive(false);
            Tank03Prefab.SetActive(false);
            Tank04Prefab.SetActive(false);
        }
        else if (savedPlayer == 2)
        {
            Tank01Prefab.SetActive(false);
            Tank02Prefab.SetActive(true);
            Tank03Prefab.SetActive(false);
            Tank04Prefab.SetActive(false);
        }
        else if (savedPlayer == 3)
        {
            Tank01Prefab.SetActive(false);
            Tank02Prefab.SetActive(false);
            Tank03Prefab.SetActive(true);
            Tank04Prefab.SetActive(false);
        }
        else if (savedPlayer == 4)
        {
            Tank01Prefab.SetActive(false);
            Tank02Prefab.SetActive(false);
            Tank03Prefab.SetActive(false);
            Tank04Prefab.SetActive(true);
        }
    }
}
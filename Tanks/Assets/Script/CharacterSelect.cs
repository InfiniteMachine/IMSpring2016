using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour {
    public string[] areaNames;
    private int selectedPlayer = 0;
    private int player = 1;
    public Text display;
    private GameObject playerSelect;
    private GameObject sceneSelect;
    // Update is called once per frame
    void Start()
    {
        playerSelect = transform.FindChild("PlayerSelect").gameObject;
        sceneSelect = transform.FindChild("SceneSelect").gameObject;
        sceneSelect.SetActive(false);
        //Replace with control for selecting players
        Manager.instance.playerTanks = new int[4];
    }

    public void SelectedCharacter(int character)
    {
        selectedPlayer = character;
        if (player == 1)
        {
            Manager.instance.playerTanks[0] = character;
            player = 2; 
            display.text = "Player Select";
        }
        else if (player == 2)
        {
            Manager.instance.playerTanks[0] = character;
            sceneSelect.SetActive(true);
            playerSelect.SetActive(false);
        }
    }

    public void SelectedArena(int Area)
    {
        if (Area <= areaNames.Length && Area > 0)
        {
            SceneManager.LoadScene(areaNames[Area - 1]);
            Manager.instance.InitOnNextScene();
        }
    }
}
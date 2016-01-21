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
    }

    void Update()
    {
        /*
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.collider.name == "Tank01")
                SelectedCharacter(player, 1);
            if (hit.collider.name == "Tank02")
                SelectedCharacter(player, 2);
            if (hit.collider.name == "Tank03")
                SelectedCharacter(player, 3);
            if (hit.collider.name == "Tank04")
                SelectedCharacter(player, 4);
        }
        */
    }

    public void SelectedCharacter(int character)
    {
        selectedPlayer = character;
        if (player == 1)
        {
            PlayerPrefs.SetInt("selectedPlayer1", selectedPlayer);
            player = 2;
            display.text = "Player 2 Select";
        }
        else if (player == 2)
        {
            PlayerPrefs.SetInt("selectedPlayer2", selectedPlayer);
            sceneSelect.SetActive(true);
            playerSelect.SetActive(false);
        }
        
    }

    public void SelectedArena(int Area)
    {
        if (Area <= areaNames.Length && Area > 0)
            SceneManager.LoadScene(areaNames[Area - 1]);
    }
}
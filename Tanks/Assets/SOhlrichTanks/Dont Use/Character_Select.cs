using UnityEngine;
using System.Collections;

public class Character_Select : MonoBehaviour {

	//this is the currently selected player.  Also the one that will be saved to PlayerPrefs
	//int selectedPlayer = 0;

	void Update()
	{
		if (Input.GetMouseButtonUp (0) ) 
		{
			// Wouldnt work if we were using roullete select
			Ray ray = Camera.main.ScreenPointToRay  (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast   (ray, out hit, 100) )
			{
				// Formated incorrectly (I removed the extra left parentheses).
				/*if(hit.collider.name == "Tank01")
					SelectedCharacter1();

				if(hit.collider.name == "Tank02")
					SelectedCharacter2();

					if(hit.collider.name == "Tank03")
						SelectedCharacter3();

						if(hit.collider.name == "Tank04")
							SelectedCharacter4();*/

			}
			else 
			{
				return;
			}
		}
	}
		
	// Couldve just used one function with an int parameter
							/*
function SelectedCharacter1 () 
{
       //Debug.Log ("Character 1 SELECTED");
          selectedPlayer = 1;
          PlayerPrefs.Set Int ("selectedPlayer", (selectedPlayer));
}

function SelectedCharacter2 () 
{
       //Debug.Log ("Character 2 SELECTED");
          selectedPlayer = 2;
          PlayerPrefs.Set Int ("selectedPlayer", (selectedPlayer));
}

function SelectedCharacter3 () 
{
       //Debug.Log ("Character 3 SELECTED");
          selectedPlayer = 3;
          PlayerPrefs.Set Int ("selectedPlayer", (selectedPlayer));
}

function SelectedCharacter4 () 
{
       //Debug.Log ("Character 4 SELECTED");
          selectedPlayer = 4;
          PlayerPrefs.Set Int ("selectedPlayer", (selectedPlayer));
}

*/
}

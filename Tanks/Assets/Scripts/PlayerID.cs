using UnityEngine;
using System.Collections;

public class PlayerID : MonoBehaviour, IPlayerID {
    private int playerID = -1;
    public int GetPlayerID()
    {
        return playerID;
    }

    public void SetPlayerID(int newID)
    {
        playerID = newID;
    }
}

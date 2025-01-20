using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    public class OnClickPositionEventArgs : EventArgs {
        public int posX, posY;
        public PlayerType playerType;
    }
    public event EventHandler<OnClickPositionEventArgs> OnClickPosition;

    public enum PlayerType {
        cross,
        circle
    }

    private PlayerType playerType;
    private PlayerType playerTurn;
    private void Awake() {
        if(instance == null)
            instance = this;
        else {
            Debug.Log("There are more than one Gamemanger");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }

    public override void OnNetworkSpawn() {
        playerType = (NetworkManager.Singleton.LocalClientId == 0)? PlayerType.cross : PlayerType.circle;
        Debug.Log("ClientId: " + NetworkManager.Singleton.LocalClientId + ", PlayerType: " + this.playerType);
    }

    [Rpc(SendTo.Server)]
    public void HandlerClickPositionRpc(int x, int y, PlayerType playerType) {
        if (playerType != playerTurn)
            return;
        Debug.Log("Click from: " + NetworkManager.Singleton.LocalClientId);
        OnClickPosition?.Invoke(this, new OnClickPositionEventArgs {
            posX = x,
            posY = y,
            playerType = playerType
        });

        if(playerTurn == PlayerType.cross) 
            playerTurn = PlayerType.circle;
        else
            playerTurn = PlayerType.cross;
    }

    public PlayerType GetPlayerType() {
        return playerType;
    }
}

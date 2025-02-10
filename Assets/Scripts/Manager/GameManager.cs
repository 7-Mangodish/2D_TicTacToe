using NUnit.Framework.Constraints;
using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    public class OnClickPositionEventArgs : EventArgs {
        public int posX, posY;
        public PlayerType playerType;
    }
    public class OnPlayerWinArgs : EventArgs {
        public int posWinX, posWinY;
        public PlayerType winType;
        public float winAngle;
    }
    public class OnStartGameArgs : EventArgs {
        public string crossPlayerName;
        public string circlePlayerName;
        public PlayerType playerType;
        public string codeGame;
    }
    public event EventHandler<OnClickPositionEventArgs> OnClickPosition;
    public event EventHandler<OnPlayerWinArgs> OnPlayerWin;
    public event EventHandler OnPlayerTie;
    public event EventHandler<OnStartGameArgs> OnStartGame;
    public event EventHandler OnRematch;
    public event EventHandler OnPlayerHost;

    public enum PlayerType {
        None,
        cross,
        circle
    }

    private PlayerType playerType;
    public string playerName;
    public string circlePlayerName;
    public string crossPlayerName;
    public NetworkVariable <PlayerType> playerTurn = new NetworkVariable<PlayerType>(PlayerType.cross);
    public NetworkVariable<int> crossPlayerScore = new NetworkVariable<int>(0);
    public NetworkVariable<int> circlePlayerScore = new NetworkVariable<int>(0);

    private PlayerType[,] arrayPlayer = new PlayerType[3, 3];
    public struct locWin {
        public Vector2Int pos;
        public float angle;
        public PlayerType type;
    }
    private locWin win = new locWin();
    public bool isReady = false;
    public bool canPlay = false;


    private void Awake() {
        if(instance == null)
            instance = this;
        else {
            Destroy(gameObject);
        }

        playerName = MenuGameUiManager.namePlayer;
        Debug.Log(playerName);
    }



    public override void OnNetworkSpawn() {

        if (NetworkManager.Singleton.LocalClientId == 0) {
            playerType = PlayerType.cross;
            crossPlayerName = playerName;
            OnPlayerHost?.Invoke(this, EventArgs.Empty);
        }
        else {
            playerType = PlayerType.circle;
            circlePlayerName = playerName;
            SetCirclePlayerNameRpc(playerName);
        }
        if (IsServer) {

            NetworkManager.Singleton.OnClientConnectedCallback += StartGame_OnClientConnectedCallBack;

        }

    }

    public override void OnNetworkDespawn() {
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += OutMatch_OnClientDisconnectCallback;
        }
    }

    public void Disconect() {
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += OutMatch_OnClientDisconnectCallback;
        }
        if (IsClient) {
            TestRelay.Instance.OutMatch();
        }
    }

    private void OutMatch_OnClientDisconnectCallback(ulong obj) {
        Debug.Log("Client out");
    }

    [Rpc(SendTo.Server)]
    public void HandlerClickPositionRpc(int x, int y, PlayerType playerType) {
        if (!canPlay)
            return;
        if (playerType != playerTurn.Value)
            return;
        if (arrayPlayer[x, y] != PlayerType.None)
            return;

        OnClickPosition?.Invoke(this, new OnClickPositionEventArgs {
            posX = x,
            posY = y,
            playerType = playerType
        });
        arrayPlayer[x, y] = playerType;

        if(playerTurn.Value == PlayerType.cross) 
            playerTurn.Value = PlayerType.circle;
        else
            playerTurn.Value = PlayerType.cross;


        if (CheckPlayerWin()) {
            win.type = arrayPlayer[win.pos.x, win.pos.y];
            SetPlayerWinRpc(win.pos.x, win.pos.y, win.angle, win.type);

            if (win.type == PlayerType.cross)
                crossPlayerScore.Value += 1;
            else
                circlePlayerScore.Value += 1;

            canPlay = false;
        }
        else {
            if (CheckPlayerTie()) {
                OnPlayerTie?.Invoke(this, EventArgs.Empty);
                canPlay = false;
                Debug.Log("Tie!!");
            }
        }
    }

    #region CheckWin
    private bool CheckWin(PlayerType a1, PlayerType a2, PlayerType a3) {
        if (a1 == PlayerType.None || a2 == PlayerType.None || a3 == PlayerType.None)
            return false;
        if (a1 != a2 || a2 != a3 || a1 != a3)
            return false;
        return true;
    }
    private bool CheckPlayerWin() {
        if (CheckWin(arrayPlayer[1,1] , arrayPlayer[0,0], arrayPlayer[2, 2])) {
            win.pos = new Vector2Int(1, 1);
            win.angle = -45;
            return true;
        }
        else if(CheckWin(arrayPlayer[1, 1], arrayPlayer[0, 2], arrayPlayer[2, 0])) {
            win.pos = new Vector2Int(1, 1);
            win.angle = 45;
            return true;
        }
        else if (CheckWin(arrayPlayer[1, 1], arrayPlayer[0, 1], arrayPlayer[2, 1])) {
            win.pos = new Vector2Int(1, 1);
            win.angle = 90;
            return true;
        }
        else if (CheckWin(arrayPlayer[1, 1], arrayPlayer[1, 0], arrayPlayer[1, 2])) {
            win.pos = new Vector2Int(1, 1);
            win.angle = 0;
            return true;

        }
        else if (CheckWin(arrayPlayer[0, 1], arrayPlayer[0, 0], arrayPlayer[0, 2])) {
            win.pos = new Vector2Int(0, 1);
            win.angle = 0;
            return true;
        }
        else if (CheckWin(arrayPlayer[2, 1], arrayPlayer[2, 0], arrayPlayer[2, 2])) {
            win.pos = new Vector2Int(2, 1);
            win.angle = 0;
            return true;
        }
        else if (CheckWin(arrayPlayer[0, 0], arrayPlayer[1, 0], arrayPlayer[2, 0])) {
            win.pos = new Vector2Int(1, 0);
            win.angle = 90;
            return true;
        }
        else if (CheckWin(arrayPlayer[1, 2], arrayPlayer[0, 2], arrayPlayer[2, 2])) {
            win.pos = new Vector2Int(1, 2);
            win.angle = 90;
            return true;
        }
        return  false;
    }
    private bool CheckPlayerTie() {
        for(int i=0; i<arrayPlayer.GetLength(0); i++) {
            for(int j = 0; j<arrayPlayer.GetLength(1); j++) {
                if (arrayPlayer[i, j] == PlayerType.None)
                    return false;
            }

        }
        return true;
    }
    #endregion

    #region StartGame
    private void StartGame_OnClientConnectedCallBack(ulong obj) {
        if(NetworkManager.Singleton.ConnectedClientsList.Count == 2) {
            // Start game
            this.playerTurn.Value = PlayerType.cross;
            canPlay = true;

            UpdateNameAndCodeRpc(crossPlayerName, circlePlayerName);
            Debug.Log("Run");
            TriggerStartGameRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerStartGameRpc() {
        Debug.Log("Start Game");

        string code;

        if(IsServer)
            code = TestRelay.CodeGame;
        else
            code = MenuGameUiManager.codeType;


        OnStartGame?.Invoke(this, new OnStartGameArgs {
            crossPlayerName = this.crossPlayerName,
            circlePlayerName = this.circlePlayerName,
            playerType = PlayerType.cross,
            codeGame = code
        }) ;
    }
    [Rpc(SendTo.NotServer)]
    private void UpdateNameAndCodeRpc(string crossPlayerName, string circlePlayerName) {
        this.crossPlayerName = crossPlayerName;
        this.circlePlayerName = circlePlayerName;

        Debug.Log(this.crossPlayerName + " " + this.circlePlayerName + " ");
    }
    #endregion

    #region Rematch
    [Rpc(SendTo.Server)]
    public void RematchRpc() {
        Debug.Log("IsReady: " + isReady);
        if (isReady) {
            for (int i = 0; i < arrayPlayer.GetLength(0); i++) {
                for (int j = 0; j < arrayPlayer.GetLength(1); j++) {
                    arrayPlayer[i, j] = PlayerType.None;
                }
            }
            OnRematch?.Invoke(this, EventArgs.Empty);
            isReady = false;
            canPlay = true;
        }

    }

    [Rpc(SendTo.Server)]
    public void ReadyMatchRpc() {
        isReady = true;
    }
    #endregion



    [Rpc(SendTo.Everyone)]
    private void SetPlayerWinRpc(int posWinX, int posWinY, float angleWin,PlayerType playerWinType) {
        Debug.Log(playerWinType);
        OnPlayerWin?.Invoke(this, new OnPlayerWinArgs {
            posWinX = posWinX,
            posWinY = posWinY,
            winType = playerWinType,
            winAngle = angleWin,
        });

        Debug.Log( posWinX + " " + posWinY+ " " + angleWin + " " + playerWinType);

    }
    public PlayerType GetPlayerType() {
        return playerType;
    }

    public void SetPlayerName(string name) {
        this.playerName = name;
    }

    [Rpc(SendTo.Server)]
    public void SetCirclePlayerNameRpc( string circlePlayerName) {
        this.circlePlayerName = circlePlayerName;
    }

}

using System.Runtime.CompilerServices;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class TestLobby : MonoBehaviour
{
    private int maxPlayer = 4;
    private string nameLobby = "Test Lobby";
    private float timeHearbeat = 15;
    private Lobby hostLobby;
    private string playerName ="";
    async void Start()
    {
        playerName= "Mau" + UnityEngine.Random.Range(0,99);
        // Init Unity Service to using Service in project
        await UnityServices.InitializeAsync();

        //Sign In with Anonymous Mode
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Sign In: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void Update() {
        SendHeartBeat();
    }

    private async void SendHeartBeat()  {
        try {
            timeHearbeat -= Time.deltaTime;
            if(timeHearbeat < 0) {
                timeHearbeat = 15;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
        catch {
            
        }
    }

    public async void CreateLobby() {
        try {

            CreateLobbyOptions options = new CreateLobbyOptions {
                IsPrivate = false,
                Player = GetPlayer(),
                
            };
            hostLobby = await LobbyService.Instance.CreateLobbyAsync(nameLobby, maxPlayer, options);
            Debug.Log("Created Lobby: " + nameLobby + " " + hostLobby.Id + " " + hostLobby.LobbyCode);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.ToString());
        }
    }


    public async void ListLobbies() {
        try {
            // Create Filter
            QueryLobbiesOptions queryOptions = new QueryLobbiesOptions {
                Count = 10,
                Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "1", QueryFilter.OpOptions.GT)
                }
            };
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);

            List <Lobby> listLobby = response.Results;
            Debug.Log("Find quantity of lobies: " + listLobby.Count);
            foreach (Lobby lob in listLobby) {
                Debug.Log(lob.Name + lob.MaxPlayers);
            }
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.ToString());
        }
    }

    public async void JoinLobbyByCode(string lobbyCode) {
        try {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions {
                Player = GetPlayer(),
            };

            Lobby lobbyJoint = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode,options);
            Debug.Log("Join the Lobby " + lobbyJoint.LobbyCode);

            PrintPlayer(lobbyJoint);

        }
        catch (LobbyServiceException e){
            Debug.LogError(e.ToString());  
        }
    }

    public void PrintPlayer(Lobby lobby) {
        foreach(Player player in lobby.Players) {
            Debug.Log("Player in lobby: " + player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    private Player GetPlayer() {
        return new Player {
            Data = new Dictionary<string, PlayerDataObject> {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
        
    }
}

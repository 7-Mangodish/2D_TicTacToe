using System;
using System.Security;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestRelay : MonoBehaviour
{
    private static TestRelay instance;
    public static TestRelay Instance { get { return instance; } }
    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    async void Start()
    {

        await UnityServices.InitializeAsync();

        InitializationOptions hostOptions = new InitializationOptions().SetProfile("host");
        InitializationOptions clientOptions = new InitializationOptions().SetProfile("client");

        await UnityServices.InitializeAsync(hostOptions);

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        };

        if (AuthenticationService.Instance.IsAuthorized) {
            Debug.Log("Authorized");
            AuthenticationService.Instance.SignOut();
            await UnityServices.InitializeAsync(clientOptions);
        }
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void CreateRelay() {
        try {
            Allocation allocation =  await RelayService.Instance.CreateAllocationAsync(1);
            string code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            GameManager.Instance.SetCodeGame(code);
            Debug.Log(code);
            

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, "dtls"));
            NetworkManager.Singleton.StartHost();
        }
        catch(RelayServiceException e) {
            Debug.Log(e);
        }
    }

    public async void JoinRelay(string code) {
        try {

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                AllocationUtils.ToRelayServerData(joinAllocation, "dtls"));

            Debug.Log("Join Relay: " + code);
            NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }
}

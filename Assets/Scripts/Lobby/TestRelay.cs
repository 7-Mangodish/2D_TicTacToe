using System;
using System.Security;
using System.Threading.Tasks;
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

    [SerializeField] private GameObject[] listNetworkObj;

    private static string codeGame;
    public static string CodeGame { get { return codeGame; } }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);
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


    public async void CreateRelay() {
        try {

            foreach (GameObject obj in listNetworkObj) {
                Instantiate(obj, Vector3.zero, Quaternion.identity);
                Debug.Log("Init " + obj.name);

            }

            await Task.Delay(100);

            Allocation allocation =  await RelayService.Instance.CreateAllocationAsync(1);
            string code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            codeGame = code;
            Debug.Log(code);



            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, "dtls"));

            if(NetworkManager.Singleton == null) {
                Debug.Log("Null");
            }
            NetworkManager.Singleton.StartHost();
        }
        catch(RelayServiceException e) {
            Debug.Log(e);
        }
    }

    public async void JoinRelay(string code) {
        try {

            foreach (GameObject obj in listNetworkObj) {
                Instantiate(obj, Vector3.zero, Quaternion.identity);
                Debug.Log("Init " + obj.name);

            }

            await Task.Delay(100);

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

    public void OutMatch() {
        GameObject[] listGameObjects = GameObject.FindGameObjectsWithTag("NetworkObjects");
        Debug.Log(listGameObjects.Length);

        foreach (GameObject obj in listGameObjects) {
            Debug.Log(obj.name);
            Destroy(obj);
        }

        NetworkManager.Singleton.Shutdown();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}

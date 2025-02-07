using Unity.Netcode;
using UnityEngine;

//public class JoinGameManager : MonoBehaviour
//{
//    private static JoinGameManager instance;
//    public static JoinGameManager Instance { get { return instance; } }

//    [SerializeField] private GameObject networkObj;

//    private void Awake() {
//        if (instance == null)
//            instance = this;
//        else
//            Destroy(this.gameObject);
//        DontDestroyOnLoad(gameObject);
//    }
//    void Start()
//    {
        
//    }

//    void Update()
//    {
        
//    }

//    public void HostGame() {
//        TestRelay.Instance.CreateRelay();

//        GameObject[] listNetwork = GameObject.FindGameObjectsWithTag("NetworkManager");
//        Debug.Log("Found: " + listNetwork.Length);
//        foreach (GameObject network in listNetwork) {
//            Destroy(network);
//        }

//        GameObject objSpawn = Instantiate(networkObj, Vector3.zero, Quaternion.identity);
//        Debug.Log("Spawn");
//        objSpawn.GetComponent<NetworkManager>().StartHost();
//    }

//    public void JoinGame(string codeGame) {
//        TestRelay.Instance.JoinRelay(codeGame);

//        GameObject[] listNetwork = GameObject.FindGameObjectsWithTag("NetworkManager");
//        Debug.Log("Found: " + listNetwork.Length);
//        foreach (GameObject network in listNetwork) {
//            Destroy(network);
//        }

//        GameObject objSpawn = Instantiate(networkObj, Vector3.zero, Quaternion.identity);

//        objSpawn.GetComponent<NetworkManager>().StartClient();
//    }

//    public void ShutdownHostAndCLient() {

//        GameObject[] listNetwork = GameObject.FindGameObjectsWithTag("NetworkManager");
//        Debug.Log("Found: " + listNetwork.Length);
//        foreach (GameObject network in listNetwork) {
//            Destroy(network);
//        }

//        NetworkManager.Singleton.Shutdown();
//    }

//}

using Unity.Netcode;
using UnityEngine;

public class VisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossTransform;
    [SerializeField] private Transform circleTransform;
    [SerializeField] private Transform lineWinTransform;
    private float gridSize = 3.2f;
    void Start()
    {
        GameManager.Instance.OnClickPosition += VisualManager_OnClickPosition;
        GameManager.Instance.OnPlayerWin += VisualManager_OnPlayerWin;
    }



    void Update()
    {
        
    }

    private void VisualManager_OnPlayerWin(object sender, GameManager.OnPlayerWinArgs e) {
        SpawnLineWinRpc(e.posWinX, e.posWinY, e.winAngle);
    }

    [Rpc(SendTo.Server)]
    private void SpawnLineWinRpc(int x, int y, float angle) {
        Transform prefab = Instantiate(lineWinTransform, GetPositionOnGrid(x, y),Quaternion.Euler(0, 0, angle));
        prefab.GetComponent<NetworkObject>().Spawn(true);
        Debug.Log("Spawn Line: " + " " + x + " " + y + " " + angle);    
    }

    private void VisualManager_OnClickPosition(object sender, GameManager.OnClickPositionEventArgs e) {
        SpawnGameObjectRpc(e.posX, e.posY, e.playerType);
    }

    [Rpc(SendTo.Server)] // Goi tu client den Server (Thuc hien tai Server) 
    private void SpawnGameObjectRpc(int x, int y, GameManager.PlayerType playerType) {
        Transform prefab;
        if (playerType == GameManager.PlayerType.cross)
            prefab = crossTransform;
        else
            prefab = circleTransform;
        Transform spawnPrefab = Instantiate(prefab, GetPositionOnGrid(x, y), Quaternion.identity);
        spawnPrefab.GetComponent<NetworkObject>().Spawn(true);
        //Debug.Log("Spawned PlayerType: " + playerType);

    }

    private Vector2 GetPositionOnGrid(int x, int y) {
        //Debug.Log("posX: " + x + ",posY: " + y);
        return new Vector2(gridSize*(y-1), gridSize * (1-x));
    }
}

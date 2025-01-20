using Unity.Netcode;
using UnityEngine;

public class VisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossTransform;
    [SerializeField] private Transform circleTransform;
    private float gridSize = 3.2f;
    void Start()
    {
        GameManager.Instance.OnClickPosition += GameMagager_OnClickPosition;
    }


    void Update()
    {
        
    }


    private void GameMagager_OnClickPosition(object sender, GameManager.OnClickPositionEventArgs e) {
        Debug.Log(e.playerType);
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
        Debug.Log("Spawned PlayerType: " + playerType);

    }

    private Vector2 GetPositionOnGrid(int x, int y) {
        //Debug.Log("posX: " + x + ",posY: " + y);
        return new Vector2(gridSize*(y-1), gridSize * (1-x));
    }
}

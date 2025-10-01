using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class VisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossTransform;
    [SerializeField] private Transform circleTransform;
    [SerializeField] private Transform lineWinTransform;
    [SerializeField] private GameObject crossParticle;
    [SerializeField] private GameObject circleParticle;

    private List <GameObject> listSprite;
    private float gridSize = 3.2f;
    void Start()
    {
        listSprite = new List<GameObject>();
        GameManager.Instance.OnClickPosition += VisualManager_OnClickPosition;
        GameManager.Instance.OnPlayerWin += VisualManager_OnPlayerWin;
        GameManager.Instance.OnRematch += VisualManager_OnRematch;
    }

    private void VisualManager_OnRematch(object sender, System.EventArgs e) {
        foreach(GameObject obj in listSprite) {
            Destroy(obj);
        }
        listSprite.Clear();
    }

    // Show Line when player win
    private void VisualManager_OnPlayerWin(object sender, GameManager.OnPlayerWinArgs e) {
        SpawnLineWinRpc(e.posWinX, e.posWinY, e.winAngle);
    }

    [Rpc(SendTo.Server)]
    private void SpawnLineWinRpc(int x, int y, float angle) {
        Transform prefab = Instantiate(lineWinTransform, GetPositionOnGrid(x, y),Quaternion.Euler(0, 0, angle));
        prefab.GetComponent<NetworkObject>().Spawn(true);

        listSprite.Add(prefab.gameObject);
        //Debug.Log("Spawn Line: " + " " + x + " " + y + " " + angle);    
    }

    // Show sprite
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
        SetParticleRpc(x,y,playerType);

        listSprite.Add(spawnPrefab.gameObject);
        //Debug.Log("Spawned PlayerType: " + playerType);

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetParticleRpc(int x, int y, GameManager.PlayerType type) {
        if(type == GameManager.PlayerType.cross) {
            Instantiate(crossParticle, GetPositionOnGrid(x,y), Quaternion.identity);
        }
        else {
            Instantiate(circleParticle, GetPositionOnGrid(x, y), Quaternion.identity);

        }
    }

    private Vector2 GetPositionOnGrid(int x, int y) {
        //Debug.Log("posX: " + x + ",posY: " + y);
        return new Vector2(gridSize*(y-1), gridSize * (1-x));
    }

}

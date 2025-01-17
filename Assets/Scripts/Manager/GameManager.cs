using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance { get => instance; }

    private void Awake() {
        if(instance == null)
            instance = this;
        else {
            Debug.Log("There are more than one Gamemanger");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void HandlerClickPosition(int x, int y) {
        Debug.Log("Call from GameManager: " + x + y);
    }
}

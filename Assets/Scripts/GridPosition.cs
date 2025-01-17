using UnityEngine;

public class GridPosition : MonoBehaviour
{
    private int x, y;
    void Start()
    {
        string name = this.gameObject.name;
        string []word = name.Split(new char[] {' '}, System.StringSplitOptions.RemoveEmptyEntries);
        x = int.Parse(word[1]);
        y = int.Parse(word[2]);
    }

    void Update()
    {
        
    }

    private void OnMouseDown() {
        GameManager.Instance.HandlerClickPosition(x, y);
        Debug.Log("Click on " + name);
    }

}

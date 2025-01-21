using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] private GameObject crossScore;
    [SerializeField] private GameObject crossArrow;

    [SerializeField] private GameObject circleScore;
    [SerializeField] private GameObject circleArrow;

    private void Awake() {
        crossArrow.SetActive(false);
        circleArrow.SetActive(false);
    }
    void Start()
    {
        GameManager.Instance.playerTurn.OnValueChanged += ChangeArrow_OnChangeTurn;
        //GameManager.Instance.OnChangePlayerTurn += ChangeArrow_OnChangePlayerTurn;
    }


    private void  ChangeArrow_OnChangeTurn(GameManager.PlayerType oldVal, GameManager.PlayerType newVal) {
        Debug.Log("Old: " + oldVal);
        Debug.Log("New: " + newVal);
        Debug.Log("Turn: "+GameManager.Instance.playerTurn.Value);

        if (newVal == GameManager.PlayerType.cross) {
            crossArrow.SetActive(true);
            circleArrow.SetActive(false);
        }
        else {
            circleArrow.SetActive(true);
            crossArrow.SetActive(false);
        }

    }

}

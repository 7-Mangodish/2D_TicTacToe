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
        GameManager.Instance.crossPlayerScore.OnValueChanged += UpdateCrossScore_OnValueChange;
        GameManager.Instance.circlePlayerScore.OnValueChanged += UpdateCircleScore_OnValueChange;
        GameManager.Instance.OnStartGame += ScoreBoard_OnStartGame;
    }

    private void UpdateCrossScore_OnValueChange(int oldVal, int newVal) {
        crossScore.GetComponent<TextMeshProUGUI>().text = newVal.ToString();
    }

    private void UpdateCircleScore_OnValueChange(int oldVal, int newVal) {
        circleScore.GetComponent<TextMeshProUGUI>().text = newVal.ToString();
    }

    private void ScoreBoard_OnStartGame(object sender, GameManager.PlayerType typeStart) {
        Debug.Log("Start with type: " + typeStart);
        if(typeStart == GameManager.PlayerType.cross)
            crossArrow.SetActive(true);
        else
            circleArrow.SetActive(true);
    }

    private void  ChangeArrow_OnChangeTurn(GameManager.PlayerType oldVal, GameManager.PlayerType newVal) {
        //Debug.Log("Old: " + oldVal);
        //Debug.Log("New: " + newVal);
        //Debug.Log("Turn: "+GameManager.Instance.playerTurn.Value);

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

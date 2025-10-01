using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] private GameObject crossNameText;
    [SerializeField] private GameObject crossScore;
    [SerializeField] private GameObject crossArrow;

    [SerializeField] private GameObject circleNameText;
    [SerializeField] private GameObject circleScore;
    [SerializeField] private GameObject circleArrow;

    [SerializeField] private GameObject code;
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
        GameManager.Instance.OnPlayerHost += ScoreBoard_OnPlayerHost;
        GameManager.Instance.OnClientOutMatch += ScoreBoard_OnClientOutMatch;

    }

    private void ScoreBoard_OnClientOutMatch(object sender, EventArgs e) {
        circleNameText.gameObject.GetComponent<TextMeshProUGUI>().text = "Wait for other player";
        crossArrow.gameObject.SetActive(false);
    }

    private void ScoreBoard_OnPlayerHost(object sender, EventArgs e) {
        crossNameText.gameObject.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.crossPlayerName;
        circleNameText.gameObject.GetComponent<TextMeshProUGUI>().text = "Wait for other player";
        code.SetActive(true);
        code.gameObject.GetComponent<TextMeshProUGUI>().text = "Code: " + TestRelay.CodeGame;
    }

    private void UpdateCrossScore_OnValueChange(int oldVal, int newVal) {
        crossScore.GetComponent<TextMeshProUGUI>().text = newVal.ToString();
    }

    private void UpdateCircleScore_OnValueChange(int oldVal, int newVal) {
        circleScore.GetComponent<TextMeshProUGUI>().text = newVal.ToString();
    }

    private void ScoreBoard_OnStartGame(object sender, GameManager.OnStartGameArgs e) {
        SetUpToStart(e.crossPlayerName, e.circlePlayerName, e.playerType, e.codeGame);
    }

    private void SetUpToStart(string crossPlayerName, string circlePlayerName, GameManager.PlayerType firstType, string codeGame) {
        Debug.Log(crossPlayerName + " " + circlePlayerName + " " + firstType + " " + codeGame);

        crossNameText.gameObject.GetComponent<TextMeshProUGUI>().text = crossPlayerName;
        circleNameText.gameObject.GetComponent<TextMeshProUGUI>().text = circlePlayerName;

        if (firstType == GameManager.PlayerType.cross) {
            crossArrow.SetActive(true);
        }
        else {
            circleArrow.SetActive(true);
        }
        code.gameObject.GetComponent<TextMeshProUGUI>().text = "Code: " + codeGame;

        //Debug.Log("Start with type: " + firstType);
        //Debug.Log("PlayerType: " + GameManager.Instance.GetPlayerType());
        //Debug.Log("CrossPlayerName: " + GameManager.Instance.crossPlayerName);
        //Debug.Log("CirclePlayerName: " + GameManager.Instance.circlePlayerName);
    }


    private void  ChangeArrow_OnChangeTurn(GameManager.PlayerType oldVal, GameManager.PlayerType newVal) {
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

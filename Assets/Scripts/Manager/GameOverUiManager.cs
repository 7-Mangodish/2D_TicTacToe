using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUiManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject wintTitle;
    [SerializeField] private GameObject loseTitle;
    [SerializeField] private GameObject tieTitle;
    [SerializeField] private Button rematchBtn;
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button menuBtn;

    [SerializeField] private TextMeshProUGUI crossPlayerScore;
    [SerializeField] private TextMeshProUGUI circlePlayerScore;

    [SerializeField] private GameObject notifyUI;


    private async void  Start() {
        await Task.Delay(1000); // wait for GameManager 
        GameManager.Instance.OnPlayerWin += GameOver_OnPlayerWin;
        GameManager.Instance.crossPlayerScore.OnValueChanged += GameOver_crossPlayerScoreChange;
        GameManager.Instance.circlePlayerScore.OnValueChanged += GameOver_circlePlayerScoreChange;
        GameManager.Instance.OnClientOutMatch += GameOver_OnClientOutMatch;

        readyBtn.onClick.AddListener(() => {
            SoundManager.Instance.Play("ClickBtn");

            GameManager.Instance.ReadyMatchRpc();
            TurnOffUI();
        });

        rematchBtn.onClick.AddListener(() => {
            SoundManager.Instance.Play("ClickBtn");

            if (GameManager.Instance.isReady) {
                GameManager.Instance.RematchRpc();
                TurnOffUI();
            }
            else {
                notifyUI.gameObject.SetActive(true);
            }
        });

        menuBtn.onClick.AddListener(() => {
            TurnOffUI();
            SoundManager.Instance.Play("ClickBtn");

            TestRelay.Instance.OutMatch();
            //GameManager.Instance.Disconect();
        });
    }

    private void GameOver_OnClientOutMatch(object sender, System.EventArgs e) {
        TurnOffUI();
    }

    private async void GameOver_OnPlayerWin(object sender, GameManager.OnPlayerWinArgs e) {
        await Task.Delay(500);
        if (GameManager.Instance.IsServer)
            Debug.Log("Call from Server");
        else
            Debug.Log("Call from Client");
        Debug.Log("Type win" + e.winType);

        gameOverUI.SetActive(true);
        SetButton();
        SetTitle(e.winType);
    }

    private void SetButton() {
        if(GameManager.Instance.IsServer)
            rematchBtn.gameObject.SetActive(true);
        else
            readyBtn.gameObject.SetActive(true);
    }

    private void SetTitle(GameManager.PlayerType playerWin) {
        if(GameManager.Instance.GetPlayerType() == playerWin)
            wintTitle.SetActive(true);
        else
            loseTitle.SetActive(true);
    }

    private void GameOver_crossPlayerScoreChange(int oldVal, int newVal){
        crossPlayerScore.text = newVal.ToString();
    }

    private void GameOver_circlePlayerScoreChange(int oldVal, int newVal) {
        circlePlayerScore.text = newVal.ToString();
    }

    private void TurnOffUI() {
        wintTitle.gameObject.SetActive(false);
        loseTitle.gameObject.SetActive(false);
        tieTitle.gameObject.SetActive(false);

        readyBtn.gameObject.SetActive(false);
        rematchBtn.gameObject.SetActive(!false);
        gameOverUI.SetActive(false);
        notifyUI.SetActive(false);
    }
}

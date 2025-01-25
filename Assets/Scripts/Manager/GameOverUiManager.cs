using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUiManager : NetworkBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject notifyPanel;
    [SerializeField] private GameObject winTiltle;
    [SerializeField] private GameObject loseTiltle;
    [SerializeField] private GameObject tieTiltle;
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button rematchBtn;
    [SerializeField] private TextMeshProUGUI crossPlayerScore;
    [SerializeField] private TextMeshProUGUI circlePlayerScore;

    private void Awake() {
        rematchBtn.onClick.AddListener(() => {
            Debug.Log("rematchBtn");
            if(GameManager.Instance.isReady == true) {
                Hide();
                GameManager.Instance.RematchRpc();
            }
            else {
                notifyPanel.SetActive(true);
            }
        });
        readyBtn.onClick.AddListener(() => {
            Debug.Log("Ready");
            GameManager.Instance.ReadyMatchRpc();
            Hide();
        });
    }
    void Start()
    {

        GameManager.Instance.OnPlayerWin += GameOver_OnPlayerWinRpc;
        GameManager.Instance.crossPlayerScore.OnValueChanged += UpdateCrossScore_OnValueChange;
        GameManager.Instance.circlePlayerScore.OnValueChanged += UpdateCircleScore_OnValueChange;
        GameManager.Instance.OnPlayerTie += GameOver_OnPlayerTie;
    }



    void Update()
    {
        
    }

    private void GameOver_OnPlayerTie(object sender, System.EventArgs e) {
        SetPanelTieRpc();
    }
    private void UpdateCrossScore_OnValueChange(int oldVal, int newVal) {
        crossPlayerScore.text = newVal.ToString();
    }

    private void UpdateCircleScore_OnValueChange(int oldVal, int newVal) {
        circlePlayerScore.text = newVal.ToString();
    }
    private void GameOver_OnPlayerWinRpc(object sender, GameManager.OnPlayerWinArgs e) {
        SetPanelRpc(e.winType);
    }

    #region When Player Win
    [Rpc(SendTo.ClientsAndHost)]
    private void SetPanelRpc(GameManager.PlayerType winType) {

        gameOverUI.SetActive(true);
        if (GameManager.Instance.GetPlayerType() == winType) {
            winTiltle.SetActive(true);
            Debug.Log("Win");
        }
        else {
            loseTiltle.SetActive(true);
            Debug.Log("Lose");
        }
        SetRematchBtn();
    }
    #endregion

    #region When Player Tie
    [Rpc(SendTo.ClientsAndHost)]
    private void SetPanelTieRpc() {
        gameOverUI.SetActive(true) ;
        tieTiltle.SetActive(true);
        SetRematchBtn() ;
    }
    #endregion

    private void SetRematchBtn() {
        if (GameManager.Instance.IsServer) {
            rematchBtn.gameObject.SetActive(true);
        }
        else {
            readyBtn.gameObject.SetActive(true);
        }
    }
    private void Hide() {
        gameOverUI.SetActive(false);
        winTiltle.SetActive(false);
        loseTiltle.SetActive(false);
        tieTiltle.SetActive(false);
        readyBtn.gameObject.SetActive(false);
        rematchBtn.gameObject.SetActive(false);
        notifyPanel.gameObject.SetActive(false);
    }
}

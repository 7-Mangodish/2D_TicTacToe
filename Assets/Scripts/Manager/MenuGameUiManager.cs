using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuGameUiManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField namePlayerInput;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button joinBtn;

    [SerializeField] private GameObject joinPanel;
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private Button joinGameBtn;

    private string code;
    private string namePlayer;
    void Start()
    {
        namePlayerInput.onEndEdit.AddListener((string s) => {
            GameManager.Instance.SetPlayerName(s);
            Debug.Log("PlayerName: " + GameManager.Instance.playerName);
        });
        hostBtn.onClick.AddListener(() =>{
            SceneManager.LoadScene(1);
            TestRelay.Instance.CreateRelay();
 
        });
        joinBtn.onClick.AddListener(() => {
            joinPanel.SetActive(true);
        });
        codeInput.onEndEdit.AddListener((string s) => {
            code = s;
        });
        joinGameBtn.onClick.AddListener(() => {
            SceneManager.LoadScene(1);
            TestRelay.Instance.JoinRelay(code);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

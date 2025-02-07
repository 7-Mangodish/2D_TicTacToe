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

    public static string codeType;
    public static string namePlayer;
    void Start()
    {
        namePlayerInput.onEndEdit.AddListener((string s) => {
            namePlayer = s;
        });

        hostBtn.onClick.AddListener(() =>{
            SoundManager.Instance.Play("ClickBtn");
            SceneManager.LoadScene(1);

            SceneManager.sceneLoaded += HostGame_sceneLoaded;
        });

        joinBtn.onClick.AddListener(() => {
            SoundManager.Instance.Play("ClickBtn");

            joinPanel.SetActive(true);
        });

        codeInput.onEndEdit.AddListener((string s) => {
            codeType = s;
        });

        joinGameBtn.onClick.AddListener(() => {
            SoundManager.Instance.Play("ClickBtn");
            SceneManager.LoadScene(1);

            SceneManager.sceneLoaded += JoinGame_sceneLoaded;
        });
    }

    private void JoinGame_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
        if (arg0.buildIndex == 1) {
            SceneManager.sceneLoaded -= JoinGame_sceneLoaded;
            Debug.Log("Loaded");
            TestRelay.Instance.JoinRelay(codeType);
        }
    }

    private void HostGame_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
        if(arg0.buildIndex == 1) {
            SceneManager.sceneLoaded -= HostGame_sceneLoaded;
            Debug.Log("Loaded");
            TestRelay.Instance.CreateRelay();
        }
    }
}

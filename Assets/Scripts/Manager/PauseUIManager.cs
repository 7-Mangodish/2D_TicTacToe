using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUIManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Slider backgroundSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button menuBtn;
    [SerializeField] private Button continueBtn;

    private void Awake() {
        backgroundSlider.value = 0.2f;
        sfxSlider.value = 0.5f;


        backgroundSlider.onValueChanged.AddListener((float value) => {
            SoundManager.Instance.ChangeVolume(SoundManager.SoundType.backround, value);
        });
        sfxSlider.onValueChanged.AddListener((float value) => {
            SoundManager.Instance.ChangeVolume(SoundManager.SoundType.sfx, value);
        });
        menuBtn.onClick.AddListener(() => {
            SoundManager.Instance.Play("ClickBtn");

        });
        continueBtn.onClick.AddListener(() => {
            SoundManager.Instance.Play("ClickBtn");
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        });

    }

    void Update()
    {
        if (Input.GetKeyDown("escape")) {
            Time.timeScale = 0;
            pausePanel.gameObject.SetActive(true);
        }
    }
}

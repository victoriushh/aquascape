using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    public Button buttonOpenMenu;
    public GameObject panelMenu;
    public Button buttonCloseMenu;
    public Button buttonRestart;
    public Button buttonExit;

    public GameObject inGameScript;

    public GameObject panelInstuction;

    public Button buttonStart;

    public GameObject panelWarning;
    public TextMeshProUGUI errorMessage;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        OnAppStart();

        UiFunction();
    }
    void Update()
    {
        scoreText.text = "Score: " + GameManager.Instance.score;
        highScoreText.text = "High Score: " + GameManager.Instance.highScore;
    }

    void UiFunction()
    {
        buttonOpenMenu.onClick.AddListener(ButtonOpenMenuPressed);
        buttonCloseMenu.onClick.AddListener(ButtonExitMenuPressed);
        buttonRestart.onClick.AddListener(ButtonRestartPressed);
        buttonExit.onClick.AddListener(ButtonExitPressed);
        buttonStart.onClick.AddListener(ButtonStartPressed);
    }

    void ButtonOpenMenuPressed()
    {
        inGameScript.SetActive(false);
        Time.timeScale = 0f; // pause
        panelMenu.SetActive(true);
    }

    void ButtonExitMenuPressed()
    {
        inGameScript.SetActive(true);
        Time.timeScale = 1f; // pause
        panelMenu.SetActive(false);
    }

    void ButtonRestartPressed()
    {
        GameManager.Instance.GameOver();
    }

    void ButtonExitPressed()
    {
        Application.Quit();
    }

    void OnAppStart()
    {

        inGameScript.SetActive(false);
        panelInstuction.SetActive(true);
    }
    void ButtonStartPressed()
    {
        inGameScript.SetActive(true);
        panelInstuction.SetActive(false);
    }

    public void ShowPopUp(string message)
    {
        panelWarning.SetActive(true);
        errorMessage.text = message;
    }
}
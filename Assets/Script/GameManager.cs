using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public int highScore = 0;

    private const string HIGH_SCORE_KEY = "HIGH_SCORE";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadHighScore();
    }

    // ================= SCORE =================
    public void AddScore(int amount)
    {
        score += amount;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }
    }

    public void MinScore(int amount)
    {
        score -= amount;
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
        SceneManager.LoadScene("SampleScene"); //restart
    }


    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }
}
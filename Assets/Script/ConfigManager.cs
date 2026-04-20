using UnityEngine;
using System.IO;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance;

    public GameConfig config;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadConfig();
    }

    void LoadConfig()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "config.json");
        Debug.Log(path);
        if (!File.Exists(path))
        {
            if (UiManager.Instance != null)
                UiManager.Instance.ShowPopUp("Gagal membaca config, menggunakan default config");

            config = GetDefaultConfig();
            string json = JsonUtility.ToJson(GetDefaultConfig(), true);
            File.WriteAllText(path, json);
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            config = JsonUtility.FromJson<GameConfig>(json);

            if (config == null)
            {
                if (UiManager.Instance != null)
                    UiManager.Instance.ShowPopUp("Gagal membaca config, menggunakan default config");
                config = GetDefaultConfig();
            }
        }
        catch
        {
            if (UiManager.Instance != null)
            {
                UiManager.Instance.ShowPopUp("Gagal membaca config, menggunakan default config");
            }
            config = GetDefaultConfig();
        }
    }

    private GameConfig GetDefaultConfig()
    {
        return new GameConfig
        {
            folderPath = Path.Combine(Application.streamingAssetsPath, "assetAquascape"),
            fishMinSpeed = 1f,
            fishMaxSpeed = 2f,
            hungerCooldown = 10f,
            trashSpawnIntervalMin = 5f,
            trashSpawnIntervalMax = 15f,
            foodLifetime = 10f
        };
    }
}
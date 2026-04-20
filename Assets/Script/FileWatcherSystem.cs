using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;


public class FileWatcherSystem : MonoBehaviour
{
    public string folderPath;
    public float scanInterval = 2f;

    private HashSet<string> processedFiles = new HashSet<string>();

    void Start()
    {
        folderPath = ConfigManager.Instance.config.folderPath;
        InvokeRepeating(nameof(ScanFolder), 0f, scanInterval);
    }

    async void ScanFolder()
    {
        if (!Directory.Exists(folderPath))
        {
            UiManager.Instance?.ShowPopUp($"Folder tidak ditemukan: {folderPath}");
            return;
        }

        var files = Directory.GetFiles(folderPath);

        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);

            if (processedFiles.Contains(file))
                continue;

            bool isValid = true;

            if (Path.GetExtension(file).ToLower() != ".png")
            {
                UiManager.Instance?.ShowPopUp($"Skip {fileName}: harus .png");
                isValid = false;
            }

            string nameWithoutExt = Path.GetFileNameWithoutExtension(file);
            string[] parts = nameWithoutExt.Split('_');

            if (parts.Length != 3)
            {
                UiManager.Instance?.ShowPopUp($"Format salah {fileName}: harus CATEGORY_TYPE_TIMESTAMP");
                isValid = false;
            }

            string category = "";
            string type = "";
            string timestamp = "";

            if (parts.Length == 3)
            {
                category = parts[0];
                type = parts[1];
                timestamp = parts[2];

                if (category != "FISH" && category != "TRASH")
                {
                    UiManager.Instance?.ShowPopUp($"Kategori salah {fileName}: harus FISH / TRASH");
                    isValid = false;
                }

                if (string.IsNullOrEmpty(type))
                {
                    UiManager.Instance?.ShowPopUp($"Type kosong {fileName}");
                    isValid = false;
                }

                if (timestamp.Length != 14 || !long.TryParse(timestamp, out _) || !IsValidDateTime(timestamp))
                {
                    UiManager.Instance?.ShowPopUp($"Timestamp invalid {fileName}");
                    isValid = false;
                }
            }

            if (!isValid)
            {
                processedFiles.Add(file); 
                continue;
            }

            Texture2D tex = await LoadTextureAsync(file);

            if (tex == null)
            {
                UiManager.Instance?.ShowPopUp($"Gagal load texture: {fileName}");
                processedFiles.Add(file);
                continue;
            }

            ParsedFileData data = ParseFileName(file);
            SpawnManager.Instance.RegisterEntity(data, tex);

            processedFiles.Add(file);
        }

        ParsedFileData ParseFileName(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            string[] parts = fileName.Split('_');

            if (parts.Length < 3) return null;

            ParsedFileData data = new ParsedFileData();
            data.fullPath = path;

            if (parts[0] == "FISH")
                data.category = EntityCategory.Fish;
            else if (parts[0] == "TRASH")
                data.category = EntityCategory.Trash;
            else
                return null;

            data.type = parts[1];
            return data;
        }

        async Task<Texture2D> LoadTextureAsync(string path)
        {
            byte[] bytes = await File.ReadAllBytesAsync(path);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);

            return tex;
        }

        bool IsValidDateTime(string timestamp)
        {
            try
            {
                int year = int.Parse(timestamp.Substring(0, 4));
                int month = int.Parse(timestamp.Substring(4, 2));
                int day = int.Parse(timestamp.Substring(6, 2));
                int hour = int.Parse(timestamp.Substring(8, 2));
                int minute = int.Parse(timestamp.Substring(10, 2));
                int second = int.Parse(timestamp.Substring(12, 2));

                System.DateTime dt = new System.DateTime(year, month, day, hour, minute, second);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
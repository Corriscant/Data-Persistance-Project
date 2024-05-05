using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

[System.Serializable]
public class HighscoreEntry
{
    public string PlayerName;
    public int Score;
}

public class HighscoreTable : MonoBehaviour
{
    public List<HighscoreEntry> HighscoreList = new List<HighscoreEntry>();
    public GameObject HighscoreEntryPrefab;
    public Transform HighscoreTableContainer;
    public int MaxEntries = 5;
    private string _saveFilePath;

    void Start()
    {
        _saveFilePath = Path.Combine(Application.persistentDataPath, "highscore.json");
        Debug.Log("Save file path: " + _saveFilePath);
        LoadHighscores();
    }

    public void AddNewEntry(string playerName, int score)
    {
        // Добавляем новую запись
        HighscoreList.Add(new HighscoreEntry { PlayerName = playerName, Score = score });

        // Сортируем по убыванию и обрезаем до максимального количества
        HighscoreList = HighscoreList.OrderByDescending(e => e.Score).Take(MaxEntries).ToList();

        // Сохраняем таблицу и обновляем UI
        SaveHighscores();
        UpdateHighscoreTable();
    }

    void UpdateHighscoreTable()
    {
        // Очистить старые записи
        foreach (Transform child in HighscoreTableContainer)
        {
            Destroy(child.gameObject);
        }

        // Заполнить новыми записями
        foreach (var entry in HighscoreList)
        {
            var entryGO = Instantiate(HighscoreEntryPrefab, HighscoreTableContainer);
            var texts = entryGO.GetComponentsInChildren<TMPro.TextMeshProUGUI>();

            texts[0].text = entry.PlayerName;
            texts[1].text = entry.Score.ToString();

            Debug.Log("HighscoreTable.UpdateHighscoreTable: " + entry.PlayerName + " " + entry.Score);
        }
    }

    void SaveHighscores()
    {
        string json = JsonConvert.SerializeObject(HighscoreList, Formatting.Indented);
        File.WriteAllText(_saveFilePath, json);
    }

    void LoadHighscores()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            HighscoreList = JsonConvert.DeserializeObject<List<HighscoreEntry>>(json)
              .OrderByDescending(e => e.Score)
              .Take(MaxEntries)
              .ToList();
            UpdateHighscoreTable();
        }
        else
        {
            // Добавь тестовые данные
            AddNewEntry("Alice", 500);
            AddNewEntry("Bob", 300);
            AddNewEntry("Charlie", 100);
            AddNewEntry("Dave", 400);
            AddNewEntry("Eve", 200);
            // AddNewEntry("Frank", 600);
        }
    }

    // Button to reset the highscore table
    public void ResetHighscoreTable()
    {
        HighscoreList.Clear();
        UpdateHighscoreTable();
    }

}

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
    static public List<HighscoreEntry> HighscoreList = new List<HighscoreEntry>();
    static private bool hadBeenLoaded = false;
    public GameObject HighscoreEntryPrefab;
    public Transform HighscoreTableContainer;
    static public int MaxEntries = 5;
    static private string _saveFilePath;

    void Start()
    {
        Debug.Log("Save file path: " + GetSaveFilePath());

        if ((!hadBeenLoaded) && File.Exists(GetSaveFilePath()))
        {
            LoadHighscores();
        }
        else if (!hadBeenLoaded)
        {
            // ƒобавь тестовые данные
            AddNewEntry("Alice", 500);
            AddNewEntry("Bob", 300);
            AddNewEntry("Charlie", 100);
            AddNewEntry("Dave", 400);
            AddNewEntry("Eve", 200);
            // AddNewEntry("Frank", 600);
        }
        else // ≈сли таблица уже загружена (получаетс€, что мы возвращаемс€ из меню)
        {

        }
        UpdateHighscoreTable();
    }

    static string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "highscore.json");
    }

    // Initialize the highscore table (call from MainMenu)
    static public void InitializeHighscoreTable()
    {
        if (File.Exists(GetSaveFilePath()))
        {
            LoadHighscores();
        }
    }

    static public void AddNewEntry(string playerName, int score)
    {
        // ƒобавл€ем новую запись
        HighscoreList.Add(new HighscoreEntry { PlayerName = playerName, Score = score });


        HighscoreList
            .Select((entry, index) => new { entry, index })
            .ToList()
            .ForEach(e => Debug.Log($"ADD  HighscoreTable[{e.index}]: {e.entry.PlayerName} {e.entry.Score}"));

        // —ортируем по убыванию и обрезаем до максимального количества
        HighscoreList = HighscoreList.OrderByDescending(e => e.Score).Take(MaxEntries).ToList();

        // Log with foreach
        HighscoreList
            .Select((entry, index) => new { entry, index })
            .ToList()
            .ForEach(e => Debug.Log($"AFTER SORT HighscoreTable[{e.index}]: {e.entry.PlayerName} {e.entry.Score}"));

        // Log all HighscoreList (old way)
        //  for (int i = 0; i < HighscoreList.Count; i++)
        //  {
        //      var entry = HighscoreList[i];
        //      Debug.Log("HighscoreTable[" + i + "]: " + entry.PlayerName + " " + entry.Score);
        //  }

        // —охран€ем таблицу и обновл€ем UI
        SaveHighscores();
    }

    void UpdateHighscoreTable()
    {
        // ќчистить старые записи
        foreach (Transform child in HighscoreTableContainer)
        {
            Destroy(child.gameObject);
        }

        // «аполнить новыми запис€ми
        foreach (var entry in HighscoreList)
        {
            var entryGO = Instantiate(HighscoreEntryPrefab, HighscoreTableContainer);
            var texts = entryGO.GetComponentsInChildren<TMPro.TextMeshProUGUI>();

            texts[0].text = entry.PlayerName;
            texts[1].text = entry.Score.ToString();

            Debug.Log("HighscoreTable.UpdateHighscoreTable: " + entry.PlayerName + " " + entry.Score);
        }
    }

    static public void SaveHighscores()
    {
        string json = JsonConvert.SerializeObject(HighscoreList, Formatting.Indented);
        File.WriteAllText(GetSaveFilePath(), json);
    }

    static void LoadHighscores()
    {
        if (hadBeenLoaded)
        {
            return;
        }
        else
        {
            string json = File.ReadAllText(GetSaveFilePath());
            HighscoreList = JsonConvert.DeserializeObject<List<HighscoreEntry>>(json)
              .OrderByDescending(e => e.Score)
              .Take(MaxEntries)
              .ToList();
            hadBeenLoaded = true;
        }
    }


    // Click on Return to Menu button
    public void ReturnToMenu()
    {
        MainMenu.ReturnToMenu();
    }

    // Button to reset the highscore table
    public void ResetHighscoreTable()
    {
        HighscoreList.Clear();
        SaveHighscores();
        UpdateHighscoreTable();
    }

}

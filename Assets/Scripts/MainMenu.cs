using TMPro;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // to accwss File class

public class MainMenu : MonoBehaviour
{
    public static string currentPlayerName = "Player";  // Переменная для хранения имени игрока
    public static int currentScore = 0;  // Переменная для хранения текущего результата
    static int bestScore = 0;  // Переменная для хранения лучшего результата
    public static string bestPlayerName = "Player";  // Переменная для хранения имени игрока с лучшим результатом
    public static string bestScoreText;  // Переменная для хранения текста лучшего результата
    public TMP_InputField inputField;  // Переменная для хранения ссылки на TMP_InputField
    public TextMeshProUGUI bestScoreTextUI;  // Переменная для хранения ссылки на TextMeshProUGUI

    // button object for Difficulty settings to mod the difficulty level and show current level with text and color
    public GameObject difficultyButton;

    // Метод для загрузки основной сцены игры
    public void StartGame()
    {
        SceneManager.LoadScene("main", LoadSceneMode.Single); // Указываем название сцены, которое вы использовали в Unity
    }

    // Open Scene to set difficulty level
    public void SetDifficultyClick()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Single);
    }

    // Method to set the difficulty button state (text and color)
    private void UpdateDifficultyButtonState()
    {
        // Set the text of the button to the current difficulty level
        // Получаем ссылку на TextMeshProUGUI внутри кнопки
        var buttonText = difficultyButton.GetComponentInChildren<TextMeshProUGUI>();

        // Устанавливаем текст и цвет в зависимости от уровня сложности
        switch (Settings.difficultyLevel)
        {
            case 1:
                buttonText.text = "Easy";
                buttonText.color = Color.green;
                break;
            case 2:
                buttonText.text = "Normal";
                buttonText.color = Color.blue;
                break;
            case 3:
                buttonText.text = "Hard";
                buttonText.color = Color.red;
                break;
            default:
                buttonText.text = "Difficulty";
                buttonText.color = Color.white;
                break;
        }
    }

    // Метод для выхода из игры
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
    }

    // open the highscore table
    public void OpenHighscoreTable()
    {
        SceneManager.LoadScene("HighscoreTable", LoadSceneMode.Single);
    }

    static public void ReturnToMenu()
    {
        // Код для возвращения в главное меню
        if (SceneManager.GetActiveScene().name != "MainMenu")
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void OnNameEntered()
    {
        currentPlayerName = inputField.text;  // Прямой доступ к тексту поля ввода

        if (string.IsNullOrEmpty(name))
            Debug.Log("The input field is empty or null.");
        else
            Debug.Log("User name entered: " + name);
    }

    private void Update()
    {
        //  Debug.Log("MainMenu.Update: Current score: " + currentScore);

    }


    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        LoadSession();

        UpdateDifficultyButtonState();
    }

    private void OnDestroy()
    {
        SaveSession();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Debug.Log("We are back in the MainMenu scene");
            // Здесь можно разместить код, аналогичный тому, что вы хотели выполнить в Start или OnEnable
            HandleReturningToMenu();
        }
    }

    private void HandleReturningToMenu()
    {
        Debug.Log("Handle the logic for returning to the menu");
        // Обновите UI или выполните другие действия
        inputField.text = currentPlayerName;
        // lowest score in the highscore table
        float lowestScore = HighscoreTable.HighscoreList.Count > 0 ? HighscoreTable.HighscoreList[HighscoreTable.HighscoreList.Count - 1].Score : 0;

        if ((currentScore > lowestScore) || (HighscoreTable.HighscoreList.Count < HighscoreTable.MaxEntries))
        {
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                bestPlayerName = currentPlayerName;
            }

            // Add new highscore entry if it is in top HighscoreTable.MaxEntries
            if (currentScore > 0)
            {
                HighscoreTable.AddNewEntry(currentPlayerName, currentScore);
            }
            currentScore = 0;
        }
        bestScoreText = "Best score: " + bestPlayerName + " : " + bestScore;
        bestScoreTextUI.text = bestScoreText;
        Debug.Log("Menu enabled. Best score: " + bestPlayerName + " : " + bestScore);
    }

    // Save saveData to a file
    [System.Serializable]
    class SaveData
    {
        // All values of MainMenu we want to save between sessions
        public string currentPlayerName;
        //  public int bestScore;
        //  public string bestPlayerName;
    }

    public void SaveSession()
    {
        SaveData saveData = new SaveData();
        saveData.currentPlayerName = currentPlayerName;
        //  saveData.bestScore = bestScore;
        //  saveData.bestPlayerName = bestPlayerName;

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);

        // SaveHighscores
        HighscoreTable.SaveHighscores();
    }

    public void LoadSession()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            currentPlayerName = saveData.currentPlayerName;

            // Initialize or Load the highscore table
            HighscoreTable.InitializeHighscoreTable();

            //   bestScore = saveData.bestScore;
            //   bestPlayerName = saveData.bestPlayerName;
            if (HighscoreTable.HighscoreList.Count > 0)
            {
                bestScore = HighscoreTable.HighscoreList[0].Score;
                bestPlayerName = HighscoreTable.HighscoreList[0].PlayerName;
            }
            else
            {
                bestScore = 0;
                bestPlayerName = "Player";
            }
        }
    }

}

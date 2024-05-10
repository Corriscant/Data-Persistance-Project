using TMPro;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // to accwss File class

public class MainMenu : MonoBehaviour
{
    public static string currentPlayerName = "Player";  // ���������� ��� �������� ����� ������
    public static int currentScore = 0;  // ���������� ��� �������� �������� ����������
    static int bestScore = 0;  // ���������� ��� �������� ������� ����������
    public static string bestPlayerName = "Player";  // ���������� ��� �������� ����� ������ � ������ �����������
    public static string bestScoreText;  // ���������� ��� �������� ������ ������� ����������
    public TMP_InputField inputField;  // ���������� ��� �������� ������ �� TMP_InputField
    public TextMeshProUGUI bestScoreTextUI;  // ���������� ��� �������� ������ �� TextMeshProUGUI

    // button object for Difficulty settings to mod the difficulty level and show current level with text and color
    public GameObject difficultyButton;

    // ����� ��� �������� �������� ����� ����
    public void StartGame()
    {
        SceneManager.LoadScene("main", LoadSceneMode.Single); // ��������� �������� �����, ������� �� ������������ � Unity
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
        // �������� ������ �� TextMeshProUGUI ������ ������
        var buttonText = difficultyButton.GetComponentInChildren<TextMeshProUGUI>();

        // ������������� ����� � ���� � ����������� �� ������ ���������
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

    // ����� ��� ������ �� ����
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
        // ��� ��� ����������� � ������� ����
        if (SceneManager.GetActiveScene().name != "MainMenu")
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void OnNameEntered()
    {
        currentPlayerName = inputField.text;  // ������ ������ � ������ ���� �����

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
            // ����� ����� ���������� ���, ����������� ����, ��� �� ������ ��������� � Start ��� OnEnable
            HandleReturningToMenu();
        }
    }

    private void HandleReturningToMenu()
    {
        Debug.Log("Handle the logic for returning to the menu");
        // �������� UI ��� ��������� ������ ��������
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

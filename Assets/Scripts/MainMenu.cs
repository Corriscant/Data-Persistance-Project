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

    // ����� ��� �������� �������� ����� ����
    public void StartGame()
    {
        SceneManager.LoadScene("main", LoadSceneMode.Single); // ��������� �������� �����, ������� �� ������������ � Unity
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
        Debug.Log("MainMenu.Update: Current score: " + currentScore);
    }


    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        LoadSession();
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
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            bestPlayerName = currentPlayerName;
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
        public int bestScore;
        public string bestPlayerName;
    }

    public void SaveSession()
    {
        SaveData saveData = new SaveData();
        saveData.currentPlayerName = currentPlayerName;
        saveData.bestScore = bestScore;
        saveData.bestPlayerName = bestPlayerName;

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadSession()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            currentPlayerName = saveData.currentPlayerName;
            bestScore = saveData.bestScore;
            bestPlayerName = saveData.bestPlayerName;
        }
    }

}

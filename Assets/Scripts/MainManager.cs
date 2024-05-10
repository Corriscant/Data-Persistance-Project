using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// class Event to search for End game condition
[Serializable]
public class BrickDestroyedEvent : UnityEvent { }


public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    private float BaseBallSpeed = 1.0f;
    private float BallSpeed;

    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        ScoreText.text = $"{MainMenu.currentPlayerName} Score : {m_Points}"; // Отображаем текущий результат
        BestScoreText.text = MainMenu.bestScoreText; // Отображаем лучший результат

        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                // это когда удар произошел (но потом еще пауза идет для отскока)
                brick.onDestroyed.AddListener(AddPoint);
                // а вот это уже непосредственно уничтожение
                brick.OnActualDestroy.AddListener(SearchForEndGame);
            }
        }


        switch (Settings.difficultyLevel)
        {
            case 1:
                BallSpeed = BaseBallSpeed * 2.0f;
                break;
            case 2:
                BallSpeed = BaseBallSpeed * 3.0f;
                break;
            case 3:
                BallSpeed = BaseBallSpeed * 4.0f;
                break;
            default:
                BallSpeed = BaseBallSpeed;
                break;
        }
        Debug.Log("Difficulty: " + BallSpeed);

    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * BallSpeed, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MainMenu.currentScore = m_Points; // Сохраняем текущий результат
               // Debug.Log("Current score (MainManager): " + MainMenu.currentScore);
                MainMenu.ReturnToMenu(); // Возвращаемся в главное меню
               // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        // Give points by difficulty level
        point *= Settings.difficultyLevel;
        m_Points += point;
        ScoreText.text = $"{MainMenu.currentPlayerName} Score : {m_Points}";  // это интерполяция строк, типа Format в Delphi
    }

    void SearchForEndGame()
    {
        var bricks = FindObjectsOfType<Brick>();
        if (bricks.Length == 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }
}

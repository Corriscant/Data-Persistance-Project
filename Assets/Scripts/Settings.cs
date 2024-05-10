using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    static public int difficultyLevel = 1;  // ”ровень сложности игры

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // метод дл€ установки уровн€ сложности
    private void SetDifficulty(int level)
    {
        difficultyLevel = level;
        MainMenu.ReturnToMenu();
    }

    // ћетод дл€ установки легкого уровн€ сложности
    public void SetEasyDifficulty()
    {
        SetDifficulty(1);
    }

    // ћетод дл€ установки среднего уровн€ сложности
    public void SetMediumDifficulty()
    {
        SetDifficulty(2);
    }

    // метод дл€ установки сложного уровн€ сложности
    public void SetHardDifficulty()
    {
        SetDifficulty(3);
    }

}

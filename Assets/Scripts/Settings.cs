using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    static public int difficultyLevel = 1;  // ������� ��������� ����

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ����� ��� ��������� ������ ���������
    private void SetDifficulty(int level)
    {
        difficultyLevel = level;
        MainMenu.ReturnToMenu();
    }

    // ����� ��� ��������� ������� ������ ���������
    public void SetEasyDifficulty()
    {
        SetDifficulty(1);
    }

    // ����� ��� ��������� �������� ������ ���������
    public void SetMediumDifficulty()
    {
        SetDifficulty(2);
    }

    // ����� ��� ��������� �������� ������ ���������
    public void SetHardDifficulty()
    {
        SetDifficulty(3);
    }

}

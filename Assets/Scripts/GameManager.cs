using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public UnityEvent OnUpdate = new UnityEvent();
    public HealthBox HealthBox;
    public TextMeshProUGUI SpecialCooldownText;
    public TextMeshProUGUI PointsText;
    public TextMeshProUGUI LevelText;
    public GameObject PauseMenu;
    public TextMeshProUGUI FinalScoreText;
    public Canon player;
    public EnemySpawner spawner;
    public UnityEvent OnGameRestart = new UnityEvent();

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    public void LoseGame(int points)
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
        FinalScoreText.text = string.Format("Your score: {0}", points);
    }

    public void RestartGame()
    {
        player.Reset();
        PauseMenu.SetActive(false);
        spawner.Reset();
        OnGameRestart.Invoke();
        Time.timeScale = 1f;
    }

    public void UpdateHealth(float currentHealth)
    {
        HealthBox.UpdateHealth(currentHealth);
    }

    public void SetMaxHealth(float maxHealth)
    {
        HealthBox.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        OnUpdate!.Invoke();
    }

    public void UpdateSpecialCooldown(float currentCooldown, float Cooldown)
    {
        if (currentCooldown > 0)
        {
            SpecialCooldownText.text = string.Format("Special CD: {0} ({1})", (int)currentCooldown, (int)Cooldown);
            return;
        }
        SpecialCooldownText.text = "Special Ready";
    }

    public void UpdatePoints(int points)
    {
        PointsText.text = string.Format("Points: {0}", points);
    }
     public void UpdateLevel(int level)
    {
        LevelText.text = string.Format("Level: {0}", level);
    }
}

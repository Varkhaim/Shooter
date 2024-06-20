using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBox : MonoBehaviour
{
    [SerializeField] private Image healthCircle;
    [SerializeField] private TextMeshProUGUI healthText;

    private float maxHealth = 1f;
    private float currentHealth = 1f;

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        RefreshUI();
    }

    public void UpdateHealth(float health)
    {
        currentHealth = health;
        RefreshUI();
    }

    private void RefreshUI()
    {
        float percentage = currentHealth / maxHealth;
        healthCircle.fillAmount = percentage;
        healthText.text = string.Format("{0}/{1}", currentHealth, maxHealth);
    }
}

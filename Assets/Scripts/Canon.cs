using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class Canon : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform canonParent;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 0.01f;
    [SerializeField] private float minPositionX = -1f;
    [SerializeField] private float maxPositionX = 1f;

    [Header("Attack")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject mainAbilityPrefab;
    [SerializeField] private GameObject specialAbilityPrefab;
    private Ability mainAbility;
    private Ability specialAbility;

    [Header("Experience")]
    private int currentExperience = 0;
    [SerializeField] private int experiencePerLevel = 10;
    private int level = 1;

    [Header("Health")]
    [SerializeField] private float maxHealth = 10f;
    private float currentHealth;

    [Header("Upgrades")]
    [SerializeField] private List<LevelUpConfig> levelUpConfigs;
    private int damageIncrease = 0;
    private int cooldownReduction = 0;

    private Vector2 movementVector = Vector2.zero;
    private int Points = 0;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        mainAbility = new BasicShot(mainAbilityPrefab, this);
        specialAbility = new ExplosiveShot(specialAbilityPrefab, this);
        currentHealth = maxHealth;
        GameManager.Instance.SetMaxHealth(maxHealth);
        GameManager.Instance.UpdateHealth(currentHealth);
    }

    private void Update()
    {
        if (movementVector.x < 0f) MoveLeft();
        if (movementVector.x > 0f) MoveRight();
    }

    public void Move(InputAction.CallbackContext context)
    {
        movementVector = context.ReadValue<Vector2>();
    }

    public void BasicShot(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        mainAbility.OnAbilityUse(spawnPoint.position, canonParent.forward);
    }

    public void SpecialShot(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        specialAbility.OnAbilityUse(spawnPoint.position, canonParent.forward);
    }

    private void MoveLeft()
    {
        if (canonParent.transform.position.x - movementSpeed > minPositionX)
            canonParent.Translate(canonParent.right * -movementSpeed);

    }

    private void MoveRight()
    {
        if (canonParent.transform.position.x + movementSpeed < maxPositionX)
            canonParent.Translate(canonParent.right * movementSpeed);
    }

    private void CheckLevelUpRewards()
    {
        if (!levelUpConfigs.Exists(x => x.level == level)) return;

        LevelUpConfig levelUpConfig = levelUpConfigs.Find(x => x.level == level);
        damageIncrease += levelUpConfig.damageIncrease;
        cooldownReduction += levelUpConfig.cooldownReduction;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        GameManager.Instance.UpdateHealth(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameManager.Instance.LoseGame(Points);
        }
    }

    public int GetDamageIncrease()
    {
        return damageIncrease;
    }

    public int GetCooldownReduction()
    {
        return cooldownReduction;
    }

    public void AwardExperience(int experienceReward)
    {
        currentExperience += experienceReward;
        if (currentExperience >= experiencePerLevel)
        {
            currentExperience -= experiencePerLevel;
            level++;
            CheckLevelUpRewards();
            GameManager.Instance.UpdateLevel(level);
        }
    }

    public void AwardPoints(int pointsReward)
    {
        Points += pointsReward;
        GameManager.Instance.UpdatePoints(Points);
    }

    public void Reset()
    {
        currentExperience = 0;
        level = 1;
        damageIncrease = 0;
        cooldownReduction = 0;
        Points = 0;
        currentHealth = maxHealth;
        GameManager.Instance.UpdatePoints(Points);
        GameManager.Instance.UpdateHealth(currentHealth);
        specialAbility.Init();
    }
}

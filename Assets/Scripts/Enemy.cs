using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Jobs;
using UnityEngine.Jobs;
using System.ComponentModel;
using UnityEngine.UIElements;
using Unity.Collections;

public abstract class Enemy : MonoBehaviour
{
    protected float CurrentHealth;
    protected float MaxHealth;
    protected float BaseMovementSpeed;
    protected float MovementSpeed;
    protected float BonusMovementSpeed = 0f;
    protected int experienceReward;
    protected int pointsReward;
    protected float baseDamage;

    public UnityEvent OnBeingHit;
    public UnityEvent OnDeath;
    public UnityEvent OnReachingPlayer;

    [SerializeField] private Renderer renderer;
    protected float finishZ;
    protected EnemySpawner spawner;


    public enum EnemyType
    {
        SMALL_CUBE,
        BIG_CUBE,
        SMALL_BALL,
        BIG_BALL
    }

    public virtual void Init(float finishZ, EnemySpawner spawner, float baseMovementSpeed)
    {
        CurrentHealth = MaxHealth;
        Color randomColor = GetRandomColor();
        renderer.material.SetColor("_Color", randomColor);
        BonusMovementSpeed = 0;
        this.finishZ = finishZ;
        this.spawner = spawner;
        GameManager.Instance.OnGameRestart.AddListener(Reset);
    }

    private Color GetRandomColor()
    {
        return new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), 1f);
    }

    public virtual void TakeDamage(int amount, Canon source)
    {
        OnBeingHit?.Invoke();
        CurrentHealth -= amount;
        if (CurrentHealth < 0f)
        {
            source.AwardExperience(experienceReward);
            source.AwardPoints(pointsReward);
            Death();
        }
    }

    public float GetDamageValue()
    {
        return baseDamage;
    }

    public virtual void Death()
    {
        OnDeath?.Invoke();
        DestroyObject();
    }

    public virtual void ReachPlayer()
    {
        OnReachingPlayer?.Invoke();
        DestroyObject();
    }

    private void Update()
    {

        NativeArray<Vector3> positions = new NativeArray<Vector3>(1, Allocator.Persistent);

        EnemyMovementJob job = new EnemyMovementJob()
        {
            positions = positions,
            MovementSpeed = MovementSpeed,
            BonusMovementSpeed = BonusMovementSpeed,
            Position = transform.position,
            DeltaTime = Time.deltaTime
        };

        JobHandle jobHandle = job.Schedule();

        jobHandle.Complete();

        transform.position = job.positions[0];

        positions.Dispose();

        if (transform.position.z < finishZ)
        {
            ReachPlayer();
        }
    }

    private void DestroyObject()
    {
        OnBeingHit.RemoveAllListeners();
        OnDeath.RemoveAllListeners();
        OnReachingPlayer.RemoveAllListeners();
        gameObject.SetActive(false);
    }

    public void HealMaxHP(float percentage)
    {
        CurrentHealth += MaxHealth * percentage;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        Debug.Log("Healed percentage HP");
    }

    public void HealWoundedToFull()
    {
        if (CurrentHealth < MaxHealth * 0.5f)
        CurrentHealth = MaxHealth;
        Debug.Log("Healed to Full HP");
    }

    public void IncreaseMovementSpeed(float percentage)
    {
        BonusMovementSpeed += percentage * MovementSpeed;
        Debug.Log("Speed increased");
    }

    public void DecreaseMovementSpeed(float percentage)
    {
        MovementSpeed -= percentage * MovementSpeed;
        Debug.Log("Speed decreased");
    }

    public void Reset()
    {
        OnBeingHit.RemoveAllListeners();
        OnDeath.RemoveAllListeners();
        OnReachingPlayer.RemoveAllListeners();
        BonusMovementSpeed = 0f;
        MovementSpeed = BaseMovementSpeed;
        gameObject.SetActive(false);
    }
}

public struct EnemyMovementJob : IJob
{
    public NativeArray<Vector3> positions;

    public float MovementSpeed;
    public float BonusMovementSpeed;
    public Vector3 Position;
    public float DeltaTime;

    public void Execute()
    {
        positions[0] = new Vector3(Position.x, Position.y, Position.z - (MovementSpeed + BonusMovementSpeed) * DeltaTime);
    }
}

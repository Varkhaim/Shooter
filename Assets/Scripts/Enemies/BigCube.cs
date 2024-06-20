using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCube : Enemy
{
    public override void Init(float finishZ, EnemySpawner spawner, float baseMovementSpeed)
    {
        MaxHealth = 20f;
        MovementSpeed = BaseMovementSpeed = baseMovementSpeed * 0.8f;
        experienceReward = 1;
        pointsReward = 2;
        baseDamage = 1f;

        base.Init(finishZ, spawner, baseMovementSpeed);
    }
}

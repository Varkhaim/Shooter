using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCube : Enemy
{
    public override void Init(float finishZ, EnemySpawner spawner, float baseMovementSpeed)
    {
        MaxHealth = 10f;
        MovementSpeed = BaseMovementSpeed = baseMovementSpeed;
        experienceReward = 1;
        pointsReward = 1;
        baseDamage = 1f;

        base.Init(finishZ, spawner, baseMovementSpeed);
    }
}

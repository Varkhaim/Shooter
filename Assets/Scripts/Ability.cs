using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    public abstract void Init();
    public abstract void OnAbilityUse(Vector3 spawnPosition, Vector3 shotDirection);
}

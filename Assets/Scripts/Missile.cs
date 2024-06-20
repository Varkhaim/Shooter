using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Missile : MonoBehaviour
{
    protected float bulletSpeed;
    protected Vector3 movementDirection;

    public abstract void Init(float bulletSpeed, Vector3 movementDirection, int bulletDamage, Canon owner);
    protected abstract void Movement();

    protected void Update()
    {
        Movement();
    }
}

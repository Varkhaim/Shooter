using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveShot : Ability
{
    private GameObject missilePrefab;
    private float missileSpeed = 1f;
    private int missileDamage = 5;
    private float currentCooldown = 0f;
    private float Cooldown = 5f;
    private float explosionRadius = 2f;
    private GameObject shotMissile = null;
    private Canon owner;

    public ExplosiveShot(GameObject missilePrefab, Canon owner)
    {
        this.missilePrefab = missilePrefab;
        this.owner = owner;
    }

    public override void Init()
    {
        currentCooldown = 0f;
        GameManager.Instance.UpdateSpecialCooldown(currentCooldown, Cooldown);
    }

    public override void OnAbilityUse(Vector3 spawnPosition, Vector3 shotDirection)
    {
        if (shotMissile != null && shotMissile.activeSelf)
        {
            DetonateMissile();
            return;
        }
        if (currentCooldown > 0f)
            return;
        SpawnMissile(spawnPosition, shotDirection);

    }

    private void DetonateMissile()
    {
        Collider[] colliders = Physics.OverlapSphere(shotMissile.transform.position, explosionRadius, LayerMask.NameToLayer("Enemy"), QueryTriggerInteraction.UseGlobal);
        foreach (Collider collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (!enemy) continue;
            enemy.TakeDamage(missileDamage + owner.GetDamageIncrease(), owner);
        }
        shotMissile.SetActive(false);
    }

    private void SpawnMissile(Vector3 spawnPosition, Vector3 shotDirection)
    {
        if (shotMissile == null)
        {
            shotMissile = GameObject.Instantiate(missilePrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            shotMissile.SetActive(true);
            shotMissile.transform.position = spawnPosition;
        }
        Missile bulletScript = shotMissile.GetComponent<Missile>();
        bulletScript.Init(missileSpeed, shotDirection, missileDamage, owner);
        currentCooldown = Cooldown - owner.GetCooldownReduction();
        GameManager.Instance.OnUpdate.AddListener(UpdateCooldown);
    }

    private void UpdateCooldown()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown < 0f)
        {
            currentCooldown = 0f;
            GameManager.Instance.OnUpdate.RemoveListener(UpdateCooldown);
        }
        GameManager.Instance.UpdateSpecialCooldown(currentCooldown, Cooldown - owner.GetCooldownReduction());
    }
}

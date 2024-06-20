using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShot : Ability
{
    private GameObject missilePrefab;
    private float missileSpeed = 1f;
    private int missileDamage = 4;
    private Canon owner;



    private List<GameObject> missiles = new List<GameObject>();

    public BasicShot(GameObject missilePrefab, Canon owner)
    {
        this.missilePrefab = missilePrefab;
        this.owner = owner;
    }

    public override void Init()
    {

    }

    public override void OnAbilityUse(Vector3 spawnPosition, Vector3 shotDirection)
    {
        var bullet = GetMissile();
        bullet.transform.position = spawnPosition;
        Missile bulletScript = bullet.GetComponent<Missile>();
        bulletScript.Init(missileSpeed, shotDirection, missileDamage+owner.GetDamageIncrease(), owner);
    }
    private GameObject GetMissile()
    {
        GameObject inactiveObject = missiles.Find(x => !x.activeSelf);
        if (inactiveObject)
        {
            inactiveObject.SetActive(true);
            return inactiveObject;
        }
        GameObject gameObject = GameObject.Instantiate(missilePrefab);
        missiles.Add(gameObject);
        return gameObject;
    }
}

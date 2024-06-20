using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class BasicBullet : Missile
{
    private int missileDamage;
    private Canon owner;
    private NativeArray<Vector3> positions;

    private void Awake()
    {
        positions = new NativeArray<Vector3>(1, Allocator.Persistent);
    }

    public override void Init(float bulletSpeed, Vector3 movementDirection, int missileDamage, Canon owner)
    {
        this.bulletSpeed = bulletSpeed;
        this.movementDirection = movementDirection;
        this.missileDamage = missileDamage;
        this.owner = owner;
        GameManager.Instance.OnGameRestart.AddListener(DeactivateObject);
    }

    protected override void Movement()
    {
        BulletMovementJob job = new BulletMovementJob()
        {
            positions = positions,
            bulletSpeed = bulletSpeed,
            Position = transform.position,
            DeltaTime = Time.deltaTime
        };

        JobHandle jobHandle = job.Schedule();

        jobHandle.Complete();

        transform.position = job.positions[0];

        if (transform.position.z > 0)
        {
            gameObject.SetActive(false); 
            GameManager.Instance.OnGameRestart.RemoveListener(DeactivateObject);
        }
    }

    private void DeactivateObject()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedGameObject = collision.gameObject;
        if (collidedGameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
            GameManager.Instance.OnGameRestart.RemoveListener(DeactivateObject);
            collidedGameObject.GetComponent<Enemy>().TakeDamage(missileDamage, owner);
        }
    }

    public struct BulletMovementJob : IJob
    {
        public NativeArray<Vector3> positions;

        public float bulletSpeed;
        public Vector3 Position;
        public float DeltaTime;

        public void Execute()
        {
            positions[0] = new Vector3(Position.x, Position.y, Position.z + bulletSpeed * DeltaTime);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Shooting : MonoBehaviour
{
    public List <Transform> bulletSpawnPointList;
    public List<GameObject> barrels;
    public Bullet bulletPrefab;
    public MovingForward movingForward;
    public TurretSpawner spawner;
    public Vector3 origScale;

    public float coolDown;
    [SerializeField] private float _clickTimeReduceAmount;

    public int level;
    public int recoilSize;

    private float lastShot;
    private int currBarrel = 0;

    private void Start()
    {
        origScale = barrels[0].transform.localScale;
    }
    public void SetSpawner(TurretSpawner spawner)
    {
        this.spawner = spawner;
        movingForward = spawner.movingForward;
    }

    private void Update()
    {
        if (spawner.isMerging == true)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }

        CoolDown();
    }

    private void TryShoot()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if(movingForward.isMoving == true)
        {
            return;
        }

        ReduceLastShotTime(_clickTimeReduceAmount);
    }

    private void CoolDown()
    {
        if (Time.time - lastShot < coolDown)
        {
            return;
        }

        Shoot();

        lastShot = Time.time;
    }

    private void ReduceLastShotTime(float amount)
    {
        lastShot -= amount;
    }

    private void Shoot()
    {
        barrels[currBarrel].transform.localScale = origScale;
        var bullet = Instantiate(bulletPrefab, bulletSpawnPointList[currBarrel].position, bulletSpawnPointList[currBarrel].rotation);
        bullet.gameObject.transform.Rotate(bulletSpawnPointList[currBarrel].rotation.x + 90, bulletSpawnPointList[currBarrel].rotation.y, bulletSpawnPointList[currBarrel].rotation.z);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPointList[currBarrel].forward * bulletPrefab.bulletSpeed;

        barrels[currBarrel].transform.DOScale(new Vector3(barrels[currBarrel].transform.localScale.x + recoilSize,
                                                          barrels[currBarrel].transform.localScale.y + recoilSize,
                                                          barrels[currBarrel].transform.localScale.z + recoilSize), 0.1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo);

        currBarrel = (currBarrel + 1) % bulletSpawnPointList.Count;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float bulletSpeed = 25f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float shotCooldown = 0.25f;

    private float nextShotTime;

    private void Update()
    {
        if(Input.GetButtonDown("Fire1") && Time.time >= nextShotTime)
        {
            Shoot(); 
        }

    }

    private void Shoot()
    {
        nextShotTime = Time.time + shotCooldown;
        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.Launch(bulletSpeed, damage);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public ParticleSystem particle;

    public float bulletSpeed = 100f;    
    public float bulletDamage = 10;

    private void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Wall>(out Wall enemyComponent))
        {
            enemyComponent.TakeDamage(bulletDamage);

            MeshRenderer wallRenderer = other.gameObject.GetComponent<MeshRenderer>();

            wallRenderer.enabled = false;

            enemyComponent.CoinSpawn(transform.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shatter"))
        {
            Rigidbody shatterRb = collision.gameObject.GetComponent<Rigidbody>();
            shatterRb.isKinematic = false;
            shatterRb.AddExplosionForce(700, new Vector3(shatterRb.transform.position.x,
                                                         shatterRb.transform.position.y,
                                                         shatterRb.transform.position.z + 1), 2f);

            Renderer wallMaterial = collision.gameObject.GetComponentInChildren<MeshRenderer>();

            ParticleSystem spawnedParticle = Instantiate(particle, transform.position, particle.transform.rotation);
            ParticleSystemRenderer particleSystemRenderer = spawnedParticle.GetComponent<ParticleSystemRenderer>();
            particleSystemRenderer.material.color = wallMaterial.material.color;

            //StartCoroutine(DestroyShatter(collision.gameObject));

            Destroy(gameObject);
        }
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }

    IEnumerator DestroyShatter(GameObject shatter)
    {   
        gameObject.transform.DOScale(new Vector3(0, 0, 0), 0);

        yield return new WaitForSeconds(2);

        shatter.transform.DOScale(new Vector3(0, 0, 0), 0.2f);

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }
}

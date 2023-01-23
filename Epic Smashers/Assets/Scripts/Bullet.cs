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
        StartCoroutine(Destroy());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<Wall>(out Wall enemyComponent))
        {
            enemyComponent.TakeDamage(bulletDamage);

            enemyComponent.CoinSpawn(transform.position);
           
            Renderer wallMaterial = collision.gameObject.GetComponentInChildren<MeshRenderer>();
            Transform wallTransform = collision.gameObject.GetComponent<Transform>();

            Vector3 wallOrigScale = wallTransform.localScale;
            Vector3 wallScaleTo = new Vector3(wallOrigScale.x - 0.02f, wallOrigScale.y, wallOrigScale.z - 0.02f);

            wallTransform.DOScale(wallScaleTo, 0.1f)
                .SetEase(Ease.InOutBounce);

            ParticleSystem spawnedParticle = Instantiate(particle, transform.position, particle.transform.rotation);

            ParticleSystemRenderer particleSystemRenderer = spawnedParticle.GetComponent<ParticleSystemRenderer>();

            particleSystemRenderer.material.color = wallMaterial.material.color;
            

        }

        Destroy(gameObject);
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }
}

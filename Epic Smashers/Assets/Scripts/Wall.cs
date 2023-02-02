using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Wall : MonoBehaviour
{
    [SerializeField] private Collider wallMainCollider;   
    [SerializeField] private Rigidbody[] shattersRgBodies;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private TurretSpawner spawner;
    [SerializeField] private Coin coin;

    [SerializeField] private Vector3 _originalScale;
    [SerializeField] private Vector3 _scaleTo;

    public float health, maxHealth = 20f;
    public float coinAmount = 10;

    private void Start()
    {
        health = maxHealth;
        wallMainCollider = GetComponent<Collider>();
        shattersRgBodies = GetComponentsInChildren<Rigidbody>();

        spawner = FindObjectOfType<TurretSpawner>();

        _originalScale = spawner.scoreText.transform.localScale;
        _scaleTo = _originalScale * 1.2f;
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if(health <= 0)
        {
            wallMainCollider.enabled = false;

            for(int i = 0; i < shattersRgBodies.Length; i++)
            {
                shattersRgBodies[i].isKinematic = false;
                shattersRgBodies[i].AddExplosionForce(700, transform.position, 5f);
                StartCoroutine(destroyDelay(shattersRgBodies[i].gameObject));
            }

            for(int i = 0; i < coinAmount; i++)
            {
                CoinSpawn(transform.position);
            }
        }
    }

    public void CoinSpawn(Vector3 bulletPos)
    {   
        Instantiate(coinPrefab, bulletPos, Quaternion.identity);

        spawner.score += coin.value;
        PlayerPrefs.SetInt("Score", spawner.score);

        spawner.scoreText.transform.DOScale(_scaleTo, 0.1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                spawner.scoreText.transform.DOScale(_originalScale, 0.1f)
                .SetEase(Ease.OutBounce);
            });
    }

    IEnumerator destroyDelay(GameObject shatter)
    {
        yield return new WaitForSeconds(Random.Range(1, 3));

        shatter.transform.DOScale(new Vector3(0,0,0), 0.5f)
            .SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(5f);

        Destroy(shatter);
    }

}

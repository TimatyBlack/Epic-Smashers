using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    public int value;
    private Camera targetCamera;
    [SerializeField] private bool _ignoreY;
    [SerializeField] private bool _revertForward;

    private void Awake()
    {
        targetCamera = Camera.main;
        value = PlayerPrefs.GetInt("Value");
    }

    private void LateUpdate()
    {
        Vector3 targetDirection = targetCamera.transform.position - transform.position;
        if (_ignoreY)
            targetDirection.y = 0;
        if (_revertForward)
            targetDirection = -targetDirection;
        Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = lookRotation;
    }

    void Start()
    {
        if (value == 0)
        {
            value = 1;
        }

        StartCoroutine(DestroyDelay());
    }

    public void Income()
    {
        value++;
        PlayerPrefs.SetInt("Value", value);
    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(3f);

        transform.DOScale(new Vector3(0, 0, 0), 0.5f);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}

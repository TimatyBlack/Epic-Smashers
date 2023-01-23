using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonDelayer : MonoBehaviour
{
    [SerializeField] private float _delay = 1;
    private Button _button;
    private float _currDelay;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(StartDelay);
    }

    private IEnumerator DelayCountDown()
    {   
        while(_currDelay > 0)
        {
            _currDelay -= Time.deltaTime;

            yield return null;
        }

        _button.interactable = true;
    }

    public void StartDelay()
    {   
        _currDelay = _delay;

        _button.interactable = false;

        StartCoroutine(DelayCountDown());
    }
}

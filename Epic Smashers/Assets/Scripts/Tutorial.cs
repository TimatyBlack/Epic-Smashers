using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    public Image finger;
    public Image fingerClicked;
    public Coroutine startTutorial;
    public TurretSpawner spawner;

    private bool runOnceStart = false;
    private bool runOnceShoot;
    private bool runOnceMerge;

    private void Start()
    {
        runOnceShoot = false;
        runOnceMerge = false;

        runOnceStart = PlayerPrefs.GetInt("Tutorial_0", 0) == 1;
        runOnceShoot = PlayerPrefs.GetInt("Tutorial_1", 0) == 1;
        runOnceMerge = PlayerPrefs.GetInt("Tutorial_2", 0) == 1;

        finger.enabled = false;
        fingerClicked.enabled = false;

        if (runOnceShoot == false)
        {
            startTutorial = StartCoroutine(StartTutorialAnimation());
        }
    }
    
    private void Update()
    {
        if(spawner.score >= 100 && spawner.currTurretList.Count >= 3 && runOnceMerge == false)
        {   
            StartCoroutine(MergeButtonTutorial());
        }
    }

    IEnumerator StartTutorialAnimation()
    {
        runOnceStart = true;
        PlayerPrefs.SetInt("Tutorial_0", 1);

        finger.enabled = true;
        fingerClicked.enabled = false;

        yield return new WaitForSeconds(1);

        finger.transform.DOLocalMove(new Vector3(-160, -206, 0), 1f)
            .SetEase(Ease.InOutSine);
        finger.transform.DOLocalRotate(new Vector3(0, 0, 165), 1f)
            .SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(1f);

        StartCoroutine(ShowOnAdd());
    }

    public void ClickToShootMethod()
    {   
        if(runOnceShoot == false)
        StartCoroutine(ClickToShootTutorial());
    }

    IEnumerator ClickToShootTutorial()
    {
        runOnceShoot = true;
        PlayerPrefs.SetInt("Tutorial_1", 1);

        StopCoroutine(startTutorial);
        DOTween.CompleteAll();

        finger.enabled = false;
        finger.transform.DOLocalMove(new Vector3(0,0,0), 0f)
            .SetEase(Ease.InOutSine);
        finger.transform.DOLocalRotate(new Vector3(0,0,0), 0f)
            .SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(1);

        finger.enabled = true;

        yield return new WaitForSeconds(0.1f);

        bool isClicked = false;

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);

            finger.enabled = !isClicked;
            fingerClicked.enabled = isClicked;

            isClicked = !isClicked;
        }

        yield return new WaitForSeconds(1);

        finger.enabled = false;
        fingerClicked.enabled = false;
    }

    IEnumerator MergeButtonTutorial()
    {
        runOnceMerge = true;
        PlayerPrefs.SetInt("Tutorial_2", 1);

        finger.enabled = true;
        finger.transform.DOLocalMove(new Vector3(0,-200,0), 0)
            .SetEase(Ease.InOutSine);
        finger.transform.DOLocalRotate(new Vector3(0, 0, 165), 0)
            .SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(0.1f);

        finger.transform.DOLocalMove(new Vector3(0, -185, 0), 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(10, LoopType.Yoyo);

        yield return new WaitForSeconds(5);

        finger.enabled = false;
    }

    IEnumerator ShowOnAdd()
    {
        finger.transform.DOLocalMove(new Vector3(-160, -190, 0), 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(6, LoopType.Yoyo);

        yield return new WaitForSeconds(3);

        finger.enabled = false;
    }
}

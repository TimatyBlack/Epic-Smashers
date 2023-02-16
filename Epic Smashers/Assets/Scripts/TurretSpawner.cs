using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class TurretSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> targetList;
    [SerializeField] private List<Shooting> turretList;
                     public  List<Shooting> currTurretList;
    [SerializeField] private List<Shooting> mergeList;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject circle;
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private Coin coin;
    [SerializeField] private Button addButton;
    [SerializeField] private Button mergeButton;
    [SerializeField] private Button incomeButton;

    [SerializeField] private GameObject addButtonEnabled;
    [SerializeField] private GameObject mergeButtonEnabled;
    [SerializeField] private GameObject incomeButtonEnabled;

    [SerializeField] private ParticleSystem poofPref;

    [SerializeField] private TMP_Text addTurretValueText;
    [SerializeField] private TMP_Text mergeValueText;
    [SerializeField] private TMP_Text incomeValueText;
    [SerializeField] private float radius = 3f;
    [SerializeField] private int addTurretCost = 10;
    [SerializeField] private int mergeCost = 100;
    [SerializeField] private int incomeCost = 150;

    public float rotationSpeed = 0.4f;

    public TMP_Text scoreText;
    public int score = 10;

    private float moveProgress;
    private Vector3 turretScale;

    public MovingForward movingForward;

    public bool isMoving;
    public bool isMerging;

    public event System.Action<int> onTurretsCountChange;

    Vector3 startOffset = new Vector3(0, 5, -10);
    private CinemachineTransposer transposer;

    public int levelToUp;

    private void Awake()
    {
        score = PlayerPrefs.GetInt("Score");
        addTurretCost = PlayerPrefs.GetInt("TurretCost");
        mergeCost = PlayerPrefs.GetInt("MergeCost");
        incomeCost = PlayerPrefs.GetInt("IncomeCost");
    }

    private void Start()
    {   
        if(score == 0)
        {
            score = 10;
        }
        if (addTurretCost == 0)
        {
            addTurretCost = 10;
        }
        if (mergeCost == 0)
        {
            mergeCost = 200;
        }
        if (incomeCost == 0)
        {
            incomeCost = 150;
        }

        //transposer = camera.AddCinemachineComponent<CinemachineTransposer>();

        turretScale = turretList[0].transform.localScale;
        onTurretsCountChange += UpdatePosition;

        List<int> turretsLevel = PlayerPrefsExtra.GetList<int>("CurrentTurretListLevel");

        for (int i = 0; i < turretsLevel.Count; i++)
        {
            AddTurret(turretsLevel[i], false);
        }

        addTurretValueText.text = addTurretCost.ToString();
        mergeValueText.text = mergeCost.ToString();
        incomeValueText.text = incomeCost.ToString();

        scoreText.text = score.ToString();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && rotationSpeed <= 160f)
        {
            rotationSpeed += 20f;
        }
        else if(rotationSpeed > 80f)
        {
            rotationSpeed -= 50f * Time.deltaTime;
        }

        if (currTurretList.Count > 0)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);
        }
        
        if(currTurretList.Count > 8)
        {
            circle.SetActive(true);
        }
        else
        {
            circle.SetActive(false);
        }

        moveTurretToTarget();
        //Camera();

        addTurretValueText.text = addTurretCost.ToString();
        mergeValueText.text = mergeCost.ToString();
        incomeValueText.text = incomeCost.ToString();

        scoreText.text = score.ToString();

        AffordCheck();
    }

    public void AddTurret(int level = 0, bool animate = true)
    {   
        if(currTurretList.Count < 16)
        {
            moveProgress = 0;
            Shooting turret = Instantiate(turretList[level], transform);
            GameObject target = Instantiate(targetPrefab, transform);
            turret.SetSpawner(this);

            if (animate)
            {
                turret.transform.localScale = new Vector3(0, 0, 0);
                turret.transform.DOScale(turretScale, 0.3f)
                    .SetEase(Ease.InOutSine);
            }

            targetList.Add(target);

            currTurretList.Add(turret);
            
            onTurretsCountChange?.Invoke(targetList.Count);
            turret.transform.localPosition = target.transform.localPosition;

            SaveCurrentTurrets();
        }
        
    }

    public void BuyTurret()
    {
        if(score >= addTurretCost)
        {
            AddTurret();
            
            score -= addTurretCost;
            PlayerPrefs.SetInt("Score", score);

            addTurretCost = Mathf.FloorToInt(addTurretCost * 1.5f);
            PlayerPrefs.SetInt("TurretCost", addTurretCost);

        }
    }

    public void BuyMergeTurret()
    {
        MergeCheck();

        if (score >= mergeCost && mergeList.Count >= 3)
        {
            Merge();

            score -= mergeCost;
            PlayerPrefs.SetInt("Score", score);

            mergeCost = Mathf.FloorToInt(mergeCost * 2f);
            PlayerPrefs.SetInt("MergeCost", mergeCost);

        }
    }
    public void IncomeUpgrade()
    {
        if (score >= incomeCost)
        {
            coin.Income();

            score -= incomeCost;
            PlayerPrefs.SetInt("Score", score);

            incomeCost = Mathf.FloorToInt(incomeCost * 2f);
            PlayerPrefs.SetInt("IncomeCost", incomeCost);

        }
    }

    private void RemoveTurret(Shooting turret, GameObject target)
    {
        currTurretList.Remove(turret);
        SaveCurrentTurrets();

        targetList.Remove(target);
        Destroy(turret.gameObject);
        Destroy(target);
    }

    public void UpdatePosition()
    {   
        for (int i = 0; i < currTurretList.Count; i++)
        {
             if (currTurretList.Count < 17)
             {
                    int totalCircles = Mathf.FloorToInt(currTurretList.Count / 8) + 1;
                    int currCircle = Mathf.FloorToInt(i / 8) + 1;
                    int turretsInCircle = currCircle <= totalCircles - 1 ? 8 : currTurretList.Count % 8;
                    Debug.Log($"{i} = {currCircle}");
                    float anglePos = i % 8 * Mathf.PI * 2 / turretsInCircle;
                    float x = Mathf.Cos(anglePos) * radius / (Mathf.CeilToInt(i / 8) + 1);
                    float y = Mathf.Sin(anglePos) * radius / (Mathf.CeilToInt(i / 8) + 1);
                    Vector3 pos = new Vector3(x, y, 0);
                    float angleDegrees = anglePos * Mathf.Rad2Deg;
                    Quaternion rot = Quaternion.Euler(0, 0, angleDegrees - 90);
                    targetList[i].transform.localPosition = pos;
                    targetList[i].transform.localRotation = rot;
             }
        }
        
    }

    private void moveTurretToTarget()
    {
        if (isMerging == false)
        {
            for (int i = 0; i < currTurretList.Count; i++)
            {
                currTurretList[i].transform.localPosition = Vector3.Lerp(currTurretList[i].transform.localPosition, targetList[i].transform.localPosition, moveProgress);
                currTurretList[i].transform.localRotation = Quaternion.Lerp(currTurretList[i].transform.localRotation, targetList[i].transform.localRotation, moveProgress);
            }

            moveProgress += Time.deltaTime;
        }
    }



    private void UpdatePosition(int _)
    {
       
        UpdatePosition();
        
    }

    public bool MergeCheck()
    {
        mergeList.Clear();

        for (int i = 0; i < currTurretList.Count; i++)
        {
            mergeList.Clear();
            mergeList.Add(currTurretList[i]);

            for (int k = i+1; k < currTurretList.Count; k++)
            {   
                if (currTurretList[i].level == currTurretList[k].level)
                {
                    mergeList.Add(currTurretList[k]);

                    if (mergeList.Count >= 3)
                    {
                        return true;
                    }
                }
            }

        }

        return false;
    }

    public void Camera()
    {
        if (currTurretList.Count > 8)
        {
            transposer.m_FollowOffset = new Vector3(0, startOffset.y + 1, startOffset.z - 3f);
        }
        else
        {
            transposer.m_FollowOffset = startOffset;
        }
    }

    public void Merge()
    {   
        MergeCheck();

        if(mergeList.Count >= 3)
        {
            levelToUp = mergeList[0].level;

            if (mergeList.Count >= 3)
            {
                MergeAnimation();

                StartCoroutine(AddTurretTimeDelay());
            }
        }
    }

    public void MergeAnimation()
    {
        isMerging = true;

        StartCoroutine(MergingTimeDelays());
    }

    public void SaveCurrentTurrets()
    {
        List<int> turretsLevel = new List<int>();

        for (int i = 0; i < currTurretList.Count; i++)
        {
            turretsLevel.Add(currTurretList[i].level);
        }

        PlayerPrefsExtra.SetList<int>("CurrentTurretListLevel", turretsLevel);
    }

    IEnumerator MergingTimeDelays()
    {
        mergeList[0].gameObject.transform.DOMove(new Vector3(-1f, 3, mergeList[0].transform.position.z), 1f)
            .SetEase(Ease.InOutSine);
        mergeList[1].gameObject.transform.DOMove(new Vector3(0f, 3, mergeList[1].transform.position.z), 1f)
            .SetEase(Ease.InOutSine);
        mergeList[2].gameObject.transform.DOMove(new Vector3(1f, 3, mergeList[2].transform.position.z), 1f)
            .SetEase(Ease.InOutSine);

        mergeList[0].gameObject.transform.DORotate(new Vector3(0, 180, 0), 1f)
            .SetEase(Ease.InOutSine);
        mergeList[1].gameObject.transform.DORotate(new Vector3(0, 180, 0), 1f)
            .SetEase(Ease.InOutSine);
        mergeList[2].gameObject.transform.DORotate(new Vector3(0, 180, 0), 1f)
            .SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(1);

        mergeList[0].gameObject.transform.DOMove(new Vector3(0f, 3, mergeList[1].transform.position.z), 0.3f)
            .SetEase(Ease.InOutSine);
        mergeList[1].gameObject.transform.DOMove(new Vector3(0f, 3, mergeList[1].transform.position.z), 0.3f)
            .SetEase(Ease.InOutSine);
        mergeList[2].gameObject.transform.DOMove(new Vector3(0f, 3, mergeList[1].transform.position.z), 0.3f)
            .SetEase(Ease.InOutSine);

        mergeList[0].gameObject.transform.DORotate(new Vector3(0, 1080, 0), 0.3f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine);
        mergeList[1].gameObject.transform.DORotate(new Vector3(0, 1080, 0), 0.3f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine);
        mergeList[2].gameObject.transform.DORotate(new Vector3(0, 1080, 0), 0.3f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(0.3f);

        mergeList[0].gameObject.SetActive(false);
        mergeList[1].gameObject.SetActive(false);
        mergeList[2].gameObject.SetActive(false);

        Instantiate(poofPref, new Vector3(mergeList[0].gameObject.transform.position.x, 
                                          mergeList[0].gameObject.transform.position.y + 0.4f, 
                                          mergeList[0].gameObject.transform.position.z - 2), mergeList[0].transform.rotation);

        GameObject newTurret = Instantiate(turretList[mergeList[0].level + 1].gameObject, mergeList[0].transform.position, mergeList[0].transform.rotation);
        newTurret.GetComponent<Shooting>().enabled = false;

        yield return new WaitForSeconds(1);

        newTurret.transform.DOScale(new Vector3(0, 0, 0), 0.1f)
            .SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(1);

        Destroy(newTurret.gameObject);
    }
    
    IEnumerator AddTurretTimeDelay()
    {
        yield return new WaitForSeconds(2.3f);

        for (int i = 0; i < 3; i++)
        {
            RemoveTurret(mergeList[i], targetList[0]);
        }

        isMerging = false;

        Debug.Log("Add");
        AddTurret(levelToUp + 1);

        addButton.interactable = true;
        addButtonEnabled.SetActive(false);

        incomeButton.interactable = true;
        incomeButtonEnabled.SetActive(false);
    }

    private void AffordCheck()
    {
        if (score >= addTurretCost && isMerging == false)
        {
            addButton.interactable = true;
            addButtonEnabled.SetActive(false);
        }
        else
        {
            addButton.interactable = false;
            addButtonEnabled.SetActive(true);
        }

        if (score >= mergeCost && MergeCheck() == true && isMerging == false)
        {
            mergeButton.interactable = true;
            mergeButtonEnabled.SetActive(false);
        }
        else
        {
            mergeButton.interactable = false;
            mergeButtonEnabled.SetActive(true);
        }

        if (score >= incomeCost && isMerging == false)
        {
            incomeButton.interactable = true;
            incomeButtonEnabled.SetActive(false);
        }
        else
        {
            incomeButton.interactable = false;
            incomeButtonEnabled.SetActive(true);
        }
    }
}

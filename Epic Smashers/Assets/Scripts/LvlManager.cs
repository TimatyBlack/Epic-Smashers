using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LvlManager : MonoBehaviour
{
    public List<GameObject> levels;
    public List<GameObject> gradients;
    public GameObject currentLevel;
    public MovingForward player;
    public GameObject nextLevelMenu;
    public Image transition;
    public CanvasGroup buttons;
    public CanvasGroup upValuesImages;
    public Button nextLevelButton;

    public int currLvlIndex = 0;

    private void Start()
    {
        currLvlIndex = PlayerPrefs.GetInt("Level");
        gradients[currLvlIndex].SetActive(true);
        currentLevel = Instantiate(levels[currLvlIndex]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void NextLevel()
    {
        nextLevelButton.interactable = false;
        StartCoroutine(NextLevelAnimation());
    }

    IEnumerator NextLevelAnimation()
    {   
        transition.transform.DOLocalMove(new Vector3(-1500, 0, 0), 0);

        yield return new WaitForSeconds(0.1f);

        transition.transform.DOLocalMove(new Vector3(1500, 0, 0), 2)
            .SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(1f);

        buttons.alpha = 1;
        upValuesImages.alpha = 1;

        player.transform.position = new Vector3(0, 0, -1.8f);

        nextLevelMenu.SetActive(false);

        player.doOnce = false;

        Destroy(currentLevel);
        
        if(currLvlIndex >= 4)
        {
            currLvlIndex = -1;
        }

        currentLevel = Instantiate(levels[currLvlIndex + 1]);

        currLvlIndex++;
        gradients[currLvlIndex].SetActive(true);

        PlayerPrefs.SetInt("Level", currLvlIndex);
    }
}

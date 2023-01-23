using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingForward : MonoBehaviour
{
    [SerializeField] private float d_MaxDistance;
    [SerializeField] private LayerMask wallMask;

    private bool d_HitDetected;
    private RaycastHit d_HitInfo;

    private Collider d_Collider;
    private Transform d_Transform;

    public CanvasGroup nextLevelMenu;
    public CanvasGroup buttons;
    public CanvasGroup upValuesImages;

    public GameObject textIncredible;
    public GameObject nextButton;

    public bool isMoving;
    public bool doOnce;

    private void Start()
    {
        doOnce = false;
        d_Transform = GetComponent<Transform>();
        d_Collider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        d_HitDetected = Physics.BoxCast(d_Collider.transform.position, d_Collider.transform.localScale, d_Transform.forward, out d_HitInfo, d_Transform.rotation, d_MaxDistance, wallMask);
        if (d_HitDetected)
        {
            isMoving = false;

            if (d_HitInfo.collider.gameObject.tag == "Finish" && doOnce == false)
            {
                doOnce = true;

                StartCoroutine(NextLvlMenuOn());
            }
        }
        else
        {
            transform.position += transform.forward * 0.05f;
            isMoving = true;
        }
    }

    IEnumerator NextLvlMenuOn()
    {
        nextLevelMenu.gameObject.SetActive(true);
        nextLevelMenu.alpha = 0;
        textIncredible.transform.localPosition = new Vector3(-500, 280, 0);
        nextButton.transform.localScale = new Vector3(0, 0, 0);
        nextButton.transform.DORotate(new Vector3(0, 0, 3), 0);

        buttons.DOFade(0, 0.5f)
            .SetEase(Ease.InOutSine);
        
        upValuesImages.DOFade(0, 0.5f)
            .SetEase(Ease.InOutSine);
        
        nextLevelMenu.DOFade(1, 0.5f)
            .SetEase(Ease.InOutSine);

        textIncredible.transform.DOLocalMove(new Vector3(0, 280, 0), 0.5f)
            .SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(0.5f);

        nextButton.transform.DOScale(new Vector3(1, 1, 1), 0.5f)
            .SetEase(Ease.InOutSine);

        nextButton.transform.DORotate(new Vector3(0, 0, -3), 0.2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(20, LoopType.Yoyo);

        yield return new WaitForSeconds(4);

        nextButton.transform.DORotate(new Vector3(0, 0, 0), 0.2f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (d_HitDetected)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * d_HitInfo.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * d_HitInfo.distance, transform.localScale);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * d_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * d_MaxDistance, transform.localScale);
        }
    }
}

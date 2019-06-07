using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class InterfaceAnimator : MonoBehaviour
{
    public Canvas canvas;
    public CanvasGroup aim;
    public Image ringSlider;
    public GameObject lockPrefab;

    private void Start()
    {
        aim.alpha = 0;
    }

    public void ShowAim(bool state)
    {
        aim.DOComplete();
        ringSlider.DOComplete();

        float alpha = state ? 1 : 0;
        float fill = state ? 1 : 0;
        float endFill = state ? 0 : 1;
        float time = state ? 8 : 0;

        aim.alpha = alpha;
        ringSlider.fillAmount = fill;
        ringSlider.DOFillAmount(endFill, time);
    }

    public void LockTarget(Transform target)
    {
        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(target.position);

        LockFollowUI[] locks = FindObjectsOfType<LockFollowUI>();

        foreach (LockFollowUI l in locks)
        {
            if (l.target == target)
            {
                l.Animate();
                return;
            }
        }

        GameObject lockIcon = Instantiate(lockPrefab, targetScreenPos, Quaternion.identity, canvas.transform);
        lockIcon.GetComponent<LockFollowUI>().target = target;
    }

}

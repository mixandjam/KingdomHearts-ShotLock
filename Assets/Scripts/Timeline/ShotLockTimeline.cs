using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShotLockTimeline : MonoBehaviour
{

    private ShotLock shotlock;
    private MovementInput movement;

    private void OnEnable()
    {
        shotlock = GetComponentInParent<ShotLock>();
        movement = FindObjectOfType<MovementInput>();
        shotlock.ActivateShotLock();
        movement.enabled = false;

    }

    private void OnDisable()
    {
        shotlock.cinematic = false;
        shotlock.Aim(false);

        movement.transform.DOMoveY(0, .5f).SetEase(Ease.InSine).OnComplete(() => movement.enabled = true);

    }
}

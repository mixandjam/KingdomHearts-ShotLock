using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetection : MonoBehaviour
{
    public ShotLock shotLock;
    private Collider collider;

    [Space]
    [Header("Targets")]
    public List<Transform> targets = new List<Transform>();

    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    public void SetCollider(bool state)
    {
        collider.enabled = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            shotLock.TargetState(other.transform, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            shotLock.TargetState(other.transform, false);

        }
    }
}

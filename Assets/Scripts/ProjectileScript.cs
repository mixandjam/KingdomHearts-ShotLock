using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProjectileScript : MonoBehaviour
{
    public float rotationSpeed = 5;
    public float movementSpeed = 10;
    public float initialWait = 1;
    public bool initial = true;

    public Transform target;
    public GameObject hitParticle;
    private float multiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
        multiplier = Random.Range(1, 3);
        StartCoroutine(InitialWait());
        Destroy(gameObject, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (initial)
        {
            transform.eulerAngles += Vector3.forward * Time.deltaTime * (rotationSpeed/2);
            transform.position += transform.up * Time.deltaTime * (movementSpeed / 8f);
        }
        else
        {
            var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, .5f);
            //transform.LookAt(target.position);
            transform.position += transform.forward * Time.deltaTime * (movementSpeed * multiplier);
            transform.GetChild(0).eulerAngles += Vector3.forward * Time.deltaTime * rotationSpeed * 1.5f;
        }
    }

    IEnumerator InitialWait()
    {
        yield return new WaitForSeconds(initialWait / 2);
        transform.GetChild(0).GetChild(0).DOLocalMoveY(1, .2f);
        yield return new WaitForSeconds(initialWait/2);
        DOVirtual.Float(rotationSpeed, rotationSpeed * 1.5f, .3f, SetRotationSpeed);
        initial = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == target.name)
        {
            Instantiate(hitParticle, transform.position, Quaternion.identity);
            Transform trail = transform.GetChild(0).GetChild(0);
            Destroy(trail.gameObject, .8f);
            trail.parent = null;
            Destroy(gameObject);
        }
    }

    private void SetRotationSpeed(float x)
    {
        rotationSpeed = x;
    }

}

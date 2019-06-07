using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class ShotLock : MonoBehaviour
{

    Animator anim;
    MovementInput input;
    InterfaceAnimator ui;
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;

    public PlayableDirector director;
    public bool cinematic;

    [Header("Targets")]
    public TargetDetection detection;
    public List<Transform> finalTargets = new List<Transform>();

    [Space]
    [Header("Aim and Zoom")]
    public Transform weaponTip;

    [Space]
    [Header("Aim and Zoom")]
    public bool aiming;
    public CinemachineFreeLook thirdPersonCamera;
    public float zoomDuration = .3f;
    private float originalFOV;
    public float zoomFOV;
    private Vector3 originalCameraOffset;
    public Vector3 zoomCameraOffset;

    [Space]
    [Header("Prefab")]
    public GameObject projectilePrefab;

    private float time;
    private int index = 0;
    private int limit = 25;

    void Start()
    {
        Cursor.visible = false;
        postVolume = Camera.main.GetComponentInChildren<PostProcessVolume>();
        postProfile = postVolume.profile;
        ui = GetComponent<InterfaceAnimator>();
        anim = this.GetComponent<Animator>();
        input = GetComponent<MovementInput>();
        originalFOV = thirdPersonCamera.m_Lens.FieldOfView;
        originalCameraOffset = thirdPersonCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

        if (Input.GetMouseButtonDown(1))
        {
            DOVirtual.Float(postProfile.GetSetting<Vignette>().intensity.value, .8f, .2f, SetVignette);
            Aim(true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            DOVirtual.Float(postProfile.GetSetting<Vignette>().intensity.value, 0, .2f, SetVignette);

            if(finalTargets.Count > 0)
            {
                director.Play();
                input.anim.SetFloat("PosY", 1);
                input.enabled = false;
                cinematic = true;
                transform.position += Vector3.up * 3;

                LockFollowUI[] locks = FindObjectsOfType<LockFollowUI>();
                foreach (LockFollowUI l in locks)
                {
                    Destroy(l.gameObject);
                }
            }

            Aim(false);
        }

        if (aiming)
        {
 
            if(time >= 5)
            {
                time = 0;

                List<Transform> oldTargets = new List<Transform>();
                oldTargets = detection.targets;

                if (oldTargets.Count > 0 && finalTargets.Count < limit)
                {
                    if (index < oldTargets.Count)
                    {
                        ui.LockTarget(oldTargets[index]);
                        finalTargets.Add(oldTargets[index]);
                    }

                    index = Mathf.Min(oldTargets.Count - 1, index+1);
                    if(index == oldTargets.Count - 1)
                    {
                        index = 0;
                    }
                }
            }
            else
            {
                time++;
            }
        }
    }

    public void ActivateShotLock()
    {
        for (int i = 0; i < finalTargets.Count; i++)
        {
            float angle = (360 / finalTargets.Count);
            float z = angle * (i+1);
            Vector3 cam = Camera.main.transform.eulerAngles;
            GameObject projectile = Instantiate(projectilePrefab, weaponTip.transform.position, Quaternion.Euler(cam.x, cam.y, z));
            projectile.GetComponent<ProjectileScript>().target = finalTargets[i];
        }

    }

    public void Aim(bool state)
    {
        ui.ShowAim(state);

        if (!state && !cinematic)
        {
            StopAllCoroutines();
            detection.targets.Clear();
            finalTargets.Clear();
            index = 0;
        }

        detection.SetCollider(state);
        aiming = state;
        float fov = state ? zoomFOV : originalFOV;
        Vector3 offset = state ? zoomCameraOffset : originalCameraOffset;
        float stasisEffect = state ? .4f : 0;

        CinemachineComposer composer = thirdPersonCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
        DOVirtual.Float(thirdPersonCamera.m_Lens.FieldOfView, fov, zoomDuration, SetFieldOfView);
        DOVirtual.Float(composer.m_TrackedObjectOffset.x, offset.x, zoomDuration, SetCameraOffsetX);
        DOVirtual.Float(composer.m_TrackedObjectOffset.y, offset.y, zoomDuration, SetCameraOffsetY);
    }

    public void TargetState(Transform target, bool state)
    {
        if (!state && detection.targets.Contains(target))
            detection.targets.Remove(target);

        if (state && !detection.targets.Contains(target))
            detection.targets.Add(target);
    }

    void SetFieldOfView(float x)
    {
        thirdPersonCamera.m_Lens.FieldOfView = x;
    }
    void SetCameraOffsetX(float x)
    {
        for (int i = 0; i < 3; i++)
        {
            thirdPersonCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.x = x;
        }
    }
    void SetCameraOffsetY(float y)
    {
        for (int i = 0; i < 3; i++)
        {
            thirdPersonCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = y;
        }
    }

    void SetVignette(float x)
    {
        postProfile.GetSetting<Vignette>().intensity.value = x;
    }


}

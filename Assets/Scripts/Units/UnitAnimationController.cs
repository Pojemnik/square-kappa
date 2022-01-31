using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class UnitAnimationController : MonoBehaviour
{
    [Header("Refernces")]
    public UnitController owner;
    public AnimationEventsAdapter eventsAdapter;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private PlayerCameraController cameraController;

    [Header("Wall animations settings")]
    [SerializeField]
    private bool useWallAnimations;
    [SerializeField]
    private float weaponMoveSpeed;

    [Header("Weapon change animation parameters")]
    [SerializeField]
    private float weaponChangeDuration;

    private float wallDistanceMultipler;
    private float wallDistanceOffset;

    private HashSet<string> paramaterNames;
    private float currentWallValue;

    private float currentPullValue;
    private bool animateWeaponPull;
    private float currentPullAnimationTime;

    public void SetState(string state)
    {
        if (!paramaterNames.Contains(state))
        {
            return;
        }
        animator.SetTrigger(state);
    }

    public void SetStaticState(string state)
    {
        if (!paramaterNames.Contains(state))
        {
            return;
        }
        animator.SetBool(state, true);
    }

    public void ResetStaticState(string state)
    {
        if (!paramaterNames.Contains(state))
        {
            return;
        }
        animator.SetBool(state, false);
    }

    public void ResetTriggers()
    {
        animator.ResetTrigger("Move");
        animator.ResetTrigger("Stop");
        animator.ResetTrigger("Spotted");
    }

    public void SetRotationVector(Vector2 rotation)
    {
        animator.SetFloat("Xrotation", rotation.x);
        animator.SetFloat("Yrotation", rotation.y);
    }

    public void UpdateWeaponAnimation(WeaponController newWeapon)
    {
        if (newWeapon)
        {
            switch (newWeapon.Config.type)
            {
                case WeaponConfig.WeaponType.Rifle:
                    SetAnimatorLayer("Chemirail");
                    break;
                case WeaponConfig.WeaponType.LaserPistol:
                    SetAnimatorLayer("LaserPistol");
                    break;
                case WeaponConfig.WeaponType.LaserRifle:
                    SetAnimatorLayer("LaserRifle");
                    break;
                case WeaponConfig.WeaponType.Fists:
                    SetAnimatorLayer("Unarmed");
                    break;
                default:
                    SetAnimatorLayer("Unarmed");
                    Debug.LogError("Unkonown weapon type. Used default");
                    break;

            }
        }
        else
        {
            SetAnimatorLayer("Unarmed");
            Debug.LogError("Weapon in null. Animation set to default");
        }
    }

    public void Deactivate()
    {
        animator.enabled = false;
        enabled = false;
    }

    private void SetAnimatorLayer(string name)
    {
        int index = animator.GetLayerIndex(name);
        animator.SetLayerWeight(index, 1);
        for (int i = 1; i < animator.layerCount; i++)
        {
            if (i != index && animator.GetLayerName(i) != "Legs")
            {
                animator.SetLayerWeight(i, 0);
            }
        }
    }

    private void OnEnable()
    {
        AnimatorControllerParameter[] parameters = animator.parameters;
        paramaterNames = new HashSet<string>(parameters.Select(e => e.name));
    }

    private void SetWallParameter(float value)
    {
        animator.SetFloat("Wall", value);
    }

    private void Start()
    {
        if (useWallAnimations)
        {
            cameraController.wallCloseEvent += OnWallClose;
            owner.itemChanger.weaponChangeEvent.AddListener(UpdateWallPullParameters);
            currentWallValue = 0;
        }
        if (owner.itemChanger != null)
        {
            owner.itemChanger.weaponChangeEvent.AddListener(UpdateWeaponAnimation);
            owner.itemChanger.weaponChangeEvent.AddListener((_) => StartPullAnimation());
        }
    }

    private void StartPullAnimation()
    {
        animateWeaponPull = true;
        currentPullAnimationTime = 0;
        currentPullValue = 0;
    }

    private void UpdateWallPullParameters(WeaponController newWeapon)
    {
        wallDistanceOffset = newWeapon.Config.wallDistanceWhenPulled;
        wallDistanceMultipler = 1 / (newWeapon.Config.length - wallDistanceOffset);
    }

    private void OnWallClose(object sender, float distance)
    {
        float target;
        if (distance == -1)
        {
            target = 0;
        }
        else
        {
            target = Mathf.Clamp01(1 - (distance - wallDistanceOffset) * wallDistanceMultipler);
        }
        currentWallValue = Mathf.MoveTowards(currentWallValue, target, weaponMoveSpeed);
        SetWallParameter(Mathf.Max(currentWallValue, currentPullValue));
    }

    private void Update()
    {
        if (animateWeaponPull)
        {
            if (currentPullAnimationTime < weaponChangeDuration)
            {
                currentPullAnimationTime += Time.deltaTime;
                currentPullValue = Mathf.Lerp(1, 0, currentPullAnimationTime / weaponChangeDuration);
                SetWallParameter(Mathf.Max(currentWallValue, currentPullValue));
            }
            else
            {
                currentPullAnimationTime = 0;
                currentPullValue = 0;
                animateWeaponPull = false;
            }
        }
    }
}
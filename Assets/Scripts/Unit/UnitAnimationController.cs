using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitAnimationController : MonoBehaviour
{
    [Header("Refernces")]
    public UnitController owner;
    public AnimationEventsAdapter eventsAdapter;
    [SerializeField]
    private Animator animator;

    private HashSet<string> paramaterNames;

    public void SetState(string state)
    {
        if(!paramaterNames.Contains(state))
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
            switch(newWeapon.Config.type)
            {
                case WeaponConfig.WeaponType.Rifle:
                    SetAnimatorLayer("Chemirail");
                    break;
                case WeaponConfig.WeaponType.Pistol:
                    SetAnimatorLayer("Laser Pistol");
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
            if (i != index)
            {
                animator.SetLayerWeight(i, 0);
            }
        }
    }

    private void Start()
    {
        AnimatorControllerParameter[] parameters = animator.parameters;
        paramaterNames = new HashSet<string>(parameters.Select(e => e.name));
    }
}

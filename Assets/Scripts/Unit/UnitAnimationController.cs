using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
    [Header("Refernces")]
    public UnitController owner;
    [SerializeField]
    private Animator animator;

    public void SetState(string state)
    {
        animator.SetTrigger(state);
    }

    public void SetStaticState(string state)
    {
        animator.SetBool(state, true);
    }

    public void ResetStaticState(string state)
    {
        animator.SetBool(state, false);
    }

    public void SetAnimatorLayer(string name)
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
}

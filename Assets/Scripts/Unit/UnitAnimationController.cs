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
            if (newWeapon.config.type == WeaponConfig.WeaponType.Rifle)
            {
                SetAnimatorLayer("Chemirail");
            }
            else if (newWeapon.config.type == WeaponConfig.WeaponType.Pistol)
            {
                SetAnimatorLayer("Laser Pistol");
            }
        }
        else
        {
            SetAnimatorLayer("Unarmed");
        }
    }

    private void Start()
    {
        UpdateWeaponAnimation(owner.CurrentWeaponController);
    }
}

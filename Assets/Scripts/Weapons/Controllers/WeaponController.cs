using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PickableItem))]
abstract public class WeaponController : MonoBehaviour
{
    public enum MagazineStateType
    {
        Empty,
        Full,
        NotEmptyNotFull
    }

    abstract public MagazineStateType MagazineState { get; }
    abstract public WeaponConfig Config { get; }
    abstract public Quaternion AttackDirection { get; set; }
    abstract public float Spread { get; }
    abstract public void StartAttack();
    abstract public void StopAttack();
    abstract public int Reload(int amount);
    abstract public void SetTotalAmmo(int amount);

    public event System.EventHandler attackEvent;

    protected void InvokeAttackEvent()
    {
        attackEvent?.Invoke(this, null);
    }

    public event System.EventHandler<(int, int)> ammoChangeEvent;

    protected void InvokeAmmoChengeEvent(int arg1, int arg2)
    {
        ammoChangeEvent?.Invoke(this, (arg1, arg2));
    }
}

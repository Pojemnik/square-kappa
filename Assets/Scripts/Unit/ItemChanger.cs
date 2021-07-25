using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitShooting))]
public class ItemChanger : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Unit owner;

    [Header("Default weapon")]
    [SerializeField]
    private bool useDefaultWeapon;
    [SerializeField]
    private GameObject defaultWeapon;
    
    [Header("Camera")]
    [SerializeField]
    private GameObject firstPresonCamera;

    [Header("Item pickup")]
    [SerializeField]
    private float weaponPickupRange;
    [SerializeField]
    private GameObject weaponMountingPoint;

    [Header("Item dropping")]
    [SerializeField]
    private float weaponThrowForce;
    [SerializeField]
    private float weaponDropCollisionTimeout;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent<WeaponController> weaponChangeEvent;

    private WeaponController defaultWeaponController;
    private PlayerCameraController cameraController;
    private GameObject selectedItem;
    private new Rigidbody rigidbody;
    private UnitShooting shooting;

    private void SelectWorldItem(GameObject item)
    {
        if (item)
        {
            if (item.CompareTag("Item"))
            {
                if (selectedItem != item)
                {
                    selectedItem = item;
                }
            }
            else
            {
                selectedItem = null;
            }
        }
        else
        {
            if (selectedItem)
            {
                selectedItem = null;
            }
        }
    }

    public void PickOrSwapWeapon()
    {
        if (owner.CurrentWeapon == defaultWeapon || owner.CurrentWeapon == null)
        {
            PickWeaponUp();
        }
        else
        {
            SwapWeapons();
        }
    }

    public void SwapWeapons()
    {
        if (selectedItem)
        {
            DropWeapon();
            PickWeaponUp();
        }
    }

    public void DropWeapon()
    {
        if(owner.CurrentWeapon == defaultWeapon)
        {
            return;
        }
        Rigidbody weaponRB = owner.CurrentWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = false;
        weaponRB.AddRelativeForce(0, 0, weaponThrowForce);
        weaponRB.AddRelativeTorque(0, -20, 0);
        owner.CurrentWeapon.transform.parent = null;
        weaponRB.AddForce(rigidbody.velocity, ForceMode.VelocityChange);
        owner.CurrentWeaponController.StopAttack();
        int ammoLeft = owner.CurrentWeaponController.Reload(-1);
        shooting.PickUpAmmo(owner.CurrentWeaponController.Config.type, ammoLeft);
        StartCoroutine(owner.CurrentWeapon.GetComponent<PickableItem>().SetLayerAfterDelay(weaponDropCollisionTimeout, 0));
        if (useDefaultWeapon)
        {
            GrabWeapon(defaultWeapon);
        }
        else
        {
            owner.CurrentWeapon = null;
            owner.AnimationController.UpdateWeaponAnimation(null);
            weaponChangeEvent.Invoke(null);
        }
    }

    public void GrabWeapon(GameObject weapon)
    {
        if(weapon == defaultWeapon)
        {
            print("Selected default weapon");
        }
        owner.CurrentWeapon = weapon;
        owner.CurrentWeapon.SetActive(true);
        Rigidbody weaponRB = owner.CurrentWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = true;
        owner.CurrentWeapon.transform.parent = weaponMountingPoint.transform;
        owner.CurrentWeapon.layer = 6; //player layer
        owner.AnimationController.UpdateWeaponAnimation(owner.CurrentWeaponController);
        PickableItem pickable = owner.CurrentWeapon.GetComponent<PickableItem>();
        owner.CurrentWeapon.transform.localPosition = pickable.relativePosition;
        owner.CurrentWeapon.transform.localRotation = Quaternion.Euler(pickable.relativeRotation);
        weaponChangeEvent.Invoke(owner.CurrentWeaponController);
    }

    public void PickWeaponUp()
    {
        if (selectedItem)
        {
            GrabWeapon(selectedItem);
            selectedItem = null;
        }
    }

    private void Awake()
    {
        cameraController = firstPresonCamera.GetComponent<PlayerCameraController>();
        rigidbody = GetComponent<Rigidbody>();
        shooting = GetComponent<UnitShooting>();
    }

    private void Start()
    {
        cameraController.ignoredLayers = new int[2] { 6, 7 };
        cameraController.targettingRange = weaponPickupRange;
        if (useDefaultWeapon)
        {
            if (defaultWeapon == null)
            {
                throw new System.Exception("No default weapon");
            }
            defaultWeaponController = defaultWeapon.GetComponent<WeaponController>();
            if (defaultWeaponController == null)
            {
                throw new System.Exception("No controller in default weapon");
            }
            GrabWeapon(defaultWeapon);
        }
        else
        {
            GrabWeapon(null);
        }
    }

    private void Update()
    {
        if (cameraController)
        {
            SelectWorldItem(cameraController.targetItem);
        }
    }

}

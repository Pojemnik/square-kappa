using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChanger : MonoBehaviour
{
    [Header("References")]
    public Unit owner;

    private PlayerCameraController cameraController;
    [SerializeField]
    private GameObject firstPresonCamera;
    private GameObject selectedItem;

    [Header("Item pickup")]
    public float weaponPickupRange;
    public float weaponThrowForce;
    public GameObject weaponMountingPoint;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent<RangedWeaponController> weaponChangeEvent;

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
        Rigidbody weaponRB = owner.CurrentWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = false;
        weaponRB.AddRelativeForce(weaponThrowForce, 0, 0);
        weaponRB.AddRelativeTorque(UnityEngine.Random.onUnitSphere);
        owner.CurrentWeapon.transform.parent = null;
        StartCoroutine(owner.CurrentWeaponController.SetLayerAfterDelay(3F, 0));
        owner.CurrentWeapon = null;
        owner.AnimationController.UpdateWeaponAnimation(null);
    }

    public void GrabWeapon(GameObject weapon)
    {
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
    }

    private void Start()
    {
        cameraController.ignoredLayers = new int[2] { 6, 7 };
        cameraController.targettingRange = weaponPickupRange;
    }

    private void Update()
    {
        if (cameraController)
        {
            SelectWorldItem(cameraController.targetItem);
        }
    }

}

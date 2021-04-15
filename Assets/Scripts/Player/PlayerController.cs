using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector3 speed;
    public float rollSpeed;
    public float cameraSensitivity;
    public GameObject jetpack = null;
    public Animator playerAnimator = null;
    public GameObject currentWeapon = null;
    public float weaponThrowForce;
    public GameObject rightHand;
    public float weaponPickupRange;

    private new Rigidbody rigidbody;
    private Vector2 rawInputXZ;
    private float rawInputY;
    private float rawInputRoll;
    private Quaternion lookTarget;
    private Vector3 lastMoveDelta;
    private JetpackController jetpackController;
    private WeaponController currentWeaponController;
    private GameObject selectedItem;
    private Health health;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        jetpackController = jetpack.GetComponent<JetpackController>();
        lastMoveDelta = Vector3.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentWeaponController = currentWeapon.GetComponent<WeaponController>();
        lookTarget = rigidbody.rotation;
        health = GetComponent<Health>();
    }

    public void MoveXZ(InputAction.CallbackContext context)
    {
        rawInputXZ = context.ReadValue<Vector2>();
        if (rawInputXZ.y > 0)
        {
            playerAnimator.SetTrigger("MoveForward");
            jetpackController.OnMoveForward();
        }
        else if (rawInputXZ.y < 0)
        {
            playerAnimator.SetTrigger("MoveBackward");
            jetpackController.OnMoveBackward();
        }
        if (rawInputXZ.x > 0)
        {
            jetpackController.OnMoveRight();
        }
        else if (rawInputXZ.x < 0)
        {
            jetpackController.OnMoveLeft();
        }
    }

    public void MoveVertical(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == 1)
        {
            rawInputY = 1;
            playerAnimator.SetTrigger("MoveUpDown");
            jetpackController.OnMoveUp();
        }
        else if (context.ReadValue<float>() == -1)
        {
            rawInputY = -1;
            playerAnimator.SetTrigger("MoveUpDown");
            jetpackController.OnMoveDown();
        }
        else
        {
            rawInputY = 0;
        }
    }

    public void Roll(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == 1)
        {
            rawInputRoll = 1;
        }
        else if (context.ReadValue<float>() == -1)
        {
            rawInputRoll = -1;
        }
        else
        {
            rawInputRoll = 0;
        }
    }

    public void RelativeLook(InputAction.CallbackContext context)
    {
        Vector2 rawInputLook = context.ReadValue<Vector2>();
        Vector2 deltaLook = rawInputLook * cameraSensitivity;
        if (deltaLook != Vector2.zero)
        {
            Quaternion xRotation = Quaternion.AngleAxis(-deltaLook.x, Vector3.forward);
            Quaternion yRotation = Quaternion.AngleAxis(-deltaLook.y, Vector3.right);
            lookTarget = rigidbody.rotation * xRotation * yRotation;
        }
    }

    public void LookAt(Vector3 direction)
    {
        lookTarget = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 0, 0);
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (currentWeapon == null || currentWeaponController == null)
        {
            return;
        }
        if (context.started)
        {
            currentWeaponController.startShoot();
            playerAnimator.SetTrigger("Fire");
        }
        else if (context.canceled)
        {
            currentWeaponController.stopShoot();
            playerAnimator.SetTrigger("StopFire");
        }
    }

    public void ActionOne(InputAction.CallbackContext context)
    {
        if(!context.started)
        {
            return;
        }
        if (currentWeapon != null)
        {
            DropWeapon();
        }
    }

    public void ActionTwo(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        if (currentWeapon)
        {
            SwapWeapons();
        }
        else
        {
            PickWeapon();
        }
    }

    private void SwapWeapons()
    {
        if (selectedItem)
        {
            DropWeapon();
            PickWeapon();
        }
    }

    private void MovePlayer()
    {
        Vector3 deltaSpeed = speed * Time.fixedDeltaTime;
        Vector3 moveDelta = new Vector3(rawInputXZ.x * deltaSpeed.x, rawInputY * deltaSpeed.y, -rawInputXZ.y * deltaSpeed.z);
        if (moveDelta == Vector3.zero && lastMoveDelta != Vector3.zero)
        {
            playerAnimator.SetTrigger("Stop");
            jetpackController.OnStop();
        }
        lastMoveDelta = moveDelta;
        rigidbody.AddRelativeForce(moveDelta);
    }

    private void RotatePlayer()
    {
        float deltaRoll = rollSpeed * Time.fixedDeltaTime * rawInputRoll;
        rigidbody.MoveRotation(lookTarget * Quaternion.Euler(0, deltaRoll, 0));
        lookTarget = rigidbody.rotation;
        int layerMask = 1 << 6 | 1 << 7; //do not include player and enemy layers
        layerMask = ~layerMask;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out RaycastHit hit, weaponPickupRange, layerMask))
        {
            print(hit.collider.gameObject.name);
            if (hit.collider.gameObject.CompareTag("Item"))
            {
                if (selectedItem != hit.collider.gameObject)
                {
                    selectedItem = hit.collider.gameObject;
                    selectedItem.GetComponent<PickableItem>().outline.enabled = true;
                }
            }
        }
        else
        {
            if (selectedItem)
            {
                selectedItem.GetComponent<PickableItem>().outline.enabled = false;
                selectedItem = null;
            }
        }
    }

    private void DropWeapon()
    {
        Rigidbody weaponRB = currentWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = false;
        weaponRB.AddRelativeForce(weaponThrowForce, 0, 0);
        weaponRB.AddRelativeTorque(5, 7, 9);
        currentWeapon.layer = 0; //default layer
        currentWeapon.transform.parent = null;
        currentWeapon = null;
        currentWeaponController = null;
    }

    private void PickWeapon()
    {
        if (selectedItem)
        {
            currentWeapon = selectedItem;
            currentWeapon.GetComponent<PickableItem>().outline.enabled = false;
            Rigidbody weaponRB = currentWeapon.GetComponent<Rigidbody>();
            weaponRB.isKinematic = true;
            currentWeapon.transform.parent = rightHand.transform;
            currentWeapon.layer = 6; //player layer
            currentWeaponController = currentWeapon.GetComponent<WeaponController>();
            PickableItem pickable = currentWeapon.GetComponent<PickableItem>();
            currentWeapon.transform.localPosition = pickable.relativePosition;
            currentWeapon.transform.localRotation = Quaternion.Euler(pickable.relativeRotation);
            selectedItem = null;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if((collision.gameObject.layer == 8 && gameObject.layer == 7) || (collision.gameObject.layer == 9 && gameObject.layer == 6))
        {
            health.Damaged(collision.gameObject.GetComponent<ProjectileController>().damage);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }
}

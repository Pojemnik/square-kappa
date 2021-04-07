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
    public GameObject weapon = null;
    public float weaponThrowForce;
    public GameObject rightHand;

    private new Rigidbody rigidbody;
    private Vector2 rawInputXZ;
    private float rawInputY;
    private float rawInputRoll;
    private Quaternion lookTarget;
    private Vector3 lastMoveDelta;
    private JetpackController jetpackController;
    private WeaponController weaponController;
    private GameObject selectedItem;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        jetpackController = jetpack.GetComponent<JetpackController>();
        lastMoveDelta = Vector3.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        weaponController = weapon.GetComponent<WeaponController>();
        lookTarget = rigidbody.rotation;
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
        if (weapon == null || weaponController == null)
        {
            return;
        }
        if (context.started)
        {
            weaponController.startShoot();
            playerAnimator.SetTrigger("Fire");
        }
        else if (context.canceled)
        {
            weaponController.stopShoot();
            playerAnimator.SetTrigger("StopFire");
        }
    }

    public void ActionOne(InputAction.CallbackContext context)
    {
        if (weapon != null)
        {
            DropWeapon();
        }
    }

    public void ActionTwo(InputAction.CallbackContext context)
    {
        if (weapon == null)
        {
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
        int layerMask = 1 << 6;
        layerMask = ~layerMask;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Item"))
            {
                selectedItem = hit.collider.gameObject;
                print(string.Format("Selected {0}", selectedItem.name));
            }
        }
    }

    private void DropWeapon()
    {
        Rigidbody weaponRB = weapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = false;
        weaponRB.AddRelativeForce(weaponThrowForce, 0, 0);
        weaponRB.AddRelativeTorque(5, 7, 9);
        weapon.layer = 0; //set layer to default
        weapon.transform.parent = null;
        weapon = null;
        weaponController = null;
    }

    private void PickWeapon()
    {
        if (selectedItem)
        {
            weapon = selectedItem;
            Rigidbody weaponRB = weapon.GetComponent<Rigidbody>();
            weaponRB.isKinematic = true;
            weapon.transform.parent = rightHand.transform;
            weapon.layer = 6; //set layer to player layer
            weaponController = weapon.GetComponent<WeaponController>();
            PickableItem pickable = weapon.GetComponent<PickableItem>();
            weapon.transform.localPosition = pickable.relativePosition;
            weapon.transform.localRotation = Quaternion.Euler(pickable.relativeRotation);
            selectedItem = null;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }
}

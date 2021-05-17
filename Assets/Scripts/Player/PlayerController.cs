using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class Inventory
    {
        private List<InventorySlot> smallSlots;
        private List<InventorySlot> bigSlots;
        private int totalSlots;
        private int maxSmallSlots;
        private int maxBigSlots;

        public Inventory(int bigSlotsNumber, int smallSlotsNumber)
        {
            smallSlots = new List<InventorySlot>();
            bigSlots = new List<InventorySlot>();
            maxBigSlots = bigSlotsNumber;
            maxSmallSlots = smallSlotsNumber;
        }

        public int AddWeapon(GameObject weapon)
        {
            WeaponController.WeaponSize size = weapon.GetComponent<WeaponController>().size;
            if (size == WeaponController.WeaponSize.Small)
            {
                if (smallSlots.Count < maxSmallSlots)
                {
                    smallSlots.Add(new InventorySlot(WeaponController.WeaponSize.Small));
                    smallSlots[smallSlots.Count - 1].AddWeapon(weapon);
                    return smallSlots.Count - 1 + maxBigSlots;
                }
            }
            if (size == WeaponController.WeaponSize.Big)
            {
                if (bigSlots.Count < maxBigSlots)
                {
                    bigSlots.Add(new InventorySlot(WeaponController.WeaponSize.Big));
                    bigSlots[bigSlots.Count - 1].AddWeapon(weapon);
                    return bigSlots.Count - 1;
                }
            }
            return -1;
        }

        public GameObject GetWeapon(int index)
        {
            if (maxBigSlots > index)
            {
                if (bigSlots.Count > index)
                {
                    GameObject weapon = bigSlots[index].weapon;
                    bigSlots.RemoveAt(index);
                    return weapon;
                }
                return null;
            }
            index -= maxBigSlots;
            if (maxSmallSlots > index)
            {
                if (smallSlots.Count > index)
                {
                    GameObject weapon = smallSlots[index].weapon;
                    smallSlots.RemoveAt(index);
                    return weapon;
                }
                return null;
            }
            return null;
        }
    }

    [System.Serializable]
    public class InventorySlot
    {
        public WeaponController.WeaponSize slotSize { get { return size; } }
        public GameObject weapon { get { return weaponObject; } }
        public WeaponController controller { get { return controllerObject; } }
        public bool empty { get { return weaponObject == null; } }

        private GameObject weaponObject;
        private WeaponController controllerObject;
        private WeaponController.WeaponSize size;

        public InventorySlot(WeaponController.WeaponSize weaponSize)
        {
            size = weaponSize;
        }

        public void AddWeapon(GameObject weapon)
        {
            if (empty)
            {
                WeaponController tempController = weapon.GetComponent<WeaponController>();
                if (tempController.size != size)
                {
                    throw new Exception("Wrong weapon size!");
                }
                weaponObject = weapon;
                controllerObject = tempController;
                if (controllerObject == null)
                {
                    throw new Exception("No weapon controller in weapon!");
                }
            }
            else
            {
                throw new Exception("Weapon slot already occupied");
            }
        }
        public void RemoveWeapon()
        {
            if (weaponObject == null)
            {
                Debug.LogWarning("Removing empty weapon from inventory!");
            }
            weaponObject = null;
            controllerObject = null;
        }
    }

    public Vector3 speed;
    public float rollSpeed;
    public float cameraSensitivity;
    public GameObject jetpack = null;
    public Animator playerAnimator = null;
    public GameObject currentWeapon = null;
    public float weaponThrowForce;
    public GameObject rightHand;
    public float weaponPickupRange;
    public UnityEngine.Events.UnityEvent<int, string> inventoryChange;
    public float dashForceMultipler;
    public float dashStopForceMultipler;
    public float dashCooldownTime;

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
    private Inventory inventory;
    private bool canDash;
    private bool dashMode;

    private void Awake()
    {
        //get components
        rigidbody = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        if (jetpack != null)
        {
            jetpackController = jetpack.GetComponent<JetpackController>();
        }
        if (currentWeapon != null)
        {
            currentWeaponController = currentWeapon.GetComponent<WeaponController>();
        }
        canDash = true;
    }

    public void Start()
    {
        //init camera
        lastMoveDelta = Vector3.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        lookTarget = rigidbody.rotation;
        //init inventory
        inventory = new Inventory(1, 2);
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
        lookTarget = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartFire();
        }
        else if (context.canceled)
        {
            StopFire();
        }
    }

    public void StartFire()
    {
        if (currentWeapon == null || currentWeaponController == null)
        {
            return;
        }
        currentWeaponController.startShoot();
        playerAnimator.SetTrigger("Fire");
    }

    public void StopFire()
    {
        if (currentWeapon == null || currentWeaponController == null)
        {
            return;
        }
        currentWeaponController.stopShoot();
        playerAnimator.SetTrigger("StopFire");
    }

    public void ActionOne(InputAction.CallbackContext context)
    {
        if (!context.started)
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

    public void ActionThree(InputAction.CallbackContext context)
    {
        if (context.started && canDash)
        {
            Time.timeScale = 0.2F;
            dashMode = true;
            return;
        }
        if (context.canceled)
        {
            dashMode = false;
            Time.timeScale = 1;
        }
    }

    public void MoveTowards(Vector3 direction)
    {
        direction = direction.normalized;
        direction = transform.right * direction.x + transform.up * direction.y + transform.forward * direction.z;
        rawInputXZ = new Vector3(direction.x, -direction.z);
        rawInputY = direction.y;
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
        if (dashMode && moveDelta != Vector3.zero)
        {
            print("Dashed");
            StartCoroutine(dashCoroutine(moveDelta * dashForceMultipler));
            dashMode = false;
            canDash = false;
            StartCoroutine(dashCooldownCoroutine(dashCooldownTime));
        }
        else
        {
            rigidbody.AddRelativeForce(moveDelta);
        }
        lastMoveDelta = moveDelta;
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
            GrabWeapon(selectedItem);
            selectedItem = null;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.layer == 8 && gameObject.layer == 7) || (collision.gameObject.layer == 9 && gameObject.layer == 6))
        {
            health.Damaged(collision.gameObject.GetComponent<ProjectileController>().damage);
        }
    }

    private void GrabWeapon(GameObject weapon)
    {
        currentWeapon = weapon;
        currentWeapon.SetActive(true);
        currentWeapon.GetComponent<PickableItem>().outline.enabled = false;
        Rigidbody weaponRB = currentWeapon.GetComponent<Rigidbody>();
        weaponRB.isKinematic = true;
        currentWeapon.transform.parent = rightHand.transform;
        currentWeapon.layer = 6; //player layer
        currentWeaponController = currentWeapon.GetComponent<WeaponController>();
        PickableItem pickable = currentWeapon.GetComponent<PickableItem>();
        currentWeapon.transform.localPosition = pickable.relativePosition;
        currentWeapon.transform.localRotation = Quaternion.Euler(pickable.relativeRotation);
    }

    private IEnumerator dashCoroutine(Vector3 force)
    {
        Time.timeScale = 1;
        rigidbody.AddRelativeForce(force, ForceMode.VelocityChange);
        yield return new WaitForSecondsRealtime(1);
        rigidbody.AddRelativeForce(-force * dashStopForceMultipler, ForceMode.VelocityChange);
    }

    private IEnumerator dashCooldownCoroutine(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        canDash = true;
        print("Can dash now");
    }

    private void SelectWeapon(int slot)
    {
        GameObject weapon = inventory.GetWeapon(slot);
        if (currentWeapon)
        {
            //some weapon currently in hands
            int insertedSlot = inventory.AddWeapon(currentWeapon);
            if (insertedSlot != -1)
            {
                //there is place for it in inventory
                //print(String.Format("Weapon {0} placed in inventory", currentWeapon.name));
                inventoryChange.Invoke(insertedSlot, currentWeapon.name);
                currentWeapon.SetActive(false);
                currentWeapon = null;
                currentWeaponController = null;
            }
            else
            {
                //there is no place
                //print(String.Format("No space in inventory. Dropping weapon {0}", currentWeapon.name));
                DropWeapon();
            }
        }
        if (weapon)
        {
            //take weapon from selected slot
            inventoryChange.Invoke(slot, "");
            GrabWeapon(weapon);
            //print(String.Format("Taken weapon from inventory: {0}", weapon.name));
        }
    }

    public void SelectWeapon1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SelectWeapon(0);
        }
    }

    public void SelectWeapon2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SelectWeapon(1);
        }
    }

    public void SelectWeapon3(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SelectWeapon(2);
        }
    }


    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }
}

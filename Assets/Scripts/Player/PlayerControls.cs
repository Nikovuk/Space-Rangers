using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngineInternal;

public class PlayerControls : MonoBehaviour
{
    [Header("Params")]
    [Range(10f, 100f)]
    [SerializeField] public float ForwardSpeed = 50f;
    [Range(1f, 15f)]
    [SerializeField] public float StrafeSpeed = 7.5f;
    [Range(1f, 10f)]
    [SerializeField] public float HoverSpeed = 5f;
    [Range(1f, 180f)]
    [SerializeField] public float RollSpeed = 5f;
    [Range(10f, 1000f)]
    [SerializeField] public float BoostSpeed = 200f;
    [Range(0.1f, 5f)]
    [SerializeField] public float ForwardSmooth = 0.5f;
    [Range(0.1f, 5f)]
    [SerializeField] public float BoostSmooth = 0.5f;
    [Range(0.1f, 5f)]
    [SerializeField] public float StrafeSmooth = 0.8f;
    [Range(0.1f, 5f)]
    [SerializeField] public float HoverSmooth = 1.2f;
    [Range(0.1f, 5f)]
    [SerializeField] public float RollSmooth = 0.5f;
    [Range(1f, 180f)]
    [SerializeField] public float LookRotateSpeed = 90f;
    [Range(0.01f, 2f)]
    [SerializeField] public float BrakeSmooth = 0.4f;
    [Range(0f, 1f)]
    [SerializeField] public float LookDeadZone = 0.2f;
    [SerializeField] public float interactionRange = 2;
    [SerializeField] public float interactionRangeSphere = 2;
    [SerializeField] int ammoPerShot = 1;
    [SerializeField] KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] KeyCode BrakeKey = KeyCode.C;
    [SerializeField] float fuelPerSecond = 1f;
    [SerializeField] float boostFuelMultiplier = 2f;
    [SerializeField] float shootCooldown = 0.12f;
    //[Range(0.1f, 10f)]
    //[SerializeField] public float drag = 0.5f;


    [Header("Links")]
    [SerializeField] TMP_Text itemDescription;
    [SerializeField] GameObject stockShopPanel;
    [SerializeField] StockMarketUI StockMarketUI;
    [SerializeField] PlayerResources playerResources;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] CameraScript CameraControl;
    [SerializeField] SceneManagerScript SceneManagerScript;
    public LayerMask itemsLayer;
    private float ActiveForwardSpeed, ActiveStrafeSpeed, ActiveHoverSpeed, RefForwardVel, RefStrafeVel, RefHoverVel, RollInput, RefRoll;
    private Vector2 LookInput, ScreenCenter, MouseDist;
    bool IsBoost = false;

    Rigidbody rb;
    float shootTimer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ScreenCenter.x = Screen.width * 0.5f;
        ScreenCenter.y = Screen.height * 0.5f;
        if (playerResources == null)
        {
            playerResources = GetComponent<PlayerResources>();
            if (playerResources == null)
                Debug.LogWarning("NoPlayerRes");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LookInput.x = Input.mousePosition.x;
        LookInput.y = Input.mousePosition.y;

        MouseDist.x = (LookInput.x - ScreenCenter.x) / ScreenCenter.y;
        MouseDist.y = (LookInput.y - ScreenCenter.y) / ScreenCenter.y;

        MouseDist = Vector2.ClampMagnitude(MouseDist, 1f);

        float mouseMagnitude = MouseDist.magnitude;
        if (mouseMagnitude <= LookDeadZone)
        {
            MouseDist = Vector2.zero;
        }
        else if (mouseMagnitude > 0f)
        {
            float mappedMagnitude = (mouseMagnitude - LookDeadZone) / (1f - LookDeadZone);
            MouseDist = MouseDist.normalized * Mathf.Clamp01(mappedMagnitude);
        }

        transform.Rotate(-MouseDist.y * LookRotateSpeed * Time.deltaTime, MouseDist.x * LookRotateSpeed * Time.deltaTime, RollInput, Space.Self);

        //Двигало

        ActiveStrafeSpeed = Mathf.SmoothDamp(ActiveStrafeSpeed, Input.GetAxisRaw("Horizontal") * StrafeSpeed, ref RefStrafeVel, StrafeSmooth);

        ActiveHoverSpeed = Mathf.SmoothDamp(ActiveHoverSpeed, Input.GetAxisRaw("Hover") * HoverSpeed, ref RefHoverVel, HoverSmooth);
        IsBoost = Input.GetKey(KeyCode.LeftShift);

        if (IsBoost == true)
        {
            ActiveForwardSpeed = Mathf.SmoothDamp(ActiveForwardSpeed, BoostSpeed, ref RefForwardVel, BoostSmooth);
        }
        else
        {
            ActiveForwardSpeed = Mathf.SmoothDamp(ActiveForwardSpeed, Input.GetAxisRaw("Vertical") * ForwardSpeed, ref RefForwardVel, ForwardSmooth);
        }

        rb.linearVelocity = transform.forward * ActiveForwardSpeed + transform.right * ActiveStrafeSpeed + transform.up * ActiveHoverSpeed;

        RollInput = Mathf.SmoothDamp(RollInput, Input.GetAxisRaw("Roll") * RollSpeed, ref RefRoll, RollSmooth);
        FuelCons();
        //CameraControl.follow();

        bool IsBraking = Input.GetKey(BrakeKey);
        if (IsBraking)
        {
            ActiveStrafeSpeed = Mathf.SmoothDamp(ActiveStrafeSpeed, 0f, ref RefStrafeVel, BrakeSmooth);
            ActiveHoverSpeed = Mathf.SmoothDamp(ActiveHoverSpeed, 0f, ref RefHoverVel, BrakeSmooth);
            ActiveForwardSpeed = Mathf.SmoothDamp(ActiveForwardSpeed, 0f, ref RefForwardVel, BrakeSmooth);
            RollInput = Mathf.SmoothDamp(RollInput, 0f, ref RefRoll, BrakeSmooth);
            IsBoost = false;
        }

        if (Input.GetKey(KeyCode.R) == true)
        {
            SceneManagerScript.reloadScene();
        }

        if (Input.GetKey(KeyCode.Tab) == true)
        {
            if (stockShopPanel.activeInHierarchy == true)
                StockMarketUI.OpenStockShop();
            else
                StockMarketUI.CloseStockShop();
        }


        //shoot
        int ammo = playerResources.ammo;
        if (shootTimer > 0f) shootTimer -= Time.deltaTime;
        if (Input.GetKey(shootKey) && shootTimer <= 0f && playerResources.ammo > 0)
        {
            ShootLaser();
            shootTimer = shootCooldown;
        }
        RaycastHit hit;
        Ray ray = new Ray(CameraControl.transform.position, CameraControl.transform.forward);

        if (Physics.Raycast(ray, out hit, interactionRange, itemsLayer))
        {
            ItemContainerScript container = hit.transform.GetComponent<ItemContainerScript>();
            itemDescription.text = container.item.itemData.name;

            if (Input.GetKeyDown(KeyCode.F))
            {
                ItemInstance item = container.item;
                int amount = container.amount;
                int remaining = GetComponent<Inventory>().addItems(item, amount);
                container.pickUp(remaining);
            }
        }
        else
        {
            Collider[] nearbyItems = Physics.OverlapSphere(transform.position, interactionRangeSphere, itemsLayer);

            if (nearbyItems.Length > 0)
            {
                ItemContainerScript container = nearbyItems[0].GetComponent<ItemContainerScript>();
                itemDescription.text = container.item.itemData.name;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    ItemInstance item = container.item;
                    int amount = container.amount;
                    int remaining = GetComponent<Inventory>().addItems(item, amount);
                    container.pickUp(remaining);
                }
            }
            else
            {
                itemDescription.text = "";
            }
        }
        void FuelCons()
        {
            float forwardFraction = Mathf.Clamp01(Mathf.Abs(ActiveForwardSpeed) / Mathf.Max(ForwardSpeed, 0.0001f));
            // base consumption scaled by forward fraction
            float consumptionThisFrame = fuelPerSecond * forwardFraction * Time.fixedDeltaTime;

            if (IsBoost)
            {
                consumptionThisFrame *= boostFuelMultiplier;
            }

            // Attempt to use fuel. If not enough fuel, optionally reduce speed (simple fallback).
            bool hadFuel = playerResources.UseFuel(consumptionThisFrame);
            if (!hadFuel)
            {
                // Out of fuel: clamp speeds down (a simple, gentle stop)
                ActiveForwardSpeed = Mathf.SmoothDamp(ActiveForwardSpeed, 0f, ref RefForwardVel, 0.5f);
                ActiveStrafeSpeed = Mathf.SmoothDamp(ActiveStrafeSpeed, 0f, ref RefStrafeVel, 0.5f);
                ActiveHoverSpeed = Mathf.SmoothDamp(ActiveHoverSpeed, 0f, ref RefHoverVel, 0.5f);
            }

        }
        void ShootLaser()
        {
            playerResources.SpendAmmo(ammoPerShot);
            GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
        }

    }
}

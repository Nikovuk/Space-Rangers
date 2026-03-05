using System.Threading;
using UnityEngine;

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

    //[Range(0.1f, 10f)]
    //[SerializeField] public float drag = 0.5f;

    [Header("Links")]
    [SerializeField] CameraScript CameraControl;
    [SerializeField] SceneManagerScript SceneManagerScript;
    private float ActiveForwardSpeed, ActiveStrafeSpeed, ActiveHoverSpeed, RefForwardVel, RefStrafeVel, RefHoverVel, RollInput, RefRoll;
    private Vector2 LookInput, ScreenCenter, MouseDist;
    bool IsBoost = false;

    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ScreenCenter.x = Screen.width * 0.5f;
        ScreenCenter.y = Screen.height * 0.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ////Попробовал через ларп - получилось не оч - не останавливается, через смуздамп намного лучше получилось, но пришлось сменить на велосити
        //ActiveForwardSpeed = Mathf.Lerp(ActiveForwardSpeed, Input.GetAxisRaw("Vertical") * ForwardSpeed, ForwardSmooth * Time.deltaTime);
        ////Спросить почему лерп не уводит в ноль скорость, хотя инпут ноль
        //Debug.LogWarning(Input.GetAxisRaw("Vertical"));
        //Debug.Log(ActiveForwardSpeed);
        //ActiveStrafeSpeed = Mathf.Lerp(ActiveStrafeSpeed, Input.GetAxisRaw("Horizontal") * StrafeSpeed, StrafeSmooth * Time.deltaTime);
        //ActiveHoverSpeed = Mathf.Lerp(ActiveHoverSpeed, Input.GetAxisRaw("Hover") * HoverSpeed, HoverSmooth * Time.deltaTime);

        ////transform.position += transform.forward * ActiveForwardSpeed * Time.deltaTime;
        //Vector3 forward = transform.forward * ActiveForwardSpeed;
        //Vector3 strafe = transform.right * ActiveStrafeSpeed;
        //Vector3 hover = transform.up * ActiveHoverSpeed;

        //Vector3 movement = forward + strafe + hover;
        //rb.AddForce(movement);


        //Крутяшки
        // Надо придумать как увеличить дедзону середины экрана, не оч удобно 
        LookInput.x = Input.mousePosition.x;
        LookInput.y = Input.mousePosition.y;

        MouseDist.x = (LookInput.x - ScreenCenter.x) / ScreenCenter.y;
        MouseDist.y = (LookInput.y - ScreenCenter.y) / ScreenCenter.y;// Делим на наименьшую величину, чтобы была КаМсистентность

        MouseDist = Vector2.ClampMagnitude(MouseDist, 1f); // Ограничивает скорость до 1
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
        //CameraControl.follow();
        if (Input.GetKey(KeyCode.R) == true)
        {
            SceneManagerScript.reloadScene();
        }
    }
    //////Отмена, камеру прикрутил к звездолету
    //// Во-первых вынес расчеты по экрану в камеру, что кажется логичным поначалу, но в итоге странная функция получилась.
    //// Во-вторых, я хз как норм крутить корабль с помощью ригидбади, поэтому трансформ, но чет фигня какая-то тк в остальном ригидабади юзается
    //public void RotatePlayer(float MouseY, float MouseX, Vector2 MouseDist)
    //{
    //    transform.Rotate(-MouseY*LookRotateSpeed*Time.deltaTime,MouseX*LookRotateSpeed *Time.deltaTime,0f,Space.Self);
    //}
}

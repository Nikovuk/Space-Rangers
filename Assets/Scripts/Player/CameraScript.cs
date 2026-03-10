using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Params")]
    [Range(0.1f, 100f)]
    [SerializeField] float rotationSpeed;
    [Range(0, 360)]
    [SerializeField] float rotationAngle = 0;

    [Header("Links")]
    [SerializeField] PlayerControls Controls;
    [SerializeField] Transform player;

    //private Vector2 LookInput, ScreenCenter, MouseDist;
    Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position - player.position;
        //ScreenCenter.x = Screen.width * 0.5f;
        //ScreenCenter.y = Screen.height * 0.5f;
    }

    public void follow()
    {
        transform.position = RotatePointAroundPivot(player.position + offset, player.position, rotationAngle);
        transform.LookAt(player.position);
        //player.position + offset;
    }
    public void rotate(float delta)
    {
        rotationAngle += delta + rotationSpeed * Time.deltaTime;
    }
    public Vector3 getDirection(Vector3 dir)
    {
        return transform.TransformDirection(dir);
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
    {
        return Quaternion.Euler(0, angle, 0) * (point - pivot) + pivot;
    }


    void FixedUpdate()
    {
        //LookInput.x = Input.mousePosition.x;
        //LookInput.y = Input.mousePosition.y;

        //MouseDist.x = (LookInput.x - ScreenCenter.x) / ScreenCenter.y;
        //MouseDist.y = (LookInput.y - ScreenCenter.y) / ScreenCenter.y;// ƒелим на наименьшую величину, чтобы была  аћсистентность

        //MouseDist = Vector2.ClampMagnitude(MouseDist, 1f); // ќграничивает скорость до 1

        //Controls.RotatePlayer(MouseDist.y, MouseDist.x, MouseDist);
    }
}

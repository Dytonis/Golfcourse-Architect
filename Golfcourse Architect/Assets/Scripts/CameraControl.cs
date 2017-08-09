using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Vector3 Velocity;

    public bool Left;
    public bool Right;
    public bool Up;
    public bool Down;

    public float Speed;
    public float Accel;
    public float Brake;

    public Vector2 MinMaxZoom;

    public GameObject VerticalRotator;
    public GameObject Camera;

    // Use this for initialization
    void Start()
    {
        Velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();

        float min = -4f;

        float accelNormalized = ((-Camera.transform.localPosition.z - min) / (MinMaxZoom.y - min)) * Accel;
        float speedNormalized = ((-Camera.transform.localPosition.z - min) / (MinMaxZoom.y - min)) * Speed;
        float brakeNormalized = ((-Camera.transform.localPosition.z - min) / (MinMaxZoom.y - min)) * Brake;

        ChangeVelocity(accelNormalized, brakeNormalized);
        ClampSpeed(speedNormalized);

        transform.Translate(new Vector3(Velocity.x, 0, Velocity.y) * Time.deltaTime);
        HandleCameraRotation();
        HandleZoom();
    }

    private void ChangeVelocity(float accelNormalized, float brakeNormalized)
    {
        if (Up)
        {
            Velocity = new Vector3(Velocity.x, Velocity.y + (accelNormalized * Time.deltaTime));
        }
        if (Down)
        {
            Velocity = new Vector3(Velocity.x, Velocity.y - (accelNormalized * Time.deltaTime));
        }
        if (Left)
        {
            Velocity = new Vector3(Velocity.x - (accelNormalized * Time.deltaTime), Velocity.y);
        }
        if (Right)
        {
            Velocity = new Vector3(Velocity.x + (accelNormalized * Time.deltaTime), Velocity.y);
        }

        HandleBraking(brakeNormalized);
    }

    private void HandleBraking(float brakeNormalized)
    {
        if (Mathf.Abs(Velocity.y) >= 0.25f)
        {
            if (!Up && !Down)
            {
                if (Velocity.y > 0.01)
                    Velocity = new Vector3(Velocity.x, Velocity.y - (brakeNormalized * Time.deltaTime));
                else if (Velocity.y < 0.01)
                    Velocity = new Vector3(Velocity.x, Velocity.y + (brakeNormalized * Time.deltaTime));
            }
        }

        if (Mathf.Abs(Velocity.x) >= 0.25f)
        {
            if (!Left && !Right)
            {
                if (Velocity.x > 0.01)
                    Velocity = new Vector3(Velocity.x - (brakeNormalized * Time.deltaTime), Velocity.y);
                else if (Velocity.x < 0.01)
                    Velocity = new Vector3(Velocity.x + (brakeNormalized * Time.deltaTime), Velocity.y);
            }
        }
    }

    private void ClampSpeed(float speedNormalized)
    {
        if (Velocity.y > speedNormalized)
            Velocity = new Vector3(Velocity.x, speedNormalized);
        if (Velocity.y < -speedNormalized)
            Velocity = new Vector3(Velocity.x, -speedNormalized);
        if (Velocity.x > speedNormalized)
            Velocity = new Vector3(speedNormalized, Velocity.y);
        if (Velocity.x < -speedNormalized)
            Velocity = new Vector3(-speedNormalized, Velocity.y);

        if (!Up && !Down)
        {
            if (Mathf.Abs(Velocity.y) < 0.25f)
                Velocity.y = 0;
        }
        if (!Right && !Left)
        {
            if (Mathf.Abs(Velocity.x) < 0.25f)
                Velocity.x = 0;
        }
    }

    private void GetInputs()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && !Input.GetMouseButton(1))
        {
            if (Input.mousePosition.x / Screen.width <= 0.05f)
                Left = true;
            else
                Left = false;

            if (Input.mousePosition.x / Screen.width >= 0.95f)
                Right = true;
            else
                Right = false;

            if (Input.mousePosition.y / Screen.height <= 0.05f)
                Down = true;
            else
                Down = false;

            if (Input.mousePosition.y / Screen.height >= 0.95f)
                Up = true;
            else
                Up = false;
        }
        else
        {
            Up = false;
            Down = false;
            Left = false;
            Right = false;
        }
    }

    public void HandleZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            Camera.transform.localPosition = new Vector3(0, 0, Camera.transform.localPosition.z + Input.mouseScrollDelta.y);

            if (Camera.transform.localPosition.z <= -MinMaxZoom.y)
                Camera.transform.localPosition = new Vector3(0, 0, -MinMaxZoom.y);

            if (Camera.transform.localPosition.z >= -MinMaxZoom.x)
                Camera.transform.localPosition = new Vector3(0, 0, -MinMaxZoom.x);
        }
    }

    Vector2 lastMousePosition = Vector2.zero;
    public void HandleCameraRotation()
    {
        if (Input.GetMouseButton(1))
        {
            Vector2 mouseVelocity = new Vector2(lastMousePosition.x - Input.mousePosition.x, lastMousePosition.y - Input.mousePosition.y);

            if (mouseVelocity.x >= 150)
                mouseVelocity.x = 150;
            if (mouseVelocity.y >= 150)
                mouseVelocity.y = 150;
            if (mouseVelocity.x <= -150)
                mouseVelocity.x = -150;
            if (mouseVelocity.y <= -150)
                mouseVelocity.y = -150;

            VerticalRotator.transform.Rotate(new Vector3(mouseVelocity.y / 10f, 0, 0));
            transform.Rotate(new Vector3(0, -mouseVelocity.x / 10f, 0));
        }

        if (VerticalRotator.transform.eulerAngles.z >= 179)
            VerticalRotator.transform.eulerAngles = new Vector3(90, VerticalRotator.transform.rotation.eulerAngles.y, VerticalRotator.transform.rotation.eulerAngles.z);
        if (VerticalRotator.transform.eulerAngles.x < 25)
            VerticalRotator.transform.eulerAngles = new Vector3(25, VerticalRotator.transform.rotation.eulerAngles.y, VerticalRotator.transform.rotation.eulerAngles.z);

        lastMousePosition = Input.mousePosition;
    }
}

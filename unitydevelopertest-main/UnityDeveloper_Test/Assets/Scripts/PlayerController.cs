using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    [SerializeField]
    private bool isGrounded;
    private PlayerAnimationController animController;
    [SerializeField]
    private List<GameObject>holograms = new List<GameObject>();
    [SerializeField]
    public CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    private bool applyGravity;
    private bool isGravityDirectionSelected;
    private Vector3 newGravityDirection;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animController = GetComponent<PlayerAnimationController>();
    }

    void Update()
    {
        Move();
        Rotate();
        Jump();
        SelectGravityDirection();
        ApplyGravity();
      
    }

    void Move()
    {
        float moveVertical = 0;
        if (Input.GetKey(KeyCode.W))
        {
            moveVertical = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVertical = -1;
        }

        Vector3 movement = transform.forward * moveVertical * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        if (animController != null)
        {
            animController.RunningAnim(moveVertical);
        }
    }

    void Rotate()
    {
        float rotateHorizontal = 0;
        if (Input.GetKey(KeyCode.A))
        {
            rotateHorizontal = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotateHorizontal = 1;
        }
        Vector3 rotation = new Vector3(0, rotateHorizontal * rotationSpeed * Time.deltaTime, 0);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 jumpDirection = -Physics.gravity.normalized;
            rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
            animController.JumpingAnim(true);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
        animController.JumpingAnim(false);
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        animController.JumpingAnim(true);
    }
    void SelectGravityDirection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            newGravityDirection = Vector3.down;
            ShowHologram(1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            newGravityDirection = Vector3.forward;
            ShowHologram(0);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            newGravityDirection = Vector3.left;
            ShowHologram(2);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            newGravityDirection = Vector3.right;
            ShowHologram(3);
        }

        if (Input.GetKeyDown(KeyCode.Return)) // Enter key
        {
            isGravityDirectionSelected = true;
            applyGravity = true;
        }
    }
    void ShowHologram(int index)
    {
        // Deactivate all holograms
        foreach (var hologram in holograms)
        {
            hologram.SetActive(false);
        }

        // Activate the selected hologram
        if (index >= 0 && index < holograms.Count)
        {
            holograms[index].SetActive(true);
        }
    }
    void ApplyGravity()
    {
        if (isGravityDirectionSelected)
        {
            // Apply the new gravity direction relative to the player's rotation
            Vector3 adjustedGravityDirection = transform.TransformDirection(newGravityDirection);
            Physics.gravity = adjustedGravityDirection * Physics.gravity.magnitude;

            // Update the player's orientation
            UpdatePlayerOrientation(adjustedGravityDirection);

            // Update the camera's orientation
            UpdateCameraOrientation();

            // Reset the gravity application state
            ResetGravity();
        }
    }
    void UpdatePlayerOrientation(Vector3 adjustedGravityDirection)
    {
        transform.up = -adjustedGravityDirection;
    }

    void UpdateCameraOrientation()
    {
        // Adjust the camera's rotation to match the player's orientation
        if (virtualCamera != null)
        {
            virtualCamera.transform.rotation = Quaternion.LookRotation(transform.forward, -Physics.gravity.normalized);
        }
    }

    void ResetGravity()
    {
        applyGravity = false;
        isGravityDirectionSelected = false;

        // Hide all holograms
        foreach (var hologram in holograms)
        {
            hologram.SetActive(false);
        }
    }
}

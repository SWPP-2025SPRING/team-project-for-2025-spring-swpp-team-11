using UnityEngine;


namespace GuidanceLine
{

    /// <summary>
    /// A simple script to test out the GuidanceLine inthe example scene!
    /// </summary>

    public class Player : MonoBehaviour
    {
        public float speed = 5f;
        public float gravity = -9.81f;
        public float jumpForce = 5f;
        public float mouseSensitivity = 2f;
        public float pitchLimit = 80f;

        private float yVelocity = 0f;
        private bool isGrounded;

        private CharacterController controller;
        private Transform playerCamera;
        private float xRotation = 0f;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            playerCamera = Camera.main.transform;
        }

        void Update()
        {
            isGrounded = controller.isGrounded;

            float moveDirectionX = Input.GetAxis("Horizontal");
            float moveDirectionZ = Input.GetAxis("Vertical");

            Vector3 move = transform.right * moveDirectionX + transform.forward * moveDirectionZ;

            if (isGrounded)
            {
                yVelocity = -0.5f;
            }

            yVelocity += gravity * Time.deltaTime;

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                yVelocity = jumpForce;
            }

            Vector3 velocity = move * speed;
            velocity.y = yVelocity;
            controller.Move(velocity * Time.deltaTime);

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -pitchLimit, pitchLimit);
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}

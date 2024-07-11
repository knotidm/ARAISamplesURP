using UnityEngine;

/*
 *  Handles player
 *   - player being able to push objects.
 *   - detects when it hits a rigidbody gives it a bit of velocity
 */
public class Player : MonoBehaviour
{
    private CharacterController controller;

    // this script pushes all rigid bodies that the character touches
    private float pushPower = 2.0f;
    private float speed = 4; //walking speed
    private float gravity = 0; //current falling speed due to gravity

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Player movement:
        float mouseSensitivity = 1f;
        float vert = Input.GetAxis("Vertical");
        float horiz = Input.GetAxis("Horizontal");
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        float factor = 1;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) factor = 2;

        gravity -= 9.81f * Time.deltaTime;

        controller.Move((transform.forward * vert + transform.right * horiz) * Time.deltaTime * speed * factor + transform.up * (gravity) * Time.deltaTime);
        if (controller.isGrounded) gravity = 0;

        controller.transform.Rotate(Vector3.up, mouseX);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
        {
            return;
        }
        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.linearVelocity = pushDir * pushPower;
    }
}

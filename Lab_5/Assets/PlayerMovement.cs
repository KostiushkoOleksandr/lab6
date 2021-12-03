using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed, gravityModifire, jumpHeight, runSpeed, Doublejump;
    public CharacterController characterController;
    public Transform cameraTransform, feetPoint, FirePoint;
    public LayerMask groundLayer;
    public bool invertedMouseX;
    public bool invertedMouseY;
    public Animator animationController;
    public GameObject bulletInstance;


    private Vector3 moveInput;
    private Vector2 mouseInput;
    private bool isAllowedToJump;
    private bool isAllowedToDoubleJump;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float storedY = moveInput.y;

        Vector3 horizontalMove = transform.right * Input.GetAxis("Horizontal");
        Vector3 verticalMove = transform.forward * Input.GetAxis("Vertical");

        moveInput = verticalMove + horizontalMove;
        moveInput.Normalize();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveInput = moveInput * runSpeed;
        }
        else
        {
            moveInput = moveInput * moveSpeed;
        }
        //moveInput = moveInput * moveSpeed;

        moveInput.y = storedY;

        moveInput.y += Physics.gravity.y * gravityModifire * Time.deltaTime;

        if (characterController.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifire * Time.deltaTime;
        }

       

        isAllowedToJump = Physics.OverlapSphere(feetPoint.position, .25f, groundLayer).Length > 0;

        if (Input.GetKeyDown(KeyCode.Space) && isAllowedToJump)
        {
            moveInput.y += Doublejump;

            isAllowedToDoubleJump = true;
        }
        else if(isAllowedToDoubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            moveInput.y = Doublejump;

            isAllowedToDoubleJump = false;
        }

        characterController.Move(moveInput * Time.deltaTime);

        mouseInput = new Vector2(x:Input.GetAxisRaw("Mouse X"), y:Input.GetAxisRaw("Mouse Y"));

        if (invertedMouseX)
        {
            mouseInput.x = -mouseInput.x;
        }

        if (invertedMouseY)
        {
            mouseInput.y = -mouseInput.y;
        }

        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            y:transform.rotation.eulerAngles.y + mouseInput.x,
            transform.eulerAngles.z);


        cameraTransform.rotation = Quaternion.Euler(
            cameraTransform.eulerAngles + new Vector3(x:-mouseInput.y, y:0f, z:0f)
            );

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out raycastHit, 50f))
            {
                FirePoint.LookAt(raycastHit.point);
            }

            Instantiate(bulletInstance, FirePoint.position, FirePoint.rotation);
        }

         animationController.SetFloat(name:"moveSpeed", moveInput.magnitude);
         animationController.SetBool(name: "isGrounded", isAllowedToJump);
    }
}

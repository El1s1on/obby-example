using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using YG;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform orientation;
    [SerializeField] private GameObject ragdoll;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    private CharacterController controller;
    private Animator animator;
    private Vector3 moveInput;
    private bool readyForJump = true;
    private float gravity;

    [Header("Look")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform playerModel;
    [SerializeField] private float clampAngle = 90f;
    [SerializeField] private float mouseSensitivity = 100f;
    private float rotY = 0;
    private float rotX = 0;

    [Header("Zoom")]
    [SerializeField] private Transform meshModel;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minZoomDistance;
    [SerializeField] private float maxZoomDistance;
    private Camera mainCamera;

    [Header("Ground Check")]
    [SerializeField] private Transform groundPosition;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRadius;
    
    [Header("Vars")]
    private bool isGrounded;
    private bool isMoving;
    private bool inLadder;
    private bool firstPerson;
    private bool died;
    private bool hasLanded = true;

    [Header("Audio")]
    [SerializeField] private AudioSource footstepsAudio;
    [SerializeField] private AudioClip footstepSFX;
    [SerializeField] private AudioClip deathSFX;
    private AudioSource audioSource;

    private void OnEnable() => YandexGame.GetDataEvent += YGInit;

    private void OnDisable() => YandexGame.GetDataEvent -= YGInit;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        mainCamera = Camera.main;

        if (YandexGame.SDKEnabled == true)
        {
            YGInit();
        }
    }

    private void Update()
    {
        Zoom();
        Look();
        Vars();

        if (died) return;
        Movement();
        RotateBody();
    }

    public void YGInit()
    {
        Teleport(YandexGame.savesData.spawnPointPosition);
    }

    private void RotateBody()
    {
        if(!firstPerson)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (horizontalInput != 0f || verticalInput != 0f)
            {
                Vector3 moveInput = new Vector3(horizontalInput, 0f, verticalInput);
                Quaternion targetRotation = Quaternion.LookRotation(orientation.TransformDirection(moveInput));

                playerModel.rotation = Quaternion.Lerp(playerModel.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
        else
        {
            playerModel.rotation = orientation.rotation;
        }
    }

    private void Vars()
    {
        isGrounded = Physics.CheckSphere(groundPosition.position, groundRadius, groundLayer);
        isMoving = controller.velocity.x != 0f || controller.velocity.z != 0f;
        firstPerson = mainCamera.transform.localPosition.z == minZoomDistance;
        inLadder = Physics.Raycast(playerModel.position, playerModel.TransformDirection(Vector3.forward), out RaycastHit hit, 0.5f, LayerMask.GetMask("Ladder")) && !isGrounded;
    }

    private void Movement()
    {
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal") * speed, gravity, Input.GetAxisRaw("Vertical") * speed);
        
        controller.Move(orientation.TransformDirection(moveInput) * Time.deltaTime);

        animator.SetBool("Walk", isMoving && isGrounded && !died);
        animator.SetBool("OnLadder", inLadder);

        if(isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) && readyForJump && !inLadder)
            {
                StartCoroutine(Jump());
            }

            if(!hasLanded)
            {
                hasLanded = true;
                StartCoroutine(Land());
            }
        }
        else
        {
            hasLanded = false;

            if (inLadder)
            {
                gravity = 0f;
                if(Input.GetAxisRaw("Vertical") > 0)
                {
                    controller.Move(Vector3.up * speed * Time.deltaTime);
                    animator.SetBool("LadderClimb", true);
                    return;
                }
            }
            else
            {
                gravity += Physics.gravity.y * Time.deltaTime;
            }
        }

        animator.SetBool("LadderClimb", false);
    }

    private IEnumerator Land()
    {  
        Footstep();

        if(!Input.GetKeyDown(KeyCode.Space))
        {
            yield return new WaitForSeconds(0.5f);

            if (isGrounded && gravity < -5f)
            {
                gravity = 0f;
            }
        }
    }

    private IEnumerator Jump()
    {
        readyForJump = false;
        gravity = 0f;
        gravity += jumpForce;

        if(!inLadder)
        {
            animator.SetTrigger("Jump");
        }    
        
        yield return new WaitForSeconds(0.5f);
        readyForJump = true;
    }

    public void Die()
    {
        if (died) return;

        died = true;
        playerModel.gameObject.SetActive(false);
        var ragdoll = Instantiate(this.ragdoll, playerModel);
        ragdoll.transform.parent = null;
        cameraPivot.parent = null;

        animator.SetBool("Walk", false);
        animator.SetTrigger("Hurt");

        audioSource.PlayOneShot(deathSFX, 0.35f);

        Rigidbody[] parts = ragdoll.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody part in parts)
        {
            part.AddExplosionForce(Random.Range(5f, 10f), transform.position, Random.Range(20f, 50f), 0f, ForceMode.Impulse);
        }

        Destroy(ragdoll, 3f);
        Invoke(nameof(Respawn), 3f);
    }

    private void Respawn()
    {
        playerModel.gameObject.SetActive(true);
        cameraPivot.parent = transform;

        gravity = 0f;

        Teleport(YandexGame.savesData.spawnPointPosition);
        YandexGame.FullscreenShow();
        died = false;
    }

    public void Teleport(Vector3 pos)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(pos, Quaternion.identity);
        controller.enabled = true;
    }

    private void Zoom()
    {
        float zoomAmount = 0;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            zoomAmount += zoomSpeed;
        } 
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            zoomAmount -= zoomSpeed;
        }

        Vector3 zoomDirection = transform.forward * zoomAmount;
        Vector3 newPosition = mainCamera.transform.localPosition + zoomDirection;

        mainCamera.transform.localPosition = new Vector3(0f, 0f, Mathf.Clamp(newPosition.z, -maxZoomDistance, -minZoomDistance));
        meshModel.gameObject.SetActive(!firstPerson && !died);
    }

    private void Look()
    {
        if (!Input.GetMouseButton(1) && !firstPerson)
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity;
        rotX += mouseY * mouseSensitivity;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0f);

        cameraPivot.rotation = localRotation;
        orientation.rotation = Quaternion.Euler(0f, rotY, 0f);
    }

    public void Footstep()
    {
        if (!isGrounded && !inLadder) return;
        footstepsAudio.pitch = Random.Range(0.5f, 1.2f);
        footstepsAudio.PlayOneShot(footstepSFX, Random.Range(0.7f,1.2f));
    }
}

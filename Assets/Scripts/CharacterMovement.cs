﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float JumpHeight;

    [SerializeField]
    private float gravityMultiplier;

    [SerializeField]
    private float JumpHorizontalSpeed;

    [SerializeField]
    private float JunpButtonGracePeriodSpeed;

    [SerializeField]
    private float cameraTransform;

    // Start is called before the first frame update
    Animator animator;
    CharacterController characterController;
    public float speed = 3.0f;
    public float roatationSpeed = 25;
    public float jumpSpeed = 5f;
    public float gravity = 20.0f;
    Vector3 inputVec;
    Vector3 targetDiriction;
    private Vector3 moveDirection = Vector3.zero;
    private float ySpeed;
    void Start()
    {
        Time.timeScale = 1;
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    public LayerMask hitLayers;
    void Update()
    {
        //Check A* กับ Enemy to Player โดยค้นหา Shotest Path จาก Pathfinding.cs
        if (Input.GetMouseButtonDown(0))//คลิกตัวผู้เล่นเเละเคลื่อยย้ายจากจุดเพื่อทดสอบตำแหน่ง Enemy to Player
        {
            Vector3 mouse = Input.mousePosition;//รับตำแหน่งเมาส์
            Ray castPoint = Camera.main.ScreenPointToRay(mouse);//ScreenPointToRayเพื่อให้ได้ตำแหน่งที่เมาส์ชี้มา
            RaycastHit hit;//เก็บตำแหน่ง ray hit.
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, hitLayers))//ถ้า raycast ไม่ชนกำแพง
            {
                this.transform.position = hit.point;//ย้ายเป้าหมายไปที่ตำแหน่งเมาส์
            }
        }
        float x = -(Input.GetAxisRaw("Vertical"));
        float y = Input.GetAxisRaw("Jump");
        float z = Input.GetAxisRaw("Horizontal");
        inputVec = new Vector3(x, y , z);

        animator.SetFloat("Input X", z);
        animator.SetFloat("Input Y", 0);
        animator.SetFloat("Input Z", -(x));

        ySpeed += Physics.gravity.y * Time.deltaTime;
        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            ySpeed = jumpSpeed;
        }

        if (x != 0 || z != 0) //X ,Y or Z != 0
        {
            animator.SetBool("Moving", true);
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Moving", false);
            animator.SetBool("Running", false);
        }
        //Input=Horizontal,Jump,Vertical
        if (characterController.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")); // 
            moveDirection *= speed;
        }
        characterController.Move(moveDirection * Time.deltaTime);
        UpdateMovement();
    }

    void UpdateMovement()
    {
        Vector3 motion = inputVec;
        motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1) ? .7f : 1 ;
        RotateTowardMovementDirection();
        getCameraRealtive();
    }

    void RotateTowardMovementDirection()
    {
        if (inputVec != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(targetDiriction),Time.deltaTime*roatationSpeed);
        }
    }
    void getCameraRealtive()
    {
        Transform cameraTransform = Camera.main.transform; //เคลื่อนที่ไปทางไหนทิศทางจะตามด้วย
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);      //เปลี่ยนตำแหน่งมุมมองใหม่เมื่อตัวละครหันกลับมา
        float v = Input.GetAxisRaw("Vertical");
        //float j = Input.GetAxisRaw("Jump");
        float h = Input.GetAxisRaw("Horizontal");
        targetDiriction = (h * right) + (v * forward);
    }
}
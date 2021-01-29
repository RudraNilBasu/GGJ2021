using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System; // Enum.GetNames

// TODO(@rudra): Move this to a common file
using f32 = System.Single;
using u8  = System.Byte;
using i32 = System.Int32;
using u32 = System.UInt32;
using b32 = System.Boolean;
using v3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public Camera Camera;
    public GameManager GameManager;
    
    Rigidbody RigidBody;
    float MovementSpeed = 9.5f; // 1.5f TODO: @Console: Control these from console
    public v3 dP; // Movement delta for every frame
    bool LockMovement;
    
    public CapsuleCollider PlayerCapsuleCollider;
    public LayerMask WhatIsGround;
    public b32 AllowGroundCheck; // TODO: Deprecate this
    
    f32 NormalPlayerSpeed = 3.0f; //1.5f;
    
    void Start()
    {
        PlayerCapsuleCollider = GetComponent<CapsuleCollider>();
        RigidBody = GetComponent<Rigidbody>();
        
        MovementSpeed = NormalPlayerSpeed;
    }
    
    void FixedUpdate()
    {
        dP = Vector3.zero;
        
        if (GameManager.ShouldMove())
        {
            
            if (!LockMovement)
            {
                // Will Smoothing via Input.GetAxis("Horizontal") help in Gameplay?
                float RightMovement = Input.GetAxisRaw("Horizontal");
                float ForwardMovement = Input.GetAxisRaw("Vertical");
                Vector3 MovementVector = (transform.forward * ForwardMovement) + (transform.right * RightMovement);
                
                if (AllowGroundCheck)
                {
                    Vector3 ForwardChecker = (transform.position + MovementVector*MovementSpeed*Time.fixedDeltaTime) + new Vector3(0, 0, PlayerCapsuleCollider.radius);
                    Vector3 BackChecker = (transform.position + MovementVector*MovementSpeed*Time.fixedDeltaTime) - new Vector3(0, 0, PlayerCapsuleCollider.radius);
                    Vector3 LeftChecker = (transform.position + MovementVector*MovementSpeed*Time.fixedDeltaTime) - new Vector3(PlayerCapsuleCollider.radius, 0, 0);
                    Vector3 RightChecker = (transform.position + MovementVector*MovementSpeed*Time.fixedDeltaTime) + new Vector3(PlayerCapsuleCollider.radius, 0, 0);
                    if (MovementVector != Vector3.zero)
                    {
                        if (!GroundCheck(ForwardChecker))
                        {
                            if (MovementVector.z > 0)
                            {
                                Vector3 tmp = MovementVector;
                                tmp.z = 0;
                                MovementVector = tmp;
                            }
                        }
                        if (!GroundCheck(BackChecker))
                        {
                            if (MovementVector.z < 0)
                            {
                                Vector3 tmp = MovementVector;
                                tmp.z = 0;
                                MovementVector = tmp;
                            }
                        }
                        if (!GroundCheck(LeftChecker))
                        {
                            if (MovementVector.x < 0)
                            {
                                Vector3 tmp = MovementVector;
                                tmp.x = 0;
                                MovementVector = tmp;
                            }
                        }
                        if (!GroundCheck(RightChecker))
                        {
                            if (MovementVector.x > 0)
                            {
                                Vector3 tmp = MovementVector;
                                tmp.x = 0;
                                MovementVector = tmp;
                            }
                        }
                        
                    }
                }
                
                RigidBody.MovePosition(transform.position + MovementVector * MovementSpeed * Time.fixedDeltaTime);
                dP = MovementVector;
            }
        }
    }
    
    void OnTriggerEnter(Collider coll)
    {
        
    }
    
    void OnTriggerExit(Collider coll)
    {
        
    }
    
    public b32 GroundCheck(v3 Position)
    {
        RaycastHit hit;
        if (Physics.Raycast(Position,
                            transform.TransformDirection(Vector3.down), out hit,
                            2.5f, WhatIsGround))
        {
            Debug.DrawRay(Position,
                          transform.TransformDirection(Vector3.down) * hit.distance,
                          Color.yellow);
            return true;
        }
        else
        {
            Debug.DrawRay(Position,
                          transform.TransformDirection(Vector3.down) * 1000, Color.red);
            return false;
        }
    }
    
}

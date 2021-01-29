using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO(@rudra): Move this to a common file
using f32 = System.Single;
using u8  = System.Byte;
using u32 = System.UInt32;
using i32 = System.Int32;
using b32 = System.Boolean;
using v3 = UnityEngine.Vector3;

public class CameraRotation : MonoBehaviour
{
    public GameManager GameManager;
    public GameObject TheCamera;
    
    f32 MouseSensitivity;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        MouseSensitivity = 1.0f;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.ShouldMove())
        {
            float _xRot = Input.GetAxisRaw("Mouse Y"); // up and down
            float _yRot = Input.GetAxisRaw("Mouse X"); // left and right
            
            
            v3 CameraRotation = new v3(_xRot, 0, 0) * MouseSensitivity;
            TheCamera.transform.Rotate(-CameraRotation);
            
            CheckEdges();
            
            v3 ObserverRotation = new v3(0, _yRot, 0) * MouseSensitivity;
            transform.Rotate(ObserverRotation);
        }
    }
    
    public void CheckEdges()
    {
        if (TheCamera.transform.localRotation.x > 0.3f)
        {
            Quaternion CameraLocalRotation = TheCamera.transform.localRotation;
            TheCamera.transform.localRotation = new Quaternion(0.3f, CameraLocalRotation.y, CameraLocalRotation.z, CameraLocalRotation.w);
        }
        if (TheCamera.transform.localRotation.x < -0.2f)
        {
            Quaternion CameraLocalRotation = TheCamera.transform.localRotation;
            TheCamera.transform.localRotation = new Quaternion(-0.2f, CameraLocalRotation.y, CameraLocalRotation.z, CameraLocalRotation.w);
        }
    }
}

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
using v2 = UnityEngine.Vector2;

public class CSG : MonoBehaviour
{
    public v3 Center;
    public v3 Dim;
    
    // Start is called before the first frame update
    void Start()
    {
        Center = transform.position;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(Dim.x, Dim.y, Dim.z));
    }
    
    public Rectangle GetRect()
    {
        Rectangle Result = new Rectangle();
        
        v3 HalfDim = 0.5f*Dim;
        Result.Min = Center - HalfDim;
        Result.Max = Center + HalfDim;
        
        return(Result);
    }
    
}

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

public class Rectangle
{
    public v3 Min;
    public v3 Max;
    
    
    public Rectangle()
    {
        Min = new v3(0, 0, 0);
        Max = new v3(0, 0, 0);
    }
    
    public Rectangle(v3 _Min, v3 _Max)
    {
        Min = _Min;
        Max = _Max;
    }
    
    public static b32 IsInRectangle(Rectangle Rect, v3 Test)
    {
        b32 Result = ((Test.x >= Rect.Min.x) &&
                      (Test.y >= Rect.Min.y) &&
                      (Test.z >= Rect.Min.z) &&
                      (Test.x < Rect.Max.x) &&
                      (Test.y < Rect.Max.y) &&
                      (Test.z < Rect.Max.z));
        
        return(Result);
    }
    
}

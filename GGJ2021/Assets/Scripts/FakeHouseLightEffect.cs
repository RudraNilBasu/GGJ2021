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

public class FakeHouseLightEffect : MonoBehaviour
{
    public GameManager GM;
    MeshRenderer MR;
    
    b32 Started;
    
    // Start is called before the first frame update
    void Start()
    {
        MR = GetComponent<MeshRenderer>();
        MR.enabled = false;
        
        Started = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!Started && GM.ProgramMode == GameManager.Program_Mode.GAME)
        {
            Started = true;
            StartCoroutine(StartBlinking());
        }
    }
    
    IEnumerator StartBlinking()
    {
        MR.enabled = true;
        yield return new WaitForSeconds(0.5f);
        MR.enabled = false;
        yield return new WaitForSeconds(0.5f);
        MR.enabled = true;
        yield return new WaitForSeconds(0.5f);
        MR.enabled = false;
        yield return new WaitForSeconds(0.5f);
        MR.enabled = true;
        yield return new WaitForSeconds(0.5f);
        MR.enabled = false;
        yield return new WaitForSeconds(0.5f);
        MR.enabled = true;
    }
}

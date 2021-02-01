using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trailer : MonoBehaviour
{
    public GameObject BlackScreen, GameName;
    public AudioSource ELEKTRICITY, VHS;
    
    public Animation TrailerCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        BlackScreen.SetActive(false);
        GameName.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TrailerCamera.Play();
            
            StartCoroutine(ShowGameNameWhenNecessary());
        }
    }
    
    IEnumerator ShowGameNameWhenNecessary()
    {
        yield return new WaitForSeconds(50.0f);
        VHS.Stop();
        ELEKTRICITY.Stop();
        BlackScreen.SetActive(true);
        GameName.SetActive(true);
    }
}

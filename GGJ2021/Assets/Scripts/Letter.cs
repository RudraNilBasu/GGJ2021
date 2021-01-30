using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    public enum Tag {
        NONE,
        FIRST,
        SECOND,
        THIRD,
        FOURTH,
        OTHER_1,
        OTHER_2,
        OTHER_3,
        OTHER_4,
    }
    
    public Tag LetterTag;
    
    [TextArea(15,20)]
        public string Contents;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

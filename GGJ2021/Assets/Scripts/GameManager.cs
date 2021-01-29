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

public class GameManager : MonoBehaviour
{
    public enum Game_Mode {
        OVERWORLD,
        HEROES_OF_SOKOBAN,
    };
    
    public enum Program_Mode {
        GAME,
        EDITOR,
        MENU,
        START_SCREEN
    }
    
    public Program_Mode ProgramMode;
    
    // Start is called before the first frame update
    void Start()
    {
        ProgramMode = Program_Mode.START_SCREEN;
    }
    
    public GameObject []StartScreenItems;
    public GameObject ThePlayer;
    
    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ProgramMode == Program_Mode.MENU)
            {
                ProgramMode = Program_Mode.GAME;
            }
            else if (ProgramMode == Program_Mode.GAME)
            {
                ProgramMode = Program_Mode.MENU;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ProgramMode == Program_Mode.START_SCREEN)
            {
                ProgramMode = Program_Mode.GAME;
            }
        }
        
        if (ProgramMode == Program_Mode.START_SCREEN)
        {
            for (u32 Index = 0;
                 Index < StartScreenItems.Length;
                 ++Index)
            {
                StartScreenItems[Index].SetActive(true);
            }
            ThePlayer.SetActive(false);
        }
        else
        {
            for (u32 Index = 0;
                 Index < StartScreenItems.Length;
                 ++Index)
            {
                StartScreenItems[Index].SetActive(false);
            }
            ThePlayer.SetActive(true);
        }
    }
    
    public b32 ShouldMove()
    {
        return ProgramMode == Program_Mode.GAME;
    }
}

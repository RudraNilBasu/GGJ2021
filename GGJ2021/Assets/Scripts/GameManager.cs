using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public CameraController Camera;
    
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
    
    public GameObject MiddleDoor;
    
    
    public enum PLAYER_CURRENT_ROOM {
        ROOM_NONE,
        ROOM_1,
        ROOM_2
    }
    public CSG CSG_Room1, CSG_Room2;
    public PLAYER_CURRENT_ROOM CurrentPlayerRoom;
    
    public enum DOOR_STATE {
        DOOR_NONE,
        DOOR_OPEN,
        DOOR_CLOSED
    }
    public DOOR_STATE DoorState;
    
    public enum GAMEPLAY_STATE {
        UNKNOWN,
        INIT,
        FIRST_COLLECTED,
        WAITING_FOR_NOTES,
        READ_FIRST_NOTE,
        SECOND_NOTE_SENT,
    }
    public GAMEPLAY_STATE GamePlayState;
    
    public enum READING_STATE {
        NONE,
        CAN_READ,
        READING
    }
    
    public READING_STATE ReadingState;
    
    public LayerMask LetterLayer;
    public Text LetterText, InteractText;
    
    b32 EKeyProcessed;
    
    public GameObject []StartScreenItems;
    public GameObject ThePlayer;
    
    public GameObject Letter, Letter_2;
    
    AudioSource DoorAudioSource;
    public AudioClip DoorKnock, DoorOpen, DoorClose;
    
    public Transform Letter_1_Table_Position;
    
    // Start is called before the first frame update
    void Start()
    {
        ProgramMode = Program_Mode.START_SCREEN;
        DoorState = DOOR_STATE.DOOR_CLOSED;
        GamePlayState = GAMEPLAY_STATE.INIT;
        ReadingState = READING_STATE.NONE;
        
        DoorAudioSource = MiddleDoor.GetComponent<AudioSource>();
        // TODO: DELETE
        //Letter.GetComponent<Animation>().Play("SendLetter");
    }
    
    
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
            
            StartCoroutine(DoorOpenEffect());
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
        
        v3 PlayerPosition = ThePlayer.transform.position; // TODO(@rudra): Every frame?
        
        if (Rectangle.IsInRectangle(CSG_Room1.GetRect(), PlayerPosition))
        {
            CurrentPlayerRoom = PLAYER_CURRENT_ROOM.ROOM_1;
        }
        
        if (Rectangle.IsInRectangle(CSG_Room2.GetRect(), PlayerPosition))
        {
            CurrentPlayerRoom = PLAYER_CURRENT_ROOM.ROOM_2;
        }
        
        if (CurrentPlayerRoom == PLAYER_CURRENT_ROOM.ROOM_2 && DoorState == DOOR_STATE.DOOR_OPEN && GamePlayState == GAMEPLAY_STATE.INIT)
        {
            CloseDoor();
            
            StartCoroutine(OpenDoorWhenCollected());
        }
        
        if (CurrentPlayerRoom == PLAYER_CURRENT_ROOM.ROOM_1 && DoorState == DOOR_STATE.DOOR_OPEN && GamePlayState == GAMEPLAY_STATE.FIRST_COLLECTED)
        {
            CloseDoor();
            GamePlayState = GAMEPLAY_STATE.WAITING_FOR_NOTES;
            Letter.GetComponent<Animation>().Play("SendLetter");
        }
        
        if (Camera.HitWithRaycast(LetterLayer) && ThePlayer.GetComponent<Player>().CollidedWithLetter)
        {
            if (ReadingState == READING_STATE.NONE)
            {
                ReadingState = READING_STATE.CAN_READ;
            }
        }
        else
        {
            ReadingState = READING_STATE.NONE;
        }
        
        EKeyProcessed = false;
        
        if (ReadingState == READING_STATE.READING && Input.GetKeyDown(KeyCode.E))
        {
            EKeyProcessed = true;
            ReadingState = READING_STATE.CAN_READ;
            
            if (GamePlayState == GAMEPLAY_STATE.WAITING_FOR_NOTES)
            {
                GamePlayState = GAMEPLAY_STATE.READ_FIRST_NOTE;
                Letter.transform.position = Letter_1_Table_Position.position;
                
                GamePlayState = GAMEPLAY_STATE.SECOND_NOTE_SENT;
                Letter_2.GetComponent<Animation>().Play("SendLetter");
            }
        }
        
        
        if (ReadingState == READING_STATE.CAN_READ && Input.GetKeyDown(KeyCode.E) && !EKeyProcessed)
        {
            EKeyProcessed = true;
            ReadingState = READING_STATE.READING;
        }
        
        
        
        if (ReadingState == READING_STATE.CAN_READ)
        {
            InteractText.text = "E";
        }
        else
        {
            InteractText.text = "";
        }
        
        LetterText.text = "";
        if (ReadingState == READING_STATE.READING)
        {
            if (GamePlayState == GAMEPLAY_STATE.WAITING_FOR_NOTES)
            {
                LetterText.text = "It's the guy behind the other door\nHe's the one doing all these";
            }
            else if (GamePlayState == GAMEPLAY_STATE.SECOND_NOTE_SENT)
            {
                LetterText.text = "Together, we can destroy it";
            }
        }
    }
    
    
    public b32 ShouldMove()
    {
        return ProgramMode == Program_Mode.GAME && ReadingState != READING_STATE.READING;
    }
    
    IEnumerator DoorOpenEffect()
    {
        DoorAudioSource.clip = DoorKnock;
        
        yield return new WaitForSeconds(5.0f);
        MiddleDoor.GetComponent<AudioSource>().Play();
        
        yield return new WaitForSeconds(5.0f);
        MiddleDoor.GetComponent<AudioSource>().Play();
        
        yield return new WaitForSeconds(7.0f);
        OpenDoor();
    }
    
    public void OpenDoor()
    {
        DoorState = DOOR_STATE.DOOR_OPEN;
        DoorAudioSource.clip = DoorOpen;
        MiddleDoor.GetComponent<AudioSource>().Play();
        MiddleDoor.GetComponent<Animation>().Play("DoorOpen");
    }
    
    
    public void CloseDoor()
    {
        DoorState = DOOR_STATE.DOOR_CLOSED;
        DoorAudioSource.clip = DoorClose;
        MiddleDoor.GetComponent<AudioSource>().Play();
        MiddleDoor.GetComponent<Animation>().Play("DoorClose");
    }
    
    IEnumerator OpenDoorWhenCollected()
    {
        // TODO(@rudra): NONONONONONONONONONONONONONO
        // When you reach it, it's gone, plus HORROR
        yield return new WaitForSeconds(10.0f);
        
        GamePlayState = GAMEPLAY_STATE.FIRST_COLLECTED;
        
        if (DoorState == DOOR_STATE.DOOR_CLOSED && CurrentPlayerRoom == PLAYER_CURRENT_ROOM.ROOM_2)
        {
            OpenDoor();
        }
    }
}

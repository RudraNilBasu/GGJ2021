using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

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
        ROOM_2,
        ROOM_3,
    }
    public CSG CSG_Room1, CSG_Room2, CSG_Room3;
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
        READ_SECOND_NOTE,
        THIRD_NOTE_SENT,
        READ_THIRD_NOTE,
        FOURTH_NOTE_SENT,
        READ_FOURTH_NOTE,
        LEVER_SEQUENCE,
        LEVER_ACTIVATED,
        RIGHT_DOOR_OPENED,
        START_CRAZINESS,
        CREDITS,
    }
    public GAMEPLAY_STATE GamePlayState;
    
    public enum READING_STATE {
        NONE,
        CAN_READ,
        READING
    }
    
    public READING_STATE ReadingState;
    
    public enum LEVER_STATE {
        NONE,
        LEVER_CANNOT_ACCESS,
        LEVER_CAN_ACCESS,
        LEVER_ON,
    }
    public LEVER_STATE LeverState;
    
    public LayerMask LetterLayer, LeverLayer;
    public Text LetterText, InteractText;
    
    b32 EKeyProcessed;
    
    public GameObject []StartScreenItems;
    public GameObject BlackScreen, Credits;
    public GameObject ThePlayer;
    
    public GameObject Letter_1, Letter_2, Letter_3, Letter_4;
    
    AudioSource DoorAudioSource;
    public AudioClip DoorKnock, DoorOpen, DoorClose;
    
    public Transform Letter_1_Table_Position, Letter_2_Table_Position, Letter_3_Table_Position, Letter_4_Table_Position;
    
    public GameObject Lever;
    
    public GameObject []AllLightsToTurnOffAfterLever;
    
    public GameObject RightDoor, RightDoorPivot;
    public AudioClip RightDoorThumping, RightDoorOpen;
    
    public AudioClip PowerDownAudio, VCR_Low, VCR_Mid, VCR_High;
    AudioSource GM_AudioSource;
    
    i32 OtherNotesProgressHash, GameCompleteHash;
    
    public GameObject Human;
    
    public AudioMixer GameMixer;
    
    f32 VCR_Current_Volume, VCR_Target_Volume, T;
    
    public GameObject ReadingBackground;
    
    public AudioSource ELEKTRICITY;
    
    public GameObject FirstCoffinLight, SecondCoffinLight, ThirdCoffinLight;
    b32 FirstCoffinActivated, SecondCoffinActivated, ThirdCoffinActivated;
    
    f32 CoffinMinDistance;
    
    // TODO(@rudra): HACXXXX
    public b32 StartedAlready;
    
    // Start is called before the first frame update
    void Start()
    {
        ProgramMode = Program_Mode.START_SCREEN;
        DoorState = DOOR_STATE.DOOR_CLOSED;
        GamePlayState = GAMEPLAY_STATE.INIT;
        ReadingState = READING_STATE.NONE;
        
        DoorAudioSource = MiddleDoor.GetComponent<AudioSource>();
        
        Lever.transform.parent.gameObject.SetActive(false);
        GM_AudioSource = GetComponent<AudioSource>();
        
        OtherNotesProgressHash = 0;
        GameCompleteHash = (1 << (i32)Letter.Tag.OTHER_1) | (1 << (i32)Letter.Tag.OTHER_2) | (1 << (i32)Letter.Tag.OTHER_3) | (1 << (i32)Letter.Tag.OTHER_4);
        
        Human.SetActive(false);
        
        BlackScreen.SetActive(false);
        Credits.SetActive(false);
        
        GameMixer.GetFloat("GM_Volume", out VCR_Current_Volume);
        VCR_Target_Volume = VCR_Current_Volume;
        T = 0.0f;
        
        FirstCoffinActivated = false;
        SecondCoffinActivated = false;
        ThirdCoffinActivated = false;
        
        FirstCoffinLight.SetActive(false);
        SecondCoffinLight.SetActive(false);
        ThirdCoffinLight.SetActive(false);
        
        CoffinMinDistance = 14.0f;
        
        StartedAlready = false;
        
#if IGNORED
        GameMixer.GetFloat("GM_Volume", out OriginalTrainVolume);
        GameMixer.SetFloat("GM_Volume", OriginalTorchSound);
#endif
    }
    
    
    // Update is called once per frame
    void Update()
    {
        //print(GamePlayState);
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
                ELEKTRICITY.Play();
            }
            
            StartCoroutine(DoorOpenEffect());
        }
        
        
        if (Input.GetKeyDown(KeyCode.Escape) && ProgramMode == Program_Mode.START_SCREEN)
        {
            Application.Quit(0);
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
        
        if (Rectangle.IsInRectangle(CSG_Room3.GetRect(), PlayerPosition))
        {
            CurrentPlayerRoom = PLAYER_CURRENT_ROOM.ROOM_3;
            
            if (GamePlayState == GAMEPLAY_STATE.RIGHT_DOOR_OPENED)
            {
                GamePlayState = GAMEPLAY_STATE.START_CRAZINESS;
                Human.transform.rotation = Quaternion.Euler(0.0f, 25.0f, 0.0f);
                Camera.IncreaseVHSVerticalOffset(0.04f);
                
                VCR_Target_Volume = 9.0f;
                T = 0.0f;
#if IGNORED
                f32 CurrentPlayTime = GM_AudioSource.time;
                
                GM_AudioSource.clip = VCR_Mid;
                GM_AudioSource.loop = true;
                GM_AudioSource.Play();
                GM_AudioSource.time = CurrentPlayTime;
#endif
            }
        }
        
        
        if (CurrentPlayerRoom == PLAYER_CURRENT_ROOM.ROOM_2 && DoorState == DOOR_STATE.DOOR_OPEN && GamePlayState == GAMEPLAY_STATE.INIT)
        {
            CloseDoor();
            
            StartCoroutine(ActivateCoffins());
            //StartCoroutine(OpenDoorWhenCollected());
        }
        
        
        if (CurrentPlayerRoom == PLAYER_CURRENT_ROOM.ROOM_2 && DoorState == DOOR_STATE.DOOR_OPEN && GamePlayState == GAMEPLAY_STATE.READ_FOURTH_NOTE)
        {
            CloseDoor();
            GamePlayState = GAMEPLAY_STATE.LEVER_SEQUENCE;
            StartCoroutine(WaitAndStartCoffinsAndInitLever());
        }
        
        if (CurrentPlayerRoom == PLAYER_CURRENT_ROOM.ROOM_1 && DoorState == DOOR_STATE.DOOR_OPEN && GamePlayState == GAMEPLAY_STATE.FIRST_COLLECTED)
        {
            CloseDoor();
            GamePlayState = GAMEPLAY_STATE.WAITING_FOR_NOTES;
            
            Letter_1.GetComponent<Animation>().Play("SendLetter");
            Human.SetActive(true);
        }
        
        if (Camera.HitWithRaycast(LetterLayer) && ThePlayer.GetComponent<Player>().CollidedWithLetter)
        {
            Animation LetterAnimation = Camera.LookingAt.transform.parent.gameObject.GetComponent<Animation>();
            if (LetterAnimation.isPlaying)
            {
            }
            else
            {
                
                if (ReadingState == READING_STATE.NONE)
                {
                    ReadingState = READING_STATE.CAN_READ;
                }
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
            
            Letter _L = Camera.LookingAt.GetComponent<Letter>();
            if (_L.LetterTag == Letter.Tag.FIRST)
            {
                if (GamePlayState == GAMEPLAY_STATE.WAITING_FOR_NOTES)
                {
                    GamePlayState = GAMEPLAY_STATE.READ_FIRST_NOTE;
                    Letter_1.transform.position = Letter_1_Table_Position.position;
                    
                    GamePlayState = GAMEPLAY_STATE.SECOND_NOTE_SENT;
                    Letter_2.GetComponent<Animation>().Play("SendLetter");
                }
            }
            else if (_L.LetterTag == Letter.Tag.SECOND)
            {
                if (GamePlayState == GAMEPLAY_STATE.SECOND_NOTE_SENT)
                {
                    GamePlayState = GAMEPLAY_STATE.READ_SECOND_NOTE;
                    Letter_2.transform.position = Letter_2_Table_Position.position;
                    
                    GamePlayState = GAMEPLAY_STATE.THIRD_NOTE_SENT;
                    Letter_3.GetComponent<Animation>().Play("SendLetter");
                }
            }
            else if (_L.LetterTag == Letter.Tag.THIRD)
            {
                if (GamePlayState == GAMEPLAY_STATE.THIRD_NOTE_SENT)
                {
                    GamePlayState = GAMEPLAY_STATE.READ_THIRD_NOTE;
                    Letter_3.transform.position = Letter_3_Table_Position.position;
                    
                    GamePlayState = GAMEPLAY_STATE.FOURTH_NOTE_SENT;
                    Letter_4.GetComponent<Animation>().Play("SendLetter");
                }
            }
            else if (_L.LetterTag == Letter.Tag.FOURTH)
            {
                if (GamePlayState == GAMEPLAY_STATE.FOURTH_NOTE_SENT)
                {
                    GamePlayState = GAMEPLAY_STATE.READ_FOURTH_NOTE;
                    Letter_4.transform.position = Letter_4_Table_Position.position;
                    
                    StartCoroutine(OpenDoorAndEnableLever());
                }
            }
            else
            {
                // NOTE(@rudra): Case for notes on the other side
                OtherNotesProgressHash = OtherNotesProgressHash | 1 << (i32)_L.LetterTag;
                if (OtherNotesProgressHash == GameCompleteHash)
                {
                    StartCoroutine(FinishingSequence());
                }
            }
            
        }
        
        
        if (ReadingState == READING_STATE.CAN_READ && Input.GetKeyDown(KeyCode.E) && !EKeyProcessed)
        {
            EKeyProcessed = true;
            ReadingState = READING_STATE.READING;
        }
        
        ReadingBackground.SetActive(ReadingState == READING_STATE.READING);
        
        
        // NOTE(@rudra): Lever
        if (Camera.HitWithRaycast(LeverLayer) && ThePlayer.GetComponent<Player>().CollidedWithLever)
        {
            if (LeverState != LEVER_STATE.LEVER_ON)
            {
                LeverState = LEVER_STATE.LEVER_CAN_ACCESS;
            }
        }
        else
        {
            if (LeverState != LEVER_STATE.LEVER_ON)
            {
                LeverState = LEVER_STATE.LEVER_CANNOT_ACCESS;
            }
        }
        
        
        // NOTE(@rudra): Interact text
        if (ReadingState == READING_STATE.CAN_READ || LeverState == LEVER_STATE.LEVER_CAN_ACCESS)
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
            Letter _L = Camera.LookingAt.GetComponent<Letter>();
            if (_L != null)
            {
                LetterText.text = _L.Contents;
            }
        }
        
        // NOTE(@rudra): Handle Lever
        if (LeverState == LEVER_STATE.LEVER_CAN_ACCESS && Input.GetKeyDown(KeyCode.E))
        {
            LeverState = LEVER_STATE.LEVER_ON;
            Lever.GetComponent<Animation>().Play("LeverOn");
            
            StartCoroutine(LightOffSequence());
        }
        
        // NOTE(@rudra): Right door opening sequence
        if (GamePlayState == GAMEPLAY_STATE.LEVER_ACTIVATED)
        {
            f32 SqDistanceFromPlayer = DistanceSq(RightDoor.transform.position, ThePlayer.transform.position);
            
            if (SqDistanceFromPlayer <= 30.0f)
            {
                GamePlayState = GAMEPLAY_STATE.RIGHT_DOOR_OPENED;
                RightDoorPivot.GetComponent<Animation>().Play("RightDoorOpen");
                
                AudioSource RightDoorAudio = RightDoor.GetComponent<AudioSource>();
                RightDoorAudio.clip = RightDoorOpen;
                RightDoorAudio.Play();
            }
        }
        
        // NOTE(@rudra): Ending sequence
        if (CurrentPlayerRoom == PLAYER_CURRENT_ROOM.ROOM_3)
        {
            
        }
        
        GameMixer.GetFloat("GM_Volume", out VCR_Current_Volume);
        VCR_Current_Volume = Mathf.Lerp(VCR_Current_Volume, VCR_Target_Volume, T);
        GameMixer.SetFloat("GM_Volume", VCR_Current_Volume);
        T += 1.0f*Time.deltaTime;
        
        // NOTE(@rudra): Coffin sequence
        if (FirstCoffinActivated)
        {
            f32 SqDistanceFromPlayer = DistanceSq(FirstCoffinLight.transform.position, ThePlayer.transform.position);
            if (SqDistanceFromPlayer <= CoffinMinDistance)
            {
                StartCoroutine(WaitAndActivateSecondCoffin());
            }
        }
        if (SecondCoffinActivated)
        {
            f32 SqDistanceFromPlayer = DistanceSq(SecondCoffinLight.transform.position, ThePlayer.transform.position);
            if (SqDistanceFromPlayer <= CoffinMinDistance)
            {
                StartCoroutine(WaitAndActivateThirdCoffin());
            }
        }
        if (ThirdCoffinActivated && !StartedAlready)
        {
            f32 SqDistanceFromPlayer = DistanceSq(ThirdCoffinLight.transform.position, ThePlayer.transform.position);
            if (SqDistanceFromPlayer <= CoffinMinDistance)
            {
                StartCoroutine(WaitAndActivateOpenDoor());
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
        MiddleDoor.GetComponent<Animation>().Play("DoorClose");
        StartCoroutine(PlayDoorCloseAudio());
    }
    
    IEnumerator OpenDoorAndEnableLever()
    {
        yield return new WaitForSeconds(5.0f);
        //Lever.transform.parent.gameObject.SetActive(true);
        OpenDoor();
    }
    
    IEnumerator PlayDoorCloseAudio()
    {
        yield return new WaitForSeconds(0.2f);
        DoorAudioSource.clip = DoorClose;
        MiddleDoor.GetComponent<AudioSource>().Play();
    }
    
    // TODO(@rudra): Unreachable code, remove
    /*
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
    */
    
    IEnumerator LightOffSequence()
    {
        //CloseDoor();
        
        yield return new WaitForSeconds(1.5f);
        
        for (u32 Index = 0;
             Index < AllLightsToTurnOffAfterLever.Length;
             ++Index)
        {
            AllLightsToTurnOffAfterLever[Index].SetActive(false);
        }
        GM_AudioSource.clip = PowerDownAudio;
        GM_AudioSource.Play();
        ELEKTRICITY.Stop();
        
        FirstCoffinLight.SetActive(false);
        SecondCoffinLight.SetActive(false);
        ThirdCoffinLight.SetActive(false);
        
        AudioSource RightDoorAudio = RightDoor.GetComponent<AudioSource>();
        RightDoorAudio.loop = false;
        RightDoorAudio.Stop();
        
        GamePlayState = GAMEPLAY_STATE.LEVER_ACTIVATED;
        
        yield return new WaitForSeconds(5.0f);
        
        GM_AudioSource.clip = VCR_Low;
        GM_AudioSource.loop = true;
        GM_AudioSource.Play();
    }
    
    f32 Square(f32 A)
    {
        f32 Result = A*A;
        
        return(Result);
    }
    
    f32 DistanceSq(v3 A, v3 B)
    {
        f32 Result;
        
        Result = Square(A.x - B.x) + Square(A.y - B.y) + Square(A.z - B.z);
        
        return(Result);
    }
    
    IEnumerator ActivateCoffins()
    {
        yield return new WaitForSeconds(5.0f);
        FirstCoffinActivated = true;
        FirstCoffinLight.SetActive(true);
    }
    
    IEnumerator WaitAndActivateSecondCoffin()
    {
        yield return new WaitForSeconds(10.0f);
        FirstCoffinLight.SetActive(false);
        FirstCoffinActivated = false;
        SecondCoffinActivated = true;
        SecondCoffinLight.SetActive(true);
    }
    
    
    IEnumerator WaitAndActivateThirdCoffin()
    {
        yield return new WaitForSeconds(10.0f);
        SecondCoffinLight.SetActive(false);
        SecondCoffinActivated = false;
        ThirdCoffinActivated = true;
        ThirdCoffinLight.SetActive(true);
    }
    
    
    IEnumerator WaitAndActivateOpenDoor()
    {
        // TODO(@rudra): Find a better way, too tired now
        StartedAlready = true;
        
        yield return new WaitForSeconds(10.0f);
        ThirdCoffinLight.SetActive(false);
        ThirdCoffinActivated = false;
        
        yield return new WaitForSeconds(10.0f);
        
        GamePlayState = GAMEPLAY_STATE.FIRST_COLLECTED;
        
        if (DoorState == DOOR_STATE.DOOR_CLOSED && CurrentPlayerRoom == PLAYER_CURRENT_ROOM.ROOM_2)
        {
            OpenDoor();
        }
    }
    
    IEnumerator WaitAndStartCoffinsAndInitLever()
    {
        yield return new WaitForSeconds(5.0f);
        
        FirstCoffinLight.SetActive(true);
        SecondCoffinLight.SetActive(true);
        ThirdCoffinLight.SetActive(true);
        
        yield return new WaitForSeconds(10.0f);
        
        AudioSource RightDoorAudio = RightDoor.GetComponent<AudioSource>();
        RightDoorAudio.clip = RightDoorThumping;
        RightDoorAudio.loop = true;
        RightDoorAudio.Play();
        
        yield return new WaitForSeconds(10.0f);
        Lever.transform.parent.gameObject.SetActive(true);
    }
    
    
    IEnumerator FinishingSequence()
    {
        f32 CurrentPlayTime = GM_AudioSource.time;
        
        VCR_Target_Volume = 19.0f;
        T = 0.0f;
#if IGNORED
        GM_AudioSource.clip = VCR_High;
        GM_AudioSource.loop = true;
        GM_AudioSource.Play();
        GM_AudioSource.time = CurrentPlayTime;
#endif
        
        Camera.IncreaseVHSVerticalOffset(0.1f);
        
        yield return new WaitForSeconds(5.5f);
        
        Camera.IncreaseVHSVerticalOffset(0.9f);
        
        yield return new WaitForSeconds(15.5f);
        
        GM_AudioSource.Stop();
        BlackScreen.SetActive(true);
        
        yield return new WaitForSeconds(5.5f);
        
        Credits.SetActive(true);
        
        GamePlayState = GAMEPLAY_STATE.CREDITS;
        
        Scene CurrentScene = SceneManager.GetActiveScene();
        
        yield return new WaitForSeconds(10.0f);
        
        SceneManager.LoadScene(CurrentScene.name);
    }
}

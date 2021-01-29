using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public GameObject LookingAt;
    // public enum LookingAt { Letter, PaintDoor };

    private VHS_Effect VHSEffect;

    // Vertical Offset
    private float MinVHSVerticalOffset, MaxVHSVerticalOffset, VHSVerticalOffsetTime;
    private float MinTextureIntensityOffset, MaxTextureIntensityOffset, VHSTextureIntensityTime;
    private bool ShouldTransitionVHSOffset;
    private bool ShouldTransitionTextureIntensity;

    // Start is called before the first frame update
    void Start()
    {
        VHSEffect = GetComponent<VHS_Effect>();

        // Vertical Offset
        MinVHSVerticalOffset = 0.0f;
        MaxVHSVerticalOffset = 0.0f;
        VHSVerticalOffsetTime = 0.0f;
        VHSTextureIntensityTime = 0.0f;
        ShouldTransitionVHSOffset = false;

        MinTextureIntensityOffset = 0.995f;
        MaxTextureIntensityOffset = 0.0f;
        VHSTextureIntensityTime = 0.0f;
        ShouldTransitionTextureIntensity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldTransitionVHSOffset)
        {
            VHSVerticalOffsetTime += 0.5f * Time.deltaTime;
            float vhs_vertical_offset = Mathf.Lerp(MinVHSVerticalOffset,
                    MaxVHSVerticalOffset, VHSVerticalOffsetTime);
            VHSEffect._verticalOffset = vhs_vertical_offset;
        }

        if (ShouldTransitionTextureIntensity)
        {
            VHSTextureIntensityTime += 0.5f * Time.deltaTime;
            float vhs_texture_offset = Mathf.Lerp(MinTextureIntensityOffset,
                    MaxTextureIntensityOffset, VHSTextureIntensityTime);
            VHSEffect._textureIntensity = vhs_texture_offset;
        }
    }

    public void SetAudioListener(bool Value)
    {
        GetComponent<AudioListener>().enabled = Value;
    }

    public bool HitWithRaycast(LayerMask LayersToHit)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,
                    transform.TransformDirection(Vector3.forward), out hit,
                    Mathf.Infinity, LayersToHit))
        {
            LookingAt = hit.collider.gameObject;
            Debug.DrawRay(transform.position,
                    transform.TransformDirection(Vector3.forward) * hit.distance,
                    Color.yellow);
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position,
                    transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            return false;
        }
    }

    public void EnableVHSEffect()
    {
        VHSEffect.enabled = true;
        // 0.063f is a good mid way point
        // 0.179f is high! probably during the ending sequence!
        // VHSEffect._verticalOffset = 0.179f;
    }

    public void DisableVHSEffect()
    {
        VHSEffect.enabled = false;
    }

    public void IncreaseVHSVerticalOffset(float _MaxVerticalOffset)
    {
        MaxVHSVerticalOffset = _MaxVerticalOffset;
        ShouldTransitionVHSOffset = true;
    }

    public void StopVHSVerticalOffset()
    {
        VHSEffect._verticalOffset = 0.0f;
        ShouldTransitionVHSOffset = false;
    }

    public void IncreaseNoiseOffset(float _MaxNoiseOffset)
    {
        // 0.6f;
        ShouldTransitionTextureIntensity = true;
        MaxTextureIntensityOffset = _MaxNoiseOffset;
    }

    public void StopNoiseOffset()
    {
        VHSEffect._textureIntensity = MinTextureIntensityOffset;
        ShouldTransitionTextureIntensity = false;
    }
}

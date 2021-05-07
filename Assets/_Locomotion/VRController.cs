using System.Collections;
using System.Collections.Generic;
using Pixelplacement; 
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Valve.VR;

public class VRController : MonoBehaviour
{
    public float m_Gravity = 30.0f;
    public float m_Sensitivity = 0.1f;
    public float m_MaxSpeed = 1.0f;
    public float m_RotateIncrement = 90;

    public SteamVR_Action_Boolean m_RotatePressL = null;
    public SteamVR_Action_Boolean m_RotatePressR = null;
	//public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;

    private float m_Speed = 0.0f;

    private CharacterController m_CharacterController = null;
    public Transform m_CameraRig = null;
    public Transform m_Head = null;
	public float intensity = 0.75f;
    public float duration = 0.5f;
    public Volume volume = null;
	private Vignette vignette = null;
	
    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
		if (volume.profile.TryGet(out Vignette vignette))
            this.vignette = vignette;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        HandleHeight();
        CalculateMovement();
        //SnapRotation();
		VignetteApply();
    }

    private void HandleHeight()
    {
        // Get the head in local space
        float headHeight = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_CharacterController.height = headHeight;

        // Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_CharacterController.height / 2;
        newCenter.y += m_CharacterController.skinWidth;

        // Move capsule in local space
        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;

        // Apply
        m_CharacterController.center = newCenter;
    }

    private void CalculateMovement()
    {
        // Figure out movement orientation
        Quaternion orientation = CalculateOrientation();
        Vector3 movement = Vector3.zero;

        // If not moving
        if (m_MoveValue.axis.magnitude == 0)
            m_Speed = 0;

        // Add, clamp
        m_Speed += m_MoveValue.axis.magnitude * m_Sensitivity;
        m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

        // Orientation, and gravity
        movement += orientation * (m_Speed * Vector3.forward);
        movement.y -= m_Gravity * Time.deltaTime;
		


        // Apply
        m_CharacterController.Move(movement * Time.deltaTime);
    }

    private Quaternion CalculateOrientation()
    {
        float rotation = Mathf.Atan2(m_MoveValue.axis.x, m_MoveValue.axis.y);
        rotation *= Mathf.Rad2Deg;

        Vector3 orientationEuler = new Vector3(0, m_Head.eulerAngles.y + rotation + 35, 0);
        return Quaternion.Euler(orientationEuler);
    }

    private void SnapRotation()
    {
        float snapValue = 0.0f;

        if (m_RotatePressL.GetStateDown(SteamVR_Input_Sources.RightHand))
            snapValue = -Mathf.Abs(m_RotateIncrement);

        if (m_RotatePressR.GetStateDown(SteamVR_Input_Sources.RightHand))
            snapValue = Mathf.Abs(m_RotateIncrement);

        transform.RotateAround(m_Head.position, Vector3.up, snapValue);
    }
	private void VignetteApply()
	{
		if (m_MoveValue.axis.x != 0)
		{
			 Tween.Value(0, intensity, ApplyValue, duration, 0);
		}
		else
		{
			Tween.Value(intensity, 0, ApplyValue, duration, 0);
		}


	}
	private void ApplyValue(float value)
    {
        // We need to override the original intensity
        vignette.intensity.Override(value);
    }
}

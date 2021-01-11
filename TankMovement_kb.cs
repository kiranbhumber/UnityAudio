using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    
    private string m_MovementAxisName;     //       m_MovementAxisName = "Vertical" + m_PlayerNumber;
    private string m_TurnAxisName;         // m_TurnAxisName = "Horizontal" + m_PlayerNumber;
    private Rigidbody m_Rigidbody;

    private float m_MovementInputValue;   // store the value of input.GetAxis
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         // starting pitch


    private float m_RigidMag;
    private float m_TurnVal;
    private float m_ClampedVal;
    private float m_magInterp;

    float minFwdPitch = 1f;
    float maxFwdPitch = 2f;
    float minRotPitch = 0f;
    float maxRotPitch = 1f;
    bool is_stationary;

    float engineVol = 1f;
    public float fadeTime = 0.1f;
    bool is_fading = false;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
      


    }


    private void OnEnable () // called after awake - enabling the tank
    {
        m_Rigidbody.isKinematic = false; 
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true; // dont want physics effects applied to it when we want it dies, just want it to die 
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
        m_MovementAudio.volume = 0f;




    }
    

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName); // values from -1 to 1
       //(m_MovementInputValue);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);
       //print(m_TurnInputValue);

        EngineAudio ();
        print(m_MovementAudio.volume);




    }


    //***** ADDED BY KB **** //
    private void EnginePitch (float speed, float turn, float minFwdPitch, float maxFwdPitch, float minTurnPitch, float maxTurnPitch, bool is_stationary) {

        float minPitch = minFwdPitch; 
        float maxPitch = maxFwdPitch;
        float minRotPitch = minTurnPitch;
        float maxRotPitch = maxTurnPitch;


        float pitchMod = maxPitch - minPitch;
        float engineFwdVelocity = minPitch + (speed / 1f) * pitchMod;
  
    
        float rotPitchMod = maxRotPitch - minRotPitch;
        float engineRotVelocity = minTurnPitch + turn * rotPitchMod;

        float totalEnginePitch = engineFwdVelocity + engineRotVelocity;
        //print(engineFwdVelocity);
        //print(totalEnginePitch);
        //print(engineRotVelocity);

        m_MovementAudio.clip = m_EngineDriving;
        m_MovementAudio.pitch = totalEnginePitch;
        //print(m_MovementAudio.pitch);
        //m_MovementAudio.Play();

        if (!m_MovementAudio.isPlaying)
        {

            m_MovementAudio.Play();

        }

    }

 

    IEnumerator FadeEngine(float startValue, float EndValue, float duration)
    {
        float currentTime = 0;
        //print(duration);

        while (currentTime <= duration)
        {
            

            m_MovementAudio.volume =  Mathf.Lerp(startValue, EndValue, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;

        }

        
    }


    private void EngineAudio()
    {
        

        if (m_PlayerNumber == 1) 
        {


        //print(is_stationary);

        if (is_stationary && is_fading == false ) { // if stationary and hasnt faded out 
      
          {
            print("fading out");
            StartCoroutine(FadeEngine(m_MovementAudio.volume, 0.7f, fadeTime));
            is_fading = true;


          }
            
        }

        else if (!is_stationary && is_fading == true)  // is not stationary and has previously faded out
        {
            print("fading in");
            StartCoroutine(FadeEngine(0.7f, engineVol, fadeTime));
            is_fading = false;
           
        

        }

            if (Mathf.Abs(m_MovementInputValue) > 0.1f)
            {
                if (Mathf.Abs(m_TurnInputValue) < 0.1f)
                {

                    is_stationary = false;
        
                    EnginePitch(m_RigidMag, m_TurnVal, minFwdPitch, maxFwdPitch, minRotPitch, maxRotPitch, is_stationary);

                  

                    //print("1 going forward");

                }

                else
                {
                    //print("3 driving and turning");

                    float minFwdPitch = 1f;
                    float maxFwdPitch = 1.5f;
                    float minRotPitch = 0.5f;
                    float maxRotPitch = 1.5f;

                    is_stationary = false;
                   
                    EnginePitch(m_RigidMag, m_TurnVal, minFwdPitch, maxFwdPitch, minRotPitch, maxRotPitch, is_stationary);
                   

                }

            }

            else if (Mathf.Abs(m_MovementInputValue) < 0.1f)
            
                if (Mathf.Abs(m_TurnInputValue) > 0.1f)
                {
                    //print("2 We are turning");
                    float minFwdPitch = 0f;
                    float maxFwdPitch = 1f;
                    float minRotPitch = 1f;
                    float maxRotPitch = 1.5f;

                    is_stationary = false;
                    
                    EnginePitch(m_RigidMag, m_TurnVal, minFwdPitch, maxFwdPitch, minRotPitch, maxRotPitch, is_stationary);
                  
                }

                else
                {
                    //print("4 stationary");


                    is_stationary = true;
                    

                    EnginePitch(m_RigidMag, m_TurnVal, minFwdPitch, maxFwdPitch, minRotPitch, maxRotPitch, is_stationary);
          
                }
            }

        }
    
    //******* END OF ADDED BY KB

    private void FixedUpdate() // running every physics step
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime; // proportional to a second instead of per frame
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement); // apply to the rigidBody - MovePosition - we're adding movement to the current position of the tank
        // pass value to engine Audio function
        m_RigidMag = movement.magnitude * 5; // *** MODIFIED BY KB

    
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        m_TurnVal = Mathf.Abs(turn) / 3.55f;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0); // just a float, have to make it a vector 3 with angles
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation); // have to multiply, not add to it 
     

    }
}


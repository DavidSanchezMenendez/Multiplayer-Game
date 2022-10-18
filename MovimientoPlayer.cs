using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoPlayer : MonoBehaviour
{
    private const float NORMAL_FOV = 60f;
    private const float HOOK_FOV = 120f;


    public float tiempodash = 2;
    
    public float speed = 12f;
    Vector3 velocity;
    Vector3 hookShootPosition;
    private Vector3 inersia;

   // private CameraFovOnHook cameraFov;
    public Transform player;
    //private Disparo disparo;
    private float hookshotSize;


    public float JumpSpeed;
    public Transform debugHitPoint;
    public Transform HookShotTransform;
    public float CharacterVelocityY;
    float time = 0;
    Vector3 move;
    public Camera cam;
    public ParticleSystem luz;
    public bool EnMuro = false;
    public float gravedad;
    public ParticleSystem GannchoLuz;
    public int id;
    public string username;
    public CharacterController controller;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;
    private bool[] inputs;
    private float yVelocity = 0;
    static private State state;
    public AudioClip Nigga;
    public AudioSource Sonido;
    private enum State
    {
        Normal,
        inHook,
        HookShootTrown,


    }
    
    private void Awake()
    {
        state = State.Normal;
        //cameraFov = cam.GetComponent<CameraFovOnHook>();
    }
    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
        Sonido = GetComponent<AudioSource>();
    }

    
    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;

        inputs = new bool[6];
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void Update()
    {

       
        switch (state)
        {
            default:
            case State.Normal:
                
                
                Vector2 _inputDirection = Vector2.zero;
                if (inputs[0])
                {
                    _inputDirection.y += 1;
                }
                if (inputs[1])
                {
                    _inputDirection.y -= 1;
                }
                if (inputs[2])
                {
                    _inputDirection.x -= 1;
                }
                if (inputs[3])
                {
                    _inputDirection.x += 1;
                }

                Move(_inputDirection);
               // HandleHookShootStart(_inputDirection);
                break;

            case State.inHook:
               // HandleHooksShootMoviment();

                break;
            case State.HookShootTrown:
                //MovimientoDelPersonaje();

               // HandleHookShootThrow();
                break;
                //case State.GetShoot:

                // break;



        }

        
    }

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection)
    {
        if (inputs[6])
        {
            //Sonido.PlayOneShot(Nigga);
           // ServerSend.Sonido(this);

        }
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

       // ServerSend.PlayerPosition(this);
       // ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }
    private void HandleHookShootStart(Vector2 _inputDirection)
    {
        if (inputs[5])
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit raycastHit))//Golpea a cualqueiro cosa
            {
                debugHitPoint.position = raycastHit.point;//que la posuicion de el objeto sea igual a donde estemos apuntando con el raycast
                hookShootPosition = raycastHit.point; //decimos que el vector 3 hookshotposituon sea igual al hit de el raycast
                hookshotSize = 0f;
                HookShotTransform.gameObject.SetActive(true);
                HookShotTransform.localScale = Vector3.zero; //resetea el agncho, aunque no lo veas porque esta desactivado la scala sigue siendo la del gancho que has lanzado anterioremente, asi que la devolvemos a 0 para que se al activasre por segunda vez empieze de 0
                state = State.HookShootTrown;//Lanzar la cuerda

            }
        }
    }
    public bool AtterptPickItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }
        itemAmount++;
        return true;
    }
}
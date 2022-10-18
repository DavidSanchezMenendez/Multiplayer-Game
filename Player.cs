using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool pegarse = true;
    private const float NORMAL_FOV = 60f;
    private const float HOOK_FOV = 120f;
    private bool inhook = false;



   
   
    Vector3 hookShootPosition;
    private Vector3 inersia;
    bool moverse = true;
    // private CameraFovOnHook cameraFov;
    public Transform player;
    //private Disparo disparo;
    private float hookshotSize;
    public Transform shootOrigin;
    public float health;
    public float maxHealth=100f;

    Vector2 _inputDirection;

    public float JumpSpeed;
    public Transform debugHitPoint;
    public Transform HookShotTransform;
    public float CharacterVelocityY;
    float time = 0;
    bool dejargancho = false;
    Vector3 move;
    public Camera cam;
    public ParticleSystem luz;
    public bool EnMuro = false;
   
    public ParticleSystem GannchoLuz;
    public int id;
    public string username;
    public CharacterController controller;
    
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
   
    private bool[] inputs;
    private float yVelocity = 0;
    static private State state;
    private bool firstHook = true;
    public GameObject instanciar;
   
    
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
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;

        inputs = new bool[9];
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        
        HookShotTransform.gameObject.SetActive(false);
        hookshotSize = 0;

        if (health <= 0f)
        {
            return;
        }
        

       

        switch (state)
        {

            default:
            case State.Normal:


                 _inputDirection = Vector2.zero;
                if (inputs[0]&& moverse)
                {
                    _inputDirection.y += 1;
                    
                }
                if (inputs[1] && moverse)
                {
                    _inputDirection.y -= 1;
                }
                if (inputs[2] && moverse)
                {
                    _inputDirection.x -= 1;
                }
                if (inputs[3] && moverse)
                {
                    _inputDirection.x += 1;
                }

                Move(_inputDirection);
                 //HandleHookShootStart(_inputDirection);
                break;

            case State.inHook:

                 //HandleHooksShootMoviment(_inputDirection);

                break;
            case State.HookShootTrown:
                //MovimientoDelPersonaje();

                // HandleHookShootThrow();
                break;
                //case State.GetShoot:

                // break;



        }

        
    }

    private void Teleports(Vector3 _viewDirection)
    {

        if (Physics.Raycast(cam.transform.position, _viewDirection, out RaycastHit tp, 3f))
        {
            if (tp.transform.CompareTag("TP1") && inputs[5] )
            {
            controller.enabled = false;// SI el controller esta activado no puede teleportarse
            transform.position = new Vector3(0f, 10f, 0f);
            controller.enabled = true;//Activo de vuelta
            }
            if (tp.transform.CompareTag("PanaTriste") && inputs[5])
            {
                controller.enabled = false;// SI el controller esta activado no puede teleportarse
                transform.position = new Vector3(-960.229f, -3978.031f, 105.8683f);
                controller.enabled = true;//Activo de vuelta
            }

            if (tp.transform.CompareTag("Spawn") && inputs[5])
            {
                controller.enabled = false;// SI el controller esta activado no puede teleportarse
                transform.position = new Vector3(26.88f, 1010.1f, -59.67f);
                controller.enabled = true;//Activo de vuelta
            }


        }


    }

    public void Gancho(Vector3 _viewDirection)
    {

        Teleports(_viewDirection);
        if (inputs[7])
        {
            if (Physics.Raycast(cam.transform.position, _viewDirection, out RaycastHit raycastHit))
            {
                Debug.Log(raycastHit.transform.tag);
                Instantiate(instanciar, raycastHit.point, Quaternion.identity);
                
                ServerSend.InstanciarObjeto(this, raycastHit.point);
            }
        }
        if (inputs[6])
        {
            controller.enabled = false;// SI el controller esta activado no puede teleportarse
            transform.position = new Vector3(26.88f, 1012.1f, -59.67f);
            controller.enabled = true;//Activo de vuelta

            ServerSend.PlayerSonido(this);
        }

        if (inputs[5] && !dejargancho)//Todo lo del agancho
        {
            
            if (Physics.Raycast(cam.transform.position, _viewDirection, out RaycastHit raycastHit, 100f))//Golpea a cualqueiro cosa
            {



                if (raycastHit.transform.CompareTag("Mapa"))
                {
                   
                    moverse = false;

                    Vector3 aa;
                    yVelocity = 0;

                    aa = raycastHit.point; //decimos que el vector 3 hookshotposituon sea igual al hit de el raycast
                    if (firstHook)
                    {
                        hookShootPosition = aa;
                        firstHook = false;
                    }





                    Vector3 HookShootDirection = (hookShootPosition - transform.position).normalized;//la direccion a la que tiene que ir 
                    float velocidadaMaxima = 10;
                    float velocidadMinima = 1;
                    float HookShootSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookShootPosition), velocidadMinima, velocidadaMaxima);//te da la distancia desde tu hasta el punto donde has disparadao
                    float hooksshotspeedMultipler = 2f;
                    Vector3 _moveDirection = HookShootDirection * HookShootSpeed * hooksshotspeedMultipler * Time.deltaTime;



                    controller.Move(_moveDirection);

                    //Pegar aqui codigo para que se vea el gancho en el servidor



                    float reachHookshotPositionDistance = 10f;
                    inersia = HookShootDirection * HookShootSpeed * 0.045f;

                    if (inputs[4])//Si la distancia de nuestro transform es menor  o = a 1 que es el reachhookdistance pasa a State normal
                    {

                        yVelocity = jumpSpeed;
                        //dejargancho = true;
                    }

                }



            }

        }
        else
        {
            moverse = true;
            firstHook = true;
            dejargancho = false;
            EnMuro = false;
        }





    }


    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection)
    {


        //dejargancho = true;
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            //dejargancho = false;
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;


            }
        }

        if (!EnMuro)
        {
            yVelocity += gravity;
        }





        _moveDirection.y = yVelocity;

        _moveDirection += inersia;
        controller.Move(_moveDirection);


        if (inersia.magnitude >= 0)
        {
            float emuje = 3f;

            inersia -= inersia * emuje * Time.deltaTime;

            if (inersia.magnitude < 0.1f)
            {

                inersia = Vector3.zero;

            }


            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);

        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("Mapa"))
        {
            EnMuro = true;
            yVelocity = 0;
        }
        if (hit.transform.CompareTag("Invisible"))
        {
            transform.position = new Vector3(26.88f, 1010.1f, -59.67f);
           
        }

    }
    



    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
       

    }
  
    private void ResetGravedad()
    {
        yVelocity = 0;
    }
    public void Shoot(Vector3 _viewDirection)
    {
        
        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit))
        {
            
            if (_hit.collider.CompareTag("Player"))
            {
                
               
                hookShootPosition = _hit.point;

                _hit.transform.GetComponent<Player>().Hited(hookShootPosition);
               
            }
        }
    }
    public void Hited(Vector3 hookShootPosition)
    {
        
        Vector3 HookShootDirection = (hookShootPosition - transform.position).normalized;//la direccion a la que tiene que ir
        inersia = -HookShootDirection /3;
        
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRespawned(this);


    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            controller.enabled = false;
            transform.position = new Vector3(0f, 10f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f);

        health = maxHealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }
   

}

using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;



public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float currentSpeed; //Variable que define la velocidad del player
    public float jumpForce; //Define la fuerza de salto
    public bool useLinearVelocity; //Si es verdadero, usará el movimiento físico por motor de aceleración, si no, el de fuerza de empuje
    Vector3 movement; //lVel del playerRb normalizado
    Vector3 horizontalVelocity; //velocidad vectorial en eje "x" y eje "y"

    public float horizontalSpeed; //magnitud velocidad en eje "x" junto a eje "z"
    public float verticalSpeed; // magnitud velocidad en eje "y"
    public float overallSpeed; //magnitud velocidad total
    public Vector3 direction; //dirección de movimiento
    public float rawDX; //magnitud de velocidad x
    public float rawDZ; //magnitud de velocidad z

    public float maxWalkSpeed; //Valor que mide la velocidad a la que se limita el movimiento al caminar.
    public float maxSprintSpeed;
    public float currentMaxSpeed;  //Valor que mide la velocidad a la que se limita el movimiento de forma absoluta.

    [Header("Reference Variables")]
    public Rigidbody playerRb; //Almacén de referencia del rb, una vez relleno permite modificar sus variables internas
    public Vector2 moveInput; //Almacén de los valores de pulsación de movimiento (0, 1, -1)
    public AudioSource sfxSource; //Ref al altavoz del player, para reproducir sonidos
    public PlayerInteractor playerInteractor; //Ref al script, permite acceder a su lista de sonidos públicos.

    [Header("Alternative Values")]
    public float sprintSpeed; // Variable que describe la Velocidad extra al esprintar
    public float walkSpeed; //Variable que sirve para recordar la velocidad base.
    public float dashSpeed; //Velocidad de la mecánica del "Dash"

    [Header("Checks")]
    private bool groundCheck; //Bool que determina si el pj puede saltar o no (cuando está en el suelo puede)
    public bool isSprinting; //Bool que determina el estado de sprint.

    
   

    #region func definition

    

    public static float Lerp(float initialSpeed, float finalSpeed, float interpolationFactor)
    {
        

        return initialSpeed + (finalSpeed - initialSpeed) * interpolationFactor;
        

    }







    #endregion





    void Start()
    {
        walkSpeed = currentSpeed;
        isSprinting = false;
        groundCheck = false;

        Debug.Log("debuglogfunciona");
    }

    // Update is called once per frame
    void Update()
    {
        //Ejecuta todo lo que depende del procesador y ejecuta el input
        //CinematicMove();
        SpeedCheck();
        //SpeedCheck2();
    }

    private void FixedUpdate()
    {
        //FixedUpdate ejecuta todo lo que tenga que ver con el motor de físicas
        PhysicalMove();


        

        horizontalVelocity = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z);

        // The speed on the x-z plane ignoring any speed
        horizontalSpeed = horizontalVelocity.magnitude;
        // The speed from gravity or jumping
        verticalSpeed = playerRb.linearVelocity.y;
        // The overall speed
        overallSpeed = playerRb.linearVelocity.magnitude;

        direction = horizontalVelocity.normalized;




        rawDX = playerRb.linearVelocity.x;
        rawDZ = playerRb.linearVelocity.z;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundCheck = true; //Al chocar con el suelo se devuelve la posibilidad de saltar
        }
    }

    void CinematicMove()
    {
        //Se usa transform.Translate para moverse (dirección * velocidad)
        transform.Translate(Vector3.forward * currentSpeed * moveInput.y * Time.deltaTime);
        transform.Translate(Vector3.right * currentSpeed * moveInput.x * Time.deltaTime);
    }

    void PhysicalMove()
    {

        if (useLinearVelocity == false)
        {
            /*movement = new Vector3(moveInput.x, 0f, moveInput.y);
            movement.Normalize();
            playerRb.AddForce(movement * currentSpeed,ForceMode.VelocityChange);*/




            /*
            playerRb.AddForce(Vector3.forward * moveInput.y * speed, ForceMode.Force);
            playerRb.AddForce(Vector3.right * moveInput.x * speed, ForceMode.Force);*/


            playerRb.AddForce(Vector3.right * currentSpeed/2 * moveInput.x,ForceMode.VelocityChange);
            playerRb.AddForce(Vector3.forward * currentSpeed/2 * moveInput.y,ForceMode.VelocityChange);


            
        }
        else
        {

            playerRb.linearVelocity = new Vector3(moveInput.x * currentSpeed, playerRb.linearVelocity.y, moveInput.y * currentSpeed);

        }

    }
    void SpeedCheck()
    {
        if (isSprinting == true && horizontalSpeed > currentMaxSpeed)
        {
            horizontalSpeed = currentMaxSpeed;

            //horizontalVelocity = new Vector3(direction.x * maxWalkSpeed,0,direction.z *maxWalkSpeed);
        }
        else if (isSprinting == false && horizontalSpeed > maxWalkSpeed)
        {
            horizontalSpeed = maxWalkSpeed;
            //horizontalVelocity = new Vector3(direction.x * maxWalkSpeed,0,direction.z *maxWalkSpeed);
        }

    }
    void SpeedCheck2()
    {
        if (isSprinting == true && horizontalSpeed > currentMaxSpeed)
        {
            Mathf.Clamp(horizontalSpeed, 0f, currentMaxSpeed);
        }else if (isSprinting == false && horizontalSpeed > maxWalkSpeed)
        {
            Mathf.Clamp(horizontalSpeed, 0f, maxWalkSpeed);
        }
        }
    void Jump()
        {
            groundCheck = false; //Al saltar, se quita la posibilidad de volver a saltar hasta tocar suelo
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //Reproducir sonido de salto: accede al player interactor y busca el sonido exacto en la lista
            sfxSource.PlayOneShot(playerInteractor.sfxList[0]);
        }

    void Sprint()
    {
        if (isSprinting == true )
        {
             
            /* Mathf.Clamp(playerRb.linearVelocity.magnitude, 0, maxSpeed);*/

            currentSpeed = sprintSpeed;

            /*if (speed>= maxSpeed)
            {
                speed = maxSpeed;
            }*/
        }
        if (isSprinting == false)
        {


            currentSpeed = walkSpeed;

          /*  if (speed >= maxWalkSpeed)
            {
                speed = maxWalkSpeed;
            }*/
        }
    }

    void Dash()
    {
        playerRb.AddForce(moveInput.x, playerRb.linearVelocity.y, moveInput.y * dashSpeed, ForceMode.VelocityChange);
        
    }


    #region Input Methods
    //Declaramos los métodos de input (eventos de unity)
    //Siguen esta estructura: public void NombreAccion(referencia al contexto = botones de la acción)
    public void OnMove(InputAction.CallbackContext context)
    {
        //Se rellena la variable de referencia con el valor de cualquier contexto
        moveInput = context.ReadValue<Vector2>();
        
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Si aprieto el botón...
            //Y si estoy en el suelo...
            if (groundCheck == true)
            {
                Jump();
            }
            
        }
        
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
       

        if (context.performed)
        {
            isSprinting = true;
        }
        if (context.canceled)
        {
            isSprinting = false;
            
        }

        Sprint();

    }




    #endregion

    #region InputActionSystem








    #endregion


  


}

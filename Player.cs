using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerState))]
[RequireComponent(typeof(PlayerDie))]
public class Player : MonoBehaviour
{
    //Sets the overall damping and sensitivity of the mouse
    [System.Serializable]
    public class MouseInput
    {
        public Vector2 Damping;
        public Vector2 Sensitivity;
        public bool LockMouse;
    }

    [SerializeField] SwatSoldier settings;
    [SerializeField] MouseInput MouseControl;
    [SerializeField] AudioController footSteps;
    [SerializeField] float minimalMoveThreshold;

    public PlayerAim playerAim;

    Vector3 previousPosition;

    private CharacterController m_MoveController;

    private PlayerState m_PlayerState;
    public PlayerState PlayerState
    {
        get
        {
            if (m_PlayerState == null)
                m_PlayerState = GetComponent<PlayerState>();
            return m_PlayerState;
        }
    }

    private PlayerDie m_PlayerDie;
    public PlayerDie PlayerDie
    {
        get
        {
            if (m_PlayerDie == null)
                m_PlayerDie = GetComponent<PlayerDie>();
            return m_PlayerDie;
        }
    }


    private PlayerShoot m_PlayerShoot;
    public PlayerShoot PlayerShoot
    {
        get
        {
            if (m_PlayerShoot == null)
                m_PlayerShoot = GetComponent<PlayerShoot>();
            return m_PlayerShoot;
        }
    }

    //this sets the movement to equal whatever component has the MoveController script attached
    public CharacterController MoveController {
        get
        {
            if (m_MoveController == null)
                m_MoveController = GetComponent<CharacterController>();
            return m_MoveController;
        }
    }
            
    InputController playerInput;
    Vector2 mouseInput;

    //this sets the localplayer variable in the gamemanager script to equal whatever object this script is attached towards
	void Awake ()
    {
        playerInput = GameManager.Instance.InputController;
        GameManager.Instance.Localplayer = this;
        if(MouseControl.LockMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
	}
	
    //this will move the player in the intended riection
	void FixedUpdate ()
    {
        if (!PlayerDie.IsAlive)
            return;

        if (MenuScript.GameIsPaused == false)
        {
            Move();
            LookAround();
        }
    }

    void Move()
    {
        float moveSpeed = settings.RunSpeed;
        if (playerInput.IsWalking)
            moveSpeed = settings.WalkSpeed;
        if (playerInput.IsSprinting)
            moveSpeed = settings.SprintSpeed;
        if (playerInput.IsCrouched)
            moveSpeed = settings.CrouchSpeed;

        Vector2 direction = new Vector2(playerInput.Vertical * moveSpeed, playerInput.Horizontal * moveSpeed);

        MoveController.SimpleMove(transform.forward * direction.x + transform.right * direction.y);

        if (Vector3.Distance(transform.position, previousPosition) > minimalMoveThreshold)
            footSteps.Play();

        previousPosition = transform.position;
    }

    private void LookAround()
    {
        mouseInput.x = Mathf.Lerp(mouseInput.x, playerInput.MouseInput.x, 1f / MouseControl.Damping.x);
        mouseInput.y = Mathf.Lerp(mouseInput.y, playerInput.MouseInput.y, 1f / MouseControl.Damping.y);
        transform.Rotate(Vector3.up, mouseInput.x * MouseControl.Sensitivity.x);
        playerAim.SetRoation(mouseInput.y * MouseControl.Sensitivity.y);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "EndLevel")
        {
            SceneManager.LoadScene("Scenes/End");
        }
    }
}

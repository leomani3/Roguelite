using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float dashSpeed;

    private Vector2 moveVec;
    private bool dashing = false;

    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private Animator animator;
    // Start is called before the first frame update

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.PlayerControls.Dash.performed += ctx => OnDash();

        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dashing)
        {
            moveVec = playerInput.PlayerControls.Move.ReadValue<Vector2>();
        }
    }

    private void FixedUpdate()
    {
        //rb.MovePosition(rb.position + (moveVec * speed * Time.fixedDeltaTime));
        rb.velocity = moveVec * speed * Time.fixedDeltaTime;
        if (moveVec == Vector2.zero)
        {
            animator.SetBool("Running", false);
        }
        else
        {
            if (moveVec.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 0);
            }
            else if(moveVec.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 0);
            }
            animator.SetBool("Running", true);
        }
    }


    public void OnDash()
    {
        StartCoroutine(Dash());
    }

    public IEnumerator Dash()
    {
        dashing = true;
        float baseSpeed = speed;
        moveVec.Normalize();
        speed = dashSpeed;
        yield return new WaitForSeconds(0.1f);
        speed = baseSpeed;
        dashing = false;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
}

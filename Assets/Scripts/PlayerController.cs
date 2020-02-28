using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private PlayerInput playerInput;
    // Start is called before the first frame update

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.PlayerControls.Move.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove(Vector2 direction)
    {
        Debug.Log(direction);
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

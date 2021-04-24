using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Controls controls;
    Rigidbody2D rb;

    bool facingRight;
    bool falling;

    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.Enable();
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerMovement = controls.Gameplay.Movement.ReadValue<Vector2>();
        if (playerMovement != Vector2.zero)
        {
            rb.velocity = playerMovement;
            if (playerMovement.x != 0)
                facingRight = playerMovement.x == 1 ? true : false;
        }
        else
            rb.velocity = Vector2.zero;

        






            


    }
}

using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public Rigidbody rb;

   // public float fbforce = 110f;
   // public float sbsmovement = 120f;
    public float umovement = 55f;
    public float damper = 0.4f; //put damper on non-primary touch direction
    public float jump = 115f; //Jump height
    public float fall = 2f;
    public float speed_x, speed_y, touch_x, touch_y;


    // FixedUpdate is called once per frame and deals with Physics
    private void FixedUpdate ()
    {

        // Updates app with remote inputs
        OVRInput.FixedUpdate();
        
            // If player presses touch button/spacebar from ground then jump in the air 
            if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.Space)) && rb.position.y < 4.2f)
            {
                rb.AddExplosionForce(jump * Time.deltaTime, rb.position, 7f, 9f, ForceMode.Impulse);
            }
            // If player is in the air, increase gravity
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector2.down * Physics2D.gravity.y * fall * Time.deltaTime);

            }
            // If touch position on x/y touchpad is found
            if ((OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).x != 0) && (OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y != 0))
            {
                touch_x = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).x;
                touch_y = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;

                if (touch_x < 0 && touch_y < 0)
                {
                    //If negative x/y then compare values
                    if (touch_y <= touch_x)
                    {
                        //fbforce
                        rb.AddForce(new Vector3(touch_x * damper, 0, touch_y) * umovement * Time.deltaTime, ForceMode.Impulse);
                        rb.AddTorque(new Vector3(touch_y, 0, touch_x * damper) * umovement * Time.deltaTime, ForceMode.Impulse);
                    }
                    else
                    {
                        //sbs
                        rb.AddForce(new Vector3(touch_x, 0, touch_y * damper) * umovement * Time.deltaTime, ForceMode.Impulse);
                        rb.AddTorque(new Vector3(0, touch_x * damper, touch_y) * umovement * Time.deltaTime, ForceMode.Impulse);
                    }
                }
                else
                {
                    if (touch_y >= touch_x)
                    {
                        //fbforce
                        rb.AddForce(new Vector3(touch_x * damper, 0, touch_y) * umovement * Time.deltaTime, ForceMode.Impulse);
                        rb.AddTorque(new Vector3(touch_y, 0, touch_x * damper) * umovement * Time.deltaTime, ForceMode.Impulse);
                    }
                    else
                    {
                        //sbs
                        rb.AddForce(new Vector3(touch_x, 0, touch_y * damper) * umovement * Time.deltaTime, ForceMode.Impulse);
                        rb.AddTorque(new Vector3(0, touch_x * damper, touch_y) * umovement * Time.deltaTime, ForceMode.Impulse);
                    }
                }
            }
            else
            {
                //get speed on x & y axis
                speed_x = Input.GetAxis("Horizontal");
                speed_y = Input.GetAxis("Vertical");

                //If w is pressed move the z axis forward 500 + Rotate Player
                if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow))
                {
                    //fbforce
                    rb.AddForce(Vector3.forward * umovement * Time.deltaTime, ForceMode.Impulse);
                    rb.AddTorque(new Vector3(speed_y, 0, speed_x) * umovement * Time.deltaTime, ForceMode.Impulse);

                }
                //If s is pressed move the z axis back 500
                if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow))
                {
                    //fbforce
                    rb.AddForce(Vector3.back * umovement * Time.deltaTime, ForceMode.Impulse);
                    rb.AddTorque(new Vector3(speed_y, 0, speed_x) * -umovement * Time.deltaTime, ForceMode.Impulse);
                }

                //If d is pressed move the x axis 500
                if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
                {
                    //sbs
                    rb.AddForce(Vector3.right * umovement * Time.deltaTime, ForceMode.Impulse);
                    rb.AddTorque(new Vector3(0, speed_x, speed_y) * umovement * Time.deltaTime, ForceMode.Impulse);
                }

                //If a is pressed move the x axis -500
                if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
                {
                    //sbs
                    rb.AddForce(Vector3.left * umovement * Time.deltaTime, ForceMode.Impulse);
                    rb.AddTorque(new Vector3(0, speed_x, speed_y) * -umovement * Time.deltaTime, ForceMode.Impulse);
                }
            } 
    }
    
}

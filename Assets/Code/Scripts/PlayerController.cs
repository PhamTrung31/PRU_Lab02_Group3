using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] float torqueAmount = 1f;
    //[SerializeField] float normalSpeed = 20f;
    //[SerializeField] float boostSpeed = 40f;

    //Rigidbody2D rb2d;
    //SurfaceEffector2D surfaceEffector2D;
    //bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        //    rb2d = GetComponent<Rigidbody2D>();
        //    surfaceEffector2D = FindObjectOfType<SurfaceEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //    if (canMove)
        //    {
        //        RotatePlayer();
        //        RespondToBoost();
        //    }
        //}

        //public void DisableControls()
        //{
        //    canMove = false;
        //}

        //void RespondToBoost()
        //{
        //    if (Keyboard.current.upArrowKey.isPressed) 
        //    {
        //        surfaceEffector2D.speed = boostSpeed;
        //    }
        //    else
        //    {
        //        surfaceEffector2D.speed = normalSpeed;
        //    }
        //}

        //void RotatePlayer()
        //{
        //    if (Keyboard.current.leftArrowKey.isPressed) 
        //    {
        //        rb2d.AddTorque(torqueAmount);
        //    }
        //    else if (Keyboard.current.rightArrowKey.isPressed) 
        //    {
        //        rb2d.AddTorque(-torqueAmount);
        //    }
        //}
    }
}

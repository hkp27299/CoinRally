using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObstacle : MonoBehaviour
{
    public MovementBehaviour movementBehaviour;
    public float speed=2f * 5f;
    private void Awake()
    {
        movementBehaviour = new MovementBehaviour(transform, Vector3.forward, speed);//game speed
    }

    private void Update()
    {
        movementBehaviour.Move();
    }
    public void updateSpeed(){
        speed=1.05f*speed;
        Debug.Log("speed updated");
        movementBehaviour = new MovementBehaviour(transform, Vector3.forward, speed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarMovement>() != null)
        {
            
            FindObjectOfType<GameManager>().GameOver();
            FindObjectOfType<SoundManager>().Play("Crash");
            Debug.Log("You Lost the Game!");
        }
    }
}

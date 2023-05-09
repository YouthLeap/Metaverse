using UnityEngine;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour
{
  
    public ControlPanel ControlPanel;
  
    private float _engineForce;
   

    private Vector2 hMove = Vector2.zero;


    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {







        float tempY = 0;
        float tempX = 0;

        // stable forward
        if (hMove.y > 0)
            tempY = -Time.fixedDeltaTime;
        else
            if (hMove.y < 0)
            tempY = Time.fixedDeltaTime;

        // stable lurn
        if (hMove.x > 0)
            tempX = -Time.fixedDeltaTime;
        else
            if (hMove.x < 0)
            tempX = Time.fixedDeltaTime;



    }
        
}

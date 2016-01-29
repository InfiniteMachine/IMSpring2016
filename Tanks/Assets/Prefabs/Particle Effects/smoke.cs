using UnityEngine;
using System.Collections;

public class smoke : MonoBehaviour
{
    public GameObject pt1;
    public GameObject pt2;
    public GameObject player;
    Vector3 current;


    // Use this for initialization
    void Awake()
    {
        current = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float correctionLeft = player.transform.position.x -.5F;
        float correctionRight = player.transform.position.x +.5F;
        float correctionDown = (player.transform.position.y-.25F);
        if (Input.GetKey(KeyCode.D))
        {
                transform.eulerAngles = new Vector3(0, 0, 0);
                transform.position = new Vector3(correctionLeft, correctionDown, player.transform.position.z);
        }

        if (Input.GetKey(KeyCode.A))
        {
                transform.eulerAngles = new Vector3(0, 0, 180);
                //transform.position = current;
                transform.position = new Vector3 (correctionRight, correctionDown, player.transform.position.z);
                
        }
    }
}

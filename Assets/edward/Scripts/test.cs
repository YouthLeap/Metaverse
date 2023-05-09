using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public LineRenderer m_line;
    public Camera m_viewCam;

    Vector3 m_TargetPos;
    bool bClicked = false;
    Vector2 m_OrigPos = Vector2.zero;
    public bool bPick = false;

    public GameObject m_targetObj;
    public GameObject m_plane;
    public Plane plane = new Plane(Vector3.up, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height)
        {
            if (bClicked == false)
            {
                m_OrigPos = Input.mousePosition;
                bClicked = true;
            }
            else
            {
                float fClickDelta = Vector2.Distance(m_OrigPos, Input.mousePosition);
                Debug.Log(fClickDelta.ToString());
                if (fClickDelta < 10f)
                {
                    bPick = true;
                }

            }

            if (bPick)
            {
                float distance = 250f;
                Vector3 pos = Input.mousePosition;
                pos.z = m_viewCam.nearClipPlane;
                Ray ray = m_viewCam.ScreenPointToRay(pos);
                RaycastHit hit;

                if (m_plane.GetComponent<BoxCollider>().Raycast(ray, out hit, distance))
                {
                    m_targetObj.transform.position = ray.GetPoint(distance);
                }
                bPick = false;
            }
        }
        else
        {
            bClicked = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NavigationManager : MonoBehaviour
{
    public LineRenderer m_line;
    public Camera m_viewCam;

    Vector3 m_TargetPos;
    bool bClicked = false;
    Vector2 m_OrigPos = Vector2.zero;

    public GraphicRaycaster m_GraphicRayCaster;
    public EventSystem m_EventSystem;
    PointerEventData m_PointerEventData;

    //public GameObject m_targetObj;
    public GameObject m_plane;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCanvasButtonPressed()) return;

        if (m_line.positionCount == 2)
        {
            ShowNavi();
        }

        if (!FindObjectOfType<MapManager>().m_BigMap.activeSelf) return;

        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height)
        {
            if (!bClicked)
            {
                m_OrigPos = Input.mousePosition;
                bClicked = true;
            }
        }
        if (Input.GetMouseButtonUp(0) && Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height)
        {
            if (bClicked)
            {
                float fClickDelta = Vector2.Distance(m_OrigPos, Input.mousePosition);
                if (fClickDelta < 10f)
                {
                    float distance = 500f;
                    Vector3 pos = Input.mousePosition;
                    pos.z = m_viewCam.nearClipPlane;
                    Ray ray = m_viewCam.ScreenPointToRay(pos);
                    RaycastHit hit;
                    if (m_plane.GetComponent<BoxCollider>().Raycast(ray, out hit, distance))
                    {
                        m_TargetPos = ray.GetPoint(250f);
                        m_TargetPos.y = m_plane.transform.position.y + 15f;
                        //m_targetObj.transform.position = m_TargetPos;
                        Vector3 vec1 = GameManager.instance.mycontroller.gameObject.transform.position;
                        vec1.y = m_plane.transform.position.y + 15f;
                        m_line.positionCount = 2;
                        m_line.SetPosition(0, vec1);
                        m_line.SetPosition(1, m_TargetPos);
                    }
                }
                bClicked = false;
            }
        }
    }

    void ShowNavi()
    {
        Vector3 vec1 = GameManager.instance.mycontroller.gameObject.transform.position;
        vec1.y = m_plane.transform.position.y + 15f;

        float mDelta = Vector3.Distance(m_viewCam.WorldToScreenPoint(vec1), m_viewCam.WorldToScreenPoint(m_TargetPos));
        if(mDelta < 30f)
        {
            m_line.positionCount = 0;
        }
        else
        {
            m_line.SetPosition(0, vec1);
        }
    }

    public void ClearNaviButtonClick()
    {
        m_line.positionCount = 0;
    }

    public bool IsCanvasButtonPressed()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        m_GraphicRayCaster.Raycast(m_PointerEventData, results);

        bool resultIsButton = false;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<CanvasRenderer>())
            {
                resultIsButton = true;
                break;
            }
        }
        return resultIsButton;
    }
}

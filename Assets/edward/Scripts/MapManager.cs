    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public GameObject m_BigMap;
    public Slider m_ZoomSlide;
    public GameObject m_SlideGroup;
    public CameraController camControl;

    public bool bMap;

    private void Awake()
    {
#if UNITY_EDITOR || UNITY_WEBGL || UNITY_STANDALONE
        m_SlideGroup.SetActive(true);
#elif UNITY_ANDROID || UNITY_IOS
        m_SlideGroup.SetActive(false);
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        bMap = false;
    }

    public void SliderChange(float fValue)
    {
        //Debug.Log(fValue);
        camControl.ZoomFactor = (camControl.zoomMax - camControl.zoomMin) / (m_ZoomSlide.maxValue - m_ZoomSlide.minValue) * (fValue - m_ZoomSlide.minValue) + camControl.zoomMin;
        camControl.SetCameraPosition(camControl.cam.transform.localPosition, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.mycontroller.currentState != PlayerController.playerState.chatting && Input.GetKeyDown(KeyCode.B))
        {
            if (m_BigMap.activeSelf)
            {
                m_BigMap.SetActive(false);
                GameManager.instance.ChatBox.SetActive(true);
                bMap = false;
                GameManager.instance.FreeCursor(false);
            }
            else
            {
                m_BigMap.SetActive(true);
                GameManager.instance.ChatBox.SetActive(false);
                bMap = true;
                GameManager.instance.FreeCursor(true);
            }
        }
    }

    public void ShowBigMapButtonClick()
    {
        m_BigMap.SetActive(true);
        GameManager.instance.ChatBox.SetActive(false);
        bMap = true;
        GameManager.instance.FreeCursor(true);
    }

    public void HideBigMapButtonClick()
    {
        m_BigMap.SetActive(false);
        GameManager.instance.ChatBox.SetActive(true);
        bMap = false;
        GameManager.instance.FreeCursor(false);
    }
}

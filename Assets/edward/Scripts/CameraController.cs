using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
	public Camera cam;
	public int m_mode = 0;
	public Vector3 m_DefaultPos = new Vector3(-120f, 250f, -90f);
	public float m_defaultZoom = 567f;
	public float zoomMax = 567f, zoomMin = 100f, zoomStep = 10f, moveStep = 0.01f;
	public float MinX = -862f, MaxX = 1044f, MinY = -558f, MaxY = 421f;
	public float ZoomFactor;

	float fDeltaX, fDeltaY;
	Vector3 vel;
	float currentFingerDistance, previousFingerDistance;
	float speed = 5;
	bool Drag = false;

	public GraphicRaycaster m_GraphicRayCaster;
	public EventSystem m_EventSystem;
	PointerEventData m_PointerEventData;


	void Awake()
	{
		cam = GetComponent<Camera>();
	}

    private void OnEnable()
    {
		InitValues();
	}

    void InitValues()
    {
		Drag = false;
		ZoomFactor = m_defaultZoom;
		cam.transform.localPosition = m_DefaultPos;
		cam.orthographicSize = ZoomFactor;
		FindObjectOfType<MapManager>().m_ZoomSlide.value = (ZoomFactor - zoomMin) * (1f - 0.2f) / (zoomMax - zoomMin) + 0.2f;
	}

	void LateUpdate()
	{
        MouseZoom();
        TouchMoveZoom();
		ZoomCamera();
		MoveCamera();
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



	[UsedImplicitly]
	private void MoveCamera()
	{
		if (IsCanvasButtonPressed()) return;

		if (Input.GetMouseButton(0) && Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height)
		{
			if (Drag == false)
			{
				Drag = true;
			}
		}
		else
		{
			Drag = false;
		}

		if (Drag)
		{
			float rotateHorizontal = Input.GetAxis("Mouse X");
			float rotateVertical = Input.GetAxis("Mouse Y");

			Vector3 pos = Vector3.zero;
			if (m_mode == 0) //main map
            {
				//pos = cam.transform.position + new Vector3(rotateVertical, 0, -rotateHorizontal) * (ZoomFactor - 0) * (1f - 0.2f) / (zoomMax - zoomMin) * moveStep;
				pos = cam.transform.localPosition + new Vector3(-rotateHorizontal, -rotateVertical, 0) * (ZoomFactor - 0) * (1f - 0.2f) / (zoomMax - zoomMin) * moveStep;
			}
			else if (m_mode == 1) //castle
			{
				//pos = cam.transform.position + new Vector3(-rotateHorizontal, 0, -rotateVertical) * (ZoomFactor - 0) * (1f - 0.2f) / (zoomMax - zoomMin) * moveStep;
				pos = cam.transform.localPosition + new Vector3(-rotateHorizontal, -rotateVertical, 0) * (ZoomFactor - 0) * (1f - 0.2f) / (zoomMax - zoomMin) * moveStep;
			}
			else if (m_mode == 2) //car race
			{
				//pos = cam.transform.position + new Vector3(rotateHorizontal, 0, rotateVertical) * (ZoomFactor - 0) * (1f - 0.2f) / (zoomMax - zoomMin) * moveStep;
				pos = cam.transform.localPosition + new Vector3(-rotateHorizontal, -rotateVertical, 0) * (ZoomFactor - 0) * (1f - 0.2f) / (zoomMax - zoomMin) * moveStep;
			}
			SetCameraPosition(pos, false);
		}
	}

	public void SetCameraPosition(Vector3 pos, bool bSet)
    {
		if(bSet)
        {
			FindObjectOfType<MapManager>().m_ZoomSlide.value = (ZoomFactor - zoomMin) * (1f - 0.2f) / (zoomMax - zoomMin) + 0.2f;
		}

		if (m_mode == 0)
		{
			fDeltaX = 230f; fDeltaY = 230f;
		}
		else if (m_mode == 1)
		{
			fDeltaX = 600f; fDeltaY = 400f;
		}
		else if (m_mode == 2)
		{
			fDeltaX = 600f; fDeltaY = 280f;
		}

		float x1 = MinX + (ZoomFactor - zoomMin) / (zoomMax - zoomMin) * fDeltaX;
		float x2 = MaxX - (ZoomFactor - zoomMin) / (zoomMax - zoomMin) * fDeltaX;
		float y1 = MinY + (ZoomFactor - zoomMin) / (zoomMax - zoomMin) * fDeltaY;
		float y2 = MaxY - (ZoomFactor - zoomMin) / (zoomMax - zoomMin) * fDeltaY;
		if (pos.x < x1) pos.x = x1;
		if (pos.x > x2) pos.x = x2;
		if (pos.y < y1) pos.y = y1;
		if (pos.y > y2) pos.y = y2;
		cam.transform.localPosition = Vector3.SmoothDamp(cam.transform.localPosition, pos, ref vel, Time.smoothDeltaTime * speed);
	}

	private void ZoomCamera()
	{
		cam.orthographicSize = ZoomFactor;
    }

	private void MouseZoom()
	{
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			if (ZoomFactor < zoomMax)
			{
				ZoomFactor += zoomStep;   //In
            }
            else
            {
				ZoomFactor = zoomMax;
			}
			SetCameraPosition(cam.transform.localPosition, true);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (ZoomFactor > zoomMin)
			{
				ZoomFactor -= zoomStep; //Out
            }
            else
            {
				ZoomFactor = zoomMin;
			}
			SetCameraPosition(cam.transform.localPosition, true);
		}
	}


	private void TouchMoveZoom()
	{
		if (Input.touchCount > 1 && Input.GetTouch(0).phase == TouchPhase.Moved//chech for 2 fingers on screen
				&& Input.GetTouch(1).phase == TouchPhase.Moved)
		{
			Vector2 touchPosition0 = Input.GetTouch(0).position;//positions for both fingers for pinch zoom in/out
			Vector2 touchPosition1 = Input.GetTouch(1).position;

			currentFingerDistance = Vector2.Distance(touchPosition0, touchPosition1);//distance between fingers

			//MANUAL ZOOM

			if (currentFingerDistance > previousFingerDistance)
			{
				if (ZoomFactor > zoomMin)
				{
					ZoomFactor -= zoomStep; //Out
					SetCameraPosition(cam.transform.localPosition, true);
				}
			}
			else if (currentFingerDistance < previousFingerDistance)
			{
				if (ZoomFactor < zoomMax)
				{
					ZoomFactor += zoomStep;   //In
					SetCameraPosition(cam.transform.localPosition, true);
				}
			}

			previousFingerDistance = currentFingerDistance;

		}
		else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary)// 
		{
		}
	}
}
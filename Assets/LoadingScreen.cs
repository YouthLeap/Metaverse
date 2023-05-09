using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    private void Awake()
    {
        instance = this;
    }
    public Image loading;
    // Start is called before the first frame update
    void Start()
    {
        LoadingStop();
    }

    // Update is called once per frame
    void Update()
    {
        loading.transform.rotation *= Quaternion.Euler(0,0,-45*Time.deltaTime);
    }

    public void LoadingStart()
    {
        gameObject.SetActive(true);
    }

    public void LoadingStop()
    {
        gameObject.SetActive(false);
    }

}

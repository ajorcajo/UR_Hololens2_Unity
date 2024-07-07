using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOrigin : MonoBehaviour
{
    public GameObject origin;
    public GameObject sphere;
    public GameObject PanelOrigin;
    public GameObject robot;
    private bool show;

    // Start is called before the first frame update
    void Start()
    {
        show = true;
    }

    public void originsetted()
    {
        show = false;
    }

    public void setorigin()
    {
        show = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(show == false)
        {
            origin.SetActive(false);
            PanelOrigin.SetActive(false);
            sphere.SetActive(false);
        }
        else
        {
            origin.SetActive(true);
            PanelOrigin.SetActive(true);
            sphere.SetActive(true);
        }
    }
}

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
    public Transform robot_pos;
    public Transform OriginRef;

    // Start is called before the first frame update
    void Start()
    {
        show = true;
    }

    public void originsetted()
    {
        robot.transform.position = OriginRef.position;
        OriginRef.position = robot.transform.position;
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
            origin.SetActive(true);
            PanelOrigin.SetActive(false);
            sphere.SetActive(true);
        }
        else
        {
            origin.SetActive(true);
            PanelOrigin.SetActive(true);
            sphere.SetActive(true);
        }
    }
}

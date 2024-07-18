using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTCP : MonoBehaviour
{
    // Variables para las posiciones y orientaciones
    public Vector3 originalPosition; // Posici�n en metros
    //public Vector3 originalRotation; // Rotaci�n en radianes

    // Update is called once per frame
    void Update()
    {
        // Convertir las posiciones y rotaciones
        Vector3 convertedPosition = ConvertPosition(originalPosition);
       // Vector3 convertedRotation = ConvertRotationToDegrees(originalRotation);

        // Asignar las posiciones y rotaciones convertidas al objeto
        transform.position = convertedPosition;
        //transform.eulerAngles = convertedRotation;
    }
    Vector3 ConvertPosition(Vector3 position)
    {
        // Invertir los signos de cada componente
        return new Vector3(-position.x, -position.y, -position.z);
    }

    // Funci�n para convertir la rotaci�n de radianes a grados e invertir signos
    /*Vector3 ConvertRotationToDegrees(Vector3 rotation)
    {
        // Convertir de radianes a grados e invertir los signos
        return new Vector3(-rotation.x * Mathf.Rad2Deg, -rotation.y * Mathf.Rad2Deg, -rotation.z * Mathf.Rad2Deg);
    }*/
}

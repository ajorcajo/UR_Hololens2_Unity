using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

using UnityEngine.UI;
using UnityEngine;

using TMPro;

public class PointsManagement : MonoBehaviour
{

    public string[] coordinates = new string[6] { "0.0", "0.0", "0.0", "0.0", "0.0", "0.0" };
    public static Vector3 targetpos = new Vector3();
    public Vector3 rot = new Vector3();
    public Quaternion newRotation = new Quaternion();
    public Vector3 eulerDegrees = new Vector3();
    public Vector3 eulerRadians = new Vector3();
    public static Vector3 offsetpos = new Vector3();
    private static bool completed = false;

    //Pruebas
    public Vector3 real_rot_Degree = new Vector3();
    public Vector3 real_rot_Radian = new Vector3();


    public GameObject PanelProgramation;
    public GameObject Origin;
    public GameObject Origin_BaseRef;
    public Transform PointsParent;
    // -------------------- UTF8Encoding -------------------- //
    private UTF8Encoding utf8 = new UTF8Encoding();

    // Ejemplos de Puntos, para cogerlos como referencia y poder duplicarlos luego
    public GameObject MoveL_Point;
    public GameObject MoveJ_Point;
    public GameObject MoveP_Point;
    public GameObject Gripper_Point;

    // Objetos de representaci�n en la tabla que tambien se van a duplicar
    public GameObject MoveL_Point_table;
    public GameObject MoveJ_Point_table;
    public GameObject MoveP_Point_table;
    public GameObject Gripper_Point_table;

    public Transform MoveL_Point_Pos;
    public Transform MoveJ_Point_Pos;
    public Transform MoveP_Point_Pos;
    public Transform Origin_Pos;
    public Transform tcp;

    // Contadores
    public int cont_points = 0;
    public int cont_timer = 0;
    public int cont = 1;
    public int aviso_type = 0;

    //TIPO CLASE PUNTO
    public class point
    {
        // Type is 0 = MoveL ; 1 = MoveJ ; 2 = MoveP
        public int type;
        public GameObject PointObject;
        public LineRenderer line;
    }

    public point[] Points;
    public int segments = 20;

    //public LineRenderer[] lineRenderer;
    //public GameObject[] lines;
    public Material lineMaterial;
    LineRenderer newline;

    public bool bandera_sendcommand;
    public bool gripper_state;

    //Avisos
    public TextMeshProUGUI aviso;
    public GameObject avisos;

    // �Dentro de esfera?
    public GameObject esferaGrande;
    public GameObject esferaPeque�a;
    public bool isInside = false;
    public float distancia = 0f;
    public float radioGrande = 0f;
    public float radioPeque�a = 0f;


    // Start is called before the first frame update
    void Start()
    {
        // Ensure Points array is initialized
        Points = new point[0];
        AddToArray(Origin,0);
        inicio();
        bandera_sendcommand = false;
        gripper_state = false;
        completed = false;
    }

    void inicio()
    {
        // Ejemplos de Puntos
        MoveL_Point.SetActive(false);
        MoveJ_Point.SetActive(false);
        MoveP_Point.SetActive(false);
        Gripper_Point.SetActive(false);

        // Objetos de representaci�n en la tabla
        MoveL_Point_table.SetActive(false);
        MoveJ_Point_table.SetActive(false);
        MoveP_Point_table.SetActive(false);
        Gripper_Point_table.SetActive(false);
    }
    public void createmoveL()
    {
        if (isInside == false)
        {
            aviso_type = 2;
        }
        else
        {
            if (cont_points < 2)
            {
                Vector3 newPosition = Points[cont_points - 1].PointObject.transform.position + new Vector3(0.2f, 0, -0.2f);
                GameObject newObject = Instantiate(MoveL_Point, newPosition, Points[cont_points - 1].PointObject.transform.rotation);
                AddToArray(newObject, 0);
                newObject.SetActive(true);
                newObject.transform.SetParent(PointsParent, false);
                newObject.transform.position = newPosition;

            }
            else
            {
                aviso_type = 1;
            }
        }
    }

    public void createmoveJ()
    {
        if (isInside == false)
        {
            aviso_type = 2;
        }
        else
        {
            if (cont_points < 2)
            {
                Vector3 newPosition = Points[cont_points - 1].PointObject.transform.position + new Vector3(0.2f, 0, -0.2f);
                GameObject newObject = Instantiate(MoveJ_Point, newPosition, Points[cont_points - 1].PointObject.transform.rotation);
                AddToArray(newObject, 1);
                newObject.SetActive(true);
                newObject.transform.SetParent(PointsParent, false);
                newObject.transform.position = newPosition;
            }
            else
            {
                aviso_type = 1;
            }
        }
    }
    public void createmoveP()
    {

        if (isInside == false)
        {
            aviso_type = 2;
        }
        else
        {
            if (cont_points < 2)
            {
                Vector3 newPosition = Points[cont_points - 1].PointObject.transform.position + new Vector3(0.2f, 0, -0.2f);
                GameObject newObject = Instantiate(MoveP_Point, newPosition, Points[cont_points - 1].PointObject.transform.rotation);
                AddToArray(newObject, 2);
                newObject.SetActive(true);
                newObject.transform.SetParent(PointsParent, false);
                newObject.transform.position = newPosition;
            }
            else
            {
                aviso_type = 1;
            }
        }
    }
    public void createGripper()
    {
        if(isInside == false)
        {
            aviso_type = 2;
        }
        else { if (cont_points < 2)
            {
                Vector3 newPosition = Points[cont_points - 1].PointObject.transform.position;
                GameObject newObject = Instantiate(Gripper_Point, newPosition, Points[cont_points - 1].PointObject.transform.rotation);
                AddToArray(newObject, 3);
                newObject.SetActive(true);
                newObject.transform.parent = Points[cont_points - 2].PointObject.transform;
                newObject.transform.position = newPosition;
            }
            else
            {
                aviso_type = 1;
            }
        }
    }


    void AddToArray(GameObject newObject, int typemovement)
    {
        // Crea un nuevo array con una longitud incrementada en 1
        point[] newArray = new point[Points.Length + 1];

        // Copia los elementos del array original al nuevo array
        for (int i = 0; i < Points.Length; i++)
        {
            newArray[i] = Points[i];
        }

        // Se a�ade el nuevo class punto (para inicializarlo)
        newArray[Points.Length] = new point();
        // A�ade el nuevo objeto al final del nuevo array
        newArray[Points.Length].PointObject = newObject;
        newArray[Points.Length].line = newline;
        //A�ade el linerenderer al objeto
        newArray[Points.Length].line = newArray[Points.Length].PointObject.AddComponent<LineRenderer>();

        // Configuraci�n comun a todos los tipos de movimiento del LineRenderer
        newArray[Points.Length].line.material = lineMaterial;
        newArray[Points.Length].line.startWidth = 0.01f;
        newArray[Points.Length].line.endWidth = 0.01f;

        switch (typemovement)
        {
            case 0: //MOVEL
                newArray[Points.Length].type = 0;
                // Configurar el LineRenderer
                newArray[Points.Length].line.positionCount = 2; // Inicialmente sin puntos
                break;
            case 1: //MOVEJ
                newArray[Points.Length].type = 1;
                // Configurar el LineRenderer
                newArray[Points.Length].line.positionCount = segments;
                break;
            case 2: //MOVEP
                newArray[Points.Length].type = 2;
                // Configurar el LineRenderer
                newArray[Points.Length].line.positionCount = segments;
                break;
            case 3: //GRIPPER
                newArray[Points.Length].type = 3;
                break;
        }
        // Asigna el nuevo array al array original
        Points = newArray;
        cont_points++;
    }

    public void RemoveLastPoint()
    {
        // Verifica si el arreglo tiene al menos un elemento
        if (Points.Length > 1)
        {
            // Obt�n la referencia al �ltimo elemento del arreglo
            point lastPoint = Points[Points.Length - 1];
            point BeforelastPoint = Points[Points.Length - 2];

            //BeforelastPoint.line.positionCount = 0;

            // Libera los recursos del LineRenderer
            if (lastPoint.line != null)
            {
                Destroy(lastPoint.line);
            }
            switch (BeforelastPoint.type)
            {
                case 0:
                    Vector3 start = new Vector3();
                    Vector3 end = new Vector3();
                    BeforelastPoint.line.SetPosition(0, start);
                    BeforelastPoint.line.SetPosition(1, end);
                    break;
                case 1:
                    for (int i = 0; i < segments; i++)
                    {
                        Vector3 cero = new Vector3();
                        BeforelastPoint.line.SetPosition(i, cero);
                    }
                    break;
                case 2:
                    for (int i = 0; i < segments; i++)
                    {
                        Vector3 cero = new Vector3();
                        BeforelastPoint.line.SetPosition(i, cero);
                    }
                    break;
                case 3:
                    break;
            }
           
            // Destruye el GameObject asociado
            if (lastPoint.PointObject != null)
            {
                Destroy(lastPoint.PointObject);
            }
            
            // Redimensiona el arreglo para excluir el �ltimo elemento
            System.Array.Resize(ref Points, Points.Length - 1);

            cont_points--;
        }
        else
        {
            UnityEngine.Debug.Log("There are no points placed!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //Pruebas
        // Aplicar la nueva rotaci�n al objeto
        newRotation = Origin_BaseRef.transform.localRotation;
        // Rotaci�n de 180 grados alrededor del eje Z
        //Quaternion inversionZ = Quaternion.Euler(0, 0, 180);

        // Nueva rotaci�n con el eje Z invertido
        //newRotation = newRotation * inversionZ;
        // Convertir el quaternion a �ngulos de Euler en grados
        eulerDegrees = newRotation.eulerAngles;
        // Convertir los �ngulos de Euler de grados a radianes
        eulerRadians = eulerDegrees * Mathf.Deg2Rad;

        real_rot_Degree.x = (float)(ur_data_processing.UR_Stream_Data.C_Orientation[0] * (180.0f / Math.PI));
        real_rot_Degree.y = (float)(ur_data_processing.UR_Stream_Data.C_Orientation[1] * (180.0f / Math.PI));
        real_rot_Degree.z = (float)(ur_data_processing.UR_Stream_Data.C_Orientation[2] * (180.0f / Math.PI));

        real_rot_Radian.x = (float)(ur_data_processing.UR_Stream_Data.C_Orientation[0]);
        real_rot_Radian.y = (float)(ur_data_processing.UR_Stream_Data.C_Orientation[1]);
        real_rot_Radian.z = (float)(ur_data_processing.UR_Stream_Data.C_Orientation[2] );
        //FIN PRuebas
        */


        esferaPeque�a = Points[cont_points - 1].PointObject;
        // Obt�n los radios de las esferas
        radioGrande = esferaGrande.transform.localScale.x / 2.0f;
        radioPeque�a = esferaPeque�a.transform.localScale.x / 2.0f;

        // Calcula la distancia entre los centros de las esferas
        distancia = Vector3.Distance(esferaGrande.transform.position, esferaPeque�a.transform.position);

        // Comprueba si la esfera peque�a est� dentro de la esfera grande
        if (distancia + radioPeque�a > radioGrande)
        {
            isInside = false;
        }
        else
        {
            isInside = true;
        }

        if (isInside == false)
        {
            aviso_type = 2;
        }

        switch (aviso_type)
        {
            case 1:
                aviso.text = "Just 1 point movement supported!";
                break;
            case 2:
                aviso.text = "Out of range! Make sure is inside the large sphere";
                break;
            case 3:
                aviso.text = "No points? Out of range?";
                break;
        }

        cont_timer++;
        //tcp.position = tcpPosition;
        if (PanelProgramation.activeSelf)
        {
            Origin.SetActive(true);
        }
        else
        {
            Origin.SetActive(false);
        }

        if(cont_points > 0)
        {
            UpdateConnections();
        }
        /*
        if (bandera_sendcommand) {

            RobotController controller = new RobotController(UIPanel_Control.global_ip_address, ur_data_processing.UR_Control_Data.port_number);

            if (offsetpos.x < 0.001 && offsetpos.y < 0.001 && offsetpos.z < 0.001)
            {
                cont++;
                switch (Points[cont].type)
                {
                    case 0: //MoveL
                        controller.SendCommandAsync("movel(p[" + coordinates[0] + "," + coordinates[1] + "," + coordinates[2] + "," + coordinates[3] + "," + coordinates[4] + "," + coordinates[5] + "], a = 0.5, v=0.01, t=1, r=0)" + "\n");
                        break;
                    case 1: //MoveJ
                        controller.SendCommandAsync("movej(p[" + coordinates[0] + "," + coordinates[1] + "," + coordinates[2] + "," + coordinates[3] + "," + coordinates[4] + "," + coordinates[5] + "], a = 0.5, v=0.01, t=1, r=0)" + "\n");
                        break;
                    case 2: //MoveP
                        controller.SendCommandAsync("movep(p[" + coordinates[0] + "," + coordinates[1] + "," + coordinates[2] + "," + coordinates[3] + "," + coordinates[4] + "," + coordinates[5] + "], a = 0.5, v=0.01, r=0)" + "\n");
                        break;
                    case 3: //Gripper
                        gripper_state = !gripper_state;
                        controller.SetDigitalOutputsAsync(gripper_state);
                        break;
                }
            }

            if (cont == Points.Length - 1)
            {
                    UnityEngine.Debug.Log("Finished!!");
                    bandera_sendcommand = false;
                    cont = 1;
            }

            targetpos.x = Points[cont].PointObject.transform.localPosition.x;
            targetpos.y = Points[cont].PointObject.transform.localPosition.y;
            targetpos.z = -Points[cont].PointObject.transform.localPosition.z;

            // Conversion of rotation axes
            rot.x = Origin_BaseRef.transform.rotation.x - 90.0f;
            rot.y = -Origin_BaseRef.transform.rotation.y;
            rot.z = Origin_BaseRef.transform.rotation.z;


            coordinates[0] = targetpos.x.ToString("F4", CultureInfo.InvariantCulture);
            coordinates[1] = targetpos.y.ToString("F4", CultureInfo.InvariantCulture);
            coordinates[2] = targetpos.z.ToString("F4", CultureInfo.InvariantCulture);
            coordinates[3] = rot.x.ToString("F4", CultureInfo.InvariantCulture);
            coordinates[4] = rot.y.ToString("F4", CultureInfo.InvariantCulture);
            coordinates[5] = rot.z.ToString("F4", CultureInfo.InvariantCulture);


            

            // Comprobacion de cuando se llega a un punto
            offsetpos.x = Math.Abs(targetpos.x - (float)ur_data_processing.UR_Stream_Data.C_Position[0]);
            offsetpos.y = Math.Abs(targetpos.y - (float)ur_data_processing.UR_Stream_Data.C_Position[1]);
            offsetpos.z = Math.Abs(targetpos.z - (float)ur_data_processing.UR_Stream_Data.C_Position[2]);
            /*
            if (offsetpos.x < 0.001 && offsetpos.y < 0.001 && offsetpos.z < 0.001)
            {
                completed = true;
                // Block the code until the robot makes the movement
            }
            
        }
        */
    }

    public void UpdateConnections()
    {
        for (int j = 1; j < Points.Length; j++)
        {
            switch (Points[j].type)
            {
                case 0: //MoveL
                    UpdateLineForMoveL(Points[j - 1].line, Points[j - 1].PointObject.transform.position, Points[j].PointObject.transform.position);
                    break;
                case 1: //MoveJ
                    UpdateLineForMoveJ(Points[j - 1].line, Points[j - 1].PointObject.transform.position, Points[j].PointObject.transform.position);
                    break;
                case 2: //MoveP
                    UpdateLineForMoveP(Points[j - 1].line, Points[j - 1].PointObject.transform.position, Points[j].PointObject.transform.position);
                    break;
                case 3: //Gripper
                   //Nothing
                    break;
            }
        }

    }

    public void UpdateLineForMoveL(LineRenderer line, Vector3 start, Vector3 end)
    {
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    public void UpdateLineForMoveJ(LineRenderer line, Vector3 start, Vector3 end)
    {
        line.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);
            Vector3 position = InterpolateMoveJ(start, end, t);
            line.SetPosition(i, position);
        }
    }
    Vector3 InterpolateMoveJ(Vector3 start, Vector3 end, float t)
    {
        // Interpolaci�n esf�rica (Slerp)
        return Vector3.Slerp(start, end, t);
    }

    public void UpdateLineForMoveP(LineRenderer line, Vector3 start, Vector3 end)
    {
        line.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);
            Vector3 position = InterpolateMoveP(start, end, t);
            line.SetPosition(i, position);
        }
    }
    Vector3 InterpolateMoveP(Vector3 start, Vector3 end, float t)
    {
        // Interpolaci�n c�bica Hermite
        float t2 = t * t;
        float t3 = t2 * t;
        float h1 = 2 * t3 - 3 * t2 + 1;          // Hermite cubic interpolation basis function 1
        float h2 = -2 * t3 + 3 * t2;             // Hermite cubic interpolation basis function 2
        float h3 = t3 - 2 * t2 + t;              // Hermite cubic interpolation basis function 3
        float h4 = t3 - t2;                      // Hermite cubic interpolation basis function 4

        Vector3 interpolatedPoint = h1 * start + h2 * end;   // Interpolated point position
        Vector3 interpolatedDerivative = h3 * (end - start); // Interpolated derivative (velocity)

        // Return the interpolated position
        return interpolatedPoint;
    }

    public void sendcommand()
    {
        // YOU NEED TO MAKE SURE THE BASE AXES REFERENCES OF THE ROBOT ARE THE SAME AS UNITY, FOR YOUR SECURITY
        RobotController controller = new RobotController(UIPanel_Control.global_ip_address, ur_data_processing.UR_Control_Data.port_number);

        bandera_sendcommand = true;
        /*
        switch (Points[1].type)
        {
            case 0: //MoveL
                controller.SendCommandAsync("movel(p[" + coordinates[0] + "," + coordinates[1] + "," + coordinates[2] + "," + coordinates[3] + "," + coordinates[4] + "," + coordinates[5] + "], a = 0.5, v=0.01, t=1, r=0)" + "\n");
                break;
            case 1: //MoveJ
                controller.SendCommandAsync("movej(p[" + coordinates[0] + "," + coordinates[1] + "," + coordinates[2] + "," + coordinates[3] + "," + coordinates[4] + "," + coordinates[5] + "], a = 0.5, v=0.01, t=1, r=0)" + "\n");
                break;
            case 2: //MoveP
                controller.SendCommandAsync("movep(p[" + coordinates[0] + "," + coordinates[1] + "," + coordinates[2] + "," + coordinates[3] + "," + coordinates[4] + "," + coordinates[5] + "], a = 0.5, v=0.01, r=0)" + "\n");
                break;
            case 3: //Gripper
                gripper_state = !gripper_state;
                controller.SetDigitalOutputsAsync(gripper_state);
                break;
        }
        */
        
        if (bandera_sendcommand && cont_points > 0 && isInside)
        {
            for (int j = 1; j < Points.Length; j++)
            {
                targetpos.x = Points[j].PointObject.transform.localPosition.x;
                targetpos.y = Points[j].PointObject.transform.localPosition.y;
                targetpos.z = -Points[j].PointObject.transform.localPosition.z;

                //Pruebas
                /*
                // Rotaci�n original
                Quaternion originalRotation = Origin_BaseRef.transform.localRotation; // Ejemplo de rotaci�n en Euler angles

                // Rotaci�n de 180 grados alrededor del eje Z
                Quaternion inversionZ = Quaternion.Euler(0, 0, 180);

                // Nueva rotaci�n con el eje Z invertido
                newRotation = originalRotation * inversionZ;

                // Convertir el quaternion a �ngulos de Euler en grados
                eulerDegrees = newRotation.eulerAngles;

                // Convertir los �ngulos de Euler de grados a radianes
                eulerRadians = eulerDegrees * Mathf.Deg2Rad;
                //FIN pruebas
                */
                // Aplicar la nueva rotaci�n al objeto
                rot.x = (float)ur_data_processing.UR_Stream_Data.C_Orientation[0];
                rot.y = (float)ur_data_processing.UR_Stream_Data.C_Orientation[1];
                rot.z = (float)ur_data_processing.UR_Stream_Data.C_Orientation[2];

                coordinates[0] = targetpos.x.ToString("F4", CultureInfo.InvariantCulture);
                coordinates[1] = targetpos.y.ToString("F4", CultureInfo.InvariantCulture);
                coordinates[2] = targetpos.z.ToString("F4", CultureInfo.InvariantCulture);
                coordinates[3] = rot.x.ToString("F4", CultureInfo.InvariantCulture);
                coordinates[4] = rot.y.ToString("F4", CultureInfo.InvariantCulture);
                coordinates[5] = rot.z.ToString("F4", CultureInfo.InvariantCulture);

                switch (Points[j].type)
                {
                    case 0: //MoveL
                        controller.SendCommandAsync("movel(p[" + coordinates[0] + "," + coordinates[1] + "," + coordinates[2] + "," + coordinates[3] + "," + coordinates[4] + "," + coordinates[5] + "], a = 0.5, v=0.01, t=1, r=0)" + "\n");
                        break;
                    case 1: //MoveJ
                        controller.SendCommandAsync("movej(p[" + coordinates[0] + "," + coordinates[1] + "," + coordinates[2] + "," + coordinates[3] + "," + coordinates[4] + "," + coordinates[5] + "], a = 0.5, v=0.01, t=1, r=0)" + "\n");
                        break;
                    case 2: //MoveP
                        controller.SendCommandAsync("movep(p[" + coordinates[0] + "," + coordinates[1] + "," + coordinates[2] + "," + coordinates[3] + "," + coordinates[4] + "," + coordinates[5] + "], a = 0.5, v=0.01, r=0)" + "\n");
                        break;
                    case 3: //Gripper
                        gripper_state = !gripper_state;
                        controller.SetDigitalOutputsAsync(gripper_state);
                        /*                      

                                                gripper_state = !gripper_state;
                                                string comando_str;
                                                // Enviar primer comando
                                                comando_str = "set_tool_digital_out(0, " + (gripper_state ? "True" : "False") + ")\n ";
                                                byte[] comando;
                                                comando = utf8.GetBytes(comando_str);

                                                network_stream.Write(comando, 0, comando.Length);
                                                //Thread.Sleep(100);
                                                // ur_data_processing.UR_Control_Data.aux_command_str = "set_tool_digital_out(1, " + (gripper_state ? "True" : "False") + ")\n";
                        */
                        break;
                }
                UnityEngine.Debug.Log("Wait...");
                /*
                cont_timer = 0;
                while (cont_timer < 50)
                {

                }
                
                while (completed == false)
                {
                    // Block the code until the robot makes the movement
                }
                */
                UnityEngine.Debug.Log("Completed, going to next point");

                offsetpos.x = Math.Abs(targetpos.x - (float)ur_data_processing.UR_Stream_Data.C_Position[0]);
                offsetpos.y = Math.Abs(targetpos.y - (float)ur_data_processing.UR_Stream_Data.C_Position[1]);
                offsetpos.z = Math.Abs(targetpos.z - (float)ur_data_processing.UR_Stream_Data.C_Position[2]);

            }
        }
        else
        {
                    UnityEngine.Debug.Log("There are no points!");
                    aviso_type = 3;
        }

                UnityEngine.Debug.Log("Finished!");
                bandera_sendcommand = false;
        
    }

    public class RobotController
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public RobotController(string ipAddress, int port)
        {
            _client = new TcpClient();
            _client.Connect(ipAddress, port);
            _stream = _client.GetStream();
        }

        public async Task SendCommandAsync(string command)
        {
            byte[] commandBytes = Encoding.ASCII.GetBytes(command);
            await _stream.WriteAsync(commandBytes, 0, commandBytes.Length);
            UnityEngine.Debug.Log("Comando enviado: " + command);
            await Task.Delay(500); // Is to make sure that the target is changed before waiting for confirmation
            //  If this is not done, the target won�t change at time and WaitForConfirmationAsync() will be "ignored"

            //await WaitForConfirmationAsync();
        }

        private async Task WaitForConfirmationAsync()
        {

            while (completed == false)
            {
                // Block the code until the robot makes the movement
            }
            
        }

        public async Task SetDigitalOutputsAsync(bool gripperState)
        {
            string command = $"set_tool_digital_out({(gripperState ? "1" : "0")}, False)\n";
  
            await SendCommandAsync(command);
            await Task.Delay(500); // Pausa de medio segundo
            command = $"set_tool_digital_out({(gripperState ? "0" : "1")}, True)\n";
            await SendCommandAsync(command);

        }

    }


}


using UnityEngine;

using System;
using System.Collections;

using MMTD_Client.Domain;
using MMTD_Client.Gui;

public class CharacterControl : MonoBehaviour
{

    public Transform cam;
    public float walkSpeed = 0.1f;
    public float mouseRotation = 3.5f;

    public GameObject controlObject { get; set; }
    public ControllerObjectScript controlScript { get; set; }
    public bool cursorLocked { get; set; }

    private DomainController domainController;

    // Use this for initialization
    void Start()
    {
        domainController = DomainController.getInstance();
        controlObject = GameObject.Find("ControllerObject");
        controlScript = (ControllerObjectScript)controlObject.GetComponent("ControllerObjectScript");
        controlScript.SetPlayerObject(this.gameObject);
        cam.parent = transform;
        cursorLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        //transform.Translate(x * walkSpeed, 0, z * walkSpeed);

        float rot = 0;

        if (Input.GetMouseButton(1))
        {
            Screen.lockCursor = true;
            rot = Input.GetAxisRaw("Mouse X");
        }
        else
        {
            Screen.lockCursor = false;
        }

        transform.Rotate(Vector3.up, rot * mouseRotation, 0);
        domainController.OutgoingHomeQueue.Enqueue(domainController.myAccount.accountId + "|" + x + "|" + z + "|" + rot);
        cam.position = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(0, 2, -6) + transform.position;
        cam.rotation = transform.rotation;


        string message = domainController.playerInfo[domainController.myAccount.accountId];
        if (message != null)
        {
            message = message.Replace(',', '.');
            //GuiController.getInstance().UnityLog("got string from playerinfo: " + message);
            string[] array = message.Split('|');
            float PosX = (float)Math.Round(Convert.ToDouble(array[0]), 2);
            float PosY = (float)Math.Round(Convert.ToDouble(array[1]), 2);
            float PosZ = (float)Math.Round(Convert.ToDouble(array[2]), 2);
            float RotX = (float)Math.Round(Convert.ToDouble(array[3]), 2);
            float RotY = (float)Math.Round(Convert.ToDouble(array[4]), 2);
            float RotZ = (float)Math.Round(Convert.ToDouble(array[5]), 2);
            float RotW = (float)Math.Round(Convert.ToDouble(array[6]), 2);
            transform.position = new Vector3(PosX, PosY, PosZ);
            //GuiController.getInstance().UnityLog("moving player to: " + PosX + "," + PosY + "," + PosZ);
        }

    }
}

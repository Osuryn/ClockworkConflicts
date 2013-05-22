using UnityEngine;

using System;
using System.Collections;

using MMTD_Client.Domain;

public class OtherPlayerScript : MonoBehaviour {

    public int id { get; set; }
    private DomainController domainController;

	// Use this for initialization
	void Start () {
        domainController = DomainController.getInstance();
	}
	
	// Update is called once per frame
	void Update () {
        string message = domainController.playerInfo[id];
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

using UnityEngine;
using System.Collections;

using MMTD_Client.Domain;

public class CharacterControl : MonoBehaviour {

    public Transform cam;
    public float walkSpeed = 0.1f;
    public float mouseRotation = 3.5f;

    public GameObject controlObject { get; set; }
    public ControllerObjectScript controlScript { get; set; }
    public bool cursorLocked { get; set; }

    private DomainController domainController;

	// Use this for initialization
	void Start () {
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
        transform.Translate(x * walkSpeed, 0, z * walkSpeed);

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
        //domainController.OutgoingHomeQueue.Enqueue(domainController.myAccount.accountId + "|" + x + "|" + z + "|" + rot);
        cam.position = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(0, 2, -6) + transform.position;
        cam.rotation = transform.rotation;
    }
}

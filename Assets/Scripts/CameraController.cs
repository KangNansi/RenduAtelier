using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    Vector2 mouse;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.up*Input.GetAxis("Vertical") * 1f * Time.deltaTime;
        transform.position += transform.right * Input.GetAxis("Horizontal") * -1f * Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            transform.Rotate(new Vector3(Input.mousePosition.y - mouse.y,  0, Input.mousePosition.x - mouse.x));
        }
        mouse = Input.mousePosition;
        if (Input.GetKey(KeyCode.G))
            GetComponent<Rigidbody>().isKinematic = !GetComponent<Rigidbody>().isKinematic;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatInSpace : MonoBehaviour {

    public float x_rot;
    public float y_rot;
    public float z_rot;

    // Use this for initialization
    void Start () {
        x_rot = Random.Range(5.0f, 15.0f);
        y_rot = Random.Range(5.0f, 15.0f);
        z_rot = Random.Range(5.0f, 15.0f);

        x_rot *= (Random.Range(-1, 1) < 0) ? -1 : 1;
        y_rot *= (Random.Range(-1, 1) < 0) ? -1 : 1;
        z_rot *= (Random.Range(-1, 1) < 0) ? -1 : 1;
    }


	
	// Update is called once per frame
	void Update () {
        Vector3 rotation = new Vector3(x_rot, y_rot, z_rot);
        transform.Rotate(rotation * Time.deltaTime, Space.World);
    }
}

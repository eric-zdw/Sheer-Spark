using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateParticleRotation : MonoBehaviour {

	GameObject player;
	ParticleSystem.MainModule ps;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		ps = GetComponent<ParticleSystem>().main;
	}
	
	// Update is called once per frame
	void Update () {
		ps.startRotation3D = true;
		float rot = Mathf.Deg2Rad * player.transform.rotation.eulerAngles.z;
		ps.startRotationZ = rot;
	}
}

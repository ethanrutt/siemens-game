using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseBehavior : MonoBehaviour
{
    private System.Random rand = new System.Random();

    public float[] speeds = {55f, 66f, 77f, 88f, 99f, 110f, 121f, 132f, 143f, 154f, 165f, 176f, 187f, 198f};

    public float neurofluxLevel = 0;

    public Vector3 direction = Vector3.right;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction.normalized * speeds[rand.Next(speeds.Length)] * (1 + neurofluxLevel * rand.Next(2)) * Time.deltaTime;
    }
}

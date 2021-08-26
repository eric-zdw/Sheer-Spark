using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    LineRenderer line;
    GameObject boss;

    float rayRadius = 1f;

    int collisionMask = 1 << 8 | 1 << 17;

    Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        boss = Physics.OverlapSphere(transform.position, 0.1f)[0].gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, boss.transform.position);
        Vector3 dir = WaveSystem.player.transform.position - boss.transform.position;
        RaycastHit hit;

        if (Physics.SphereCast(boss.transform.position, rayRadius, dir, out hit, 2000f, collisionMask))
        {
            line.SetPosition(1, boss.transform.position + Vector3.Normalize(dir) * (hit.distance + rayRadius));
        }
        else
        {
            line.SetPosition(1, boss.transform.position + (Vector3.Normalize(dir) * 500f));
        }

        //transform.GetChild(0).position = transform.InverseTransformPoint(line.GetPosition(1));
        ParticleSystem.ShapeModule shape1 = transform.GetChild(0).GetComponent<ParticleSystem>().shape;
        shape1.position = transform.InverseTransformPoint(line.GetPosition(1));
        ParticleSystem.ShapeModule shape2 = transform.GetChild(1).GetComponent<ParticleSystem>().shape;
        shape2.position = transform.InverseTransformPoint(line.GetPosition(1));

        line.widthMultiplier -= 2f * Time.deltaTime;
        if (line.widthMultiplier <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        Vector3 dir = WaveSystem.player.transform.position - boss.transform.position;
        RaycastHit hit;

        if (Physics.SphereCast(boss.transform.position, rayRadius, dir, out hit, 2000f, collisionMask))
        {
            targetPos = boss.transform.position + Vector3.Normalize(dir) * (hit.distance + rayRadius);
        }
        else
        {
            targetPos = boss.transform.position + (Vector3.Normalize(dir) * 500f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public float maxDistance = 30;

    public GameManager gm;

    public float laserLength = 1.0f;

    LineRenderer lr;

    Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;

        lr = GetComponent<LineRenderer>();
        
        if( lr != null)
        {
            lr.positionCount = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // difference in all coordinate
        float diffX = Mathf.Abs(initPos.x - transform.position.x);
        float diffY = Mathf.Abs(initPos.y - transform.position.y);
        float diffZ = Mathf.Abs(initPos.z - transform.position.z);

        // destroy if it's too far away
        if(diffX >= maxDistance || diffY >= maxDistance || diffZ >= maxDistance)
        {
            Destroy(gameObject);
        }

        Vector3 endPos = transform.position + transform.forward * laserLength;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, endPos);

    }


    void OnTriggerEnter(Collider other)
    {
        // check if we hit an enemy
        if(other.CompareTag("Player Body"))
        {
            gm.PlayerHP--;
            gm.SetHPText();
            gm.hurtsound.Play();
            if(gm.PlayerHP <= 0){
                gm.GameOver();
            }
        }else if(other.CompareTag("Shield"))
        {
            other.gameObject.GetComponent<AudioSource>().Play();
            Destroy(gameObject);
        }

    }
}

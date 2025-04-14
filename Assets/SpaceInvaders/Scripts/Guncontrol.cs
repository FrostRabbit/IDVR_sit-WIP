using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Guncontrol : MonoBehaviour
{
    public float cooltime = 0.5f;
    public ParticleSystem flash;

    public float bulletSpeed = 10;
    public GameObject crosshair;
    public GameObject Raser;
    public float laserDistance = 100f;

    public Animator animator;

    public int bulletcapacity = 30;
    public int currentbullet = 0;

    public float reloadtime = 1.0f;
    
    [System.Serializable]
    public class HapticFeedback
    {
        public float amplitude = 0.5f;
        public float duration = 0.1f;
    }
    public HapticFeedback hapticFeedback;

    public TextMeshProUGUI bulletText;

    public AudioSource reloadSound;
    private LineRenderer lineRenderer;


    void Start()
    {
        currentbullet = bulletcapacity;
        Setbulletcount();
        lineRenderer = Raser.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        RaycastHit hit;
        ParticleSystem crosshairParticle = crosshair.GetComponent<ParticleSystem>();

        if(Physics.Raycast(Raser.transform.position, transform.forward, out hit, laserDistance))
        {
            crosshair.transform.position = hit.point;
            lineRenderer.SetPosition(0, Raser.transform.position);
            lineRenderer.SetPosition(1, hit.point);

            if(!crosshairParticle.isPlaying)
            {
                crosshairParticle.Play();
            }
        }
        else
        {
            lineRenderer.SetPosition(0, Raser.transform.position);
            lineRenderer.SetPosition(1, Raser.transform.position + transform.forward * laserDistance);

            crosshairParticle.Stop();
        }
    }

    public void Setbulletcount()
    {
        // if current bullet is 0, change the color to red
        if(currentbullet == 0)
        {
            bulletText.text = "<color=#FF0000>" + currentbullet.ToString() + "</color>" + "/" + bulletcapacity.ToString();
        }else{
            bulletText.text = currentbullet.ToString() + "/" + bulletcapacity.ToString();
        }
    }
}

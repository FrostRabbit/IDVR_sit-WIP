using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Oculus.Haptics;

public class PlayerController : MonoBehaviour
{
    public HapticClip hapticClip;

    public AudioSource EmptyGunSound;

    // Gun shoot out position
    public List<GameObject> ShootOutput = new List<GameObject>();

    // bullet prefab
    public GameObject bulletPrefab;
    public List<GrabDetector> gunGrabDetector = new List<GrabDetector>();


    [Range(0.01f, 1f)]
    public float speedH = 1.0f;
    [Range(0.01f, 1f)]
    public float speedV = 1.0f;

    private int currentGun = 2;
    private AudioSource audio;

    private ParticleSystem particle;

    private float FireTime = 0.0f;
    private float FireCoolTime = 0.0f;

    private float reloadTime = 0.0f;
    private float bulletSpeed = 10;

    private HapticClipPlayer hapticClipPlayer;

    GameManager gm;


    // Start is called before the first frame update
    void Start()
    {
        hapticClipPlayer = new HapticClipPlayer(hapticClip);
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    void Update()
    {   
        FireTime += Time.deltaTime;
        reloadTime += Time.deltaTime;
        gunSwitch();
        GunActionManager();
    }


    void OnFire()
    {
        Guncontrol gun = ShootOutput[currentGun].GetComponent<Guncontrol>();

        if(FireTime < FireCoolTime)
        {
            return;
        }else if(gun.currentbullet <= 0 || reloadTime < gun.reloadtime)
        {
            EmptyGunSound.Play();
            return;
        }

        // decrease the bullet count
        gun.currentbullet--;

        // Set the bullet text
        gun.Setbulletcount();

        // spawn a new bullet
        GameObject newBullet = Instantiate(bulletPrefab);

        // pass the game manager
        newBullet.GetComponent<BulletController>().gm = gm;

        // position will be that of the gun
        newBullet.transform.position = ShootOutput[currentGun].transform.position;

        // get rigid body
        Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();

        // let the bullet face to the forward when shoot
        newBullet.transform.LookAt(ShootOutput[currentGun].transform.right * 30f);

        // let the bullet's rotation be the same as the gun
        newBullet.transform.rotation = Quaternion.LookRotation(ShootOutput[currentGun].transform.forward);

        // give the bullet velocity
        bulletRb.velocity = ShootOutput[currentGun].transform.forward * bulletSpeed;

        // play the sound
        audio.Play();

        // play the particle
        particle.Play();

        // play the animation
        gun.animator.SetTrigger("Recoil");
        FireTime = 0.0f;
    }

    void GunActionManager()
    {
        // shoot gun
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && gunGrabDetector[currentGun].isGrabbed )
        {
            OnFire();
            Guncontrol gun = ShootOutput[currentGun].GetComponent<Guncontrol>();
            
            InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            HapticCapabilities capabilities;
            if(device.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
            {
                device.SendHapticImpulse(0, gun.hapticFeedback.amplitude, gun.hapticFeedback.duration);
            }

            /*
            if (hapticClip){
                hapticClipPlayer.Play(Oculus.Haptics.Controller.Right);
            }*/
        }

        // reload gun
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Guncontrol gun = ShootOutput[currentGun].GetComponent<Guncontrol>();
            gun.currentbullet = gun.bulletcapacity;
            gun.Setbulletcount();
            gun.reloadSound.Play();
            reloadTime = 0.0f;
        }

        if (OVRInput.GetDown(OVRInput.Button.Three)){
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }

    void gunSwitch()
    {
        for (int i = 0; i < gunGrabDetector.Count; i++)
        {
            if (gunGrabDetector[i].isGrabbed)
            {
                currentGun = i;
                break;
            }
        }
        audio = ShootOutput[currentGun].GetComponent<AudioSource>();
        FireCoolTime = ShootOutput[currentGun].GetComponent<Guncontrol>().cooltime;
        particle = ShootOutput[currentGun].GetComponent<Guncontrol>().flash;
        bulletSpeed = ShootOutput[currentGun].GetComponent<Guncontrol>().bulletSpeed;
    }
    
}

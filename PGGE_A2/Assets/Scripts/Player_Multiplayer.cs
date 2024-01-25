using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGGE.Patterns;
using Photon.Pun;

public class Player_Multiplayer : MonoBehaviourPunCallbacks, IPunObservable
{
    private PhotonView mPhotonView;

    [HideInInspector]
    public FSM mFsm = new FSM();
    public Animator mAnimator;
    public PlayerMovement mPlayerMovement;

    // This is the maximum number of bullets that the player 
    // needs to fire before reloading.
    public int mMaxAmunitionBeforeReload = 40;

    // This is the total number of bullets that the 
    // player has.
    [HideInInspector]
    public int mAmunitionCount = 100;

    // This is the count of bullets in the magazine.
    [HideInInspector]
    public int mBulletsInMagazine = 40;

    [HideInInspector]
    public bool[] mAttackButtons = new bool[3];

    public Transform mGunTransform;
    public LayerMask mPlayerMask;
    public Canvas mCanvas;
    public RectTransform mCrossHair;


    public GameObject mBulletPrefab;
    public float mBulletSpeed = 10.0f;

    public int[] RoundsPerSecond = new int[3];
    bool[] mFiring = new bool[3];


    // Start is called before the first frame update
    void Start()
    {
        mPhotonView = GetComponent<PhotonView>();

        mFsm.Add(new PlayerState_Multiplayer_MOVEMENT(this));
        mFsm.Add(new PlayerState_Multiplayer_ATTACK(this));
        mFsm.Add(new PlayerState_Multiplayer_RELOAD(this));
        mFsm.SetCurrentState((int)PlayerStateType.MOVEMENT);
    }

    #region IPunObservable implementation

    // Used for network synchronisation, specifically to propogate bullets across the network.
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(mFiring);
        }
        else
        {
            // Network player, receive data
            this.mFiring = (bool[])stream.ReceiveNext();
        }

    }

    #endregion

    void Update()
    {
        if (!mPhotonView.IsMine) return;

        mFsm.Update();
        Aim();

        // Refactoring 2 (Amended redundant comments)

        // Logic to check which button is pressed and updates attack button states.
        if (Input.GetButton("Fire1"))
        {
            mAttackButtons[0] = true;
            mAttackButtons[1] = false;
            mAttackButtons[2] = false;
        }
        else
        {
            mAttackButtons[0] = false;
        }

        if (Input.GetButton("Fire2"))
        {
            mAttackButtons[0] = false;
            mAttackButtons[1] = true;
            mAttackButtons[2] = false;
        }
        else
        {
            mAttackButtons[1] = false;
        }

        if (Input.GetButton("Fire3"))
        {
            mAttackButtons[0] = false;
            mAttackButtons[1] = false;
            mAttackButtons[2] = true;
        }
        else
        {
            mAttackButtons[2] = false;
        }
    }

    public void Aim()
    {
        // Refactoring 2 (Amended redundant comments)

        Vector3 dir = -mGunTransform.right.normalized;
        // Find gunpoint as mentioned in the worksheet.
        Vector3 gunpoint = mGunTransform.transform.position +
                           dir * 1.2f -
                           mGunTransform.forward * 0.1f;
        // Fine the layer mask for objects that you want to intersect with.
        LayerMask objectsMask = ~mPlayerMask;

        // Do the Raycast
        RaycastHit hit;
        bool flag = Physics.Raycast(gunpoint, dir,
                        out hit, 50.0f, objectsMask);
        if (flag)
        {
            // Draw a line as debug to show the aim of fire in scene view.
            Debug.DrawLine(gunpoint, gunpoint +
                (dir * hit.distance), Color.red, 0.0f);

            // Find the transformed intersected point to screenspace
            // and then transform the crosshair position to this
            // new position.
            // first you need the RectTransform component of your mCanvas
            RectTransform CanvasRect = mCanvas.GetComponent<RectTransform>();

            // then you calculate the position of the UI element.
            // Remember that 0,0 for the mCanvas is at the centre of the screen. 
            // But WorldToViewPortPoint treats the lower left corner as 0,0. 
            // Because of this, you need to subtract the height / width 
            // of the mCanvas * 0.5 to get the correct position.

            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(hit.point);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            //now you can set the position of the UI element
            mCrossHair.anchoredPosition = WorldObject_ScreenPosition;


            // Enable or set active the crosshair gameobject.
            mCrossHair.gameObject.SetActive(true);
        }
        else
        {
            // Hide or set inactive the crosshair gameobject.
            mCrossHair.gameObject.SetActive(false);
        }
    }

    public void Move()
    {
        if (!mPhotonView.IsMine) return;

        mPlayerMovement.HandleInputs();
        mPlayerMovement.Move();
    }

    public void NoAmmo()
    {

    }

    public void Reload()
    {

    }

    public void Fire(int id)
    {
        if (mFiring[id] == false)
        {
            photonView.RPC("Coroutine_Firing", RpcTarget.All, id); 
        }
    }


    // Instantiates the bullet in a specific direction with ids for specific attack buttons (0,1,2)
    [PunRPC]
    public void FireBullet(int id)
    {
        if (mBulletPrefab == null) return;

        Vector3 dir = -mGunTransform.right.normalized;
        Vector3 firePoint = mGunTransform.transform.position + dir *
            1.2f - mGunTransform.forward * 0.1f;
        GameObject bullet = Instantiate(mBulletPrefab, firePoint,
            Quaternion.LookRotation(dir) * Quaternion.AngleAxis(90.0f, Vector3.right)); // Instantiates bullet at calculated pos + orientation.

        bullet.GetComponent<Rigidbody>().AddForce(dir * mBulletSpeed, ForceMode.Impulse);
    }

    // Handles firing process including shooting of bullets. Also with ids for specific attack buttons (0,1,2)
    [PunRPC] 
    IEnumerator Coroutine_Firing(int id)
    {
        mFiring[id] = true;
        FireBullet(id);
        yield return new WaitForSeconds(1.0f / RoundsPerSecond[id]);
        mFiring[id] = false;
        mBulletsInMagazine -= 1;
    }
}

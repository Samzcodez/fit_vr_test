using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using UnityEngine.Animations.Rigging;

public class NetworkAvatarManager : MonoBehaviour
{
    public GameObject sportman;

    public Transform avatar_head;

    private Transform headRig;

    private PhotonView photonView;

    private bool uses_fullbody_tracking = false;

    
    private FullBodyTrackingManager fullbodytracking_controller;
    private PopupManager popup_manager;
    private MainMenu main_menu;

    // Start is called before the first frame update
    void Start()
    {
        this.fullbodytracking_controller = this.GetComponentInChildren<FullBodyTrackingManager>();
        this.popup_manager = this.GetComponentInChildren<PopupManager>();
        this.main_menu = this.GetComponentInChildren<MainMenu>();


        photonView = GetComponent<PhotonView>();

        XRRig rig = FindObjectOfType<XRRig>();
        headRig = rig.transform.Find("Camera Offset/Main Camera");

        if (this.isClientCharacter() == false)
        {
            print("Slave code");

            Object.Destroy(this.main_menu.gameObject);
            Object.Destroy(this.fullbodytracking_controller.gameObject);
            Object.Destroy(this.popup_manager.gameObject);

            this.sportman.GetComponent<RigBuilder>().enabled = false;
            this.sportman.GetComponent<VRRig>().enabled = false;
            this.sportman.GetComponent<VRFootIK>().enabled = false;
            this.sportman.GetComponent<VRAnimatorController>().enabled = false;
            this.sportman.GetComponent<Animator>().enabled = false;
        }
    }


    public void startFullBodyTracking()
    {
        if(this.isClientCharacter() == false)
        {
            return ;
        }

        print("using fbt");
        this.uses_fullbody_tracking = true;

        this.sportman.GetComponent<RigBuilder>().enabled = false;
        this.sportman.GetComponent<VRRig>().enabled = false;

        this.avatar_head.localScale  = new Vector3(0.0f, 0.0f, 0.0f);
        this.popup_manager.showPopup("Fullbody tracking is enabled");

    }


    public void stopFullBodyTracking()
    {
        if(this.isClientCharacter() == false)
        {
            return ;
        }

        print("stopping fbt");

        this.uses_fullbody_tracking = false;

        this.sportman.GetComponent<RigBuilder>().enabled = true;
        this.sportman.GetComponent<VRRig>().enabled = true;

        this.avatar_head.localScale  = new Vector3(1.0f, 1.0f, 1.0f);
        this.popup_manager.showPopup("Fullbody tracking is disabled");
    }

    public bool usesFullBodyTracking()
    {
        return this.uses_fullbody_tracking;
    }

    public GameObject getAvatarObject()
    {
        return this.sportman;
    }

    public Transform getMainCamera()
    {
        return this.headRig;
    }

    public bool isClientCharacter()
    {
        return this.photonView.IsMine;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.Globalization;
using ExciteOMeter;
using System.Linq;

public class BotAnimationStateController : MonoBehaviour
{
    Animator animator;
    public Text timeText;
    public GameObject botAvatar;
    public GameObject botUI;
    public GameObject botZumbaUI;
    public GameObject botWorkoutUI;
    public GameObject botExercisesUI;

    public int count = 0;
    public double a, b;
    public double average, max;

    public Button startButton;
    public Button repeatButton;
    public Button disableButton;

    public Button zumbaButton;
    public Button workoutButton;

    public Button stopZumbaButton;
    public Button repeatZumbaButton;
    public Button disableZumbaButton;

    public Button startWorkoutButton;
    public Button startZumbaButton;

    public Button chooseExerciseButton;

    public GameObject greetingAudioSource;
    public GameObject zumbaAudioSource;
    public GameObject workoutAudioSource;
    AudioSource greetingAudio;
    AudioSource zumbaAudio;
    AudioSource workoutAudio;

    bool isStarted;
    bool isZumbaHandled;
    bool isDancing;
    bool isWorkoutHandled;
    bool isWorkingOut;
    public static bool isWorkOutStop;

    public Button zumbaMusicButton;

    public Button disableWorkoutButton;
    public Button stopWorkoutButton;
    public Button repeatWorkoutButton;

    public Button bicepCurlsButton;
    public Button frontRaisesButton;
    public Button squatButton;
    public Button jumpingJacksButton;
    public int userid;


    [HideInInspector]
    public Performance p;
    [HideInInspector]
    public dbconn db;
    [HideInInspector]
    public userdata us;
    
    [HideInInspector]
    public static List<float> HR_Data;
 
    public Button endWorkoutButton;
    public Button endZumbaButton;

    AudioSource bicepCurlsAudio;
    AudioSource frontRaisesAudio;
    AudioSource squatAudio;
    AudioSource jumpingJacksAudio;

    AudioSource endZumbaAudio;
    AudioSource endWorkoutAudio;
    [HideInInspector]
    public DateTime dateStart, dateEnd;
    void Start()
    {
        db = new dbconn();
        us = new userdata();
        p = new Performance();
        HR_Data = new List<float>();


        animator = GetComponentInChildren<Animator>();
        startButton.onClick.AddListener(TaskOnStartClick);
        repeatButton.onClick.AddListener(TaskOnRepeatClick);
        disableButton.onClick.AddListener(TaskOnDisableClick);

        zumbaButton.onClick.AddListener(TaskOnZumbaClick);
        workoutButton.onClick.AddListener(TaskOnWorkoutClick);

        stopZumbaButton.onClick.AddListener(TaskOnZumbaStopClick);
        repeatZumbaButton.onClick.AddListener(TaskOnZumbaRepeatClick);
        disableZumbaButton.onClick.AddListener(TaskOnZumbaDisableClick);

        disableWorkoutButton.onClick.AddListener(TaskOnWorkoutDisableClick);

        greetingAudio = greetingAudioSource.GetComponent<AudioSource>();
        zumbaAudio = zumbaAudioSource.GetComponent<AudioSource>();
        workoutAudio = workoutAudioSource.GetComponent<AudioSource>();


        isStarted = false;
        isZumbaHandled = false;
        isDancing = false;
        isWorkoutHandled = false;
        isWorkingOut = false;
        zumbaMusicButton.onClick.AddListener(TaskOnZumbaMusicClick);

        botZumbaUI.SetActive(false);
        botWorkoutUI.SetActive(false);
        botExercisesUI.SetActive(false);
        zumbaButton.interactable = false;
        workoutButton.interactable = false;
        repeatButton.interactable = false;

        repeatWorkoutButton.onClick.AddListener(TaskOnWorkoutRepeatClick);
        chooseExerciseButton.onClick.AddListener(TaskOnExerciseClick);
        stopWorkoutButton.onClick.AddListener(TaskOnWorkoutStopClick);

        bicepCurlsButton.onClick.AddListener(TaskOnBicepCurlsClick);
        frontRaisesButton.onClick.AddListener(TaskOnFrontRaisesClick);
        squatButton.onClick.AddListener(TaskOnSquatClick);
        jumpingJacksButton.onClick.AddListener(TaskOnJumpingJacksClick);

        bicepCurlsAudio = bicepCurlsButton.GetComponent<AudioSource>();
        frontRaisesAudio = frontRaisesButton.GetComponent<AudioSource>();
        squatAudio = squatButton.GetComponent<AudioSource>();
        jumpingJacksAudio = jumpingJacksButton.GetComponent<AudioSource>();

        endWorkoutButton.onClick.AddListener(TaskOnEndWorkoutClick);
        endZumbaButton.onClick.AddListener(TaskOnEndZumbaClick);

        endZumbaAudio = endZumbaButton.GetComponent<AudioSource>();
        endWorkoutAudio = endWorkoutButton.GetComponent<AudioSource>();

        startZumbaButton.onClick.AddListener(TaskOnStartZumbaClick);
    }

    void Update()
    {
        if (!greetingAudio.isPlaying && isStarted)
        {
            animator.SetBool("isTalking", false);
            animator.SetBool("isIdle", true);
            zumbaButton.interactable = true;
            workoutButton.interactable = true;
        }

        if (!zumbaAudio.isPlaying && isZumbaHandled && !isDancing)
        {
            animator.SetBool("isTalking", false);
            animator.SetBool("isIdle", true);
        }

        if (!workoutAudio.isPlaying && isWorkoutHandled && !isWorkingOut)
        {
            animator.SetBool("isTalking", false);
            animator.SetBool("isIdle", true);
            chooseExerciseButton.interactable = true;
        }
    }

    void TaskOnStartZumbaClick()
    {
        if (!zumbaMusicButton.GetComponent<AudioSource>().isPlaying)
        {
            zumbaMusicButton.onClick.Invoke();
            animator.SetBool("isIdle", false);
            animator.SetBool("isDancing", true);
            isDancing = true;
        }

    }

    void TaskOnStartClick()
    {
        db.CreateDB();
        isStarted = true;
        startButton.interactable = false;
        repeatButton.interactable = true;
        greetingAudio.Play();
        animator.SetBool("isTalking", true);
    }

    void TaskOnRepeatClick()
    {
        greetingAudio.Play();
        animator.SetBool("isIdle", false);
        animator.SetBool("isTalking", true);
    }

    void TaskOnDisableClick()
    {
        botAvatar.SetActive(false);
        botUI.SetActive(false);
    }

    void TaskOnZumbaClick()
    {
        startZumbaButton.interactable = false;
        isWorkoutHandled = false; // (*)
        botUI.SetActive(false);
        isStarted = false;
        iTween.MoveTo(botAvatar, iTween.Hash("position", new Vector3(-10.303f, 1.8446f, -14.568f), "time", 4f, "easetype", iTween.EaseType.easeInOutSine));
        animator.SetBool("isIdle", false);
        animator.SetBool("isTalking", false);
        animator.SetBool("isWalking", true);
        StartCoroutine(HandleZumbaPressed());
    }

    private IEnumerator HandleZumbaPressed()
    {
        yield return new WaitForSeconds(4f);
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        animator.SetBool("isWalking", false);
        animator.SetBool("isTalking", true);
        botZumbaUI.SetActive(true);
        zumbaAudio.Play();
        isZumbaHandled = true;
    }

    void TaskOnZumbaMusicClick()
    {
        if (isZumbaHandled && !isDancing)
        {
            animator.SetBool("isDancing", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isTalking", false);
            isDancing = true;
        }
    }

    void TaskOnZumbaStopClick()
    {
        animator.SetBool("isDancing", false);
        animator.SetBool("isTalking", false);
        animator.SetBool("isIdle", true);
        zumbaAudio.Stop();
        if (zumbaMusicButton.GetComponent<AudioSource>().isPlaying)
        {
            zumbaMusicButton.onClick.Invoke();
        }
        isDancing = false;
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        startZumbaButton.interactable = true;
    }

    void TaskOnZumbaRepeatClick()
    {
        zumbaAudio.Play();
        if (!isDancing)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isTalking", true);
        }
    }

    void TaskOnZumbaDisableClick()
    {
        botAvatar.SetActive(false);
        botZumbaUI.SetActive(false);
    }

    public void TaskOnWorkoutClick()
    {
        isStarted = false;
        isZumbaHandled = false; // (*)
        isWorkoutHandled = true;
        startWorkoutButton.interactable = false;
        botUI.SetActive(false);
        botWorkoutUI.SetActive(true);
        botAvatar.transform.localPosition = new Vector3(2.8f, -0.6f, 2.4f);
        animator.SetBool("isIdle", false);
        animator.SetBool("isTalking", true);
        workoutAudio.Play();
        chooseExerciseButton.interactable = false;
    }

    void TaskOnExerciseClick()
    {
        if (!botExercisesUI.activeSelf)
        {
            botExercisesUI.SetActive(true);
        }
        else
        {
            botExercisesUI.SetActive(false);
        }
    }

    void TaskOnWorkoutDisableClick()
    {
        botAvatar.SetActive(false);
        botWorkoutUI.SetActive(false);
    }

    void TaskOnWorkoutStopClick()
    {

        isWorkingOut = false;
        animator.SetBool("isBicepCurls", false);
        animator.SetBool("isFrontRaises", false);
        animator.SetBool("isSquat", false);
        animator.SetBool("isJumpingJacks", false);
        animator.SetBool("isTalking", false);
        animator.SetBool("isIdle", true);
        workoutAudio.Stop();
        if (bicepCurlsButton.gameObject.activeSelf)
        {
            a = Compute_MAX_HR(HR_Data);
            b = Compute_AVG_HR(HR_Data);
            bicepCurlsAudio.Stop();
            isWorkOutStop = true;
            if (us.userid == 4 && us.exercise_id == 1 && us.exercise_end == " ")
            {
                us.exercise_end = System.DateTime.Now.ToString("yyyy - MM - dd\\T HH: mm:ss\\Z");
                us.noOfSets = p.getSets();
                us.avg_HR = b;
                us.max_HR = a;
                Debug.Log("stored average value is  " + b);
                Debug.Log("stored max value is  " + a);
                dateStart = DateTime.Parse(us.exercise_start);
                dateEnd = DateTime.Parse(us.exercise_end);
                us.calories_burnt = db.calculateCaloriesBurnt(us.userid, us.exercise_id, dateStart, dateEnd);
                db.AddRecord(us);

            }
        }
        if (frontRaisesButton.gameObject.activeSelf)
        {
           
            a = Compute_MAX_HR(HR_Data);
            b = Compute_AVG_HR(HR_Data);
            Debug.Log("front raise stored average value is  " +b);
            Debug.Log("stored max value is  " + a);
            frontRaisesAudio.Stop();
            isWorkOutStop = true;
            if (us.userid == 4 && us.exercise_id == 2 && us.exercise_end == " ")
            {
                us.exercise_end = System.DateTime.Now.ToString("yyyy - MM - dd\\T HH: mm:ss\\Z");
                us.noOfSets = p.getSets();
                us.avg_HR = b;
                us.max_HR = a;
                Debug.Log("stored average value is  " +b);
                Debug.Log("stored max value is  " +a);
                dateStart = DateTime.Parse(us.exercise_start);
                dateEnd = DateTime.Parse(us.exercise_end);
                us.calories_burnt = db.calculateCaloriesBurnt(us.userid, us.exercise_id, dateStart, dateEnd);
                db.AddRecord(us);

            }
        }

        if (squatButton.gameObject.activeSelf)
        {
            a = Compute_MAX_HR(HR_Data);
            b = Compute_AVG_HR(HR_Data);
            squatAudio.Stop();
            isWorkOutStop = true;
            if (us.userid == 4 && us.exercise_id == 3 && us.exercise_end == " ")
            {
                us.exercise_end = System.DateTime.Now.ToString("yyyy - MM - dd\\T HH: mm:ss\\Z");
                us.noOfSets = p.getSets();
                us.avg_HR = b;
                us.max_HR = a;
                Debug.Log("stored average value is  " + b);
                Debug.Log("stored max value is  " + a);
                dateStart = DateTime.Parse(us.exercise_start);
                dateEnd = DateTime.Parse(us.exercise_end);
                us.calories_burnt = db.calculateCaloriesBurnt(us.userid, us.exercise_id, dateStart, dateEnd);
                db.AddRecord(us);

            }
        }
        if (jumpingJacksButton.gameObject.activeSelf)
        {
            a = Compute_MAX_HR(HR_Data);
            b = Compute_AVG_HR(HR_Data);
            jumpingJacksAudio.Stop();
            isWorkOutStop = true;
            if (us.userid == 4 && us.exercise_id == 4 && us.exercise_end == " ")
            {
                us.exercise_end = System.DateTime.Now.ToString("yyyy - MM - dd\\T HH: mm:ss\\Z");
                us.noOfSets = p.getSets();
                us.avg_HR = b;
                us.max_HR = a;
                Debug.Log("stored average value is  " + b);
                Debug.Log("stored max value is  " + a);
                dateStart = DateTime.Parse(us.exercise_start);
                dateEnd = DateTime.Parse(us.exercise_end);
                us.calories_burnt = db.calculateCaloriesBurnt(us.userid, us.exercise_id, dateStart, dateEnd);
                db.AddRecord(us);

            }
        }

        botAvatar.transform.localPosition = new Vector3(2.8f, -0.6f, 2.4f);
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }

    void TaskOnWorkoutRepeatClick()
    {
        workoutAudio.Play();
        if (animator.GetBool("isIdle"))
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isTalking", true);
        }
    }

    void TaskOnBicepCurlsClick()
    {
        print("BicepCurls");
        isWorkingOut = true;
        us.userid = 4;
        us.exercise_id = 1;
        us.exercise_start = System.DateTime.Now.ToString("yyyy - MM - dd\\T HH: mm:ss\\Z");
        us.exercise_end = " ";
        us.noOfSets = 0;
        us.avg_HR = 0;
        us.max_HR = 0;
        us.calories_burnt = 0;
     
        botAvatar.transform.localPosition = new Vector3(2.8f, -0.6f, 2.4f);
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        animator.SetBool("isBicepCurls", true);
        animator.SetBool("isFrontRaises", false);
        animator.SetBool("isSquat", false);
        animator.SetBool("isJumpingJacks", false);
        animator.SetBool("isTalking", false);
        animator.SetBool("isIdle", false);
        bicepCurlsAudio.Play();
        frontRaisesAudio.Stop();
        squatAudio.Stop();
        jumpingJacksAudio.Stop();
    }

    void TaskOnFrontRaisesClick()
    {
        print("FrontRaises");
        isWorkingOut = true;
        isWorkOutStop = false;
        us.userid = 4;
        us.exercise_id = 2;
        us.exercise_start = System.DateTime.Now.ToString("yyyy - MM - dd\\T HH: mm: ss\\Z");
        us.exercise_end = " ";
        us.noOfSets = 0;
        us.avg_HR = 0;
        us.max_HR = 0;
        us.calories_burnt = 0;
        botAvatar.transform.localPosition = new Vector3(2.8f, -0.6f, 2.4f);
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        animator.SetBool("isBicepCurls", false);
        animator.SetBool("isFrontRaises", true);
        animator.SetBool("isSquat", false);
        animator.SetBool("isJumpingJacks", false);
        animator.SetBool("isTalking", false);
        animator.SetBool("isIdle", false);
        bicepCurlsAudio.Stop();
        frontRaisesAudio.Play();
        squatAudio.Stop();
        jumpingJacksAudio.Stop();
    }

    void TaskOnSquatClick()
    {
        print("Squats");
        isWorkingOut = true;
        isWorkOutStop = false;
        us.userid = 4;
        us.exercise_id = 3;
        us.exercise_start = System.DateTime.Now.ToString("yyyy - MM - dd\\T HH: mm: ss\\Z");
        us.exercise_end = " ";
        us.noOfSets = 0;
        us.avg_HR = 0;
        us.max_HR = 0;
        us.calories_burnt = 0;
        botAvatar.transform.localPosition = new Vector3(2.8f, -0.6f, 2.4f);
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        animator.SetBool("isBicepCurls", false);
        animator.SetBool("isFrontRaises", false);
        animator.SetBool("isSquat", true);
        animator.SetBool("isJumpingJacks", false);
        animator.SetBool("isTalking", false);
        animator.SetBool("isIdle", false);
        bicepCurlsAudio.Stop();
        frontRaisesAudio.Stop();
        squatAudio.Play();
        jumpingJacksAudio.Stop();
    }

    void TaskOnJumpingJacksClick()
    {
        print("JumpingJacks");
        isWorkingOut = true;
        isWorkOutStop = false;
        us.userid = 4;
        us.exercise_id = 4;
        us.exercise_start = System.DateTime.Now.ToString("yyyy - MM - dd\\T HH: mm: ss\\Z");
        us.exercise_end = " ";
        us.noOfSets = 0;
        us.avg_HR = 0;
        us.max_HR = 0;
        us.calories_burnt = 0;
        botAvatar.transform.localPosition = new Vector3(2.8f, -0.6f, 2.4f);
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        animator.SetBool("isBicepCurls", false);
        animator.SetBool("isFrontRaises", false);
        animator.SetBool("isSquat", false);
        animator.SetBool("isJumpingJacks", true);
        animator.SetBool("isTalking", false);
        animator.SetBool("isIdle", false);
        bicepCurlsAudio.Stop();
        frontRaisesAudio.Stop();
        squatAudio.Stop();
        jumpingJacksAudio.Play();
    }

    void TaskOnEndWorkoutClick()
    {

        isWorkoutHandled = false; // (*)
        isStarted = false;

        animator.SetBool("isBicepCurls", false);
        animator.SetBool("isFrontRaises", false);
        animator.SetBool("isSquat", false);
        animator.SetBool("isJumpingJacks", false);
        isWorkingOut = false;
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isTalking", true);
        workoutAudio.Stop();

        if (bicepCurlsButton.gameObject.activeSelf)
        {
            bicepCurlsAudio.Stop();


        }
        if (frontRaisesButton.gameObject.activeSelf)
        {
            frontRaisesAudio.Stop();

        }
        if (squatButton.gameObject.activeSelf)
        {
            squatAudio.Stop();

        }

        if (jumpingJacksButton.gameObject.activeSelf)
        {
            jumpingJacksAudio.Stop();
        }
        // ***
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        endWorkoutAudio.Play();
        StartCoroutine(WaitForEndWorkoutAudio());
    }

    private IEnumerator WaitForEndWorkoutAudio()
    {
        yield return new WaitForSeconds(15f);

        //
        animator.SetBool("isIdle", true);
        animator.SetBool("isTalking", false);

        botWorkoutUI.SetActive(false); // ***

        botAvatar.transform.position = new Vector3(-10.303f, 1.8446f, -14.568f);
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        botZumbaUI.SetActive(true);
        startZumbaButton.interactable = false;

        yield return new WaitForSeconds(5f);

        zumbaAudio.Play();

        //
        animator.SetBool("isIdle", false);
        animator.SetBool("isTalking", true);

        isZumbaHandled = true;
    }

    void TaskOnEndZumbaClick()
    {
        isZumbaHandled = false; // (*)
        isStarted = false;

        animator.SetBool("isDancing", false);
        isDancing = false;
        animator.SetBool("isIdle", false);
        animator.SetBool("isTalking", true);

        if (zumbaMusicButton.GetComponent<AudioSource>().isPlaying)
        {
            zumbaMusicButton.onClick.Invoke();
        }
        zumbaAudio.Stop();
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        endZumbaAudio.Play();
        StartCoroutine(WaitForEndZumbaAudio());
    }

    private IEnumerator WaitForEndZumbaAudio()
    {
        yield return new WaitForSeconds(15f);
        animator.SetBool("isIdle", true);
        animator.SetBool("isTalking", false);

        botZumbaUI.SetActive(false);
        botAvatar.transform.localPosition = new Vector3(2.8f, -0.6f, 2.4f);
        botAvatar.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        botWorkoutUI.SetActive(true);
        startWorkoutButton.interactable = false;
        chooseExerciseButton.interactable = false;

        yield return new WaitForSeconds(10f);

        workoutAudio.Play();
        animator.SetBool("isIdle", false);
        animator.SetBool("isTalking", true);

        isWorkoutHandled = true;
    }
    public void Get_HR_Value(float new_value)
    {
        print("new value inside gethrvalue is" +new_value);
        if (!isWorkOutStop)//false
        {
            HR_Data.Add(new_value);
            Debug.Log("value of isWorkOutStop is " +isWorkOutStop);
        }
    }
    public double Compute_MAX_HR(List<float> data1)
    {
        max = data1.Max();
        print("the max value is" + max);
        return max;
    }
    public double Compute_AVG_HR(List<float> data2)
    {
        Debug.Log("values stored in list are");
        display(data2);
        average = data2.Average();
        print("the average value is" + average);
        return average;
    }
    public void display(List<float> data3)
    {
        for (int i = 0; i < data3.Count; i++)
        {
            Debug.Log(HR_Data[i]);
        }
    }
}

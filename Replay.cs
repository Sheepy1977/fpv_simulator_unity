using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class PosRotation
{
    public Vector3 position;
    public Quaternion rotation;

    public bool Save(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        return true;
    }
}
public class Replay : MonoBehaviour
{
    public GameObject MainUI;
    public GameObject ReplayUI;
    public Button pausePlayButton, rewindButton;
    //public int cameraSwitchPerFrame = 150; //多少帧切换一次相机位置；
    public GameObject replayCamera;
    public GameObject mainCamera;
    public GameObject trail;
    public GameObject replayLogo;
    public GameObject hud;
    public Text rewindText;
    public Button playButton;
    public Text replayTimeText;
    public Text viewSwitchText;
    public AudioSource engineAudio;

    public Text speed, alt;

    public GameObject replayLeftControlPoint;
    public GameObject replayRightControlPoint;
    
    public Sprite playIcon,pauseIcon;

    public static bool isOnReplay;

    private object[] recordData;
    private bool isPause,is1stView,isHudShow;
    private int frameCounter;
    private int maxFrame;
    private float startReplayTimeStamp,pauseTime;
    //private int cameraSwitchFrame;
    private PosRotation lastPR = new();
    private int fixStep;
    private int stepCounter;
    void Start()
    {
        rewindText.text = "Rewind";
        ReplayUI.SetActive(false);
        StartCoroutine(FlashLogo());
        hud.SetActive(false);
    }
    IEnumerator FlashLogo()
    {
        bool isVisible = false;
       
        while (true)
        {
            if (isVisible)
            {
                replayLogo.SetActive(false);
                isVisible = false;
            }
            else
            {
                replayLogo.SetActive(true);
                isVisible =true;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void FixedUpdate()
    {
        lastPR.Save(gameObject.transform);
        if (isOnReplay)
        {
            //if (frameCounter > cameraSwitchFrame) replayCamera.transform.position = GetCameraPosition();
            if (startReplayTimeStamp == 0) startReplayTimeStamp = Time.time;
            var duration = Time.time - startReplayTimeStamp - pauseTime;
            if (duration > Record.recordTime) duration = Record.recordTime;
            replayTimeText.text = "Replay:" + (Record.recordTime - duration).ToString("F1") + "s";

            if (frameCounter < maxFrame)
            {
                stepCounter++; //计数器，
                var t = recordData[frameCounter].ToString();
                var r = JsonUtility.FromJson<RecordData>(t);
                engineAudio.pitch = r.engineSoundPitch;
                if (stepCounter == 1)
                {
                    gameObject.transform.SetPositionAndRotation(r.position, r.rotation);
                    replayLeftControlPoint.transform.position = r.leftControlPointPos;
                    replayRightControlPoint.transform.position = r.rightControlPointPos;
                    speed.text = r.speed.ToString("F0") + "km/h";
                    alt.text = r.relativeAlt.ToString("F1") + "m";

                    replayCamera.transform.LookAt(gameObject.transform);
                    CheckDistance();
                    if (!isPause)
                    {
                        frameCounter++;
                    }
                    lastPR.Save(gameObject.transform);
                }
                else
                {
                    gameObject.transform.SetPositionAndRotation(lastPR.position, lastPR.rotation);
                }
                if (stepCounter > fixStep) stepCounter = 0;
                if (frameCounter > 10)
                {
                    trail.SetActive(true);
                    SwitchEmitting(true);
                }
                //10帧之后才开始拖尾
            }
            else
            {
                gameObject.transform.SetPositionAndRotation(lastPR.position, lastPR.rotation);
                trail.SetActive(false);
                StartCoroutine(DelayRewind());
            }
        }

        if (isPause)
        {
            pauseTime += Time.deltaTime;
        }
    }

    IEnumerator DelayRewind()
    {
        isPause = true;
        yield return new WaitForSeconds(1f);
        Rewind();
    }

    void CheckDistance()
    {
        var dis = Vector3.Distance(replayCamera.transform.position,gameObject.transform.position);
        var fov = replayCamera.GetComponent<Camera>().fieldOfView;
        if (dis > 50)
        {
            if (fov > 10)
            {
                replayCamera.GetComponent<Camera>().fieldOfView -= 2f;
            }
        }
        else
        {
            if (fov < 50)
            {
                replayCamera.GetComponent<Camera>().fieldOfView += 2f;
            }
        }
    }


    void SwitchEmitting(bool isOn)
    {
        trail.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = isOn;
        trail.transform.GetChild(1).GetComponent<TrailRenderer>().emitting = isOn;
    }

    Vector3 GetCameraPosition()
    {
        float maxHeight = 0;
        for (int i = 0;i < maxFrame; i++)
        {
            Vector3 k = JsonUtility.FromJson<RecordData>(recordData[i].ToString()).position;
            if (k.y > maxHeight) maxHeight = k.y;
        }
        
        var pos1 = JsonUtility.FromJson<RecordData>(recordData[0].ToString()).position;
        var pos2 = JsonUtility.FromJson<RecordData>(recordData[maxFrame - 1].ToString()).position;
        var t = GetBetweenPoint(pos1, pos2); //取起点和终点的中间点
        var randomX = Random.Range(-20, 20);
        var randomZ = Random.Range(-20, 20);
        Vector3 newPos = new Vector3(t.x + randomX, maxHeight, t.z + randomZ);//调整高度为整个轨迹最高点
        
        var terrainH = Terrain.activeTerrain.SampleHeight(newPos);
        if (terrainH >  newPos.y) //避免沉入地下
        {
            newPos = new Vector3(newPos.x, terrainH + Random.Range(3,10), newPos.z);
        }
        return newPos;
    }

    private Vector3 GetBetweenPoint(Vector3 start, Vector3 end, float percent = 0.5f)
    {
        Vector3 normal = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        return normal * (distance * percent) + start;
    }

    public void HudSwitch()
    {
        if (isHudShow)
        {
            hud.SetActive(false);
            isHudShow = false;
        }
        else
        {
            hud.SetActive(true);
            isHudShow = true;
        }
    }


    public void Play()
    {
        isOnReplay = true;
        isPause = false;
        MainUI.SetActive(false);
        ReplayUI.SetActive(true);
        playButton.image.sprite = pauseIcon;
        recordData = Record.jsonArray;
        fixStep = Record.fixStep;
        maxFrame = recordData.Length;
        gameObject.GetComponent<Rigidbody>().isKinematic = true; //关闭rigbody
        View3rd();
        SendMessage("OnReplayMsg");
    }


    public void PausePlay()
    {   
        if (isPause)
        {
            isPause = false;
            playButton.image.sprite = pauseIcon;
        }
        else
        {
            isPause = true;
            playButton.image.sprite = playIcon;
        }
        
    }

    void View1st()
    {
        mainCamera.SetActive(true);
        trail.SetActive(false);
        replayCamera.SetActive(false);
        viewSwitchText.text = "1st";
        hud.SetActive(true);
        isHudShow = true;
    }

    void View3rd()
    {
        replayCamera.transform.position = GetCameraPosition();
        mainCamera.SetActive(false);
        trail.SetActive(false);
        replayCamera.SetActive(true);
        viewSwitchText.text = "TV";
    }

    public void Rewind() 
    {
        SwitchEmitting(false);
        startReplayTimeStamp = 0;
        pauseTime = 0;
        replayCamera.transform.position = GetCameraPosition();
        frameCounter = 0;
        trail.SetActive(true) ;
        isPause = false;
    }

    public void SwitchView()
    {
        if (is1stView)
        {
            is1stView = false;
            View3rd();
        }
        else
        {
            is1stView = true;
            View1st();
        }
    }
    public void RestartMsg()
    {
        isOnReplay = false;
    }
    public void Restart()
    {
        isOnReplay = false;
        SendMessage("RestartMsg");
        SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);
    }
}

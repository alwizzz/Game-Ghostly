using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public LevelMaster levelMaster;

    GameObject arrowHour;
    float arrowHourInitialZ = 0;
    float minuteTravelled = 0;
    float minuteSigma = 0;

    GameObject arrowMinute;
    float arrowMinuteInitialZ = 0;

    private void Start()
    {
        levelMaster = LevelMaster.GetThisSingletonScript();
        arrowHour = transform.Find("Arrow Hour").gameObject;
        arrowMinute = transform.Find("Arrow Minute").gameObject;

        arrowHourInitialZ = arrowHour.transform.eulerAngles.z;
        arrowMinuteInitialZ = arrowMinute.transform.eulerAngles.z;
    }

    private void Update()
    {
        RotateArrowMinute();
        RotateArrowHour();
    }

    void RotateArrowMinute()
    {
        var ratio = 1 - (levelMaster.timer / (levelMaster.timerMax-0.0001f));
        var angle = (ratio * 360) - arrowMinuteInitialZ;

        var prevZ = Mathf.Abs(arrowMinute.transform.eulerAngles.z);
        arrowMinute.transform.eulerAngles = new Vector3(0, 0, -angle);
        var afterZ = Mathf.Abs(arrowMinute.transform.eulerAngles.z);

        minuteSigma = Mathf.Abs(afterZ - prevZ);

        minuteTravelled += minuteSigma;
        //Debug.Log("total angle " + minuteTravelled + ",minuteSigma " + minuteSigma
        //    + "\nprevZ " + prevZ + ", afterZ " + afterZ);
    }
    void RotateArrowHour()
    {
        var ratio = 1 - (minuteTravelled / 360);
        var angle = (ratio * 30) - arrowHourInitialZ;
        arrowHour.transform.eulerAngles = new Vector3(0, 0, angle);
    }
}

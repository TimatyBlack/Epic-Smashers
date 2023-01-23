using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSpeedControl : MonoBehaviour
{
    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = scale * 0.02f;
    }

    public void TimeSpeedUp()
    {
        SetTimeScale(5f);
    }
    
    public void TimeToNormal()
    {
        SetTimeScale(1f);
    }


}

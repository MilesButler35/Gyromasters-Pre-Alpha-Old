using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{

    public void StageOneStart()
    {
        MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "Stage 1");
    }

    public void StageTwoStart()
    {
        MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "Stage 2");
    }

    public void StageThreeStart()
    {
        MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "Stage 3");
    }

    public void BackButton()
    {
        MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "TitleMenu");
    }
}

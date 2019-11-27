using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForResumeGame : CustomYieldInstruction
{
    public override bool keepWaiting
    {
        get
        {
            return GameManager.Instance.GetState() == GameManager.GameState.Paused;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionProccessor
{
    void BeginExecute();
    void ContinueExecute();
    void EndExecute();
    void SetActionStatus();
}

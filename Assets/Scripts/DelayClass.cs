using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class DelayClass
{
    private static DelayClassHelper _helper;
    static DelayClass()
    {
        GameObject helperObject = new GameObject("DelayClassHelper");
        UnityEngine.Object.DontDestroyOnLoad(helperObject);
        _helper = helperObject.AddComponent<DelayClassHelper>();
    }

    public static void DelayMethod(Action action, float delay)
    {
        if (action != null)
        {
            _helper.StartCoroutine(_helper.InvokeActionWithDelay(action, delay));
        }
    }

    private class DelayClassHelper : MonoBehaviour
    {
        public IEnumerator InvokeActionWithDelay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }       
    }
}


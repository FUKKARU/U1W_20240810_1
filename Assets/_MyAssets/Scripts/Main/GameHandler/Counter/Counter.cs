using UnityEngine;

namespace Main.GameHandler
{
    public sealed class Counter : MonoBehaviour
    {
        internal void Wait(System.Action action, float waitSeconds)
        {
            StartCoroutine(General.Ex.Wait(action, waitSeconds));
        }
    }
}
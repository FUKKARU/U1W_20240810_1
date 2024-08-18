using UnityEngine;

namespace Main.GameHandler
{
    public sealed class Counter : MonoBehaviour
    {
        internal void Wait(System.Action action, float waitSeconds)
        {
            StartCoroutine(Ex.Wait(action, waitSeconds));
        }
    }
}
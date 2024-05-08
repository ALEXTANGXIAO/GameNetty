using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuninBackGround : MonoBehaviour
{
    void Start()
    {
        Application.runInBackground = true;
    }
}

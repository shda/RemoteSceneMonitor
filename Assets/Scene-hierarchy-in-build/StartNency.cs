using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CinemaLib.Utils;
using Nancy.Hosting.Self;
using UnityEngine;

public class StartNency : MonoBehaviour
{
    private NancyHost _nancyHost;

    private void OnDestroy()
    {
        _nancyHost.Dispose();
        _nancyHost = null;
    }

    private void Awake()
    {
        _nancyHost = new NancyHost(
            new Uri("http://localhost:8888/nancy/"),
            new Uri("http://127.0.0.1:8898/nancy/"),
            new Uri("http://localhost:8889/nancytoo/"));
        
        _nancyHost.Start();

        Process.Start("http://localhost:8888/nancy/hierarchy/");
    }
}

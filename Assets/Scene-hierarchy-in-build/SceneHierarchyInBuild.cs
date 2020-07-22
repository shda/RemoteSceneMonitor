using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CinemaLib.Utils;
using Http;
using Nancy.Hosting.Self;
using UnityEngine;

public class SceneHierarchyInBuild : MonoBehaviour
{
    [SerializeField]
    private TextAsset treeHtmlFile;
    
    //private NancyHost _nancyHost;

    private HttpServer _httpServer;
    
    private void Awake()
    {
        /*
        
        */
        //var config = new HostConfiguration();
       // config.RewriteLocalhost = true;

        /*
        _nancyHost = new NancyHost(config ,
            new Uri("http://192.168.1.35:8888/nancy/"),
            new Uri("http://127.0.0.1:8898/nancy/"),
            new Uri("http://localhost:8889/nancytoo/"));
        */
        
        _httpServer = new HttpServer(1234);
        _httpServer.Start();
        
        /*
        _nancyHost = new NancyHost(config , new Uri("http://localhost:1234"));
        
        HierarchySceneNancyModule.SetTemplates(new Dictionary<string, string>()
        {
            {TemplateFileNames.treeHtmlFile, treeHtmlFile.text},
        });
        */
        
       // _nancyHost.Start();
#if UNITY_EDITOR
        Process.Start("http://localhost:1234/nancy/hierarchy/");
#endif
    }

    private void OnDestroy()
    {
        //_nancyHost.Dispose();
        //_nancyHost = null;
        
        _httpServer.Dispose();
        _httpServer = null;
    }
}

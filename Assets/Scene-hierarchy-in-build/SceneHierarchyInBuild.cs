using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CinemaLib.Utils;
using Nancy.Hosting.Self;
using UnityEngine;

public class SceneHierarchyInBuild : MonoBehaviour
{
    [SerializeField]
    private TextAsset treeHtmlFile;
    
    private NancyHost _nancyHost;
    
    private void Awake()
    {
        _nancyHost = new NancyHost(
            new Uri("http://localhost:8888/nancy/"),
            new Uri("http://127.0.0.1:8898/nancy/"),
            new Uri("http://localhost:8889/nancytoo/"));
        
        HierarchySceneNancyModule.SetTemplates(new Dictionary<string, string>()
        {
            {TemplateFileNames.treeHtmlFile, treeHtmlFile.text},
        });
        
        _nancyHost.Start();
 
        Process.Start("http://localhost:8888/nancy/hierarchy/");
    }

    private void OnDestroy()
    {
        _nancyHost.Dispose();
        _nancyHost = null;
    }
}

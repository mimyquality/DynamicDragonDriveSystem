using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System;
using System.Threading;

public class AssetListMaker
{
    [MenuItem( "AssetList/Make Asset List" )]
    static private void MakeAssetList()
    {
        TextAsset csvFile = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/AssetListMaker/Data/AssetListURLs.csv");
        string[] URLs;
        URLs = csvFile.text.Split('\n');
        int cnt = 0;
        foreach(string url in URLs){
            if(url == "") break;
            GetURLs(url, cnt);
            cnt++;
            Thread.Sleep(1500);
        }
    }
    static async void GetURLs(string url, int cnt)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest>();
        var request = UnityWebRequest.Get(url);
        var async = request.SendWebRequest();
        async.completed += _ => tcs.SetResult(async.webRequest);
        var response = await tcs.Task;

        if (response.isHttpError || response.isNetworkError) return;

        GameObject parent = GameObject.Find("AssetListMaker/Canvas/Content");
        var titlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AssetListMaker/Prefabs/PageTitle.prefab");
        GameObject titleObject = PrefabUtility.InstantiatePrefab(titlePrefab) as GameObject;
        titleObject.name = "PageTitle (" + cnt + ")";
        titleObject.transform.SetParent(parent.transform);
        titleObject.transform.SetSiblingIndex(cnt * 2 + 1); 
        UnityEngine.UI.Text title = titleObject.GetComponent<UnityEngine.UI.Text>();
        
        var urlPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AssetListMaker/Prefabs/URL.prefab");
        GameObject urlObject = PrefabUtility.InstantiatePrefab(urlPrefab) as GameObject;
        urlObject.name = "URL (" + cnt + ")";
        urlObject.transform.SetParent(parent.transform);
        urlObject.transform.SetSiblingIndex(cnt * 2 + 2); 
        UnityEngine.UI.Text urlText = urlObject.GetComponent<UnityEngine.UI.Text>();

        System.Text.RegularExpressions.Regex r =
            new System.Text.RegularExpressions.Regex(
                @"<(title)\b[^>]*>(.*?)</\1>",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
                | System.Text.RegularExpressions.RegexOptions.Singleline);

        System.Text.RegularExpressions.Match m = r.Match(request.downloadHandler.text);
        title.text = "■" + m.Groups[2].Value;
        urlText.text = "  " + url;
    }
}
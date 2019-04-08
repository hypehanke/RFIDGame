using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using UnityEngine.Networking;

public class parseJSON
{
    public string Name;
    public string Time;
    public ArrayList readTags;
}
public class ReadRFID : MonoBehaviour
{
    //Vasen: 546F5053686F70000000192C
    //Oikea: 546F5053686F70000000192B
    //Keski: 546F5053686F700000001928

    public GameObject vasenObjekti, oikeaObjekti, keskiObjekti;
    public GameObject kamera;

    public Text tags;
    public string currentTag;

    public string url = "http://10.103.1.197:5000/?timeout=200";
    public string method = "GET";

    // Sample JSON for the following script has attached.
    private void Start()
    {
        tags.text = "RFID Tägit:" + "\n" + "\n";
        Debug.Log("Start reading my dude.");
        //StartCoroutine("GetRfidValues");
        
        StartCoroutine(GetRfidValues(url,method));
    }

    private void Update()
    {

        //Vasen
        if (currentTag == "546F5053686F70000000192C")
        {
            //Käännytään vasemmalle
            //kamera.transform.LookAt(vasenObjekti.transform);

            //calculate the rotation needed
            Quaternion neededRotation = Quaternion.LookRotation(vasenObjekti.transform.position - kamera.transform.position);

            //use spherical interpollation over time
            kamera.transform.rotation = Quaternion.Slerp(kamera.transform.rotation, neededRotation, Time.deltaTime * 2f);
        }
        //Oikea
        if (currentTag == "546F5053686F70000000192B")
        {
            //Käännytään oikealle
            //kamera.transform.LookAt(oikeaObjekti.transform);

            //calculate the rotation needed
            Quaternion neededRotation = Quaternion.LookRotation(oikeaObjekti.transform.position - kamera.transform.position);

            //use spherical interpollation over time
            kamera.transform.rotation = Quaternion.Slerp(kamera.transform.rotation, neededRotation, Time.deltaTime * 2f);
        }
        //Keski
        if(currentTag == "546F5053686F700000001928")
        {
            //Käännytään keskelle
            //kamera.transform.LookAt(keskiObjekti.transform);

            //calculate the rotation needed
            Quaternion neededRotation = Quaternion.LookRotation(keskiObjekti.transform.position - kamera.transform.position);

            //use spherical interpollation over time
            kamera.transform.rotation = Quaternion.Slerp(kamera.transform.rotation, neededRotation, Time.deltaTime * 2f);
        }
    }

    IEnumerator GetRfidValues(string url, string method)
    {
        //string url = "http://10.103.1.200:5000/?timeout=200";

        UnityWebRequest www = new UnityWebRequest(url,method);
        /*yield return www;
        if (www.error == null)
        {
            //Processjson(www.text);
            //We get data? Start again my dude!
        }
        else
        {
            Debug.Log("ERROR: " + www.error);
        }
        */
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                Processjson(webRequest.downloadHandler.text);
            }
        }
    }

    public void Processjson(string jsonString)
    {
        tags.text = "RFID Tägit:" + "\n" + "\n";
        JsonData jsonvale = JsonMapper.ToObject(jsonString);
        parseJSON parsejson;
        parsejson = new parseJSON();
        parsejson.Name = jsonvale["Name"].ToString();
        parsejson.Time = jsonvale["Expiry"].ToString();
        parsejson.readTags = new ArrayList();
        for (int i = 0; i < jsonvale["rfidDataToString"].Count; i++)
        {
            parsejson.readTags.Add(jsonvale["rfidDataToString"][i].ToString());
            
        }
        foreach (string tagi in parsejson.readTags)
        {
            Debug.Log(tagi.ToString());
            tags.text = tags.text + tagi.ToString() + "\n";
        }
        if(parsejson.readTags.Count > 0)
        {
            currentTag = parsejson.readTags[0].ToString();
        } 
        Debug.Log(parsejson.Name);
        Debug.Log(parsejson.Time);
        StartCoroutine(GetRfidValues(url, method));

    }
}

//Tai foreach
/*
   for(int i = 0; i<jsonvale["buttons"].Count; i++)
      {
        parsejson.but_title.Add(jsonvale["buttons"][i]["title"].ToString());
        parsejson.but_image.Add(jsonvale["buttons"][i]["image"].ToString());
      }   

*/

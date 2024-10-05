using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class GoogleMaps : MonoBehaviour
{
    private string urlInicial = "https://maps.googleapis.com/maps/api/staticmap?";
    private string apiKey = "AIzaSyCESoM0Tb0Rbw2rqhGbcqGbw8uJYsI9U5w";


    public void getCurrentLocation(VisualIGoQuick visualQuick, bool update, bool testMode)
    {
        StartCoroutine(GetCoords(visualQuick, update, testMode));
    }

    public void ShowMap(VisualIGoQuick visualQuick, GPS gps, HashSet<GPS> markers, RawImage mapImg, bool NightMode, int zoom, string markerColor)
    {
        StartCoroutine(GetLocationRoutine(visualQuick, gps, markers, mapImg, NightMode, zoom, markerColor));
    }

    private IEnumerator GetCoords(VisualIGoQuick visualQuick, bool update, bool testMode)
    {
        if (Input.location.isEnabledByUser){
            Debug.Log("Starting GPS");
            Input.location.Start();
            int maxWait = 15;
            while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1f);
                maxWait--;
            }
            if(maxWait < 1){
                Debug.Log("Timed out");
                PlayerPrefs.SetInt("errogps", 1);
                SceneManager.LoadScene("Startup");
                yield break;
            }
            if(Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("Unable to find device location");
                PlayerPrefs.SetInt("errogps", 1);
                SceneManager.LoadScene("Startup");
            }
            else{
                Debug.Log("Getting last location");
                float latCoord = Input.location.lastData.latitude;
                float longCoord = Input.location.lastData.longitude;
                visualQuick.gps = new GPS(latCoord, longCoord);
                visualQuick.reloadMap();
                if(update) StartCoroutine(visualQuick.autoUpdateMap());
            }
        }else{
            if (testMode)
            {
                visualQuick.gps = VisualIGoQuick.defaultGPS;
                visualQuick.reloadMap();
                if (update) StartCoroutine(visualQuick.autoUpdateMap());
            }
            else
            {
                Debug.Log("Location Services not enabled");
                PlayerPrefs.SetInt("errogps", 1);
                SceneManager.LoadScene("Startup");
            }
        }
    }

    private IEnumerator GetLocationRoutine(VisualIGoQuick visualIQuick, GPS gps, HashSet<GPS> markers, 
        RawImage mapImg, bool NightMode, int zoom, string markerColor)
    {
        string url = urlInicial;
        url = url + "center=" + gps.getLat() + "," + gps.getLon() + "&zoom=" +
            zoom + "&size=" + 1920 + "x" + 1920;

        
        // Add markers
        foreach (GPS g in markers)
        {
            url = url + "&markers=color:0x" + markerColor +
                "%7Clabel:%7C" + g.getLat() + "," + g.getLon();
        }
        url = url + "&style=feature:poi|visibility:off" +
                    "&style=feature:transit.station.bus|visibility:off";
        if (NightMode)
            url = url +
                "&style=element:geometry|color:0x131820" +
                "&style=element:labels.text.stroke|color:0x242f3e" +
                "&style=element:labels.text.fill|color:0x746855" +
                "&style=feature:administrative.locality|element:labels.text.fill|color:0xd59563" +
                "&style=feature:road|element:geometry|color:0x38414e" +
                "&style=feature:road|element:geometry.stroke|color:0x212a37" +
                "&style=feature:road|element:labels.text.fill|color:0x9ca5b3" +
                "&style=feature:road.highway|element:geometry|color:0x746855" +
                "&style=feature:road.highway|element:geometry.stroke|color:0x1f2835" +
                "&style=feature:road.highway|element:labels.text.fill|color:0xf3d19c" +
                "&style=feature:transit|element:geometry|color:0x2f3948" +
                "&style=feature:transit.station|element:labels.text.fill|color:0xd59563" +
                "&style=feature:water|element:geometry|color:0x17263c" +
                "&style=feature:water|element:labels.text.fill|color:0x515c6d" +
                "&style=feature:water|element:labels.text.stroke|color:0x17263c";

            url = url + "&key=" + apiKey;

        using (UnityWebRequest map = UnityWebRequestTexture.GetTexture(url))
        {
            yield return map.SendWebRequest();

            if(map.result == UnityWebRequest.Result.ConnectionError ||
                map.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading map image");
            }
            if(map.responseCode >= 400)
            {
                yield return new WaitForSeconds(0.5f);
                getCurrentLocation(visualIQuick, false, false);
                yield return null;
            }
            mapImg.texture = DownloadHandlerTexture.GetContent(map);
            visualIQuick.runningShowMap = false;
        }
    }

}

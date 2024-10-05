using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.UI;


public class GuestGoQuick : IGuestGoQuick
{
    private string access_key = "nhc34t88h9n4t3vy4vwyc45wcgt45";
    private static string server = "https://goquickserver.duarte75.repl.co/";
    private string dataOut = "";
    private GoogleMaps mapApi = null;
    private Dictionary<GPS, string> allMarkers = new Dictionary<GPS, string>();
    private ConcurrentDictionary<string, Paragem> paragemCache = new ConcurrentDictionary<string, Paragem>();


    public GuestGoQuick(GameObject go)
    {
        mapApi = go.AddComponent<GoogleMaps>();
    }

    private List<string> Request()
    {
        int index = dataOut.LastIndexOf("«");
        if (index >= 0 && index < dataOut.Length) dataOut.Remove(index);

        UnityWebRequestAsyncOperation asyncOperation = 
            UnityWebRequest.Get(server + "request/" + access_key + "«" + dataOut).SendWebRequest();

        while (!asyncOperation.isDone)
            Thread.Sleep(10);

        dataOut = "";

        if(asyncOperation.webRequest.isNetworkError ||
           (asyncOperation.webRequest.downloadHandler.text.Length > 0 &&
            asyncOperation.webRequest.downloadHandler.text.Substring(0, 1).Equals("<")))
        {
            Debug.Log("Server offline");
            PlayerPrefs.SetInt("serveroffline", 1);
            SceneManager.LoadScene("Startup");
            return null;
        }
        else
        {
            List<string> list = new List<string>(asyncOperation.webRequest.
                downloadHandler.text.Split("«".ToCharArray()));
            list.RemoveAt(list.Count - 1);
            return list;
        }
    }
   

    private void Write(string s)
    {
        dataOut += s + "«";
    }

    public HashSet<GPS> getParagensLocalizadas(GPS gps, int raio)
    {
        HashSet<GPS> coords = new HashSet<GPS>();
        // Send Tag
        Write(1.ToString());
        // Send Coordinates
        Write(gps.getLat().ToString().Replace(",", "."));
        Write(gps.getLon().ToString().Replace(",", "."));
        // Send Radius
        Write(raio.ToString());
        List<string> list;
        if ((list = Request()) == null) return null;

        // Receive list of coordinates
        for (int i = 0; i < list.Count; i += 3)
        {
            string nome = list[i];
            float latitude = float.Parse(list[i + 1].Replace(".", ","));
            float longitude = float.Parse(list[i + 2].Replace(".", ","));
            GPS coord = new GPS(latitude, longitude);
            allMarkers.Add(coord, nome);
            coords.Add(coord);
        }

        return coords;
    }


    public Paragem getInfoParagem(GPS coordParagem, string n)
    {
        // Check for name of gps
        string name;
        if (coordParagem == null) name = n;
        else allMarkers.TryGetValue(coordParagem, out name);

        // Check if in cache
        if (paragemCache.ContainsKey(name))
        {
            Paragem p;
            paragemCache.TryGetValue(name, out p);
            return p;
        }

        // Start fetching by name on server
        Write(2.ToString());
        Write(name);

        List<string> list;
        if ((list = Request()) == null || list.Count == 0) return null;
        if (list[0] == "erro" || list.Count < 7) return null;

        List<Rota> rotas = new List<Rota>();

        for (int i = 7; i < list.Count; i += 8)
            rotas.Add(new Rota(
                list[i],
                list[i + 1],
                list[i + 2],
                list[i + 3],
                list[i + 4],
                list[i + 5],
                list[i + 6],
                list[i + 7]));

        Paragem paragem = new Paragem(
            list[0],
            new GPS(float.Parse(list[4].Replace(".", ",")), float.Parse(list[5].Replace(".", ","))),
            list[1],
            list[2],
            list[3],
            list[6],
            rotas);
        paragemCache[paragem.getNome()] = paragem;
        return paragem;
    }


    public void ShowMap(VisualIGoQuick visualQuick, GPS gps, HashSet<GPS> markers, 
        RawImage mapImg, bool NightMode, int zoom, string markerColor)
    {
        mapApi.ShowMap(visualQuick, gps, markers, mapImg, NightMode, zoom, markerColor);
    }


    public void getCurrentLocation(VisualIGoQuick visualQuick, bool update, bool testMode)
    {
        mapApi.getCurrentLocation(visualQuick, update, testMode);
    }
}

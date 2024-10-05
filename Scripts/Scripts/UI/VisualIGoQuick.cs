using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;


public class VisualIGoQuick : MonoBehaviour
{
    [Header("Show Paragem Components")]
    [SerializeField] RectTransform rotasContentTransform = null;
    [SerializeField] GameObject rotaGameObject = null;
    [SerializeField] RawImage imagemParagem = null;
    [SerializeField] Animator infoAnimator = null;
    [SerializeField] Text descricaoParagem = null;
    [SerializeField] Text enderecoParagem = null;
    [SerializeField] Image infoBackground = null;
    [SerializeField] Text nomeParagem = null;

    [Header("UI NightMode Components")]
    [SerializeField] Image toggleButton = null;
    [SerializeField] Sprite toggleNight = null;
    [SerializeField] Sprite backNight = null;
    [SerializeField] Sprite zoomNight = null;
    [SerializeField] Image backButton = null;
    [SerializeField] Image zoomButton = null;
    [SerializeField] Sprite toggleDay = null;
    [SerializeField] Text radiusText = null;
    [SerializeField] Sprite backDay = null;
    [SerializeField] Sprite zoomDay = null;
    [SerializeField] Image downBar = null;
    [SerializeField] Image topBar = null;
    [SerializeField] Image title = null;

    [Header("Google Maps Components")]
    [SerializeField] Color markerBaseColorNight = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] Color markerBaseColorDay = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] float mapCoordDiff = 0.0002f;
    [SerializeField] float mapReloadTime = 5f;
    [SerializeField] RawImage mapImg = null;
    [SerializeField] int pixelError = 25;
    [SerializeField] Camera cam = null;

    [HideInInspector]public static GPS defaultGPS = new GPS(41.552733f, -8.401622f);
    [HideInInspector] public string markerColorString = "";
    [HideInInspector] public bool runningShowMap = true;
    [HideInInspector] public GPS gps = null;

    [SerializeField] Color clearColor = new Color(0.1411764706f, 0.1843137255f, 0.2431372549f);
    [SerializeField] Color darkColor = new Color(0.9490196078f, 0.9529411765f, 0.9568627451f);
    private HashSet<GPS> markers = new HashSet<GPS>();
    private bool showingParagem = false;
    private GPS lastGps = new GPS(0, 0);
    private bool lastNightMode = true;
    private bool NightMode = false;
    private IGuestGoQuick sistema;
    private bool testMode = false;
    private bool usingGps = false;
    private bool clicked = false;
    private bool mouse = false;
    private int lastZoom = 20;
    private int zoom = 16;
    private Rect viewRect;


    void Start()
    {
        if (PlayerPrefs.GetInt("nightmode", 0) == 1) NightMode = true;
        if(PlayerPrefs.GetInt("usinggps", 0) == 1) usingGps = true;
        if(PlayerPrefs.GetInt("testmode", 0) == 1) testMode = true;
        sistema = new GuestGoQuick(this.gameObject);

        mapImg.color = new Color(1, 1, 1, 1);
        lastNightMode = !NightMode;
        viewRect = cam.pixelRect;
        colorUI();

        if (usingGps) sistema.getCurrentLocation(this, true, testMode);
        else reloadMap();
    }

    public IEnumerator autoUpdateMap()
    {
        while (true)
        {
            yield return new WaitForSeconds(mapReloadTime);
            sistema.getCurrentLocation(this, false, testMode);
        }
    }

    private void colorUI()
    {
        if (NightMode)
        {
            title.color = darkColor;
            backButton.sprite = backNight;// backDay;
            toggleButton.sprite = toggleNight;// toggleDay;
            zoomButton.sprite = zoomNight;// zoomDay;
            topBar.color = clearColor;
            downBar.color = new Color(.9f, .9f, .9f, .45f);
            radiusText.color = darkColor;
            cam.backgroundColor = clearColor;
            // Info
            infoBackground.color = clearColor;
            nomeParagem.color = darkColor;
            enderecoParagem.color = darkColor;
            descricaoParagem.color = darkColor;
            // Markers
            string colorR = ((int)(markerBaseColorNight.r * 255f)).ToString("X");
            if (colorR.Length < 2) colorR = "0" + colorR;
            string colorG = ((int)(markerBaseColorNight.g * 255f)).ToString("X");
            if (colorG.Length < 2) colorG = "0" + colorG;
            string colorB = ((int)(markerBaseColorNight.b * 255f)).ToString("X");
            if (colorB.Length < 2) colorB = "0" + colorB;
            markerColorString = colorR + colorG + colorB;
        }
        else
        {
            title.color = clearColor;
            backButton.sprite = backNight;
            toggleButton.sprite = toggleNight;
            zoomButton.sprite = zoomNight;
            topBar.color = darkColor;
            downBar.color = new Color(.1f, .1f, .1f, .45f);
            radiusText.color = clearColor;
            cam.backgroundColor = darkColor;
            // Info
            infoBackground.color = clearColor;
            nomeParagem.color = darkColor;
            enderecoParagem.color = darkColor;
            descricaoParagem.color = darkColor;
            // Markers
            string colorR = ((int)(markerBaseColorDay.r * 255f)).ToString("X");
            if (colorR.Length < 2) colorR = "0" + colorR;
            string colorG = ((int)(markerBaseColorDay.g * 255f)).ToString("X");
            if (colorG.Length < 2) colorG = "0" + colorG;
            string colorB = ((int)(markerBaseColorDay.b * 255f)).ToString("X");
            if (colorB.Length < 2) colorB = "0" + colorB;
            markerColorString = colorR + colorG + colorB;
        }
    }

    public void toggleNightMode()
    {
        if (runningShowMap) return;
        runningShowMap = true;
        if (NightMode) PlayerPrefs.SetInt("nightmode", 0);
        else PlayerPrefs.SetInt("nightmode", 1);
        NightMode = !NightMode;
        colorUI();
        reloadMap();
    }

    public void goBack()
    {
        SceneManager.LoadScene("Startup");
    }

    public void MoreZoom()
    {
        if (runningShowMap) return;
        runningShowMap = true;
        zoom = Math.Min(19, zoom + 1);
        reloadMap();
    }

    public void LessZoom()
    {
        if (runningShowMap) return;
        runningShowMap = true;
        zoom = Math.Max(6, zoom - 1);
        reloadMap();
    }

    public void reloadMap()
    {
        if (usingGps && testMode)
        {
            gps = defaultGPS;
        }
        else if(!usingGps)
        {
            gps = defaultGPS;
            ShowParagem(null, PlayerPrefs.GetString("paragem", ""));
        }

        if (gps == null || (gps.getLat() == 0 && gps.getLon() == 0)) {
            SceneManager.LoadScene("Startup");
            return;
        }

        if (lastZoom == zoom                                         &&
            lastNightMode == NightMode                               &&
            Math.Abs(gps.getLat() - lastGps.getLat()) < mapCoordDiff &&
            Math.Abs(gps.getLon() - lastGps.getLon()) < mapCoordDiff)
        {
            Debug.Log("Not reloading map");
            runningShowMap = false;
            return;
        }

        Debug.Log("Reloading map");

        int dist = (int)(metersOfTile() / viewRect.height * viewRect.width * 0.947 / 2 / 1.17708333f);
        radiusText.text = dist > 1000 ? ((float) dist / 1000f).ToString(".0") + "Km" : dist.ToString() + "m";
        Debug.Log("Radius: " + dist);

        HashSet<GPS> set = sistema.getParagensLocalizadas(gps, dist);
        markers = set == null ? markers : set;

        sistema.ShowMap(this, gps, markers, mapImg, NightMode, zoom, markerColorString);

        lastNightMode = NightMode;
        lastZoom = zoom;
        lastGps = gps;
    }

    public double metersOfTile()
    {
        int pixels = 190;
        double latitude = gps.getLat();
        return 10 * (Math.Cos(Math.PI / 180.0 * latitude) * 40007863 / Math.Pow(2, zoom) / 770 * pixels);
    }

    void Update()
    {
        mouse = false;
        if (Input.touchCount == 0 && !Input.GetMouseButton(0)) clicked = false;
        else if (!showingParagem && (usingGps && !clicked && (Input.touchCount > 0 || (mouse = Input.GetMouseButton(0)))))
        {
            clicked = true;
            if (mouse)
            {
                selectMarker(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            }
            else
            {
                Touch touch = Input.GetTouch(0);
                selectMarker(touch.position);
            }
        }
    }

    private void selectMarker(Vector2 pos)
    {
        if (Math.Abs((int)pos.y - viewRect.height / 2) > 0.32f * viewRect.height)
            return;
        double metersPerPixel = metersOfTile() / viewRect.height;
        double m = (1 / ((2 * Math.PI / 360) * 6378.137)) / 1000;

        GPS coords = null;
        foreach (GPS g in markers)
        {
            int diffY = (int)((g.getLat() - gps.getLat()) / m / metersPerPixel);
            int diffX = (int)((g.getLon() - gps.getLon()) * Math.Cos(g.getLat() * (Math.PI / 180)) / m / metersPerPixel);
            int posY = (int)(viewRect.height / 2) + diffY;
            int posX = (int)(viewRect.width / 2) + diffX;
            int myDiffX = (int)Math.Abs(pos.x - posX);
            int myDiffY = (int)Math.Abs(pos.y - posY - pixelError);
            if (myDiffX < pixelError && myDiffY < pixelError)
            {
                coords = g;
                break;
            }
        }
        if (coords != null)
        {
            Debug.Log("Showing bus stop at: " + coords);
            ShowParagem(coords, null);
        }
    }

    private void ShowParagem(GPS gps, string nome)
    {
        Paragem paragem = sistema.getInfoParagem(gps, nome);
        if(paragem == null)
        {
            Debug.Log("Paragem nao existe");
            PlayerPrefs.SetInt("erroparagem", 1);
            SceneManager.LoadScene("Startup");
            return;
        }
        showingParagem = true;
        StartCoroutine(DownloadImage(paragem.getLinkImage(), imagemParagem));
        nomeParagem.text = paragem.getNome();
        enderecoParagem.text = paragem.getEndereco();
        descricaoParagem.text = paragem.getDescricao();
        List<Rota> rotas = paragem.getRotas();
        float initY = -105f;
        float sizeY = 210;
        foreach(Rota rota in rotas)
        {
            GameObject r = Instantiate(rotaGameObject, rotasContentTransform);
            r.GetComponent<RectTransform>().localPosition = new Vector2(0, initY);
            initY -= sizeY;
            Transform t = r.transform;
            t.GetChild(0).GetComponent<Text>().text = rota.getDuracao() + " min";
            t.GetChild(1).GetComponent<Text>().text = rota.getFim();
            t.GetChild(2).GetComponent<Text>().text = rota.getInicio();
            t.GetChild(3).GetComponent<Text>().text = rota.getDestino();
            t.GetChild(4).GetComponent<Text>().text = rota.getOrigem();
            t.GetChild(8).GetComponent<Text>().text = rota.getNumeroAutocarro();
            t.GetChild(10).GetComponent<Text>().text = rota.getFuncionamento();
        }
        rotasContentTransform.offsetMin = new Vector2(0,
            766f -  (sizeY * rotas.Count));
        rotasContentTransform.offsetMax = Vector2.zero;

        infoAnimator.Play("ParagemEntry");
    }

    public void stopShowingParagem()
    {
        if (!usingGps) return;
        showingParagem = false;
        infoAnimator.Play("ParagemExit");
        foreach (Transform t in rotasContentTransform)
            Destroy(t.gameObject);
    }

    private IEnumerator DownloadImage(string MediaUrl, RawImage image)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            while (!request.isDone) yield return new WaitForSeconds(0.01f);
            image.SetNativeSize();
            RectTransform t = image.gameObject.GetComponent<RectTransform>();
            t.sizeDelta = new Vector2(370, t.sizeDelta.y);
        }
    }

}

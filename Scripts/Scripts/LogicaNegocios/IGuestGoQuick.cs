using System.Collections.Generic;
using UnityEngine.UI;


public interface IGuestGoQuick
{
    HashSet<GPS> getParagensLocalizadas(GPS gps, int raio);

    Paragem getInfoParagem(GPS coordParagem, string n);

    void getCurrentLocation(VisualIGoQuick visualQuick, bool update, bool testMode);

    void ShowMap(VisualIGoQuick visualQuick, GPS gps, HashSet<GPS> markers,
        RawImage mapImg, bool NightMode, int zoom, string markerColor);
}

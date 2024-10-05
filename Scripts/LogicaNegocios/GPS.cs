
public class GPS
{
    private float latitude;
    private float longitude;

    public GPS(float lat, float lon)
    {
        latitude = lat;
        longitude = lon;
    }

    public float getLat() { return latitude; }

    public void setLat(float lat) { latitude = lat; }

    public float getLon() { return longitude; }

    public void setLon(float lon) { longitude = lon; }

    override
    public string ToString()
    {
        return "Location: " + latitude + ", " + longitude;
    }
}

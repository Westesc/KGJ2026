using UnityEngine;

public class ConnectionsData
{
    public int indexA;
    public int indexB;
    public bool isHorizontal;

    public ConnectionsData(int a, int b, bool isHor)
    {
        this.indexA = a;
        this.indexB = b;
        this.isHorizontal = isHor;
    }
}

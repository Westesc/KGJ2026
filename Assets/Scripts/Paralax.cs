using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [System.Serializable]
    public class LayerLine
    {
        public SpriteRenderer centerLayer;
        public SpriteRenderer leftLayer;
        public SpriteRenderer rightLayer;
    }

    [System.Serializable]
    public class Layer
    {
        public SpriteRenderer baseLayer;
        [Range(0.0f, 1.0f)]
        public float paralaxEffect;

        internal float length;
        internal float depth;
        internal float errorX = 0;
        internal float errorZ = 0;
        internal LayerLine topLine;
        internal LayerLine centerLine;
        internal LayerLine bottomLine;
    }

    public GameObject playerCamera;
    public List<Layer> layers;

    private Vector3 lastPlayerCameraPos = Vector3.zero;

    void InitLayerLine(ref LayerLine line, SpriteRenderer centerLayer, float length)
    {
        line = new()
        {
            centerLayer = centerLayer,
            leftLayer = Instantiate(centerLayer, transform, false),
            rightLayer = Instantiate(centerLayer, transform, false)
        };
        line.leftLayer.transform.position = new Vector3(centerLayer.transform.position.x - length, centerLayer.transform.position.y, centerLayer.transform.position.z);
        line.rightLayer.transform.position = new Vector3(centerLayer.transform.position.x + length, centerLayer.transform.position.y, centerLayer.transform.position.z);
    }

    void InitLayer(Layer layer)
    { 
        layer.length = layer.baseLayer.bounds.size.x;
        layer.depth = layer.baseLayer.size.y * layer.baseLayer.transform.lossyScale.y / Mathf.Sqrt(2);

        // TOP
        SpriteRenderer topCenterLayer = Instantiate(layer.baseLayer, transform, false);
        topCenterLayer.transform.position = new Vector3(layer.baseLayer.transform.position.x, layer.baseLayer.transform.position.y + layer.depth, layer.baseLayer.transform.position.z + layer.depth);
        InitLayerLine(ref layer.bottomLine, topCenterLayer, layer.length);

        // CENTER
        InitLayerLine(ref layer.centerLine, layer.baseLayer, layer.length);

        // BOTTOM
        SpriteRenderer bottomCenterLayer = Instantiate(layer.baseLayer, transform, false);
        bottomCenterLayer.transform.position = new Vector3(layer.baseLayer.transform.position.x, layer.baseLayer.transform.position.y - layer.depth, layer.baseLayer.transform.position.z - layer.depth);
        InitLayerLine(ref layer.topLine, bottomCenterLayer, layer.length);
    }

    void Start()
    {
        lastPlayerCameraPos = playerCamera.transform.position;
        for (int i = 0; i < layers.Count; ++i)
        {
            InitLayer(layers[i]);
        }
    }

    void UpdateLine(LayerLine line, float distX, float distZ)
    {
        line.leftLayer.transform.localPosition = new Vector3(line.leftLayer.transform.localPosition.x - distX, line.leftLayer.transform.localPosition.y - distZ, line.leftLayer.transform.localPosition.z);

        line.centerLayer.transform.localPosition = new Vector3(line.centerLayer.transform.localPosition.x - distX, line.centerLayer.transform.localPosition.y - distZ, line.centerLayer.transform.localPosition.z);
        
        line.rightLayer.transform.localPosition = new Vector3(line.rightLayer.transform.localPosition.x - distX, line.rightLayer.transform.localPosition.y - distZ, line.rightLayer.transform.localPosition.z);
    }

    void UpdateLayer(Layer layer, Vector3 camDiff)
    {
        float distX = camDiff.x * layer.paralaxEffect;
        float distZ = camDiff.z * layer.paralaxEffect;

        layer.errorX += distX;

        if (layer.errorX > layer.length)
        {
            distX -= layer.length;
            layer.errorX -= layer.length;
        }
        else if (layer.errorX < -layer.length)
        {
            distX += layer.length;
            layer.errorX += layer.length;
        }

        layer.errorZ += distZ;

        if (layer.errorZ > layer.depth)
        {
            distZ -= layer.depth;
            layer.errorZ -= layer.depth;
        }
        else if (layer.errorZ < -layer.depth)
        {
            distZ += layer.depth;
            layer.errorZ += layer.depth;
        }

        UpdateLine(layer.centerLine, distX, distZ);

        UpdateLine(layer.topLine, distX, distZ);

        UpdateLine(layer.bottomLine, distX, distZ);
    }

    void Update()
    {
        Vector3 camDiff = playerCamera.transform.position - lastPlayerCameraPos;
        for (int i = 0; i < layers.Count; ++i)
        {
            UpdateLayer(layers[i], camDiff);
        }
        lastPlayerCameraPos = playerCamera.transform.position;
    }
}

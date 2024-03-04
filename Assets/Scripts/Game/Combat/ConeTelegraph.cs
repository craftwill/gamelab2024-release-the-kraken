using System.Collections;

using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;



public class ConeTelegraph : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float chargeRatio;
    [SerializeField] private float _range;
    [SerializeField] private float _angle;
    [SerializeField] private LayerMask _obstructingLayer;//layers that block the cone
    [SerializeField] private int _triangleResolution = 120;

    [SerializeField] private Material _materialInner;
    [SerializeField] private Material _materialOuter;
    [SerializeField] private MeshRenderer _meshRendererInner;
    [SerializeField] private MeshRenderer _meshRendererOuter;
    [SerializeField] private MeshFilter _meshFilterInner;
    [SerializeField] private MeshFilter _meshFilterOuter;

    private Mesh _coneMeshInner;
    private Mesh _coneMeshOuter;
    

    private void Start()
    {
        _meshRendererInner.material = _materialInner;
        _meshRendererOuter.material = _materialOuter;

        _coneMeshInner = new Mesh();
        _coneMeshOuter = new Mesh();

        _angle *= Mathf.Deg2Rad;
    }

    private void FixedUpdate()
    {
        DrawVisionCone(chargeRatio);
    }



    void DrawVisionCone(float chargeRatio)
    {
        int[] trianglesInner = new int[(_triangleResolution - 1) * 3];
        int[] trianglesOuter = new int[(_triangleResolution - 1) * 6];

        Vector3[] verticesInner = new Vector3[_triangleResolution + 1];
        Vector3[] verticesOuter = new Vector3[_triangleResolution * 2];

        verticesInner[0] = Vector3.zero; //start position

        float Currentangle = -_angle / 2;

        float angleIncrement = _angle / (_triangleResolution - 1);
        
        float Sine;

        float Cosine;

        for(int i = 0; i < _triangleResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);

            Cosine = Mathf.Cos(Currentangle);

            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);

            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);

            if(Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, _range, _obstructingLayer))
            {
                if (hit.transform.CompareTag("Player")){
                }
                verticesInner[i + 1] = VertForward * Mathf.Min(chargeRatio * _range, hit.distance);
                verticesOuter[2 * i] = VertForward * Mathf.Min(chargeRatio * _range, hit.distance);
                verticesOuter[(2 * i) + 1] = VertForward * Mathf.Min(_range, hit.distance);
            }
            else
            {
                verticesInner[i + 1] = VertForward * chargeRatio * _range;
                verticesOuter[2 * i] = VertForward * chargeRatio * _range;
                verticesOuter[(2 * i) + 1] = VertForward * _range;
            }

            Currentangle += angleIncrement;
        }

        for(int i = 0, j = 0; i < trianglesInner.Length; i += 3, j++)
        {
            trianglesInner[i] = 0;
            trianglesInner[i + 1] = j + 1;
            trianglesInner[i + 2] = j + 2;
        }

        for(int i = 0, j = 0; i < (_triangleResolution - 1); i++, j += 2)
        {
            int triangleIndex = i * 6;

            trianglesOuter[triangleIndex] = j;
            trianglesOuter[triangleIndex + 1] = j + 2;
            trianglesOuter[triangleIndex + 2] = j + 1;

            trianglesOuter[triangleIndex + 3] = j + 1;
            trianglesOuter[triangleIndex + 4] = j + 2;
            trianglesOuter[triangleIndex + 5] = j + 3;
        }

        _coneMeshInner.Clear();
        _coneMeshOuter.Clear();

        _coneMeshInner.vertices = verticesInner;
        _coneMeshOuter.vertices = verticesOuter;

        _coneMeshInner.triangles = trianglesInner;
        _coneMeshOuter.triangles = trianglesOuter;

        _meshFilterInner.mesh = _coneMeshInner;
        _meshFilterOuter.mesh = _coneMeshOuter;
    }
}
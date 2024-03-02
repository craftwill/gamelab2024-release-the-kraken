using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Cone : MonoBehaviour
{
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;

    private List<Vector3> _vertices;
    private List<int> _triangles;

    [SerializeField] private Material _material;
    [SerializeField] private float _height;
    [SerializeField] private float _radius;
    [SerializeField] private int _segments;
    [SerializeField] private MeshCollider _meshCollider;

    private Vector3 _pos;
    private float _angle;
    private float _angleAmount;
    // Start is called before the first frame update
    void Start()
    {
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = _material;
        _mesh = new Mesh();
        meshFilter.mesh = _mesh;

        _vertices = new List<Vector3>();
        _pos = new Vector3();

        _angleAmount = 2 * Mathf.PI / _segments;
        _angle = 0f;

        _pos.x = 0f;
        _pos.y = _height;
        _pos.z = 0f;
        _vertices.Add(new Vector3(_pos.x, _pos.y, _pos.z));

        _pos.y = 0f;
        _vertices.Add(new Vector3(_pos.x, _pos.y, _pos.z));

        for(int i = 0; i <_segments; ++i)
        {
            _pos.x = _radius * Mathf.Sin(_angle);
            _pos.z = _radius * Mathf.Cos(_angle);

            _vertices.Add(new Vector3(_pos.x, _pos.y, _pos.z));
            _angle -= _angleAmount;
        }

        _mesh.vertices = _vertices.ToArray();

        _triangles = new List<int>();

        for(int i = 2; i <_segments + 1; ++i)
        {
            _triangles.Add(0);
            _triangles.Add(i + 1);
            _triangles.Add(i);
        }

        _triangles.Add(0);
        _triangles.Add(2);
        _triangles.Add(_segments + 1);

        _mesh.triangles = _triangles.ToArray();


        string path = Path.Combine(Application.persistentDataPath, "");
        
        //_meshCollider.material = _material;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

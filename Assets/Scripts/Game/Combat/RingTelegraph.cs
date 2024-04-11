using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class RingTelegraph : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilterInner;
        [SerializeField] private MeshFilter _meshFilterOuter;

        private Mesh _meshInner;
        private Mesh _meshOuter;
        private Vector3[] _innerPolygonPoints;
        private Vector3[] _outerPolygonPoints;
        private int[] _innerPolygonTriangles;
        private int[] _outerPolygonTriangles;

        private const int POLYGON_SIDES = 50;

        public void StartTelegraph(float chargeTime, float telegraphRadius, float offsetRadius = 0f, int damage = 0)
        {
            _meshInner = new Mesh();
            _meshOuter = new Mesh();
            _meshFilterInner.mesh = _meshInner;
            _meshFilterOuter.mesh = _meshOuter;
            StartCoroutine(DrawCoroutine(chargeTime, telegraphRadius, offsetRadius, damage));
        }

        private IEnumerator DrawCoroutine(float chargeTime, float telegraphRadius, float offsetRadius, int damage)
        {
            float time = 0f;

            while(time < chargeTime)
            {
                float chargeRatio = time / chargeTime;

                DrawHollow(POLYGON_SIDES, (chargeRatio * telegraphRadius) + offsetRadius, offsetRadius, ref _innerPolygonPoints, ref _innerPolygonTriangles, ref _meshInner);
                DrawHollow(POLYGON_SIDES, telegraphRadius + offsetRadius, (chargeRatio * telegraphRadius) + offsetRadius, ref _outerPolygonPoints, ref _outerPolygonTriangles, ref _meshOuter);
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                PlayerEntity[] players = CombatUtils.GetPlayerEntities();

                System.Array.ForEach(players, p =>
                {
                    if (p == null) return;
                    float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(p.transform.position.x, p.transform.position.z));
                    //added 0.5f because you otherwise it was unreliable to hit
                    if (distance >= offsetRadius && distance <= telegraphRadius + offsetRadius + 0.5f)
                    {
                        var ddc = p.GetComponentInChildren<Kraken.Game.DetectDamageComponent>();
                        if (ddc) ddc.TakeDamageFromOtherSource(damage);
                    }
                });
            }

            Destroy(this.gameObject);
        }

        //got help for the below functions from https://www.youtube.com/watch?v=YG-gIX_OvSE
        private static void DrawFilled(int sides, float radius, ref Vector3[] polyPoints, ref int[] polyTriangles, ref Mesh mesh, float chargeRatio = 1f)
        {
            polyPoints = GetCircumferencePoints(sides, radius).ToArray();
            polyTriangles = DrawFilledTriangles(polyPoints);
            mesh.Clear();
            mesh.vertices = polyPoints;
            mesh.triangles = polyTriangles;
        }

        private static void DrawHollow(int sides, float outerRadius, float innerRadius, ref Vector3[] polyPoints, ref int[] polyTriangles, ref Mesh mesh, float chargeRatio = 1f)
        {
            List<Vector3> pointsList = new List<Vector3>();
            List<Vector3> outerPoints = GetCircumferencePoints(sides, outerRadius);
            pointsList.AddRange(outerPoints);
            List<Vector3> innerPoints = GetCircumferencePoints(sides, innerRadius);
            pointsList.AddRange(innerPoints);

            polyPoints = pointsList.ToArray();

            polyTriangles = DrawHollowTriangles(polyPoints);
            mesh.Clear();
            mesh.vertices = polyPoints;
            mesh.triangles = polyTriangles;
        }

        private static int[] DrawHollowTriangles(Vector3[] points)
        {
            int sides = points.Length / 2;
            int triangleAmount = sides * 2;

            List<int> newTriangles = new List<int>();
            for (int i = 0; i < sides; i++)
            {
                int outerIndex = i;
                int innerIndex = i + sides;

                //first triangle starting at outer edge i
                newTriangles.Add(outerIndex);
                newTriangles.Add(innerIndex);
                newTriangles.Add((i + 1) % sides);

                //second triangle starting at outer edge i
                newTriangles.Add(outerIndex);
                newTriangles.Add(sides + ((sides + i - 1) % sides));
                newTriangles.Add(outerIndex + sides);
            }
            return newTriangles.ToArray();
        }

        private static List<Vector3> GetCircumferencePoints(int sides, float radius)
        {
            List<Vector3> points = new List<Vector3>();
            float circumferenceProgressPerStep = (float)1 / sides;
            float TAU = 2 * Mathf.PI;
            float radianProgressPerStep = circumferenceProgressPerStep * TAU;

            for (int i = 0; i < sides; i++)
            {
                float currentRadian = radianProgressPerStep * i;
                points.Add(new Vector3(Mathf.Cos(currentRadian) * radius, Mathf.Sin(currentRadian) * radius, 0));
            }
            return points;
        }

        private static int[] DrawFilledTriangles(Vector3[] points)
        {
            int triangleAmount = points.Length - 2;
            List<int> newTriangles = new List<int>();
            for (int i = 0; i < triangleAmount; i++)
            {
                newTriangles.Add(0);
                newTriangles.Add(i + 2);
                newTriangles.Add(i + 1);
            }
            return newTriangles.ToArray();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Chain : MonoBehaviour
{

    [SerializeField]
    private bool makeCircle;

    [SerializeField]
    private GameObject pointPrefab, chainColliderPrefab, pushOutBoosterPrefab;

    [SerializeField]
    private int pointCount;

    [SerializeField]
    private float radius, chainConnectorWidth, chainReactionInitialDelay, iterationDelayCoefficient;

    private GameObject[] points, connectors;

    private bool reactionBegan = false;

    private void Start()
    {
        if (!makeCircle)
        {
            points = new GameObject[transform.childCount];


            for (int i = 0; i < points.Length; i++)
            {
                points[i] = transform.GetChild(i).gameObject;
            }

        } else
        {
            points = new GameObject[pointCount];
            connectors = new GameObject[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                Vector3 pos = new Vector3(Mathf.Cos(2F * Mathf.PI * (i / (float)pointCount)) * radius,
                    0F,
                    Mathf.Sin(2F * Mathf.PI * (i / (float)pointCount)) * radius);

                points[i] = Instantiate(pointPrefab, transform.position + pos, Quaternion.identity);
                points[i].transform.parent = transform;
            }

            for (int i = 0; i < pointCount; i++)
            {
                var p0 = points[i].transform.position;
                var p1 = (i >= pointCount - 1) ? points[0].transform.position : points[i + 1].transform.position;

                var collider = Instantiate(chainColliderPrefab);
                collider.transform.position = Vector3.Lerp(p0, p1, 0.5F);

                collider.transform.forward = (p0 - p1).normalized;

                collider.transform.localScale = new Vector3(chainConnectorWidth, chainConnectorWidth, Vector3.Distance(p0, p1));

                collider.transform.parent = transform;

                collider.GetComponent<LineRenderer>().startWidth = chainConnectorWidth;

                connectors[i] = collider;
            }
        }
    }

    public void BeginReaction(GameObject initial)
    {
        if (reactionBegan)
            return;

        reactionBegan = true;

        if (points.Contains(initial))
        {
            StartCoroutine(Reaction(points.ToList().IndexOf(initial)));
        }
        else
        {
            int targetIndex = 0;

            float sqrMag = Mathf.Infinity;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] == null)
                    continue;

                var m = (points[i].transform.position - initial.transform.position).sqrMagnitude;
                if (m < sqrMag)
                {
                    targetIndex = i;
                    sqrMag = m;
                }
            }

            StartCoroutine(Reaction(targetIndex));
        }
    }

    private IEnumerator Reaction(int startIndex)
    {
        int upIndex = startIndex + 1;
        int downIndex = startIndex - 1;

        var initPos = points[startIndex].transform.position;

        Instantiate(pushOutBoosterPrefab, points[startIndex].transform.position, Quaternion.identity);

        Destroy(points[startIndex]);
        Destroy(connectors[startIndex]);

        float delay = chainReactionInitialDelay;

        yield return new WaitForSeconds(delay);

        delay *= iterationDelayCoefficient;

        do
        {
            bool bothFailed = true;

            if(upIndex >= points.Length)
            {
                upIndex = 0;
            }

            if (points[upIndex])
            {
                Destroy(points[upIndex]);
                Destroy(connectors[upIndex]);

                upIndex++;
                bothFailed = false;
            }

            if (downIndex < 0)
            {
                downIndex = points.Length - 1;    
            }

            if (points[downIndex])
            {
                Destroy(points[downIndex]);
                Destroy(connectors[downIndex]);

                downIndex--;
                bothFailed = false;
            }

            if (bothFailed)
                break;



            yield return new WaitForSeconds(delay);

            delay *= iterationDelayCoefficient;
        } while (true);

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public List<PathComponent> components = new List<PathComponent>();
    float timer = 0;
    Vector3 startPosition;

    void Start ()
    {
        startPosition = transform.position;
    }
	
	void Update ()
    {
        timer += Time.deltaTime;

        var currentOffset = Vector3.zero;

        foreach (var component in components)
        {
            var value = component.curve.Evaluate(timer * component.speed);
            currentOffset += component.direction * value;
        }

        transform.position = startPosition + currentOffset;
    }

    [System.Serializable]
    public class PathComponent
    {
        public AnimationCurve curve;
        public Vector3 direction;
        public float speed = 1;
    }
}

using System.Linq;
using Fusion;
using UnityEngine;

public class TrajectoryPrediction : NetworkBehaviour
{
    [SerializeField] private GameObject predictionPointPrefab;
    [SerializeField] private int amountOfPoints;
    [SerializeField] private float spaceBetweenPoints;
    [SerializeField] private float hideTime;

    private GameObject[] _predictionPoints;
    private bool Visible => _predictionPoints.All(x => x.activeSelf);

    [Networked] private TickTimer HideTimer { get; set; }

    public override void Spawned()
    {
        _predictionPoints = new GameObject[amountOfPoints];

        for (var i = 0; i < amountOfPoints; i++)
        {
            var point = Instantiate(predictionPointPrefab, transform.position, Quaternion.identity);
            point.transform.SetParent(transform);
            _predictionPoints[i] = point;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HideTimer.Expired(Runner) && Visible)
        {
            SetVisible(false);
        }
    }

    public void DisplayTrajectory(float stretch, float speed, Vector2 direction)
    {
        if (!Object.HasInputAuthority) return;

        HideTimer = TickTimer.None;

        if (!Visible)
        {
            SetVisible(true);
        }

        for (var i = 0; i < _predictionPoints.Length; i++)
        {
            _predictionPoints[i].transform.position = PointPosition(i * stretch * spaceBetweenPoints, stretch, speed, direction);
        }
    }

    private void SetVisible(bool visible)
    {
        foreach (var point in _predictionPoints)
        {
            point.SetActive(visible);
        }
    }

    public void HideWithDelay()
    {
        HideTimer = TickTimer.CreateFromSeconds(Runner, hideTime);
    }

    private Vector2 PointPosition(float time, float stretch, float speed, Vector2 direction)
    {
        var force = speed * stretch;
        return (Vector2)transform.position + (direction * force * time) + 0.5f * Physics2D.gravity * (time * time);
    }
}
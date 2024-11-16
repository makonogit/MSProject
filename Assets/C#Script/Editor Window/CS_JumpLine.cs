using System.Collections.Generic;
using UnityEngine;

public class CS_JumpLine :MonoBehaviour
{
    [SerializeField] private bool Show;
    [SerializeField] private Color PointColor;
    [SerializeField] private Color LineColor;
    [SerializeField] private float JumpPower = 5f;
    [SerializeField] private float speed = 5f;


    private void OnDrawGizmos()
    {
        if (Show) DrawJumpLine();
    }

    private void OnDrawGizmosSelected()
    {
        if (!Show) DrawJumpLine();
    }

    private void DrawJumpLine() 
    {
        List<Vector3>points = DrawDebris();
        Gizmos.color = LineColor;
        Gizmos.DrawLineStrip(points.ToArray(), false);
    }


    private List<Vector3> DrawDebris()
    {
        float deltaTime = 0.04167f;

        List<Vector3> Points = new List<Vector3>();

        // 初期位置設定
        Vector3 position = this.transform.position;
        Gizmos.color = PointColor;
        Gizmos.DrawWireSphere(position, 0.5f);
        Gizmos.DrawSphere(position, 0.5f);

        Vector3 jumpVec = Vector3.up * JumpPower;
        Vector3 moveVec = this.transform.forward * speed;

        Points.Add(position);
        // 初速度の設定
        Vector3 Velocity = (jumpVec+moveVec) * (deltaTime * deltaTime * 0.5f);
        RaycastHit hit = new RaycastHit();
        // ぶつかるまでの軌道の線を引く
        for (float time = 0.0f; time < 10; time += deltaTime)
        {
            Velocity += Vector3.down * (9.81f * deltaTime * deltaTime);
            Ray ray = new Ray(position, Velocity.normalized);
            bool IsHit = Physics.Raycast(ray, out hit, Velocity.magnitude);
            position += Velocity;
            if (IsHit)
            {
                Points.Add(hit.point);
                break;
            }
            Points.Add(position);
        }
        // 着地点表示
        Gizmos.DrawWireSphere(hit.point, 0.5f);
        Gizmos.DrawSphere(hit.point, 0.5f);

        return Points;
    }
}
using UnityEngine;
using UnityEngine.UIElements;

public class CS_Ruler : MonoBehaviour
{
    [SerializeField] private int length = 11;
    [SerializeField] private bool show = true;
    [Header("X軸の目盛り")]
    [SerializeField] private bool XY_Line = true;
    [SerializeField] private bool XZ_Line = true;
    [SerializeField] private Color XY = Color.red;
    [SerializeField] private Color XZ = Color.red;
    [Header("Y軸の目盛り")]
    [SerializeField] private bool YX_Line = true;
    [SerializeField] private bool YZ_Line = true;
    [SerializeField] private Color YX = Color.green;
    [SerializeField] private Color YZ = Color.green;
    [Header("Z軸の目盛り")]
    [SerializeField] private bool ZX_Line = true;
    [SerializeField] private bool ZY_Line = true;
    [SerializeField] private Color ZX = Color.blue;
    [SerializeField] private Color ZY = Color.blue;

    private void OnDrawGizmos()
    {
        if (show) DrawRuler();
    }

    private void OnDrawGizmosSelected()
    {
        if (!show) DrawRuler();
    }
    private void DrawRuler() 
    {
        int x = Mathf.FloorToInt(transform.position.x);
        int y = Mathf.FloorToInt(transform.position.y);
        int z = Mathf.FloorToInt(transform.position.z);

        Vector3 Position = new Vector3(x, y, z);
        // Y目盛り
        if (YX_Line) DrawLines(Position + (Vector3.right + Vector3.up) * length * 0.5f, Vector3.right, Vector3.down, Vector3.up, YX);
        if (YZ_Line) DrawLines(Position + (Vector3.up + Vector3.forward) * length * 0.5f, Vector3.forward, Vector3.down, Vector3.up, YZ);

        // X目盛り
        if (XY_Line) DrawLines(Position + (Vector3.right + Vector3.up) * length * 0.5f, Vector3.up, Vector3.left, Vector3.right, XY);
        if (XZ_Line) DrawLines(Position + (Vector3.right + Vector3.forward) * length * 0.5f, Vector3.forward, Vector3.left, Vector3.right, XZ);

        // Z目盛り
        if (ZX_Line) DrawLines(Position + (Vector3.right + Vector3.forward) * length * 0.5f, Vector3.right, Vector3.back, Vector3.forward, ZX);
        if (ZY_Line) DrawLines(Position + (Vector3.up + Vector3.forward) * length * 0.5f, Vector3.up, Vector3.back, Vector3.forward, ZY);

    }

    private void DrawLines(Vector3 Position, Vector3 a, Vector3 c, Vector3 b, Color color)
    {
        int len = Mathf.FloorToInt(length) - 1;
        Vector3 lengthVec = a * length * 0.5f;
        Color subColor = color;
        subColor.a *= 0.5f;
        Color subsubColor = subColor;
        subsubColor.a *= 0.5f;
        for (int i = 0; i < length; i++)
        {
            Vector3 line = c * (len * 0.5f - i);
            Line(Position + lengthVec + line, Position - lengthVec + line, b, 0.125f, color, subColor, subsubColor);
        }
    }

    private void Line(Vector3 from, Vector3 to, Vector3 zurasi, float offset, Color color, Color subColor, Color subsubColor)
    {
        for (int i = 0; i < 8; i++)
        {
            if (i == 0) Gizmos.color = color;
            else if (i == 4) Gizmos.color = subColor;
            else Gizmos.color = subsubColor;
            Gizmos.DrawLine(from + zurasi * (offset * i), to + zurasi * (offset * i));
        }
    }


}
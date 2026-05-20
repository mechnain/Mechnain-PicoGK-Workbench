using PicoGK;
using System.Numerics;

namespace Workbench.Generators.Pico;

public interface ISdfBody
{
    float Distance(in Vector3 p);
    BBox3 Bounds { get; }
}

public sealed class UnionImplicit : IBoundedImplicit
{
    readonly IReadOnlyList<ISdfBody> _bodies;

    public UnionImplicit(IReadOnlyList<ISdfBody> bodies)
    {
        _bodies = bodies;
    }

    public float fSignedDistance(in Vector3 p)
    {
        float distance = float.MaxValue;

        foreach (ISdfBody body in _bodies)
            distance = MathF.Min(distance, body.Distance(p));

        return distance;
    }

    public BBox3 oBounds => BoundsOf(_bodies);

    public static BBox3 BoundsOf(IEnumerable<ISdfBody> bodies)
    {
        Vector3 min = new(float.MaxValue);
        Vector3 max = new(float.MinValue);
        bool any = false;

        foreach (ISdfBody body in bodies)
        {
            min = Vector3.Min(min, body.Bounds.vecMin);
            max = Vector3.Max(max, body.Bounds.vecMax);
            any = true;
        }

        return any
            ? new BBox3(min, max)
            : new BBox3(new Vector3(-1), new Vector3(1));
    }
}

public sealed class DifferenceImplicit : IBoundedImplicit
{
    readonly ISdfBody _solid;
    readonly IReadOnlyList<ISdfBody> _cuts;

    public DifferenceImplicit(ISdfBody solid, IReadOnlyList<ISdfBody> cuts)
    {
        _solid = solid;
        _cuts = cuts;
    }

    public float fSignedDistance(in Vector3 p)
    {
        float distance = _solid.Distance(p);

        foreach (ISdfBody cut in _cuts)
            distance = MathF.Max(distance, -cut.Distance(p));

        return distance;
    }

    public BBox3 oBounds => _solid.Bounds;
}

public sealed class BoxBody : ISdfBody
{
    readonly Vector3 _center;
    readonly Vector3 _half;
    readonly float _radius;

    public BoxBody(Vector3 center, Vector3 size, float radius = 0f)
    {
        _center = center;
        _half = size * 0.5f;
        _radius = MathF.Max(0f, radius);
    }

    public float Distance(in Vector3 p)
    {
        Vector3 q = Abs(p - _center) - (_half - new Vector3(_radius));
        Vector3 outside = Vector3.Max(q, Vector3.Zero);
        float inside = MathF.Min(MathF.Max(q.X, MathF.Max(q.Y, q.Z)), 0f);
        return outside.Length() + inside - _radius;
    }

    public BBox3 Bounds => new(_center - _half - new Vector3(_radius + 1), _center + _half + new Vector3(_radius + 1));

    static Vector3 Abs(Vector3 v)
    {
        return new Vector3(MathF.Abs(v.X), MathF.Abs(v.Y), MathF.Abs(v.Z));
    }
}

public sealed class OrientedBoxBody : ISdfBody
{
    readonly Vector3 _center;
    readonly Vector3 _axisX;
    readonly Vector3 _axisY;
    readonly Vector3 _axisZ;
    readonly Vector3 _half;
    readonly float _radius;

    public OrientedBoxBody(Vector3 center, Vector3 axisX, Vector3 axisY, Vector3 axisZ, Vector3 size, float radius = 0f)
    {
        _center = center;
        _axisX = Vector3.Normalize(axisX);
        _axisY = Vector3.Normalize(axisY);
        _axisZ = Vector3.Normalize(axisZ);
        _half = size * 0.5f;
        _radius = radius;
    }

    public float Distance(in Vector3 p)
    {
        Vector3 local = p - _center;
        Vector3 q = new(
            MathF.Abs(Vector3.Dot(local, _axisX)),
            MathF.Abs(Vector3.Dot(local, _axisY)),
            MathF.Abs(Vector3.Dot(local, _axisZ)));

        q -= _half - new Vector3(_radius);
        Vector3 outside = Vector3.Max(q, Vector3.Zero);
        float inside = MathF.Min(MathF.Max(q.X, MathF.Max(q.Y, q.Z)), 0f);
        return outside.Length() + inside - _radius;
    }

    public BBox3 Bounds
    {
        get
        {
            float radius = _half.Length() + _radius + 2f;
            return new BBox3(_center - new Vector3(radius), _center + new Vector3(radius));
        }
    }
}

public sealed class SphereBody : ISdfBody
{
    readonly Vector3 _center;
    readonly float _radius;

    public SphereBody(Vector3 center, float radius)
    {
        _center = center;
        _radius = radius;
    }

    public float Distance(in Vector3 p) => (p - _center).Length() - _radius;

    public BBox3 Bounds => new(_center - new Vector3(_radius + 1), _center + new Vector3(_radius + 1));
}

public sealed class CylinderBody : ISdfBody
{
    readonly Vector3 _center;
    readonly Vector3 _axis;
    readonly float _radius;
    readonly float _height;

    public CylinderBody(Vector3 center, Vector3 axis, float radius, float height)
    {
        _center = center;
        _axis = Vector3.Normalize(axis);
        _radius = radius;
        _height = height;
    }

    public float Distance(in Vector3 p)
    {
        Vector3 q = p - _center;
        float axial = Vector3.Dot(q, _axis);
        Vector3 radialVector = q - _axis * axial;
        Vector2 d = new(radialVector.Length() - _radius, MathF.Abs(axial) - _height * 0.5f);
        Vector2 outside = new(MathF.Max(d.X, 0), MathF.Max(d.Y, 0));
        return outside.Length() + MathF.Min(MathF.Max(d.X, d.Y), 0f);
    }

    public BBox3 Bounds
    {
        get
        {
            float extent = MathF.Sqrt(_radius * _radius + (_height * 0.5f) * (_height * 0.5f)) + 2f;
            return new BBox3(_center - new Vector3(extent), _center + new Vector3(extent));
        }
    }
}

public sealed class CapsuleBody : ISdfBody
{
    readonly Vector3 _a;
    readonly Vector3 _b;
    readonly float _radius;

    public CapsuleBody(Vector3 a, Vector3 b, float radius)
    {
        _a = a;
        _b = b;
        _radius = radius;
    }

    public float Distance(in Vector3 p)
    {
        Vector3 pa = p - _a;
        Vector3 ba = _b - _a;
        float h = Math.Clamp(Vector3.Dot(pa, ba) / Vector3.Dot(ba, ba), 0f, 1f);
        return (pa - ba * h).Length() - _radius;
    }

    public BBox3 Bounds
    {
        get
        {
            Vector3 min = Vector3.Min(_a, _b) - new Vector3(_radius + 1);
            Vector3 max = Vector3.Max(_a, _b) + new Vector3(_radius + 1);
            return new BBox3(min, max);
        }
    }
}

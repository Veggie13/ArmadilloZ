using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsTest
{
    struct Point2
    {
        public float X, Y;

        public static Vector2 operator -(Point2 a, Point2 b)
        {
            return new Vector2() { X = a.X - b.X, Y = a.Y - b.Y };
        }

        public static Point2 operator +(Point2 a, Vector2 b)
        {
            return new Point2() { X = a.X + b.X, Y = a.Y + b.Y };
        }
    }

    struct Vector2
    {
        public float X, Y;

        public Vector2 Unit
        {
            get
            {
                float mag = Magnitude;
                if (mag == 0f) return new Vector2();
                return this * (1f / mag);
            }
        }
        public float Magnitude { get { return (float)Math.Sqrt(this * this); } }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2() { X = a.X - b.X, Y = a.Y - b.Y };
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2() { X = a.X + b.X, Y = a.Y + b.Y };
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2() { X = a.X * b, Y = a.Y * b };
        }

        public static float operator *(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
    }

    abstract class Region2
    {
        public abstract bool Contains(Point2 point);
    }

    class Polygon2 : Region2
    {
        public List<Point2> Points = new List<Point2>();

        public override bool Contains(Point2 point)
        {
            throw new NotImplementedException();
        }
    }

    class Circle2 : Region2
    {
        public Point2 Center;
        public float Radius;

        public override bool Contains(Point2 point)
        {
            return (point - Center).Magnitude < Radius;
        }
    }

    class Annulus2 : Region2
    {
        public Point2 Center;
        public float InnerRadius;
        public float OuterRadius;
        public float MinArc;
        public float MaxArc;

        public override bool Contains(Point2 point)
        {
            var radial = point - Center;
            float radius = radial.Magnitude;
            if (!(InnerRadius < radius && radius < OuterRadius)) return false;
            float bearing = (float)Math.Atan2(radial.Y, radial.X);
            double normBearing = (bearing - MinArc + Math.PI * 2) % (Math.PI * 2);
            double arc = (MaxArc - MinArc + Math.PI * 2) % (Math.PI * 2);
            return (normBearing < arc);
        }
    }

    abstract class WaveForceRegion
    {
        public float Wavelength;

        public abstract Region2 Region { get; }

        public abstract float GetAmplitude(Point2 pos);

        public Vector2 GetVelocity(Point2 pos)
        {
            if (!Region.Contains(pos))
            {
                return new Vector2();
            }

            return getVelocity(pos);
        }

        public Vector2 GetForce(Point2 pos, Vector2 velocity)
        {
            if (!Region.Contains(pos))
            {
                return new Vector2();
            }

            var waveVelocity = getVelocity(pos);
            float relativeSpeed = ((velocity - waveVelocity) * waveVelocity) * -1f;
            if (relativeSpeed < 0f)
            {
                return new Vector2();
            }

            return waveVelocity.Unit * relativeSpeed * GetAmplitude(pos);
        }

        protected abstract Vector2 getVelocity(Point2 pos);

        public abstract float GetWaveValue(Point2 pos);

        public abstract void Tick(float dt);
    }

    //class LinearWaveForceRegion : WaveForceRegion
    //{
    //    public float Amplitude;
    //    public Vector2 Velocity;

    //    public override float GetAmplitude(Point2 pos)
    //    {
    //        return Amplitude;
    //    }

    //    protected override Vector2 getVelocity(Point2 pos)
    //    {
    //        return Velocity;
    //    }
    //}

    class RadialWaveForceRegion : WaveForceRegion
    {
        public Annulus2 AnnularRegion;
        public Point2 Center;
        public float Speed;
        public float UnitAmplitude;

        public override Region2 Region
        {
            get
            {
                return AnnularRegion;
            }
        }

        protected override Vector2 getVelocity(Point2 pos)
        {
            var radial = pos - Center;
            return radial.Unit * Speed;
        }

        public override float GetAmplitude(Point2 pos)
        {
            var radial = pos - Center;
            float interp = (radial.Magnitude - AnnularRegion.InnerRadius) / (AnnularRegion.OuterRadius - AnnularRegion.InnerRadius);
            return UnitAmplitude * interp;
        }

        public override void Tick(float dt)
        {
            AnnularRegion.OuterRadius += Speed * dt;
            AnnularRegion.InnerRadius += Speed * dt;
        }

        public override float GetWaveValue(Point2 pos)
        {
            var radial = pos - Center;
            float interp = 1f - (radial.Magnitude - AnnularRegion.InnerRadius) / (AnnularRegion.OuterRadius - AnnularRegion.InnerRadius);
            float phase = interp / Wavelength;
            return GetAmplitude(pos) * (float)Math.Sin(phase * Math.PI * 2);
        }
    }

    class SceneObject
    {
        public Point2 Position;
        public Vector2 Velocity;
        public float DragCoefficient;
        public float StopSpeed;

        public void AcceptForce(Vector2 force, float dt)
        {
            Velocity += force * dt;
        }

        public void Tick(float dt)
        {
            Position += Velocity * dt;
            Velocity *= (float)Math.Pow(DragCoefficient, dt);
            if (Velocity.Magnitude < StopSpeed)
            {
                Velocity = new Vector2();
            }
        }
    }

    class Scene
    {
        public List<SceneObject> Objects = new List<SceneObject>();
        public List<WaveForceRegion> Regions = new List<WaveForceRegion>();

        public void Tick(float dt)
        {
            foreach (var obj in Objects)
            {
                foreach (var region in Regions)
                {
                    region.Tick(dt);
                    obj.AcceptForce(region.GetForce(obj.Position, obj.Velocity), dt);
                }

                obj.Tick(dt);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WaveField {
    const float amplitude = 0.1f;
    const float kmultiplier = 10f;

    public abstract class Region2
    {
        public abstract bool Contains(Vector2 point);
    }

    public class Everywhere2 : Region2
    {
        public static readonly Everywhere2 Instance = new Everywhere2();
        public override bool Contains(Vector2 point)
        {
            return true;
        }
    }

    //class Polygon2 : Region2
    //{
    //    public List<Vector2> Points = new List<Vector2>();

    //    public override bool Contains(Vector2 point)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //class Circle2 : Region2
    //{
    //    public Vector2 Center;
    //    public float Radius;

    //    public override bool Contains(Vector2 point)
    //    {
    //        return (point - Center).Magnitude < Radius;
    //    }
    //}

    public class Annulus2 : Region2
    {
        public Vector2 Center;
        public float InnerRadius;
        public float OuterRadius;
        public float MinArc;
        public float MaxArc;

        public override bool Contains(Vector2 point)
        {
            var radial = point - Center;
            float radius = radial.magnitude;
            if (!(InnerRadius < radius && radius < OuterRadius)) return false;
            float bearing = Mathf.Atan2(radial.y, radial.x);
            double normBearing = (bearing - MinArc + Mathf.PI * 4) % (Mathf.PI * 2);
            double arc = (MaxArc - MinArc + Mathf.PI * 4) % (Mathf.PI * 2);
            return (arc == 0f) || (normBearing < arc);
        }
    }

    public abstract class WaveForceRegion
    {
        public float Wavelength;

        public abstract Region2 Region { get; }

        public abstract float GetAmplitude(Vector2 pos);

        public Vector2 GetVelocity(Vector2 pos)
        {
            if (!Region.Contains(pos))
            {
                return new Vector2();
            }

            return getVelocity(pos);
        }

        public Vector2 GetForce(Vector2 pos, Vector2 velocity)
        {
            if (!Region.Contains(pos))
            {
                return new Vector2();
            }

            var waveVelocity = getVelocity(pos);
            float relativeSpeed = Vector2.Dot(velocity - waveVelocity, waveVelocity) * -1f;
            if (relativeSpeed < 0f)
            {
                return new Vector2();
            }

            return waveVelocity.normalized * relativeSpeed * GetAmplitude(pos);
        }

        protected abstract Vector2 getVelocity(Vector2 pos);

        public abstract float GetWaveValue(Vector2 pos);

        public abstract bool Tick(float dt);
    }

    //class LinearWaveForceRegion : WaveForceRegion
    //{
    //    public float Amplitude;
    //    public Vector2 Velocity;

    //    public override float GetAmplitude(Vector2 pos)
    //    {
    //        return Amplitude;
    //    }

    //    protected override Vector2 getVelocity(Vector2 pos)
    //    {
    //        return Velocity;
    //    }
    //}

    public class RadialWaveForceRegion : WaveForceRegion
    {
        public Annulus2 AnnularRegion;
        public Vector2 Center;
        public float Speed;
        public float UnitAmplitude;
        public float UnitWaveAmplitude;
        public float Attenuation;
        public float MaxRadius;

        public override Region2 Region
        {
            get
            {
                return AnnularRegion;
            }
        }

        protected override Vector2 getVelocity(Vector2 pos)
        {
            var radial = pos - Center;
            return radial.normalized * Speed;
        }

        public override float GetAmplitude(Vector2 pos)
        {
            if (!Region.Contains(pos))
            {
                return 0f;
            }
            var radial = pos - Center;
            float interp = (radial.magnitude - AnnularRegion.InnerRadius) / (AnnularRegion.OuterRadius - AnnularRegion.InnerRadius);
            return UnitAmplitude * interp;
        }

        public override bool Tick(float dt)
        {
            AnnularRegion.OuterRadius += Speed * dt;
            AnnularRegion.InnerRadius += Speed * dt;
            UnitAmplitude *= Mathf.Pow(Attenuation, dt);
            UnitWaveAmplitude *= Mathf.Pow(Attenuation, dt);
            return (AnnularRegion.OuterRadius < MaxRadius);
        }

        public override float GetWaveValue(Vector2 pos)
        {
            if (!Region.Contains(pos))
            {
                return 0f;
            }
            var radial = pos - Center;
            float interp = (radial.magnitude - AnnularRegion.InnerRadius) / (AnnularRegion.OuterRadius - AnnularRegion.InnerRadius);
            float phase = (1f - interp) / Wavelength;
            return UnitWaveAmplitude * interp * Mathf.Sin(phase * Mathf.PI * 2);
        }
    }

    public class BackgroundRegion : WaveForceRegion
    {
        public static readonly BackgroundRegion Instance = new BackgroundRegion();

        public override Region2 Region
        {
            get
            {
                return Everywhere2.Instance;
            }
        }

        public override float GetAmplitude(Vector2 pos)
        {
            return 0f;
        }

        protected override Vector2 getVelocity(Vector2 pos)
        {
            return Vector2.zero;
        }

        public override float GetWaveValue(Vector2 pos)
        {
            float shortRadius = 25f * Mathf.Sin(Mathf.PI / 3);
            return (1f - pos.SqrMagnitude() / shortRadius / shortRadius) + 0.05f - 0.1f * Mathf.PerlinNoise(pos.x, pos.y);
        }

        public override bool Tick(float dt)
        {
            return true;
        }
    }

    public class Movable
    {
        public Rigidbody Body;
        public Vector2 Position
        {
            get
            {
                return new Vector2(Body.position.x, Body.position.z);
            }
        }
        public Vector2 Velocity
        {
            get
            {
                return new Vector2(Body.velocity.x, Body.velocity.z);
            }
        }
        public float DragCoefficient;
        public float StopSpeed;

        private Vector2 curForce = new Vector2();

        public void AcceptForce(Vector2 force, float dt)
        {
            curForce += force;
        }

        public void Tick(float dt)
        {
            Body.AddForce(new Vector3(curForce.x, 0f, curForce.y), ForceMode.Acceleration);
            curForce = Vector2.zero;

            var planeVelocity = new Vector2(Body.velocity.x, Body.velocity.z);
            planeVelocity *= Mathf.Pow(DragCoefficient, dt);
            if (planeVelocity.magnitude < StopSpeed)
            {
                planeVelocity = Vector2.zero;
            }
            Body.velocity = new Vector3(planeVelocity.x, Body.velocity.y, planeVelocity.y);
        }
    }

    public class ForceManager
    {
        public List<Movable> Objects = new List<Movable>();
        public List<WaveForceRegion> Regions = new List<WaveForceRegion>()
        {
            BackgroundRegion.Instance
        };

        public void Tick(float dt)
        {
            foreach (var region in Regions.ToArray())
            {
                if (!region.Tick(dt)) Regions.Remove(region);
            }
            foreach (var obj in Objects)
            {
                foreach (var region in Regions)
                {
                    obj.AcceptForce(region.GetForce(obj.Position, obj.Velocity), dt);
                }

                obj.Tick(dt);
            }
        }
    }

    public static ForceManager forceMgr = new ForceManager();

    public static float GetAmplitude(float x, float z)
    {
        var pos = new Vector2(x, z);
        float y = forceMgr.Regions.Sum(r => r.GetWaveValue(pos));
        return y;
    }
}

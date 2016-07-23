using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace WFOpenTKTest
{
    class RenderObject
    {
        public IList<RenderObject> Children { get; private set; }
        public Vector3[] Verticies { get; private set; }
        public Color Color;
        public Vector3 Position;

        public RenderObject(Vector3 position)
        {
            Children = new List<RenderObject>();
            Position = position;
            Color = Color.Empty;
        }

        public RenderObject(Vector3 position, Color color, Vector3[] vectors) : this(position)
        {
            Color = color;
            Verticies = vectors;
        }

        public void Render()
        {
            Render(Vector3.Zero);
        }

        public void Render(Vector3 parentCoordinates)
        {
            Vector3 position = Position + parentCoordinates;
            if (Verticies != null)
            {
                GL.Color3(Color);
                switch (Verticies.Length)
                {
                    case 4:
                        GL.Begin(BeginMode.Quads);
                        break;
                    case 3:
                        GL.Begin(BeginMode.Triangles);
                        break;
                    case 2:
                    default:
                        GL.Begin(BeginMode.Lines);
                        break;
                }
                foreach (Vector3 vector in Verticies)
                {
                    Vector3 transformedVector = vector + position;
                    GL.Vertex3(
                        transformedVector.X,
                        transformedVector.Y,
                        transformedVector.Z);
                }
                GL.End();
            }

            foreach (RenderObject child in Children)
            {
                child.Render(position);
            }
        }

        public RenderObject FindHit(Vector3 start, Vector3 end)
        {
            return FindHit(start, end, Vector3.Zero);
        }

        public RenderObject FindHit(Vector3 start, Vector3 end, Vector3 parentCoordinates)
        {
            Vector3 position = Position + parentCoordinates;

            Dictionary<float, RenderObject> hitObjects = new Dictionary<float, RenderObject>();

            foreach (RenderObject child in Children)
            {
                float distance = child.FindHitDistance(start, end, child.Position + position);
                if (distance != 0)
                    hitObjects.Add(distance, child);
            }

            if (hitObjects.Keys.Count != 0)
            {
                float minDistance = 0;
                foreach (KeyValuePair<float, RenderObject> hitObject in hitObjects)
                {
                    if (minDistance == 0 || hitObject.Key < minDistance)
                        minDistance = hitObject.Key;
                }
                return hitObjects[minDistance];
            }

            foreach (RenderObject child in Children)
            {
                RenderObject hit = child.FindHit(start, end, position);
                if (hit != null)
                    return hit;
            }

            return null;
        }

        public float FindHitDistance(Vector3 start, Vector3 end, Vector3 position)
        {
            if (Verticies != null && Verticies.Length == 4)
            {
                Vector3 normal = Vector3.Normalize(Vector3.Cross(Verticies[1] - Verticies[0], Verticies[3] - Verticies[0]));

                float intersectionDegree = Vector3.Dot(normal, end - start);

                if (Math.Abs(intersectionDegree) > 0)
                {
                    float t = -Vector3.Dot(normal, start - (position + Verticies[0])) / intersectionDegree;
                    Vector3 M = start + (end - start) * t;

                    float u = Vector3.Dot(M - (position + Verticies[0]), Vector3.Normalize(Verticies[1] - Verticies[0]));
                    float v = Vector3.Dot(M - (position + Verticies[0]), Vector3.Normalize(Verticies[3] - Verticies[0]));

                    if (u >= 0.0f && u <= (Verticies[1] - Verticies[0]).Length &&
                        v >= 0.0f && v <= (Verticies[3] - Verticies[0]).Length)
                        return t;
                }
            }
            return 0;
        }
    }
}

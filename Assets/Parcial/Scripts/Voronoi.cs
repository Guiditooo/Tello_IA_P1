using System;
using System.Collections.Generic;
using System.Linq;

namespace FlyEngine
{
    public class Voronoi
    {
        private List<Vector2> nodeList; //Reemplazar por node para hacerlo por pesos

        public Voronoi()
        {
            nodeList = new List<Vector2>();
        }

        public void AddNode(Vector2 v)
        {
            nodeList.Add(v);
        }

        public void AddNodes(List<Vector2> nodes)
        {
            nodeList.AddRange(nodes);
        }

        public Vector2 GetMostNearbyNode(Vector2 node) //Reemplazar por voronoi
        {
            if (nodeList == null || nodeList.Count == 0)
                throw new ArgumentException("La lista de nodeList no puede estar vacía.");

            Vector2 nearby = nodeList.OrderBy(n => Vector2.Distance(n, node)).First();

            return nearby;
        }
    }

}
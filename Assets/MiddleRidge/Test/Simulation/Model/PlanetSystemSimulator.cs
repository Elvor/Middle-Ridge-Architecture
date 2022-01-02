using System.Linq;
using UnityEngine;

namespace MiddleRidge.Test.Simulation.Model
{
    public class PlanetSystemSimulator
    {
        private readonly CelestialBody[] bodies;
        private readonly float g;

        internal PlanetSystemSimulator(CelestialBody[] bodies, float g)
        {
            this.bodies = bodies;
            this.g = g;
        }

        public Vector3[] Update(float deltaTime)
        {
            var accelerations = bodies.Select(targetBody =>
            {
                return bodies.Aggregate(Vector3.zero, (sum, sourceBody) =>
                {
                    if (sourceBody == targetBody)
                    {
                        return sum;
                    }

                    var vector = sourceBody.Position - targetBody.Position;
                    var distance = vector.magnitude;
                    return sum + g * sourceBody.Mass * vector / (distance * distance * distance);
                });
            }).ToArray();
            var result = new Vector3[bodies.Length];
            for (var i = 0; i < bodies.Length; i++)
            {
                result[i] = bodies[i].Simulate(deltaTime, accelerations[i]);
            }

            return result;
        }
    }
}
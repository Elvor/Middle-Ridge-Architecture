using System.Collections.Generic;
using UnityEngine;

namespace MiddleRidge.Test.Simulation.Model
{
    public class PlanetSystemSimulationBuilder
    {
        private List<CelestialBody> celestialBodies = new List<CelestialBody>();

        public PlanetSystemSimulationBuilder AddBody(float mass, Vector3 initialPosition, Vector3 initialVelocity)
        {
            var body = new CelestialBody(initialPosition, initialVelocity, mass);
            celestialBodies.Add(body);
            return this;
        }

        public PlanetSystemSimulator Build()
        {
            return new PlanetSystemSimulator(celestialBodies.ToArray(),0.05f);
        }
    }
}
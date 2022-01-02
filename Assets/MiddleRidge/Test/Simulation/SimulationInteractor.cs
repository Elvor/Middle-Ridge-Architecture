using MiddleRidge.DI.Lifecycle;
using MiddleRidge.MVP;
using MiddleRidge.Test.Simulation.Model;
using UnityEngine;

namespace MiddleRidge.Test.Simulation
{
    [TestComponent]
    public class SimulationInteractor : StatefulInteractor, IUpdatable
    {
        private GameObject[] bodies;
        private PlanetSystemSimulator planetSystemSimulator;
        public override void OnCreate()
        {
            base.OnCreate();
            bodies = new GameObject[]{CreatePlanet(), CreatePlanet()};

            planetSystemSimulator = new PlanetSystemSimulationBuilder()
                .AddBody(100, Vector3.zero, Vector3.zero)
                .AddBody(1, new Vector3(0, 3, 0), Vector3.left)
                .Build();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var gameObject in bodies)
            {
                Object.Destroy(gameObject);
            }

            planetSystemSimulator = null;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            foreach (var body in bodies)
            {
                body.SetActive(true);
            }
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            foreach (var body in bodies)
            {
                body.SetActive(false);
            }
        }

        public void Update(float deltaTime)
        {
            var newPos = planetSystemSimulator.Update(deltaTime);
            for (var i = 0; i < newPos.Length; i++)
            {
                bodies[i].transform.position = newPos[i];
            }
        }

        private GameObject CreatePlanet()
        {
            var planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            planet.SetActive(false);
            return planet;
        }
    }
}
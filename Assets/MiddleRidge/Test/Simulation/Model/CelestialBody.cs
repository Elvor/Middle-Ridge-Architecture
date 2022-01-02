using UnityEngine;

namespace MiddleRidge.Test.Simulation.Model
{
    public class CelestialBody
    {
        private Vector3 velocity;
        
        public CelestialBody(Vector3 initialPosition, Vector3 initialVelocity, float mass)
        {
            Position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
            velocity = new Vector3(initialVelocity.x, initialVelocity.y, initialVelocity.z);
            Mass = mass;
        }

        public float Mass { get; }
        public Vector3 Position { get; private set; }

        public Vector3 Simulate(float delataTime, Vector3 appliedA)
        {
            var x = Position.x;
            var vx = velocity.x;
            var nx = x + delataTime * vx + appliedA.x  * delataTime * delataTime / 2;
            var nvx = vx + delataTime * appliedA.x;
            
            var y = Position.y;
            var vy = velocity.y;
            var ny = y + delataTime * vy + appliedA.y  * delataTime * delataTime / 2;
            var nvy = vy + delataTime * appliedA.y;
            
            var z = Position.x;
            var vz = velocity.x;
            var nz = z + delataTime * vz + appliedA.z  * delataTime * delataTime / 2;
            var nvz = vz + delataTime * appliedA.z;
            
            Position = new Vector3(nx, ny, nz);
            velocity = new Vector3(nvx, nvy, nvz);
            return Position;
        }
    }
}
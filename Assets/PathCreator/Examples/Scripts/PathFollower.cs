using System;
using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        float _distanceTravelled;
        private Camera _camera;
        bool _moveCar, _reverse;

        void Start() {

            _camera = Camera.main;

            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            SelectTheCar();
            MoveTheCar(_moveCar);
        }

       
        void SelectTheCar()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.collider.CompareTag("Car"))
                    {
                        if (hit.collider.gameObject == gameObject)
                        {
                            _moveCar = true;
                            print("Click");
                        }
                    }
                }
            }
        }

        void MoveTheCar(bool move)
        {
            if (pathCreator != null && move)
            {
                float distance = pathCreator.path.GetClosestDistanceAlongPath(transform.position);

                if (!_reverse)
                {
                    _distanceTravelled += speed * Time.deltaTime;
                }
               
                else
                    _distanceTravelled -= speed * Time.deltaTime;


                transform.position = pathCreator.path.GetPointAtDistance(_distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(_distanceTravelled, endOfPathInstruction);

                if(distance >= pathCreator.path.length && !_reverse)  // car move the last Distination
                {
                    _moveCar = false;
                    _reverse = true;
                }

                if(distance <=0 && _reverse)   // car Initial Position
                {
                    _moveCar = false;
                    _reverse = false;
                }
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            _distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}
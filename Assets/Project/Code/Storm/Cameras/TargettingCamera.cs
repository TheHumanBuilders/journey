﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;
using Storm.TransitionSystem;

namespace Storm.Cameras {
    public class TargettingCamera : MonoBehaviour
    {

        private static string virtualCameraName;

        public static Transform target;
        public static PlayerCharacter player;
        public float targetSmoothing;
        public float playerSmoothing;

        public float sizeSmoothing;
        public Vector3 velocity;

        public Vector3 targetOffset;
        public Vector3 leftOffset;
        public Vector3 rightOffset;
        public static bool isCentered;

        private static Camera targetSettings;

        private static Camera defaultSettings;

        void Start() {
            Debug.Log("Camera Startup");
            defaultSettings = GetComponent<Camera>();
            if (player == null) {
                player = FindObjectOfType<PlayerCharacter>();
                if (player == null) return;
            }

            if (virtualCameraName != null && virtualCameraName != "") {

                // Try to find the correct virtual camera view and have the Trailing Camera snap to it.
                foreach (VirtualCamera cam in FindObjectsOfType<VirtualCamera>()) {
                    if (cam.name == virtualCameraName) {
                        // Make this virtual camera the active camera view.
                        cam.Activate();
                        SnapToTarget();
                        return;
                    }
                }

                throw new UnityException("Could not find Virtual Camera \"" + virtualCameraName + "\" in the current scene.");
            } else {
                ClearTarget();
                SnapToSpawn();
            }
        }

        void FixedUpdate() {
            if (target == null) {
                return;
            }

            if (player == null) {
                player = FindObjectOfType<PlayerCharacter>();
                if (player == null) return;
            }

            
            float futureSize = targetSettings.orthographicSize;
            float smoothing = (target == player.transform) ? playerSmoothing : targetSmoothing;
            Vector3 futurePos = getFuturePosition();
            
            // interpolate camera zoom
            Camera.main.orthographicSize = interpolate(Camera.main.orthographicSize, targetSettings.orthographicSize, sizeSmoothing);
            
            // interpolate camera position
            transform.position = Vector3.SmoothDamp(transform.position, futurePos, ref velocity, smoothing);
        }


        private Vector3 getFuturePosition() {
            Vector3 pos;

            // if following the player
            if (target == player.transform) {
                pos = player.transform.position;

                // choose appropriate camera offset.
                if (isCentered) {
                    pos += targetOffset;
                } else if (player.normalMovement.isFacingRight) {
                    pos += rightOffset;
                } else if (!player.normalMovement.isFacingRight) {
                    pos += leftOffset;
                }
                
            // moving to a predefined location
            } else {
                pos = target.transform.position + targetOffset;
            }

            return pos;
        }


        private float interpolate(float x, float y, float a) {
            return x*a + y*(1-a);
        }


        public void SetPlayerSmoothing(float smoothing) {
            playerSmoothing = smoothing;
        }

        public void SetTargetSmoothing(float smoothing) {
            targetSmoothing = smoothing;
        }

        public static void SetTarget(string vcamName) {
            virtualCameraName = vcamName;
        }

        public static void SetTarget(Camera cameraSettings) {
            //Debug.Log("Setting Target!");
            targetSettings = cameraSettings;
            isCentered = true;
            target = cameraSettings.transform;
        }

        public static void ClearTarget() {
            //Debug.Log("Clearing Target!");
            targetSettings = defaultSettings;
            target = player.transform;
            isCentered = false;
        }

        public void SnapToTarget() {
            transform.position = target.position;
            if (target == player.transform) {
                if (player.normalMovement.isFacingRight) {
                    transform.position += rightOffset;
                } else {
                    transform.position += leftOffset;
                }
            } else if (isCentered) {
                transform.position += targetOffset;
            }
        }

        public void SnapToSpawn() {
            Vector3 pos = TransitionManager.Instance.GetCurrentSpawnPosition();
            bool isFacingRight = TransitionManager.Instance.GetCurrentSpawnFacing();

            Debug.Log("Facing " + (isFacingRight ? "Right!" : "Left!"));

            transform.position = pos + (isFacingRight ? rightOffset : leftOffset);
        }

        public void SetCentered(bool centered) {
            isCentered = centered;
        }
    }

}

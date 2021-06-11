using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using System;

namespace EsnyaFactory.InariUdon.Player
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    class PlayerPositionRecovery : UdonSharpBehaviour
    {
        public float maxDistanceFromOrigin = 100;
        public float timeoutSeconds = 60*10;
        public int bufferSize = 128;
        public UI.UdonLogger logger;

        private string[] displayNames;
        private Vector3[] positions;
        private Quaternion[] rotations;
        private float[] times;
        private int currentndex = 0;

        [UdonSynced] private int[] targetPlayerIds = {};
        [UdonSynced] private Vector3[] targetPositions = {};
        [UdonSynced] private Quaternion[] targetRotations = {};
        private float[] targetAddedTimes = {};

        private bool dirty = false;

        private void Start()
        {
            displayNames = new string[bufferSize];
            positions = new Vector3[bufferSize];
            rotations = new Quaternion[bufferSize];
            times = new float[bufferSize];

            Log("Info", "Initialized");
        }

        private void Update()
        {
            if (dirty)
            {
                Log("Info", $"Requesting serialization (Length: {targetPlayerIds.Length})");
                RequestSerialization();
                dirty = false;
            }
        }

        private int FindIndex(string displayName)
        {
            for (int i = 0; i < bufferSize; i++)
            {
                if (displayNames[currentndex % bufferSize] == displayName) return i;
            }
            return -1;
        }

        private int Append(string displayName)
        {
            var i = currentndex % bufferSize;
            currentndex++;
            displayNames[i] = displayName;
            return i;
        }

        private int FindTimeoutedIndex()
        {
            for (int i = 0; i < targetAddedTimes.Length; i++)
            {
                if (targetAddedTimes[i] - Time.time < timeoutSeconds) return i;
            }
            return -1;
        }

        private void AddTelepoprtTarget(int playerId, Vector3 position, Quaternion rotation)
        {
            var index = FindTimeoutedIndex();
            if (index < 0)
            {
                var length = targetAddedTimes.Length + 1;

                var newPlayerIds = new int[length];
                Array.Copy(targetPlayerIds, newPlayerIds, length - 1);
                targetPlayerIds = newPlayerIds;

                var newPositions = new Vector3[length];
                Array.Copy(targetPositions, newPositions, length - 1);
                targetPositions = newPositions;

                var newRotations = new Quaternion[length];
                Array.Copy(targetRotations, newRotations, length - 1);
                targetRotations = newRotations;

                var newAddedTimes = new float[length];
                Array.Copy(targetAddedTimes, newAddedTimes, length - 1);
                targetAddedTimes = newAddedTimes;

                index = length - 1;
            }

            targetPlayerIds[index] = playerId;
            targetPositions[index] = position;
            targetRotations[index] = rotation;
            targetAddedTimes[index] = Time.time;

            dirty = true;
        }

        public override void OnDeserialization()
        {
            var localPlayer = Networking.LocalPlayer;
            for (int i = 0; i < targetPlayerIds.Length; i++)
            {
                if (targetPlayerIds[i] == localPlayer.playerId)
                {
                    Log("Info", $"Recovery position received");
                    Log("Info", $"Teleporting to {targetPositions[i]}");
                    localPlayer.TeleportTo(targetPositions[i], targetRotations[i]);
                    break;
                }
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            var index = FindIndex(player.displayName);
            if (index < 0)
            {
                Append(player.displayName);
            }
            else
            {
                var isOwner = Networking.IsOwner(gameObject);
                var isLive = Time.time - times[index] < timeoutSeconds;
                Log("Info", $"Player re-joining detected (IsOwner: {isOwner}, Timeout: {!isLive}");
                if (isOwner && isLive)
                {
                    Log("Info", $"Requesting to teleport player {player.displayName} to: {positions[index]}");
                    AddTelepoprtTarget(player.playerId, positions[index], rotations[index]);
                }
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            var index = FindIndex(player.displayName);
            if (index < 0) index = Append(player.displayName);
            times[index] = Time.time;
            positions[index] = player.GetPosition();
            rotations[index] = player.GetRotation();
            Log("Info", $"Player {player.displayName} left at: {positions[index]}");
        }

        #region UdonLogger
        private void Log(string level, string log)
        {
            if (logger == null) Debug.Log($"{level} {log}");
            else logger.Log(level, gameObject.name, log);
        }
        #endregion
    }
}

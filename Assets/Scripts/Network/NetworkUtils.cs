using System;
using System.IO;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

namespace Kraken.Network
{
    public class NetworkUtils
    {
        public const string PhotonPrefabsPath = "PhotonPrefabs";
        public const int MAX_LOBBY_SIZE = 4;
        public const int MAX_NICKNAME_LENGTH = 10;

        static public GameObject Instantiate(string objectPath)
        {
            return PhotonNetwork.Instantiate(Path.Combine(PhotonPrefabsPath, objectPath), Vector3.zero, Quaternion.identity);
        }

        static public GameObject Instantiate(string objectPath, Vector3 position)
        {
            //Debug.Log(Path.Combine(PhotonPrefabsPath, objectPath));
            return PhotonNetwork.Instantiate(Path.Combine(PhotonPrefabsPath, objectPath), position, Quaternion.identity);
        }
        
        static public GameObject Instantiate(string objectPath, Vector3 position, Quaternion rotation)
        {
            return PhotonNetwork.Instantiate(Path.Combine(PhotonPrefabsPath, objectPath), position, rotation);
        }
        
        /**
         * Create a random room code of the pattern "a1a1"
         */
        static public string CreateRandomRoomCode()
        {
            string alphabet = "abcdejfhijklmnopqrstuvwxyz".ToUpper();
            string digit = "0123456789";

            return $"{GetRandomCharacterFrom(alphabet)}{GetRandomCharacterFrom(digit)}{GetRandomCharacterFrom(alphabet)}{GetRandomCharacterFrom(digit)}";
        }

        /**
         * Temporary nickname creation
         */
        static public string CreateRandomNickname()
        {
            string alphabet = "abcdejfhijklmnopqrstuvwxyz";
            string name = String.Empty;
            for (int i = 0; i < 5; i++)
            {
                name += GetRandomCharacterFrom(alphabet);
            }
            return name;
        }

        /**
         * Returns a random character from a string
         */
        static private char GetRandomCharacterFrom(string aString)
        {
            return aString[Random.Range(0, aString.Length)];
        }

        static public Player GetPlayerWithUserId(string playerUserId)
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.UserId == playerUserId) { return player; }
            }

            return null;
        }
    }
}
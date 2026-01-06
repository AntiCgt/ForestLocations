using BepInEx;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForestLocations
{
    [BepInPlugin("AC.ForestLocations", "Forest Locations", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        struct LocationMarker
        {
            public Vector3 Position;
            public string Text;
        }
        private List<GameObject> textObjects = new List<GameObject>();
        private bool markersCreated = false;

        private LocationMarker[] locations = new LocationMarker[]
        {
            new LocationMarker { Position = new Vector3(-62.33f, 9f, -62.53f), Text = "GAZEBO" },
            new LocationMarker { Position = new Vector3(-65.7f, 23.3f, -83f), Text = "STUMP" },
            new LocationMarker { Position = new Vector3(-81f, 14f, -83.5f), Text = "STUMP WALL" },
            new LocationMarker { Position = new Vector3(-33f, 23.3f, -80f), Text = "PIG WALL" },
            new LocationMarker { Position = new Vector3(-25f, 23.3f, -49f), Text = "DEATH WALL" },
            new LocationMarker { Position = new Vector3(-48f, 26f, -31.5f), Text = "BLACK WALL" },
            new LocationMarker { Position = new Vector3(-84.5f, 26f, -49f), Text = "TALL WALL" },
            new LocationMarker { Position = new Vector3(-73.9f, 40f, -47.35f), Text = "TALLEST" },
            new LocationMarker { Position = new Vector3(-72f, 21.5f, -54.5f), Text = "DOUBLE WALLS" },
            new LocationMarker { Position = new Vector3(-70f, 22.5f, -61f), Text = "TALL PLAT" },
            new LocationMarker { Position = new Vector3(-61f, 18f, -42.8f), Text = "TREEHOUSE" },
            new LocationMarker { Position = new Vector3(-60.7f, 27.7f, -50f), Text = "SLIDE TREE" },
            new LocationMarker { Position = new Vector3(-65.5f, 3.7f, -71.7f), Text = "GROUND" },
            new LocationMarker { Position = new Vector3(-54.1f, 5.5f, -55.4f), Text = "SLIDE" },
            new LocationMarker { Position = new Vector3(-50.7f, 28.7f, -38.6f), Text = "BAGEL" },
            new LocationMarker { Position = new Vector3(-41.5f, 23.9f, -41.2f), Text = "APOLLO" },
            new LocationMarker { Position = new Vector3(-42.75f, 25.5f, -49.3f), Text = "SKY TREE" },
            new LocationMarker { Position = new Vector3(-33.8f, 17.2f, -49.6f), Text = "SMALL TREE" },
            new LocationMarker { Position = new Vector3(-34f, 31.4f, -58.7f), Text = "2nd TALLEST" },
            new LocationMarker { Position = new Vector3(-37.2f, 16f, -65.8f), Text = "PLATFORMS" },
            new LocationMarker { Position = new Vector3(-39f, 7f, -72.2f), Text = "BLENDER" }
        };

        void Awake()
        {
            Harmony harmony = new Harmony("AC.ForestLocations");
            harmony.PatchAll();
        }

        void Start()
        {
            // Запускаем корутину для ожидания инициализации
            StartCoroutine(InitializeAfterDelay());
        }

        IEnumerator InitializeAfterDelay()
        {
            // Ждем пока игра полностью загрузится
            yield return new WaitForSeconds(2f);

            // Ждем пока VRRig станет доступен
            while (VRRig.LocalRig == null)
            {
                yield return new WaitForSeconds(0.5f);
            }

            // Создаем маркеры
            CreateMarkers();
            markersCreated = true;
        }

        void CreateMarkers()
        {
            for (int i = 0; i < locations.Length; i++)
            {
                GameObject tmp = CreateMarker(locations[i]);
                textObjects.Add(tmp);
            }
        }

        GameObject CreateMarker(LocationMarker marker)
        {
            GameObject go = new GameObject($"Marker_{marker.Text}");
            go.transform.position = marker.Position;

            // Добавляем Canvas для лучшего рендеринга
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            // Создаем дочерний объект для текста
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(go.transform);
            textObj.transform.localPosition = Vector3.zero;
            textObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = marker.Text;
            tmp.fontSize = 24f;
            tmp.color = new Color(1f, 1f, 1f, 0.3f);
            tmp.alignment = TextAlignmentOptions.Center;

            // Добавляем обводку для лучшей видимости
            tmp.fontMaterial.EnableKeyword("OUTLINE_ON");
            tmp.outlineWidth = 0.15f;
            tmp.outlineColor = Color.black;

            return go;
        }

        void Update()
        {
            if (!markersCreated || VRRig.LocalRig == null)
                return;

            Vector3 playerPosition = VRRig.LocalRig.transform.position;

            foreach (GameObject text in textObjects)
            {
                if (text != null)
                {
                    // Поворачиваем текст к игроку
                    text.transform.LookAt(playerPosition);
                    text.transform.Rotate(0, 180, 0);
                }
            }
        }
    }
}

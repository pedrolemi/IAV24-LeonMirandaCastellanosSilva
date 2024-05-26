using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline;
using UnityEngine;


namespace IAV24.Final
{
    public class LevelManager : MonoBehaviour
    {
        // Ciclo de dia
        private int days;
        [SerializeField]
        private float initTime = 6.0f;

        private float prevTime = 0.0f,
                      hoursPassed = 0.0f,
                      hour = 0.0f;

        [SerializeField]
        private float speedFactor = 2.0f;

        [SerializeField]
        private Gradient lightGradient;

        private Light lightSource;
        private Transform lightTr;

        [SerializeField]
        private TextMeshProUGUI dayText;


        // Enemigos
        private Transform[] spawnpoints = null;
        
        [SerializeField]
        private GameObject[] enemies = null;

        private Transform enemiesGroup;

        private float spawnTimer;

        [SerializeField]
        private float spawnStartHour = 20.0f;
        [SerializeField]
        private float spawnEndHour = 5.0f;


        [SerializeField]
        private float minSpawnDelay = 2.0f;
        [SerializeField]
        private float maxSpawnDelay = 5.0f;
        [SerializeField]
        private int maxEnemies = 10;

        private float spawnCooldown;

        [SerializeField]
        private float spawnOffset = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            prevTime = hoursPassed = hour = initTime / speedFactor;

            days = 1;

            lightSource = GameObject.Find("Light").GetComponent<Light>();
            lightTr = lightSource.transform;

            dayText = GameObject.Find("DayText").GetComponent<TextMeshProUGUI>();
            dayText.text = "Day " + days;

            changeLight(hour / 24.0f);

            enemiesGroup = GameObject.Find("Enemies").transform;
            GameObject sp = GameObject.Find("Spawnpoints");
            spawnpoints = new Transform[sp.transform.childCount];
            for (int i = 0; i < sp.transform.childCount; i++)
            {
                spawnpoints[i] = sp.transform.GetChild(i);
            }
            spawnCooldown = Random.Range(minSpawnDelay, maxSpawnDelay);
        }

        // Update is called once per frame
        void Update()
        {
            // Actualiza la hora y las horas que han
            // pasado desde el inicio de la simulacion
            hour += Time.deltaTime * speedFactor;
            hoursPassed += Time.deltaTime * speedFactor;

            // El tiempo que ha pasado desde el ultimo guardado
            // supera las 24 horas, ha pasado un dia completo
            if ((hoursPassed - prevTime) * speedFactor >= 24)
            {
                // Se actualiza el dia y el texto
                days++;
                dayText.text = "Day " + days;

                // Se reinician las horas
                hoursPassed = prevTime = initTime;
            }
            hour %= (24 / speedFactor);
            changeLight(hour / (24.0f / speedFactor));
            //Debug.Log(hour * speedFactor);

            // Si la hora se encuentra entre las horas en las que pueden spawnear enemigos
            if (hour * speedFactor > spawnStartHour || hour * speedFactor < spawnEndHour)
            {
                //Debug.Log("Spawneable");

                // Se va actualizando el contador para spawnear enemigos
                spawnTimer += Time.deltaTime;

                // Si se ha pasado el cooldown de spawneo y no se supera el maximo
                // numero de enemigos, spawnea un enemigo, cambia el cooldown de 
                // spawneo, y reinicia el contador
                if (spawnTimer > spawnCooldown && enemiesGroup.childCount < maxEnemies)
                {
                    spawnEnemy();
                    spawnCooldown = Random.Range(minSpawnDelay, maxSpawnDelay);
                    spawnTimer = 0;
                }
            }
        }

        // Cambia el color de la luz dependiendo de la hora del dia.
        // El dia va progresando desde las 00 hasta las 23:59. A las
        // 00 el dia estara un 0% completo y a las 23:59 al 99% (aprox).
        // El color de la luz es % del degradado correspondiente al %
        // de que tan completo esta el dia
        private void changeLight(float timePercent)
        {
            lightSource.color = lightGradient.Evaluate(timePercent);
            lightTr.rotation = Quaternion.Euler(new Vector3((timePercent * 360.0f) - 90.0f, -90.0f, 0.0f));
        }

        // Instancia un enemigo aleatorio en un punto aleatorio de los
        // colocados en el mapa y lo mete al grupo de enemigos
        private void spawnEnemy()
        {
            Transform tr = spawnpoints[Random.Range(0, spawnpoints.Length)];
            GameObject prefab = enemies[Random.Range(0, enemies.Length)];

            GameObject enemy = Instantiate(prefab, tr);
            enemy.transform.position += new Vector3(Random.Range(-spawnOffset, spawnOffset), 0, Random.Range(-spawnOffset, spawnOffset));
            enemy.transform.parent = enemiesGroup;
        }

        public int getDays() { return days; }


    }

}
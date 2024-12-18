using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        private List<EnemyHate> enemyHateList = new();

        private void Start()
        {
            EnemyHate[] enemyHate = GameObject.FindObjectsByType<EnemyHate>(FindObjectsSortMode.None);
            enemyHateList = new List<EnemyHate>(enemyHate);
        }

        private void Update()
        {
            if (enemyHateList.Count != 0)
            {
                var closestEnemy = enemyHateList[0];
                foreach (EnemyHate enemyHate in enemyHateList)
                {
                    if (Vector3.Distance(transform.position, enemyHate.transform.position) <
                        Vector3.Distance(transform.position, closestEnemy.transform.position))
                    {
                        closestEnemy = enemyHate;
                    }
                }

                var range = 10f;
                var speed = 10f;
                if (Vector3.Distance(transform.position, closestEnemy.transform.position) < range)
                {
                    transform.LookAt(closestEnemy.transform);
                    transform.position += transform.forward * (Time.deltaTime * speed);
                }
            }
        }
    }
}
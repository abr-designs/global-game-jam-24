using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Randall.Scripts.ScoringSystem
{
    public class PointsPopupManager : MonoBehaviour
    {

        [SerializeField] private GameObject pointsPopupPrefab;

        [SerializeField] private bool clearExistingPopups = true;

        [SerializeField] private float popupMinDistance = 50f;

        private Dictionary<Vector2Int, PointsPopup> _popupGridPoints = new Dictionary<Vector2Int, PointsPopup>();

        private void OnEnable()
        {
            GameScoreManager.OnPointsScored += GameScoreManager_OnPointsScored;
        }

        private void OnDisable()
        {

            GameScoreManager.OnPointsScored -= GameScoreManager_OnPointsScored;
        }

        private void GameScoreManager_OnPointsScored(object sender, ScorePointsArgs args)
        {

            if (clearExistingPopups)
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
            }

            GeneratePopup(args);

        }

        private void GeneratePopup(ScorePointsArgs args)
        {
            GameObject newPopup = Instantiate(pointsPopupPrefab, transform);
            PointsPopup pointsPopup = newPopup.GetComponent<PointsPopup>();

            pointsPopup.InitializeVisual(args);

            DistributePopup(pointsPopup);
        }

        // Move a popup so that it doesn't overlap previous
        private void DistributePopup(PointsPopup popup)
        {

            float gridSize = popupMinDistance;
            float randomOffset = gridSize * .25f;
            float xOffset = Screen.width / gridSize / 2f;
            float yOffset = Screen.height / gridSize / 2f;
            
            
            // Get current points popup coords
            Vector2Int popupGridPt = new Vector2Int(
                Mathf.FloorToInt((popup.transform.position.x + xOffset) / gridSize),
                Mathf.FloorToInt((popup.transform.position.y + yOffset) / gridSize)
            );

            Vector2Int newGridPt = popupGridPt;
            int i=1;
            _popupGridPoints.TryGetValue(popupGridPt, out PointsPopup pp);
            while(pp != null && i<=5)
            {
                for(int y=-i;y<=i;y++)
                {
                    for(int x=-i;x<=i;x++)
                    {
                        newGridPt = popupGridPt + new Vector2Int(x,y);
                        _popupGridPoints.TryGetValue(newGridPt, out pp);
                        if(pp == null) break;
                    }
                    if(pp == null) break;
                }
                i++;
            }

            // If popup was moved, add a small random movement to break up pattern)
            if(newGridPt != popupGridPt)
            {
                xOffset += UnityEngine.Random.Range(-randomOffset,randomOffset);
                yOffset += UnityEngine.Random.Range(-randomOffset,randomOffset);
                
                // Calculate new popup position 
                popup.transform.position = new Vector3(
                    newGridPt.x * gridSize - xOffset, 
                    newGridPt.y * gridSize - yOffset, 
                    0
                );
            }
            
            _popupGridPoints[newGridPt] = popup;
        }

        [ContextMenu("Test Popups")]
        private void TestPopups()
        {
            for(int i=0;i<20;i++)
            {
                ScorePointsArgs args = new ScorePointsArgs();
                args.worldPos = Vector3.zero;
                args.description = "Test";
                args.points = 100;
                GeneratePopup(args);
            }
        }
    }
}

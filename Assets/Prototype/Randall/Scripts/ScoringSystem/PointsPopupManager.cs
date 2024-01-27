using UnityEngine;

namespace Prototype.Randall.Scripts.ScoringSystem
{
    public class PointsPopupManager : MonoBehaviour
    {

        [SerializeField] private GameObject pointsPopupPrefab;

        [SerializeField] private bool clearExistingPopups = true;

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
        }
    }
}

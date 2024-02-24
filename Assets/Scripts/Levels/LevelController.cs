using System.Linq;
using UnityEngine;
using Utilities.ReadOnly;

namespace Levels
{
    public class LevelController : MonoBehaviour
    {
        public string levelName;
        [TextArea]
        public string levelDescription;
        public PropObjectSO[] avoidObjects;
        
        [Min(0), Space(10f)]
        public int levelTime;
        [Min(0)]
        public int minScoreToPass;


        //Unity Editor Functions
        //============================================================================================================//

#if UNITY_EDITOR
    
        [ReadOnly]
        public int maxScorePossibleInLevel;

        [ReadOnly]
        public int maxPenaltyScorePossibleInLevel;
        
        private void OnValidate()
        {
            GetMaxScores();
        }

        [ContextMenu("Update Max Score")]
        private void GetMaxScores()
        {
            var propObjects = gameObject.GetComponentsInChildren<PropObject>();

            var sum = 0;
            var penalty = 0;
            foreach (var propObject in propObjects)
            {
                var scriptableObject = propObject.propObjectSO;

                if(scriptableObject == null)
                    continue;
                
                if (avoidObjects != null && avoidObjects.Length > 0)
                {
                    var meantToAvoid = avoidObjects.Any(x => x.Equals(scriptableObject));
                
                    if (meantToAvoid)
                        penalty += scriptableObject.kingPenalty;
                    else
                        sum += scriptableObject.collideScore;

                    continue;
                }

                sum += scriptableObject.collideScore;
            }

            maxScorePossibleInLevel = sum;
            maxPenaltyScorePossibleInLevel = penalty;
        }
        
#endif
        //============================================================================================================//
    }
}
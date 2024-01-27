using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
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
        
        private void OnValidate()
        {
            maxScorePossibleInLevel = GetMaxScore();
        }

        [ContextMenu("Update Max Score")]
        private int GetMaxScore()
        {
            var propObjects = gameObject.GetComponentsInChildren<PropObject>();

            var sum = 0;
            foreach (var propObject in propObjects)
            {
                var scriptableObject = propObject.GetPropObjectSO();

                if (avoidObjects != null && avoidObjects.Length > 0)
                {
                    var meantToAvoid = avoidObjects.Any(x => x.Equals(scriptableObject));
                
                    if (meantToAvoid)
                        sum -= scriptableObject.kingPenalty;

                    continue;
                }

                sum += scriptableObject.collideScore;
            }

            return sum;
        }
        
#endif
        //============================================================================================================//
    }
}
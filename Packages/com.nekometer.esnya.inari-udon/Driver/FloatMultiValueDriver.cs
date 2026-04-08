#pragma warning disable IDE1006

using UdonSharp;
using UnityEngine;
using VRC.Udon;

namespace InariUdon.Driver
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class FloatMultiValueDriver : UdonSharpBehaviour
    {
        public float value;
        public bool castToInt = false;

        [Header("Write UdonBehaviour ProgramVariables")]
        public bool writeProgramVariables = false;
        public bool sendEvent = false;
        public bool ignoreFirstEvent = false;
        public bool findTargetFromChildren = false;
        public UdonSharpBehaviour[] behaviours = {};
        public string[] variableNames = {};
        public string[] eventNames = {};
        public Transform behaviourParent;
        public bool useFind;
        public string findPath;
        public string variableName;
        public string eventName;

        private bool isFirst = true;

        private UdonSharpBehaviour GetUdonSharpBehaviourFromChild(Transform parent, int index)
        {
            var child = parent.GetChild(index);
            var found = useFind ? child.Find(findPath) : child;
            if (found == null) return null;
            return (UdonSharpBehaviour)found.GetComponent(typeof(UdonBehaviour));
        }

        private void Start()
        {
            if (writeProgramVariables && findTargetFromChildren)
            {
                var targetCount = 0;
                for (int i = 0; i < behaviourParent.childCount; i++)
                {
                    var udon = GetUdonSharpBehaviourFromChild(behaviourParent, i);
                    if (udon != null) targetCount++;
                }

                behaviours = new UdonSharpBehaviour[targetCount];
                variableNames = new string[targetCount];
                eventNames = new string[targetCount];
                for (int i = 0; i < targetCount; i++)
                {
                    behaviours[i] =  GetUdonSharpBehaviourFromChild(behaviourParent, i);
                    variableNames[i] = variableName;
                    eventNames[i] = eventName;
                }
            }
        }

        private void WriteProgramVariables()
        {
            if (!writeProgramVariables) return;

            var variableLength = Mathf.Min(behaviours.Length, variableNames.Length);
            for (int i = 0; i < variableLength; i++)
            {
                var target = behaviours[i];
                if (target == null) continue;

                if (castToInt) target.SetProgramVariable(variableNames[i], (int)value);
                else target.SetProgramVariable(variableNames[i], value);
            }

            if (!sendEvent) return;

            if (ignoreFirstEvent && isFirst)
            {
                isFirst = false;
                return;
            }

            var eventLength = Mathf.Min(variableLength, eventNames.Length);
            for (int i = 0; i < eventLength; i++)
            {
                var target = behaviours[i];
                if (target == null) continue;
                target.SendCustomEvent(eventNames[i]);
            }
        }

        public void _ValueChanged()
        {
            WriteProgramVariables();
        }

        public void _SetValue(float value)
        {
            this.value = value;
            _ValueChanged();
        }

    }
}

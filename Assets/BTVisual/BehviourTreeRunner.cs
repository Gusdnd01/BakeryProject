using UnityEngine;
using BTVisual;

namespace BTVisual
{
    public class BehviourTreeRunner : MonoBehaviour
    {
        public BossBrain brain;
        public BehaviourTree tree;

        private void Start()
        {
            tree = tree.Clone(); //복제해서 시작함.
            var context = Context.CreateFromGameObject(gameObject);
            tree.Bind(context, brain); //만약 EnemyBrain과 같은 녀석을 여기서 바인드해서 넣어줘야 한다면 수정
        }

        public void Update()
        {
            if (brain.turnStatus == TurnStatus.Running)
            {
                if (tree.Update() != Node.State.RUNNING) brain.turnStatus = TurnStatus.End;
            }
        }
    }
}

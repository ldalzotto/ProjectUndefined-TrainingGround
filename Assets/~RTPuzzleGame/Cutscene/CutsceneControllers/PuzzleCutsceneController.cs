using UnityEngine;
using System.Collections;
using CoreGame;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class PuzzleCutsceneController : AbstractCutsceneController
    {
        public void Init(Rigidbody rigidBody, NavMeshAgent agent, Animator animator)
        {
            this.BaseInit(rigidBody, agent, animator);
        }
    }

}

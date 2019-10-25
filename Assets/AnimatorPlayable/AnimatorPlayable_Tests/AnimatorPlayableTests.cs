using System.Collections;
using System.Collections.Generic;
using AnimatorPlayable;
using CoreGame;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace AnimatorPlayable_Tests
{
    public class AnimatorPlayableTests
    {
        [TearDown]
        public void OnTearDown()
        {
            foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (obj.name != "Code-based tests runner")
                {
                    MonoBehaviour.Destroy(obj);
                }
            }
        }

        [UnityTest]
        public IEnumerator OneClip_NoTransition()
        {
            var AnimatorPlayableGameObject = new AnimatorPlayableGameObject();

            float clipTime = 0.25f;
            AnimatorPlayableGameObject.PlaySequencedAnimation(new SequencedAnimationInput()
            {
                isInfinite = false,
                BeginTransitionTime = 0f,
                layerID = 0,
                EndTransitionTime = 0f,
                UniqueAnimationClips = new List<UniqueAnimationClip>()
                {
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime)
                    }
                }
            });
            yield return null;
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying.Count == 1);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputWeight(0) == 1);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[0].GetType() == typeof(SequencedAnimationLayer));
            var SequencedAnimationLayer = AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[0] as SequencedAnimationLayer;
            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetTime() == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputCount() == 1);
            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 1f);
            yield return new WaitForSeconds(clipTime);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying.Count == 0);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputCount() == 0);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OneClip_NoTransition_Infinite()
        {
            var AnimatorPlayableGameObject = new AnimatorPlayableGameObject();

            float clipTime = 0.25f;
            AnimatorPlayableGameObject.PlaySequencedAnimation(new SequencedAnimationInput()
            {
                isInfinite = true,
                BeginTransitionTime = 0f,
                layerID = 0,
                EndTransitionTime = 0f,
                UniqueAnimationClips = new List<UniqueAnimationClip>()
                {
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime)
                    }
                }
            });
            yield return null;
            yield return new WaitForSeconds(clipTime);
            var SequencedAnimationLayer = AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[0] as SequencedAnimationLayer;
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying.Count == 1);
            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputCount() == 1);
            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 1f);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OneClip_StartTransition()
        {
            var AnimatorPlayableGameObject = new AnimatorPlayableGameObject();

            float beginTransitionTime = 0.0625f;
            float clipTime = 0.25f;
            AnimatorPlayableGameObject.PlaySequencedAnimation(new SequencedAnimationInput()
            {
                isInfinite = false,
                BeginTransitionTime = beginTransitionTime,
                layerID = 0,
                EndTransitionTime = 0f,
                UniqueAnimationClips = new List<UniqueAnimationClip>()
                {
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime)
                    }
                }
            });
            yield return null;
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputCount() == 1);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputWeight(0) == 0f);
            var SequencedAnimationLayer = AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[0] as SequencedAnimationLayer;

            // The first animation is weighter to 1 bu paused -> this allows the layer to transition to the first frame
            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetTime() == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Paused);
            ////* *////


            yield return new WaitForSeconds(beginTransitionTime * 0.5f);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputWeight(0) >= 0.5f &&
                          AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputWeight(0) < 0.9f);
            yield return new WaitWhile(() => AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputWeight(0) < 1f);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputWeight(0) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Playing);
            yield return new WaitForSeconds(clipTime);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying.Count == 0);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputCount() == 0);
        }

        [UnityTest]
        public IEnumerator OneClip_EndTransition()
        {
            var AnimatorPlayableGameObject = new AnimatorPlayableGameObject();

            float endTransitionTime = 0.0625f;
            float clipTime = 0.25f;
            AnimatorPlayableGameObject.PlaySequencedAnimation(new SequencedAnimationInput()
            {
                isInfinite = false,
                BeginTransitionTime = 0f,
                layerID = 0,
                EndTransitionTime = endTransitionTime,
                UniqueAnimationClips = new List<UniqueAnimationClip>()
                {
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime)
                    }
                }
            });

            yield return null;
            yield return new WaitForSeconds(clipTime);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying.Count == 1);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputCount() == 1);
            var SequencedAnimationLayer = AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[0] as SequencedAnimationLayer;
            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 1f);
            yield return null;
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputWeight(0) < 1f);
            yield return new WaitForSeconds(endTransitionTime);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying.Count == 0);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputCount() == 0);
        }

        [UnityTest]
        public IEnumerator ThreeClip_NoTransition()
        {
            var AnimatorPlayableGameObject = new AnimatorPlayableGameObject();

            float clipTime = 0.25f;
            AnimatorPlayableGameObject.PlaySequencedAnimation(new SequencedAnimationInput()
            {
                isInfinite = false,
                BeginTransitionTime = 0f,
                layerID = 0,
                EndTransitionTime = 0f,
                UniqueAnimationClips = new List<UniqueAnimationClip>()
                {
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime)
                    },
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime)
                    },
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime)
                    }
                }
            });
            yield return null;
            var SequencedAnimationLayer = AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[0] as SequencedAnimationLayer;
            //Only the first clip has weight
            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Paused);

            yield return new WaitForSeconds(clipTime);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Paused);

            yield return new WaitForSeconds(clipTime);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Playing);

            yield return new WaitForSeconds(clipTime);

            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying.Count == 0);
            Assert.IsTrue(AnimatorPlayableGameObject.AnimatorPlayableObject.AnimationLayerMixerPlayable.GetInputCount() == 0);
        }

        [UnityTest]
        public IEnumerator ThreeClip_InnerBlending()
        {
            var AnimatorPlayableGameObject = new AnimatorPlayableGameObject();

            float transitionTime = 0.0625f;
            float clipTime = 0.25f;
            AnimatorPlayableGameObject.PlaySequencedAnimation(new SequencedAnimationInput()
            {
                isInfinite = false,
                BeginTransitionTime = 0f,
                layerID = 0,
                EndTransitionTime = 0f,
                UniqueAnimationClips = new List<UniqueAnimationClip>()
                {
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime),
                        TransitionBlending = new LinearBlending()
                        {
                            EndTransitionTime = transitionTime
                        }
                    },
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime),
                        TransitionBlending = new LinearBlending()
                        {
                            EndTransitionTime = transitionTime
                        }
                    },
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime),
                        TransitionBlending = new LinearBlending()
                        {
                            EndTransitionTime = transitionTime
                        }
                    }
                }
            });
            yield return null;
            var SequencedAnimationLayer = AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[0] as SequencedAnimationLayer;
            //Only the first clip has weight
            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Paused);

            yield return new WaitForSeconds(clipTime - transitionTime);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) < 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) > 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Paused);

            yield return new WaitForSeconds(transitionTime);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Paused);

            yield return new WaitForSeconds(clipTime - transitionTime - transitionTime);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) < 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) > 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Playing);

            yield return new WaitForSeconds(transitionTime);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Playing);
        }

        [UnityTest]
        public IEnumerator ThreeClip_InnterBlending_InnerDelay()
        {
            var AnimatorPlayableGameObject = new AnimatorPlayableGameObject();

            float transitionTime = 0.0625f;
            float clipTime = 0.25f;
            AnimatorPlayableGameObject.PlaySequencedAnimation(new SequencedAnimationInput()
            {
                isInfinite = false,
                BeginTransitionTime = 0f,
                layerID = 0,
                EndTransitionTime = 0f,
                UniqueAnimationClips = new List<UniqueAnimationClip>()
                {
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime),
                        TransitionBlending = new LinearBlending()
                        {
                            EndTransitionTime = transitionTime
                        }
                    },
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime),
                        TransitionBlending = new LinearBlending()
                        {
                            EndTransitionTime = transitionTime,
                            EndClipDelay = transitionTime
                        }
                    },
                    new UniqueAnimationClip()
                    {
                        AnimationClip = CreateClip(clipTime),
                        TransitionBlending = new LinearBlending()
                        {
                            EndTransitionTime = transitionTime
                        }
                    }
                }
            });
            yield return null;

            var SequencedAnimationLayer = AnimatorPlayableGameObject.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[0] as SequencedAnimationLayer;
            //Only the first clip has weight
            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Paused);

            yield return new WaitForSeconds(clipTime - transitionTime);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) < 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) > 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Paused);

            yield return new WaitForSeconds(transitionTime);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Paused);

            yield return new WaitForSeconds(clipTime - transitionTime);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) < 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Playing);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) > 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Playing);

            yield return new WaitForSeconds(transitionTime);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(0) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[0].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(1) == 0f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[1].GetPlayState() == PlayState.Paused);

            Assert.IsTrue(SequencedAnimationLayer.AnimationMixerPlayable.GetInputWeight(2) == 1f);
            Assert.IsTrue(SequencedAnimationLayer.AssociatedAnimationClipsPlayable[2].GetPlayState() == PlayState.Playing);
        }

        public static AnimationClip CreateClip(float duration)
        {
            var clip1 = new AnimationClip();
            clip1.SetCurve("", typeof(Animator), "test", AnimationCurve.Linear(0, 0, duration, 1));
            return clip1;
        }
    }

    class AnimatorPlayableGameObject
    {
        public GameObject animatorPlayableGameObject { get; private set; }
        public Animator Animator { get; private set; }
        public AnimatorPlayableObject AnimatorPlayableObject { get; private set; }
        private AnimatorPlayableBehavior animatorPlayableBehavior;

        public AnimatorPlayableGameObject(GameObject parent = null)
        {
            this.animatorPlayableGameObject = new GameObject();
            if (parent != null)
            {
                this.animatorPlayableGameObject.transform.parent = parent.transform;
                this.animatorPlayableGameObject.transform.ResetLocal();
            }

            this.Animator = this.animatorPlayableGameObject.AddComponent<Animator>();
            this.AnimatorPlayableObject = new AnimatorPlayableObject("Test", this.Animator);
            this.animatorPlayableBehavior = this.animatorPlayableGameObject.AddComponent<AnimatorPlayableBehavior>();
            this.animatorPlayableBehavior.Init(this.AnimatorPlayableObject);
        }

        public void PlaySequencedAnimation(SequencedAnimationInput SequencedAnimationInput)
        {
            this.AnimatorPlayableObject.PlaySequencedAnimation(SequencedAnimationInput);
        }

        class AnimatorPlayableBehavior : MonoBehaviour
        {
            private AnimatorPlayableObject AnimatorPlayableObject;

            public void Init(AnimatorPlayableObject AnimatorPlayableObject)
            {
                this.AnimatorPlayableObject = AnimatorPlayableObject;
            }

            private void Update()
            {
                this.AnimatorPlayableObject.Tick(Time.deltaTime, 0f);
            }

            private void OnDestroy()
            {
                this.AnimatorPlayableObject.Destroy();
            }
        }
    }
}
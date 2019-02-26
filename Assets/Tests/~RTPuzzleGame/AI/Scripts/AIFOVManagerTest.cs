﻿using NUnit.Framework;
using RTPuzzle;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Tests
{
    public class AIFOVManagerTest
    {

        private AIFOVManager InitializeFOVManager()
        {
            NavMeshAgent agent = new NavMeshAgent();
            return new AIFOVManager(agent, null);
        }

        [Test]
        public void AIFOVManagerTest_Intersect_1stPass_UpAndDown()
        {
            var aiFOVManager = InitializeFOVManager();
            var cuttedFOV = aiFOVManager.IntersectFOV(-128f, 51f);
            Assert.AreEqual(new List<FOVSlice>() { new FOVSlice(232f, 360f), new FOVSlice(0f, 51f) }, cuttedFOV);
        }
        [Test]
        public void AIFOVManagerTest_Intersect_1stPass_Up()
        {
            var aiFOVManager = InitializeFOVManager();
            var cuttedFOV = aiFOVManager.IntersectFOV(190f, 260);
            Assert.AreEqual(new List<FOVSlice>() { new FOVSlice(190f, 260) }, cuttedFOV);
        }

        [Test]
        public void AIFOVManagerTest_Intersect_1stPass_Down()
        {
            var aiFOVManager = InitializeFOVManager();
            var cuttedFOV = aiFOVManager.IntersectFOV(260f, 190f);
            Assert.AreEqual(new List<FOVSlice>() { new FOVSlice(260f, 190f) }, cuttedFOV);
        }

        [Test]
        public void AIFOVManagerTest_Intersect_1stPass_UpAndDown_2ndPass_UpOutOfRange()
        {
            var aiFOVManager = InitializeFOVManager();
            var cuttedFOV = aiFOVManager.IntersectFOV(-124f, 55f);
            Assert.AreEqual(new List<FOVSlice>() { new FOVSlice(236f, 360f), new FOVSlice(0f, 55f) }, cuttedFOV);
            var cuttedFOV2ndPass = aiFOVManager.IntersectFOV(-223f, -43f);
            Assert.AreEqual(new List<FOVSlice>() { new FOVSlice(236f, 317f) }, cuttedFOV2ndPass);
        }

        [Test]
        public void AIFOVManagerTest_RaycastAngleCalculation()
        {
            var fov = new FOV(null);
            fov.ReplaceFovSlices(new List<FOVSlice>() { new FOVSlice(50, 150), new FOVSlice(200, 250) });
            var calculatedAngles = AIFOVManager.CalculateAnglesForRayCast(3, fov, false);
            Assert.AreEqual(new float[3] { 50f, 100f, 200f }, calculatedAngles);
        }
        [Test]
        public void AIFOVManagerTest_RaycastAngleCalculation_WithDownSlices()
        {
            var fov = new FOV(null);
            fov.ReplaceFovSlices(new List<FOVSlice>() { new FOVSlice(50, 150), new FOVSlice(250, 200) });
            var calculatedAngles = AIFOVManager.CalculateAnglesForRayCast(3, fov, false);
            Assert.AreEqual(new float[3] { 50f, 100f, 200f }, calculatedAngles);
        }

        [Test]
        public void AIFOVManagerTest_RaycastAngleCalculation_WithDownSlices_WithAnotherOrder()
        {
            var fov = new FOV(null);
            fov.ReplaceFovSlices(new List<FOVSlice>() { new FOVSlice(250, 200), new FOVSlice(50, 150) });
            var calculatedAngles = AIFOVManager.CalculateAnglesForRayCast(3, fov, false);
            Assert.AreEqual(new float[3] { 200f, 50f, 100f }, calculatedAngles);
        }

        [Test]
        public void AIFOVManagerTest_RaycastAngleCalculation_WithDownSlices_WithAnotherOrder_WithRandomness()
        {
            var fov = new FOV(null);
            fov.ReplaceFovSlices(new List<FOVSlice>() { new FOVSlice(250, 200), new FOVSlice(50, 150) });
            var calculatedAngles = AIFOVManager.CalculateAnglesForRayCast(30, fov, true);
            for (var i = 0; i < calculatedAngles.Length; i++)
            {
                Assert.IsTrue((calculatedAngles[i] >= 50f && calculatedAngles[i] <= 150f) || (calculatedAngles[i] >= 200f && calculatedAngles[i] <= 250f));
            }
        }

    }
}

using CoreGame;
using RTPuzzle;
using System;

namespace Tests
{

    public class MockPersistanceManager : PersistanceManager
    {
        public override void Init()
        {

        }

        public override void Tick(float d)
        {
        }

        public override void OnPersistRequested(Action persistAction)
        {
        }

        public override T Load<T>(string folderPath, string dataPath, string filename, string fileExtension)
        {
            return default(T);
        }
    }

    public class MockDottedLineRendererManager : DottedLineRendererManager
    {

        public override void OnComputeBeziersInnerPointEvent(DottedLine DottedLine)
        {

        }

        protected override void OnComputeBeziersInnerPointResponse(ComputeBeziersInnerPointResponse ComputeBeziersInnerPointResponse)
        {

        }

        public override void OnDottedLineDestroyed(DottedLine dottedLine)
        {

        }

        public override void OnLevelExit()
        {
        }

        public override void Tick()
        {
        }

    }
}

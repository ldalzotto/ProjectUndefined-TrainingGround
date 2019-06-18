using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class TestTimelineManager : TimelineNodeManagerV2<TestTimelineContext, TestTimelineKey>
    {
        private TestTimelineContext testTimelineContext = new TestTimelineContext();

        public TestTimelineContext TestTimelineContext { get => testTimelineContext; }

        protected override TestTimelineContext workflowActionPassedDataStruct => this.testTimelineContext;

        protected override TimelineID TimelineID => TimelineID.TESTING_TIMELINE;

        protected override bool isPersisted => false;
    }
}

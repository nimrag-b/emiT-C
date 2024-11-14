using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emiT_C
{
    public class Multiverse
    {
        Stack<Timeline> timelineStack = new Stack<Timeline>();

        Timeline active;

        int Timelines;

        bool Switching = false;

        public void Run(Timeline prime)
        {
            prime.SetEnumerator();
            timelineStack.Push(prime);
            Timelines++;
            Timeline current;
            int enumFrames = 0;
            while (timelineStack.Count > 0)
            {
                current = timelineStack.Pop();
                active = current;
                while (active.Enumerator.MoveNext())
                {
                    enumFrames++;
                    if (Switching)
                    {

                        Switching = false;
                        break;
                    }
                }

            }

            Console.WriteLine("Timelines Created: " + Timelines);
        }

        public void ChangeTimeline(Timeline timeline)
        {
            Switching = true;
            timelineStack.Push(active);
            timeline.SetEnumerator();
            timelineStack.Push(timeline);
            Timelines++;
        }
    }

    public class SavedTimelineState
    {
        public Timeline timeline;
        public TimeIndex index;

        public SavedTimelineState(Timeline timeline)
        {
            this.timeline = timeline;
            index = new TimeIndex(0, null);
        }

        public SavedTimelineState(Timeline timeline, TimeIndex index)
        {
            this.timeline = timeline;
            this.index = index;
        }
    }

    public class TimeIndex
    {
        public int index;
        public TimeIndex? child = null;

        public TimeIndex GetLast()
        {
            TimeIndex c = this;
            while(c.child != null)
            {
                c = c.child;
            }
            return c;
        }

        public TimeIndex(int index, TimeIndex child)
        {
            this.index = index;
            this.child = child;
        }
    }
}

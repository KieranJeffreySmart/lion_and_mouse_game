

namespace Lion_and_mouse.src.StoryContext
{
    public class Story(int currentDay)
    {
        public int CurrentDay = currentDay;

        internal Story IncrementDay()
        {
            return new Story(CurrentDay+1);
        }
    }
}
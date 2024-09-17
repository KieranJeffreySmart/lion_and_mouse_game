

namespace Lion_and_mouse.src.StoryContext
{
    public class Story(int currentDay)
    {
        public Guid Id  { get; } = Guid.NewGuid();
        public int CurrentDay = currentDay;
        private List<string> paragraphs = new List<string>();

        public string Text => String.Join("\r\n\r\n", paragraphs);

        public Story IncrementDay()
        {
            return new Story(CurrentDay+1);
        }

        public void AddParagraph(string text)
        {
            this.paragraphs.Add(text);
        }
    }
}
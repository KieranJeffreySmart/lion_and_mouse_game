namespace lion_and_mouse_game.StoryContext
{
    public class Story(int currentDay, IEnumerable<string>? paragraphs = null)
    {
        public Guid Id { get; } = Guid.NewGuid();
        public int CurrentDay = currentDay;
        private List<string> Paragraphs = paragraphs?.ToList() ?? new List<string>();

        public string Text => string.Join("\r\n\r\n", Paragraphs);

        public Story IncrementDay()
        {
            return new Story(CurrentDay + 1, Paragraphs);
        }

        public Story AddParagraph(string text)
        {
            return new Story(CurrentDay, Paragraphs.Union([text]));
        }
    }
}
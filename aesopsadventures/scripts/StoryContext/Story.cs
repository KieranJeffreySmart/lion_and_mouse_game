using System;
using System.Collections.Generic;
using System.Linq;

namespace lion_and_mouse_game.StoryContext
{
	public class Story
	{
		public Guid Id { get; } = Guid.NewGuid();
		public int CurrentDay;
		private readonly Dictionary<int, List<string>> Paragraphs;
		public string Text => string.Join("\r\n\r\n", Paragraphs[CurrentDay]);
		public Story(IDictionary<int, List<string>> paragraphs = null, int currentDay = 0) 
		{
			CurrentDay = currentDay;
			Paragraphs = new Dictionary<int, List<string>>(paragraphs ?? new Dictionary<int, List<string>>());
		}

		public Story IncrementDay()
		{
			return new Story(Paragraphs, CurrentDay + 1);
		}

		public Story AddParagraph(string text)
		{
			var newParagraphs = new Dictionary<int, List<string>>(Paragraphs);
			
			if (newParagraphs.ContainsKey(CurrentDay)) 
			{
				var dayParagraphs = newParagraphs[CurrentDay].ToList();
				dayParagraphs.Add(text);
				newParagraphs[CurrentDay] = dayParagraphs;
			}
			else
				newParagraphs.Add(CurrentDay, new List<string>() {text});

			return new Story(newParagraphs, CurrentDay);
		}

		
	}
}

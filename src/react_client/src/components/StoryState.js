function StoryState(storyData) {
    return (
      <div className="StoryState">
        <div className="text-start">
          Day: {storyData.CurrentDay}
        </div>  
        <div className="text-start">
          Story: {storyData.StoryText}
        </div> 
      </div>
    );
  }
  
  export default StoryState;
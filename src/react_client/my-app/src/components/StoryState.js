function StoryState(storyData) {
    return (
      <div className="StoryState">
        <div class="container">
            <div class="row">Day: {storyData.CurrentDay}</div>  
        </div>
      </div>
    );
  }
  
  export default StoryState;
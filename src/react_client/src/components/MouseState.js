function MouseState(mouse) {
    return (
      <div className="MouseState">
        <div className="text-start">
            State: {mouse ? mouse.state : 'Unknown'}
        </div>
        <div className="text-start">
            Food: {mouse ? mouse.food : 'Unknown'}
        </div>
      </div>
    );
  }
  
  export default MouseState;
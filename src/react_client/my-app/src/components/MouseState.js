function MouseState(mouse) {
    return (
      <div className="MouseState">
        <div class="container">
          <div class="row">
              State: {mouse ? <pre>{JSON.stringify(mouse.state, null, 2)}</pre> : 'Unknown'}
          </div>
          <div class="row">
              Food: {mouse ? <pre>{JSON.stringify(mouse.food, null, 2)}</pre> : 'Unknown'}
          </div>
        </div>
      </div>
    );
  }
  
  export default MouseState;
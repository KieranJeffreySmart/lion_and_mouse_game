import { useState, useCallback, useEffect } from 'react';

function MainMenu() {
    const [playerId, setPlayerId] = useState(null);

    const handleNewGame = 
    useCallback(() => {
        fetch('http://localhost:5250/play?playerName=Bob', {
          method: "post"
        })
          .then(response => response.json())
          .then(json => {setPlayerId(json.playerId); console.log("New Player ID: " + json.playerId);})
          .catch(error => console.error(error));
      }, []);

    return (
      <div style={{display:'flex', justifyContent:'flex-start'}} className="MainMenu">
        <button type="button" className="btn btn-primary" onClick={handleNewGame}>New Game</button>
        <div style={{marginLeft: '10px', fontSize: '16px'}}>Player ID: {playerId}</div>
      </div>
    );
  }
  
  export default MainMenu;
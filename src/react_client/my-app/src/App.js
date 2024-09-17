import './App.css';
import React, { useState, useEffect } from 'react';
import StoryState from './components/StoryState';

function App() {

  const [storyData, setStoryData] = useState(null);
  const [mouseData, setMouseData] = useState(null);

  useEffect(() => {
      fetch('https://api.example.com/data')
        .then(response => response.json())
        .then(json => setMouseData(json))
        .catch(error => console.error(error));
    }, []);



  return (
    <div className="App">
      <header className="App-header">
The Mouse And The Lion
      </header>
      <div class="container">
        <div class="row">
          <div class="col-xs-2">
            <div class="row"><div class="col">Day</div></div>
            {/* <div class="row"><StoryState Story={storyData} ></div> */}
            <div class="row"><div class="col">Mouse</div></div>
            {/* <div class="row">{MouseStatus}</div> */}

          </div>
          <div class="col">

          </div>
        </div>
      </div>
    </div>
  );
}

export default App;

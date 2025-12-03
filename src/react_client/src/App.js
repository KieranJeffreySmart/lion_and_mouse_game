import './App.css';
import { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import useWebSocket from 'react-use-websocket';
import StoryState from './components/StoryState';
import MouseState from './components/MouseState';
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

function App() {

    const [socketUrl, setSocketUrl] = useState('wss://echo.websocket.org');

    const [playerId, setPlayerId] = useState(null);
  
    const { sendJsonMessage, lastJsonMessage } = useWebSocket(socketUrl, {
      onOpen: () => {
        console.log('WebSocket connection established.');
      },
      onClose: () => {
        console.log('WebSocket connection closed.');
      }
    });

    const handleNewGame = 
    useCallback(() => {
        fetch('http://localhost:5250/play?playerName=Bob', {
          method: "post"
        })
          .then(response => response.json())
          .then(json => { setPlayerId(json.playerId); setSocketUrl(json.socketAddress); })
          .catch(error => console.error(error));
      fetch('http://localhost:5250/mouse')
        .then(response => response.json())
        .then(json => { setMouseData(json); console.log(`setMouseData ${JSON.stringify(json)}`); })
        .catch(error => console.error(error));
      fetch('http://localhost:5250/story')
        .then(response => response.json())
        .then(json => { setStoryData(json); console.log(`setStoryData ${JSON.stringify(json)}`); })
        .catch(error => console.error(error));
      }, []);

  function SendHuntCommand() {
    sendJsonMessage({ commandType: 0, playerId: playerId });
  }
  
  function SendStayAtHomeCommand() {
    sendJsonMessage({ commandType: 1, playerId: playerId });
  }

  const [storyData, setStoryData] = useState(null);
  const [mouseData, setMouseData] = useState(null);

  useEffect(() => {
     if (lastJsonMessage !== null) {
      fetch('http://localhost:5250/mouse')
        .then(response => response.json())
        .then(json => { setMouseData(json); console.log(`setMouseData ${JSON.stringify(json)}`); })
        .catch(error => console.error(error));
      fetch('http://localhost:5250/story')
        .then(response => response.json())
        .then(json => { setStoryData(json); console.log(`setStoryData ${JSON.stringify(json)}`); })
        .catch(error => console.error(error));
     }
  }, [lastJsonMessage]);

  return (
    <div className="App">
      <header className="App-header">
        <Container fluid>
          <Row>
            <Col>The Mouse And The Lion </Col>
          </Row>
          <Row>
            <Col style={{display:'flex', justifyContent:'flex-start'}}><button type="button" className="btn btn-primary" onClick={handleNewGame}>New Game</button></Col>
          </Row>
        </Container>
      </header>
      <Container fluid>
        <Row>
          <Col xs={3}>
            <div className="MouseState">
              <div className="text-start">
                  State: {mouseData ? mouseData.state : 'Unknown'}
              </div>
              <div className="text-start">
                  Food: {mouseData ? mouseData.food : 'Unknown'}
              </div>
            </div>
          </Col>
          <Col>
            <div className="StoryState">
              <div className="text-start">
                Day: {storyData ? storyData.currentDay : 'Unknown'}
              </div>  
              <div className="text-start">
                Story: {storyData ? storyData.storyText : 'Unknown'}
              </div> 
            </div>
          </Col>
        </Row>
      </Container>
      <Container fluid>
        <Row>
          <Col style={{display:'flex', justifyContent:'center'}}>
            <button type="button" className="btn btn-success" onClick={SendHuntCommand}>Hunt</button>
            <button type="button" className="btn btn-danger" onClick={SendStayAtHomeCommand}>Stay At Home</button>
          </Col>
        </Row>
      </Container>
    </div>
  );
}

export default App;

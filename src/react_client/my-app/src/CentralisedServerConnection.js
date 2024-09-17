import useWebSocket from 'react-use-websocket';

const WS_URL = 'ws://127.0.0.1:8000';

function CentralConnection() {
  useWebSocket(WS_URL, {
    onOpen: () => {
      console.log('WebSocket connection established.');
    }
  });
}

export default CentralConnection;
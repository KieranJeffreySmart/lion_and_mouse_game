import { useEffect, useRef } from 'react';
import useWebSocket from 'react-use-websocket';

const WS_URL = 'ws://127.0.0.1:8000';

function CentralConnection() {
  const wsRef = useRef(null);
  const { getWebSocket } = useWebSocket(WS_URL, {
    onOpen: () => {
      console.log('WebSocket connection established.');
      wsRef.current = getWebSocket();
    },
    onClose: () => {
      console.log('WebSocket connection closed.');
    }
  });

  useEffect(() => {
    return () => {
      if (wsRef.current && wsRef.current.readyState === WebSocket.OPEN) {
        wsRef.current.close();
      }
    };
  }, []);
}

export default CentralConnection;
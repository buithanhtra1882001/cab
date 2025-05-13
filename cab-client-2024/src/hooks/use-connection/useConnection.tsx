import { HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';

import { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { LocalStorageService, RootState } from '../../configuration';
import { PUBLIC_API_ENDPOINT } from '../../constants/platform';

export type ConnectionStatus = keyof typeof HubConnectionState;

const useConnectionBuilder = (url: string) => {
  const isAuthenticated = useSelector((state: RootState) => state.features.auth.profile);

  const [connection, setConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
    const accessToken = LocalStorageService.getItem('accessToken');

    if (!accessToken) {
      return;
    }

    if (!connection) {
      const connect = new HubConnectionBuilder()
        .withUrl(`${PUBLIC_API_ENDPOINT}/${url}`, {
          transport: HttpTransportType.WebSockets,
          accessTokenFactory: () => accessToken,
          skipNegotiation: true,
        })
        .withAutomaticReconnect()
        .build();

      setConnection(connect);
    }
  }, [isAuthenticated]);
  return { connection, setConnection };
};

export { useConnectionBuilder };

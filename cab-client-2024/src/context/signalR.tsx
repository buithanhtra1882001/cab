import { HubConnection, HubConnectionState } from '@microsoft/signalr';
import React, { createContext, Dispatch, PropsWithChildren, SetStateAction, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { useEventListener } from 'usehooks-ts';
import { LocalStorageService, RootState } from '../configuration';
import { useConnectionBuilder } from '../hooks/use-connection/useConnection';
import { HubEvent, HubName } from '../enums/hubnames';
import toast from 'react-hot-toast';
import { IChat } from '../types/chat';

interface ISignalRContext {
  state: {
    connectionChat: HubConnection | null;
    setConnection: Dispatch<SetStateAction<HubConnection | null>>;
    connectionNoti: HubConnection | null;
  };
}

export const SignalRContext = createContext<ISignalRContext>({
  state: {
    connectionChat: null,
    setConnection: () => null,
    connectionNoti: null,
  },
});

const SignalRProvider = ({ children }: PropsWithChildren) => {
  const isAuthenticated = useSelector((state: RootState) => state.features.auth.profile);
  const userProfile = useSelector((state: RootState) => state.features.auth.profile);

  const { connection: connectionChat, setConnection } = useConnectionBuilder(HubName.Chat);
  const { connection: connectionPresent } = useConnectionBuilder(HubName.Present);
  const { connection: connectionNoti } = useConnectionBuilder(HubName.Noti);

  // const createSignalHandler = (hubName: HubName) => {
  //   return () => {
  //     if (connection?.state === HubConnectionState.Connected) {
  //       connection.on(hubName, (signal: any) => {
  //         if (signal) {
  //           console.log('signal', signal);
  //         }
  //       });
  //     }
  //   };
  // };

  const invokeHandler = () => {
    const accessToken = LocalStorageService.getItem('accessToken');
  };
  const setupConnectionNoti = () => {
    if (connectionNoti && connectionNoti.state === HubConnectionState.Disconnected) {
      connectionNoti
        .start()
        .then(() => {
          invokeHandler();
        })
        .then(() => {
          //
        })
        .catch((error) => {
          console.log(`error occurred while starting connection: ${error.message}`);
        });

      connectionNoti.onclose((error) => {
        //
      });

      connectionNoti.onreconnected(() => {
        if (connectionPresent?.state === HubConnectionState.Connected) {
          invokeHandler();
        }
      });
    }
  };
  const setupConnection = () => {
    if (connectionChat && connectionChat.state === HubConnectionState.Disconnected) {
      connectionChat
        .start()
        .then(() => {
          invokeHandler();
        })
        .then(() => {
          //
        })
        .catch((error) => {
          console.log(`error occurred while starting connection: ${error.message}`);
        });

      connectionChat.onclose((error) => {
        //
      });

      connectionChat.onreconnected(() => {
        if (connectionChat?.state === HubConnectionState.Connected) {
          invokeHandler();
        }
      });
    }
  };

  const setupConnectionPresent = () => {
    if (connectionPresent && connectionPresent.state === HubConnectionState.Disconnected) {
      connectionPresent
        .start()
        .then(() => {
          invokeHandler();
        })
        .then(() => {
          //
        })
        .catch((error) => {
          console.log(`error occurred while starting connection: ${error.message}`);
        });

      connectionPresent.onclose((error) => {
        //
      });

      connectionPresent.onreconnected(() => {
        if (connectionPresent?.state === HubConnectionState.Connected) {
          invokeHandler();
        }
      });
    }
  };

  useEventListener('online', () => {
    if (connectionChat?.state === HubConnectionState.Disconnected) {
      connectionChat.stop().then(() => {
        setupConnection();
      });
    }
  });

  useEffect(() => {
    if (!isAuthenticated || !connectionChat) {
      return;
    }

    setupConnectionPresent();
    setupConnection();
    setupConnectionNoti();

    return () => {
      if (connectionChat?.state === HubConnectionState.Connected) {
        connectionChat.stop().then(() => {
          console.log('stop connection.');
        });
      }

      if (connectionPresent?.state === HubConnectionState.Connected) {
        connectionPresent.stop().then(() => {
          console.log('stop connectionPresent.');
        });
      }

      if (connectionNoti?.state === HubConnectionState.Connected) {
        connectionNoti.stop().then(() => {
          console.log('stop connectionNoti.');
        });
      }
    };
  }, [isAuthenticated, connectionChat, connectionNoti]);

  useEffect(() => {
    if (!connectionChat || !userProfile) {
      return;
    }

    connectionChat?.on(HubEvent.NewMessage, (message: IChat) => {
      const isNewMessage = message.recipientUserId === userProfile.userId;
      isNewMessage &&
        toast.success(`Bạn có tin nhắn mới từ ${message.senderName}`, {
          duration: 2000,
        });
    });

    connectionNoti?.on(HubEvent.NewNotification, (message: any) => {
      toast.success(`Bạn có tin nhắn mới từ `, {
        duration: 2000,
      });
    });
  }, [connectionChat, userProfile]);

  return (
    <SignalRContext.Provider
      // eslint-disable-next-line react/jsx-no-constructed-context-values
      value={{
        state: {
          connectionChat,
          setConnection,
          connectionNoti,
        },
      }}
    >
      {children}
    </SignalRContext.Provider>
  );
};

export default SignalRProvider;

import React, { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { PUBLIC_API_ENDPOINT } from '../../constants/platform';
import { useParams } from 'react-router-dom';
import ReactAudioPlayer from 'react-audio-player';
import ImageGif from '../../assets/gif/pikachu.gif';

const PageDonateLivestream = () => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [messages, setMessages] = useState<any>();
  const [isShow, setIsShow] = useState<any>(false);
  const { contentCreatorId } = useParams<{ contentCreatorId: string }>();
  useEffect(() => {
    console.log('🚀 ~ PageDonateLivestream ~ contentCreatorId:', contentCreatorId);

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${PUBLIC_API_ENDPOINT}/hubs/liveDonation?contentCreator=${contentCreatorId}`, {
        transport: signalR.HttpTransportType.WebSockets,
        skipNegotiation: true,
      })
      .withAutomaticReconnect()
      .build();
    setConnection(newConnection);

    return () => {
      if (newConnection) {
        newConnection.stop();
      }
    };
  }, [contentCreatorId]);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          console.log('Connected to SignalR');
          connection.on('RecieveDonationInfo', (message: any) => {
            console.log('🚀 ~ connection.on ~ message:', message);
            setIsShow(true);
            // setMessages((prevMessages) => [...prevMessages, message]);
            setMessages(message);
            setTimeout(() => {
              setIsShow(false);
            }, 6000);
          });
        })
        .catch((error) => console.error('Connection failed: ', error));
    }
  }, [connection]);
  const formatCurrency = (value) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
  };
  return (
    isShow && (
      <div className=" flex justify-center items-center flex-col gap-2 py-4 h-screen">
        <img alt="gif" src={ImageGif} />
        <ReactAudioPlayer src="https://files.playerduo.net/production/images/donate_sound/14.mp3" autoPlay loop />
        <div className="text-4xl font-extrabold text-stroke text-stroke-indigo-200 text-stroke-fill-indigo-600  ">
          {messages?.senderUserName} đã donate {formatCurrency(messages?.donationValue)}
        </div>
        <div className="text-2xl font-extrabold text-stroke  text-white">{messages?.message}</div>
      </div>
    )
  );
};

export default PageDonateLivestream;

import React from 'react';
import ReactDOM from 'react-dom/client';
import reportWebVitals from './reportWebVitals';
import Root from './root.component';
import { store } from './configuration';
import { Provider } from 'react-redux';
import { HelmetProvider } from 'react-helmet-async';
import { GoogleOAuthProvider } from '@react-oauth/google';
import './assets/styles/global/global.css';
import SignalRProvider from './context/signalR';

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement);

root.render(
  <GoogleOAuthProvider clientId="825503268617-mmn16hiugq7jc3k4shokna20h576dffa.apps.googleusercontent.com">
    <Provider store={store}>
      <HelmetProvider>
        <SignalRProvider>
          <Root />
        </SignalRProvider>
      </HelmetProvider>
    </Provider>
  </GoogleOAuthProvider>,
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();

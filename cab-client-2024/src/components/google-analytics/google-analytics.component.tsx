import React from 'react';
import { Helmet } from 'react-helmet-async';
import { IGoogleAnalyticsProps } from './google-analytics.type';
import { GA_MEASUREMENT_ID } from '../../constants/platform';

export const GoogleAnalytics: React.FC<IGoogleAnalyticsProps> = (props) => {
  return (
    <Helmet>
      <script async src={`https://www.googletagmanager.com/gtag/js?id=${GA_MEASUREMENT_ID}`} />
      <script>
        {`
      window.dataLayer = window.dataLayer || [];

      function gtag() {
        dataLayer.push(arguments);
      }

      gtag('js', new Date());
      gtag('config', '${GA_MEASUREMENT_ID}');
    `}
      </script>
    </Helmet>
  );
};

import React, { useEffect } from 'react';
import { LangService } from './configuration/language';
import { useObserver } from 'mobx-react';
import { SystemConfigStore } from './store';
import { initWuiLoginLanguage } from './languages';
import { RouterProvider } from 'react-router-dom';
import { RouterLinks } from './routes/router';
import dayjs from 'dayjs';
import { MOMENT_UPDATE_LOCALE_VI } from './constants/configuration.constant';
import updateLocale from 'dayjs/plugin/updateLocale';
import { Toaster } from 'react-hot-toast';
import { useDarkMode } from './hooks';
import { useDispatch } from 'react-redux';
import { AppDispatch } from './configuration';
import { fetchProfile } from './redux/features/auth/slice';
import UIkit from 'uikit';

initWuiLoginLanguage();

const Root = () => {
  const dispatch = useDispatch<AppDispatch>();

  const defaultLanguage = useObserver(() => SystemConfigStore.defaultLanguageSelector());

  const { isDarkMode } = useDarkMode();

  useEffect(() => {
    if (defaultLanguage && !LangService.instance().hasLocalLanguage()) {
      LangService.instance().changeLanguage(defaultLanguage);
    }
  }, [defaultLanguage]);

  useEffect(() => {
    /**
     * TODO:  When changing languages, remember to updateLocale according to the corresponding language
     */
    dayjs.extend(updateLocale);

    dayjs.updateLocale('en', MOMENT_UPDATE_LOCALE_VI);
  }, []);

  useEffect(() => {
    dispatch(fetchProfile());
  }, []);
  useEffect(() => {
    // Reinitialize UIkit slider
    UIkit.slider('.uk-slider');
    UIkit.drop('.uk-drop');
  }, []);

  return (
    <>
      <RouterProvider router={RouterLinks} fallbackElement={<p>Initial·Load...</p>} />
      <Toaster
        key={String(isDarkMode)}
        toastOptions={{
          style: {
            backgroundColor: isDarkMode ? '#27272a' : '#fff',
          },
          className: 'text-slate-800 dark:text-slate-50 text-sm',
          position: 'top-right',
        }}
      />
    </>
  );
};

export default Root;

import { useState, useEffect, useCallback } from 'react';

export type Theme = 'dark' | 'light';

export const useDarkMode = () => {
  const [theme, setTheme] = useState<Theme>('light');
  const [isDarkMode, setIsDarkMode] = useState<boolean>();

  const setMode = useCallback((mode: Theme) => {
    setTheme(mode);
    window.localStorage.setItem('darkMode', mode);
    document.documentElement.classList.add(mode);
    document.documentElement.classList.remove(mode === 'light' ? 'dark' : 'light');
  }, []);

  const toggleTheme = useCallback(() => {
    theme === 'dark' ? setMode('light') : setMode('dark');
  }, [theme]);

  useEffect(() => {
    const localTheme = window.localStorage.getItem('darkMode') as Theme;
    localTheme ? setTheme(localTheme) : setMode('light');
    localTheme ? setIsDarkMode(localTheme === 'dark') : setIsDarkMode(false);
  }, [window.localStorage.getItem('darkMode')]);

  return {
    theme,
    toggleTheme,
    isDarkMode,
  };
};

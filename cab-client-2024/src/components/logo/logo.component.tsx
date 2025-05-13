import React from 'react';
import { Link } from 'react-router-dom';
import { routePaths } from '../../routes/routes-path';
import { useDarkMode } from '../../hooks';
import { IMAGES } from '../../constants/common';

const Logo = () => {
  const { isDarkMode } = useDarkMode();

  return (
    <Link to={routePaths.home}>
      {isDarkMode ? (
        <img src={IMAGES.LOGO} alt="cab VN" className="w-24 h-14 object-contain" />
      ) : (
        <img src={IMAGES.LOGO_DARK} alt="cab VN" className="w-24 h-14 object-contain" />
      )}
    </Link>
  );
};

export default Logo;

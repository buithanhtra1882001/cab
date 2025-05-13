import React from 'react';
import { Outlet } from 'react-router-dom';
import { TopNav } from '../../views';
import { GoogleAnalytics } from '..';

const BlankLayout = () => {
  return (
    <>
      <TopNav />

      <div className="mt-16">
        <div className="">
          <Outlet />
        </div>
      </div>
      <GoogleAnalytics />
    </>
  );
};

export default BlankLayout;

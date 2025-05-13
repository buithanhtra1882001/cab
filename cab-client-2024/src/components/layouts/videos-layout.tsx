import React from 'react';
import { Outlet } from 'react-router-dom';
import { TopNav } from '../../views';
import { GoogleAnalytics } from '..';
import { AsideLeft } from '../../views/aside-left';

const VideosLayout = () => {
  return (
    <>
      <TopNav />
      <div className="fixed top-0 left-0 z-[99] pt-[--m-top] overflow-hidden transition-transform xl:duration-500 max-xl:w-full max-xl:-translate-x-full">
        <AsideLeft />
      </div>
      <main className="2xl:ml-[--w-side]  xl:ml-[--w-side-sm] p-2.5 h-[calc(100vh-var(--m-top))] mt-[--m-top]">
        <Outlet />
      </main>
      <GoogleAnalytics />
    </>
  );
};

export default VideosLayout;

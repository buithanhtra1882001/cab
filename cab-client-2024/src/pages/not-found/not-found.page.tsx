import React from 'react';
import { Helmet } from 'react-helmet-async';
import ButtonPrimary from '../../components/button-refactor/button-primary';
import { Link } from 'react-router-dom';
import { routePaths } from '../../routes/routes-path';

export const NotFoundPage = () => {
  return (
    <div className="toto-not-found">
      <Helmet>
        <title>404 Page Not Found</title>
        <meta name="description" content="Page not found" />
      </Helmet>
      <div
        className="pt-8 flex justify-center items-center "
        style={{
          height: 'calc(100vh - 64px)',
        }}
      >
        <div className="flex items-center flex-col">
          <h1 className="text-slate-800 dark:text-zinc-50 mb-5 font-medium">
            404 | Trang bạn tìm kiếm dường như không có!
          </h1>
          <Link to={routePaths.home}>
            <ButtonPrimary>Trang chủ</ButtonPrimary>
          </Link>
        </div>
      </div>
    </div>
  );
};

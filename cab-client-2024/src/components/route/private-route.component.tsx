/* eslint-disable react/react-in-jsx-scope */
import { Navigate, Outlet } from 'react-router-dom';
import { routePaths } from '../../routes/routes-path';
import { LocalStorageService } from '../../configuration';

const PrivateRoute = () => {
  const isLogged = LocalStorageService.getItem('accessToken');

  if (!isLogged) {
    return <Navigate to={routePaths.login} />;
  }

  return <Outlet />;
};

export default PrivateRoute;

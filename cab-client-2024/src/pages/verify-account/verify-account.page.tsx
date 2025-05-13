import React from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { useVerifyEmail } from '../../hooks/use-verify-email/use-verify-email';
import Alert from '../../components/alert/alert';
import { Helmet } from 'react-helmet-async';
import { routePaths } from '../../routes/routes-path';
import ButtonPrimary from '../../components/button-refactor/button-primary';
import Loading from '../../components/loading/loading.component';

interface IParamsPage {
  token: string;
  userId: string;
}

export const VerifyAccountPage = () => {
  const [searchParams] = useSearchParams();
  const paramValues = Object.fromEntries(searchParams) as unknown as IParamsPage;

  const navigate = useNavigate();

  const onSuccess = () => {
    navigate(routePaths.login);
  };

  const { loading, errorMessage } = useVerifyEmail({
    token: paramValues.token || '',
    userId: paramValues.userId || '',
    onVerifySuccess: onSuccess,
  });

  if (loading) {
    return (
      <div className="h-screen flex justify-center items-center">
        <Loading />
      </div>
    );
  }

  return (
    <div>
      <Helmet>
        <title>Xác nhận email</title>
        <meta name="description" content="Xác nhận email" />
      </Helmet>

      <div className="container">
        <div className="h-screen flex justify-center items-center">
          <div className="max-w-md w-full">
            <Alert visible={!!errorMessage} type="warning">
              {errorMessage}
            </Alert>

            <Link to={routePaths.login} className="inline-block mt-4 w-full">
              <ButtonPrimary className="w-full flex justify-center">Đăng nhập</ButtonPrimary>
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

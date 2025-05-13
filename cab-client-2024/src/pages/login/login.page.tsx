import React, { useState } from 'react';
import { ILoginThreeFormModel } from './login.type';
import { useLogin, useLoginThreeRd } from '../../hooks/use-login';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { SubmitHandler, useForm } from 'react-hook-form';
import { Helmet } from 'react-helmet-async';
import { GoogleLogin } from '@react-oauth/google';
import InputComponent from '../../components/input/input.component';
import { Link, useNavigate } from 'react-router-dom';
import ErrorValidateComponent from '../../components/error-validate/error-validate.component';
import { routePaths } from '../../routes/routes-path';
import { Facebook } from 'lucide-react';
import { ILoginPayLoad } from '../../types/authentication';
import ButtonPrimary from '../../components/button-refactor/button-primary';
import { useDispatch } from 'react-redux';
import { AppDispatch } from '../../configuration';
import { fetchProfile } from '../../redux/features/auth/slice';
import Alert from '../../components/alert/alert';
import { authenticationServices } from '../../services/authentication.service';

const loginSchema = yup.object().shape({
  email: yup.string().email('Email không hợp lệ').required('Vui lòng nhập email'),
  password: yup.string().required('Vui lòng nhập mật khẩu'),
  passCode: yup.string(),
});

export const LoginPage = () => {
  const { handleLoginThreeRd } = useLoginThreeRd();

  const dispatch = useDispatch<AppDispatch>();

  const navigate = useNavigate();

  const [resendSuccess, setResendSuccess] = useState<boolean>(false);

  const {
    handleSubmit,
    register,
    getValues,
    formState: { errors },
  } = useForm<ILoginPayLoad>({
    mode: 'all',
    resolver: yupResolver(loginSchema),
  });

  function handleLoginWithFacebook() {
    const popup = window.open(
      `https://www.facebook.com/v12.0/dialog/oauth?client_id=743059063074786&redirect_uri=${encodeURIComponent(
        window.location.origin,
      )}&scope=email`,
      'facebook-login-popup',
      'width=600,height=600',
    );
  }

  const onLoginSuccess = async () => {
    await dispatch(fetchProfile());

    navigate(routePaths.profile);
  };

  const { handleLogin, loading, errorMessage, isErrorEmailConfirm } = useLogin(onLoginSuccess);

  const onSubmit: SubmitHandler<ILoginPayLoad> = async (formValues) => {
    await handleLogin(formValues);
  };

  const handleResendVerifyEmail = async () => {
    try {
      const email = getValues('email');
      await authenticationServices.resendVerifyEmail(email);
      setResendSuccess(true);
    } catch (error) {
      setResendSuccess(false);
    } finally {
      //
    }
  };

  // useEffect(() => {
  //   const urlParams = new URLSearchParams(window.location.search);
  //   const code = urlParams.get('code');

  //   if (code) {
  //     fetch('YOUR_SERVER_ENDPOINT', {
  //       method: 'POST',
  //       headers: {
  //         'Content-Type': 'application/json',
  //       },
  //       body: JSON.stringify({ code }),
  //     })
  //       .then((response) => response.json())
  //       .then((data) => {
  //         const accessToken = data.access_token;

  //         fetch(`https://graph.facebook.com/me?fields=id,name,email,picture&access_token=${accessToken}`)
  //           .then((response) => response.json())
  //           .then((userData) => {
  //             console.log('User data:', userData);
  //           })
  //           .catch((error) => {
  //             console.error('Error fetching user data:', error);
  //           });
  //       })
  //       .catch((error) => {
  //         console.error('Error exchanging code for access token:', error);
  //       });
  //   }
  // });

  const responseMessage = (response) => {
    const dataThreeRd: ILoginThreeFormModel = {
      provider: 'google',
      accessToken: response.credential,
      passCode: getValues('passCode'),
    };

    handleLoginThreeRd(dataThreeRd);
  };

  return (
    <div className="toto-login">
      <Helmet
        style={[
          {
            cssText: `
            #login-google {
              [role='button'] {
                border: none;
                background: none;
              }
            }
        `,
          },
        ]}
      >
        <title>Đăng nhập</title>
        <meta name="description" content="Description of SignUpPage" />
      </Helmet>

      <div className="flex justify-center items-center min-h-screen">
        <div className="w-full px-5 lg:px-0 mx-auto py-8">
          <div className="rounded-md bg-slate-50 dark:bg-zinc-800 max-w-md mx-auto p-6 shadow-lg">
            <h2 className="text-3xl font-bold mb-8 text-center">Đăng nhập</h2>

            <form className="mb-4" onSubmit={handleSubmit(onSubmit)}>
              <div className="mb-4">
                <label className="block font-medium text-sm mb-2 " htmlFor="email">
                  Email
                </label>

                <InputComponent register={register('email')} type="email" id="email" />
                <ErrorValidateComponent visible={!!errors.email}>{errors.email?.message}</ErrorValidateComponent>
              </div>

              <div className="mb-4">
                <label className="block font-medium text-sm mb-2 " htmlFor="password">
                  Mật khẩu
                </label>

                <InputComponent register={register('password')} type="password" id="password" />
                <ErrorValidateComponent visible={!!errors.password}>{errors.password?.message}</ErrorValidateComponent>
              </div>

              <div className="mb-4">
                <label className="block font-medium text-sm mb-2 " htmlFor="passCode">
                  Pass code
                </label>

                <InputComponent register={register('passCode')} id="passCode" type="password" />
              </div>

              <div className="flex justify-end mb-4" data-svelte-h="svelte-sotu5m">
                <Link
                  className="inline-block text-sm font-semibold text-primary-6 transition duration-200"
                  to="/account/forgot"
                >
                  Quên mật khẩu
                </Link>
              </div>

              <Alert type="error" classNames="mb-4" visible={!!errorMessage && !resendSuccess}>
                {errorMessage}
                {isErrorEmailConfirm ? (
                  <p>
                    Nếu bạn không nhập được mail hoặc liên kết xác nhận đã hết hạn.{' '}
                    <button
                      className="text-primary-5"
                      type="button"
                      onClick={() => {
                        handleResendVerifyEmail();
                      }}
                    >
                      Nhấn vào Đây
                    </button>{' '}
                    để nhận lại email xác thực.
                  </p>
                ) : null}
              </Alert>

              <Alert type="success" classNames="mb-4" visible={resendSuccess}>
                Chúng tôi vừa gửi liên kết xác thực tài khoản về mail của bạn.
              </Alert>

              <ButtonPrimary type="submit" className="w-full justify-center" loading={loading} disabled={loading}>
                Đăng nhập
              </ButtonPrimary>
            </form>

            <div className="mb-8">
              <p className="text-sm text-slate-800 dark:text-gray-100">
                Bạn chưa có tài khoản?{' '}
                <Link to={routePaths.register} className="text-primary-5 font-semibold">
                  Đăng ký
                </Link>
              </p>
            </div>

            <div className="flex items-center text-center my-4" data-svelte-h="svelte-1sasxhj">
              <div className="h-[1px] bg-slate-200 dark:bg-zinc-50/10 flex-1" />
              <p className=" text-zinc-500 dark:text-zinc-300 text-center text-sm mx-4">Hoặc</p>
              <div className="h-[1px] bg-slate-200 dark:bg-zinc-50/10 flex-1" />
            </div>

            <div className="flex flex-col justify-center items-center mb-8" data-svelte-h="svelte-tkh4w5">
              <button
                className="rounded-2xl flex justify-center items-center bg-zinc-50 p-2 text-sm
                mb-3 text-slate-800"
                onClick={handleLoginWithFacebook}
              >
                <Facebook className="mr-2 text-primary-5" />
                Đăng nhập bằng Facebook
              </button>
              {/* <div>
                <FacebookLogin
                  appId="743059063074786"
                  autoLoad={false}
                  fields="name,email,picture"
                  callback={responseFacebook}
                />
              </div> */}

              <GoogleLogin
                onSuccess={responseMessage}
                width={10}
                shape="circle"
                containerProps={{
                  id: 'login-google',
                  style: {
                    background: 'none',
                  },
                }}
              />
            </div>

            <p className="text-sm text-center text-zinc-500 dark:text-zinc-300" data-svelte-h="svelte-sgs55j">
              ©2022 cab.vn
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

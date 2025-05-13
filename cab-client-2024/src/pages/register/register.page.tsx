import React, { useState } from 'react';
import { useRegister } from '../../hooks/use-register';
import { yupResolver } from '@hookform/resolvers/yup';
import { SubmitHandler, useForm } from 'react-hook-form';
import * as yup from 'yup';
import { Helmet } from 'react-helmet-async';
import InputComponent from '../../components/input/input.component';
import ErrorValidateComponent from '../../components/error-validate/error-validate.component';
import { Link } from 'react-router-dom';
import { routePaths } from '../../routes/routes-path';
import ButtonPrimary from '../../components/button-refactor/button-primary';
import Alert from '../../components/alert/alert';

const registerSchema = yup.object().shape({
  email: yup.string().email('Email không hợp lệ').required('Email không được bỏ trống'),
  username: yup.string().required('Tên đăng nhập không được bỏ trống'),
  fullName: yup.string().required('Tên không được bỏ trống'),
  password: yup.string().required('Mật khẩu không được bỏ trống').min(6, 'Mật khẩu phải có ít nhất 6 ký tự'),
  confirmPassword: yup
    .string()
    .required('Nhập lại mật khẩu không khớp')
    .oneOf([yup.ref('password'), ''], 'Nhập lại mật khẩu không khớp'),
  code: yup.string().required('Mã giới thiệu không được bỏ trống'),
});

export interface IRegisterFormValues {
  email: string;
  fullName: string;
  username: string;
  password: string;
  code: string;
  confirmPassword: string;
}

export const RegisterPage = () => {
  const [registerSuccess, setRegisterSuccess] = useState<boolean>(false);

  const onRegisterSuccess = () => {
    setRegisterSuccess(true);
  };

  const { handleRegister, loading } = useRegister(onRegisterSuccess);

  const {
    handleSubmit,
    register,
    formState: { errors },
  } = useForm<IRegisterFormValues>({
    mode: 'all',
    resolver: yupResolver(registerSchema),
  });

  const onSubmit: SubmitHandler<IRegisterFormValues> = async (formValues) => {
    await handleRegister(formValues);
  };

  return (
    <div className="toto-register">
      <Helmet>
        <title>Đăng ký</title>
        <meta name="register" content="Description of register" />
      </Helmet>

      <div className="min-h-screen flex justify-center items-center p-2">
        <div className="w-full px-5 lg:px-0 mx-auto py-8">
          <div className="rounded-md bg-transparent md:bg-slate-50 dark:bg-zinc-800 max-w-md mx-auto p-6 shadow-lg">
            <h2 className="text-2xl font-bold mb-8 text-center">Đăng ký tài khoản</h2>

            <form className="mb-4" onSubmit={handleSubmit(onSubmit)}>
              <div className="mb-4">
                <label className="block font-medium text-sm mb-2 " htmlFor="email">
                  Email
                </label>

                <InputComponent register={register('email')} type="email" id="email" />
                <ErrorValidateComponent visible={!!errors.email}>{errors.email?.message}</ErrorValidateComponent>
              </div>
              <div className="mb-4">
                <label className="block font-medium text-sm mb-2 " htmlFor="fullName">
                  Họ và Tên
                </label>
                <InputComponent register={register('fullName')} id="fullName" />
                <ErrorValidateComponent visible={!!errors.fullName}>{errors.fullName?.message}</ErrorValidateComponent>
              </div>
              <div className="mb-4">
                <label className="block font-medium text-sm mb-2 " htmlFor="username">
                  Tên đăng nhập
                </label>
                <InputComponent register={register('username')} id="username" />
                <ErrorValidateComponent visible={!!errors.username}>{errors.username?.message}</ErrorValidateComponent>
              </div>
              <div className="mb-4">
                <label className="block font-medium text-sm mb-2 " htmlFor="password">
                  Mật khẩu
                </label>
                <InputComponent register={register('password')} id="password" type="password" />
                <ErrorValidateComponent visible={!!errors.password}>{errors.password?.message}</ErrorValidateComponent>
              </div>
              <div className="mb-4">
                <label className="block font-medium text-sm mb-2 " htmlFor="confirmPassword">
                  Nhập lại mật khẩu
                </label>
                <InputComponent register={register('confirmPassword')} id="confirmPassword" type="password" />
                <ErrorValidateComponent visible={!!errors.confirmPassword}>
                  {errors.confirmPassword?.message}
                </ErrorValidateComponent>
              </div>
              <div className="mb-4">
                <label className="block font-medium text-sm mb-2 " htmlFor="code">
                  Mã giới thiệu
                </label>
                <InputComponent register={register('code')} id="code" />
                <ErrorValidateComponent visible={!!errors.code}>{errors.code?.message}</ErrorValidateComponent>
              </div>

              <Alert visible={registerSuccess} type="success" classNames="mb-4">
                Chúng tôi vừa gửi mail xác nhận đến địa chỉ mail của bạn.
              </Alert>

              <ButtonPrimary
                type="submit"
                className="w-full justify-center"
                loading={loading}
                disabled={loading}
                onClick={() => {
                  setRegisterSuccess(false);
                }}
              >
                Đăng ký
              </ButtonPrimary>
            </form>

            <div className="mb-8">
              <p className="text-sm text-slate-800 dark:text-gray-100">
                Bạn đã có tài khoản?{' '}
                <Link to={routePaths.login} className="text-primary-5 font-semibold">
                  Đăng nhập
                </Link>
              </p>
            </div>

            <p className="text-xs text-center text-zinc-500 dark:text-zinc-300 mb-4">
              Bằng việc đăng kí tài khoản, bạn đã đồng ý với{' '}
              <a className="text-primary-6 font-medium " href="/" target="_blank">
                Điều khoản sử dụng
              </a>{' '}
              và dã đọc{' '}
              <a className="text-primary-6 font-medium " href="/" target="_blank">
                Chính sách quyền riêng tư
              </a>{' '}
              của chúng tôi.
            </p>
            <p className="text-xs text-center text-zinc-500 dark:text-zinc-300" data-svelte-h="svelte-sgs55j">
              ©2022 cab.vn
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

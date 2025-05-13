import React, { FormEventHandler, useCallback } from 'react';
import { IGatePageProps } from './gate.type';
import { useNavigate } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';

const { PUBLIC_PRIVATE_TEST_CODE } = process.env;

export const GatePage: React.FC<IGatePageProps> = () => {
  const navigate = useNavigate();

  const handleSubmit: FormEventHandler = useCallback((ev) => {
    ev.preventDefault();
    const target = ev.target as typeof ev.target & {
      privateTestCode: { value: string };
    };
    const privateTestCode = target.privateTestCode.value;

    if (privateTestCode === '' || privateTestCode !== PUBLIC_PRIVATE_TEST_CODE) {
      return;
    }

    localStorage.setItem('privateTestCode', privateTestCode);
    navigate('/');
  }, []);

  return (
    <div
      className="toto-gate w-screen min-h-screen bg-center bg-cover flex justify-center items-center p-2"
      style={{ backgroundImage: 'url("https://picsum.photos/1600/900")' }}
    >
      <Helmet>
        <title>Gate Page</title>
        <meta name="gate" content="Gate Page" />
      </Helmet>
      <div className="w-full max-w-[1300px] px-2 lg:px-0 mx-auto">
        <div className="rounded-2xl bg-white dark:bg-black lg:w-[500px] mx-auto p-8">
          <div className="text-3xl font-bold mb-8 text-center">Trải nghiệm private test</div>
          <form className="space-y-4 mb-4" action="#" onSubmit={handleSubmit}>
            <input
              className="rounded-md block w-full bg-zinc-100 dark:bg-zinc-800 text-sm px-4 py-2 "
              type="text"
              placeholder="Nhập code của bạn"
              id="privateTestCode"
              name="privateTestCode"
            />
            <button
              type="submit"
              className="rounded-md flex justify-center items-center gap-2 h-10 text-sm px-4 py-2 bg-primary-4 dark:bg-primary text-white dark:text-gray-100 hover:bg-primary dark:hover:bg-primary-6 break-words transition duration-200 w-full"
            >
              <div className="line-clamp-1">Tham gia</div>
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};

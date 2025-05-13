/* eslint-disable react/react-in-jsx-scope */
import { memo, PropsWithChildren } from 'react';

interface IErrorValidateProps extends PropsWithChildren {
  visible?: boolean;
}

const ErrorValidate = ({ visible = false, children }: IErrorValidateProps) => {
  return visible ? <p className="text-red-600 mt-2 text-xs font-medium">{children}</p> : null;
};

export default memo(ErrorValidate);

import { computedFn } from 'mobx-utils';
import { isMustVerifyPhoneSelector } from './is-must-verify-phone.selector';

export const isLoginAndNotMustVerifyPhoneSelector = computedFn(() => {
  return !isMustVerifyPhoneSelector();
});

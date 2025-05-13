import { action } from 'satcheljs';

export const loginAction = action('loginAction', (email: string, password: string, passCode?: string) => {
  return { email, password, passCode };
});

export const loginThreeRdAction = action(
  'loginThreeRdAction',
  (provider: string, accessToken: string, passCode?: string) => {
    return { provider, accessToken, passCode };
  },
);

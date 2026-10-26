import { useMsal } from '@azure/msal-react';
import { loginRequest } from '../config/authConfig';

export const Login = () => {
  const { instance } = useMsal();

  const handleSignIn = () => {
    instance.loginRedirect(loginRequest);
  };

  return (
    <button type="button" onClick={handleSignIn}>
      Sign in
    </button>
  );
};

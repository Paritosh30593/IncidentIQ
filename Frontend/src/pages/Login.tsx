import { Navigate, useLocation } from 'react-router';
import { InteractionStatus } from '@azure/msal-browser';
import { useIsAuthenticated, useMsal } from '@azure/msal-react';
import { loginRequest } from '../config/authConfig';

export const Login = () => {
  const { instance, inProgress } = useMsal();
  const isAuthenticated = useIsAuthenticated();
  const location = useLocation();
  const from = (location.state as { from?: { pathname: string } } | null)?.from?.pathname ?? '/';

  if (inProgress !== InteractionStatus.None) {
    return null;
  }

  if (isAuthenticated) {
    return <Navigate to={from} replace />;
  }

  const handleSignIn = () => {
    instance.loginRedirect({ ...loginRequest, redirectStartPage: from });
  };

  return (
    <button type="button" onClick={handleSignIn}>
      Sign in
    </button>
  );
};

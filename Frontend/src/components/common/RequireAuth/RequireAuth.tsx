import { Navigate, Outlet, useLocation } from 'react-router';
import { InteractionStatus } from '@azure/msal-browser';
import { useMsal } from '@azure/msal-react';

export const RequireAuth = () => {
  const { accounts, inProgress } = useMsal();
  const location = useLocation();

  if (inProgress !== InteractionStatus.None) {
    return null;
  }

  if (accounts.length === 0) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />;
};

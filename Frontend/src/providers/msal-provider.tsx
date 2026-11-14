import type { ReactNode } from 'react';
import { MsalProvider } from '@azure/msal-react';
import { msalInstance } from '../lib/auth/msalInstance';

export const MsalClientProvider = ({ children }: { children: ReactNode }) => (
  <MsalProvider instance={msalInstance}>{children}</MsalProvider>
);

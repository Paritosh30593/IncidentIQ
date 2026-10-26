import type { ReactNode } from 'react';
import { PublicClientApplication } from '@azure/msal-browser';
import { MsalProvider } from '@azure/msal-react';
import { msalConfig } from '../config/authConfig';

const msalInstance = new PublicClientApplication(msalConfig);
await msalInstance.initialize();

export const MsalClientProvider = ({ children }: { children: ReactNode }) => (
  <MsalProvider instance={msalInstance}>{children}</MsalProvider>
);

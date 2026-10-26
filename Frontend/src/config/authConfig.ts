/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { LogLevel, type Configuration } from '@azure/msal-browser';

const normalizeOrigin = (value?: string) => {
  if (!value) return undefined;

  const trimmed = value.trim();
  if (!trimmed) return undefined;

  // Ensure we only accept absolute http(s) origins for MSAL redirect URLs.
  if (!/^https?:\/\//i.test(trimmed)) return undefined;

  return trimmed.replace(/\/+$/, '');
};

const appOrigin = normalizeOrigin(import.meta.env.VITE_AZURE_HOST) ?? window.location.origin;

/**
 * Configuration object to be passed to MSAL instance on creation.
 * For a full list of MSAL.js configuration parameters, visit:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md
 */
export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_CLIENT_ID ?? '',
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_AZURE_TENANT_ID ?? 'organizations'}/v2.0`,
    redirectUri: appOrigin,
    postLogoutRedirectUri: appOrigin,
  },
  cache: {
    // Persist auth cache across browser restarts.
    cacheLocation: 'localStorage',
    // Keep migrated legacy cache entries for 1 day before cleanup.
    cacheRetentionDays: 1,
  },
  system: {
    loggerOptions: {
      /**
       * Below you can configure MSAL.js logs. For more information, visit:
       * https://docs.microsoft.com/azure/active-directory/develop/msal-logging-js
       */
      loggerCallback: (level: LogLevel, message: string, containsPii: boolean) => {
        if (containsPii) {
          return;
        }
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            return;
          case LogLevel.Info:
            console.info(message);
            return;
          case LogLevel.Verbose:
            console.debug(message);
            return;
          case LogLevel.Warning:
            console.warn(message);
            return;
          default:
            return;
        }
      },
    },
  },
};

/**
 * Add here the endpoints and scopes when obtaining an access token for protected web APIs. For more information, see:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/resources-and-scopes.md
 */
export const protectedResources = {
  api: {
    endpoint: import.meta.env.VITE_API_BASE_URL,
    scopes: [`api://${import.meta.env.VITE_AZURE_AUTH_APP_ID}/.default`],
  },
};

/**
 * Scopes you add here will be prompted for user consent during sign-in.
 * By default, MSAL.js will add OIDC scopes (openid, profile, email) to any login request.
 * For more information about OIDC scopes, visit:
 * https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent#openid-connect-scopes
 */
export const loginRequest = {
  scopes: protectedResources.api.scopes,
  redirectUri: appOrigin,
};

import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'
import { QueryProvider } from './providers/query-provider.tsx'
import { MsalClientProvider } from './providers/msal-provider.tsx'

createRoot(document.getElementById('root')!).render(
	<StrictMode>
		<MsalClientProvider>
			<QueryProvider>
				<App />
			</QueryProvider>
		</MsalClientProvider>
	</StrictMode>,
)

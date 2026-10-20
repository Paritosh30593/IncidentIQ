import './App.css';
import { useTestGet } from './features/test/hooks/useTestGet';

function App() {
    const { data, isLoading, error } = useTestGet();

    return (
        <>
            <h1>Hello, World!</h1>
            {isLoading && <p>Loading...</p>}
            {error && <p>Failed to load test data.</p>}
            {data && <p>{data.sampleText}</p>}
        </>
    )
}

export default App

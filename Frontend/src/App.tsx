import { useEffect } from 'react';
import './App.css';
import { httpClient } from './lib/api/httpClient';

function App() {
    useEffect(() => {
        const checkTest = async () => {
            const response = await httpClient.get('/test');
            console.log(response);
        };
        checkTest();
    }, []);

    return (
        <>
            <h1>Hello, World!</h1>
            <p>Check the console for the test API response.</p>
        </>
    )
}

export default App

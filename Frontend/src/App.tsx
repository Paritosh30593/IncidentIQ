import './App.css';
import { BrowserRouter, Route, Routes } from 'react-router';
import { RequireAuth } from './components/common/RequireAuth';
import { Home } from './pages/Home';
import { Login } from './pages/Login';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route element={<RequireAuth />}>
                    <Route path="/" element={<Home />} />
                </Route>
            </Routes>
        </BrowserRouter>
    )
}

export default App

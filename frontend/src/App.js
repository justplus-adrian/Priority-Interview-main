import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Navigation from './components/Navigation';
import Welcome from './pages/Welcome';
import './App.css';

// TO ADD NEW PAGES: Add items to this array
const routes = [
  { path: '/', name: 'Home', component: Welcome }
  // Example: { path: '/about', name: 'About', component: About }
];

function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <Navigation routes={routes} />
        <main className="main-content">
          <Routes>
            {routes.map((route) => (
              <Route 
                key={route.path} 
                path={route.path} 
                element={<route.component />} 
              />
            ))}
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;


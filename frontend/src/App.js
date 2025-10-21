import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Navigation from './components/Navigation';
import Welcome from './pages/Welcome';
import CustomerProfile from './pages/CustomerProfile';
import VisitationAnalytics from './pages/VisitationAnalytics';
import './App.css';

// TO ADD NEW PAGES: Add items to this array
const routes = [
  { path: '/', name: 'Home', component: Welcome },
  { path: '/customer', name: 'Customers', component: CustomerProfile },
  { path: '/analytics', name: 'Analytics', component: VisitationAnalytics }
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
            {/* Additional route for customer profile with ID */}
            <Route path="/customer/:id" element={<CustomerProfile />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;


import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import Dashboard from "./pages/customer/Dashboard.jsx";
import Admin from "./pages/admin/Admin.jsx";
import Analytics from "./pages/admin/Analytics.jsx";
import AdminOrders from "./pages/admin/Orders.jsx";
import Account from "./pages/customer/Account.jsx";
import Orders from "./pages/customer/Orders.jsx";
import Cart from "./pages/customer/Cart.jsx";
import NotFound from "./pages/NotFound.jsx";
import PrivateAuth from "./components/auth/PrivateAuth";
import { CartProvider } from "./context/CartContext.jsx";

function App() {
  return (
    <CartProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Navigate to="/admin" replace />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route
            path="/dashboard"
            element={
              <PrivateAuth>
                <Dashboard />
              </PrivateAuth>
            }
          />
          <Route 
            path="/admin" 
            element={
              <PrivateAuth>
                <Admin />
              </PrivateAuth>
            } 
          />
          <Route 
            path="/admin/analytics" 
            element={
              <PrivateAuth>
                <Analytics />
              </PrivateAuth>
            } 
          />
          <Route 
            path="/admin/orders" 
            element={
              <PrivateAuth>
                <AdminOrders />
              </PrivateAuth>
            } 
          />
          <Route
            path="/account"
            element={
              <PrivateAuth>
                <Account />
              </PrivateAuth>
            }
          />
          <Route
            path="/orders"
            element={
              <PrivateAuth>
                <Orders />
              </PrivateAuth>
            }
          />
          <Route
            path="/cart"
            element={
              <PrivateAuth>
                <Cart />
              </PrivateAuth>
            }
          />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </BrowserRouter>
    </CartProvider>
  );
}

export default App;

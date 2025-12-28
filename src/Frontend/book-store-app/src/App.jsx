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
    <BrowserRouter>
      <CartProvider>
        <Routes>
          {/* Root goes to login */}
          <Route path="/" element={<Navigate to="/login" replace />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route
            path="/dashboard"
            element={
              <PrivateAuth requiredRole="Customer">
                <Dashboard />
              </PrivateAuth>
            }
          />
          <Route
            path="/admin"
            element={
              <PrivateAuth requiredRole="Admin">
                <Admin />
              </PrivateAuth>
            }
          />
          <Route
            path="/admin/analytics"
            element={
              <PrivateAuth requiredRole="Admin">
                <Analytics />
              </PrivateAuth>
            }
          />
          <Route
            path="/admin/orders"
            element={
              <PrivateAuth requiredRole="Admin">
                <AdminOrders />
              </PrivateAuth>
            }
          />
          <Route
            path="/account"
            element={
              <PrivateAuth requiredRole="Customer">
                <Account />
              </PrivateAuth>
            }
          />
          <Route
            path="/orders"
            element={
              <PrivateAuth requiredRole="Customer">
                <Orders />
              </PrivateAuth>
            }
          />
          <Route
            path="/cart"
            element={
              <PrivateAuth requiredRole="Customer">
                <Cart />
              </PrivateAuth>
            }
          />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </CartProvider>
    </BrowserRouter>
  );
}

export default App;

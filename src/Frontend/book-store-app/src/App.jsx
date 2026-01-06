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
      <Routes>
        {/* Root goes to login */}
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route
          path="/dashboard"
          element={
            <CartProvider>
              <PrivateAuth requiredRole="Customer">
                <Dashboard />
              </PrivateAuth>
            </CartProvider>
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
            <CartProvider>
              <PrivateAuth requiredRole="Customer">
                <Account />
              </PrivateAuth>
            </CartProvider>
          }
        />
        <Route
          path="/orders"
          element={
            <CartProvider>
              <PrivateAuth requiredRole="Customer">
                <Orders />
              </PrivateAuth>
            </CartProvider>
          }
        />
        <Route
          path="/cart"
          element={
            <CartProvider>
              <PrivateAuth requiredRole="Customer">
                <Cart />
              </PrivateAuth>
            </CartProvider>
          }
        />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;

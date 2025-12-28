import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';

export default function PrivateAuth({ children, requiredRole }) {
  const location = useLocation();
  const access = localStorage.getItem('access');
  const role = localStorage.getItem('role');

  if (!access) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  // If a specific role is required, check if user has it
  if (requiredRole && role !== requiredRole) {
    const redirectTo = role === "Admin" ? "/admin" : "/dashboard";
    return <Navigate to={redirectTo} replace />;
  }

  return children;
}

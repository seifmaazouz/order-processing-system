import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';

export default function PrivateAuth({ children }) {
  const location = useLocation();
  const access = localStorage.getItem('access');

  if (!access) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  return children;
}

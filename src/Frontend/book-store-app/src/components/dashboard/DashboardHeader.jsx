import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import {
  faBook,
  faCog,
  faShoppingCart,
  faUser,
  faBoxOpen,
  faArrowRightFromBracket,
} from '@fortawesome/free-solid-svg-icons';
import LogoutConfirmation from '../shared/LogoutConfirmation.jsx';

export default function DashboardHeader({ showSettings, onToggleSettings, settingsRef, cartTotal }) {
  const navigate = useNavigate();
  const [showLogoutConfirm, setShowLogoutConfirm] = useState(false);

  const handleNavigate = (path) => {
    onToggleSettings(false);
    navigate(path);
  };

  const handleLogout = () => {
    setShowLogoutConfirm(false);
    onToggleSettings(false);
    localStorage.removeItem('access');
    localStorage.removeItem('role');
    localStorage.removeItem('userId');
    localStorage.removeItem('authToken');
    navigate('/login', { replace: true });
  };

  return (
    <>
    <header className="w-full px-6 py-4 flex items-center justify-between sticky top-0 z-50 bg-background-light/80 backdrop-blur-md">
      {/* Logo */}
      <div className="flex items-center gap-3">
        <div className="size-10 bg-primary rounded-full flex items-center justify-center text-white">
          <FontAwesomeIcon icon={faBook} className="text-[20px] text-white" />
        </div>
        <h2 className="text-xl font-bold tracking-tight hidden sm:block">
          Bookstore
        </h2>
      </div>

      {/* Actions */}
      <div className="flex items-center gap-3">
        <div className="relative" ref={settingsRef}>
          <button
            onClick={() => onToggleSettings(!showSettings)}
            className="flex items-center gap-2 px-4 h-10 rounded-full bg-white border border-gray-200 hover:border-primary/50 transition-colors shadow-sm group"
          >
            <FontAwesomeIcon icon={faCog} className="text-gray-500 group-hover:text-primary text-[20px]" />
            <span className="text-sm font-semibold hidden sm:inline">
              Settings
            </span>
          </button>

          {showSettings && (
            <div className="absolute right-0 mt-2 w-48 rounded-lg border border-gray-200 bg-white shadow-lg overflow-hidden z-50">
              <button
                type="button"
                onClick={() => handleNavigate('/account')}
                className="w-full flex items-center gap-2 px-4 py-3 text-sm font-semibold text-left hover:bg-primary/10 transition-colors"
              >
                <FontAwesomeIcon icon={faUser} className="text-gray-500" />
                Account
              </button>
              <button
                type="button"
                onClick={() => handleNavigate('/orders')}
                className="w-full flex items-center gap-2 px-4 py-3 text-sm font-semibold text-left hover:bg-primary/10 transition-colors"
              >
                <FontAwesomeIcon icon={faBoxOpen} className="text-gray-500" />
                Orders
              </button>
              <button
                type="button"
                onClick={() => setShowLogoutConfirm(true)}
                className="w-full flex items-center gap-2 px-4 py-3 text-sm font-semibold text-left hover:bg-red-100 text-red-600 transition-colors"
              >
                <FontAwesomeIcon icon={faArrowRightFromBracket} />
                Logout
              </button>
            </div>
          )}
        </div>

        <button
          onClick={() => navigate('/cart')}
          className="flex items-center gap-2 px-4 h-10 rounded-full bg-primary text-white font-bold hover:bg-primary/90 transition-colors shadow-sm relative"
        >
          <FontAwesomeIcon icon={faShoppingCart} className="text-[20px] text-white" />
          <span className="text-sm hidden sm:inline text-white">Cart</span>
          <span className="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-white text-primary text-[10px] font-bold ring-2 ring-background-light">
            {cartTotal}
          </span>
        </button>
      </div>
    </header>
    <LogoutConfirmation
      isOpen={showLogoutConfirm}
      onConfirm={handleLogout}
      onCancel={() => setShowLogoutConfirm(false)}
    />
    </>
  );
}

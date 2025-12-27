import React from 'react';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faHome, faArrowLeft } from '@fortawesome/free-solid-svg-icons';

export default function NotFound() {
  const navigate = useNavigate();

  return (
    <div className="light min-h-screen flex flex-col bg-background-light dark:bg-background-dark font-display text-text-main-light dark:text-text-main-dark transition-colors duration-300">
      <div className="flex-1 flex items-center justify-center px-4 py-20">
        <div className="text-center max-w-md">
          {/* 404 Number */}
          <div className="text-9xl font-black text-primary/20 mb-4">404</div>

          {/* Error Title */}
          <h1 className="text-4xl md:text-5xl font-black mb-4 tracking-tighter">
            Page Not Found
          </h1>

          {/* Error Description */}
          <p className="text-lg text-gray-600 dark:text-gray-400 mb-8 font-medium">
            Oops! The page you're looking for doesn't exist. It might have been moved or deleted.
          </p>

          {/* Action Buttons */}
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <button
              onClick={() => navigate(-1)}
              className="flex items-center justify-center gap-2 px-6 h-12 rounded-lg bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 hover:border-primary/50 transition-colors font-semibold text-text-main-light dark:text-text-main-dark shadow-sm"
            >
              <FontAwesomeIcon icon={faArrowLeft} />
              Go Back
            </button>
            <button
              onClick={() => navigate('/dashboard')}
              className="flex items-center justify-center gap-2 px-6 h-12 rounded-lg bg-primary text-background-dark hover:bg-primary/90 transition-colors font-bold shadow-sm"
            >
              <FontAwesomeIcon icon={faHome} />
              Back to Dashboard
            </button>
          </div>
        </div>
      </div>

      {/* Footer */}
      <footer className="w-full py-6 border-t border-gray-200 dark:border-gray-800">
        <div className="max-w-7xl mx-auto px-6 flex flex-col sm:flex-row justify-between items-center text-sm text-gray-500 dark:text-gray-400">
          <p>© 2025 Bookstore Inc. All rights reserved.</p>
          <div className="flex gap-4 mt-2 sm:mt-0">
            <a href="#" className="hover:text-primary">Help Center</a>
            <a href="#" className="hover:text-primary">Privacy</a>
            <a href="#" className="hover:text-primary">Terms</a>
          </div>
        </div>
      </footer>
    </div>
  );
}

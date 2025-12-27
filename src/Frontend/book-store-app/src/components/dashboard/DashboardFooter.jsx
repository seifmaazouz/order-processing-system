import React from 'react';

export default function DashboardFooter() {
  return (
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
  );
}

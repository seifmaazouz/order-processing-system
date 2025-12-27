import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSearch } from '@fortawesome/free-solid-svg-icons';

export default function SearchBar({ register, onSearch, isLoading }) {
  return (
    <div className="relative w-full max-w-3xl sm:flex-1 mx-auto sm:mx-0 group">
      <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none z-10">
        <FontAwesomeIcon icon={faSearch} className="text-gray-400 group-focus-within:text-primary" />
      </div>

      <input
        type="text"
        placeholder="Search by title or ISBN"
        {...register('searchQuery')}
        className="block w-full h-16 pl-12 pr-32 rounded-full bg-surface-light dark:bg-surface-dark shadow-lg ring-1 ring-gray-200 dark:ring-gray-700 focus:ring-2 focus:ring-primary text-lg placeholder:text-gray-400 dark:text-white transition-all"
      />

      <div className="absolute inset-y-1 right-1">
        <button
          type="button"
          onClick={onSearch}
          disabled={isLoading}
          className="h-14 px-8 rounded-full bg-primary text-background-light font-bold hover:bg-primary/90 transition-colors disabled:opacity-50"
        >
          {isLoading ? 'Searching...' : 'Search'}
        </button>
      </div>
    </div>
  );
}

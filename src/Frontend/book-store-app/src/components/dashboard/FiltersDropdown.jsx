import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faFilter } from '@fortawesome/free-solid-svg-icons';

export default function FiltersDropdown({
  showFilters,
  onToggleFilters,
  filtersRef,
  categories,
  selectedCategory,
  onSelectCategory,
  register,
  onReset,
  onApply,
  hasFiltersApplied,
}) {
  return (
    <div ref={filtersRef} className="relative">
      <button
        onClick={onToggleFilters}
        className={`flex items-center gap-2 h-16 sm:h-16 px-4 rounded-full border transition-colors shadow-sm w-full sm:w-auto ${
          hasFiltersApplied
            ? 'bg-primary/10 border-primary text-primary'
            : 'bg-surface-light dark:bg-surface-dark border-gray-200 dark:border-gray-700 hover:border-primary'
        }`}
      >
        <FontAwesomeIcon icon={faFilter} />
        <span className="text-sm font-semibold">Filters</span>
      </button>

      {showFilters && (
        <div className="absolute top-1/2 left-[90%] -translate-y-1/2 ml-3 transform w-80 max-h-[70vh] overflow-y-auto bg-white dark:bg-surface-dark border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg p-4 z-50">
          <div className="mb-3">
            <h4 className="text-sm font-semibold mb-2">Categories</h4>
            <div className="flex flex-wrap gap-2">
              {categories.map((item) => {
                const isActive = selectedCategory === item.label;
                return (
                  <button
                    key={item.label}
                    onClick={() => onSelectCategory(item.label)}
                    className={`flex items-center gap-2 h-8 px-3 rounded-full border text-sm transition-colors ${
                      isActive
                        ? 'bg-primary/10 border-primary text-primary'
                        : 'bg-surface-light dark:bg-surface-dark border-gray-200 dark:border-gray-700 hover:border-primary hover:text-primary'
                    }`}
                    aria-pressed={isActive}
                  >
                    <FontAwesomeIcon icon={item.icon} className="text-[14px]" />
                    {item.label}
                  </button>
                );
              })}
            </div>
          </div>

          <div className="mb-3">
            <h4 className="text-sm font-semibold mb-2">Author</h4>
            <input
              type="text"
              placeholder="Type author name"
              {...register('author')}
              className="w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
            />
          </div>

          <div>
            <h4 className="text-sm font-semibold mb-2">Publisher</h4>
            <input
              type="text"
              placeholder="Type publisher name"
              {...register('publisher')}
              className="w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
            />
          </div>

          <div className="mt-4 flex justify-between gap-3">
            <button
              onClick={onReset}
              className="h-10 px-4 rounded-md border border-gray-300 dark:border-gray-600 bg-surface-light dark:bg-surface-dark font-semibold hover:border-primary hover:text-primary transition-colors shadow-sm"
            >
              Reset
            </button>
            <button
              onClick={onApply}
              className="h-10 px-4 rounded-md bg-primary text-background-light font-semibold hover:bg-primary/90 transition-colors shadow-sm"
            >
              Apply
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

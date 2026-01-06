import React from 'react';
import SearchBar from './SearchBar.jsx';
import FiltersDropdown from './FiltersDropdown.jsx';

export default function DashboardSearch({
  register,
  onSearch,
  isLoading,
  showFilters,
  onToggleFilters,
  filtersRef,
  categories,
  selectedCategory,
  onSelectCategory,
  onReset,
  onApply,
  hasFiltersApplied,
}) {
  return (
    <div className="relative mt-8 flex flex-col sm:flex-row items-stretch sm:items-center gap-3 justify-center">
      <SearchBar
        register={register}
        onSearch={onSearch}
        isLoading={isLoading}
      />

      <FiltersDropdown
        showFilters={showFilters}
        onToggleFilters={onToggleFilters}
        filtersRef={filtersRef}
        categories={categories}
        selectedCategory={selectedCategory}
        onSelectCategory={onSelectCategory}
        register={register}
        onReset={onReset}
        onApply={onApply}
        hasFiltersApplied={hasFiltersApplied}
      />
    </div>
  );
}

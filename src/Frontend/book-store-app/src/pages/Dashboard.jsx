import React, { useState, useRef, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useSearchParams } from 'react-router-dom';
import Header from '../components/shared/Header.jsx';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBook, faCog, faShoppingCart, faSearch, faStar, faPalette, faSmile, faGlobe } from '@fortawesome/free-solid-svg-icons';
import { faFilter } from '@fortawesome/free-solid-svg-icons';
import { searchBooks } from '../api/search.api.js';

export default function Dashboard() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [showFilters, setShowFilters] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(() => {
    return searchParams.get('category') || '';
  });
  const [searchResults, setSearchResults] = useState([]);
  const [hasSearched, setHasSearched] = useState(false);
  const [lastQueryLabel, setLastQueryLabel] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const filtersRef = useRef(null);

  const { register, handleSubmit, watch, setValue } = useForm({
    defaultValues: {
      searchQuery: searchParams.get('search') || '',
      author: searchParams.get('author') || '',
      publisher: searchParams.get('publisher') || '',
    },
  });

  const hasFiltersApplied = Boolean(selectedCategory || watch('author') || watch('publisher'));

  const categories = [
    { icon: faStar, label: 'Science' },
    { icon: faPalette, label: 'Art' },
    { icon: faSmile, label: 'Religion' },
    { icon: faSmile, label: 'History' },
    { icon: faSmile, label: 'Geography' },
  
  ];

  useEffect(() => {
    function handleClick(e) {
      if (filtersRef.current && !filtersRef.current.contains(e.target)) {
        setShowFilters(false);
      }
    }
    document.addEventListener('click', handleClick);
    return () => document.removeEventListener('click', handleClick);
  }, []);

  const toggleCategory = (label) => {
    setSelectedCategory((prev) => prev === label ? '' : label);
  };

  const onSubmit = async (data) => {
    setIsLoading(true);
    setError(null);
    
    try {
      const query = {
        ...(data.searchQuery && { search: data.searchQuery }),
        ...(selectedCategory && { category: selectedCategory }),
        ...(data.author && { author: data.author }),
        ...(data.publisher && { publisher: data.publisher }),
      };

      // Update URL params
      const params = new URLSearchParams();
      if (data.searchQuery) params.set('search', data.searchQuery);
      if (selectedCategory) params.set('category', selectedCategory);
      if (data.author) params.set('author', data.author);
      if (data.publisher) params.set('publisher', data.publisher);
      setSearchParams(params);

      const summary = data.searchQuery ? `"${data.searchQuery}"` : 'all books';
      setLastQueryLabel(summary);

      const results = await searchBooks(query);
      setSearchResults(results);
      setHasSearched(true);
      setShowFilters(false);
    } catch (err) {
      setError(err.message || 'Failed to search books');
      console.error('Search error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  // Trigger search on mount if URL params exist
  useEffect(() => {
    const hasParams = searchParams.get('search') || searchParams.get('category') ||
      searchParams.get('author') || searchParams.get('publisher');
    if (hasParams) {
      handleSubmit(onSubmit)();
    }
  }, []); // Only run on mount
  

  const handleApplyFilters = () => {
    // Just save/close filters without searching
    setShowFilters(false);
    setError(null);
  };

  const handleResetFilters = () => {
    setSelectedCategory('');
    setValue('author', '');
    setValue('publisher', '');
    setError(null);
  };

  return (
    <div className="light min-h-screen flex flex-col bg-background-light dark:bg-background-dark font-display text-text-main-light dark:text-text-main-dark transition-colors duration-300">

      {/* Top Navigation */}
      <header className="w-full px-6 py-4 flex items-center justify-between sticky top-0 z-50 bg-background-light/80 dark:bg-background-dark/80 backdrop-blur-md">

        {/* Logo */}
        <div className="flex items-center gap-3">
          <div className="size-10 bg-primary rounded-full flex items-center justify-center text-background-dark">
            <FontAwesomeIcon icon={faBook} className="text-[20px] text-background-dark" />
          </div>
          <h2 className="text-xl font-bold tracking-tight hidden sm:block">
            Bookstore
          </h2>
        </div>

        {/* Actions */}
        <div className="flex items-center gap-3">
          <button className="flex items-center gap-2 px-4 h-10 rounded-full bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 hover:border-primary/50 transition-colors shadow-sm group">
            <FontAwesomeIcon icon={faCog} className="text-gray-500 dark:text-gray-400 group-hover:text-primary text-[20px]" />
            <span className="text-sm font-semibold hidden sm:inline">
              Settings
            </span>
          </button>

          <button className="flex items-center gap-2 px-4 h-10 rounded-full bg-primary text-background-dark font-bold hover:bg-primary/90 transition-colors shadow-sm relative">
            <FontAwesomeIcon icon={faShoppingCart} className="text-[20px]" />
            <span className="text-sm hidden sm:inline text-background-dark">Cart</span>
            <span className="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-background-dark text-white text-[10px] ring-2 ring-background-light dark:ring-background-dark">
              2
            </span>
          </button>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col justify-center py-10">
        {/* Hero / Search */}
        <div className="w-full max-w-4xl mx-auto text-center mb-16">
          <h1 className="text-4xl md:text-5xl lg:text-6xl font-black mb-4 tracking-tighter">
            Find your next story
          </h1>

          <p className="text-lg text-gray-600 dark:text-gray-400 mb-10 max-w-2xl mx-auto font-medium">
            Discover worlds within pages. Search for your favorite titles, authors, or genres.
          </p>

          {/* Search + Filters */}
          <div className="relative mt-8 flex flex-col sm:flex-row items-stretch sm:items-center gap-3 justify-center">
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
                  onClick={handleSubmit(onSubmit)}
                  disabled={isLoading}
                  className="h-14 px-8 rounded-full bg-primary text-background-light font-bold hover:bg-primary/90 transition-colors disabled:opacity-50"
                >
                  {isLoading ? 'Searching...' : 'Search'}
                </button>

              </div>
            </div>

            <div className="" ref={filtersRef}>
              <button
                onClick={() => setShowFilters(s => !s)}
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
                <div className="absolute mt-2 right-[-313px] top-[-171px] w-80 max-h-[70vh] overflow-y-auto bg-white dark:bg-surface-dark border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg p-4 z-50">
                  <div className="mb-3">
                    <h4 className="text-sm font-semibold mb-2">Categories</h4>
                    <div className="flex flex-wrap gap-2">
                      {categories.map((item) => {
                        const isActive = selectedCategory === item.label;
                        return (
                          <button
                            key={item.label}
                            onClick={() => toggleCategory(item.label)}
                            className={`flex items-center gap-2 h-8 px-3 rounded-full border text-sm transition-colors ${isActive
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
                      onClick={handleResetFilters}
                      className="h-10 px-4 rounded-md border border-gray-300 dark:border-gray-600 bg-surface-light dark:bg-surface-dark font-semibold hover:border-primary hover:text-primary transition-colors shadow-sm"
                    >
                      Reset
                    </button>
                    <button
                      onClick={handleApplyFilters}
                      className="h-10 px-4 rounded-md bg-primary text-background-light font-semibold hover:bg-primary/90 transition-colors shadow-sm"
                    >
                      Apply
                    </button>
                  </div>
                </div>
              )}
            </div>




          </div>
          {error && (
            <div className="mt-4 text-sm text-red-600 dark:text-red-400 text-center">
              Error: {error}
            </div>
          )}
          {hasSearched && searchResults.length === 0 && (
            <div className="mt-4 text-lg text-red-600 dark:text-red-400 text-center font-semibold">
              No results found
            </div>
          )}
          {searchResults.length > 0 && (
            <div className="mt-8">
              <h3 className="text-2xl font-bold mb-4">
                Search for {lastQueryLabel || 'your query'} ({searchResults.length} results)
              </h3>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                {searchResults.map((book) => (
                  <div
                    key={book.id || book.isbn}
                    className="p-4 rounded-lg bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 hover:border-primary transition-colors"
                  >
                    <h4 className="font-bold text-lg mb-2">{book.title}</h4>
                    {book.author && <p className="text-sm text-gray-600 dark:text-gray-400">Author: {book.author}</p>}
                    {book.isbn && <p className="text-sm text-gray-600 dark:text-gray-400">ISBN: {book.isbn}</p>}
                    {book.price && <p className="text-sm font-semibold text-primary mt-2">${book.price}</p>}
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </main>

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
  )
}

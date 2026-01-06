import React, { useState, useRef, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useSearchParams } from 'react-router-dom';
import { searchBooks } from '../../api/search.api.js';
import DashboardHeader from '../../components/dashboard/DashboardHeader.jsx';
import DashboardHero from '../../components/dashboard/DashboardHero.jsx';
import DashboardSearch from '../../components/dashboard/DashboardSearch.jsx';
import DashboardFooter from '../../components/dashboard/DashboardFooter.jsx';
import ResultsGrid from '../../components/dashboard/ResultsGrid.jsx';
import { useCart } from '../../context/CartContext.jsx';
import { useNavigate } from 'react-router-dom';
import { dashboardCategories } from '../../constants/categories.js';

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
  const [showSettings, setShowSettings] = useState(false);
  const settingsRef = useRef(null);
  const navigate = useNavigate();
  const { summary, cartCount } = useCart();

  // Function to update book stock levels after cart operations
  const updateBookStock = (bookId, newStock) => {
    setSearchResults(prevResults =>
      prevResults.map(book => {
        const currentBookId = book.ISBN || book.isbn || book.id;
        if (currentBookId === bookId) {
          return { ...book, Quantity: newStock, quantity: newStock, Stock: newStock, stock: newStock, stockLevel: newStock };
        }
        return book;
      })
    );
  };

  const { register, handleSubmit, watch, setValue } = useForm({
    defaultValues: {
      searchQuery: searchParams.get('search') || '',
      author: searchParams.get('author') || '',
      publisher: searchParams.get('publisher') || '',
    },
  });

  const hasFiltersApplied = Boolean(selectedCategory || watch('author') || watch('publisher'));

  useEffect(() => {
    function handleClick(e) {
      const clickOutsideFilters = filtersRef.current && !filtersRef.current.contains(e.target);
      const clickOutsideSettings = settingsRef.current && !settingsRef.current.contains(e.target);

      if (clickOutsideFilters) {
        setShowFilters(false);
      }
      if (clickOutsideSettings) {
        setShowSettings(false);
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
      // Normalize book data: backend returns PascalCase, frontend expects camelCase
      const normalizedResults = Array.isArray(results) ? results.map(book => ({
        id: book.ISBN || book.isbn || book.id,
        isbn: book.ISBN || book.isbn,
        title: book.Title || book.title,
        year: book.Year || book.year,
        price: book.Price || book.price,
        stock: book.Stock || book.stock,
        stockLevel: book.Stock || book.stock || book.stockLevel,
        category: book.Category || book.category,
        publisher: book.Publisher || book.publisher,
        authors: book.Authors || book.authors || [],
        isAvailable: book.IsAvailable !== undefined ? book.IsAvailable : (book.Stock || book.stock || 0) > 0
      })) : [];
      setSearchResults(normalizedResults);
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
  }, []);

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
    <div className="min-h-screen flex flex-col bg-background-light font-display text-text-main transition-colors duration-300">
      <DashboardHeader
        showSettings={showSettings}
        onToggleSettings={setShowSettings}
        settingsRef={settingsRef}
        cartTotal={cartCount}
      />

      <main className="flex-1 w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col justify-center py-10">
        <DashboardHero />

        <div className="w-full max-w-4xl mx-auto">
          <DashboardSearch
            register={register}
            onSearch={() => handleSubmit(onSubmit)()}
            isLoading={isLoading}
            showFilters={showFilters}
            onToggleFilters={() => setShowFilters((s) => !s)}
            filtersRef={filtersRef}
            categories={dashboardCategories}
            selectedCategory={selectedCategory}
            onSelectCategory={toggleCategory}
            onReset={handleResetFilters}
            onApply={handleApplyFilters}
            hasFiltersApplied={hasFiltersApplied}
          />

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

          <ResultsGrid results={searchResults} lastQueryLabel={lastQueryLabel} onStockUpdate={updateBookStock} />
        </div>
      </main>

      <DashboardFooter />
    </div>
  );
}

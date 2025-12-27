import React, { useEffect, useRef, useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import {
	faThLarge,
	faBookOpen,
	faShoppingBag,
	faUsers,
	faChartBar,
	faArrowRightFromBracket,
	faFilter,
	faPlus,
} from '@fortawesome/free-solid-svg-icons';
import BookCard from '../../components/dashboard/BookCard.jsx';
import SearchBar from '../../components/dashboard/SearchBar.jsx';
import FiltersDropdown from '../../components/dashboard/FiltersDropdown.jsx';
import { searchBooks } from '../../api/search.api.js';
import { useForm } from 'react-hook-form';

export default function Admin() {
	const [books, setBooks] = useState([]);
	const [showFilters, setShowFilters] = useState(false);
	const [selectedCategory, setSelectedCategory] = useState('');
	const [isLoading, setIsLoading] = useState(false);
	const [error, setError] = useState(null);
	const filtersRef = useRef(null);
	const { register, handleSubmit, watch, setValue } = useForm({
		defaultValues: {
			searchQuery: '',
			author: '',
			publisher: '',
		},
	});

	const hasFiltersApplied = Boolean(selectedCategory || watch('author') || watch('publisher'));

	const categories = [
		{ icon: faBookOpen, label: 'Fiction' },
		{ icon: faChartBar, label: 'Business' },
		{ icon: faUsers, label: 'Self-Help' },
		{ icon: faShoppingBag, label: 'Tech' },
		{ icon: faThLarge, label: 'All' },
	];

	const statusMap = {
		'in-stock': { label: 'In Stock', color: 'bg-green-100 text-green-700' },
		'low-stock': { label: 'Low Stock', color: 'bg-orange-100 text-orange-700' },
		'out-of-stock': { label: 'Out of Stock', color: 'bg-gray-200 text-gray-600' },
	};

	const fetchBooks = async (formData) => {
		setIsLoading(true);
		setError(null);
		try {
			const query = {
				...(formData.searchQuery && { search: formData.searchQuery }),
				...(selectedCategory && { category: selectedCategory }),
				...(formData.author && { author: formData.author }),
				...(formData.publisher && { publisher: formData.publisher }),
			};
			const results = await searchBooks(query);
			setBooks(results);
		} catch (err) {
			setError(err.message || 'Failed to load books');
		} finally {
			setIsLoading(false);
		}
	};

	useEffect(() => {
		fetchBooks({ searchQuery: '', author: '', publisher: '' });
	}, []);

	useEffect(() => {
		function handleClick(e) {
			const clickOutsideFilters = filtersRef.current && !filtersRef.current.contains(e.target);
			if (clickOutsideFilters) {
				setShowFilters(false);
			}
		}
		document.addEventListener('click', handleClick);
		return () => document.removeEventListener('click', handleClick);
	}, []);

	const toggleCategory = (label) => {
		setSelectedCategory((prev) => (prev === label ? '' : label));
	};

	const handleApplyFilters = () => {
		handleSubmit(fetchBooks)();
		setShowFilters(false);
	};

	const handleResetFilters = () => {
		setSelectedCategory('');
		setValue('author', '');
		setValue('publisher', '');
		setError(null);
		setShowFilters(false);
	};

	const handleSelect = () => {};
	const handleEdit = (book) => {
		console.log('Edit book:', book);
		// TODO: Implement edit modal/form
	};

	const handleRemove = (book) => {
		console.log('Remove book:', book);
		// TODO: Implement delete confirmation
	};

	return (
		<div className="light bg-background-light dark:bg-background-dark font-display text-text-main dark:text-gray-100 antialiased overflow-hidden">
			<div className="flex h-screen w-full">
				{/* Side Navigation */}
				<aside className="hidden md:flex w-72 flex-col justify-between border-r border-[#e6e0db] dark:border-[#443628] bg-background-light dark:bg-background-dark p-6 transition-all">
					<div className="flex flex-col gap-8">
						<div className="flex flex-col gap-1 px-2">
							<h1 className="text-2xl font-black tracking-tighter text-text-main dark:text-white">Chapter One</h1>
							<p className="text-text-secondary text-sm font-medium">Admin Dashboard</p>
						</div>
						<nav className="flex flex-col gap-2">
							<a href="#" className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main dark:text-gray-200 hover:bg-[#efe9e3] dark:hover:bg-[#3a2d20] transition-colors">
								<FontAwesomeIcon icon={faThLarge} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Dashboard</p>
							</a>
							<a href="#" className="flex items-center gap-3 px-4 py-3 rounded-full bg-[#f4ede7] dark:bg-[#3a2d20] shadow-sm">
								<FontAwesomeIcon icon={faBookOpen} className="text-text-main dark:text-primary" />
								<p className="text-sm font-bold text-text-main dark:text-white">Inventory</p>
							</a>
							<a href="#" className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main dark:text-gray-200 hover:bg-[#efe9e3] dark:hover:bg-[#3a2d20] transition-colors">
								<FontAwesomeIcon icon={faShoppingBag} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Orders</p>
							</a>
							<a href="#" className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main dark:text-gray-200 hover:bg-[#efe9e3] dark:hover:bg-[#3a2d20] transition-colors">
								<FontAwesomeIcon icon={faUsers} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Customers</p>
							</a>
							<a href="#" className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main dark:text-gray-200 hover:bg-[#efe9e3] dark:hover:bg-[#3a2d20] transition-colors">
								<FontAwesomeIcon icon={faChartBar} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Analytics</p>
							</a>
						</nav>
					</div>
					<button className="flex w-full cursor-pointer items-center justify-center gap-2 overflow-hidden rounded-full h-12 px-6 bg-primary/10 hover:bg-primary/20 dark:bg-primary/20 dark:hover:bg-primary/30 text-text-main dark:text-primary text-sm font-bold transition-colors">
						<FontAwesomeIcon icon={faArrowRightFromBracket} className="text-[18px]" />
						<span className="truncate">Log Out</span>
					</button>
				</aside>

				{/* Main Content */}
				<main className="flex-1 flex flex-col h-full overflow-hidden relative">
					{/* Page Heading */}
					<header className="w-full px-8 py-8 md:py-10 bg-background-light dark:bg-background-dark z-10">
						<div className="flex flex-col gap-4 max-w-[1400px] mx-auto">
							<div className="flex flex-col gap-2">
								<h2 className="text-text-main dark:text-white text-4xl font-black tracking-tight leading-tight">Inventory Management</h2>
								<p className="text-text-secondary text-base dark:text-gray-400 font-normal">View and manage your book catalog</p>
							</div>
							<div className="flex flex-col w-full gap-4">
								<div className="relative flex flex-col lg:flex-row items-center gap-3 w-full">
									<SearchBar register={register} onSearch={handleSubmit(fetchBooks)} isLoading={isLoading} />
									<div className="relative">
										<FiltersDropdown
											showFilters={showFilters}
											onToggleFilters={() => setShowFilters((s) => !s)}
											filtersRef={filtersRef}
											categories={categories}
											selectedCategory={selectedCategory}
											onSelectCategory={toggleCategory}
											register={register}
											onReset={handleResetFilters}
											onApply={handleApplyFilters}
											hasFiltersApplied={hasFiltersApplied}
										/>
									</div>
									<button className="flex cursor-pointer items-center justify-center gap-2 overflow-hidden rounded-full h-16 px-6 bg-primary hover:bg-primary/90 text-[#1c140d] shadow-lg shadow-orange-500/20 text-sm font-bold tracking-wide transition-all transform active:scale-95 whitespace-nowrap lg:ml-auto">
										<FontAwesomeIcon icon={faPlus} className="text-[18px]" />
										<span>Add New Book</span>
									</button>
								</div>
								{error && (
									<div className="text-sm text-red-600 dark:text-red-400">{error}</div>
								)}
							</div>
						</div>
					</header>

					{/* Scrollable Content Area */}
					<div className="flex-1 overflow-y-auto px-6 pb-12 scroll-smooth">
						<div className="max-w-[1400px] mx-auto">
							{/* Product Grid */}
							<div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
								{books.map((book) => (
									<BookCard
										key={book.id}
										book={book}
										status={statusMap[book.status] || statusMap['in-stock']}
										onSelect={handleSelect}
										onEdit={handleEdit}
										onRemove={handleRemove}
										isLoading={isLoading}
										isAdminMode={true}
									/>
								))}
								{!isLoading && books.length === 0 && (
									<div className="col-span-full text-center text-gray-500 dark:text-gray-400 font-medium py-10">
										No books found. Try adjusting filters.
									</div>
								)}
							</div>
						</div>
					</div>
				</main>
			</div>
		</div>
	);
}


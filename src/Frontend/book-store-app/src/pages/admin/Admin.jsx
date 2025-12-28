import React, { useEffect, useRef, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBookOpen, faShoppingBag, faChartBar, faArrowRightFromBracket, faFilter, faPlus } from '@fortawesome/free-solid-svg-icons';
import BookCard from '../../components/dashboard/BookCard.jsx';
import SearchBar from '../../components/dashboard/SearchBar.jsx';
import FiltersDropdown from '../../components/dashboard/FiltersDropdown.jsx';
import { searchBooks } from '../../api/search.api.js';
import { addBook, editBook, removeBook } from '../../api/books.api.js';
import { useForm } from 'react-hook-form';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { dashboardCategories, categoryOptions } from '../../constants/categories.js';

export default function Admin() {
	const navigate = useNavigate();
	const [books, setBooks] = useState([]);
	const [showFilters, setShowFilters] = useState(false);
		const [searchParams, setSearchParams] = useSearchParams();
		const [selectedCategory, setSelectedCategory] = useState(() => searchParams.get('category') || '');
	const [isLoading, setIsLoading] = useState(false);
	const [error, setError] = useState(null);
	const [editingBook, setEditingBook] = useState(null);
	const [showEditModal, setShowEditModal] = useState(false);
	const [removingBook, setRemovingBook] = useState(null);
	const [showRemoveModal, setShowRemoveModal] = useState(false);
	const [showAddModal, setShowAddModal] = useState(false);
	const filtersRef = useRef(null);
	
	const { register, handleSubmit, watch, setValue } = useForm({
		defaultValues: {
			searchQuery: searchParams.get('search') || '',
			author: searchParams.get('author') || '',
			publisher: searchParams.get('publisher') || '',
		},
	});
	
	const { register: registerAdd, handleSubmit: handleSubmitAdd, watch: watchAdd, setValue: setValueAdd, reset: resetAdd, formState: { errors: errorsAdd } } = useForm({
		defaultValues: {
			title: '',
			sellingPrice: '',
			stock: 0,
			authors: [''],
			category: '',
			pubID: '',
			isbn: '',
			threshold: 0,
			publicationYear: '',
		},
	});
	
	const { register: registerEdit, handleSubmit: handleSubmitEdit, watch: watchEdit, setValue: setValueEdit, reset: resetEdit, formState: { errors: errorsEdit } } = useForm({
		defaultValues: {
			title: '',
			sellingPrice: '',
			quantity: 0,
			threshold: 0,
			year: 0,
			authors: [],
			category: '',
			publisher: '',
			isbn: '',
		},
	});

	// Logout: clear auth data and redirect to login
	const handleLogout = () => {
		localStorage.removeItem('access');
		localStorage.removeItem('role');
		localStorage.removeItem('userId');
		navigate('/login', { replace: true });
	};

	const hasFiltersApplied = Boolean(selectedCategory || watch('author') || watch('publisher'));

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

			const params = new URLSearchParams();
			if (formData.searchQuery) params.set('search', formData.searchQuery);
			if (selectedCategory) params.set('category', selectedCategory);
			if (formData.author) params.set('author', formData.author);
			if (formData.publisher) params.set('publisher', formData.publisher);
			setSearchParams(params);

			const results = await searchBooks(query);
			setBooks(results);
		} catch (err) {
			setError(err.message || 'Failed to load books');
		} finally {
			setIsLoading(false);
		}
	};

	useEffect(() => {
		handleSubmit(fetchBooks)();
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, []);

	useEffect(() => {
		function handleClick(e) {
			// Don't close filters if clicking inside any modal
			const isClickInModal = e.target.closest('.fixed.inset-0.z-50');
			if (isClickInModal) {
				return;
			}
			
			// Only handle filter dropdown clicks when it's actually visible
			if (showFilters && filtersRef.current && !filtersRef.current.contains(e.target)) {
				setShowFilters(false);
			}
		}
		document.addEventListener('click', handleClick);
		return () => document.removeEventListener('click', handleClick);
	}, [showFilters]);

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
		setSearchParams(new URLSearchParams());
		setShowFilters(false);
	};

	const handleSelect = () => {};

	const handleEdit = (book) => {
		setEditingBook(book);
		resetEdit({
			title: book.title ?? '',
			sellingPrice: book.price ?? '',
			quantity: book.stock ?? 0,
			threshold: book.threshold ?? 0,
			year: book.year ?? 0,
			authors: book.authors ?? [],
			category: book.category ?? '',
			publisher: book.publisher ?? '',
			isbn: book.isbn ?? '',
		});
		setShowEditModal(true);
	};

	const handleSaveEdit = handleSubmitEdit(async (formData) => {
		if (!editingBook) return;
		const sellingPrice = formData.sellingPrice ? Math.max(0, Number(formData.sellingPrice)) : null;
		const quantity = formData.quantity !== '' ? Math.max(0, Number(formData.quantity)) : null;
		const threshold = formData.threshold !== '' ? Math.max(0, Number(formData.threshold)) : null;
		const validAuthors = formData.authors.filter(a => a.trim() !== '');
		try {
			const payload = {
				...(formData.title && { title: formData.title }),
				...(sellingPrice !== null && sellingPrice > 0 && { sellingPrice }),
				...(quantity !== null && { quantity }),
				...(threshold !== null && { threshold }),
				...(formData.year && { publicationYear: Number(formData.year) }),
				...(validAuthors.length > 0 && { authors: validAuthors }),
				...(formData.publisher && { publisher: formData.publisher }),
			};
			const updated = await editBook(editingBook.isbn, payload);
			const updatedBook = {
				...editingBook,
				title: formData.title || editingBook.title,
				price: sellingPrice || editingBook.price,
				stock: quantity !== null ? quantity : editingBook.stock,
				threshold: formData.threshold !== null ? formData.threshold : editingBook.threshold,
				year: formData.year || editingBook.year,
				authors: validAuthors.length > 0 ? validAuthors : editingBook.authors,
				publisher: formData.publisher || editingBook.publisher,
			};
			setBooks((prev) =>
				prev.map((b) =>
					b.id === editingBook.id || b.isbn === editingBook.isbn
						? updatedBook
						: b
				)
			);
			toast.success('Book updated successfully');
			setShowEditModal(false);
			setEditingBook(null);
		} catch (err) {
			const message = err?.response?.data?.error || err?.response?.data?.message || err?.message || 'Failed to update book';
			toast.error(message);
			console.error('Edit error:', err.response?.data);
		}
	});

	const computeStatus = (qty) => {
		if (qty > 5) return 'in-stock';
		if (qty > 0) return 'low-stock';
		return 'out-of-stock';
	};

	const handleOpenAdd = () => {
		resetAdd();
		setShowAddModal(true);
	};



	const handleAuthorChange = (index, value) => {
		const authors = [...watchAdd('authors')];
		authors[index] = value;
		setValueAdd('authors', authors);
	};

	const handleAddAuthor = () => {
		setValueAdd('authors', [...watchAdd('authors'), '']);
	};

	const handleRemoveAuthorField = (index) => {
		const authors = watchAdd('authors');
		if (authors.length === 1) return;
		setValueAdd('authors', authors.filter((_, i) => i !== index));
	};

	const handleEditAuthorChange = (index, value) => {
		const authors = [...watchEdit('authors')];
		authors[index] = value;
		setValueEdit('authors', authors);
	};

	const handleEditAddAuthor = () => {
		setValueEdit('authors', [...watchEdit('authors'), '']);
	};

	const handleEditRemoveAuthorField = (index) => {
		const authors = watchEdit('authors');
		if (authors.length === 1) return;
		setValueEdit('authors', authors.filter((_, i) => i !== index));
	};

	const handleSaveNewBook = handleSubmitAdd(async (formData) => {
		try {
			const validAuthors = formData.authors.filter(author => author.trim() !== '');
			const payload = {
				...(formData.title && { title: formData.title }),
				...(formData.sellingPrice && Number(formData.sellingPrice) > 0 && { sellingPrice: Number(formData.sellingPrice) }),
				...(formData.stock >= 0 && { quantity: Number(formData.stock) }),
				...(formData.isbn && { isbn: formData.isbn }),
				...(formData.category && { category: formData.category }),
				...(formData.pubID && { pubID: Number(formData.pubID) }),
				...(validAuthors.length > 0 && { authors: validAuthors }),
				...(formData.threshold >= 0 && { threshold: Number(formData.threshold) }),
				...(formData.publicationYear && { publicationYear: Number(formData.publicationYear) }),
			};
			await addBook(payload);
			toast.success('Book added successfully!');
			setShowAddModal(false);
			resetAdd();
			handleSubmit(fetchBooks)();
		} catch (error) {
			const errorMsg = error.response?.data?.error || error.response?.data?.message || error.message || 'Failed to add book';
			toast.error(errorMsg);
			console.error('Add book error:', error.response?.data);
		}
	});

const handleRemove = (book) => {
	setRemovingBook(book);
	setShowRemoveModal(true);
};

const handleConfirmRemove = async () => {
		if (!removingBook) return;
		try {
			await removeBook(removingBook.isbn);
			toast.success('Book removed successfully');
			setShowRemoveModal(false);
			setRemovingBook(null);
			// Refetch books to ensure state is in sync with backend
			handleSubmit(fetchBooks)();
		} catch (err) {
			const message = err?.response?.data?.error || err?.response?.data?.message || err?.message || 'Failed to remove book';
			toast.error(message);
			console.error('Delete error:', err.response?.data);
		}
	};

	const handleCancelDialogs = () => {
		setShowEditModal(false);
		setShowRemoveModal(false);
		setShowAddModal(false);
		setEditingBook(null);
		setRemovingBook(null);
	};

	return (
		<div className="light bg-background-light dark:bg-background-dark font-display text-text-main dark:text-gray-100 antialiased overflow-hidden">
			<div className="flex h-screen w-full">
				<ToastContainer position="top-right" autoClose={3000} hideProgressBar theme="colored" />
				{/* Side Navigation */}
				<aside className="hidden md:flex w-72 flex-col justify-between border-r border-[#e6e0db] dark:border-[#443628] bg-background-light dark:bg-background-dark p-6 transition-all">
					<div className="flex flex-col gap-8">
						<div className="flex flex-col gap-1 px-2">
							
							<p className="text-text-secondary text-sm font-medium">Admin Dashboard</p>
						</div>
						<nav className="flex flex-col gap-2">

							<button onClick={() => navigate('/admin')} className="flex items-center gap-3 px-4 py-3 rounded-full bg-[#f4ede7] dark:bg-[#3a2d20] shadow-sm w-full text-left">
								<FontAwesomeIcon icon={faBookOpen} className="text-text-main dark:text-primary" />
								<p className="text-sm font-bold text-text-main dark:text-white">Inventory</p>
							</button>
							<button onClick={() => navigate('/admin/orders')} className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main dark:text-gray-200 hover:bg-[#efe9e3] dark:hover:bg-[#3a2d20] transition-colors w-full text-left">
								<FontAwesomeIcon icon={faShoppingBag} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Orders</p>
							</button>
							<button onClick={() => navigate('/admin/analytics')} className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main dark:text-gray-200 hover:bg-[#efe9e3] dark:hover:bg-[#3a2d20] transition-colors w-full text-left">
								<FontAwesomeIcon icon={faChartBar} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Analytics</p>
							</button>
						</nav>
					</div>
					<button
						onClick={handleLogout}
						className="flex w-full cursor-pointer items-center justify-center gap-2 overflow-hidden rounded-full h-12 px-6 bg-primary/10 hover:bg-primary/20 dark:bg-primary/20 dark:hover:bg-primary/30 text-text-main dark:text-primary text-sm font-bold transition-colors"
					>
						<FontAwesomeIcon icon={faArrowRightFromBracket} className="text-[18px]" />
						<span className="truncate">Log Out</span>
					</button>
				</aside>

				{/* Edit Modal */}
				{showEditModal && (
					<div 
						className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm px-4"
						onClick={(e) => {
							// Close modal if clicking the backdrop
							if (e.target === e.currentTarget) {
								handleCancelDialogs();
							}
						}}
					>
						<div 
							className="w-full max-w-lg rounded-2xl bg-white dark:bg-surface-dark p-6 shadow-2xl border border-gray-200 dark:border-gray-700"
							onClick={(e) => e.stopPropagation()}
						>
							<h3 className="text-xl font-bold mb-4 text-text-main dark:text-white">Edit Book</h3>
							<div className="space-y-4">
								<div>
									<label className="text-sm font-semibold">Title<span className="text-red-500">*</span></label>
									<input
										type="text"
										{...registerEdit('title', { required: 'Title is required' })}
										className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
									/>
									{errorsEdit.title && <p className="text-red-500 text-xs mt-1">{errorsEdit.title.message}</p>}
								</div>
								<div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
									<div>
										<label className="text-sm font-semibold">Selling Price<span className="text-red-500">*</span></label>
										<input
											min="0"
											type="number"
											step="0.01"
											{...registerEdit('sellingPrice', { 
												required: 'Selling price is required',
												min: { value: 0.01, message: 'Price must be greater than 0' }
											})}
											className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
										/>
										{errorsEdit.sellingPrice && <p className="text-red-500 text-xs mt-1">{errorsEdit.sellingPrice.message}</p>}
									</div>
									<div>
										<label className="text-sm font-semibold">Year<span className="text-red-500">*</span></label>
										<input
											type="number"
											min="1000"
											max={new Date().getFullYear()}
											{...registerEdit('year', { 
												required: 'Publication year is required',
												valueAsNumber: true,
												min: { value: 1000, message: 'Year must be at least 1000' },
												max: { value: new Date().getFullYear(), message: `Year cannot exceed ${new Date().getFullYear()}` }
											})}
											className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
										/>
										{errorsEdit.year && <p className="text-red-500 text-xs mt-1">{errorsEdit.year.message}</p>}
									</div>
								</div>
								<div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
									<div>
										<label className="text-sm font-semibold">Quantity<span className="text-red-500">*</span></label>
										<input
											type="number"
											min="0"
											{...registerEdit('quantity', { 
												required: 'Quantity is required',
												valueAsNumber: true,
												min: { value: 0, message: 'Quantity cannot be negative' }
											})}
											className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
										/>
										{errorsEdit.quantity && <p className="text-red-500 text-xs mt-1">{errorsEdit.quantity.message}</p>}
									</div>
									<div>
										<label className="text-sm font-semibold">Threshold<span className="text-red-500">*</span></label>
										<input
											type="number"
											min="0"
											{...registerEdit('threshold', { 
												required: 'Threshold is required',
												valueAsNumber: true,
												min: { value: 0, message: 'Threshold cannot be negative' }
											})}
											className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
										/>
										{errorsEdit.threshold && <p className="text-red-500 text-xs mt-1">{errorsEdit.threshold.message}</p>}
									</div>
								</div>

								{/* Publisher Field */}
								<div className="mt-4">
									<label className="text-sm font-semibold">Publisher<span className="text-red-500">*</span></label>
									<input
										type="text"
										{...registerEdit('publisher', { required: 'Publisher is required' })}
										className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
									/>
									{errorsEdit.publisher && <p className="text-red-500 text-xs mt-1">{errorsEdit.publisher.message}</p>}
								</div>

								{/* Authors Field */}
								<div className="mt-4">
									<label className="text-sm font-semibold">Authors<span className="text-red-500">*</span></label>
									{watchEdit('authors').map((author, index) => (
										<div key={index} className="flex gap-2 mt-2">
											<input
												type="text"
												value={author}
												onChange={(e) => handleEditAuthorChange(index, e.target.value)}
												placeholder={`Author ${index + 1}`}
												className="flex-1 h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
											/>
											{watchEdit('authors').length > 1 && (
												<button
													type="button"
													onClick={() => handleEditRemoveAuthorField(index)}
													className="px-3 py-2 rounded-md bg-red-500 text-white hover:bg-red-600"
												>
													Remove
												</button>
											)}
										</div>
									))}
									<button
										type="button"
										onClick={handleEditAddAuthor}
										className="mt-2 px-4 py-2 rounded-md bg-primary text-white hover:bg-primary/90"
									>
										Add Author
									</button>
									{watchEdit('authors').some(a => a.trim() === '') && (
										<p className="text-red-500 text-xs mt-1">All author fields must be filled or removed</p>
									)}
								</div>
							</div>
							<div className="mt-6 flex justify-end gap-3">
								<button
									onClick={handleCancelDialogs}
									className="px-4 py-2 rounded-full border border-gray-300 dark:border-gray-600 bg-surface-light dark:bg-surface-dark text-sm font-semibold hover:border-primary hover:text-primary"
								>
									Cancel
								</button>
								<button
									onClick={handleSaveEdit}
									className="px-4 py-2 rounded-full bg-primary text-white text-sm font-semibold hover:bg-primary/90"
								>
									Save Changes
								</button>
							</div>
						</div>
					</div>
				)}

				{/* Remove Confirmation */}
				{showRemoveModal && (
					<div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm px-4">
						<div className="w-full max-w-md rounded-2xl bg-white dark:bg-surface-dark p-6 shadow-2xl border border-gray-200 dark:border-gray-700">
							<h3 className="text-xl font-bold mb-3 text-text-main dark:text-white">Remove Book</h3>
							<p className="text-sm text-text-secondary dark:text-gray-300 mb-6">
								Are you sure you want to remove
								{' '}
								<span className="font-semibold">{removingBook?.title}</span>?
							</p>
							<div className="flex justify-end gap-3">
								<button
									onClick={handleCancelDialogs}
									className="px-4 py-2 rounded-full border border-gray-300 dark:border-gray-600 bg-surface-light dark:bg-surface-dark text-sm font-semibold hover:border-primary hover:text-primary"
								>
									Cancel
								</button>
								<button
									onClick={handleConfirmRemove}
									className="px-4 py-2 rounded-full bg-red-500 text-white text-sm font-semibold hover:bg-red-600"
								>
									Remove
								</button>
							</div>
						</div>
					</div>
				)}

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
													categories={dashboardCategories}
											selectedCategory={selectedCategory}
											onSelectCategory={toggleCategory}
											register={register}
											onReset={handleResetFilters}
											onApply={handleApplyFilters}
											hasFiltersApplied={hasFiltersApplied}
										/>
									</div>
									<button onClick={handleOpenAdd} className="flex cursor-pointer items-center justify-center gap-2 overflow-hidden rounded-full h-16 px-6 bg-primary hover:bg-primary/90 text-[#1c140d] shadow-lg shadow-orange-500/20 text-sm font-bold tracking-wide transition-all transform active:scale-95 whitespace-nowrap lg:ml-auto">
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

				{/* Add Book Modal */}
				{showAddModal && (
					<div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm px-4">
						<div className="w-full max-w-2xl rounded-2xl bg-white dark:bg-surface-dark p-6 shadow-2xl border border-gray-200 dark:border-gray-700 max-h-[90vh] overflow-y-auto">
							<h3 className="text-xl font-bold mb-4 text-text-main dark:text-white">Add New Book</h3>
							<div className="space-y-4">
								<div>
									<label className="text-sm font-semibold">Title<span className="text-red-500">*</span></label>
									<input
										type="text"
										{...registerAdd('title', { required: 'Title is required' })}
										className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
									/>
									{errorsAdd.title && <p className="text-red-500 text-xs mt-1">{errorsAdd.title.message}</p>}
								</div>

								<div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
									<div>
									<label className="text-sm font-semibold">Selling Price<span className="text-red-500">*</span></label>
									<input
										type="number"
										step="0.01"
										min="0"
										{...registerAdd('sellingPrice', { 
											required: 'Selling price is required',
											min: { value: 0.01, message: 'Price must be greater than 0' }
										})}
											className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
										/>
										{errorsAdd.sellingPrice && <p className="text-red-500 text-xs mt-1">{errorsAdd.sellingPrice.message}</p>}
									</div>
									<div>
										<label className="text-sm font-semibold">Quantity<span className="text-red-500">*</span></label>
										<input
											type="number"
											min="0"
											{...registerAdd('stock', { 
												required: 'Quantity is required',
												valueAsNumber: true,
												min: { value: 0, message: 'Quantity cannot be negative' }
											})}
											className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
										/>
										{errorsAdd.stock && <p className="text-red-500 text-xs mt-1">{errorsAdd.stock.message}</p>}
									</div>
									<div>
										<label className="text-sm font-semibold">Threshold<span className="text-red-500">*</span></label>
										<input
											type="number"
											min="0"
											{...registerAdd('threshold', { 
												required: 'Threshold is required',
												valueAsNumber: true,
												min: { value: 0, message: 'Threshold cannot be negative' }
											})}
											className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
										/>
										{errorsAdd.threshold && <p className="text-red-500 text-xs mt-1">{errorsAdd.threshold.message}</p>}
									</div>
								</div>

								<div>
									<label className="text-sm font-semibold">Category<span className="text-red-500">*</span></label>
									<select
										{...registerAdd('category', { required: 'Category is required' })}
										className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
									>
										<option value="">Select category</option>
										{categoryOptions.map((cat) => (
											<option key={cat} value={cat}>
												{cat}
											</option>
										))}
									</select>
									{errorsAdd.category && <p className="text-red-500 text-xs mt-1">{errorsAdd.category.message}</p>}
								</div>

<div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
					<div>
						<label className="text-sm font-semibold">Publisher ID<span className="text-red-500">*</span></label>
						<input
							type="number"
							min="1"
					{...registerAdd('pubID', { 
						required: 'Publisher ID is required',
						min: { value: 1, message: 'Publisher ID must be at least 1' }
					})}
							className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
						/>
						{errorsAdd.pubID && <p className="text-red-500 text-xs mt-1">{errorsAdd.pubID.message}</p>}
					</div>
					<div>
						<label className="text-sm font-semibold">ISBN<span className="text-red-500">*</span></label>
						<input
							type="text"
							{...registerAdd('isbn', { 
								required: 'ISBN is required',
								pattern: { value: /^[0-9-]+$/, message: 'ISBN must contain only numbers and hyphens' }
							})}
							className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
						/>
						{errorsAdd.isbn && <p className="text-red-500 text-xs mt-1">{errorsAdd.isbn.message}</p>}
					</div>
					<div>
						<label className="text-sm font-semibold">Publication Year<span className="text-red-500">*</span></label>
						<input
							type="number"
							min="1000"
							max={new Date().getFullYear()}
							{...registerAdd('publicationYear', { 
								required: 'Publication year is required',
								min: { value: 1000, message: 'Year must be at least 1000' },
								max: { value: new Date().getFullYear(), message: `Year cannot exceed ${new Date().getFullYear()}` }
							})}
											className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
										/>
										{errorsAdd.publicationYear && <p className="text-red-500 text-xs mt-1">{errorsAdd.publicationYear.message}</p>}
									</div>
								</div>

								<div className="space-y-2">
									<div className="flex items-center justify-between">
										<label className="text-sm font-semibold">Authors<span className="text-red-500">*</span></label>
										<button
											onClick={handleAddAuthor}
											type="button"
											className="px-3 py-1 rounded-full bg-primary text-white text-xs font-semibold hover:bg-primary/90"
										>
											Add Author
										</button>
									</div>
									<div className="space-y-2">
										{watchAdd('authors').map((author, idx) => (
											<div key={idx} className="space-y-1">
												<label className="text-xs font-semibold text-text-secondary dark:text-gray-300">Author {idx + 1}</label>
												<div className="flex items-center gap-2">
													<input
														type="text"
														value={author}
														onChange={(e) => handleAuthorChange(idx, e.target.value)}
														className="flex-1 h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
														placeholder="Enter author name"
													/>
													<button
														onClick={() => handleRemoveAuthorField(idx)}
														type="button"
														disabled={watchAdd('authors').length === 1}
														className="px-3 py-2 rounded-full text-xs font-semibold border border-gray-300 dark:border-gray-600 hover:border-red-500 hover:text-red-600 disabled:opacity-50"
													>
														Remove
													</button>
												</div>
											</div>
										))}
									</div>
								</div>
							</div>
							<div className="mt-6 flex justify-end gap-3">
							{watchAdd('authors').some(a => a.trim() === '') && (
								<p className="text-red-500 text-xs mt-1">All author fields must be filled or removed</p>
							)}
								<button
									onClick={handleCancelDialogs}
									className="px-4 py-2 rounded-full border border-gray-300 dark:border-gray-600 bg-surface-light dark:bg-surface-dark text-sm font-semibold hover:border-primary hover:text-primary"
								>
									Cancel
								</button>
								<button
									onClick={handleSaveNewBook}
									className="px-4 py-2 rounded-full bg-primary text-white text-sm font-semibold hover:bg-primary/90"
								>
									Add Book
								</button>
							</div>
						</div>
					</div>
				)}
			</div>
		</div>
	);
}



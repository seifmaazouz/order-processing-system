import axios from 'axios';
import API_BASE_URL from '../config/api.config.js';

const REPORTS_URL = `${API_BASE_URL}/reports`;

// a) Total sales for books in the previous month
export const getTotalSalesPreviousMonth = async () => {
	try {
		const token = localStorage.getItem('access');
		const response = await axios.get(`${REPORTS_URL}/total-sales/previous-month`, {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${token}`
			}
		});
		// Backend returns: { period, totalSalesAmount, totalTransactionCount }
		return {
			totalSales: response.data.totalSalesAmount || 0,
			totalBooksSold: 0, // Not provided by backend - would need separate query
			totalOrders: response.data.totalTransactionCount || 0
		};
	} catch (error) {
		console.error('Error fetching previous month sales:', error.response?.data);
		throw error;
	}
};

// b) Total sales for books on a certain day
export const getTotalSalesByDate = async (date) => {
	try {
		const token = localStorage.getItem('access');
		const response = await axios.get(`${REPORTS_URL}/total-sales/by-date`, {
			params: { date },
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${token}`
			}
		});
		// Backend returns: { period, totalSalesAmount, totalTransactionCount }
		return {
			totalSales: response.data.totalSalesAmount || 0,
			totalBooksSold: 0, // Not provided by backend
			totalOrders: response.data.totalTransactionCount || 0
		};
	} catch (error) {
		console.error('Error fetching sales by date:', error.response?.data);
		throw error;
	}
};

// c) Top 5 Customers (Last 3 Months)
export const getTop5Customers = async () => {
	try {
		const token = localStorage.getItem('access');
		const response = await axios.get(`${REPORTS_URL}/top-5-customers`, {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${token}`
			}
		});
		// Backend returns: [{ customerName, totalSpent, email }]
		return response.data.map((c, index) => ({
			userId: `customer-${index}`, // Generate unique key
			customerName: c.customerName || 'Unknown',
			totalSpent: c.totalSpent || 0,
			email: c.email || ''
		}));
	} catch (error) {
		console.error('Error fetching top customers:', error.response?.data);
		throw error;
	}
};

// d) Top 10 Selling Books (Last 3 Months)
export const getTop10SellingBooks = async () => {
	try {
		const token = localStorage.getItem('access');
		const response = await axios.get(`${REPORTS_URL}/top-10-selling-books`, {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${token}`
			}
		});
		// Backend returns: [{ isbn, title, totalCopiesSold }]
		return response.data.map(b => ({
			isbn: b.isbn || '',
			title: b.title || 'Unknown',
			totalCopiesSold: b.totalCopiesSold || 0,
			totalRevenue: 0 // Not provided by backend - would need price * quantity
		}));
	} catch (error) {
		console.error('Error fetching top selling books:', error.response?.data);
		throw error;
	}
};

// e) Total Number of Times a Specific Book Has Been Ordered (Replenishment)
export const getBookReplenishmentCount = async (isbn) => {
	try {
		const token = localStorage.getItem('access');
		const response = await axios.get(`${REPORTS_URL}/book-order-count`, {
			params: { isbn },
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${token}`
			}
		});
		// Backend returns: { isbn, title, timesOrderedFromPublisher }
		return {
			isbn: response.data.isbn || isbn,
			bookTitle: response.data.title || 'Unknown',
			timesOrdered: response.data.timesOrderedFromPublisher || 0,
			totalQuantityOrdered: response.data.timesOrderedFromPublisher || 0
		};
	} catch (error) {
		console.error('Error fetching book replenishment count:', error.response?.data);
		throw error;
	}
};

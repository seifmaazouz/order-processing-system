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
		return response.data;
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
		return response.data;
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
		return response.data;
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
		return response.data;
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
		return response.data;
	} catch (error) {
		console.error('Error fetching book replenishment count:', error.response?.data);
		throw error;
	}
};

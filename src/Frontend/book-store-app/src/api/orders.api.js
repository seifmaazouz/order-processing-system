import axios from 'axios';
import API_BASE_URL from '../config/api.config.js';

const CUSTOMER_ORDERS_URL = `${API_BASE_URL}/customerorder/my`;

export async function getOrders() {
  const token = localStorage.getItem('access');
  const res = await axios.get(CUSTOMER_ORDERS_URL, {
    headers: { 
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
  });
  // Backend returns: [{ OrderNumber, TotalPrice, Status, OrderDate, Items: [{ ISBN, Title, Quantity, UnitPrice }] }]
  // Handle both PascalCase and camelCase
  return res.data.map(order => ({
    id: order.OrderNumber || order.orderNumber,
    orderNumber: order.OrderNumber || order.orderNumber,
    date: order.OrderDate || order.orderDate,
    status: (order.Status || order.status || 'Pending')?.toString().toLowerCase(),
    totalPrice: order.TotalPrice || order.totalPrice,
    shippingAddress: order.ShippingAddress || order.shippingAddress,
    items: (order.Items || order.items || []).map(item => ({
      isbn: item.ISBN || item.isbn,
      title: item.Title || item.title,
      quantity: item.Quantity || item.quantity,
      unitPrice: item.UnitPrice || item.unitPrice
    }))
  }));
}

export async function getOrderDetails(orderId) {
  const token = localStorage.getItem('access');
  const res = await axios.get(`${CUSTOMER_ORDERS_URL}/${orderId}`, {
    headers: { 
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
  });
  return res.data;
}

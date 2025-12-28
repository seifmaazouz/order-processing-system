import axios from 'axios';
import API_BASE_URL from '../config/api.config.js';

const USER_URL = `${API_BASE_URL}/user`;

export async function getAccountDetails(token) {
  console.log('Token being sent:', token);
  console.log('Token type:', typeof token);
  console.log('Token length:', token?.length);
  
  if (!token) {
    throw new Error('No access token found. Please login first.');
  }
  
  const res = await axios.get(`${USER_URL}/details`, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  });
  return res.data; // { id, name, email, phone, address, ... }
}

export async function updateAccountDetails(payload, token) {
  // Backend expects: { address, email, firstName, lastName, phoneNumber }
  const requestBody = {
    address: payload.shipAddress ?? null,
    email: payload.email ?? null,
    firstName: payload.firstName ?? null,
    lastName: payload.lastName ?? null,
    phoneNumber: payload.phoneNumber ?? null,
  };

  const res = await axios.put(`${USER_URL}/profile`, requestBody, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  });
  return res.data; // { ok: true, message }
}

export async function logout(token) {
  const res = await axios.post(`${API_BASE_URL}/auth/logout`, {}, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  });
  return res.data; // { message }
}

export async function changePassword(payload, token) {
  const requestBody = {
    oldPassword: payload.oldPassword,
    newPassword: payload.newPassword,
    token: token
  };
  
  const res = await axios.post(`${USER_URL}/change-password`, requestBody, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  });
  return res.data; // { ok: true, message }
}

export async function addCreditCard(cardNumber, expiryDate, token) {
  // Validate and parse card number
  const cleanedCardNumber = cardNumber.replace(/\s/g, '');
  if (!cleanedCardNumber || cleanedCardNumber.length < 13 || cleanedCardNumber.length > 19) {
    throw new Error('Card number must be between 13 and 19 digits');
  }
  
  const cardNumberInt = parseInt(cleanedCardNumber, 10);
  if (isNaN(cardNumberInt) || cardNumberInt <= 0) {
    throw new Error('Invalid card number');
  }
  
  const requestBody = {
    CardNumber: cardNumberInt,
    ExpiryDate: expiryDate
  };
  
  const res = await axios.post(`${USER_URL}/add-credit-card`, requestBody, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  });
  return res.data; // { message: "Credit card added successfully." }
}

export async function removeCreditCard(cardNumber, token) {
  const requestBody = {
    cardNumber: cardNumber,
    token: token
  };
  
  const res = await axios.post(`${USER_URL}/remove-card`, requestBody, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  });
  return res.data; // { message: "Credit card removed successfully." }
}
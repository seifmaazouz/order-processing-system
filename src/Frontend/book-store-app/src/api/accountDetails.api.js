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
    address: payload.shipAddress,
    email: payload.email,
    firstName: payload.firstName,
    lastName: payload.lastName,
    phoneNumber: payload.phoneNumber,
  };

  const res = await axios.put(`${USER_URL}/profile`, requestBody, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  });
  return { ok: true, message: 'Profile updated successfully' }; // Backend returns 204 No Content on success
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
  return { ok: true, message: res.data }; // Backend returns success message
}

export async function addCreditCard(cardholderName, cardNumber, expiryDate, token) {
  // Validate cardholder name
  if (!cardholderName || cardholderName.trim().length < 2) {
    throw new Error('Cardholder name must be at least 2 characters');
  }

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
    CardholderName: cardholderName.trim(),
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
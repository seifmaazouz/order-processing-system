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
    address: payload.address ?? null,
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

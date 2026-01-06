import React, { useEffect, useState, useRef } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faUser, faKey, faChevronDown, faHome } from '@fortawesome/free-solid-svg-icons';
import PasswordInput from '../../components/shared/PasswordInput.jsx';
import { getAccountDetails, updateAccountDetails, changePassword, addCreditCard, removeCreditCard } from '../../api/accountDetails.api.js';
import DashboardHeader from '../../components/dashboard/DashboardHeader.jsx';
import { useCart } from '../../context/CartContext.jsx';

export default function Account() {
  const navigate = useNavigate();
  const [loadingDetails, setLoadingDetails] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [details, setDetails] = useState(null);
  const [isPasswordOpen, setIsPasswordOpen] = useState(false);
  const [isCardOpen, setIsCardOpen] = useState(false);
  const [toast, setToast] = useState(null);
  const [showSettings, setShowSettings] = useState(false);
  const settingsRef = useRef(null);
  const { summary } = useCart();
  const { register, handleSubmit, reset } = useForm();
  const {
    register: registerPwd,
    handleSubmit: handleSubmitPwd,
    reset: resetPwd,
    watch: watchPwd,
    formState: { errors: pwdErrors },
  } = useForm();
  const {
    register: registerCard,
    handleSubmit: handleSubmitCard,
    reset: resetCard,
    formState: { errors: cardErrors },
  } = useForm();

  useEffect(() => {
    async function load() {
      try {
        setLoadingDetails(true);
        const token = localStorage.getItem('access');
        console.log('Account page - stored token:', token);
        
        if (!token) {
          throw new Error('No access token found. Please login first.');
        }
        
        const data = await getAccountDetails(token);
        // Backend returns PascalCase fields, normalize to camelCase
        const normalizedData = {
          ...data,
          username: data.Username || data.username || '',
          firstName: data.FirstName || data.firstName || '',
          lastName: data.LastName || data.lastName || '',
          email: data.Email || data.email || '',
          phoneNumber: data.PhoneNumber || data.phoneNumber || '',
          shipAddress: data.Address || data.address || '',
          creditCards: (data.CreditCards || data.creditCards || []).map(card => ({
            cardNumber: card.CardNumber || card.cardNumber,
            expiryMonth: card.ExpiryMonth || card.expiryMonth,
            expiryYear: card.ExpiryYear || card.expiryYear,
            cardholderName: card.CardholderName || card.cardholderName || ''
          }))
        };
        setDetails(normalizedData);
        console.log('Fetched account details:', normalizedData);
        reset({
          username: normalizedData.username || '',
          firstName: normalizedData.firstName || '',
          lastName: normalizedData.lastName || '',
          email: normalizedData.email || '',
          phoneNumber: normalizedData.phoneNumber || '',
          shipAddress: normalizedData.shipAddress || '',
        });
      } catch (err) {
        console.error('Account loading error:', err);
        setToast({ type: 'error', message: err?.message || 'Failed to load account details' });
      } finally {
        setLoadingDetails(false);
      }
    }
    load();
  }, [reset]);

  useEffect(() => {
    function handleClick(e) {
      if (settingsRef.current && !settingsRef.current.contains(e.target)) {
        setShowSettings(false);
      }
    }

    document.addEventListener('click', handleClick);
    return () => document.removeEventListener('click', handleClick);
  }, []);

  const onSaveProfile = async (formData) => {
    try {
      const token = localStorage.getItem('access');
      const res = await updateAccountDetails(formData, token);
      if (res?.ok || res?.message) {
        setToast({ type: 'success', message: res.message || 'Profile updated' });
        // Update details state with the new data
        setDetails((prev) => ({
          ...prev,
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
          phoneNumber: formData.phoneNumber,
          shipAddress: formData.shipAddress,
        }));
        setIsEditing(false);
      } else {
        setToast({ type: 'error', message: res?.message || 'Update failed' });
      }
    } catch (err) {
      const errorMessage = err?.response?.data?.error || err?.response?.data?.message || err?.message || 'Update failed';
      setToast({ type: 'error', message: errorMessage });
    } finally {
      setTimeout(() => setToast(null), 3000);
    }
  };

  const onCancelEdit = () => {
    // Reset form to current saved details and exit edit mode
    if (details) {
      reset({
        username: details.username || '',
        firstName: details.firstName || '',
        lastName: details.lastName || '',
        email: details.email || '',
        phoneNumber: details.phoneNumber || '',
        shipAddress: details.shipAddress || '',
      });
    }
    setIsEditing(false);
  };

  const onChangePassword = async (pwdData) => {
    if (pwdData.newPassword !== pwdData.confirmPassword) {
      setToast({ type: 'error', message: 'Passwords do not match' });
      setTimeout(() => setToast(null), 3000);
      return;
    }
    try {
      const token = localStorage.getItem('access');
      const res = await changePassword({
        oldPassword: pwdData.oldPassword,
        newPassword: pwdData.newPassword,
      }, token);
      if (res?.ok) {
        setToast({ type: 'success', message: res.message || 'Password changed' });
        resetPwd();
      } else {
        setToast({ type: 'error', message: res?.message || 'Change password failed' });
      }
    } catch (err) {
      // Handle different error response formats
      let errorMessage = 'Change password failed';
      if (err?.response?.data?.error) {
        errorMessage = err.response.data.error;
      } else if (err?.response?.data?.message) {
        errorMessage = err.response.data.message;
      } else if (err?.message) {
        errorMessage = err.message;
      }
      setToast({ type: 'error', message: errorMessage });
    } finally {
      setTimeout(() => setToast(null), 3000);
    }
  };

  const onAddCreditCard = async (cardData) => {
    try {
      const token = localStorage.getItem('access');
      // Convert MM/YY to Date string (last day of the month) in YYYY-MM-DD format
      const [month, year] = cardData.expiryDate.split('/');
      const expiryYear = 2000 + parseInt(year);
      const expiryMonth = parseInt(month);
      // Get last day of the month
      const lastDay = new Date(expiryYear, expiryMonth, 0).getDate();
      // Format as YYYY-MM-DD for better compatibility
      const expiryDateStr = `${expiryYear}-${String(expiryMonth).padStart(2, '0')}-${String(lastDay).padStart(2, '0')}`;
      
      const res = await addCreditCard(cardData.cardholderName, cardData.cardNumber, expiryDateStr, token);
      setToast({ type: 'success', message: res.message || 'Credit card added successfully' });
      resetCard();
      setIsCardOpen(false);
      // Reload account details to show new card
      const data = await getAccountDetails(token);
      const normalizedData = {
        ...data,
        username: data.Username || data.username || '',
        firstName: data.FirstName || data.firstName || '',
        lastName: data.LastName || data.lastName || '',
        email: data.Email || data.email || '',
        phoneNumber: data.PhoneNumber || data.phoneNumber || '',
        shipAddress: data.Address || data.address || '',
        creditCards: data.CreditCards || data.creditCards || []
      };
      setDetails(normalizedData);
    } catch (err) {
      setToast({ type: 'error', message: err?.response?.data?.message || err?.message || 'Failed to add credit card' });
    } finally {
      setTimeout(() => setToast(null), 3000);
    }
  };

  const onRemoveCreditCard = async (cardNumber) => {
    if (!window.confirm('Are you sure you want to remove this credit card?')) {
      return;
    }
    try {
      const token = localStorage.getItem('access');
      const res = await removeCreditCard(cardNumber.toString(), token);
      setToast({ type: 'success', message: res.message || 'Credit card removed successfully' });
      // Reload account details
      const data = await getAccountDetails(token);
      const normalizedData = {
        ...data,
        username: data.Username || data.username || '',
        firstName: data.FirstName || data.firstName || '',
        lastName: data.LastName || data.lastName || '',
        email: data.Email || data.email || '',
        phoneNumber: data.PhoneNumber || data.phoneNumber || '',
        shipAddress: data.Address || data.address || '',
        creditCards: data.CreditCards || data.creditCards || []
      };
      setDetails(normalizedData);
    } catch (err) {
      setToast({ type: 'error', message: err?.response?.data?.message || err?.message || 'Failed to remove credit card' });
    } finally {
      setTimeout(() => setToast(null), 3000);
    }
  };

  return (
    <div className="min-h-screen bg-background-light dark:bg-background-dark text-text-main-light dark:text-text-main-dark">
      {/* Shared Header */}
            <DashboardHeader
              showSettings={showSettings}
              onToggleSettings={setShowSettings}
              settingsRef={settingsRef}
              cartTotal={summary?.totalItems ?? 0}
            />

      <main className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
        <div className="flex items-center justify-between mb-6">
          <button
            onClick={() => navigate('/dashboard')}
            className="flex items-center gap-2 px-4 py-2 text-sm font-semibold bg-primary text-white rounded-full hover:bg-primary/90 transition-colors"
          >
            <FontAwesomeIcon icon={faHome} /> Back to Home
          </button>
          <h1 className="text-3xl font-bold">Account</h1>
        </div>

        {/* Personal Details */}
        <section className="mb-10 rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-surface-dark p-6 shadow-sm">
          <div className="flex items-center gap-2 mb-4">
            <FontAwesomeIcon icon={faUser} className="text-gray-500" />
            <h2 className="text-xl font-semibold">Personal Details</h2>
          </div>
          {loadingDetails ? (
            <p className="text-sm text-gray-500">Loading...</p>
          ) : isEditing ? (
            <form onSubmit={handleSubmit(onSaveProfile)} className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="md:col-span-2">
                <label className="text-sm font-semibold">Username</label>
                <input
                  {...register('username')}
                  type="text"
                  disabled
                  className="mt-1 w-full h-10 px-3 rounded-md bg-gray-100 dark:bg-gray-800 border border-gray-200 dark:border-gray-700 text-gray-500 dark:text-gray-400 cursor-not-allowed"
                />
              </div>
              <div>
                <label className="text-sm font-semibold">First Name</label>
                <input
                  {...register('firstName')}
                  type="text"
                  className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                />
              </div>
              <div>
                <label className="text-sm font-semibold">Last Name</label>
                <input
                  {...register('lastName')}
                  type="text"
                  className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                />
              </div>
              <div>
                <label className="text-sm font-semibold">Email</label>
                <input
                  {...register('email')}
                  type="email"
                  className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                />
              </div>
              <div>
                <label className="text-sm font-semibold">Phone</label>
                <input
                  {...register('phoneNumber')}
                  type="text"
                  className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                />
              </div>
              <div className="md:col-span-2">
                <label className="text-sm font-semibold">Address</label>
                <input
                  {...register('shipAddress')}
                  type="text"
                  className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                />
              </div>
              <div className="md:col-span-2 flex justify-end mt-2 gap-2">
                <button type="button" onClick={onCancelEdit} className="px-5 py-2 rounded-full border border-gray-300 dark:border-gray-600 bg-surface-light dark:bg-surface-dark font-semibold hover:border-primary hover:text-primary">
                  Cancel
                </button>
                <button type="submit" className="px-5 py-2 rounded-full bg-primary text-white font-semibold hover:bg-primary/90">
                  Save Changes
                </button>
              </div>
            </form>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="md:col-span-2">
                <span className="text-sm font-semibold">Username</span>
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.username || '-'}</p>
              </div>
              <div>
                <span className="text-sm font-semibold">First Name</span>
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.firstName || '-'}</p>
              </div>
              <div>
                <span className="text-sm font-semibold">Last Name</span>
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.lastName || '-'}</p>
              </div>
              <div>
                <span className="text-sm font-semibold">Email</span>
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.email || '-'}</p>
              </div>
              <div>
                <span className="text-sm font-semibold">Phone</span>
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.phoneNumber || '-'}</p>
              </div>
              <div className="md:col-span-2">
                <span className="text-sm font-semibold">Address</span>
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.shipAddress || '-'}</p>
              </div>
              <div className="md:col-span-2 flex justify-end mt-2">
                <button type="button" onClick={() => setIsEditing(true)} className="px-5 py-2 rounded-full bg-primary text-white font-semibold hover:bg-primary/90">
                  Edit
                </button>
              </div>
            </div>
          )}
        </section>

        {/* Credit Cards Section */}
        <section className="mb-10 rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-surface-dark p-6 shadow-sm overflow-hidden">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-xl font-semibold">Payment Methods</h2>
            <button
              type="button"
              onClick={() => setIsCardOpen(!isCardOpen)}
              className="px-4 py-2 rounded-full bg-primary text-white text-sm font-semibold hover:bg-primary/90"
            >
              {isCardOpen ? 'Cancel' : '+ Add Card'}
            </button>
          </div>
          
          {/* Add Credit Card Form */}
          {isCardOpen && (
            <div className="border-t border-gray-200 dark:border-gray-700 pt-4 mb-4">
              <form onSubmit={handleSubmitCard(onAddCreditCard)} className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="md:col-span-2">
                  <label className="block text-sm font-semibold mb-2">Cardholder Name</label>
                  <input
                    type="text"
                    {...registerCard('cardholderName', {
                      required: 'Cardholder name is required',
                      minLength: {
                        value: 2,
                        message: 'Cardholder name must be at least 2 characters'
                      }
                    })}
                    placeholder="John Doe"
                    className="w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                  />
                  {cardErrors.cardholderName && (
                    <p className="text-red-500 text-xs mt-1">{cardErrors.cardholderName.message}</p>
                  )}
                </div>
                <div className="md:col-span-2">
                  <label className="block text-sm font-semibold mb-2">Card Number</label>
                  <input
                    type="text"
                    {...registerCard('cardNumber', {
                      required: 'Card number is required',
                      pattern: {
                        value: /^[\d\s]{13,19}$/,
                        message: 'Card number must be 13-19 digits'
                      }
                    })}
                    placeholder="1234 5678 9012 3456"
                    onChange={(e) => {
                      const value = e.target.value.replace(/\D/g, '');
                      const formatted = value.match(/.{1,4}/g)?.join(' ') || value;
                      e.target.value = formatted.slice(0, 19);
                    }}
                    className="w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                  />
                  {cardErrors.cardNumber && (
                    <p className="text-red-500 text-xs mt-1">{cardErrors.cardNumber.message}</p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-semibold mb-2">Expiry Date (MM/YY)</label>
                  <input
                    type="text"
                    {...registerCard('expiryDate', {
                      required: 'Expiry date is required',
                      pattern: {
                        value: /^(0[1-9]|1[0-2])\/\d{2}$/,
                        message: 'Format: MM/YY'
                      },
                      validate: (value) => {
                        if (!value) return true; // Let required handle this
                        const [month, year] = value.split('/');
                        const expiryMonth = parseInt(month);
                        const expiryYear = 2000 + parseInt(year);
                        const now = new Date();
                        const expiryDate = new Date(expiryYear, expiryMonth - 1); // Month is 0-indexed
                        if (expiryDate < now) {
                          return 'Card has expired';
                        }
                        return true;
                      }
                    })}
                    placeholder="MM/YY"
                    onChange={(e) => {
                      const value = e.target.value.replace(/\D/g, '');
                      if (value.length <= 4) {
                        const formatted = value.length >= 2 ? `${value.slice(0, 2)}${value.slice(2)}` : value;
                        // Add slash automatically when 2+ digits
                        if (formatted.length > 2) {
                          const month = formatted.slice(0, 2);
                          const year = formatted.slice(2);
                          e.target.value = `${month}/${year}`;
                        } else {
                          e.target.value = formatted;
                        }
                      }
                    }}
                    maxLength="5"
                    className="w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                  />
                  {cardErrors.expiryDate && (
                    <p className="text-red-500 text-xs mt-1">{cardErrors.expiryDate.message}</p>
                  )}
                </div>
                <div className="md:col-span-2 flex justify-end mt-2">
                  <button type="submit" className="px-5 py-2 rounded-full bg-primary text-white font-semibold hover:bg-primary/90">
                    Add Card
                  </button>
                </div>
              </form>
            </div>
          )}

          {loadingDetails ? (
            <p className="text-sm text-gray-500">Loading...</p>
          ) : details?.creditCards && details.creditCards.length > 0 ? (
            <div className="space-y-3">
              {details.creditCards.map((card, idx) => {
                const cardNumStr = card.cardNumber?.toString() || '';
                const last4 = cardNumStr.slice(-4);
                return (
                  <div 
                    key={idx} 
                    className="flex items-center justify-between p-4 rounded-lg bg-gray-50 dark:bg-gray-800 border border-gray-200 dark:border-gray-700"
                  >
                    <div className="flex items-center gap-4">
                      <div className="w-12 h-8 rounded bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center">
                        <span className="text-white text-xs font-bold">CARD</span>
                      </div>
                      <div>
                        <p className="font-mono text-lg text-gray-800 dark:text-gray-200">
                          •••• •••• •••• {last4}
                        </p>
                        <p className="text-sm text-gray-600 dark:text-gray-400 font-medium">
                          {card.cardholderName}
                        </p>
                        <p className="text-sm text-gray-500">
                          Expires: {card.expiryMonth?.padStart(2, '0') || '**'}/{card.expiryYear || '**'}
                        </p>
                      </div>
                    </div>
                    <button
                      onClick={() => onRemoveCreditCard(card.cardNumber)}
                      className="px-3 py-1 rounded-full bg-red-500 hover:bg-red-600 text-white text-sm font-semibold"
                    >
                      Remove
                    </button>
                  </div>
                );
              })}
            </div>
          ) : (
            <div className="text-center py-8">
              <p className="text-gray-500">No credit cards on file</p>
              <p className="text-sm text-gray-400 mt-1">Add a payment method to complete orders</p>
            </div>
          )}
        </section>

        {/* Change Password - Accordion */}
        <section className="rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-surface-dark shadow-sm overflow-hidden">
          <button
            type="button"
            onClick={() => setIsPasswordOpen(!isPasswordOpen)}
            className="w-full flex items-center justify-between gap-2 px-6 py-4 hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors"
          >
            <div className="flex items-center gap-2">
              <FontAwesomeIcon icon={faKey} className="text-gray-500" />
              <h2 className="text-lg font-semibold">Change Password</h2>
            </div>
            <FontAwesomeIcon
              icon={faChevronDown}
              className={`text-gray-500 transition-transform ${
                isPasswordOpen ? 'rotate-180' : ''
              }`}
            />
          </button>
          {isPasswordOpen && (
            <div className="border-t border-gray-200 dark:border-gray-700 px-6 py-4">
              <form onSubmit={handleSubmitPwd(onChangePassword)} className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <PasswordInput
                  label="Old Password"
                  register={registerPwd('oldPassword', { 
                    required: 'Old password is required',
                    minLength: { value: 6, message: 'Password must be at least 6 characters' }
                  })}
                  error={pwdErrors.oldPassword}
                />
                <PasswordInput
                  label="New Password"
                  register={registerPwd('newPassword', { 
                    required: 'New password is required',
                    minLength: { value: 6, message: 'Password must be at least 6 characters' }
                  })}
                  error={pwdErrors.newPassword}
                />
                <PasswordInput
                  label="Confirm Password"
                  register={registerPwd('confirmPassword', {
                    required: 'Please confirm your password',
                    validate: (value) => 
                      value === watchPwd('newPassword') || 'Passwords do not match'
                  })}
                  error={pwdErrors.confirmPassword}
                />
                <div className="md:col-span-2 flex justify-end mt-2">
                  <button type="submit" className="px-5 py-2 rounded-full bg-primary text-white font-semibold hover:bg-primary/90">
                    Change Password
                  </button>
                </div>
              </form>
            </div>
          )}
        </section>
      </main>

      {/* Toast */}
      {toast && (
        <div
          className={`fixed top-4 right-4 px-5 py-3 rounded-lg font-semibold text-white shadow-lg z-50 ${
            toast.type === 'success' ? 'bg-green-500' : 'bg-red-500'
          }`}
        >
          {toast.message}
        </div>
      )}
    </div>
  );
}

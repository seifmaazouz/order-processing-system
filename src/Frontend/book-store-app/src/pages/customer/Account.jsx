import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faUser, faKey, faChevronDown } from '@fortawesome/free-solid-svg-icons';
import Header from '../../components/shared/Header.jsx';
import PasswordInput from '../../components/shared/PasswordInput.jsx';
import { getAccountDetails, updateAccountDetails, changePassword } from '../../api/accountDetails.api.js';

export default function Account() {
  const [loadingDetails, setLoadingDetails] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [details, setDetails] = useState(null);
  const [isPasswordOpen, setIsPasswordOpen] = useState(false);
  const [toast, setToast] = useState(null);
  const { register, handleSubmit, reset } = useForm();
  const {
    register: registerPwd,
    handleSubmit: handleSubmitPwd,
    reset: resetPwd,
    watch: watchPwd,
    formState: { errors: pwdErrors },
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
        setDetails(data);
        console.log('Fetched account details:', data);
        reset({
          username: data.username || '',
          firstName: data.firstName || '',
          lastName: data.lastName || '',
          email: data.email || '',
          phoneNumber: data.phoneNumber || '',
          address: data.addresses?.[0] || '',
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

  const onSaveProfile = async (formData) => {
    try {
      const token = localStorage.getItem('access');
      const res = await updateAccountDetails(formData, token);
      if (res?.ok) {
        setToast({ type: 'success', message: res.message || 'Profile updated' });
        setDetails((prev) => ({ ...prev, ...formData }));
        setIsEditing(false);
      } else {
        setToast({ type: 'error', message: res?.message || 'Update failed' });
      }
    } catch (err) {
      setToast({ type: 'error', message: err?.message || 'Update failed' });
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
        address: details.addresses?.[0] || '',
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
      setToast({ type: 'error', message: err?.message || 'Change password failed' });
    } finally {
      setTimeout(() => setToast(null), 3000);
    }
  };

  return (
    <div className="min-h-screen bg-background-light dark:bg-background-dark text-text-main-light dark:text-text-main-dark">
      {/* Shared Header */}
      <Header />

      <main className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
        <h1 className="text-3xl font-bold mb-6">Account</h1>

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
                  {...register('address')}
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
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.addresses?.[0] || '-'}</p>
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
        <section className="mb-10 rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-surface-dark p-6 shadow-sm">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-xl font-semibold">Payment Methods</h2>
          </div>
          {loadingDetails ? (
            <p className="text-sm text-gray-500">Loading...</p>
          ) : details?.creditCards && details.creditCards.length > 0 ? (
            <div className="space-y-3">
              {details.creditCards.map((card, idx) => (
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
                        •••• •••• •••• {card.cardNumber?.slice(-4) || '****'}
                      </p>
                      <p className="text-sm text-gray-500">
                        Expires: {card.expiryMonth?.padStart(2, '0')}/{card.expiryYear}
                      </p>
                    </div>
                  </div>
                </div>
              ))}
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

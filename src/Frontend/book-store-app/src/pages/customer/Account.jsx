import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faUser, faKey, faChevronDown } from '@fortawesome/free-solid-svg-icons';
import Header from '../../components/shared/Header.jsx';
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
  } = useForm();

  useEffect(() => {
    async function load() {
      try {
        setLoadingDetails(true);
        const data = await getAccountDetails();
        setDetails(data);
        reset({
          name: data.name || '',
          email: data.email || '',
          phone: data.phone || '',
          address: data.address || '',
        });
      } catch (err) {
        setToast({ type: 'error', message: err?.message || 'Failed to load account details' });
      } finally {
        setLoadingDetails(false);
      }
    }
    load();
  }, [reset]);

  const onSaveProfile = async (formData) => {
    try {
      const res = await updateAccountDetails(formData);
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
        name: details.name || '',
        email: details.email || '',
        phone: details.phone || '',
        address: details.address || '',
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
      const res = await changePassword({
        oldPassword: pwdData.oldPassword,
        newPassword: pwdData.newPassword,
      });
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
              <div>
                <label className="text-sm font-semibold">Name</label>
                <input
                  {...register('name')}
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
                  {...register('phone')}
                  type="text"
                  className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                />
              </div>
              <div>
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
              <div>
                <span className="text-sm font-semibold">Name</span>
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.name || '-'}</p>
              </div>
              <div>
                <span className="text-sm font-semibold">Email</span>
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.email || '-'}</p>
              </div>
              <div>
                <span className="text-sm font-semibold">Phone</span>
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.phone || '-'}</p>
              </div>
              <div>
                <span className="text-sm font-semibold">Address</span>
                <p className="mt-1 text-gray-800 dark:text-gray-200">{details?.address || '-'}</p>
              </div>
              <div className="md:col-span-2 flex justify-end mt-2">
                <button type="button" onClick={() => setIsEditing(true)} className="px-5 py-2 rounded-full bg-primary text-white font-semibold hover:bg-primary/90">
                  Edit
                </button>
              </div>
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
                <div>
                  <label className="text-sm font-semibold">Old Password</label>
                  <input
                    {...registerPwd('oldPassword')}
                    type="password"
                    className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                  />
                </div>
                <div>
                  <label className="text-sm font-semibold">New Password</label>
                  <input
                    {...registerPwd('newPassword')}
                    type="password"
                    className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                  />
                </div>
                <div>
                  <label className="text-sm font-semibold">Confirm Password</label>
                  <input
                    {...registerPwd('confirmPassword')}
                    type="password"
                    className="mt-1 w-full h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
                  />
                </div>
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

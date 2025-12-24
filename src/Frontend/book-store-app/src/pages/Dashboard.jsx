import React from 'react';
import Header from '../components/shared/Header.jsx';

export default function Dashboard() {
  return (
    <div className="min-h-screen bg-background-light dark:bg-background-dark">
      <Header />
      <main className="p-8">
        <h1 className="text-2xl font-bold">Dashboard</h1>
        <p className="mt-4">Welcome to your dashboard. You are logged in.</p>
      </main>
    </div>
  );
}

import React from "react";
import bg from "../../assets/images/lgn-bg.png";
import { useState } from "react";
import { loginUsers } from "../../api/login.api.js";
import LoginForm from "../../components/login-form/LoginForm";
import { useNavigate } from "react-router-dom";
import Header from "../../components/shared/Header.jsx";
import { set } from "zod";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
export default function Login() {
  
  const [loading, setLoading] = useState(false);
    const [incorrect, setIncorrect] = useState(false);
  const [resetForm, setResetForm] = useState(false);

  const onSubmit = async (data) => {
    setLoading(true);

    try {
    setResetForm(false);
      const res = await loginUsers(data);
      console.log(data);
      console.log(res);
      localStorage.setItem("access", res.data.access);
      localStorage.setItem("refresh", res.data.refresh);
      localStorage.setItem("userId", data.username);
      
      setIncorrect(false);
      setLoading(false);
      toast.success("Login successful!", {
        position: "top-center",
        autoClose: 1500, // 3 seconds
        hideProgressBar: false,
        closeOnClick: true,
        
        draggable: true,
      });
      navigate('/dashboard');
    } catch (err) {
      setResetForm(true);
      setIncorrect(true);
        setLoading(false);
         toast.error("Login failed! Please check your credentials.", {
        position: "bottom-center",
        autoClose: 1500,
      });
      alert('Login failed! Please check your credentials.');
    } 
  };
  return (
    <div className="light min-h-screen">

    <Header />
      <main className="flex flex-1 flex-col lg:flex-row">
        {/* Left Image */}
        <div className="hidden lg:block lg:w-1/2 relative overflow-hidden bg-gray-100">
          <img
            src={bg}
            alt="Library"
            className="absolute inset-0 h-full w-full object-cover opacity-90 hover:scale-105 transition-transform duration-[20s]"
          />
          <div className="absolute inset-0 bg-gradient-to-t from-stone-900/60" />
          <div className="absolute bottom-12 left-12 max-w-md text-white">
            <p className="text-3xl font-bold mb-4">
              "A room without books is like a body without a soul."
            </p>
            <p className="text-lg opacity-90">
              — Marcus Tullius Cicero
            </p>
          </div>
        </div>

        {/* Right Form */}
        <div className="w-full lg:w-1/2 flex items-center justify-center px-6 py-12 lg:p-24 bg-background-light dark:bg-background-dark">
            <LoginForm onSubmit={onSubmit} resetForm={resetForm} loading={loading} />
        </div>
      </main>
    </div>
  );
}


import React from "react";
import { Link } from 'react-router-dom';
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Header from "../../components/shared/Header.jsx";
import RegisterForm from "../../components/register-form/RegisterForm";
import { registerUser } from "../../api/register.api.js";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
export default function Register() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [resetForm, setResetForm] = useState(false);
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    // trigger entrance animation
    const t = setTimeout(() => setMounted(true), 20);
    return () => clearTimeout(t);
  }, []);

  const onSubmit = async (data) => {
    setLoading(true);
    try {
      setResetForm(false);
      const res= await registerUser(data);
      console.log('register data', res);
      setLoading(false);
         
      toast.success("Registration successful!", {
        position: "top-right",
        autoClose: 3000, // 3 seconds
        hideProgressBar: false,
        closeOnClick: true,
        pauseOnHover: true,
        draggable: true,
      });
      
      navigate('/login');
    } catch (err) {
      setLoading(false);
      setResetForm(true);
         toast.error("Registration failed! Please check your details.", {
        position: "bottom-center",
        autoClose: 1500,
      });
      
    }
  };

  return (
    <div className="bg-background-light dark:bg-background-dark font-display text-text-main dark:text-white antialiased min-h-screen flex flex-col">
        
      {/* Main */}
      <div className="flex-1 flex flex-col items-center justify-center w-full px-4 py-8">
        <div className={`w-full max-w-[640px]  px-6 py-4 lg:px-10 bg-white dark:bg-slate-900 border border-border-color 
            dark:border-gray-700 rounded-2xl 
            shadow-sm transform transition-all duration-700 ease-out 
            ${mounted ? 'translate-y-0 opacity-100' : '-translate-y-8 opacity-0'}`}>

            <div className="flex flex-col gap-5">
            <div className="text-center">
              <h1 className="text-[32px] lg:text-[32px] font-bold">
                Join Our Community
              </h1>
              <p className="text-text-muted dark:text-gray-400">
                Create your account to start your reading journey.
              </p>
            </div>

              <RegisterForm onSubmit={onSubmit} resetForm={resetForm} />

            


            <p className="text-center">
              Already have an account?{" "}
              <Link to="/login" className="font-bold underline decoration-primary">Log In</Link>
            </p>
            <div>
                <ToastContainer />
            </div>

          </div>
        </div>
      </div>
    </div>
  );

}


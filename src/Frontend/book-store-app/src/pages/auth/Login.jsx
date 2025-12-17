import React from "react";
import bg from "../../assets/images/lgn-bg.png";
import {useForm} from "react-hook-form";
import { loginUsers } from "../../api/authApi";
import LoginForm from "../../components/login-form/LoginForm.jsx";
import { useNavigate } from "react-router-dom";

export default function Login() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [incorrect, setIncorrect] = useState(false);

  const onSubmit = async (data) => {
    setLoading(true);

    try {
      const res = await loginUsers(data);
      console.log(data);
      console.log(res);
      localStorage.setItem("access", res.data.access);
      localStorage.setItem("refresh", res.data.refresh);
      localStorage.setItem("userId", data.username);
      setIncorrect(false);

      alert("login success");
      navigate("/dashboard");
    } catch (err) {
      setIncorrect(true);
      alert("login failed");
    } finally {

    }
  };
  return (
    <div className="light min-h-screen">
      <header className="flex items-center justify-between whitespace-nowrap 
      border-b border-solid border-border-color dark:border-[#3a392a] px-6 py-4 lg:px-10 lg:py-5 
      bg-background-light/90 dark:bg-background-dark/90 backdrop-blur-sm sticky top-0 z-50">
        <div className="flex items-center gap-3">
          <div className="size-8 flex items-center justify-center text-primary">

          </div>
          <h2 className="text-xl font-bold tracking-[-0.015em]">
            Bookstore
          </h2>
        </div>

        <div className="hidden sm:flex flex-1 justify-end gap-8">
          <a className="text-sm font-medium hover:text-primary transition-colors" href="#">
            Home
          </a>
          <a className="text-sm font-medium hover:text-primary transition-colors" href="#">
            About Us
          </a>
          <a className="text-sm font-medium hover:text-primary transition-colors" href="#">
            Contact
          </a>
        </div>

        <div className="flex sm:hidden">
          <span className="material-symbols-outlined cursor-pointer">
            menu
          </span>
        </div>
      </header>

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
            <LoginForm onSubmit={onSubmit} incorrect={incorrect} setIncorrect={setIncorrect} />
        </div>
      </main>
    </div>
  );
}


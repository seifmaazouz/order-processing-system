import React, { useEffect, useState } from 'react'
import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { ThreeCircles } from 'react-loader-spinner';    
import { loginSchema } from "../../schemas/loginSchema";
import ErrorMsg from '../shared/ErrorMsg';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';
export default function LoginForm({ onSubmit, resetForm , loading}) {
    const { register, handleSubmit, formState: { errors }, reset } = useForm({ resolver: zodResolver(loginSchema) });
    const [showPassword, setShowPassword] = useState(false);
    useEffect(() => {
        if (resetForm) {
            reset({ username: '',
                password: '' });
        }
    }, [resetForm]);
    return (
        <div className="w-full max-w-[480px] flex flex-col gap-8">
            <div>
                <h1 className="text-[32px] lg:text-[40px] font-bold">
                    Welcome Back, Reader
                </h1>
                <p className="text-text-muted mt-2">
                    Please enter your details to access your bookshelf.
                </p>
            </div>

            <form className="flex flex-col gap-6" onSubmit={handleSubmit(onSubmit)}>
                <label className="flex flex-col gap-2">
                    <span className="font-medium">Username</span>
                    <input
                        type="text"
                        placeholder="JohnDoe123"
                        {...register("username")}
                        className="h-14 rounded-xl border border-border-color px-4 focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none"
                    />
                    {errors.username && <ErrorMsg error={errors.username.message} />}
                </label>

                <label className="flex flex-col gap-2">
                    <div className="flex justify-between">
                        <span className="font-medium">Password</span>
                        <Link className="text-sm text-primary hover:text-primary-hover" to="/forgot-password">
                                Forgot Password?
                            </Link>
                    </div>
                    <div className="flex rounded-xl border border-border-color overflow-hidden items-center">
                        <input
                            type={showPassword ? 'text' : 'password'}
                            placeholder="********"
                            {...register("password")}
                            className="flex-1 h-14 px-4 outline-none"
                        />
                        <button
                            type="button"
                            onClick={() => setShowPassword(s => !s)}
                            className="flex items-center px-4 cursor-pointer"
                            aria-label={showPassword ? "Hide password" : "Show password"}
                            title={showPassword ? "Hide password" : "Show password"}
                        >
                            <FontAwesomeIcon icon={showPassword ? faEye : faEyeSlash} />
                        </button>
                    </div>
                    {errors.password && <ErrorMsg error={errors.password.message} />}
                </label>

                <button className="h-14 rounded-full bg-primary hover:bg-primary-hover text-white font-bold transition-all"
                    type="submit"
                    disabled={loading}>
                    {loading ? (
                        <div className="flex justify-center">
                            <ThreeCircles
                                visible={true}
                                height="24"
                                width="24"
                                color="#ffffff"
                                ariaLabel="three-circles-loading"
                                wrapperStyle={{}}
                                wrapperClass=""
                            />
                        </div>
                    ) : "Login"}
                </button>
            </form>

            <p className="text-center">
                New here?{" "}
                <Link className="font-bold hover:underline decoration-primary" to="/register">
                    Create an Account
                </Link>
            </p>
        </div>
    )
}

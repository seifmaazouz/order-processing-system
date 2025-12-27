import React, { useEffect, useState } from 'react'
import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { ThreeCircles } from 'react-loader-spinner';    
import { loginSchema } from "../../schemas/loginSchema";
import ErrorMsg from '../shared/ErrorMsg';
import PasswordInput from '../shared/PasswordInput';
export default function LoginForm({ onSubmit, resetForm , loading}) {
    const { register, handleSubmit, formState: { errors }, reset } = useForm({ resolver: zodResolver(loginSchema) });
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

                <div className="flex flex-col gap-2">
                    <div className="flex justify-between">
                        <span className="font-medium">Password</span>
                        <Link className="text-sm text-primary hover:text-primary-hover" to="/forgot-password">
                                Forgot Password?
                            </Link>
                    </div>
                    <PasswordInput
                        register={register("password")}
                        error={errors.password}
                        className="h-14 rounded-xl"
                    />
                </div>

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

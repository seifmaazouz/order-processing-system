import React, { use } from 'react'
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { loginSchema } from '../../schemas/authSchemas';
export default function LoginForm({ onSubmit, incorrect,setIncorrect }) {
    const { register, handleSubmit, formState: { errors }, reset } = useForm({ resolver: zodResolver(loginSchema) });
    useffect(() => {
        if (incorrect) {
            reset({ password: '' });
        }
    }, [incorrect,reset]);
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

            <form className="flex flex-col gap-6">
                <label className="flex flex-col gap-2">
                    <span className="font-medium">Email or Username</span>
                    <input
                        type="text"
                        placeholder="user@example.com"
                        className="h-14 rounded-xl border border-border-color px-4 focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none"
                    />
                </label>

                <label className="flex flex-col gap-2">
                    <div className="flex justify-between">
                        <span className="font-medium">Password</span>
                        <a className="text-sm text-primary hover:text-primary-hover" href="#">
                            Forgot Password?
                        </a>
                    </div>
                    <input
                        type="password"
                        placeholder="********"
                        className="h-14 rounded-xl border border-border-color px-4 focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none"
                    />
                </label>

                <button className="h-14 rounded-full bg-primary hover:bg-primary-hover text-white font-bold transition-all">
                    Log In
                </button>
            </form>

            <p className="text-center">
                New here?{" "}
                <a className="font-bold hover:underline decoration-primary" href="#">
                    Create an Account
                </a>
            </p>
        </div>
    )
}

import React, { useEffect } from 'react'
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { ThreeCircles } from 'react-loader-spinner';    
import { loginSchema } from "../../schemas/loginSchema";
export default function LoginForm({ onSubmit, resetForm , loading}) {
    const { register, handleSubmit, formState: { errors }, reset } = useForm({ resolver: zodResolver(loginSchema) });
    useEffect(() => {
        if (resetForm) {
            reset({ password: '' });
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
                    {errors.username && <p className="text-red-500 text-sm">{errors.username.message}</p>}
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
                        {...register("password")}
                        className="h-14 rounded-xl border border-border-color px-4 focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none"
                    />
                    {errors.password && <p className="text-red-500 text-sm">{errors.password.message}</p>}
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
                <a className="font-bold hover:underline decoration-primary" href="#">
                    Create an Account
                </a>
            </p>
        </div>
    )
}

import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { registerSchema } from "../../schemas/registerSchema";
import PasswordInput from '../shared/PasswordInput';

function RegisterForm({ onSubmit, resetForm }) {
    const { register, handleSubmit, formState: { errors, isSubmitting }, reset, resetField } = useForm({ resolver: zodResolver(registerSchema) });

    useEffect(() => {
        if (resetForm) {
                              Object.keys(errors).forEach(field => {
                resetField(field);
            });
        }   
    }, [resetForm, reset]);

    return (
        <form className="flex flex-col gap-4" onSubmit={handleSubmit(onSubmit)}>

            {/* First Name & Last Name */}
            <div className="flex flex-col sm:flex-row gap-3">
                <label className="flex flex-col gap-1 w-full">
                    <span className="font-medium text-xs">First Name</span>
                    <input className="form-input h-10 px-3 rounded-xl border" placeholder="Jane" {...register("firstName")} />
                    {errors.firstName && <p className="text-xs text-red-500 mt-0.5">{errors.firstName.message}</p>}
                </label>

                <label className="flex flex-col gap-1 w-full">
                    <span className="font-medium text-xs">Last Name</span>
                    <input className="form-input h-10 px-3 rounded-xl border" placeholder="Doe" {...register("lastName")} />
                    {errors.lastName && <p className="text-xs text-red-500 mt-0.5">{errors.lastName.message}</p>}
                </label>
            </div>

            {/* Username & Phone Number */}
            <div className="flex flex-col sm:flex-row gap-3">
                <label className="flex flex-col gap-1 w-full">
                    <span className="font-medium text-xs">Username</span>
                    <input className="form-input h-10 px-3 rounded-xl border" placeholder="bookworm99" {...register("username")} />
                    {errors.username && <p className="text-xs text-red-500 mt-0.5">{errors.username.message}</p>}
                </label>

                <label className="flex flex-col gap-1 w-full">
                    <span className="font-medium text-xs">Phone Number</span>
                    <input type="tel" className="form-input h-10 px-3 rounded-xl border" placeholder="(555) 123-4567" {...register("phoneNumber")} />
                    {errors.phoneNumber && <p className="text-xs text-red-500 mt-0.5">{errors.phoneNumber.message}</p>}
                </label>
            </div>

            {/* Email */}
            <label className="flex flex-col gap-1">
                <span className="font-medium text-xs">Email Address</span>
                <input type="email" className="form-input h-10 px-3 rounded-xl border" placeholder="user@example.com" {...register("email")} />
                {errors.email && <p className="text-xs text-red-500 mt-0.5">{errors.email.message}</p>}
            </label>



            {/* Password */}
            <PasswordInput
                label="Password"
                register={register("password")}
                error={errors.password}
                className="h-10 rounded-xl"
            />

            {/* Submit Button */}
            <button
                type="submit"
                className="h-12 rounded-full bg-primary hover:bg-primary-hover text-white text-base font-bold transition-all active:scale-[0.98]"
                disabled={isSubmitting}
            >
                {isSubmitting ? 'Creating...' : 'Create Account'}
            </button>

        </form>
    );
}

export default RegisterForm;

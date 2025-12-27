import * as z from 'zod';

export const registerSchema = z.object({
	username: z.string().min(3, 'Username must be at least 3 characters'),
	password: z.string().min(6, 'Password must be at least 6 characters'),
	email: z.email('Invalid email address'),
	phoneNumber: z.string().min(7, 'Phone is required').max(20, 'Phone is too long'),
	firstName: z.string().min(1, 'First name is required'),
	lastName: z.string().min(1, 'Last name is required'),
	address: z.string().min(1, 'Address is required'),
});
import { z } from "zod";

export const CustomerSchema = z.object({
    firstName: z.string(),
    lastName: z.string(),
    phone: z.string()
});

export const AppointmentRequestSchema = z.object({
    customer: CustomerSchema,
    date: z.string(),
    status: z.string(),
});

export const AppointmentResponseSchema = z.object({
    id: z.string(),
    customer: CustomerSchema,
    date: z.string(),
    status: z.string(),
});

export type Customer = z.infer<typeof CustomerSchema>;
export type AppointmentRequest = z.infer<typeof AppointmentRequestSchema>;
export type AppointmentResponse = z.infer<typeof AppointmentResponseSchema>;

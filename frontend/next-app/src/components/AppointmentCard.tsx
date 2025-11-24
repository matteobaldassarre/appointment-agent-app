import React, { JSX } from "react";
import { Calendar, CheckSquare, PhoneOutgoing, XSquare } from "react-feather"
import styles from "./AppointmentCard.module.css";
import {formatDate, formatPhoneNumber} from "@/utils";
import Spacer from "./Spacer";

interface IAppointmentCardProps {
    firstName: string,
    lastName: string,
    date: string,
    phone: string,
    status: string
}

const AppointmentCard: React.FC<IAppointmentCardProps> = ({
    firstName,
    lastName,
    date,
    phone,
    status
}) => {
    let statusIcon: JSX.Element | null = null;
    const strokeWidth = 1.5;

    if (status == "Scheduled")
        statusIcon = <Calendar strokeWidth={strokeWidth} />;
    else if (status == "Fulfilled")
        statusIcon = <CheckSquare strokeWidth={strokeWidth} />;
    else
        statusIcon = <XSquare strokeWidth={strokeWidth} />;

    return (
        <>
            <article className={styles.card}>
                {/* Card Title */}
                <h2>{firstName} {lastName}</h2>

                {/* Appointment Date */}
                <div className={styles.date}>
                    {formatDate(date)}
                </div>

                {/* Appointment Phone & Status */}
                <div className={styles.phone}>
                    {formatPhoneNumber(phone)} 
                    <PhoneOutgoing 
                        strokeWidth={strokeWidth}
                    />
                </div>
                <Spacer />
                <div className={styles.status} >
                    {statusIcon} {status}
                </div>
            </article>
        </>
    );
};

export default AppointmentCard;
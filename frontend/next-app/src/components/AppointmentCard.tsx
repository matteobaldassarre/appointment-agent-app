import React from "react";
import { PhoneOutgoing } from "react-feather"
import styles from "./AppointmentCard.module.css";
import {formatDate} from "@/utils";

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
    return (
        <>
            <article className={styles.card}>
                <h2>{firstName} {lastName}</h2>
                <div className={styles.appointmentDate}>{formatDate(date)}</div>
                <div className={styles.phone}>
                    {phone} <PhoneOutgoing strokeWidth={1.5} />
                </div>
                <div>Status: {status}</div>
            </article>
        </>
    );
};

export default AppointmentCard;
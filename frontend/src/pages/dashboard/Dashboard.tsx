import React from "react";
import type {AppointmentResponse} from "../../types.ts";
import AppointmentCard from "../../components/AppointmentCard/AppointmentCard.tsx";
import styles from "./Dashboard.module.css";

const Dashboard: React.FC = () => {
    const [appointments, setAppointments] = React.useState<AppointmentResponse[]>([]);
    const [loading, setLoading] = React.useState<boolean>(true);
    const [error, setError] = React.useState<string | null>(null);

    React.useEffect(() => {
        (async () => {
            try {
                const response = await fetch("http://localhost:8080/appointments"); // TODO: Remove hardcoded api endpoint
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }

                const data: AppointmentResponse[] = await response.json();
                setAppointments(data);
            }
            catch (exception) {
                setError(exception instanceof Error ? exception.message : "Unknown error");
            }
            finally {
                setLoading(false);
            }
        })();
    }, []);

    return (
        <div>
            <h1 className={styles.title}>Appointments Summary</h1>

            <div className={styles.container}>
                {loading && <p>Loading appointments...</p>}
                {error && <p>Error: {error}</p>}

                {!loading && !error && (
                    <>
                        {appointments.length === 0
                            ? <p>No appointments found.</p>
                            : (
                                <div className={styles.list}>
                                    {appointments.map(({ id, customer, date, status }) => (
                                        <AppointmentCard
                                            key={id}
                                            firstName={customer.firstName}
                                            lastName={customer.lastName}
                                            phone={customer.phone}
                                            date={date}
                                            status={status}
                                        />
                                    ))}
                                </div>
                            )
                        }
                    </>
                )}
            </div>
        </div>
    );
};

export default Dashboard;
import Link from "next/link";
import styles from "./page.module.css";

export default function Home() {
    return (
        <div className={styles.main}>
            <Link href="/agent" className={styles.link}>
                Start Voice Agent
            </Link>
            {' '}
            <Link href="/dashboard" className={styles.link}>
                Appointments Dashboard
            </Link>
        </div>
    );
}
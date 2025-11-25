import Link from "next/link";
import styles from "./page.module.css";

export default function Page() {
    return (
        <div className={styles.main}>
            <Link href="/agent" className="link">
                Start Voice Agent
            </Link>
            {' '}
            <Link href="/dashboard" className="link">
                Appointments Dashboard
            </Link>
        </div>
    );
}
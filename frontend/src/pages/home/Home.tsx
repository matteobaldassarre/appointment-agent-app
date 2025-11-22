import React from "react";
import styles from "./Home.module.css";

const Home: React.FC = () => {
    return (
        <div className={styles.home}>
            <button>Call Agent</button>
            <hr />
            <button>End Call</button>
        </div>
    )
};

export default Home;
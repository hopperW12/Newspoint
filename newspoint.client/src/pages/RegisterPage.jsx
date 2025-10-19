import React from "react";
import Navbar from "../components/Navbar";
import Register from "../components/Register";
import "../assets/styles/pages/RegisterPage.css";

const RegisterPage = () => {
  return (
    <main className="registerpage-main">
      <Navbar />
      <Register />
    </main>
  );
};

export default RegisterPage;

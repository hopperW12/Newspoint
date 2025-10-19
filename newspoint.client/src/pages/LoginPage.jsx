import React from "react";
import Navbar from "../components/Navbar";
import Login from "../components/Login";
import "../assets/styles/pages/LoginPage.css";

const LoginPage = () => {
  return (
    <main className="loginpage-main">
      <Navbar />
      <Login />
    </main>
  );
};

export default LoginPage;

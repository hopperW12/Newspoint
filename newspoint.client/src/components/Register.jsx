import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import "../assets/styles/pages/RegisterPage.css";

const Register = () => {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (password !== confirmPassword) {
      setError("Hesla se neshodují");
      return;
    }

    try {
      const res = await fetch("/api/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ firstName, lastName, email, password }),
      });

      if (!res.ok) throw new Error("Registrace se nezdařila");

      const data = await res.json();
      localStorage.setItem("jwt", data.token);
      
      setSuccess("Účet byl úspěšně vytvořen.");
      setFirstName("");
      setLastName("");
      setEmail("");
      setPassword("");
      setConfirmPassword("");
    } catch (err) {
      setError(err.message);
    }
  };

  useEffect(() => {
    if (error || success) {
      const timer = setTimeout(() => {
        setError("");
        setSuccess("");
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [error, success]);

  return (
    <div className="register-container">
      <form className="register-form" onSubmit={handleSubmit}>
        <h2 className="register-title">Registrace</h2>

        {error && <p className="error-message">{error}</p>}
        {success && <p className="success-message">{success}</p>}

        <label htmlFor="firstname">Křestní jméno</label>
        <input
          type="text"
          id="firstname"
          placeholder="Zadejte své křestní jméno"
          value={firstName}
          onChange={(e) => setFirstName(e.target.value)}
          required
        />

        <label htmlFor="lastname">Příjmení</label>
        <input
          type="text"
          id="lastname"
          placeholder="Zadejte své příjmení" 
          value={lastName}
          onChange={(e) => setLastName(e.target.value)}
          required
        />

        <label htmlFor="email">Email</label>
        <input
          type="email"
          id="email"
          placeholder="Zadejte svůj email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />

        <label htmlFor="password">Heslo</label>
        <input
          type="password"
          id="password"
          placeholder="Zadejte heslo"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        <label htmlFor="confirmPassword">Potvrzení hesla</label>
        <input
          type="password"
          id="confirmPassword"
          placeholder="Zadejte heslo znovu"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          required
        />

        <button type="submit">Zaregistrovat se</button>

        <p className="register-login-text">
          Už máte účet? <Link to="/login">Přihlaste se.</Link>
        </p>
      </form>
    </div>
  );
};

export default Register;

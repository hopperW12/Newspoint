import { useState, useEffect } from "react";
import { Link } from "react-router-dom";

const Login = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const res = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });

      if (!res.ok) throw new Error("Špatný email nebo heslo");

      const data = await res.json();
      localStorage.setItem("jwt", data.token); 
      alert("Login successful!");
    } catch (err) {
      setError(err.message);
    }
  };

  useEffect(() => {
    if (error) {
      const timer = setTimeout(() => setError(""), 5000);
      return () => clearTimeout(timer);
    }
  }, [error]);

  return (
    <div className="login-container">
      <form className="login-form" onSubmit={handleSubmit}>
        <h2 className="login-title">Přihlášení</h2>

        {error && <p className="error-message">{error}</p>}

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
          placeholder="Zadejte své heslo"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        <button type="submit">Přihlásit se</button>

        <p className="signup-text">
          Ještě nemáte účet? <Link to="/register">Zaregistrujte se.</Link>
        </p>
      </form>
    </div>
  );
};
export default Login;

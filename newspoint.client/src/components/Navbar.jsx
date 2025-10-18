import React from "react";
import { NavLink, Link } from "react-router-dom";

const Navbar = () => {
  return (
    <nav className="navbar">
      <div className="navbar-logo">
        <Link to="/">📰 Zpravodajství</Link>
      </div>
      <ul className="navbar-links">
        <li>
          <NavLink to="/" end>
            Domů
          </NavLink>
        </li>
        <li>
          <NavLink to="/login">Přihlášení</NavLink>
        </li>
        <li>
          <NavLink to="/admin">Admin</NavLink>
        </li>
      </ul>
    </nav>
  );
};

export default Navbar;

import { useContext } from "react";
import { NavLink, Link } from "react-router-dom";
import { FaUser } from "react-icons/fa";
import { IoLogOut } from "react-icons/io5";
import { AuthContext } from "../context/AuthContext";

const Navbar = () => {
  const { user, logout } = useContext(AuthContext);

  return (
    <nav className="navbar">
      <div className="navbar-logo">
        <Link to="/">📰 Zpravodajství</Link>
      </div>
      <ul className="navbar-links">
        {user ? (
          <div className="user-links">
            <NavLink to="/user">{user.unique_name}</NavLink>
            <IoLogOut
              onClick={logout}
              className="logout-icon"
              style={{ cursor: "pointer", fontSize: "20px" }}
            />
          </div>
        ) : (
          <li>
            <NavLink to="/login">
              <FaUser />
            </NavLink>
          </li>
        )}
      </ul>
    </nav>
  );
};

export default Navbar;

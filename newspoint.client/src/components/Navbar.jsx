import { useContext } from "react";
import { NavLink, Link } from "react-router-dom";
import { FaUser } from "react-icons/fa";
import { IoIosLogOut } from "react-icons/io";
import { IoLogOut } from "react-icons/io5";
import { AuthContext } from "../context/AuthContext";

const Navbar = () => {
  const { user, logout } = useContext(AuthContext);
  console.log(user);

  return (
    <nav className="navbar">
      <div className="navbar-logo">
        <Link to="/">📰 Zpravodajství</Link>
      </div>
      <ul className="navbar-links">
        {user ? (
          <div className="user-links">
            <p>{user.unique_name}</p>
            <IoLogOut
              onClick={logout}
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

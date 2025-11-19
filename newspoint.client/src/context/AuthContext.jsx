import { createContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode"; // npm i jwt-decode

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [jwt, setJwt] = useState(localStorage.getItem("jwt"));
  const [user, setUser] = useState(() => {
    const storedJwt = localStorage.getItem("jwt");
    if (storedJwt) {
      try {
        return jwtDecode(storedJwt);
      } catch {
        return null;
      }
    }
    return null;
  });

  useEffect(() => {
    if (jwt) {
      localStorage.setItem("jwt", jwt);
      try {
        setUser(jwtDecode(jwt));
      } catch {
        setUser(null);
      }
    } else {
      localStorage.removeItem("jwt");
      setUser(null);
    }
  }, [jwt]);

  const login = (newJwt) => setJwt(newJwt);
  const logout = () => setJwt(null);

  return (
    <AuthContext.Provider value={{ jwt, user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

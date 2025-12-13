import { createContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";

// Umožní přistupovat k přihlášenému uživateli odkudkoliv
export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [jwt, setJwt] = useState(localStorage.getItem("jwt"));
  const [user, setUser] = useState(() => {
    const storedJwt = localStorage.getItem("jwt");

    // Pokud je JWT uložené v prohlížeči
    if (storedJwt) {
      try {
        // Rozkódujeme token a tím získáme objekt uživatele
        return jwtDecode(storedJwt);
      } catch {
        // Pokud je token neplatný
        return null;
      }
    }

    // Pokud žádný token neexistuje - uživatel není přihlášen
    return null;
  });

  // Tento useEffect se spustí pokaždé když se změní hodnota jwt
  useEffect(() => {
    // Pokud JWT existuje (uživatel se přihlásil)
    if (jwt) {
      // Uložíme JWT do localStorage
      localStorage.setItem("jwt", jwt);

      try {
        // Rozkódujeme JWT a uložíme uživatele do state
        setUser(jwtDecode(jwt));
      } catch {
        // Pokud se token nepodaří rozkódovat
        setUser(null);
      }
    } else {
      // Pokud JWT neexistuje (uživatel se odhlásil)
      localStorage.removeItem("jwt");
      setUser(null);
    }
  }, [jwt]);

  const login = (newJwt) => setJwt(newJwt);

  const logout = () => {
    setJwt(null);
    location.reload();
  };

  return (
    <AuthContext.Provider
      value={{
        jwt,
        user,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

import { Navigate } from "react-router-dom";

const ProtectedRoute = ({ children }) => {
  const jwt = localStorage.getItem("jwt");

  if (!jwt) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

export default ProtectedRoute;

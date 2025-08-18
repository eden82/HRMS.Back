import { Navigate, Outlet } from "react-router-dom";
import { jwtDecode } from "jwt-decode";




const ProtectedRoute = ({ allowedRoles }) => {
    const token = localStorage.getItem("jwtToken");

    if (!token) return <Navigate to="/login" />;

    try {
        const decoded = jwtDecode(token);
        const role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

        if (allowedRoles && !allowedRoles.includes(role)) {
            // Role not allowed
            return <Navigate to="/" />;
        }

        return <Outlet />;
    } catch (error) {
        console.error("Invalid token:", error);
        return <Navigate to="/login" />;
    }
};

export default ProtectedRoute;
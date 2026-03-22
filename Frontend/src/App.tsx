import { Navigate, Route, Routes } from "react-router-dom"

import { DashboardLayout } from "./layouts/DashboardLayout"
import { AuthLayout } from "./layouts/AuthLayout"

import { DashboardPage } from "./pages/Dashboard"
import { LoginPage } from "./pages/Login"
import { RegisterPage } from "./pages/Register"

import { ProtectedRoute } from "./components/ProtectedRoute/ProtectedRoute"


export function App() {

    return (
        <Routes>
            <Route element={<AuthLayout />}>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
            </Route>

            <Route path="/dashboard" element={
                <ProtectedRoute>
                    <DashboardLayout />
                </ProtectedRoute>
            }>
                <Route index element={<DashboardPage />} />
            </Route>

            <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
    )

}
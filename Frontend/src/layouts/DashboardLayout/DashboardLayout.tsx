import { Outlet } from "react-router-dom"
import { TopNav } from "../../components/TopNav"


export function DashboardLayout() {

    return (
        <div className="dashboard-layout">
            <TopNav />
            <main className="dashboard-main">
                <Outlet />
            </main>
        </div>
    )

}